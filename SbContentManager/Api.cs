using SbContentManager.ContentstackApi;
using System.Text.Json;
using SbContentManager.Models;
using Microsoft.AspNetCore.Mvc;
using Refit;

public static partial class Api
{
	public static void ConfigureApi(this WebApplication app)
	{
		app.MapGet("/contents/{templateId}/{contentId}", GetEntry); // return one or multiple entries
		app.MapGet("/contents/{templateId}", GetEntries);
		app.MapPost("/contents/{templateId}", CreateEntry);		
		app.MapMethods("/contents/{templateId}/{contentId}", new[] { "PATCH" }, UpdateEntry); // There`s no MapPatch in minimal api, so create one
		app.MapPost("/publish", PublishEntry);
		app.MapGet("/assets/{assetId}", GetAsset);
		app.MapGet("/assets/", GetAssets);
		app.MapPost("/assets/", CreateAsset);
		//app.MapGet("/assetsFolder", GetAssetFolder);
		//app.MapPost("/assets", UploadAsset);
		//app.MapGet("/assetsRef", GetAssetsRef);
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
			var response = new Response<Data>()
			{
				Message = result.GetProperty("notice").GetString(),
				Data = new Data() { Uid = uid }
			};
			// TODO add full url here
			return Results.Created($"/contents/{templateId}/{uid}", response);
		}
		catch (Exception e) {
			return Results.Problem(e.Message);
		}	
    }

	private static async Task<IResult> UpdateEntry(string templateId, string contentId, [FromBody] JsonElement content, IContentstackApi managementApi)
	{
		try
		{
			var result = await managementApi.UpdateEntry(templateId, contentId, content);
			var response = new Response<Data>()
			{
				Message = result.GetProperty("notice").GetString(),
				Data = new Data() { Uid = result.GetProperty("entry").GetProperty("uid").GetString() }
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

	private static async Task<IResult> CreateAsset(HttpRequest request)
	{
		// parent folder: blt2a46ed8f1b1d5979
		try
		{

			if (!request.HasFormContentType)
			{
				return Results.BadRequest();
			}

			var form = await request.ReadFormAsync();
			var file = form.Files["uploadedFile"];
			var title = form["title"];

			/*
			using var ms = new MemoryStream();
			formFile.CopyTo(ms);
			ByteArrayPart foo = new ByteArrayPart(ms.ToArray(), formFile.FileName);
			var result = await contentStackApi.CreateAsset(foo, folderId, title, description, new string[] { "d" });
			return Results.Ok(result);
			*/
			await Task.Delay(1000);
			return Results.Ok($"{file.FileName} - {title}");
		}
		catch (ApiException e) {
			return Results.Problem(statusCode: (int?)e.StatusCode, detail: e.Message);
		}
		catch (Exception e) {
			return Results.Problem(e.Message);
		}		
	}

	private static string GetAssetFolder()
	{
		throw new NotImplementedException();
	}

	private static string UploadAsset()
	{
		throw new NotImplementedException();
	}

	private static string GetAssetsRef()
	{
		throw new NotImplementedException();
	}

	private static async Task<IResult> PublishEntry(string templateId, string contentId, [FromBody] JsonElement publish, IContentstackApi managementApi)
	{
		try
		{
			var result = await managementApi.PublishEntry("development", templateId, contentId, publish);
			var response = new Response<Data>()
			{
				Message = result.GetProperty("notice").GetString(),
				Data = new Data() { Uid = contentId }
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