using AireLogic.TechnicalChallenege.ConnorWard;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AireLogic.TechnicalChallenge.ConnorWard.Test
{
    public class ProcessorTest
    {
        private Mock<IRecordingInformationProvider> recordingInformationProviderMock;
        private Mock<ILyricsProvider> lyricsProviderMock;
        private Mock<ILyricParser> lyricParserMock;

        [SetUp]
        public void SetUp()
        {
            recordingInformationProviderMock = new Mock<IRecordingInformationProvider>();
            lyricsProviderMock = new Mock<ILyricsProvider>();
            lyricParserMock = new Mock<ILyricParser>();
        }

        [Test]
        public void Constructor_WhenRecordingInformationProviderIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Processor(null, Mock.Of<ILyricsProvider>(), Mock.Of<ILyricParser>()));
        }

        [Test]
        public void Constructor_WhenLyricsProviderIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Processor(Mock.Of<IRecordingInformationProvider>(), null, Mock.Of<ILyricParser>()));
        }

        [Test]
        public void Constructor_WhenLyricParserIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Processor(Mock.Of<IRecordingInformationProvider>(), Mock.Of<ILyricsProvider>(), null));
        }

        [Test]
        public async Task Run_WhenArtistsIsEmpty_ReturnEmptyCollection()
        {
            var sut = GetSubjectUnderTest();

            var result = await sut.Run(new string[0], null);

            Assert.IsFalse(result.Any());
        }

        [Test]
        public async Task Run_WhenArtistsHasValue_RetrieveInformationAboutRecordings()
        {
            const string ArtistName = "AnArtistName";

            recordingInformationProviderMock.Setup(x => x.GetRecordingTitlesByArtistName(ArtistName)).Returns(Task.FromResult(new List<string>()));

            var sut = GetSubjectUnderTest();

            await sut.Run(new string[] { ArtistName }, null);

            recordingInformationProviderMock.Verify(x => x.GetRecordingTitlesByArtistName(ArtistName), Times.Once);
        }

        [Test]
        public async Task Run_WhenArtistsHasValueAndRecordingsRetrieved_RetrieveLyrics()
        {
            const string ArtistName = "AnArtistName";
            const string RecordingTitle1 = "SomeRecording";
            const string RecordingTitle2 = "SomeOtherRecording";

            var expectedRecords = new List<string> 
            {
                RecordingTitle1,
                RecordingTitle2
            };

            recordingInformationProviderMock.Setup(x => x.GetRecordingTitlesByArtistName(ArtistName)).Returns(Task.FromResult(expectedRecords));
            lyricsProviderMock.Setup(x => x.GetLyrics(ArtistName, RecordingTitle1)).Returns(Task.FromResult("Some Lyrics"));
            lyricsProviderMock.Setup(x => x.GetLyrics(ArtistName, RecordingTitle2)).Returns(Task.FromResult("Some Other Lyrics"));

            var sut = GetSubjectUnderTest();

            await sut.Run(new string[] { ArtistName }, null);

            lyricsProviderMock.Verify(x => x.GetLyrics(ArtistName, RecordingTitle1), Times.Once);
            lyricsProviderMock.Verify(x => x.GetLyrics(ArtistName, RecordingTitle2), Times.Once);
            lyricsProviderMock.Verify(x => x.GetLyrics(ArtistName, It.IsAny<string>()), Times.Exactly(expectedRecords.Count));
        }

        [Test]
        public async Task Run_WhenArtistsHasValueAndLyricsIsEmpty_ReturnExpectedValue()
        {
            const string ArtistName = "AnArtistName";
            const string RecordingTitle1 = "SomeRecording";
            const string RecordingTitle2 = "SomeOtherRecording";

            var expectedRecords = new List<string>
            {
                RecordingTitle1,
                RecordingTitle2
            };

            recordingInformationProviderMock.Setup(x => x.GetRecordingTitlesByArtistName(ArtistName)).Returns(Task.FromResult(expectedRecords));
            lyricsProviderMock.Setup(x => x.GetLyrics(ArtistName, RecordingTitle1)).Returns(Task.FromResult("Some Lyrics"));
            lyricsProviderMock.Setup(x => x.GetLyrics(ArtistName, RecordingTitle2)).Returns(Task.FromResult(string.Empty));

            var sut = GetSubjectUnderTest();

            var results =  await sut.Run(new string[] { ArtistName }, null);

            Assert.AreEqual(1, results.Count);

            var firstResult = results.First();

            Assert.AreEqual(ArtistName, firstResult.ArtistName);
            Assert.AreEqual(1, firstResult.UnavailableRecordLyrics.Count);
            Assert.AreEqual(RecordingTitle2, firstResult.UnavailableRecordLyrics.First());

            Assert.AreEqual(1, firstResult.RecordLyrics.Count);
            Assert.AreEqual(RecordingTitle1, firstResult.RecordLyrics.First().Key);
        }

        private Processor GetSubjectUnderTest()
        {
            return new Processor(recordingInformationProviderMock.Object, lyricsProviderMock.Object, lyricParserMock.Object);
        }
    }
}