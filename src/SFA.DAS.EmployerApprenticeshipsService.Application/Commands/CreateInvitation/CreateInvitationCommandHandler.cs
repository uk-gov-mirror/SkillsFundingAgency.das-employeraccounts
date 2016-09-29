﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Notification;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateInvitation
{
    public class CreateInvitationCommandHandler : AsyncRequestHandler<CreateInvitationCommand>
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IMediator _mediator;
        private readonly EmployerApprenticeshipsServiceConfiguration _employerApprenticeshipsServiceConfiguration;
        private readonly IValidator<CreateInvitationCommand> _validator;

        public CreateInvitationCommandHandler(IInvitationRepository invitationRepository, IMembershipRepository membershipRepository, IMediator mediator, EmployerApprenticeshipsServiceConfiguration employerApprenticeshipsServiceConfiguration, IValidator<CreateInvitationCommand> validator)
        {
            if (invitationRepository == null)
                throw new ArgumentNullException(nameof(invitationRepository));
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            _invitationRepository = invitationRepository;
            _membershipRepository = membershipRepository;
            _mediator = mediator;
            _employerApprenticeshipsServiceConfiguration = employerApprenticeshipsServiceConfiguration;
            _validator = validator;
        }

        protected override async Task HandleCore(CreateInvitationCommand message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            if(validationResult.IsUnauthorized)
                throw new UnauthorizedAccessException();

            var caller = await _membershipRepository.GetCaller(message.HashedId, message.ExternalUserId);

            ////Verify the email is not used by an existing invitation for the account
            var existingInvitation = await _invitationRepository.Get(caller.AccountId, message.Email);

            if (existingInvitation != null && existingInvitation.Status != InvitationStatus.Deleted && existingInvitation.Status != InvitationStatus.Accepted)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Invitation", "There is already an Invitation for this email" } });

            if (existingInvitation == null)
            {
                await _invitationRepository.Create(new Invitation
                {
                    AccountId = caller.AccountId,
                    Email = message.Email,
                    Name = message.Name,
                    RoleId = message.RoleId,
                    Status = InvitationStatus.Pending,
                    ExpiryDate = DateTimeProvider.Current.UtcNow.Date.AddDays(8)
                });
            }
            else
            {
                existingInvitation.Name = message.Name;
                existingInvitation.RoleId = message.RoleId;
                existingInvitation.Status = InvitationStatus.Pending;
                existingInvitation.ExpiryDate = DateTimeProvider.Current.UtcNow.Date.AddDays(8);

                await _invitationRepository.Resend(existingInvitation);
            }

            await _mediator.SendAsync(new SendNotificationCommand
            {
                UserId = caller.UserId,
                Data = new EmailContent
                {
                    RecipientsAddress = message.Email,
                    ReplyToAddress = "noreply@sfa.gov.uk",
                    Data = new Dictionary<string, string> { { "InviteeName",message.Name}, {"ReturnUrl", _employerApprenticeshipsServiceConfiguration.DashboardUrl } }
                },
                DateTime = DateTime.UtcNow,
                MessageFormat = MessageFormat.Email,
                ForceFormat = true,
                TemplatedId = ""
            });
        }
    }
}