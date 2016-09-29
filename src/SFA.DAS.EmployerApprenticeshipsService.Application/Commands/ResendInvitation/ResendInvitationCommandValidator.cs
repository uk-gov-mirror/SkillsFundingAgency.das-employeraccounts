﻿using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ResendInvitation
{
    public class ResendInvitationCommandValidator : IValidator<ResendInvitationCommand>
    {
        public ValidationResult Validate(ResendInvitationCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrWhiteSpace(item.Email))
                validationResult.AddError("Email", "No Email supplied");

            if (string.IsNullOrEmpty(item.HashedId))
                validationResult.AddError("HashedId", "No HashedId supplied");

            if (string.IsNullOrWhiteSpace(item.ExternalUserId))
                validationResult.AddError("ExternalUserId", "No ExternalUserId supplied");

            return validationResult;
        }

        public Task<ValidationResult> ValidateAsync(ResendInvitationCommand item)
        {
            throw new System.NotImplementedException();
        }
    }
}