using System.Text.Json.Serialization;

namespace SbContentManager.Models
{
    public partial class Response<T>
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }

    public partial class Data
    {
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }
    }
}