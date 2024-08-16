using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using Config;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;

namespace GrpcService
{
    public class GrpcCacheInterceptor : AbpInterceptor, ITransientDependency
    {
        IDistributedCache _distributedCache;
        IHttpContextAccessor _httpContext;
        ILogger<GrpcCacheInterceptor> _logger;
        IConfiguration _conf;
        int _cacheTimeOut;
        public GrpcCacheInterceptor(IHttpContextAccessor httpContext, IDistributedCache distributedCache, IConfiguration configuration, ILogger<GrpcCacheInterceptor> logger)
        {
            _httpContext = httpContext;
            _distributedCache = distributedCache;
            _logger = logger;
            _conf = configuration;
            _cacheTimeOut = Int32.TryParse(_conf["AppCacheInterceptor:TimeOut"], out var timeOut) ? timeOut : 2;
        }
        AppCacheAttribute AppCache { get; set; }
        string CacheConfig { get; set; }
        bool IsCacheTimeOut { get; set; } = false;
        public override async Task InterceptAsync(IAbpMethodInvocation invocation)
        {
            if (!invocation.Method.IsDefined(typeof(AppCacheAttribute), true))
            {
                await invocation.ProceedAsync();
                return;
            }
            this.AppCache = invocation.Method.GetCustomAttribute<AppCacheAttribute>();
            this.CacheConfig = $"AppCacheInterceptor:{AppCache._module}:{invocation.Method.Name}";
            var request = $"{this.AppCache._module} {invocation.Method.Name} {JsonConvert.SerializeObject(invocation.ArgumentsDictionary)}";
            var key = $"{this.AppCache._module}:{invocation.Method.Name}:{HashHelper.Hash_2_MD5_32(request)}";
            var values = await GetFromCache(request, key);
            if (values != null)
            {
                invocation.ReturnValue = values;
                return;
            }
            await invocation.ProceedAsync();
            await SetCache(invocation.ReturnValue, request, key);
        }
        protected async Task<object> GetFromCache(string request, string key)
        {
            StringBuilder sbCacheLog = new StringBuilder();
            sbCacheLog.Append($"traceId:{_httpContext?.HttpContext?.Items["traceId"]},{DateTime.Now.ToString("HH:mm:ss.fff")} get from cache.");
            string values = null;
            try
            {
                values = await _distributedCache.GetStringAsync(key, new CancellationTokenSource(TimeSpan.FromSeconds(_cacheTimeOut)).Token);
                sbCacheLog.Append($"request:{request},key:{key},type:{this.AppCache._obj.FullName},config:{this.CacheConfig},timeout:{_cacheTimeOut}");
                if (values?.Length > 0)
                {
                    _logger.LogInformation($"{sbCacheLog} get from cache");
                    return this.AppCache._obj.IsClass ? JsonConvert.DeserializeObject(values, this.AppCache._obj) : values;
                }
                return null;
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("timeout"))
                    await _distributedCache.RemoveAsync(key, new CancellationTokenSource(TimeSpan.FromSeconds(_cacheTimeOut)).Token);
                IsCacheTimeOut = ex.Message.Contains("timeout");
                _logger.LogError($"{sbCacheLog}\r\ncache values:{values}\r\n{DateTime.Now.ToString("HH:mm:ss.fff")} _cacheTimeOut:{_cacheTimeOut} error:{ex.Message} {ex.InnerException?.Message}\r\n{ex.Source} {ex.StackTrace}");
                return null;
            }
        }
        protected async Task SetCache(object values, string request, string key)
        {
            if (values == null || this.IsCacheTimeOut)
                return;
            var cacheValues = JsonConvert.SerializeObject(values);
            int expireCache = 0;
            try
            {
                expireCache = this.AppCache._expire > 0 ? this.AppCache._expire : (Int32.TryParse(_conf[CacheConfig], out var expire) ? expire : 10);
                await _distributedCache.SetStringAsync(key, cacheValues, new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(expireCache)
                }, new CancellationTokenSource(TimeSpan.FromSeconds(_cacheTimeOut)).Token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"traceId:{_httpContext?.HttpContext?.Items["traceId"]},{request},type:{this.AppCache._obj.FullName},set cache error,config:{CacheConfig},expireCache:{expireCache}s,timeout:{_cacheTimeOut} values:\r\n{cacheValues}\r\n{ex.Message} {ex.InnerException?.Message}\r\n{ex.Source} {ex.StackTrace}");
            }
        }
    }
}
