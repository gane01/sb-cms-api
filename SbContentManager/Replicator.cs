using SbContentManager;
using SbContentManager.Contentstack;
using System.Text.Json;

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
		var result = await CopyAssets(assets, folderId);
		return result.ToArray();
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

	private async Task<IEnumerable<string>> CopyAssets(IEnumerable<AssetModel> assets, string folderId)
	{
		var tasks = new List<Task<string>>();
		foreach (var asset in assets) {
			tasks.Add(Foo(folderId, asset));
		}
		await Task.WhenAll(tasks.ToArray());
		return tasks.Select(task => task.Result);
	}

	private async Task<string> Foo(string folderId, AssetModel asset) {
		using var downloadFileStream = (await httpClient.GetAsync(asset.Url)).Content.ReadAsStream();
		using var uploadFileStream = new MemoryStream();
		downloadFileStream.CopyTo(uploadFileStream);
		var result = await this.contentstackClient.CreateAsset(uploadFileStream, folderId, asset.FileName, asset.ContentType, asset.Title, asset.Description, string.Join(",", asset.Tags));
		return result.GetProperty("asset").GetProperty("uid").GetString();
	}

	// folderId: bltd597a35241e12bff
	// assets:
	// blt965b14e0ab5190a7
	// blt90a946c525f6c91e
}