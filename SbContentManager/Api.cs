using System.Collections.Generic;
using System.Collections;
using SbContentManager.ContentstackApi;
using System.Text.Json;
using System.Text.Json.Nodes;

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
		//app.MapGet("/contents/{templateId}/{contentId}", GetEntry); // return one or multiple entries
		//app.MapGet("/contents/{templateId}", GetEntries);
		app.MapPost("/contents", CreateEntry);
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

	//private static async Task<IResult> GetEntry(string templateId, string contentId)
	//{
	//	// templateId: test_article
	//	// contentId: blt9ca3f8ff03b970fa
	//	Entry entry = contentStack.ContentType(templateId).Entry(contentId);		
	//	try
	//	{
	//		return Results.Ok(await entry.Fetch<string>());
	//	}
	//	catch (ContentstackException ex) {
	//		return Results.Problem(ex.Message);
	//	}
	//	catch (Exception ex)
	//	{
	//		return Results.Problem(ex.Message);
	//	}
	//}

	//private static async Task<IResult> GetEntries(string templateId, string contenIds)
	//{
	//	// templateId: test_article
	//	// contentId: blt9ca3f8ff03b970fa,bltf27fe7d1dcc6b6d7
	//	ContentType contentType = contentStack.ContentType(templateId);
	//	Query query = contentType.Query();
	//	string[] ids = contenIds.Split(',');

	//	query.SetLocale(betssonCOntentstackLocale);
	//	List<Query> queryList = new();
	//	foreach (var id in ids) {
	//		queryList.Add(contentType.Query().Where("uid", id));
	//	}
	//	query.Or(queryList);

	//	try
	//	{
	//		return Results.Ok(await query.Find<TestArticle>());
	//	}
	//	catch (ContentstackException ex)
	//	{
	//		return Results.Problem(ex.Message);
	//	}
	//	catch (Exception ex)
	//	{
	//		return Results.Problem(ex.Message);
	//	}
	//}

	private static async Task<IResult> CreateEntry(string testArticle, IContentstackApi managementApi)
	{
		string ddd = @"{
			""entry"": {
				""title"": ""Article 12"",
        ""body"": ""Article 12 description"",
        ""tags"": [""test""]
        }
		}";
		await managementApi.CreateEntry("development", "content type", ddd);
		return Results.Ok("hello");
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