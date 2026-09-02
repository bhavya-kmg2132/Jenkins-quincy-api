using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;

namespace Application.Users.Commands.Teams.RemoveTeamMember
{
    public class RemoveTeamMemberValidator : AbstractValidator<RemoveTeamMemberRequest>
    {
        private readonly IUserDataAccess _dataAccess;

        public RemoveTeamMemberValidator(IUserDataAccess dataAccess)
        {
            _dataAccess = dataAccess;

            RuleFor(x => x.TeamId).Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.RemoveTeamMemberValidator_TeamId_NotEmpty);

            RuleFor(x => x.MemberId).Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.RemoveTeamMemberValidator_MemberId_NotEmpty)
                .MustAsync(IsNotOwnerOrCaptain)
                .WithMessage(Resources.ErrorMessages.RemoveTeamMemberValidator_MemberId_NotOwnerOrCaptain);
        }

        private async Task<bool> IsNotOwnerOrCaptain(RemoveTeamMemberRequest request, string memberId, CancellationToken cancellationToken)
        {
            var team = await _dataAccess.GetTeamById(request.TeamId);
            if (team == null) return true;
            return memberId != team.TeamOwnerId && memberId != team.TeamCaptainId;
        }
    }
}
