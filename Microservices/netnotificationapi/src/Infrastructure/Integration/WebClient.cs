
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using RestSharp;

namespace Infrastructure.Integration
{
    public class WebClient : IWebClient
    {
        /// <summary>
        /// Send request and get the response 
        /// </summary>
        public async Task<RestResponse> RequestAsync(string url, RestSharp.Method reqMethod, string accessToken, object toBeJson = null)
        {
            var client = new RestClient(url);
            var request = new RestRequest();
            request.Method = reqMethod;

            //Set Authorization Header with access token
            SetAuthorizationHeader(request, accessToken);

            if (toBeJson != null)
            {
                string json = JsonSerializer.Serialize(toBeJson);
                request.AddJsonBody(json);
            }
            var res = await client.ExecuteAsync(request);
            return res; //await client.ExecuteAsync(request);
        }

        /// <summary>
        /// Sends http request and parses the response to the expected type
        /// </summary>
        public async Task<T> RequestAsync<T>(string url, RestSharp.Method reqMethod, string accessToken, object toBeJson = null)
        {
            try
            {
                RestResponse response = await RequestAsync(url, reqMethod, accessToken, toBeJson);
                return JsonSerializer.Deserialize<T>(response.Content);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///  Set authorization header with token
        /// </summary>
        private void SetAuthorizationHeader(RestRequest request, string accessToken)
        {
            if (accessToken != null)
            {
                request.AddHeader("authorization", string.Format("Bearer {0}", accessToken));
                request.AddHeader("cache-control", "no-cache");
            }
        }

        /// <summary>
        /// Request to get data  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<T> RequestAsync<T>(string url, RestRequest request)
        {
            try
            {
                var client = new RestClient(url);
                RestResponse response = await client.ExecuteAsync(request);
                return JsonSerializer.Deserialize<T>(response.Content);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<RestResponse> RequestAsync(string actonUrl, string apiName, RestSharp.Method reqMethod, string accessToken, object requestJson = null)
        {
            // POST
            RestRequest Request = new RestRequest();
            RestClient Client = new RestClient();
            Request.Method = Method.Post;
            Request.Resource = actonUrl + apiName;
            Request.RequestFormat = DataFormat.Json;
            Request.AddHeader("Authorization", "Bearer " + accessToken);
            Request.AddJsonBody(requestJson);

            var response = Client.Execute(Request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicationException("Error:" + response.ErrorMessage);
            }
            await Task.CompletedTask;
            return response;
        }

        public async Task<RestResponse> GetRequestAsync(string actonUrl, string apiName, RestSharp.Method reqMethod, string accessToken, object requestJson = null)
        {
            RestRequest request = new RestRequest();
            RestClient client = new RestClient();
            request.Method = reqMethod;
            request.Resource = actonUrl + apiName;
            request.AddHeader("Authorization", "Bearer " + accessToken);
            request.AddHeader("Accept", "text/html, application/xhtml+xml, application/xml;q=0.9, image/webp, */*;q=0.8");
            var response = await client.ExecuteAsync(request);

            //foreach (var i in response.Content.Split('}'))
            //{
            //    foreach (var j in i.Split(','))
            //    { 
            //        var ttt = j.Trim(); 
            //    }
            //} 

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicationException("Error:" + response.ErrorMessage);
            }
            return response;
        }
    }
}
