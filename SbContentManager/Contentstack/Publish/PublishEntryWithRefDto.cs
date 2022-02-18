using System.Text.Json.Serialization;

namespace SbContentManager.Contentstack.Publish
{
    public class PublishEntryWirhRefDto
    {
        [JsonPropertyName("entries")]
        public IEnumerable<Entry> Entries { get; set; }

        [JsonPropertyName("locales")]
        public IEnumerable<string> Locales { get; set; }

        [JsonPropertyName("environments")]
        public IEnumerable<string> Environments { get; set; }

        [JsonPropertyName("publish_with_reference")]
        public bool PublishWithReference { get; set; }

        [JsonPropertyName("skip_workflow_stage_check")]
        public bool SkipWorkflowStageCheck { get; set; }
    }

    public class Entry
    {
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        [JsonPropertyName("content_type")]
        public string ContentType { get; set; }

        [JsonPropertyName("version")]
        public long Version { get; set; }
    }
}



