using System;
using System.Collections.Generic;
using System.Linq;

namespace AireLogic.TechnicalChallenge.ConnorWard
{
    public class LyricParser : ILyricParser
    {
        public IReadOnlyCollection<string> Parse(string lyrics)
        {
            if (string.IsNullOrEmpty(lyrics))
                return new string[0];

            return lyrics.Split(new string[] { "\r", "\n", " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
        }
    }
}