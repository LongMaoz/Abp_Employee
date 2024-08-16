using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MiddleWare;

public class HttpRequestRecordMiddleware(ILogger<HttpRequestRecordMiddleware> logger): IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path.StartsWithSegments(new PathString("/swagger")))
        {
            await next(context);
            return;
        }
        var stopwatch = Stopwatch.StartNew();
        try
        {
            logger.LogInformation("########################## HTTP REQUEST START ##########################");
            string requestBody = null;
            if (context.Request.Body.CanSeek)
            {
                context.Request.Body.Position = 0; // Reset the position to the beginning
                using (var reader = new StreamReader(context.Response.Body, leaveOpen: true))
                {
                    requestBody = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0; // Reset again for the next middleware
                }
            }

            var requestData = new
            {
                UserId = context.User?.Identity?.Name,
                ClientIp = context.Connection.RemoteIpAddress.ToString(),
                Url = context.Request.Path,
                Method = context.Request.Method,
                Body = requestBody,
                Query = context.Request.Query,
                Params = context.Request.RouteValues
            };

            logger.LogInformation($"RequestData {JsonConvert.SerializeObject(requestData)}");


            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation($"########################## HTTP REQUEST END AND COST {stopwatch.ElapsedMilliseconds}ms ##########################");
        }
    }
}