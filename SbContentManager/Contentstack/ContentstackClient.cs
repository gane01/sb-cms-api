using Refit;
using SbContentManager.Contentstack.Publish;
using System.Text.Json;
using System.Linq;

namespace SbContentManager.Contentstack
{
    public class ContentstackClient
    {
		private IContentstackApi contentStackApi;
		public ContentstackClient(IContentstackApi contentStackApi) {
			this.contentStackApi = contentStackApi;
		}

		public async Task<JsonElement> GetEntry(string templateId, string contentId)
		{
			var query = new OrQuery<UidQuery>
			{
				Values = new List<UidQuery>() {
				new UidQuery() { Uid = contentId }
				}
			};

			// TODO Environment string will come from env variable
			return await contentStackApi.GetEntries("development", templateId, JsonSerializer.Serialize(query));
		}

		public async Task<JsonElement> GetEntries(string templateId, IEnumerable<string> contenIds)
		{
			var query = new OrQuery<UidQuery> { Values = new List<UidQuery>() };
            query.Values.AddRange(from contenId in contenIds
                                  select new UidQuery() { Uid = contenId });

            // TODO Environment string will come from env variable
            return await contentStackApi.GetEntries("development", templateId, JsonSerializer.Serialize(query));
		}

		public async Task<Result<UidResult>> CreateEntry(string templateId, JsonElement content)
		{
			// TODO Environment string will come from env variable
			var result = await contentStackApi.CreateEntry("development", templateId, content);
			var uid = result.GetProperty("entry").GetProperty("uid").GetString();
			return new Result<UidResult>()
			{
				Message = result.GetProperty("notice").GetString(),
				Data = new UidResult() { Uid = uid }
			};
		}

		public async Task<Result<string>> DeleteEntry(string templateId, string contentId)
		{
			var result = await contentStackApi.DeleteEntry(templateId, contentId);
			// TODO create a response dto that is just a message? eg ResponseMessageDto?
			return new Result<string>()
			{
				Message = result.GetProperty("notice").GetString()
			};
		}

		public async Task<Result<UidResult>> UpdateEntry(string templateId, string contentId, JsonElement content)
		{
			var result = await contentStackApi.UpdateEntry(templateId, contentId, content);
			return new Result<UidResult>()
			{
				Message = result.GetProperty("notice").GetString(),
				Data = new UidResult() { Uid = result.GetProperty("entry").GetProperty("uid").GetString() }
			};
		}

		public async Task<Result<UidResult>> PublishEntry(string templateId, string contentId)
		{
			// TODO Environment string and locale will come from env variable.
			var result = await contentStackApi.PublishEntry(templateId, contentId, JsonSerializer.SerializeToElement(new PublishEntryDto("development", "en")));
			return new Result<UidResult>()
			{
				Message = result.GetProperty("notice").GetString(),
				Data = new UidResult() { Uid = contentId }
			};
		}

		public async Task<JsonElement> GetAsset(string assetId)
		{
			var query = new OrQuery<AssetQuery>
			{
				Values = new List<AssetQuery>() {				
				new AssetQuery() { Uid = assetId, Locale = "en" } // TODO Locale string will come from env variable
			}
			};

			// TODO Environment string will come from env variable
			return await contentStackApi.GetAssets("development", JsonSerializer.Serialize(query));
		}

		public async Task<JsonElement> GetAssets(IEnumerable<string> assetIds)
		{
			// blt105156a5b14511cf,blt10e1c4b9fe115494
			var query = new OrQuery<AssetQuery>
			{
				Values = new List<AssetQuery>()
			};

            query.Values.AddRange(from assetId in assetIds
                                  select new AssetQuery() { Uid = assetId, Locale = "en" });

            // TODO Environment string will come from env variable
            return await contentStackApi.GetAssets("development", JsonSerializer.Serialize(query));
		}

		public async Task<JsonElement> CreateAsset(MemoryStream file, string folderId, string fileName, string contentType, string title, string description, string tags) {
			var assetByteArray = new ByteArrayPart(file.ToArray(), fileName, contentType);
			return await contentStackApi.CreateAsset(assetByteArray, folderId, title, description, tags);
		}

		public async Task<Result<string>> DeleteAsset(string assetId)
		{
			var result = await contentStackApi.DeleteAsset(assetId);
			return new Result<string>()
			{
				Message = result.GetProperty("notice").GetString()
			};
		}

		public async Task<JsonElement> GetAssetRef(string assetId)
		{
			// blt105156a5b14511cf,blt10e1c4b9fe115494
			return await contentStackApi.GetAssetRef(assetId);
		}

		public async Task<JsonElement> GetAssetFolder(string folderName)
		{
			// testFolder
			var query = new FolderQuery
			{
				IsDir = true,
				Name = folderName
			};

			return await contentStackApi.GetAssetFolder(JsonSerializer.Serialize(query));
		}

		public async Task<Result<UidResult>> PublishAsset(string assetId)
		{
			// TODO Environment string and locale will come from env variable.
			// TODO so we really need to serialize here to json element? try to pass the native object and see if refit can serialize it
			var result = await contentStackApi.PublishAsset(assetId, JsonSerializer.SerializeToElement(new PublishAssetDto("development", "en")));
			return new Result<UidResult>()
			{
				Message = result.GetProperty("notice").GetString(),
				Data = new UidResult() { Uid = assetId }
			};
		}
	}
}
