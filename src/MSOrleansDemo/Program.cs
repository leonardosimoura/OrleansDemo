using Microsoft.AspNetCore.Http.Extensions;
using MSOrleansDemo.Grains;
using Orleans.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.OpenApi;
using Orleans.Storage;
using Orleans.Serialization;
using Microsoft.Extensions.Options;
using System.Xml.Serialization;
using Orleans.Configuration;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using MSOrleansDemo.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();

var invariant = "Npgsql";
var connectionString = "User ID=myuser;Password=123456789;Host=localhost;Port=5432;Database=orleans;";

builder.Host.UseOrleans(siloBuilder =>
{
    

    ////Local
    siloBuilder.UseLocalhostClustering();

    //siloBuilder.AddMemoryGrainStorage("agreementDetail");

    ////Persisted
    //siloBuilder.UseRedisClustering(opt =>
    //{
    //    opt.ConnectionString = "localhost:6379";
    //    opt.Database = 0;
    //});

    siloBuilder.UseAdoNetReminderService(options =>
    {
        options.Invariant = invariant;
        options.ConnectionString = connectionString;
    });

    siloBuilder.AddAdoNetGrainStorage("agreementDetail", (OptionsBuilder<AdoNetGrainStorageOptions> optionsBuilder) =>
    {
        //Suporte para salvar o estado como Json foi removido na 7.X
        //script para visualizar o json do campo binario
        //select encode(payloadbinary , 'escape')::jsonb as "payloadAsJson",* from orleansstorage o 
        optionsBuilder.Configure(options =>
        {
            options.Invariant = invariant;
            options.ConnectionString = connectionString;
        });
    });

    siloBuilder.UseDashboard(x => x.HostSelf = true);
});

builder.Services.AddDbContextPool<OrleansDemoDbContext>(options =>
            options.UseNpgsql(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo { Title = "MS Orleans Demo", Version = "v1" });
    x.EnableAnnotations();
});

var app = builder.Build();

#region Default 

app.MapPost("/stress",
    async (IGrainFactory grains, HttpRequest request) =>
    {
        var signers = Enumerable.Range(1, 100).Select(s => s.ToString()).ToList();
        await Parallel.ForEachAsync(signers, async (signerIdentifier, cancellationToken) =>
        {
            var agreements = Enumerable.Range(1, 100).Select(s => s.ToString()).ToList();
            await Parallel.ForEachAsync(agreements, async (agreementIdentifier, cancellationToken) =>
            {
                var agreementGrain = grains.GetGrain<IAgreementGrain>(agreementIdentifier);

                if (Convert.ToInt32(agreementIdentifier) % 2 == 0)
                {
                    await agreementGrain.SignAsync(signerIdentifier);

                    await agreementGrain.GeneratePdfAsync();
                }
                else
                {
                    var _state = await agreementGrain.GetState();
                }
            });
        });

        return Results.Ok();
    })
    .WithOpenApi();

app.MapPost("/agreement/{identifier}:generate-pdf",
    async (IGrainFactory grains, HttpRequest request, string identifier) =>
    {
        var grain = grains.GetGrain<IAgreementGrain>(identifier);

        var pdf = await grain.GeneratePdfAsync();

        return Results.Ok(pdf);
    })
    .WithOpenApi();

app.MapPost("/signer/{signerIdentifier}/agreement/{agreementIdentifier}/",
    async (IGrainFactory grains, HttpRequest request, string signerIdentifier, string agreementIdentifier) =>
    {
        var signerGrain = grains.GetGrain<ISignerGrain>(signerIdentifier);

        var agreementGrain = grains.GetGrain<IAgreementGrain>(agreementIdentifier);

        await signerGrain.SignAgreementAsync(agreementGrain);

        return Results.Ok();
    })
    .WithOpenApi();

app.MapPost("/signer/{signerIdentifier}/agreement/{agreementIdentifier}:noreference",
    async (IGrainFactory grains, HttpRequest request, string signerIdentifier, string agreementIdentifier) =>
    {
        var signerGrain = grains.GetGrain<ISignerGrain>(signerIdentifier);
        await signerGrain.SignAgreementAsync(agreementIdentifier);

        return Results.Ok();
    })
    .WithOpenApi();

#endregion

#region EF


app.MapPost("/EF/stress",
    async (IGrainFactory grains, HttpRequest request) =>
    {
        var signers = Enumerable.Range(1, 100).Select(s => s.ToString()).ToList();
        await Parallel.ForEachAsync(signers, async (signerIdentifier, cancellationToken) =>
        {
            var agreements = Enumerable.Range(1, 100).Select(s => s.ToString()).ToList();
            await Parallel.ForEachAsync(agreements, async (agreementIdentifier, cancellationToken) =>
            {
                var agreementGrain = grains.GetGrain<IAgreementEFGrain>(agreementIdentifier);

                if (Convert.ToInt32(agreementIdentifier) % 2 == 0)
                {
                    await agreementGrain.SignAsync(signerIdentifier);

                    await agreementGrain.GeneratePdfAsync();
                }
                else
                {
                    var _state = await agreementGrain.GetState();
                }
            });
        });

        return Results.Ok();
    })
    .WithOpenApi();

app.MapPost("/EF/agreement/{identifier}:generate-pdf",
    async (IGrainFactory grains, HttpRequest request, string identifier) =>
    {
        var grain = grains.GetGrain<IAgreementEFGrain>(identifier);

        var pdf = await grain.GeneratePdfAsync();

        return Results.Ok(pdf);
    })
    .WithOpenApi();

app.MapPost("/EF/signer/{signerIdentifier}/agreement/{agreementIdentifier}:noreference",
    async (IGrainFactory grains, HttpRequest request, string signerIdentifier, string agreementIdentifier) =>
    {
        var grain = grains.GetGrain<IAgreementEFGrain>(agreementIdentifier);
        await grain.SignAsync(signerIdentifier);

        return Results.Ok();
    })
    .WithOpenApi();


#endregion


app.Map("/dashboard", x => x.UseOrleansDashboard());

app.UseSwagger();
app.UseSwaggerUI();

app.Run();