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

        [Put("/content_types/{contentType}/entries/{entryId}/?locale=en")]
        Task<JsonElement> UpdateEntry(string contentType, string entryId, [Body] JsonElement entry);

        [Post("/content_types/{contentType}/entries/{entryId}/publish")]
        Task<JsonElement> PublishEntry(string environment, string contentType, string entryId, [Body] JsonElement publish);

        [Get("/assets?environment={environment}&query={query}")]
        Task<JsonElement> GetAssets(string environment, string query);
    }
}
