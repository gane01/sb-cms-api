using System.Text.Json.Serialization;

namespace SbContentManager.ContentstackApi
{
    public class OrQuery<T>
    {
        [JsonPropertyName("$or")]
        public List<T>? Values { get; set; }
    }

    public class UidQuery
    {
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }
    }
}
