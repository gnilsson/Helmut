using Helmut.Controls.Features;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IOperationsGrpcService, OperationsGrpcService>();

var app = builder.Build();

app.MapGet("/controls", async ([FromServices] IOperationsGrpcService grpc) =>
{
    var result = await grpc.ExecuteAsync();

    return Results.Ok(result);
});

app.MapGet("/controls2", ([FromServices] IOperationsGrpcService grpc) =>
{
    var result = grpc.Execute2Async();

    return Results.Ok(result);
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();
