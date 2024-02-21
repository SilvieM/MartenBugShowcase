using ExperimentingWithMarten;
using Marten;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Marten.Services;
using Microsoft.AspNetCore.Mvc;
using Weasel.Core;

var builder = WebApplication.CreateBuilder(args);

var marten = builder.Services.AddMarten(sp =>
{
    var options = new StoreOptions();

    options.Connection(
        "PORT = 5432; HOST = localhost; TIMEOUT = 15; POOLING = True; MINPOOLSIZE = 1; MAXPOOLSIZE = 100; COMMANDTIMEOUT = 20; DATABASE = 'testing'; PASSWORD = 'Password1!'; USER ID = 'postgres'");
    options.DatabaseSchemaName = "public";
    options.Policies.AllDocumentsEnforceOptimisticConcurrency(); // this is the problem!

    var serializer = new JsonNetSerializer { EnumStorage = EnumStorage.AsString };
    options.Serializer(serializer);
    
    options.Events.MetadataConfig.EnableAll();

    //register all proj and schemas here
    options.Schema.For<Thing>();
    options.Projections.Add<ThingProjection>(ProjectionLifecycle.Inline);

    return options;
});
marten.AddAsyncDaemon(DaemonMode.HotCold);
marten.ApplyAllDatabaseChangesOnStartup();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapPost("/rebuild", async ([FromServices] IDocumentStore documentStore) =>
{
    using var daemon = await documentStore.BuildProjectionDaemonAsync();
    await daemon.RebuildProjection("Thing", CancellationToken.None);
    return "done";
});
app.MapPost("/addThing", async ([FromServices] IDocumentSession documentSession) =>
{
    var evt = new ThingCreatedEvent(){Id = Guid.NewGuid()};
    documentSession.Events.StartStream<ThingCreatedEvent>(evt);
    documentSession.SaveChanges();
});
app.Run();