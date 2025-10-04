namespace TransferService.Api.Middleware
{
    public class ExternalAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HttpClient _httpClient;

        public ExternalAuthMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory)
        {
            _next = next;
            _httpClient = httpClientFactory.CreateClient("AuthService");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Swagger atla
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("X-Api-Key", out var apiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key is missing.");
                return;
            }

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/validate");
            request.Headers.Add("X-Api-Key", (string)apiKey);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                context.Response.StatusCode = (int)response.StatusCode;
                await context.Response.WriteAsync("Unauthorized.");
                return;
            }

            await _next(context);
        }
    }

}
