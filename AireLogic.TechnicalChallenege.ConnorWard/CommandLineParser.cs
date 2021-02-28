using System;
using System.Collections.Generic;
using System.Linq;

namespace AireLogic.TechnicalChallenege.ConnorWard
{
    public class CommandLineParser : ICommandLineParser
    {
        public List<string> Parse(string[] commandLineArguments)
        {
            if (!commandLineArguments?.Any() ?? true)
                throw new ArgumentNullException("Command line arguments cannot be null or empty");

            return commandLineArguments.Select(x => x.ToLowerInvariant()).ToList();
        }
    }
}