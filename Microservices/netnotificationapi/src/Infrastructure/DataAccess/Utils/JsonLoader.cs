using System;
using System.IO;
using System.Text.Json;

namespace Infrastructure.DataAccess.Utils
{
    public class JsonLoader
    {
        public static T LoadFromFile<T>(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string json = reader.ReadToEnd();
                T result = JsonSerializer.Deserialize<T>(json);
                return result;
            }
        }

        public static T Deserialize<T>(object jsonObject)
        {
            return JsonSerializer.Deserialize<T>(Convert.ToString(jsonObject));
        }

    }
}
