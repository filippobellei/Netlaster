using Netlaster.YoutubePlayer;

var videoId = "Qyy9tlKkKVA";

using var httpClient = new HttpClient();
var playerService = new PlayerService(httpClient);

var player = await playerService.GetPlayerAsync(videoId);
var audio = player.StreamingData.AdaptiveFormats.First(x =>
    x.MimeType.StartsWith("audio/webm")
    && (x.AudioTrack is null || x.AudioTrack.AudioIsDefault)
);

var streamUrl = audio.Url;
var contentLength = Convert.ToInt64(audio.ContentLength);

using var stream = await playerService.RetrieveContentStreamAsync(contentLength, streamUrl);

var fileName = $"{videoId}.opus";
using var fileStream = File.OpenWrite(fileName);

await stream.CopyToAsync(fileStream);
