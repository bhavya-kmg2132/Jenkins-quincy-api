using System.Threading.Tasks;
using RestSharp;

namespace Application.Common.Interfaces
{
    public interface IWebClient
    {
        Task<T> RequestAsync<T>(string url, RestSharp.Method reqMethod, string accessToken, object toBeJson = null);
        Task<RestResponse> RequestAsync(string url, RestSharp.Method reqMethod, string accessToken, object toBeJson = null);
        Task<T> RequestAsync<T>(string url, RestRequest request);
        Task<RestResponse> RequestAsync(string actonUrl, string apiName, RestSharp.Method reqMethod, string accessToken, object requestJson = null);
        Task<RestResponse> GetRequestAsync(string actonUrl, string apiName, RestSharp.Method reqMethod, string accessToken, object requestJson = null);

    }
}
