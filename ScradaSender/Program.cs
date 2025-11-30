using Microsoft.Extensions.Options;
using ScradaSender.Agents;
using ScradaSender.Agents.Interfaces;
using ScradaSender.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ScradaSettings>(builder.Configuration.GetSection("Scrada"));

builder.Services.AddHttpClient<ScradaServiceAgent>((provider, HttpClient) =>
{
    var settings = provider.GetRequiredService<IOptions<ScradaSettings>>().Value;

    HttpClient.BaseAddress = new Uri(settings.BaseUrl);
    HttpClient.DefaultRequestHeaders.Add("X-API-KEY", settings.XApiKey);
    HttpClient.DefaultRequestHeaders.Add("X-PASSWORD", settings.XPassword);
    
}).AddTypedClient<IScradaServiceAgent, ScradaServiceAgent>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
