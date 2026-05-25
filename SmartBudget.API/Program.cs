using Scalar.AspNetCore;
using Serilog;
using SmartBudget.API;
using SmartBudget.Application;
using SmartBudget.Infrastructure;


// Captures startup failures before the DI container is ready
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    var configuration = builder.Configuration;

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Environment}.{Level:u}: {Message:lj}{NewLine}{Exception}")
        .WriteTo.File(
            path: context.Configuration["LogFilePath"] ?? "/app/logs/smartbudget-.log",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Environment}.{Level:u}: {Message:lj}{NewLine}{Exception}")
        );

    builder.Services.AddOpenApi();
    builder.Services.AddProblemDetails();
    builder.Services.AddPresentation(configuration);
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(configuration);

    var app = builder.Build();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseExceptionHandler();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.MapScalarApiReference();
    app.MapControllers();

    if (app.Environment.IsProduction())
    {
        app.UseHttpsRedirection();
    }

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application failed to start");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

return 0;
