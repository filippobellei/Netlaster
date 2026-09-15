using System.Text.Json.Serialization;
using Netlaster.YoutubePlayer.Models.GetPlayer;

namespace Netlaster.YoutubePlayer.Serialization;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(GetPlayerRequest))]
[JsonSerializable(typeof(GetPlayerResponse))]
public partial class SourceGenerationContext : JsonSerializerContext;
