using System.Threading.Tasks;

namespace AireLogic.TechnicalChallenege.ConnorWard
{
    public interface ILyricsProvider
    {
        Task<string> GetLyrics(string artistName, string recordingTitle);
    }
}