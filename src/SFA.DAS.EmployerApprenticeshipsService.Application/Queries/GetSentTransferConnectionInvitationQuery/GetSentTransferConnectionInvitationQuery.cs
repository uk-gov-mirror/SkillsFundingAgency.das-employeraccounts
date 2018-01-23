using System.ComponentModel.DataAnnotations;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionInvitationQuery
{
    public class GetSentTransferConnectionInvitationQuery : IAsyncRequest<GetSentTransferConnectionInvitationResponse>
    {
        [Required]
        public long? TransferConnectionInvitationId { get; set; }
    }
}