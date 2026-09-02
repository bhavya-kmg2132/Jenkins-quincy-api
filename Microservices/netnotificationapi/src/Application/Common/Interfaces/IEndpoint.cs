using System.Net.Http;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IEndpoint
    {
        Task<string> HttpGetRequestAsync(string serverURL, string apiEndpoint, string userName_userId_userOid);
        string HttpGetRequestSync(string serverURL, string apiEndpoint);
        Task<HttpResponseMessage> HttpPostRequest(string serverURL, string apiEnpoint, object postObject);
        Task<HttpResponseMessage> HttpBatchEmailPostRequest(string serverURL, string apiEndpoint, object postObject);
    }
}
