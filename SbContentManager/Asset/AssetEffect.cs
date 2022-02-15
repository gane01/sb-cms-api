using SbContentManager.Contentstack;
using System.Text.Json;

namespace SbContentManager.Asset
{
	public class AssetEffect
	{
		private HttpClient httpClient;
		private ContentstackClient contentstackClient;
		public AssetEffect(HttpClient httpClient, ContentstackClient contentstackClient)
		{
			this.httpClient = httpClient;
			this.contentstackClient = contentstackClient;
		}

		public async Task<IEnumerable<string>> Copy(IEnumerable<string> assetIds, string folderId)
		{
			try
			{
				var assets = await GetAssets(assetIds);
				var newAssets = await CopyAssets(assets, folderId);
				await PublishAssets(newAssets);
				return newAssets;
			}
			catch (Exception e)
			{
				throw new Exception("Asset copy failed.", e);
			}
		}

		private async Task<IEnumerable<AssetModel>> GetAssets(IEnumerable<string> assetIds)
		{
			var assets = await contentstackClient.GetAssets(String.Join(",", assetIds));

			return assets.GetProperty("assets").EnumerateArray().Select((asset) =>
			{
				// 'description' can be missing from the asset JSON if uploaded from Contentstack web admin.
				JsonElement descriptionElement;
				return new AssetModel
				{
					Title = asset.GetProperty("title").GetString(),
					Description = asset.TryGetProperty("description", out descriptionElement) ? string.Empty : descriptionElement.GetString(),
					FileName = asset.GetProperty("filename").GetString(),
					ContentType = asset.GetProperty("content_type").GetString(),
					Url = asset.GetProperty("url").GetString(),
					Tags = asset.GetProperty("tags").EnumerateArray().Select((tag) => tag.GetString()).ToArray()
				};
			});
		}

		private async Task<IEnumerable<string>> CopyAssets(IEnumerable<AssetModel> assets, string folderId)
		{
			var tasks = new List<Task<string>>();
			foreach (var asset in assets)
			{
				tasks.Add(UploadAsset(folderId, asset));
			}
			await Task.WhenAll(tasks.ToArray());
			return tasks.Select(task => task.Result);
		}

		private async Task<string> UploadAsset(string folderId, AssetModel asset)
		{
			using var downloadFileStream = (await httpClient.GetAsync(asset.Url)).Content.ReadAsStream();
			using var uploadFileStream = new MemoryStream();
			downloadFileStream.CopyTo(uploadFileStream);
			var result = await this.contentstackClient.CreateAsset(uploadFileStream, folderId, asset.FileName, asset.ContentType, asset.Title, asset.Description, string.Join(",", asset.Tags));
			return result.GetProperty("asset").GetProperty("uid").GetString();
		}

		private async Task PublishAssets(IEnumerable<string> assetIds)
		{
			foreach (var assetId in assetIds)
			{
				await this.contentstackClient.PublishAsset(assetId);
			}
		}

		// folderId: bltd597a35241e12bff
		// assets:
		// blt965b14e0ab5190a7
		// blt90a946c525f6c91e
	}
}