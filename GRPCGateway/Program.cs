using GRPCGateway;
using GRPCGateway.Services;
using MassTransit;
using Microsoft.OpenApi.Models;
using Serilog;

string modulesPath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
var container = new ModulesContainer(modulesPath);
container.LoadModules();

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
          .MinimumLevel.Debug()
          .Enrich.FromLogContext()
          .WriteTo.Console()
          .WriteTo.File(
            Path.Combine("./", "logs", "diagnostics.txt"),
             rollingInterval: RollingInterval.Day,
             fileSizeLimitBytes: 10 * 1024 * 1024,
             retainedFileCountLimit: 2,
             rollOnFileSizeLimit: true,
             shared: true,
             flushToDiskInterval: TimeSpan.FromSeconds(1))
          .CreateLogger();

container.RegistryModules(builder.Services);

builder.Services.AddGrpc();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo { Title = "UTIS gRPC API (microservices)", Version = "v1", Description = "Пример реализации микросервисов с общением через брокер" });

    c.SwaggerDoc("v2",
        new OpenApiInfo { Title = "UTIS gRPC API (monolit as microservice)", Version = "v2", Description = "Пример гибрида монолитной и микросервисной архитектуры" });

    c.AddDocumentFilterInstance(new VersionFilter());
});


builder.Services.AddMassTransit(x =>
{
    x.AddDelayedMessageScheduler();
    x.SetKebabCaseEndpointNameFormatter();
    x.SetInMemorySagaRepositoryProvider();

    // В реальном проекте настраивается под любой брокер (x.UsingKafka, x.UsingRabbit и т.д.
    x.UsingInMemory((context, cfg) =>
    {
        //cfg.UseInMemoryOutbox(context);
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

app.MapGrpcService<PersonsServiceV1>();
app.MapGrpcService<LampsServiceV1>();
app.MapGrpcService<GivenLampServiceV1>();
app.MapGrpcService<PersonsServiceV2>();
app.MapGrpcService<LampsServiceV2>();
app.MapGrpcService<GivenLampServiceV2>();

app.UseHttpsRedirection();
app.UseSwagger();


app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "UTIS API v1(microservices)");
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "UTIS API v2(monolit)");
});


app.Run();

