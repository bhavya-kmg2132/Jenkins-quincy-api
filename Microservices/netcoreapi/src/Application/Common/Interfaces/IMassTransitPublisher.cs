using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IMassTransitPublisher
    {
        Task PublishEventAsync(object data, string type);
    }
}
