﻿using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.API.IntegrationTests.Extensions;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.EmployerAccountControllerTests
{
    [TestFixture]
    public class WhenGetLegalEntitiesWithNonExistentKey
    {
        private ApiIntegrationTester _tester;

        [SetUp]
        public void SetUp()
        {
            _tester = new ApiIntegrationTester(TestSetupIoC.CreateIoC);
        }

        [TearDown]
        public void TearDown()
        {
            _tester.Dispose();
        }

        [Test]
        public async Task ThenTheStatusShouldBeNotFound()
        {
            // Arrange
            var callRequirements =
                new CallRequirements("api/accounts/ZZZZZZ/legalentities")
                    .AllowStatusCodes(HttpStatusCode.NotFound)
                    .ExpectControllerType(typeof(LegalEntitiesController));

            // Act
            var legalEntities = await _tester.InvokeGetAsync<ResourceList>(callRequirements);

            // Assert
            Assert.IsNull(legalEntities.Data);
        }

        [Test]
        public async Task ThenTheStatusShouldBeFound()
        {
            // Arrange
            const string accountName = "ACME Fireworks";
            const string legalEntityName = "RoadRunner Pest Control";
            const string payeReference = "Acme PAYE";

            string hashedAccountId = null;
            _tester.InitialiseData<EmployerAccountsDbBuilder>(builder =>
            {
                builder
                    .EnsureUserExists(builder.BuildUserInput())
                    .EnsureAccountExists(builder.BuildEmployerAccountInput(accountName, payeReference))
                    .WithLegalEntity(builder.BuildEntityWithAgreementInput(legalEntityName));

                hashedAccountId = builder.Context.ActiveEmployerAccount.HashedAccountId;
            });

            var callRequirements =
                new CallRequirements($"api/accounts/{hashedAccountId}/legalentities")
                    .AllowStatusCodes(HttpStatusCode.OK)
                    .ExpectControllerType(typeof(LegalEntitiesController));

            // Act
            var legalEntities = await _tester.InvokeGetAsync<ResourceList>(callRequirements);

            // Assert
            Assert.IsNotNull(legalEntities.Data);
        }
    }
}