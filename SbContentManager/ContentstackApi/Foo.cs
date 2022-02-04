using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SbContentManager.ContentstackApi
{
    public class Foo : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) operation.Parameters = new List<OpenApiParameter>();

            if (context.ApiDescription.HttpMethod == HttpMethod.Post.ToString() && 
                context.ApiDescription.RelativePath== "assets/") {

                // -------------------------------

                    operation.Parameters.Clear();
                    var uploadFileMediaType = new OpenApiMediaType()
                    {
                        Schema = new OpenApiSchema()
                        {
                            Type = "object",
                            Properties = {
                                ["uploadedFile"] = new OpenApiSchema() {
                                    Description = "Upload File",
                                    Type = "file",
                                    Format = "binary" },
                                ["title"] = new OpenApiSchema() {
                                    Title = "This is the title",
                                    Description = "This is the title",
                                    Type = "string"
                                }
                            },
                            Required = new HashSet<string>() { "uploadedFile" }
                        }
                    };
                    operation.RequestBody = new OpenApiRequestBody
                    {
                        Content =
                        {
                            ["multipart/form-data"] = uploadFileMediaType
                        }
                    };

                // -------------------------------
            }


            Console.WriteLine("ff");
        }
    }
}
