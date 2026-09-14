namespace Netlaster.Models.GetPlayer;

public class GetPlayerResponse
{
    public required PlayabilityStatus PlayabilityStatus { get; set; }
    public required StreamingData StreamingData { get; set; }
    public required VideoDetails VideoDetails { get; set; }
}

public class PlayabilityStatus
{
    public required string Status { get; set; }
}

public class StreamingData
{
    public required IEnumerable<AdaptiveFormats> AdaptiveFormats { get; set; }
}

public class VideoDetails
{
    public required string Title { get; set; }
    public required string LengthSeconds { get; set; }
}

public class AdaptiveFormats
{
    public required string Url { get; set; }
    public required string MimeType { get; set; }
    public required int Bitrate { get; set; }
    public required string ContentLength { get; set; }
    public AudioTrack AudioTrack { get; set; }
}

public class AudioTrack
{
    public required string DisplayName { get; set; }
    public required string Id { get; set; }
    public required bool AudioIsDefault { get; set; }
}
