using SbContentManager.ContentstackApi;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Refit;
using SbContentManager.ContentstackApi.Publish;
using SbContentManager.Response;

public static partial class Api
{
	public static void ConfigureApi(this WebApplication app)
	{
		app.MapGet("/contents/{templateId}/{contentId}", GetEntry); // return one or multiple entries
		app.MapGet("/contents/{templateId}", GetEntries);
		app.MapPost("/contents/{templateId}", CreateEntry);
		app.MapDelete("/contents/{templateId}/{contentId}", DeleteEntry);
		app.MapMethods("/contents/{templateId}/{contentId}", new[] { "PATCH" }, UpdateEntry); // There`s no MapPatch in minimal api, so create one
		app.MapPost("/contents/publish", PublishEntry);
		app.MapGet("/assets/{assetId}", GetAsset);
		app.MapGet("/assets/", GetAssets);
		app.MapPost("/assets/", CreateAsset);
		app.MapDelete("/assets/{assetId}", DeleteAsset);
		app.MapGet("/assets/{assetId}/ref", GetAssetRef);
		app.MapGet("/assets/folder/{folderName}", GetAssetFolder);
		app.MapPost("/assets/publish/{assetId}", PublishAsset);
		// Todo: add asset/entry bulk publish,
		// Todo: add asset/entry bulk copy,
	}

	/// <summary>Retrieves a specific product by unique id</summary>
	/// <remarks>Awesomeness</remarks>
	/// <param name="templateId" example="123">The product id</param>
	/// <param name="contentId" example="123">The product id</param>
	/// <response code="200">Product retrieved</response>
	/// <response code="404">Product not found</response>
	/// <response code="500">Oops! Can't lookup your product right now</response>

#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
	[Microsoft.AspNetCore.Mvc.ProducesResponseType(StatusCodes.Status100Continue)]
	private static async Task<IResult> GetEntry(string templateId, string contentId, IContentstackApi contentStackApi)
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
    {
		try
		{
			var query = new OrQuery<UidQuery>
			{
				Values = new List<UidQuery>() {
					new UidQuery() { Uid = contentId }
				}
			};

			// TODO Environment string will come from env variable
			var result = await contentStackApi.GetEntries("development", templateId, JsonSerializer.Serialize(query));

			return Results.Ok(result);
		}
		catch (Exception e) {
			return Results.Problem(e.Message);
		}
    }

    private static async Task<IResult> GetEntries(string templateId, string contenIds, IContentstackApi contentStackApi)
    {
		try
		{
			var query = new OrQuery<UidQuery>
			{
				Values = new List<UidQuery>()
			};

			contenIds.Split(",").ToList().ForEach(contenId => 
				query.Values.Add(new UidQuery() { Uid = contenId })
			);

			// TODO Environment string will come from env variable
			var result = await contentStackApi.GetEntries("development", templateId, JsonSerializer.Serialize(query));

			return Results.Ok(result);
		}
		catch (Exception e) {
			return Results.Problem(e.Message);
		}
    }

    private static async Task<IResult> CreateEntry(string templateId, [FromBody] JsonElement content, IContentstackApi managementApi)
    {
		try
		{
			// TODO Environment string will come from env variable
			var result = await managementApi.CreateEntry("development", templateId, content);
			var uid = result.GetProperty("entry").GetProperty("uid").GetString();
			var response = new ResponseDto<ResponseDetailsUidDto>()
			{
				Message = result.GetProperty("notice").GetString(),
				Data = new ResponseDetailsUidDto() { Uid = uid }
			};
			// TODO add full url here
			return Results.Created($"/contents/{templateId}/{uid}", response);
		}
		catch (Exception e) {
			return Results.Problem(e.Message);
		}	
    }

	private static async Task<IResult> DeleteEntry(string templateId, string contentId, IContentstackApi managementApi)
	{
		try
		{
			var result = await managementApi.DeleteEntry(templateId, contentId);
			// TODO create a response dto that is just a message? eg ResponseMessageDto?
			var response = new ResponseDto<string>()
			{
				Message = result.GetProperty("notice").GetString(),
				Data = string.Empty
			};

			return Results.Ok(response);
		}
		catch (ApiException e)
		{
			return Results.Problem(statusCode: (int?)e.StatusCode, detail: e.Message);
		}
		catch (Exception e)
		{
			return Results.Problem(e.Message);
		}
	}

	private static async Task<IResult> UpdateEntry(string templateId, string contentId, [FromBody] JsonElement content, IContentstackApi managementApi)
	{
		try
		{
			var result = await managementApi.UpdateEntry(templateId, contentId, content);
			var response = new ResponseDto<ResponseDetailsUidDto>()
			{
				Message = result.GetProperty("notice").GetString(),
				Data = new ResponseDetailsUidDto() { Uid = result.GetProperty("entry").GetProperty("uid").GetString() }
			};
			return Results.Ok(response);
		}
		catch (ApiException e) {
			return Results.Problem(statusCode: (int?)e.StatusCode, detail: e.Message);
		}
		catch (Exception e)
		{
			return Results.Problem(e.Message);
		}
	}

