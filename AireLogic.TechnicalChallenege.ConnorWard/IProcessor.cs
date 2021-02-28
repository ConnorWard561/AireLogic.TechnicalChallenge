using AireLogic.TechnicalChallenege.ConnorWard.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AireLogic.TechnicalChallenege.ConnorWard
{
    public interface IProcessor
    {
        Task<IReadOnlyCollection<ArtistResultModel>> Run(IReadOnlyCollection<string> artists, Action<string> updateProgress);
    }
}