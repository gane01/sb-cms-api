using Refit;
using SbContentManager.ContentstackApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<HeaderHandler>();
builder.Services.AddRefitClient<IContentstackApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://eu-cdn.contentstack.com/v3"))
    .AddHttpMessageHandler<HeaderHandler>();

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