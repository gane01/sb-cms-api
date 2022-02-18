using System.Text.Json.Serialization;

namespace SbContentManager.Entry
{
    public partial class Welcome
    {
        [JsonPropertyName("entry")]
        public Entry Entry { get; set; }
    }

    public partial class Entry
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("locales")]
        public List<Locale> Locales { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }
    }

    public partial class Locale
    {
        [JsonPropertyName("language_code")]
        public string LanguageCode { get; set; }

        [JsonPropertyName("texts")]
        public List<Text> Texts { get; set; }

        [JsonPropertyName("richtexts")]
        public List<Richtext> Richtexts { get; set; }

        [JsonPropertyName("links")]
        public List<LinkElement> Links { get; set; }

        [JsonPropertyName("images")]
        public List<Image> Images { get; set; }
    }

    public partial class Image
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("image")]
        public string ImageImage { get; set; }
    }

    public partial class LinkElement
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("link")]
        public LinkLink Link { get; set; }
    }

    public partial class LinkLink
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("href")]
        public Uri Href { get; set; }
    }

    public partial class Richtext
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("richtext")]
        public string RichtextRichtext { get; set; }
    }

    public partial class Text
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("text")]
        public string TextText { get; set; }
    }
}
