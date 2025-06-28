using CrickSimPro.API.Models;
using CrickSimPro.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddScoped<MatchSimulator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/simulate", (MatchScenario scenario, MatchSimulator matchSimulator) =>
{
    var result = matchSimulator.Simulate(scenario);
    return Results.Ok(result);
});

app.Run();
