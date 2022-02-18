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
		}

		private async Task<IEnumerable<SbsmEntity<string>>> CopyEntries(string contentType, IEnumerable<Entry<JsonElement>> entries, IEnumerable<string> languageCodes, string folderId)
		{
			var tasks = new List<Task<SbsmEntity<string>>>();
			foreach (var entry in entries)
			{
				tasks.Add(CopyEntry(contentType, entry, languageCodes, folderId));
			}
			await Task.WhenAll(tasks.ToArray());
			return tasks.Select(task => task.Result);
		}

		private async Task<SbsmEntity<string>> CopyEntry(string contentType, Entry<JsonElement> entry, IEnumerable<string> languageCodes, string folderId) {
			var newEntry = new SbsmEntity<string>
			{
				Entry = new Entry<string>
				{
					Title = $"{entry.Title} - 3",
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
			var newLocales = locales.Where(locale => !languageCodes.Contains(locale.LanguageCode));
			var result = new List<Locale<string>>();
			foreach (var newLocale in newLocales) {
				result.Add(new Locale<string>() {
					LanguageCode = newLocale.LanguageCode,
					Links = newLocale.Links,
					Richtexts = newLocale.Richtexts,
					Texts = newLocale.Texts,
					Images = await CopyImages(newLocale.Images, folderId)
				});
			}
			return result;
		}

		private async Task<IEnumerable<ImageField<string>>> CopyImages(IEnumerable<ImageField<JsonElement>> images, string folderId)
		{
			var tasks = new List<Task<ImageField<string>>>();
			foreach (var image in images)
			{
				tasks.Add(CopyImage(image, folderId));
			}
			await Task.WhenAll(tasks.ToArray());
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