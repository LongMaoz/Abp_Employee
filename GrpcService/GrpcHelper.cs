using Grpc.Core;
using Grpc.Net.Client.Configuration;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using System.Text;
using Config;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GrpcService
{
    public class GrpcHelper
    {
        GrpcErpApi _grpcErpApi;
        IConfiguration _configuration;
        ILoggerFactory _loggerFactory { get; set; }
        public string GrpcUrl { get; set; }
        public StringBuilder sbLog { get; set; } = new StringBuilder();
        public GrpcHelper(IOptionsSnapshot<GrpcErpApi> grpcErpApi, IConfiguration configuration)
        {
            _grpcErpApi = grpcErpApi.Value;
            _configuration = configuration;
        }
        public async Task<GrpcChannel> GetGrpcChannel(Type target, NacosConfigKey nacosConfig)
        {
            if (_loggerFactory == null)
            {
                _loggerFactory = LoggerFactory.Create(logging =>
                {
                    LogLevel curLevel = Enum.TryParse(_grpcErpApi.LogLevel, true, out LogLevel level) ? level : LogLevel.Information;
                    if (curLevel == LogLevel.Debug)
                        logging.AddConsole();
                    logging.SetMinimumLevel(curLevel);
                });
                _loggerFactory.AddSerilog();
            }
            this.GrpcUrl = $"{_configuration[nacosConfig.Host]}:{_configuration[nacosConfig.Port]}";
            this.sbLog = new StringBuilder();
            var defaultMethodConfig = new MethodConfig
            {
                Names = { MethodName.Default },
                RetryPolicy = new RetryPolicy
                {
                    MaxAttempts = _grpcErpApi.RetryCount,
                    InitialBackoff = TimeSpan.FromSeconds(_grpcErpApi.RetrySpace),
                    MaxBackoff = TimeSpan.FromSeconds(_grpcErpApi.RetrySpace),
                    BackoffMultiplier = 1,
                    RetryableStatusCodes = { StatusCode.Unavailable, StatusCode.DeadlineExceeded }
                }
            };
            var timeOut = TimeSpan.FromSeconds(_grpcErpApi.TimeOut);
            var handler = new SocketsHttpHandler()
            {
                EnableMultipleHttp2Connections = true,
                ConnectTimeout = timeOut,
                PooledConnectionIdleTimeout = timeOut,
                KeepAlivePingTimeout = timeOut,
            };
            var channel = GrpcChannel.ForAddress($"http://{GrpcUrl}", new GrpcChannelOptions
            {
                ServiceConfig = _grpcErpApi.RetryCount > 1 ? new ServiceConfig
                {
                    MethodConfigs = { defaultMethodConfig }
                } : null,
                MaxReconnectBackoff = TimeSpan.FromSeconds(_grpcErpApi.RetrySpace),
                MaxRetryAttempts = _grpcErpApi.RetryCount,
                LoggerFactory = _loggerFactory,
                HttpHandler = handler
            });
            await channel.ConnectAsync(new CancellationTokenSource(timeOut).Token);
            return channel;
        }
    }
}
