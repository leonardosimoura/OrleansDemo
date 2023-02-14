using Microsoft.AspNetCore.Http.Extensions;
using MSOrleansDemo.Grains;
using Orleans.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder();

builder.Host.UseOrleans(siloBuilder =>
{
    var invariant = "Npgsql";
    var connectionString = "User ID=myuser;Password=123456789;Host=localhost;Port=5432;Database=orleans;";

    siloBuilder.UseLocalhostClustering();

    siloBuilder.AddMemoryGrainStorage("agreementDetail");

    siloBuilder.UseDashboard(x => x.HostSelf = true);

    // siloBuilder.UseRedisClustering(opt =>
    // {
    //     opt.ConnectionString = "localhost:6379";
    //     opt.Database = 0;
    // });

    // siloBuilder.UseAdoNetReminderService(options =>
    // {
    //     options.Invariant = invariant;
    //     options.ConnectionString = connectionString;
    // });

    // siloBuilder.AddAdoNetGrainStorage("agreementDetail", options =>
    // {
    //     options.Invariant = invariant;
    //     options.ConnectionString = connectionString;
    // });

    
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo { Title = "MS Orleans Demo", Version = "v1" });
    x.EnableAnnotations();
});



var app = builder.Build();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapGet("/stress",
    async (IGrainFactory grains, HttpRequest request) =>
    {

        var signers = Enumerable.Range(1, 1000).Select(s => Guid.NewGuid().ToString()).ToList();

        await Parallel.ForEachAsync(signers, async (signerIdentifier, cancellationToken) =>
        {
            var agreements = Enumerable.Range(1, 100).Select(s => Guid.NewGuid().ToString()).ToList();
            await Parallel.ForEachAsync(agreements, async (agreementIdentifier, cancellationToken) =>
            {
                var signerGrain = grains.GetGrain<ISignerGrain>(signerIdentifier);

                var agreementGrain = grains.GetGrain<IAgreementGrain>(agreementIdentifier);

                await signerGrain.SignAgreementAsync(agreementGrain);

                await agreementGrain.GetPdfAsync();
            });
        });

        return Results.Ok();
    })
    .WithOpenApi();

app.MapGet("/agreement/{identifier}:generate-pdf",
    async (IGrainFactory grains, HttpRequest request, string identifier) =>
    {
        var grain = grains.GetGrain<IAgreementGrain>(identifier);

        var pdf = await grain.GetPdfAsync();


        return Results.Ok(pdf);
    })
    .WithOpenApi();

app.MapGet("/signer/{signerIdentifier}/agreement/{agreementIdentifier}/",
    async (IGrainFactory grains, HttpRequest request, string signerIdentifier, string agreementIdentifier) =>
    {
        var signerGrain = grains.GetGrain<ISignerGrain>(signerIdentifier);

        var agreementGrain = grains.GetGrain<IAgreementGrain>(agreementIdentifier);

        await signerGrain.SignAgreementAsync(agreementGrain);

        return Results.Ok();
    })
    .WithOpenApi();

app.MapGet("/signer/{signerIdentifier}/agreement/{agreementIdentifier}:noreference",
    async (IGrainFactory grains, HttpRequest request, string signerIdentifier, string agreementIdentifier) =>
    {
        var signerGrain = grains.GetGrain<ISignerGrain>(signerIdentifier);
        await signerGrain.SignAgreementAsync(agreementIdentifier);

        return Results.Ok();
    })
    .WithOpenApi();

app.Map("/dashboard", x => x.UseOrleansDashboard());

app.UseSwagger();
app.UseSwaggerUI();

app.Run();