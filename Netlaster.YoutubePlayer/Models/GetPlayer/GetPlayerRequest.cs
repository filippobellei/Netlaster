namespace Netlaster.YoutubePlayer.Models.GetPlayer;

public class GetPlayerRequest
{
    public required string VideoId { get; set; }
    public required Context Context { get; set; }
}

public class Context
{
    public required Client Client { get; set; }
}

public class Client
{
    public required string ClientName { get; set; }
    public required string ClientVersion { get; set; }
    public required string VisitorData { get; set; }
}
