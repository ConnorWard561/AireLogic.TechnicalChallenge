using AireLogic.TechnicalChallenge.ConnorWard;
using AireLogic.TechnicalChallenge.ConnorWard.Models.LyricsOvhModels;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AireLogic.TechnicalChallenege.ConnorWard.Providers
{
    public class LyricsOvhProvider : ILyricsProvider
    {
        private readonly IHttpClientProvider httpClientProvider;

        public LyricsOvhProvider(IHttpClientProvider httpClientProvider)
        {
            this.httpClientProvider = httpClientProvider;

            httpClientProvider.SetTimeout(TimeSpan.FromSeconds(60));
        }

        public async Task<string> GetLyrics(string artistName, string recordingTitle)
        {
            if (string.IsNullOrEmpty(artistName))
                throw new ArgumentException($"{nameof(artistName)} cannot be null or empty");

            if (string.IsNullOrEmpty(recordingTitle))
                throw new ArgumentException($"{nameof(recordingTitle)} cannot be null or empty");

            var urlEncodedArtistName = HttpUtility.UrlEncode(artistName);
            var urlEncodedRecordingTitle = HttpUtility.UrlEncode(recordingTitle);

            var response = await GetLyricsWithRetry(urlEncodedArtistName, urlEncodedRecordingTitle);

            if (response == null || !response.IsSuccessStatusCode)
                return string.Empty;

            var responseString = await response.Content.ReadAsStringAsync();

            var responseModel = JsonSerializer.Deserialize<GetLyricsResponseModel>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (string.IsNullOrEmpty(responseModel.Lyrics))
                throw new InvalidOperationException($"Could not find any lyrics for artist '{artistName}' and recording title '{recordingTitle}'");

            return responseModel.Lyrics;
        }

        private async Task<HttpResponseMessage> GetLyricsWithRetry(string urlEncodedArtistName, string urlEncodedRecordingTitle)
        {
            var retryCount = 0;

            while (retryCount < 3)
            {
                try
                {
                    var response = await httpClientProvider.GetAsync($"https://api.lyrics.ovh/v1/{urlEncodedArtistName}/{urlEncodedRecordingTitle}");

                    return response;
                }
                catch
                { }

                retryCount++;
                await Task.Delay(2000);
            }

            return null;
        }
    }
}