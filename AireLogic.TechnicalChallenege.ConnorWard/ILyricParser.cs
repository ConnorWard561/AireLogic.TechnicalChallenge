using System.Collections.Generic;

namespace AireLogic.TechnicalChallenge.ConnorWard
{
    public interface ILyricParser
    {
        IReadOnlyCollection<string> Parse(string lyrics);
    }
}