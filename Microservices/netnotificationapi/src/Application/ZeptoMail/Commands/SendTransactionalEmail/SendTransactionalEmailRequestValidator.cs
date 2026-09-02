using Application.ZeptoMail.Commands.SendTransactionalEmail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Notification.Commands.SendTransactionalEmail
{
    public class SendTransactionalEmailRequestValidator : AbstractValidator<SendTransactionalEmailRequest>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public SendTransactionalEmailRequestValidator(IConfiguration configuration, ILogger logger)
        {
            this._configuration = configuration;
            this._logger = logger;

            //RuleFor(v => v.email_to)
            //  .NotEmpty().WithMessage(Resources.ErrorMessages.SendEmailRequestValidator_email_to_Required);
            ////.MaximumLength(50).WithMessage(Resources.ErrorMessages.CreateClientRequestValidator_DisplayName_Length);

            //RuleFor(v => v.notification_type)
            // .NotEmpty().WithMessage(Resources.ErrorMessages.SendEmailRequestValidator_notification_type_Required);

            //RuleFor(v => v.email_subject)
            //.NotEmpty().WithMessage(Resources.ErrorMessages.SendEmailRequestValidator_email_subject_Required);
        }
    }
}
