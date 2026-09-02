namespace Application.CronJobRule.Commands.InsertCronJobRule
{
    public class InsertCronJobRuleValidator : AbstractValidator<InsertCronJobRuleCommand>
    {
        /// <summary>
        /// Validates Acme create request
        /// </summary>
        public InsertCronJobRuleValidator()
        {
            RuleFor(x => x.NotificationName)
               .NotEmpty()
               .WithMessage(Resources.ErrorMessages.InsertCronJobRuleValidator_NotificationName_NotEmpty);
        }
    }
}





