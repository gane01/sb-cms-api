using System.Text.Json.Serialization;

namespace SbContentManager.ContentstackApi
{
    public class PublishAssetParam
    {
        [JsonPropertyName("asset")]
        public AssetValues? Asset { get; set; }

        public PublishAssetParam(string environment, string locale) { 
            Asset = new AssetValues() {
                Locales = new string[] { locale },
                Environments = new string[] { environment }
            };
        }
    }

    public class AssetValues
    {
        [JsonPropertyName("locales")]
        public string[]? Locales { get; set; }

        [JsonPropertyName("environments")]
        public string[]? Environments { get; set; }
    }
}



