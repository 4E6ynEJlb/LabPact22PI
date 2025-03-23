using Newtonsoft.Json;

namespace DiscordAutoliker
{
    internal class Autoliker
    {
        private readonly string _token;
        private readonly string _channelId;
        private readonly string _userId;
        private readonly int _intervalMs;
        private string? _lastMessageId;
        public Autoliker(string token, string channelId, string userId, int intervalMs)
        {
            _token = token;
            _channelId = channelId;
            _userId = userId;
            _intervalMs = intervalMs;
        }
        internal async Task RunAsync(CancellationToken cancellationToken)
        {
            int errorsCount = 0;
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        string? messageId = await GetUserMessagesAsync(cancellationToken);
                        if (messageId != null && messageId != _lastMessageId)
                        {
                            _lastMessageId = messageId;
                            PutReactionAsync(messageId, cancellationToken);
                        }
                        await Task.Delay(_intervalMs, cancellationToken);
                    }
                    catch
                    {
                        errorsCount++;
                        if (errorsCount >= 5)
                        {
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine($"Завершено со сбоем");
            }
        }
        private async Task<string?> GetUserMessagesAsync(CancellationToken cancellationToken)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://discord.com/api/v9/channels/{_channelId}/messages?limit=50");
            request.Headers.Add("accept", "*/*");
            request.Headers.Add("accept-language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
            request.Headers.Add("authorization", _token);
            request.Headers.Add("cookie", "__dcfduid=7fc5e8b0cdc611efbe2dadf83f7a85cc; __sdcfduid=7fc5e8b1cdc611efbe2dadf83f7a85cc0c1842329a329ace9d1bd82984e22a3b07eb8e9b1e3649d57bf1ef05dd5d9f4a; __cfruid=dc8b5df98a12c7a63b66094843021a05a0d42754-1742682324; _cfuvid=69Z5DkHCG3bCpGB8KVRCw1CSRtNHHraO86IMij3qP88-1742682324122-0.0.1.1-604800000; _gcl_au=1.1.506859763.1742682325; _ga=GA1.1.518779874.1742682326; locale=ru; OptanonConsent=isIABGlobal=false&datestamp=Sun+Mar+23+2025+00%3A27%3A19+GMT%2B0200+(%D0%92%D0%BE%D1%81%D1%82%D0%BE%D1%87%D0%BD%D0%B0%D1%8F+%D0%95%D0%B2%D1%80%D0%BE%D0%BF%D0%B0%2C+%D1%81%D1%82%D0%B0%D0%BD%D0%B4%D0%B0%D1%80%D1%82%D0%BD%D0%BE%D0%B5+%D0%B2%D1%80%D0%B5%D0%BC%D1%8F)&version=6.33.0&hosts=&landingPath=NotLandingPage&groups=C0001%3A1%2CC0002%3A1%2CC0003%3A1&AwaitingReconsent=false; _ga_Q149DFWHT7=GS1.1.1742682325.1.1.1742682486.0.0.0; cf_clearance=cXNc1jbS0Jljcgb9XT_wFpUXlgXE51Oc1u6ZTyo9yzU-1742689318-1.2.1.1-e3U8FAYuNn2uq1.uwIIYyGfbYbZbBF7gPF3Aw6y_3jmPxBCp4ZbgtDk5SHkHIXGsHKtm9wDw1Z9SI_XDMtHVg3K5ZVCEFnham7xgjLsS9EfDZAd99.zm_IJ9LV_lTKZKiVgkMlnFb1oyeP8SnbdFP59NCgCAmyuxMZzg4S414Vmcf6ywrAb1IxeUVXXRGYg9b4gvVMtjE9.AbSQKzgLDkci23.SCZqSWlL9Y9F7pPpnKXDf0xWfGCC5Bt.FRWYt4Pk6arze5iuMUN8heC3BH0QZav270SKGSZKSzZsJsoRndjCsF5Sr3AKj4L_jzza9Pw9qVJqJlUAG1F1oFDS9Ixx1_jLU9ri1ClSyiAvjF64U");
            request.Headers.Add("priority", "u=1, i");
            request.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"131\", \"Chromium\";v=\"131\", \"Not_A Brand\";v=\"24\"");
            request.Headers.Add("sec-ch-ua-mobile", "?0");
            request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
            request.Headers.Add("sec-fetch-dest", "empty");
            request.Headers.Add("sec-fetch-mode", "cors");
            request.Headers.Add("sec-fetch-site", "same-origin");
            request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");
            request.Headers.Add("x-debug-options", "bugReporterEnabled");
            request.Headers.Add("x-discord-locale", "ru");
            request.Headers.Add("x-discord-timezone", "Europe/Chisinau");
            request.Headers.Add("x-super-properties", "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiQ2hyb21lIiwiZGV2aWNlIjoiIiwic3lzdGVtX2xvY2FsZSI6InJ1LVJVIiwiaGFzX2NsaWVudF9tb2RzIjpmYWxzZSwiYnJvd3Nlcl91c2VyX2FnZW50IjoiTW96aWxsYS81LjAgKFdpbmRvd3MgTlQgMTAuMDsgV2luNjQ7IHg2NCkgQXBwbGVXZWJLaXQvNTM3LjM2IChLSFRNTCwgbGlrZSBHZWNrbykgQ2hyb21lLzEzMS4wLjAuMCBTYWZhcmkvNTM3LjM2IiwiYnJvd3Nlcl92ZXJzaW9uIjoiMTMxLjAuMC4wIiwib3NfdmVyc2lvbiI6IjEwIiwicmVmZXJyZXIiOiJodHRwczovL3d3dy55b3V0dWJlLmNvbS8iLCJyZWZlcnJpbmdfZG9tYWluIjoid3d3LnlvdXR1YmUuY29tIiwicmVmZXJyZXJfY3VycmVudCI6Imh0dHBzOi8vd3d3Lmdvb2dsZS5jb20vIiwicmVmZXJyaW5nX2RvbWFpbl9jdXJyZW50Ijoid3d3Lmdvb2dsZS5jb20iLCJzZWFyY2hfZW5naW5lX2N1cnJlbnQiOiJnb29nbGUiLCJyZWxlYXNlX2NoYW5uZWwiOiJzdGFibGUiLCJjbGllbnRfYnVpbGRfbnVtYmVyIjozODA3MzAsImNsaWVudF9ldmVudF9zb3VyY2UiOm51bGx9");
            var response = await client.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            List<dynamic> messages;
            messages = JsonConvert.DeserializeObject<List<dynamic>>(await response.Content.ReadAsStringAsync(cancellationToken))!;            
            dynamic? message =  messages.Where(m=>m.author.id == _userId).FirstOrDefault();
            if(message != null)
            {
                return (string)message.id;
            }
            return null;
        }
        private async void PutReactionAsync(string messageId, CancellationToken cancellationToken)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Put, $"https://discord.com/api/v9/channels/{_channelId}/messages/{messageId}/reactions/%F0%9F%91%8D/%40me?location=Message%20Hover%20Bar&type=0");
            request.Headers.Add("accept", "*/*");
            request.Headers.Add("accept-language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
            request.Headers.Add("authorization", _token);            
            request.Headers.Add("cookie", "__dcfduid=7fc5e8b0cdc611efbe2dadf83f7a85cc; __sdcfduid=7fc5e8b1cdc611efbe2dadf83f7a85cc0c1842329a329ace9d1bd82984e22a3b07eb8e9b1e3649d57bf1ef05dd5d9f4a; __cfruid=dc8b5df98a12c7a63b66094843021a05a0d42754-1742682324; _cfuvid=69Z5DkHCG3bCpGB8KVRCw1CSRtNHHraO86IMij3qP88-1742682324122-0.0.1.1-604800000; cf_clearance=9Gxkg7INDZFgjOwbTPxjy4W5G70wQOaKyJJNZaak0e8-1742682325-1.2.1.1-_CF.zkvAWqkCb_I02HLF2AZYzmSJb50CM5KUdh9i_7wHtTMVkdK.DhE8SH2yqD9qO4PozrmlwvmsOhrAb.zFqgYJoHFGLguxnOGWYLLaGpu5nivfuzIacr3CWadfvW1Y9b18UBvIhNB0v2ZFIwbHu_exNLaCGiyZuZMWFGTk21KMPAHK.KDcdX7YIYkX1UZjhKaWgIuAltOpBidOD7h4B.UvG63H9pviZNPBLWy1ejHfX0fISbosXU8Qz7fohkYBRN0JScDWLskK9.SmVSWCc06AqboFgmZuPOyOk..dc3uCVUYlo_Q9RXvrWHsfMV8rm9MjGGI9MxhQm9ZqIm7a6jGNLiCAPxSd1cnM2MUgazs; _gcl_au=1.1.506859763.1742682325; _ga=GA1.1.518779874.1742682326; locale=ru; OptanonConsent=isIABGlobal=false&datestamp=Sun+Mar+23+2025+00%3A27%3A19+GMT%2B0200+(%D0%92%D0%BE%D1%81%D1%82%D0%BE%D1%87%D0%BD%D0%B0%D1%8F+%D0%95%D0%B2%D1%80%D0%BE%D0%BF%D0%B0%2C+%D1%81%D1%82%D0%B0%D0%BD%D0%B4%D0%B0%D1%80%D1%82%D0%BD%D0%BE%D0%B5+%D0%B2%D1%80%D0%B5%D0%BC%D1%8F)&version=6.33.0&hosts=&landingPath=NotLandingPage&groups=C0001%3A1%2CC0002%3A1%2CC0003%3A1&AwaitingReconsent=false; _ga_Q149DFWHT7=GS1.1.1742682325.1.1.1742682486.0.0.0");
            request.Headers.Add("origin", "https://discord.com");
            request.Headers.Add("priority", "u=1, i");
            request.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"131\", \"Chromium\";v=\"131\", \"Not_A Brand\";v=\"24\"");
            request.Headers.Add("sec-ch-ua-mobile", "?0");
            request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
            request.Headers.Add("sec-fetch-dest", "empty");
            request.Headers.Add("sec-fetch-mode", "cors");
            request.Headers.Add("sec-fetch-site", "same-origin");
            request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");
            request.Headers.Add("x-debug-options", "bugReporterEnabled");
            request.Headers.Add("x-discord-locale", "ru");
            request.Headers.Add("x-discord-timezone", "Europe/Chisinau");
            request.Headers.Add("x-super-properties", "eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiQ2hyb21lIiwiZGV2aWNlIjoiIiwic3lzdGVtX2xvY2FsZSI6InJ1LVJVIiwiaGFzX2NsaWVudF9tb2RzIjpmYWxzZSwiYnJvd3Nlcl91c2VyX2FnZW50IjoiTW96aWxsYS81LjAgKFdpbmRvd3MgTlQgMTAuMDsgV2luNjQ7IHg2NCkgQXBwbGVXZWJLaXQvNTM3LjM2IChLSFRNTCwgbGlrZSBHZWNrbykgQ2hyb21lLzEzMS4wLjAuMCBTYWZhcmkvNTM3LjM2IiwiYnJvd3Nlcl92ZXJzaW9uIjoiMTMxLjAuMC4wIiwib3NfdmVyc2lvbiI6IjEwIiwicmVmZXJyZXIiOiJodHRwczovL3d3dy55b3V0dWJlLmNvbS8iLCJyZWZlcnJpbmdfZG9tYWluIjoid3d3LnlvdXR1YmUuY29tIiwicmVmZXJyZXJfY3VycmVudCI6Imh0dHBzOi8vd3d3Lmdvb2dsZS5jb20vIiwicmVmZXJyaW5nX2RvbWFpbl9jdXJyZW50Ijoid3d3Lmdvb2dsZS5jb20iLCJzZWFyY2hfZW5naW5lX2N1cnJlbnQiOiJnb29nbGUiLCJyZWxlYXNlX2NoYW5uZWwiOiJzdGFibGUiLCJjbGllbnRfYnVpbGRfbnVtYmVyIjozODA3MzAsImNsaWVudF9ldmVudF9zb3VyY2UiOm51bGx9");
            var response = await client.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
