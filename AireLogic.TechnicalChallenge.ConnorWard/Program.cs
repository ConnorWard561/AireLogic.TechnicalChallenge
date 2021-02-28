using AireLogic.TechnicalChallenege.ConnorWard.Providers;
using AireLogic.TechnicalChallenge.ConnorWard;
using AireLogic.TechnicalChallenge.ConnorWard.Providers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AireLogic.TechnicalChallenege.ConnorWard
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                var commandLineParser = new CommandLineParser();
                var artists = commandLineParser.Parse(args);

                // Ideally these concrete instantiations would be taken care of via dependency injection.
                var processor = new Processor(new MusicBrainzProvider(new HttpClientProvider()), new LyricsOvhProvider(new HttpClientProvider()), new LyricParser());
                var results = await processor.Run(artists, (message) =>
                {
                    Console.Clear();
                    Console.WriteLine(message);
                    Console.WriteLine($"Running duration: {stopwatch.ElapsedMilliseconds}ms");
                });

                stopwatch.Stop();

                WriteResultsToConsole(results);

                Console.WriteLine();
                Console.WriteLine($"Duration: {stopwatch.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                Console.ResetColor();
            }

            Console.WriteLine("Done");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void WriteResultsToConsole(IReadOnlyCollection<Models.ArtistResultModel> results)
        {
            const string ArtistNameColumnHeader = "Artist Name";
            const string NumberOfRecordsColumnHeader = "Number Of Records";
            const string AverageColumnHeader = "Average";
            const string MinimumColumnHeader = "Minimum";
            const string MaximumColumnHeader = "Maximum";

            var artistNameColumnWidth = results.Select(x => x.ArtistName).OrderByDescending(x => x.Length).First().Length;
            artistNameColumnWidth = Math.Max(artistNameColumnWidth, ArtistNameColumnHeader.Length);

            Console.Clear();
            Console.WriteLine($"|  {ArtistNameColumnHeader.PadRight(artistNameColumnWidth)}  |  {NumberOfRecordsColumnHeader}  |  {MinimumColumnHeader}  |  {AverageColumnHeader}  |  {MaximumColumnHeader}  |");

            foreach (var result in results)
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.Append("|  ");
                stringBuilder.Append(result.ArtistName.PadRight(artistNameColumnWidth));
                stringBuilder.Append("  |  ");
                stringBuilder.Append(result.RecordLyrics.Count.ToString().PadRight(NumberOfRecordsColumnHeader.Length));
                stringBuilder.Append("  |  ");
                stringBuilder.Append(result.MinimumNumberOfWordsPerRecording.ToString().PadRight(MinimumColumnHeader.Length));
                stringBuilder.Append("  |  ");
                stringBuilder.Append(Math.Round(result.AverageNumberOfWordsPerRecording, 2).ToString().PadRight(AverageColumnHeader.Length));
                stringBuilder.Append("  |  ");
                stringBuilder.Append(result.MaximumNumberOfWordsPerRecording.ToString().PadRight(MaximumColumnHeader.Length));
                stringBuilder.Append("  |");

                Console.WriteLine(stringBuilder);
            }
        }
    }
}