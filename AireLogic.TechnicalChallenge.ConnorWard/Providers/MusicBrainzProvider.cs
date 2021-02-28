using AireLogic.TechnicalChallenege.ConnorWard.Models.MusicBrainzModels;
using AireLogic.TechnicalChallenge.ConnorWard;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AireLogic.TechnicalChallenege.ConnorWard.Providers
{
    public class MusicBrainzProvider : IRecordingInformationProvider
    {
        private readonly IHttpClientProvider httpClientProvider;

        public MusicBrainzProvider(IHttpClientProvider httpClientProvider)
        {
            this.httpClientProvider = httpClientProvider;

            httpClientProvider.AddRequestHeader("User-Agent", "AireLogic.TechnicalChallenge");
        }

        public async Task<List<string>> GetRecordingTitlesByArtistName(string artistName)
        {
            if (string.IsNullOrEmpty(artistName))
                throw new ArgumentException($"{nameof(artistName)} cannot be null or empty");

            var artistId = await GetArtistId(artistName);

            var songNames = await GetRecordingTitlesByArtistId(artistId);

            return songNames;
        }

        private async Task<Guid> GetArtistId(string artistName)
        {
            var urlEncodedArtistName = HttpUtility.UrlEncode(artistName);

            // No pagination added here 
            var response = await httpClientProvider.GetAsync($"https://musicbrainz.org/ws/2/artist?query={urlEncodedArtistName}&fmt=json&limit=100");

            var responseString = await response.Content.ReadAsStringAsync();

            var responseModel = JsonSerializer.Deserialize<GetArtistsResponseModel>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!responseModel.Artists?.Any() ?? true)
                throw new InvalidOperationException($"Could not find any artist matching name: {artistName}");

            var artists = responseModel.Artists;

            if (artists.Count > 1)
            {
                artists = artists.OrderByDescending(x => x.Score).ToList();

                var maxScore = artists.Select(y => y.Score).Max();
                var ambiguousArtists = artists.Where(x => x.Score == maxScore).ToList();

                if (ambiguousArtists.Count() > 1)
                {
                    var message = new StringBuilder();
                    message.AppendLine("Could not determine exact artist name match. Best matches are:");

                    foreach (var artist in ambiguousArtists)
                        message.AppendLine(artist.Name);

                    throw new InvalidOperationException(message.ToString());
                }
            }

            return artists.First().Id;
        }

        private async Task<List<string>> GetRecordingTitlesByArtistId(Guid artistId)
        {
            const int NumberOfRecordsToReturn = 100;

            if (artistId == Guid.Empty || artistId == null)
                throw new ArgumentException($"{nameof(artistId)} cannot be null or empty");

            var responseModel = await GetRecordingTitles(artistId, NumberOfRecordsToReturn, 0);

            if (!responseModel.Recordings.Any())
                return new List<string>();

            var recordingTitles = responseModel.Recordings
                .Where(x => x.Length.HasValue)
                .Select(x => x.Title)
                .ToList();

            if (responseModel.RecordingCount > NumberOfRecordsToReturn)
            {
                var numberOfRequests = (responseModel.RecordingCount / NumberOfRecordsToReturn);

                var tasks = new List<Task>();

                var results = new ConcurrentBag<string>();

                for (var i = 1; i <= numberOfRequests; i++)
                {
                    var task = await Task.Factory.StartNew(async () =>
                    {
                        var responseModel = await GetRecordingTitles(artistId, NumberOfRecordsToReturn, i * NumberOfRecordsToReturn);

                        var b = responseModel.Recordings?
                            .Where(x => x.Length.HasValue)
                            .Select(x => x.Title) ?? new List<string>();

                        foreach (var a in b)
                            results.Add(a);
                    });

                    tasks.Add(task);

                    // MusicBrainz easily becomes overwhelmed, so throttle to 5 requests every 2 seconds
                    if (i % 5 == 0)
                        await Task.Delay(2000);
                }

                await Task.WhenAll(tasks);

                recordingTitles.AddRange(results.ToList());
            }

            return recordingTitles;
        }

        private async Task<GetRecordingsResponseModel> GetRecordingTitles(Guid artistId, int limit, int offset)
        {
            var url = $"https://musicbrainz.org/ws/2/recording?artist={artistId}&fmt=json&limit={limit}&offset={offset}";

            var response = await httpClientProvider.GetAsync(url);

            while (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                // Wait 1 second and try again
                await Task.Delay(1000);

                response = await httpClientProvider.GetAsync(url);
            }

            var responseString = await response.Content.ReadAsStringAsync();

            var responseModel = JsonSerializer.Deserialize<GetRecordingsResponseModel>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return responseModel;
        }
    }
}