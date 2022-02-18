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

		public async Task<IEnumerable<string>> Copy(string contentType, IEnumerable<string> entryIds, string folderId, IEnumerable<string> languageCodes)
		{
			// Get get entries
			var entries = await contentstackClient.GetEntries(contentType, entryIds);
			// Get entry assets pairs: { entryId:assetId }
			var entryAssetsPairs = GetEntryAssets(entries);
			// Get copied asset pairs: { assetId:copiedAssetId }
			var assetIdPairs = await assetEffect.Copy(entryAssetsPairs.Select(x => x.Value).Distinct(), folderId);
			// create new entries with new assets
			var entryIdPairs = CreateEntry(entries, languageCodes, entryAssetsPairs, assetIdPairs);
			// publish entries
			// publish assets
			// return ids of created entries
			return new List<string>();
		}
        private IEnumerable<JsonElement> CreateEntry(JsonElement entries, IEnumerable<string> languageCodes, IEnumerable<KeyValuePair<string, string>> entryAssetsPairs, IEnumerable<KeyValuePair<string, string>> assetIdPairs)
        {
			var result = new List<JsonElement>();

			foreach (var entry in entries.GetProperty("entries").EnumerateArray()) {
				var entryObj = JsonNode.Parse(@"{}");
				(entryObj as JsonObject).Add("entry", JsonSerializer.Deserialize<JsonNode>(entry));
				RemoveLanguages(languageCodes, ref entryObj);
				UpdateAssets(entryAssetsPairs, ref entryObj);
				result.Add(entryObj.Deserialize<JsonElement>());
			}

			return result;
        }

        private IEnumerable<KeyValuePair<string, string>> GetEntryAssets(JsonElement entries) {
			var result = new List<KeyValuePair<string, string>>();

			// Todo EnumerateArray is a disposable resource. should it use a using block?
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

		private void RemoveLanguages(IEnumerable<string> languageCodes, ref JsonNode entry) 
		{
			var removeLanguages = entry["entry"]["locales"].AsArray()
				.Where(locale => languageCodes.Contains(locale["language_code"].ToString()))
				.ToList();

			foreach (var language in removeLanguages) {
				entry["entry"]["locales"]!.AsArray().Remove(language);
			}
		}
		private void UpdateAssets(IEnumerable<KeyValuePair<string, string>> entryAssetsPairs, ref JsonNode entry)
		{
			foreach (var locale in entry["entry"]["locales"].AsArray())
			{
				foreach (var image in locale["images"].AsArray())
				{
					var entryAssetsPair = entryAssetsPairs.FirstOrDefault(x => x.Key.Equals(image["image"]["uid"].ToString()));
					if (String.IsNullOrEmpty(entryAssetsPair.Value))
					{
						throw new Exception($"Failed to find the copied asset uid for entry image: {entry["entry"]["uid"]}");
					}
					else
					{
						// Create JsonELement from JsonObject
						// https://www.c-sharpcorner.com/article/new-programming-model-for-handling-json-in-net-6/
						//image.Image = JsonSerializer.Deserialize<JsonElement>(new JsonObject { ["image"] = copiedAssetId.Value })
						image["image"] = entryAssetsPair.Value;
					}
				};
			}
		}
	}
}