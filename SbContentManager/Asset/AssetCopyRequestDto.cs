using System.Text.Json.Serialization;

namespace SbContentManager.Asset
{
    public class AssetCopyRequestDto
    {
        [JsonPropertyName("assetIds")]
        public string[] AssetIds { get; set; }
        [JsonPropertyName("folderId")]
        public string FolderId { get; set; }
    }
}
