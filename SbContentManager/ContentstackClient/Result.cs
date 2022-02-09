using System.Text.Json.Serialization;

namespace SbContentManager.ContentstackClient
{
    public class Result<T>
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }

    public class UidResult
    {
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }
    }
}