using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AireLogic.TechnicalChallenge.ConnorWard
{
    public interface IHttpClientProvider
    {
        Task<HttpResponseMessage> GetAsync(string url);
        
        void AddRequestHeader(string name, string value);

        void SetTimeout(TimeSpan timeout);
    }
}