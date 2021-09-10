using Newtonsoft.Json;
using System;
using System.Net.Http;

namespace Ams.Helper
{
    public static class HttpRequestHelper
    {
        public static T HttpRequest<T>(string url)
        {
            var result = string.Empty;
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(60);
                var response = client.GetAsync(url).Result;
                var responseContent = response.Content;
                result = responseContent.ReadAsStringAsync().Result;
            }

            return JsonConvert.DeserializeObject<T>(result);
        }
    }
}
