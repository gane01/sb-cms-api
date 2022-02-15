using SbContentManager.Asset;
using SbContentManager.Contentstack;
using System.Text.Json;

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

		public async Task<IEnumerable<string>> Copy(string contentType, IEnumerable<string> entryIds, string folderId, IEnumerable<string> languageCodes)
		{
			// Get get entries
			var entries = await contentstackClient.GetEntries(contentType, entryIds);
			// Get entry assets pairs: { entryId:assetId }
			var entryAssetsPairs = GetEntryAssets(entries);
			// Get copied asset pairs: { assetId:copiedAssetId }
			var assetIdPairs = await assetEffect.Copy(entryAssetsPairs.Select(x => x.Value).Distinct(), folderId);
			// create new entries with new assets
			// publish entries
			// publish assets
			// return ids of created entries
			return new List<string>();
		}

		private IEnumerable<KeyValuePair<string, string>> GetEntryAssets(JsonElement entries) {
			var result = new List<KeyValuePair<string, string>>();

			// Todo EnumerateArray is a disposable resource. should i use a using block?
			// Todo flatten this nested foreach loop
			foreach (var entry in entries.GetProperty("entries").EnumerateArray()) 
			{
				foreach (var locale in entry.GetProperty("locales").EnumerateArray())
				{
					if (locale.TryGetProperty("images", out JsonElement images))
					{
						foreach (var image in images.EnumerateArray())
						{
							var entryId = entry.GetProperty("uid").GetString();
							var imageElement = image.GetProperty("image");
							if (imageElement.ValueKind == JsonValueKind.Null)
							{
								throw new KeyNotFoundException($"Image element not found for entry: {entryId}");
							}
							else 
							{
								var assetId = imageElement.GetProperty("uid").GetString();
								result.Add(new KeyValuePair<string, string>(entryId, assetId));
							}
						}
					}
				}
            }

			return result;
		}
	}
}