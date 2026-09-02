using System;
using System.IO;
using System.Text.Json;

namespace Application.IntegrationTests.Utils
{
    public class JsonLoader
    {
        private static readonly JsonSerializerOptions _jsonOpts = new JsonSerializerOptions
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true
        };

        public static T LoadFromFile<T>(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string json = reader.ReadToEnd();
                T result = JsonSerializer.Deserialize<T>(json, _jsonOpts);
                return result;
            }
        }

        public static T Deserialize<T>(object jsonObject)
        {
            return JsonSerializer.Deserialize<T>(Convert.ToString(jsonObject), _jsonOpts);
        }

    }
}
