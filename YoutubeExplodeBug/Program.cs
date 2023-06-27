using System.Diagnostics;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YoutubeExplodeBug
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			string url = "https://www.youtube.com/watch?v=k59E7T0H-Us&ab_channel=EurovisionSongContest";

			YoutubeClient client = new YoutubeClient();
			var streamManifest = await client.Videos.Streams.GetManifestAsync(url);
			var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

			using Process ffmpeg = CreateStream(streamInfo.Url);
			using Stream audioStream = ffmpeg.StandardOutput.BaseStream;

			byte[] buffer = new byte[48000 * 2 * 16];

			while (true)
			{
				await audioStream.ReadAsync(buffer);
				await Task.Delay(100);
			}
		}

		private static Process CreateStream(string path) => Process.Start(new ProcessStartInfo
		{ 
			FileName = "ffmpeg",
			Arguments = $"-i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
			UseShellExecute = false,
			RedirectStandardOutput = true,
		});
	}
}