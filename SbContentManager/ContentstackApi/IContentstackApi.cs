using Refit;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SbContentManager.ContentstackApi
{
    public interface IContentstackApi
    {
        [Get("/content_types/{templateId}/entries?environment={environment}&locale=en&query={query}")]
        Task<JsonElement> GetEntries(string environment, string templateId, string query);

        [Post("/content_types/{templateId}/entries?environment={environment}&locale=en")]
        Task<JsonElement> CreateEntry(string environment, string templateId, [Body] JsonElement content);

        [Put("/content_types/{templateId}/entries/{contentId}/?locale=en")]
        Task<JsonElement> UpdateEntry(string templateId, string contentId, [Body] JsonElement content);

        /*
        [Post("/content_types/{templateId}/entries/{contentId}")]
        Task PublishEntry(string templateId, string environment, [Body] string testArticle);
        */
    }
}
