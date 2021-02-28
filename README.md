Instructions
- Language: C#
- Runtime: .Net Core 3.1
- Dependencies: No external dependencies
- How to run: 
	From the command line, run the 'AireLogic.TechnicalChallenge.ConnorWard.exe' file with the following command line arguments
		- Argument 1 - argument N: Artist names
	
Estimated Duration
- 5-6 hours including unit testing, publishing and packaging final artefact.

Notes
- The MusicBrainz API is very limited in that it can only cope with either a maximum of 300 global requests per second and 50 requests per application per second.
	This has a significant impact when retrieving the back catalogue of artists who have created a large number of recordings, I.E. Queen.
- The Lyrics API is very slow and whilst I've parallelised where possible the application still runs slowly.
- The test suite is not complete but should be sufficient to show how I would normally approach testing.