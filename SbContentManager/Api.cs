using SbContentManager.ContentstackApi;
using System.Text.Json;
using System.Text.Json.Nodes;
using SbContentManager.Models;
using Microsoft.AspNetCore.Mvc;

public static partial class Api
{
	//private static readonly string betssonCOntentstackLocale = "en";
 //   private static readonly ContentstackClient contentStack = new(
	//	apiKey: "bltc35dce36417d764b", 
	//	deliveryToken: "cs53aab06da08c2b6f4725c1d8", 
	//	environment: "development", 
	//	region: Contentstack.Core.Internals.ContentstackRegion.EU);

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
		app.MapGet("/hello", () => "hello");
	}

    // Generate classes from content type command
    // contentstack.model.generator -a bltc35dce36417d764b -d cs53aab06da08c2b6f4725c1d8 -e eu-cdn.contentstack.com

    private static async Task<IResult> GetEntry(string templateId, string contentId, IContentstackApi contentStackApi)
    {
		try
		{
			// { "$or":[{ "uid": "blt9a09d315775bb3ab"}, { "uid": "blt9ca3f8ff03b970fa"}]}
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
			// { "$or":[{ "uid": "blt9a09d315775bb3ab"}, { "uid": "blt9ca3f8ff03b970fa"}]}
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

    private static async Task<IResult> CreateEntry(string templateId, [FromBody] JsonNode content, IContentstackApi managementApi)
    {
		try
		{
			var result = await managementApi.CreateEntry("development", templateId, content);
			var response = new Response<Data>()
			{
				Message = result["notice"]!.ToString(),
				Data = new Data() { Uid = result["uid"]!.ToString() }
			};
			return Results.Created($"/contents/{templateId}/{result["uid"]}", response);
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