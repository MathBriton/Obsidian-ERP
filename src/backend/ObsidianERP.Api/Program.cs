using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using ObsidianERP.Api.Authentication;
using ObsidianERP.Api.ErrorHandling;
using ObsidianERP.Api.Filters;
using ObsidianERP.Api.HealthChecks;
using ObsidianERP.Application;
using ObsidianERP.Infrastructure;
using ObsidianERP.Infrastructure.Persistence;
using Serilog;

// Logger estático para falhas de inicialização. Mantido separado do logger de DI
// (preserveStaticLogger) para não conflitar quando o host é construído mais de uma
// vez — caso dos testes de integração com WebApplicationFactory.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Serilog como provedor de log, configurado via appsettings + enrichers.
    builder.Services.AddSerilog((services, configuration) => configuration
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console(),
        preserveStaticLogger: true);

    builder.Services
        .AddControllers(options => options.Filters.Add<ValidationActionFilter>())
        .AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(
                new System.Text.Json.Serialization.JsonStringEnumConverter()));
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Obsidian ERP API",
            Version = "v1",
            Description = "API do SaaS Obsidian ERP."
        });
    });

    // Response pattern de erros: RFC 7807 (ProblemDetails) centralizado.
    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    builder.Services.AddHealthChecks();

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddJwtAuthentication(builder.Configuration);

    var app = builder.Build();

    // Aplica migrations pendentes no startup, tornando `docker compose up` turnkey.
    // Só roda no provider relacional de produção (PostgreSQL); nos testes o banco é
    // um SQLite in-memory criado via EnsureCreated, então é pulado.
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (db.Database.ProviderName?.Contains("Npgsql", StringComparison.Ordinal) == true)
        {
            db.Database.Migrate();
        }
    }

    app.UseExceptionHandler();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = HealthResponseWriter.WriteResponse
    });

    app.Run();
}
// Não captura a exceção de controle que o WebApplicationFactory usa para
// interceptar o host durante os testes de integração — ela precisa se propagar.
catch (Exception ex) when (ex is not HostAbortedException
    && ex.GetType().Name is not "StopTheHostException")
{
    Log.Fatal(ex, "A aplicação encerrou de forma inesperada durante a inicialização.");
}
finally
{
    Log.CloseAndFlush();
}

// Exposed for integration testing via WebApplicationFactory<Program>.
public partial class Program;
