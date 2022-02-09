﻿using SbContentManager.ContentstackClient;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Refit;
using SbContentManager.ContentstackClient.Publish;

public static partial class Api
{
	public static void ConfigureApi(this WebApplication app)
	{
		app.MapGet("/contents/{templateId}/{contentId}", GetEntry);
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

	private static async Task<IResult> GetEntry(string templateId, string contentId, ContentstackClient contentstackClient)
    {
		try
		{
			return Results.Ok(await contentstackClient.GetEntry(templateId, contentId));
		}
		catch (ApiException e)
		{
			return Results.Problem(statusCode: (int?)e.StatusCode, detail: e.Message);
		}
		catch (Exception e) {
			return Results.Problem(e.Message);
		}
    }

    private static async Task<IResult> GetEntries(string templateId, string contenIds, ContentstackClient contentstackClient)
    {
		try
		{
			return Results.Ok(await contentstackClient.GetEntries(templateId, contenIds));
		}
		catch (ApiException e)
		{
			return Results.Problem(statusCode: (int?)e.StatusCode, detail: e.Message);
		}
		catch (Exception e) {
			return Results.Problem(e.Message);
		}
    }

    private static async Task<IResult> CreateEntry(string templateId, [FromBody] JsonElement content, ContentstackClient contentstackClient)
    {
		try
		{
			var result = await contentstackClient.CreateEntry(templateId, content);
			// TODO add full url here
			return Results.Created($"/contents/{templateId}/{result.Data!.Uid}", result);
		}
		catch (ApiException e)
		{
			return Results.Problem(statusCode: (int?)e.StatusCode, detail: e.Message);
		}
		catch (Exception e) {
			return Results.Problem(e.Message);
		}	
    }

	private static async Task<IResult> DeleteEntry(string templateId, string contentId, ContentstackClient contentstackClient)
	{
		try
		{
			return Results.Ok(await contentstackClient.DeleteEntry(templateId, contentId));
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

	private static async Task<IResult> UpdateEntry(string templateId, string contentId, [FromBody] JsonElement content, ContentstackClient contentstackClient)
	{
		try
		{
			return Results.Ok(await contentstackClient.UpdateEntry(templateId, contentId, content));
		}
		catch (ApiException e) {
			return Results.Problem(statusCode: (int?)e.StatusCode, detail: e.Message);
		}
		catch (Exception e)
		{
			return Results.Problem(e.Message);
		}
	}

	private static async Task<IResult> PublishEntry(string templateId, string contentId, ContentstackClient contentstackClient)
	{
		try
		{
			// TODO Environment string and locale will come from env variable.
			return Results.Ok(await contentstackClient.PublishEntry(templateId, contentId));
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

	private static async Task<IResult> GetAsset(string assetId, ContentstackClient contentstackClient)
	{
		try
		{
			return Results.Ok(await contentstackClient.GetAsset(assetId));
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

	private static async Task<IResult> GetAssets(string assetIds, ContentstackClient contentstackClient)
	{
		// blt105156a5b14511cf,blt10e1c4b9fe115494
		try
		{
			return Results.Ok(await contentstackClient.GetAssets(assetIds));
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

	private static async Task<IResult> CreateAsset(HttpRequest request, ContentstackClient contentstackClient)
	{
		// parent folder: blt2a46ed8f1b1d5979
		try
		{
			if (!request.HasFormContentType) { return Results.BadRequest(); }
			var formCollection = await request.ReadFormAsync();
			return Results.Ok(await contentstackClient.CreateAsset(formCollection));
		}
		catch (ApiException e) {
			return Results.Problem(statusCode: (int?)e.StatusCode, detail: e.Message);
		}
		catch (Exception e) {
			return Results.Problem(e.Message);
		}		
	}

	private static async Task<IResult> DeleteAsset(string assetId, ContentstackClient contentstackClient)
	{
		try
		{
			return Results.Ok(await contentstackClient.DeleteAsset(assetId));
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

	private static async Task<IResult> GetAssetRef(string assetId, ContentstackClient contentstackClient)
	{
		try
		{
			return Results.Ok(await contentstackClient.GetAssetRef(assetId));
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

	private static async Task<IResult> GetAssetFolder(string folderName, ContentstackClient contentstackClient)
	{
		// testFolder
		try
		{
			return Results.Ok(await contentstackClient.GetAssetFolder(folderName));
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

	private static async Task<IResult> PublishAsset(string assetId, ContentstackClient contentstackClient)
	{
		try
		{
			return Results.Ok(await contentstackClient.PublishAsset(assetId));
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