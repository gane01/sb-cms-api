using Refit;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SbContentManager.ContentstackApi
{
    public interface IContentstackApi
    {
        [Get("/content_types/{templateId}/entries?environment={environment}&locale=en&query={query}")]
        Task<JsonNode> GetEntries(string environment, string templateId, string query);

        [Post("/content_types/{templateId}/entries?environment={environment}&locale=en")]
        Task<JsonNode> CreateEntry(string environment, string templateId, [Body] JsonNode content);
    
        /*
        [Post("/content_types/{templateId}/entries/{contentId}")]
        Task PublishEntry(string templateId, string environment, [Body] string testArticle);
        */
    }
}
