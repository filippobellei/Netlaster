using System.Net.Mime;
using System.Web;
using Android.Content;
using Android.Provider;
using Android.Util;
using Netlaster.Application.Helpers;
using Netlaster.YoutubePlayer;

namespace Netlaster.Application;

[Activity(
    Exported = true,
    Icon = "@mipmap/appicon",
    MainLauncher = false,
    Theme = "@android:style/Theme.NoDisplay"
)]
[IntentFilter(
    [Intent.ActionSend],
    Categories = [Intent.CategoryDefault],
    DataMimeType = MediaTypeNames.Text.Plain
)]
public class MainActivity : Activity
{
    protected override async void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        try
        {
            var url = Intent.GetStringExtra(Intent.ExtraText);

            if (string.IsNullOrEmpty(url) || !url.Contains("youtube.com"))
                throw new Exception("Link non valido");

            var parsedUrl = url.Split('?')[1];
            var paramsCollection = HttpUtility.ParseQueryString(parsedUrl);
            var videoId = paramsCollection["v"];

            if (string.IsNullOrEmpty(videoId) && url.Contains("youtube.com"))
                throw new Exception("Link YouTube non valido");

            using var httpClient = new HttpClient();
            var playerService = new PlayerService(httpClient);

            Toast.MakeText(this, "Download iniziato", ToastLength.Short).Show();

            var player = playerService
                .GetPlayerAsync(videoId)
                .GetAwaiter()
                .GetResult();
            var audio = player.StreamingData.AdaptiveFormats.First(x =>
                x.MimeType.StartsWith("audio/webm")
                && (x.AudioTrack is null || x.AudioTrack.AudioIsDefault)
            );

            var streamUrl = audio.Url;
            var contentLength = Convert.ToInt64(audio.ContentLength);

            using var stream = playerService
                .RetrieveContentStreamAsync(contentLength, streamUrl)
                .GetAwaiter()
                .GetResult();

            var fileName = FileHelper.SanitizeFileName(player.VideoDetails.Title) + ".opus";

            using var values = new ContentValues();
            values.Put(MediaStore.IMediaColumns.DisplayName, fileName);
            values.Put(MediaStore.IMediaColumns.MimeType, "audio/webm");
            values.Put(MediaStore.IMediaColumns.RelativePath, Android.OS.Environment.DirectoryDownloads);

            using var uri = ApplicationContext.ContentResolver.Insert(
                MediaStore.Downloads.ExternalContentUri,
                values
            );

            using var outputStream = ApplicationContext.ContentResolver.OpenOutputStream(uri);

            stream.CopyToAsync(outputStream)
                .GetAwaiter()
                .GetResult();

            Toast.MakeText(this, "Download completato", ToastLength.Long).Show();
        }
        catch (Exception ex)
        {
            Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            Log.Error("Netlaster", ex.ToString());
        }

        Finish();
    }
}
