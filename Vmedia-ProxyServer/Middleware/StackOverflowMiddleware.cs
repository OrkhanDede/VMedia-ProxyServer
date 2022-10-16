using System.Text;
using Microsoft.Net.Http.Headers;
using VMedia_ProxyServer.Services;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

namespace VMedia_ProxyServer.Middleware
{
    public class StackOverflowMiddleware
    {
        private const string Domain = "stackoverflow.com";
        private const string Url = "https://stackoverflow.com";

        private const string UserAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.3";

        private readonly RequestDelegate _next;
        public StackOverflowMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IHttpClientFactory httpClientFactory, IContentModiferService contentModiferService)
        {
            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(Url);
            client.DefaultRequestHeaders.Add(HeaderNames.Origin, Domain);
            client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, UserAgent);
            var message = new HttpRequestMessage()
            {
                Method = new HttpMethod(context.Request.Method),
                RequestUri = new Uri(client.BaseAddress, context.Request.Path),

            };
            var response = await client.SendAsync(message, CancellationToken.None);


            foreach (var httpResponseHeader in response.Headers)
            {
                foreach (var value in httpResponseHeader.Value)
                {
                    context.Response.Headers[httpResponseHeader.Key] = value;
                }
            }
            var responseText = await response.Content.ReadAsStringAsync();

            if (Equals(response.Content.Headers.ContentType, MediaTypeHeaderValue.Parse("text/html")))
                responseText = contentModiferService.Modify(responseText);
                
            await context.Response.WriteAsync(responseText, Encoding.UTF8, CancellationToken.None);
        }

    }
}
