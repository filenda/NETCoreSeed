using NETCoreSeed.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NETCoreSeed.Shared.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        public async Task<bool> SendByPlayerAsync(string[] PlayerIds, string Message)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Runtime.OneSignalAPIKey);

                var url = Runtime.OneSignalAPIUrl;

                using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    var obj = new
                    {
                        app_id = Runtime.OneSignalAppId,
                        contents = new { en = Message, pt = Message },
                        include_player_ids = PlayerIds.Where(u => !string.IsNullOrEmpty(u)).Select(u => u.ToString()).ToArray()
                    };

                    var body = JsonConvert.SerializeObject(obj);

                    var content = new StringContent(body, UTF8Encoding.UTF8, "application/json");

                    // TODO : Optional setup for PUT and POST methods, should be removed for GET method.
                    httpRequestMessage.Content = content;

                    // Sending Request
                    using (var httpResponse = await httpClient.SendAsync(httpRequestMessage))
                    {
                        return httpResponse.IsSuccessStatusCode;
                    }
                }
            }
        }
    }
}