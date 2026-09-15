using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Netlaster.YoutubePlayer.Models.GetPlayer;
using Netlaster.YoutubePlayer.Serialization;

namespace Netlaster.YoutubePlayer;

public class PlayerService(HttpClient _httpClient)
{
    private const string VISITOR_DATA_URL = "https://www.youtube.com/sw.js_data";
    private const string PLAYER_URL = "https://www.youtube.com/youtubei/v1/player";
    private const string CLIENT_NAME = "VISIONOS";
    private const string CLIENT_VERSION = "1.02";
    private const int CHUNKSIZE = 1_000_000;

    private async Task<string> ResolveVisitorDataAsync(CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient
            .GetAsync(VISITOR_DATA_URL, cancellationToken)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var content = await response.Content
            .ReadAsStringAsync(cancellationToken)
            .ConfigureAwait(false);

        if (content.StartsWith(")]}'"))
            content = content[4..];

        var json = JsonNode.Parse(content);

        var visitorData = json?[0]?[2]?[0]?[0]?[13]?.ToString();

        if (string.IsNullOrWhiteSpace(visitorData))
            throw new Exception("Failed to resolve visitor data");

        return visitorData;
    }

    public async Task<GetPlayerResponse> GetPlayerAsync(
        string videoId,
        CancellationToken cancellationToken = default
    )
    {
        var visitorData = await ResolveVisitorDataAsync(cancellationToken)
            .ConfigureAwait(false);

        var request = new GetPlayerRequest
        {
            VideoId = videoId,
            Context = new Context
            {
                Client = new Client
                {
                    ClientName = CLIENT_NAME,
                    ClientVersion = CLIENT_VERSION,
                    VisitorData = visitorData
                }
            }
        };

        var jsonRequest = JsonSerializer.Serialize(
            request,
            SourceGenerationContext.Default.GetPlayerRequest
        );
        using var requestContent = new StringContent(jsonRequest);

        using var response = await _httpClient
            .PostAsync(
                PLAYER_URL,
                requestContent,
                cancellationToken
            )
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content
            .ReadFromJsonAsync(SourceGenerationContext.Default.GetPlayerResponse, cancellationToken)
            .ConfigureAwait(false);

        var playabilityStatus = responseContent?.PlayabilityStatus.Status;
        var details = responseContent?.VideoDetails;

        if (string.Equals(playabilityStatus, "error", StringComparison.OrdinalIgnoreCase) || details is null)
            throw new Exception($"Video '{videoId}' is not available");

        return responseContent!;
    }

    public async Task<Stream> RetrieveContentStreamAsync(
        long contentLength,
        string baseUrl,
        CancellationToken cancellationToken = default
    )
    {
        var memoryStream = new MemoryStream();

        for (long i = 0; i < contentLength; i += CHUNKSIZE)
        {
            var url = baseUrl + $"&range={i}-{i + CHUNKSIZE - 1}";

            using var response = await _httpClient
                .GetAsync(url, cancellationToken)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            await using var responseContent = await response.Content
                .ReadAsStreamAsync(cancellationToken)
                .ConfigureAwait(false);

            await responseContent
                .CopyToAsync(memoryStream, cancellationToken)
                .ConfigureAwait(false);
        }

        memoryStream.Position = 0;
        return memoryStream;
    }
}
