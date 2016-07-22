﻿using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateInvitation
{
    public class CreateInvitationCommand : IAsyncRequest
    {
        public long AccountId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Role RoleId { get; set; }
    }
}