using System.Text.Json.Serialization;

namespace SbContentManager.Entry
{
    public class EntryCopyRequestDto
    {
        [JsonPropertyName("templateId")]
        public string TemplateId { get; set; }
        [JsonPropertyName("contentIds")]
        public IEnumerable<string> ContentIds { get; set; }
        [JsonPropertyName("folderId")]
        public string FolderId { get; set; }
        // https://www.andiamo.co.uk/resources/iso-language-codes/
        [JsonPropertyName("languageCodes")]
        public IEnumerable<string> LanguageCodes { get; set; }
    }
}