using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using ScradaSender.Agents;
using ScradaSender.Agents.Interfaces;
using ScradaSender.Api.Jobs;
using ScradaSender.Api.Jobs.Interface;
using ScradaSender.DataAccess;
using ScradaSender.DataAccess.Repositories;
using ScradaSender.DataAccess.Repositories.Interfaces;
using ScradaSender.Shared.Constants;
using ScradaSender.Shared.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ScradaSettings>(builder.Configuration.GetSection(OptionsConstants.ScradaSection));
builder.Services.Configure<FileReaderSettings>(builder.Configuration.GetSection(OptionsConstants.FileReaderSection));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IFileStatussesRepository, FileStatussesRepository>();
builder.Services.AddScoped<IFileReaderJob, FileReaderJob>();
builder.Services.AddScoped<IStatusCheckerJob, StatusCheckerJob>();
builder.Services.AddScoped<IGetDocumentsJob, GetDocumentsJob>();

builder.Services.AddHttpClient<ScradaServiceAgent>((provider, HttpClient) =>
{
    var settings = provider.GetRequiredService<IOptions<ScradaSettings>>().Value;

    HttpClient.BaseAddress = new Uri(settings.BaseUrl);
    HttpClient.DefaultRequestHeaders.Add("X-API-KEY", settings.XApiKey);
    HttpClient.DefaultRequestHeaders.Add("X-PASSWORD", settings.XPassword);

}).AddTypedClient<IScradaServiceAgent, ScradaServiceAgent>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ScradaSenderDbContext>(options =>
    options.UseNpgsql(connectionString));

#region Hangfire
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options =>
    {
        options.UseNpgsqlConnection(connectionString);
    },
    new PostgreSqlStorageOptions
    {
        SchemaName = "hangfire",
        PrepareSchemaIfNecessary = true
    }));

builder.Services.AddHangfireServer();
#endregion

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

app.UseHangfireDashboard("/hangfire");

//RecurringJob.AddOrUpdate<IFileReaderJob>("ReadFiles", i => i.ReadFilesAsync(), builder.Configuration.GetSection(OptionsConstants.HangfireSection).GetSection(OptionsConstants.FileReaderJobSection).Value);
//RecurringJob.AddOrUpdate<IStatusCheckerJob>("CheckStatusses", i => i.CheckStatusAsync(), builder.Configuration.GetSection(OptionsConstants.HangfireSection).GetSection(OptionsConstants.StatusCheckerJobSection).Value);

using (var scope = app.Services.CreateScope())
{
    var myService = scope.ServiceProvider.GetRequiredService<IGetDocumentsJob>().GetDocumentsAsync();
}
app.Run();