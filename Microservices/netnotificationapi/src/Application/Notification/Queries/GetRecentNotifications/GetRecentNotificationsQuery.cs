using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Notification.Queries;

namespace Application.EmailNotification.Queries.GetRecentNotifications
{
    public class GetRecentNotificationsQuery : IRequest<InSystemNotificationVm>
    {
        public string userId { get; set; }
    }


    /// <summary>
    /// For Creating handler for the above request , created ReceiveEmailByIdQueryHandler class
    ///that implements the IRequestHandler interface as shown below.
    /// </summary>
    public class ReceiveEmailByIdQueryHandler : IRequestHandler<GetRecentNotificationsQuery, InSystemNotificationVm>
    {
        private readonly IEmailNotificationService _EmailNotificationService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Instantiates RecieveNotificationEmailQueryHandler class
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="dataAccess"></param>
        public ReceiveEmailByIdQueryHandler(IMapper mapper, IEmailNotificationService dataAccess)
        {
            _EmailNotificationService = dataAccess;
            _mapper = mapper;
        }

        /// <summary>
        /// Handler will recieve request ,process it and will return the response. 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>mapped values</returns>
        public async Task<InSystemNotificationVm> Handle(GetRecentNotificationsQuery query, CancellationToken cancellationToken)
        {
            var result = await _EmailNotificationService.GetRecentNotifications(query.userId);

            var recentNotifications = new InSystemNotificationVm
            {
                RecentNotifications = _mapper.Map<List<InSystemNotificationDto>>(result.Item2),
                UnreadCount = result.Item1
            };
            // Return the mapped EmailNotification entity
            return recentNotifications;
        }
    }
}
