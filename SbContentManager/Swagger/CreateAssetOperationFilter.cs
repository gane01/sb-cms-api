using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SbContentManager.Swagger
{
    public class CreateAssetOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) operation.Parameters = new List<OpenApiParameter>();

            if (context.ApiDescription.HttpMethod == HttpMethod.Post.ToString() && 
                context.ApiDescription.RelativePath== "assets/") {
                    operation.Parameters.Clear();
                    var uploadFileMediaType = new OpenApiMediaType()
                    {
                        Schema = new OpenApiSchema()
                        {
                            Type = "object",
                            Properties = {
                                ["asset"] = new OpenApiSchema() {
                                    Description = "Upload asset",
                                    Type = "file",
                                    Format = "binary" },
                                ["title"] = new OpenApiSchema() {
                                    Title = "Title",
                                    Description = "Asset title",
                                    Type = "string"
                                },
                                ["description"] = new OpenApiSchema() {
                                    Title = "Description",
                                    Description = "Asset desctiption",
                                    Type = "string"
                                },
                                ["tags"] = new OpenApiSchema() {
                                    Title = "Tags",
                                    Description = "Asset tags sepadated by commas",
                                    Type = "string"
                                },
                                ["folderId"] = new OpenApiSchema() {
                                    Title = "Parent folder id",
                                    Description = "The id of the parent folder of this asset",
                                    Type = "string"
                                }
                            },
                            Required = new HashSet<string>() { "asset" }
                        }
                    };
                    operation.RequestBody = new OpenApiRequestBody
                    {
                        Content =
                        {
                            ["multipart/form-data"] = uploadFileMediaType
                        }
                    };
            }


            Console.WriteLine("ff");
        }
    }
}
