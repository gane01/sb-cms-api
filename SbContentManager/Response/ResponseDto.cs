using System.Text.Json.Serialization;

namespace SbContentManager.Response
{
    public class ResponseDto<T>
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }

    public class ResponseDetailsUidDto
    {
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }
    }
}