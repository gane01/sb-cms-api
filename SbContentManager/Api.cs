using SbContentManager.ContentstackApi;
using System.Text.Json;
using System.Text.Json.Nodes;
using SbContentManager.Models;
using Microsoft.AspNetCore.Mvc;

public static partial class Api
{
	public static void ConfigureApi(this WebApplication app)
	{
		app.MapGet("/contents/{templateId}/{contentId}", GetEntry); // return one or multiple entries
		app.MapGet("/contents/{templateId}", GetEntries);
		app.MapPost("/contents/{templateId}", CreateEntry);
		//app.MapPut("/contents", UpdateEntry);
		//app.MapGet("/assets", GetAssets);
		//app.MapGet("/assetsFolder", GetAssetFolder);
		//app.MapPost("/assets", UploadAsset);
		//app.MapGet("/assetsRef", GetAssetsRef);
		//app.MapPost("/publish", Publish);
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
			var result = await managementApi.CreateEntry("development", templateId, content);
			var uid = result.GetProperty("entry").GetProperty("uid").GetString();
			var response = new Response<Data>()
			{
				Message = result.GetProperty("notice").GetString(),
				Data = new Data() { Uid = uid }
			};
			return Results.Created($"/contents/{templateId}/{uid}", response);
		}
		catch (Exception e) {
			return Results.Problem(e.Message);
		}	
    }

    private static string UpdateEntry()
	{
		throw new NotImplementedException();
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

	private static string Publish()
	{
		throw new NotImplementedException();
	}
}