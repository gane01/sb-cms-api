using Refit;
using SbContentManager.ContentstackClient;
using System.Text.Json;

namespace SbContentManager.ContentManager
{
    public class ContentManager
    {
        public MemoryStream DownloadAsset(string assetId) {
			//GetAsset(assetId)

			return new MemoryStream();
        }

		private static async Task<JsonElement> GetAssetUrl(string assetId, IContentstackApi contentStackApi) {
			try
			{
				var query = new OrQuery<AssetQuery>
				{
					Values = new List<AssetQuery>() {					
						new AssetQuery() { Uid = assetId, Locale = "en" }
					}
				};

				// TODO Environment string will come from env variable
				var result = await contentStackApi.GetAssets("development", JsonSerializer.Serialize(query));
				return result;
			}
			catch (ApiException e)
			{
				throw e;
			}
			catch (Exception)
			{
                throw;
            }
		}
    }
}