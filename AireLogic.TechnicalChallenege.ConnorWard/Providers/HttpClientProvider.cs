using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AireLogic.TechnicalChallenge.ConnorWard.Providers
{
    public class HttpClientProvider : IHttpClientProvider
    {
        private readonly HttpClient httpClient = new HttpClient();

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            var response = await httpClient.GetAsync(url);

            return response;
        }

        public void AddRequestHeader(string name, string value)
        {
            httpClient.DefaultRequestHeaders.Add(name, value);
        }

        public void SetTimeout(TimeSpan timeout)
        {
            httpClient.Timeout = timeout;
        }
    }
}