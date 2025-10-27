using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddRateLimiter(_ =>
{
    _.AddTokenBucketLimiter(policyName: TestRateLimitingAttribute.Name, options =>
    {
        options.TokenLimit = 10;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 0;
        options.ReplenishmentPeriod = TimeSpan.FromSeconds(2);
        options.TokensPerPeriod = 10;
        options.AutoReplenishment = true;
    });

    _.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseRateLimiter();

app.Run();

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class TestRateLimitingAttribute : Attribute
{
    public static string Name => "test";
    public string PolicyName => Name;
}
