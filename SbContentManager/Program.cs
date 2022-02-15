using Microsoft.OpenApi.Models;
using Refit;
using SbContentManager.Contentstack;
using SbContentManager.Asset;
using SbContentManager.Swagger;
using System.Reflection;
using SbContentManager.Entry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    {
        c.OperationFilter<CreateAssetOperationFilter>();
        c.SwaggerDoc("v1",
            new OpenApiInfo
            {
                Title = "Sb Content manager AP I",
                Version = "v0.1"
            }
         );

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
    });

builder.Services.AddSingleton<HttpClient>();
builder.Services.AddTransient<AssetEffect>();
builder.Services.AddTransient<EntryEffect>();
builder.Services.AddTransient<ContentstackClient>();
builder.Services.AddTransient<HttpHeaderHandler>();
builder.Services.AddRefitClient<IContentstackApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Config.ContentStackApiBaseUrl))
    .AddHttpMessageHandler<HttpHeaderHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureApi();
app.UseRouting();
app.Run();