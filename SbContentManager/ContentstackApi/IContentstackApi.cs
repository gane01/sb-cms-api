using Refit;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SbContentManager.ContentstackApi
{
    public interface IContentstackApi
    {
        [Get("/content_types/{contentType}/entries?environment={environment}&locale=en&query={query}")]
        Task<JsonElement> GetEntries(string environment, string contentType, string query);

        [Post("/content_types/{contentType}/entries?environment={environment}&locale=en")]
        Task<JsonElement> CreateEntry(string environment, string contentType, [Body] JsonElement entry);

        [Delete("/content_types/{contentType}/entries/{entryId}?delete_all_localized=true")]
        Task<JsonElement> DeleteEntry(string contentType, string entryId);

        [Put("/content_types/{contentType}/entries/{entryId}/?locale=en")]
        Task<JsonElement> UpdateEntry(string contentType, string entryId, [Body] JsonElement entry);

        [Post("/content_types/{contentType}/entries/{entryId}/publish")]
        Task<JsonElement> PublishEntry(string contentType, string entryId, [Body] JsonElement publish);

        [Get("/assets?environment={environment}&query={query}")]
        Task<JsonElement> GetAssets(string environment, string query);

        [Multipart]
        [Post("/assets")]
        Task<JsonElement> CreateAsset([AliasAs("asset[upload]")] ByteArrayPart file, [AliasAs("asset[parent_uid]")] string folderId, [AliasAs("asset[title]")] string title, [AliasAs("asset[description]")] string description, [AliasAs("asset[tags]")] string tags);

        [Delete("/assets/{assetId}")]
        Task<JsonElement> DeleteAsset(string assetId);

        [Get("/assets/{assetId}/references")]
        Task<JsonElement> GetAssetRef(string assetId);

        [Get("/assets?include_folders=true&query={query}")]
        Task<JsonElement> GetAssetFolder(string query);

        [Post("/assets/{assetId}/publish")]
        Task<JsonElement> PublishAsset(string assetId, [Body] JsonElement publish);
    }
}
