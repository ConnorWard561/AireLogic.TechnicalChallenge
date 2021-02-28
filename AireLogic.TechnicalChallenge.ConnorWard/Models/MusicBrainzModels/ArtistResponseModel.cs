using System;

namespace AireLogic.TechnicalChallenege.ConnorWard.Models.MusicBrainzModels
{
    public class ArtistResponseModel
    {
        public Guid Id { get; set; }

        public int Score { get; set; }

        public string Name { get; set; }
    }
}