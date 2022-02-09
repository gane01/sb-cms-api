using System.Text.Json.Serialization;

namespace SbContentManager.ContentstackApi.Publish
{
    public class PublishAssetDto
    {
        [JsonPropertyName("asset")]
        public PublishDetailsDto? Details { get; set; }

        public PublishAssetDto(string environment, string locale) {
            Details = new PublishDetailsDto() {
                Locales = new string[] { locale },
                Environments = new string[] { environment }
            };
        }
    }
}



