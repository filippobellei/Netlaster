using System.Text.Json.Serialization;
using Netlaster.Models.GetPlayer;

namespace Netlaster.Serialization;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(GetPlayerRequest))]
[JsonSerializable(typeof(GetPlayerResponse))]
public partial class SourceGenerationContext : JsonSerializerContext;
