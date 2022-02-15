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
			// get entry assets
			IEnumerable<KeyValuePair<string, string>> entryAssets = GetEntryAssets(entries);
			// duplicate assets
			//assetIds = entryAssets.Dis(x => x);
			//var result = await assetEffect.Copy(entryAssets.Distinct(), folderId);
			// create new entries with new assets
			// publish entries
			// publish assets
			// return ids of created entries
			return new List<string>();
		}

		private IEnumerable<KeyValuePair<string, string>> GetEntryAssets(JsonElement entries) {
			var result = new List<KeyValuePair<string, string>>();

			// Todo EnumerateArray is a disposable resource. should i use a using block?
			foreach (var entry in entries.GetProperty("entries").EnumerateArray()) {
				if (entry.TryGetProperty("images", out JsonElement images)) {
					foreach (var image in images.EnumerateArray()) {
						var entryId = entry.GetProperty("uid").GetString();
						var assetId = image.GetProperty("image").GetProperty("uid").GetString();
						result.Add(new KeyValuePair<string, string>(entryId, assetId));
					}
				}
            }

			return result;
		}
	}
}