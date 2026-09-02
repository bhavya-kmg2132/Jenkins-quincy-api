using System;
using System.Threading;
using System.Threading.Tasks;
using Application.AcmeProduct.Commands.UpdateAcmeProduct;
using Application.Common.Interfaces;

namespace Application.Acme.Commands.Update
{
    public class UpdateAcmeProductValidator : AbstractValidator<UpdateAcmeProductRequest>
    {
        private readonly IAcmeDataAccess _acmeDataAccess;

        public UpdateAcmeProductValidator(IAcmeDataAccess acmeDataAccess)
        {
            _acmeDataAccess = acmeDataAccess;

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.UpdateAcmeProductValidator_Name_NotEmpty)
                // Only run NotEmpty / uniqueness on create (Id empty) or when client supplied Name on update (Name != null)
                .When(x => string.IsNullOrWhiteSpace(x.Id) || x.Name != null)
                .MustAsync((request, name, cancellationToken) => IsNameUnique(request, cancellationToken))
                .WithMessage(Resources.ErrorMessages.UpdateAcmeProductValidator_Name_Unique)
                .When(x => string.IsNullOrWhiteSpace(x.Id) || x.Name != null);
        }

        public async Task<bool> IsNameUnique(UpdateAcmeProductRequest command, CancellationToken cancellationToken)
        {
            // If update and client didn't supply Name (UI disabled), skip uniqueness check.
            if (!string.IsNullOrWhiteSpace(command?.Id) && command.Name == null)
                return true;

            if (string.IsNullOrWhiteSpace(command?.Name))
                return true;

            var normalizedName = command.Name.Trim();

            // If update: allow if the name wasn't changed.
            if (!string.IsNullOrWhiteSpace(command.Id))
            {
                var existing = await _acmeDataAccess.GetAcmeProductById(command.Id);
                if (existing != null &&
                    string.Equals(existing.Name?.Trim(), normalizedName, StringComparison.OrdinalIgnoreCase))
                {
                    // Name unchanged — it's allowed.
                    return true;
                }
            }

            // Otherwise check if any product already uses this name.
            var exists = await _acmeDataAccess.FindAcmeProductByName(normalizedName);

            // valid only when it does NOT exist (exists == false)
            return !exists;
        }
    }
}