using Refit;

namespace SbContentManager.ContentstackApi
{
    public interface IContentstackApi
    {
        [Post("/content_types/{templateId}/entries?environment={environment}&locale=en")]
        Task CreateEntry(string environment, string templateId, [Body] string testArticle);

        [Post("/content_types/{templateId}/entries/{contentId}")]
        Task PublishEntry(string templateId, string environment, [Body] string testArticle);
    }
}
