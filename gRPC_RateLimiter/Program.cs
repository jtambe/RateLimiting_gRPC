using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using gRPC_RateLimiter.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddRateLimiter(ratelimiterOptions =>
{
    ratelimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    ratelimiterOptions.OnRejected = (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.AppendTrailer("error_detail","too many requests");
        return new ValueTask();
    };

    ratelimiterOptions.AddFixedWindowLimiter(policyName: "fixed", options =>
    {
        options.PermitLimit = 2;
        options.Window = TimeSpan.FromSeconds(20);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 1;
    });

    ratelimiterOptions.AddSlidingWindowLimiter(policyName: "sliding", options =>
    {
        options.PermitLimit = 5;
        options.Window = TimeSpan.FromSeconds(9);
        options.SegmentsPerWindow = 3;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 1;
    });

    ratelimiterOptions.AddTokenBucketLimiter(policyName: "token", options =>
    {
        options.TokenLimit = 1;
        options.ReplenishmentPeriod = TimeSpan.FromSeconds(5);
        options.AutoReplenishment = true;
        options.TokensPerPeriod = 1;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 1;
    });

    ratelimiterOptions.AddConcurrencyLimiter(policyName: "concurrency", options =>
    {
        options.PermitLimit = 2;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 3;
    });


});


var app = builder.Build();

app.UseRateLimiter();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
