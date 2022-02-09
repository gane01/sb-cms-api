using System.Text.Json.Serialization;

namespace SbContentManager.ContentstackClient.Publish
{
    public class PublishEntryDto
    {
        [JsonPropertyName("entry")]
        public PublishDetailsDto? Details { get; set; }

        public PublishEntryDto(string environment, string locale)
        {
            Details = new PublishDetailsDto()
            {
                Locales = new string[] { locale },
                Environments = new string[] { environment }
            };
        }
    }
}



