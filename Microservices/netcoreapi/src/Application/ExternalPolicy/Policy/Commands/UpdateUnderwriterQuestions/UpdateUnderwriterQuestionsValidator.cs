namespace Application.ExternalPolicy.Policy.Commands.UpdateUnderwriterQuestions
{
    public class UpdateUnderwriterQuestionsValidator : AbstractValidator<UpdateUnderwriterQuestionsRequest>
    {
        public UpdateUnderwriterQuestionsValidator()
        {
            RuleFor(x => x.PolicyNumber)
                .NotEmpty()
                .WithMessage(Resources.ErrorMessages.UpdateUnderwriterQuestionsValidator_PolicyNumber_NotEmpty);
        }
    }
}
