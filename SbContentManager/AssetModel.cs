namespace SbContentManager
{
    public class AssetModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string Url { get; set; }
        public string?[]? Tags { get; set; }
    }
}
