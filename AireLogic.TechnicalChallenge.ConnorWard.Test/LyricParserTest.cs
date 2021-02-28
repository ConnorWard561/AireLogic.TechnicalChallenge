using AireLogic.TechnicalChallenge.ConnorWard;
using NUnit.Framework;
using System.Linq;

namespace AireLogic.TechnicalChallenege.ConnorWard.Test
{
    public class LyricParserTest
    {
        [Test]
        public void Parse_WhenCalledWithEmptyLyrics_ReturnsEmptyReadOnlyCollection()
        {
            var sut = GetSubjectUnderTest();

            var result = sut.Parse(string.Empty);

            Assert.IsEmpty(result);
        }

        [TestCase("Hello\rWorld", "Hello", "World")]
        [TestCase("Hello\nWorld", "Hello", "World")]
        [TestCase("Hello World", "Hello", "World")]
        [TestCase("Hello\r \nWorld", "Hello", "World")]
        public void Parse_WhenCalled_ReturnsExpectedReadOnlyCollection(string testValue, string expectedValue1, string expectedValue2)
        {
            var sut = GetSubjectUnderTest();

            var result = sut.Parse(testValue);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(expectedValue1, result.ElementAt(0));
            Assert.AreEqual(expectedValue2, result.ElementAt(1));
        }

        private LyricParser GetSubjectUnderTest()
        {
            return new LyricParser();
        }
    }
}