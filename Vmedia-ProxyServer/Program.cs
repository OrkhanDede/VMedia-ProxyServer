using VMedia_ProxyServer.Middleware;
using VMedia_ProxyServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddScoped<IContentModiferService, HtmlContentModiferService>();

var app = builder.Build();


// Configure the HTTP request pipeline.

app.Use(async (context, next) =>
{
    if (context.Request.Path.Value == "/favicon.ico"||context.Request.Path== "/opensearch.xml")
    {
        return;
    }
    // No favicon, call next middleware
    await next.Invoke();
});
app.UseMiddleware<StackOverflowMiddleware>();
app.UseHttpsRedirection();


app.Run();