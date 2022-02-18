using SbContentManager.Asset;
using SbContentManager.Contentstack;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SbContentManager.Entry
{
	public class EntryEffect
	{
		private HttpClient httpClient;
		private ContentstackClient contentstackClient;
		private AssetEffect assetEffect;
		public EntryEffect(HttpClient httpClient, ContentstackClient contentstackClient, AssetEffect assetEffect)
		{
			this.httpClient = httpClient;
			this.contentstackClient = contentstackClient;
			this.assetEffect = assetEffect;
		}

		public async Task<IEnumerable<SbsmEntity<string>>> Copy(string contentType, IEnumerable<string> entryIds, string folderId, IEnumerable<string> languageCodes) {
			var entries = await contentstackClient.GetEntries(contentType, entryIds);
			var sbsmEntities = JsonSerializer.Deserialize<SbsmEntities<JsonElement>>(entries);
			return await CopyEntries(contentType, sbsmEntities.Entries, languageCodes, folderId);
			// https://www.contentstack.com/docs/developers/apis/content-management-api/#publish-an-entry-with-references
		}

		private async Task<IEnumerable<SbsmEntity<string>>> CopyEntries(string contentType, IEnumerable<Entry<JsonElement>> entries, IEnumerable<string> languageCodes, string folderId)
		{
			var tasks = entries.Select(entry => CopyEntry(contentType, entry, languageCodes, folderId)).ToList();
			await Task.WhenAll(tasks);
			return tasks.Select(task => task.Result);
		}

		private async Task<SbsmEntity<string>> CopyEntry(string contentType, Entry<JsonElement> entry, IEnumerable<string> languageCodes, string folderId) {
			var newEntry = new SbsmEntity<string>
			{
				Entry = new Entry<string>
				{
					Title = new Guid().ToString(),
					Tags = entry.Tags,
					Locales = await CopyLocales(languageCodes, entry.Locales, folderId)
				}
			};

			var result = await contentstackClient.CreateEntry(contentType, JsonSerializer.SerializeToElement(newEntry));
			newEntry.Entry.Id = result.Data.Uid;
			return newEntry;
		}

		private async Task<IEnumerable<Locale<string>>> CopyLocales(IEnumerable<string> languageCodes, IEnumerable<Locale<JsonElement>> locales, string folderId)
		{
			var filteredLocales = locales.Where(locale => !languageCodes.Contains(locale.LanguageCode));
			var tasks = filteredLocales.Select(locale => CopyLocale(locale, folderId)).ToList();
			await Task.WhenAll(tasks);
			return tasks.Select(task => task.Result);
		}

		private async Task<Locale<string>> CopyLocale(Locale<JsonElement> locale, string folderId) {
			return new Locale<string>()
			{
				LanguageCode = locale.LanguageCode,
				Links = locale.Links,
				Richtexts = locale.Richtexts,
				Texts = locale.Texts,
				Images = await CopyImages(locale.Images, folderId)
			};
		}

		private async Task<IEnumerable<ImageField<string>>> CopyImages(IEnumerable<ImageField<JsonElement>> images, string folderId)
		{
			var tasks = images.Select(image => CopyImage(image, folderId)).ToList();
			await Task.WhenAll(tasks);
			return tasks.Select(task => task.Result);
		}

		private async Task<ImageField<string>> CopyImage(ImageField<JsonElement> image, string folderId) {
			var asset = new AssetModel
			{
				Id = image.Image.GetProperty("uid").GetString(),
				Title = image.Image.GetProperty("title").GetString(),
				Description = image.Image.TryGetProperty("description", out JsonElement descriptionElement) ? descriptionElement.GetString() : string.Empty,
				FileName = image.Image.GetProperty("filename").GetString(),
				ContentType = image.Image.GetProperty("content_type").GetString(),
				Url = image.Image.GetProperty("url").GetString(),
				Tags = image.Image.GetProperty("tags").EnumerateArray().Select((tag) => tag.GetString()).ToArray()
			};

			return new ImageField<string> { Key = image.Key, Image = await assetEffect.Copy(asset, folderId) };
		}
	}
}