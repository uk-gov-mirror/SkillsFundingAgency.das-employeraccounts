﻿using System;
using System.Linq;
using System.Net;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUserAccounts;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.DbCleanup;
using SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.DependencyResolution;
using SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.Steps.CommonSteps;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.Messaging;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.Steps.InviteMember
{
    [Binding, Explicit]
    public class InviteMemberSteps
    {
        private IContainer _container;
        private string _externalUserId;
        private long _accountId;
        private Mock<IMessagePublisher> _messagePublisher;
        private ICleanDatabase _cleanDownDb;
        private Mock<IOwinWrapper> _owinWrapper;


        [BeforeScenario]
        public void Arrange()
        {
            _messagePublisher = new Mock<IMessagePublisher>();
            _owinWrapper = new Mock<IOwinWrapper>();

            _container = IoC.CreateContainer(_messagePublisher, _owinWrapper);

            _cleanDownDb = _container.GetInstance<ICleanDatabase>();
            _cleanDownDb.Execute().Wait();
        }

        [AfterScenario]
        public void TearDown()
        {
            _cleanDownDb.Execute().Wait();

            _container.Dispose();
        }
        
        [Given(@"I am an account owner")]
        public void GivenIAmAnAccountOwner()
        {
            var signInUserModel = new SignInUserModel
            {
                UserId = Guid.NewGuid().ToString(),
                Email = "accountowner@test.com",
                FirstName = "Test",
                LastName = "Tester"
            };
            UserCreationSteps.UpsertUser(signInUserModel, _container.GetInstance<IMediator>());

            var user = UserCreationSteps.GetExistingUserAccount(_container.GetInstance<HomeOrchestrator>());
            _externalUserId = user.UserId;

            
            
            AccountCreationSteps.CreateDasAccount(user, _container.GetInstance<EmployerAccountOrchestrator>());
        }

        [When(@"I invite a team member with email address ""(.*)"" and name ""(.*)""")]
        public void WhenIInviteATeamMemberWithEmailAddressAndName(string email, string name)
        {
            SetAccountIdForUser();

            CreateInvitationForGivenEmailAndName(email, name);
        }

        [Then(@"A user invite is ""(.*)""")]
        public void ThenAUserInviteIsWithPendingStatus(string createdStatus)
        {
            
            var orcehstrator = _container.GetInstance<EmployerTeamOrchestrator>();
            var teamMembers = orcehstrator.GetTeamMembers(_accountId, _externalUserId).Result;

            if (createdStatus.ToLower() == "created")
            {
                Assert.AreEqual(2,teamMembers.Data.TeamMembers.Count);
                //Check to make sure an email has been sent
                _messagePublisher.Verify(x=>x.PublishAsync(It.IsAny<SendNotificationQueueMessage>()), Times.Once);

            }
            else
            {
                Assert.AreEqual(1, teamMembers.Data.TeamMembers.Count);
                //Check to make sure an email has not been sent
                _messagePublisher.Verify(x => x.PublishAsync(It.IsAny<SendNotificationQueueMessage>()), Times.Never);
            }
        }

        [Then(@"I ""(.*)"" view team members")]
        public void ThenIViewTeamMembers(string canView)
        {
            var userId = ScenarioContext.Current["ExternalUserId"].ToString();
            var orcehstrator = _container.GetInstance<EmployerTeamOrchestrator>();
            var teamMembers = orcehstrator.GetTeamMembers(_accountId, userId).Result;
            
            if (canView.ToLower().Equals("can"))
            {
                Assert.AreEqual(HttpStatusCode.OK, teamMembers.Status);
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, teamMembers.Status);
            }
        }


        private void CreateInvitationForGivenEmailAndName(string email, string name)
        {
            var orchestrator = _container.GetInstance<InvitationOrchestrator>();
            orchestrator.CreateInvitation(new InviteTeamMemberViewModel
            {
                AccountId = _accountId,
                Email = email,
                Name = name,
                Role = Role.Transactor
            }, _externalUserId).Wait();
        }

        private void SetAccountIdForUser()
        {
            var mediator = _container.GetInstance<IMediator>();
            var getUserAccountsQueryResponse = mediator.SendAsync(new GetUserAccountsQuery {UserId = _externalUserId}).Result;

            _accountId = getUserAccountsQueryResponse.Accounts.AccountList.FirstOrDefault().Id;
        }
        
    }
}
