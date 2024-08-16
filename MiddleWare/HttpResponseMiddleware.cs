using MiddleWare.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;
using Domain.Shared.Extends;
using Volo.Abp.Http;
using System.Net;

namespace MiddleWare;

public class HttpResponseMiddleware : IMiddleware
{
    private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path.StartsWithSegments(new PathString("/swagger")) || context.Request.ContentType == "application/grpc")
        {
            await next(context);
            return;
        }

        

        var originalBodyStream = context.Response.Body;
        await using (var responseBody = new MemoryStream())
        {
            context.Response.Body = responseBody;

            await next(context);

            if (context.Response.StatusCode != StatusCodes.Status204NoContent)
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(responseBody).ReadToEndAsync();
                object resultModel = JsonConvert.DeserializeObject(responseText); // 默认反序列化

                if (context.Response.Headers.ContainsKey(AbpHttpConstsExtend.AbpValidationErrorFormat) ||
                    context.Response.Headers.ContainsKey(AbpHttpConsts.AbpErrorFormat))
                {
                    var errorInfo = JsonConvert.DeserializeObject<RemoteServiceErrorResponse>(responseText);
                    List<string> errorsList = errorInfo.Error.ValidationErrors?.Select(x => x.Message).ToList();
                    resultModel = new ErrorResponse
                    {
                        Errors = errorsList,
                        StatusCode = int.Parse(errorInfo.Error.Code ?? "200"),
                        StatusMessage = errorInfo.Error.Message
                    };
                    context.Response.StatusCode = StatusCodes.Status200OK;
                }
                else if (context.Items.ContainsKey("businessCode"))
                {
                    resultModel = new ApiResponse<object?>
                    {
                        Data = resultModel, // 直接使用已反序列化的对象
                        StatusCode = int.Parse(context.Items["businessCode"].ToString()),
                        StatusMessage = null,
                        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    };
                }
                else
                {
                    resultModel = new ApiResponse<object?>
                    {
                        Data = resultModel, // 直接使用已反序列化的对象
                        StatusCode = context.Response.StatusCode,
                        StatusMessage = null,
                        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    };
                }

                //单独处理401情况
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    context.Response.ContentType = "application/json; charset=utf-8";
                }

                var resultJson = JsonConvert.SerializeObject(resultModel, _settings);
                var responseBytes = Encoding.UTF8.GetBytes(resultJson);

                responseBody.Seek(0, SeekOrigin.Begin); // 重置流位置
                responseBody.SetLength(0); // 清空流以准备写入新内容
                await responseBody.WriteAsync(responseBytes, 0, responseBytes.Length);
                context.Response.ContentLength = responseBytes.Length;
            }

            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}
