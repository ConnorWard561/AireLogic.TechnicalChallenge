using System.Collections.Generic;

namespace AireLogic.TechnicalChallenege.ConnorWard
{
    public interface ICommandLineParser
    {
        List<string> Parse(string[] commandLineArguments);
    }
}