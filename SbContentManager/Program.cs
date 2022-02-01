using Refit;
using SbContentManager.ContentstackApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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