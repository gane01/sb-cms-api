using SbContentManager;
using SbContentManager.Contentstack;

public class Replicator
{
	private HttpClient httpClient;
	private ContentstackClient contentstackClient;
	public Replicator(HttpClient httpClient, ContentstackClient contentstackClient)
    {
		this.httpClient = httpClient;
		this.contentstackClient = contentstackClient;
    }

	public async Task<string[]> CopyAsset(string[] assetIds, string folderId) {
		var assets = await GetAssets(assetIds);
		Copy(assets, folderId);
		return new string[] { "fff" };
    }

	private async Task<IEnumerable<AssetModel>> GetAssets(string[] assetIds) {
		var assets = await contentstackClient.GetAssets(String.Join(",", assetIds));

		return assets.GetProperty("assets").EnumerateArray().Select((asset) => {
			// 'description' can be missing from the asset if uploaded from the contentstack web admin
			string description;
			try
			{
				description = asset.GetProperty("description").GetString();
			}
			catch { 
				description = String.Empty;
			}

			return new AssetModel
			{
				Title = asset.GetProperty("title").GetString(),
				Description = description,
				FileName = asset.GetProperty("filename").GetString(),
				ContentType = asset.GetProperty("content_type").GetString(),
				Url = asset.GetProperty("url").GetString(),
				Tags = asset.GetProperty("tags").EnumerateArray().Select((tag) => tag.GetString()).ToArray()
			};
		});
	}

	private void Copy(IEnumerable<AssetModel> assets, string folderId)
	{
		List<Task> tasks = new();
		foreach (var asset in assets) {
			// bltd597a35241e12bff
			var task = new Task(async () => {
				using var downloadFileStream = (httpClient.GetAsync(asset.Url)).Result.Content.ReadAsStream();
				using var uploadFileStream = new MemoryStream();
				downloadFileStream.CopyTo(uploadFileStream);

				await this.contentstackClient.CreateAsset(
					uploadFileStream, folderId, asset.FileName, asset.ContentType, asset.Title, asset.Description, string.Join(",", asset.Tags));
			});

			tasks.Add(task);
			task.Start();
		}
		try
		{
			Task.WhenAll(tasks.ToArray());
			return;
		}
		catch (Exception) {
			throw;
		}
	}
}