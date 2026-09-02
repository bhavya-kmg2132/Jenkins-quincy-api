using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent @event);
    }

}
