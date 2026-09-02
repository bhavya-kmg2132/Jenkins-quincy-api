using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Api.Utilities
{
    public static class Endpoint
    {
        private const string URL = "https://localhost:5001/";
        // private const string URL = "https://netaccountapi.azurewebsites.net/";
        public static async void HttpPostRequest(string serverURL, string apiEnpoint, object postObject)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverURL);
                // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Your Oauth token");
                var serializeObject = JsonSerializer.Serialize(postObject);
                var content = new StringContent(serializeObject, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiEnpoint, content);
                // var r = res;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                }
            }
        }

        public static void HttpPostRequestSync(string serverURL, string apiEnpoint, object postObject)
        {
            //  using (HttpClient client = new HttpClient())
            //  {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(serverURL);
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Your Oauth token");
            var serializeObject = JsonSerializer.Serialize(postObject);
            var content = new StringContent(serializeObject, Encoding.UTF8, "application/json");
            var res = client.PostAsync(apiEnpoint, content);
            // var r = res;

            //if (response.IsSuccessStatusCode)
            //{
            //    var responseContent = response.Content.ReadAsStringAsync().Result;
            //}
            //else
            //{
            //}
            //  }

            //  client.Dispose();

        }

        public static void HttpPutRequest(string serverURL, string apiEndpoint, object putObject, string id)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(serverURL);
                // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Your Oauth token");
                var serializeObject = JsonSerializer.Serialize(putObject);
                var content = new StringContent(serializeObject, Encoding.UTF8, "application/json");
                // HttpResponseMessage response = client.PutAsync(apiEndpoint + "/" + id, content).Result;
                client.PutAsync(apiEndpoint + "/" + id, content);


                //if (response.IsSuccessStatusCode)
                //{
                //    var responseContent = response.Content.ReadAsStringAsync().Result;
                //}
                //else
                //{
                //}
            }
        }
    }
}

