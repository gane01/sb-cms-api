using System.Text.Json;
using System.Text.Json.Serialization;

namespace SbContentManager.Entry
{
    public class SbsmEntity<T>
    {
        [JsonPropertyName("entry")]
        public Entry<T> Entry { get; set; }
    }

    public class SbsmEntities<T>
    {
        [JsonPropertyName("entries")]
        public IEnumerable<Entry<T>> Entries { get; set; }
    }

    public class Entry<T>
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("locales")]
        public IEnumerable<Locale<T>> Locales { get; set; }

        [JsonPropertyName("tags")]
        public IEnumerable<string> Tags { get; set; }
    }

    public class Locale<T>
    {
        [JsonPropertyName("language_code")]
        public string LanguageCode { get; set; }

        [JsonPropertyName("texts")]
        public IEnumerable<TextField> Texts { get; set; }

        [JsonPropertyName("richtexts")]
        public IEnumerable<RichtextFiled> Richtexts { get; set; }

        [JsonPropertyName("links")]
        public IEnumerable<LinkField> Links { get; set; }

        [JsonPropertyName("images")]
        public IEnumerable<ImageField<T>> Images { get; set; }
    }

    public class ImageField<T>
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("image")]
        public T Image { get; set; }
    }

    public class LinkField
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("link")]
        public Link Link { get; set; }
    }

    public class Link
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("href")]
        public Uri Href { get; set; }
    }

    public class RichtextFiled
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("richtext")]
        public string Richtext { get; set; }
    }

    public class TextField
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
