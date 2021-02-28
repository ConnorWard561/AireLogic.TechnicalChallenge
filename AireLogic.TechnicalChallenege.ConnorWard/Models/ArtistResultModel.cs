using System.Collections.Generic;
using System.Linq;

namespace AireLogic.TechnicalChallenege.ConnorWard.Models
{
    public class ArtistResultModel
    {
        public string ArtistName { get; set; }

        public Dictionary<string, IReadOnlyCollection<string>> RecordLyrics { get; set; } = new Dictionary<string, IReadOnlyCollection<string>>();

        public List<string> UnavailableRecordLyrics { get; set; } = new List<string>();

        public int MinimumNumberOfWordsPerRecording => GetNumberOfWordsPerRecording().Min();

        public int MaximumNumberOfWordsPerRecording => GetNumberOfWordsPerRecording().Max();

        public float AverageNumberOfWordsPerRecording
        {
            get
            {
                var numberOfRecordings = RecordLyrics.Count;

                var totalNumberOfWordsInRecordings = RecordLyrics.SelectMany(x => x.Value).Count();

                return (float)totalNumberOfWordsInRecordings / numberOfRecordings;
            }
        }

        private List<int> GetNumberOfWordsPerRecording()
        {
            return RecordLyrics?.Select(x => x.Value?.Count ?? 0).ToList() ?? new List<int>();
        }
    }
}