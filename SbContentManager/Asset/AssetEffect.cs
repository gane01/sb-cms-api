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

		public async Task<string> Copy(AssetModel asset, string folderId)
		{
			using var downloadFileStream = (await httpClient.GetAsync(asset.Url)).Content.ReadAsStream();
			using var uploadFileStream = new MemoryStream();
			downloadFileStream.CopyTo(uploadFileStream);
			var result = await this.contentstackClient.CreateAsset(uploadFileStream, folderId, asset.FileName, asset.ContentType, asset.Title, asset.Description, string.Join(",", asset.Tags));
			// Return the id of the original asset and its newly duplicated asset id.
			return result.GetProperty("asset").GetProperty("uid").GetString();
		}

		private async Task PublishAssets(IEnumerable<string> assetIds)
		{
			foreach (var assetId in assetIds)
			{
				await this.contentstackClient.PublishAsset(assetId);
			}
		}
	}
}