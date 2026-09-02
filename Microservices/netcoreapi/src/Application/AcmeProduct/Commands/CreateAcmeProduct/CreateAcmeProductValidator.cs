using Application.AcmeProduct.Commands.CreateAcmeProduct;
using Application.Common.Interfaces;

namespace Application.Acme.Commands.Create
{
    public class CreateAcmeProductValidator : AbstractValidator<CreateAcmeProductRequest>
    {
        /// <summary>
        /// Validates Acme create request
        /// </summary>
        public CreateAcmeProductValidator(IAcmeDataAccess AcmeDataAcces)
        {
            //1)Name
            //It is mandatory.
            RuleFor(x => x.Name)
              .NotEmpty()
              .WithMessage(Resources.ErrorMessages.CreateAcmeProductValidator_Name_NotEmpty);
            //.MaximumLength(150);
            //.WithMessage(Resources.ErrorMessages.AcmeCreateRequestValidator_FirstName_Length);
        }
    }
}





