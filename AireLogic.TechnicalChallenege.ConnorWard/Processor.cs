using AireLogic.TechnicalChallenege.ConnorWard.Models;
using AireLogic.TechnicalChallenge.ConnorWard;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AireLogic.TechnicalChallenege.ConnorWard
{
    public class Processor : IProcessor
    {
        private readonly IRecordingInformationProvider recordingInformationProvider;
        private readonly ILyricsProvider lyricsProvider;
        private readonly ILyricParser lyricParser;

        public Processor(IRecordingInformationProvider recordingInformationProvider, ILyricsProvider lyricsProvider, ILyricParser lyricParser)
        {
            this.recordingInformationProvider = recordingInformationProvider ?? throw new ArgumentNullException($"{nameof(recordingInformationProvider)} cannot be null");
            this.lyricsProvider = lyricsProvider ?? throw new ArgumentNullException($"{nameof(lyricsProvider)} cannot be null");
            this.lyricParser = lyricParser ?? throw new ArgumentNullException($"{nameof(lyricParser)} cannot be null");
        }

        public async Task<IReadOnlyCollection<ArtistResultModel>> Run(IReadOnlyCollection<string> artists, Action<string> updateProgress)
        {
            if (!artists.Any())
                return new ArtistResultModel[0];

            var results = new ConcurrentDictionary<string, ArtistResultModel>();

            var artistRecordingTitleCombinations = new List<(string, string)>();

            // Build a list of artists and their record titles so that we can process all in parallel
            foreach (var artist in artists)
            {
                var recordingTitles = await recordingInformationProvider.GetRecordingTitlesByArtistName(artist);

                recordingTitles = FilterRecordingTitles(recordingTitles);

                foreach (var recordingTitle in recordingTitles)
                    artistRecordingTitleCombinations.Add((artist, recordingTitle));

                results.GetOrAdd(artist, new ArtistResultModel
                {
                    ArtistName = artist
                });
            }

            var completedCombinations = 0;
            var totalCombinations = artistRecordingTitleCombinations.Count;

            // Foreach artist / record title combination, create a task
            var tasks = artistRecordingTitleCombinations.Select(async x =>
            {
                var artist = x.Item1;
                var record = x.Item2;

                try
                {
                    var lyrics = await lyricsProvider.GetLyrics(artist, record);

                    if (!string.IsNullOrEmpty(lyrics))
                        results[artist].RecordLyrics.Add(record, lyricParser.Parse(lyrics));
                    else
                        results[artist].UnavailableRecordLyrics.Add(record);
                }
                catch (TaskCanceledException)
                {
                    results[artist].UnavailableRecordLyrics.Add(record);
                }

                Interlocked.Increment(ref completedCombinations);

                updateProgress?.Invoke($"Completed {completedCombinations} out of {totalCombinations} artist / record combinations");
            })
            .ToList();

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);

            return results.Select(x => x.Value).ToArray();
        }

        private List<string> FilterRecordingTitles(List<string> recordTitles)
        {
            return recordTitles
                .Distinct()
                .Where(x =>
                    !(x.StartsWith("[") && x.EndsWith("]")) &&
                    !x.EndsWith(")"))
                .ToList();
        }
    }
}