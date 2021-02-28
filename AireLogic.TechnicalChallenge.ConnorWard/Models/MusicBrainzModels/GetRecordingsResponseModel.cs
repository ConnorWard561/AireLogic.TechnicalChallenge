using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AireLogic.TechnicalChallenege.ConnorWard.Models.MusicBrainzModels
{
    public class GetRecordingsResponseModel
    {
        public List<RecordingResponseModel> Recordings { get; set; }

        [JsonPropertyName("recording-count")]
        public int RecordingCount { get; set; }
    }
}