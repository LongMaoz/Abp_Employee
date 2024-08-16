using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;
using Config;

namespace GrpcService
{
    public class GrpcClientInterceptor : AbpInterceptor, ITransientDependency
    {
        ILogger<GrpcClientInterceptor> _logger;
        GrpcErpApi _grpcErpApi;
        IHttpContextAccessor _httpContext;
        GrpcHelper _grpcHelper;
        public GrpcClientInterceptor(ILogger<GrpcClientInterceptor> logger, IOptionsSnapshot<GrpcErpApi> grpcErpApi, IHttpContextAccessor httpContext, GrpcHelper grpcHelper)
        {
            _logger = logger;
            _grpcErpApi = grpcErpApi.Value;
            _httpContext = httpContext;
            _grpcHelper = grpcHelper;
        }
        public override async Task InterceptAsync(IAbpMethodInvocation invocation)
        {
            StringBuilder sb = new();
            string error = "";
            string start = DateTime.Now.ToString("HH:mm:ss.fff");
            string end = null;
            var request = JsonConvert.SerializeObject(invocation.ArgumentsDictionary);
            try
            {

                if (_httpContext.HttpContext != null && !_httpContext.HttpContext.Items.ContainsKey("traceId"))
                    _httpContext.HttpContext.Items.TryAdd("traceId", Guid.NewGuid().ToString());
                sb.AppendLine($"GrpcClientInterceptor {invocation.Method.DeclaringType.Name} {invocation.Method.Name}")
                .AppendLine($"sub:{_httpContext.HttpContext?.User?.FindFirst("sub")?.Value},traceId:{_httpContext.HttpContext?.Items["traceId"]}").AppendLine($"request:{request}")
                .AppendLine($"timeout:{_grpcErpApi.TimeOut}s");
                await invocation.ProceedAsync();
            }
            catch (RpcException ex)
            {
                sb.AppendLine($"RpcException {ex.Message} {ex.InnerException?.Message} {ex.StatusCode} {ex.StackTrace}\r\n{ex.Source}.");
                if (_httpContext.HttpContext?.Items != null)
                    _httpContext.HttpContext.Items.TryAdd("GrpcException", $"RpcException {(ex.Message.Length > 300 ? ex.Message[..300] : ex.Message)}");
            }
            catch (Exception ex)
            {
                end = DateTime.Now.ToString("HH:mm:ss.fff");
                error = $"error:{ex.Message}{ex.InnerException?.Message} time:{start}~{end} {(ex.Message.ToLower().Contains("canceled") ? $"用户请求超时{_grpcErpApi.TimeOut}s." : "")}";
                sb.AppendLine($"{error}\r\n{ex.StackTrace}\r\n{ex.Source}");
                if (_httpContext.HttpContext?.Items != null)
                    _httpContext.HttpContext.Items.TryAdd("GrpcException", ex.Message.Length > 300 ? ex.Message[..300] : ex.Message);
            }
            finally
            {
                var defaultType = invocation.Method.ReturnType?.GetProperty("Result")?.PropertyType;
                if (defaultType != null && defaultType.IsClass == false && invocation.ReturnValue == null)
                {
                    invocation.ReturnValue = Activator.CreateInstance(defaultType);
                }
                if (end == null)
                    sb.AppendLine($"start:{start},end:{DateTime.Now.ToString("HH:mm:ss.fff")}. ");
                sb.AppendLine($"grpcUrl:{_grpcHelper.GrpcUrl}\r\nresponse: {JsonConvert.SerializeObject(invocation.ReturnValue)}");
                if (_grpcHelper.sbLog?.Length > 0)
                    sb.AppendLine($"logger:{_grpcHelper.sbLog}");
                _logger.LogInformation(sb.ToString());
                var grpcSource = Activity.Current?.Source;
                if (grpcSource != null)
                {
                    using var activity = grpcSource.StartActivity($"GrpcRequest {invocation.Method.DeclaringType.Name}.{invocation.Method.Name}");
                    if (activity != null)
                    {
                        if (!string.IsNullOrEmpty(error))
                        {
                            activity.SetTag("server address", _grpcHelper.GrpcUrl);
                            activity.SetTag("exception", error);
                        }
                        activity.SetTag("params", request);
                        activity.SetTag("sub", _httpContext.HttpContext?.User?.FindFirst("sub")?.Value);
                    }
                }
            }
        }
    }
}