	private static async Task<IResult> PublishEntry(string templateId, string contentId, IContentstackApi managementApi)
	{
		try
		{
			// TODO Environment string and locale will come from env variable.
			var result = await managementApi.PublishEntry(templateId, contentId, JsonSerializer.SerializeToElement(new PublishAssetDto("development", "en")));
			var response = new ResponseDto<ResponseDetailsUidDto>()
			{
				Message = result.GetProperty("notice").GetString(),
				Data = new ResponseDetailsUidDto() { Uid = contentId }
			};
			return Results.Ok(response);
		}
		catch (ApiException e)
		{
			return Results.Problem(statusCode: (int?)e.StatusCode, detail: e.Message);
		}
		catch (Exception e)
		{
			return Results.Problem(e.Message);
		}
	}

	private static async Task<IResult> GetAsset(string assetId, IContentstackApi contentStackApi)
	{
		try
		{
			var query = new OrQuery<AssetQuery>
			{
				Values = new List<AssetQuery>() {
					// TODO Do we going to have only one locale ("en", shouldn`t it be more specific? eg en-us)?
					new AssetQuery() { Uid = assetId, Locale = "en" }
				}
			};

			// TODO Environment string will come from env variable
			var result = await contentStackApi.GetAssets("development", JsonSerializer.Serialize(query));
			return Results.Ok(result);
		}
		catch (ApiException e)
		{
			return Results.Problem(statusCode: (int?)e.StatusCode, detail: e.Message);
		}
		catch (Exception e)
		{
			return Results.Problem(e.Message);
		}
	}

	private static async Task<IResult> GetAssets(string assetIds, IContentstackApi contentStackApi)
	{
		// blt105156a5b14511cf,blt10e1c4b9fe115494
		try
		{
			var query = new OrQuery<AssetQuery>
			{
				Values = new List<AssetQuery>()
			};

			assetIds.Split(",").ToList().ForEach(assetId =>
				query.Values.Add(new AssetQuery() { Uid = assetId, Locale = "en" })
			);

			// TODO Environment string will come from env variable
			var result = await contentStackApi.GetAssets("development", JsonSerializer.Serialize(query));

			return Results.Ok(result);
		}
		catch (Exception e)
		{
			return Results.Problem(e.Message);
		}
	}

	private static async Task<IResult> CreateAsset(HttpRequest request, IContentstackApi contentStackApi)
	{
		// parent folder: blt2a46ed8f1b1d5979
		try
		{

			if (!request.HasFormContentType)
			{
				return Results.BadRequest();
			}

			var form = await request.ReadFormAsync();
			var file = form.Files["asset"];
			var title = form["title"];
			var description = form["description"];
			var tags = form["tags"];
			var folderId = form["folderId"];

			using var memoryStream = new MemoryStream();
			file!.CopyTo(memoryStream);
			var byteArray = new ByteArrayPart(memoryStream.ToArray(), file.FileName, file.ContentType);

			var result = await contentStackApi.CreateAsset(byteArray, folderId, title, description, tags);
			return Results.Ok(result);
		}
		catch (ApiException e) {
			return Results.Problem(statusCode: (int?)e.StatusCode, detail: e.Message);
		}
		catch (Exception e) {
			return Results.Problem(e.Message);
		}		
	}

	private static async Task<IResult> DeleteAsset(string assetId, IContentstackApi contentStackApi)
	{
		try
		{
			var result = await contentStackApi.DeleteAsset(assetId);
			// TODO use our on responsedeto to return the response message
			return Results.Ok(result);
		}
		catch (ApiException e)
		{
			return Results.Problem(statusCode: (int?)e.StatusCode, detail: e.Message);
		}
		catch (Exception e)
		{
			return Results.Problem(e.Message);
		}
	}

	private static async Task<IResult> GetAssetRef(string assetId, IContentstackApi contentStackApi)
	{
		// blt105156a5b14511cf,blt10e1c4b9fe115494
		try
		{		
			// TODO Environment string will come from env variable
			var result = await contentStackApi.GetAssetRef(assetId);

			return Results.Ok(result);
		}
		catch (ApiException e)
		{
			return Results.Problem(statusCode: (int?)e.StatusCode, detail: e.Message);
		}
		catch (Exception e)
		{
			return Results.Problem(e.Message);
		}
	}

	private static async Task<IResult> GetAssetFolder(string folderName, IContentstackApi contentStackApi)
	{
		// testFolder
		try
		{
			var query = new FolderQuery
			{
				IsDir = true,
				Name = folderName
			};

			// TODO Environment string will come from env variable
			var result = await contentStackApi.GetAssetFolder(JsonSerializer.Serialize(query));
			return Results.Ok(result);
		}
		catch (ApiException e)
		{
			return Results.Problem(statusCode: (int?)e.StatusCode, detail: e.Message);
		}
		catch (Exception e)
		{
			return Results.Problem(e.Message);
		}
	}

	private static async Task<IResult> PublishAsset(string assetId, IContentstackApi contentStackApi)
	{
		try
		{
			// TODO Environment string and locale will come from env variable.
			// TODO so we really need to serialize here to json element? try to pass the native object and see if refit can serialize it
			var result = await contentStackApi.PublishAsset(assetId, JsonSerializer.SerializeToElement(new PublishAssetDto("development", "en")));
			var response = new ResponseDto<ResponseDetailsUidDto>()
			{
				Message = result.GetProperty("notice").GetString(),
				Data = new ResponseDetailsUidDto() { Uid = assetId }
			};
			return Results.Ok(response);
		}
		catch (ApiException e)
		{
			return Results.Problem(statusCode: (int?)e.StatusCode, detail: e.Message);
		}
		catch (Exception e)
		{
			return Results.Problem(e.Message);
		}
	}
}