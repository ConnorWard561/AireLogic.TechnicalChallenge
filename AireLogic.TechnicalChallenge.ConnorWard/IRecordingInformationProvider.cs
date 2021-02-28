using System.Collections.Generic;
using System.Threading.Tasks;

namespace AireLogic.TechnicalChallenege.ConnorWard
{
    public interface IRecordingInformationProvider
    {
        Task<List<string>> GetRecordingTitlesByArtistName(string artistName);
    }
}