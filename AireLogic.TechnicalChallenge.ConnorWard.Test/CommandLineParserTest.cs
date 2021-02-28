using AireLogic.TechnicalChallenege.ConnorWard;
using NUnit.Framework;
using System;

namespace AireLogic.TechnicalChallenge.ConnorWard.Test
{
    public class CommandLineParserTest
    {
        [Test]
        public void Parse_WhenCommandLineArgumentsIsNull_ThrowsArgumentNullException()
        {
            var sut = GetSubjectUnderTest();

            Assert.Throws<ArgumentNullException>(() => sut.Parse(null));
        }

        [Test]
        public void Parse_WhenCommandLineArgumentsIsEmpty_ThrowsArgumentNullException()
        {
            var sut = GetSubjectUnderTest();

            Assert.Throws<ArgumentNullException>(() => sut.Parse(new string[0]));
        }

        [Test]
        public void Parse_WhenCalledWithValues_ReturnsLowerCaseValues()
        {
            const string Value1 = "VALUE1";
            const string Value2 = "Value2";
            const string Value3 = "vaLUE3";

            var sut = GetSubjectUnderTest();

            var results = sut.Parse(new string[] { Value1, Value2, Value3 });

            Assert.AreEqual(3, results.Count);
            Assert.AreEqual(Value1.ToLowerInvariant(), results[0]);
            Assert.AreEqual(Value2.ToLowerInvariant(), results[1]);
            Assert.AreEqual(Value3.ToLowerInvariant(), results[2]);
        }

        private CommandLineParser GetSubjectUnderTest()
        {
            return new CommandLineParser();
        }
    }
}