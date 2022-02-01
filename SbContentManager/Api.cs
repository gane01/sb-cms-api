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
		//app.MapGet("/assets", GetAssets);
		//app.MapGet("/assetsFolder", GetAssetFolder);
		//app.MapPost("/assets", UploadAsset);
		//app.MapGet("/assetsRef", GetAssetsRef);
		//
		//app.MapGet("/map", Bar);
	}

    private static async Task<IResult> GetEntry(string templateId, string contentId, IContentstackApi contentStackApi)
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

	private static string GetAssetFolder()
	{
		throw new NotImplementedException();
	}

	private static string GetAssets()
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