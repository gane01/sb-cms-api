using System.Text.Json.Serialization;

namespace SbContentManager.ContentstackApi.Publish
{
    public class PublishDetailsDto
    {
        [JsonPropertyName("locales")]
        public string[]? Locales { get; set; }

        [JsonPropertyName("environments")]
        public string[]? Environments { get; set; }
    }
}



