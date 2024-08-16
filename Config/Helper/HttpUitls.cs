using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Polly;
using System.Reflection;
using System.Net.Http.Headers;
using Entity.Base;
using Microsoft.Extensions.Logging;

namespace Config
{
    public class HttpUitls
    {
        IHttpContextAccessor _httpContextAccessor;
        ILogger<HttpUitls> _logger;
        public HttpUitls(IHttpContextAccessor httpContextAccessor,ILogger<HttpUitls> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        /// <summary>
        /// polly重试机制  然后每1秒重新执行一次,可以重试5次
        /// </summary>
        private T RetryTimesPolicy<T>(string httpUrl, Func<T> action)
        {
            try
            {
                var policy = Policy
                    .Handle<HttpGetException>()
                    .WaitAndRetry(
                     1,
                     retryAttempt => TimeSpan.FromSeconds(Math.Pow(1, retryAttempt)),
                     (ex, timer, c, i) =>
                     {
                         _logger.LogError($"执行失败! 重试次数 {c};异常来自 {ex.GetType().Name};异常信息:{ex.Message}");
                     });
                return policy.Execute(action);
            }
            catch (HttpGetException ex) { throw new HttpGetException(500, httpUrl + " " + ex.Message); };
        }
        private async Task<T> RetryTimesPolicyAsync<T>(string httpUrl, Func<Task<T>> action)
        {
            try
            {
                var policy = Policy
                    .Handle<HttpGetException>()
                    .WaitAndRetryAsync(
                     1,
                     retryAttempt => TimeSpan.FromSeconds(Math.Pow(1, retryAttempt)),
                     (ex, timer, c, i) =>
                     {
                         _logger.LogError($"执行失败! 重试次数 {c};异常来自 {ex.GetType().Name};异常信息:{ex.Message}");
                     });
                return await policy.ExecuteAsync(action);
            }
            catch (HttpGetException ex) { throw new HttpGetException(500, httpUrl + " " + ex.Message); };
        }
        public async Task<UnitResult<string>> HttpClientGet<T>(string url, T data, bool isToken = true, string token = null,int timeOut=2)
        {
            return await RetryTimesPolicyAsync<UnitResult<string>>("", async () =>
            {
                var jsonString = "";
                if (data != null)
                {
                    if (typeof(T) != typeof(string))
                    {
                        var properties = typeof(T).GetProperties();
                        foreach (var pro in properties)
                        {
                            string name = pro.Name;
                            object value = pro.GetValue(data);
                            jsonString += $"{name}={value}&";
                        }
                        jsonString = jsonString.TrimEnd('&');
                    }
                    else
                        jsonString = data.ToString();
                }
                url = !string.IsNullOrEmpty(jsonString) ? url + "?" + jsonString : url;

                var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.None };

                using (var httpclient = new HttpClient(handler))
                {
                    httpclient.Timeout=TimeSpan.FromSeconds(timeOut);
                    httpclient.DefaultRequestHeaders.Accept.Clear();
                    httpclient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    if (isToken)
                    {
                        if (token == null)
                            token = _httpContextAccessor.HttpContext.Items["token"].ToString();
                        httpclient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }
                    String userAgent = "Mozilla/5.0 (Windows NT 6.2; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.87 Safari/537.36";
                    httpclient.DefaultRequestHeaders.Add("User-Agent", userAgent);
                    var response = await httpclient.GetAsync(url);
                    var retString = "";
                    Stream myResponseStream = response.Content.ReadAsStreamAsync().Result;
                    StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                    retString = myStreamReader.ReadToEnd();
                    myStreamReader.Close();
                    myResponseStream.Close();
                    return new UnitResult<string>() { result = response.IsSuccessStatusCode, message = response.StatusCode + ";" + response.ReasonPhrase, obj = retString };
                };
            });
        }
        /// <summary>
        /// 以表单形式post
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="isToken"></param>
        /// <returns></returns>
        public async Task<UnitResult<string>> HttpClientPost<T>(string url, T data, bool isToken = true)
        {
            return await RetryTimesPolicyAsync<UnitResult<string>>("", async () =>
            {
                var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.None };

                using (var httpclient = new HttpClient(handler))
                {
                    if (isToken)
                    {
                        var token = _httpContextAccessor.HttpContext.Items["token"].ToString();
                        httpclient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }
                    string jsonString = "";
                    if (typeof(T) != typeof(string))
                    {
                        var properties = typeof(T).GetProperties();
                        foreach (var pro in properties)
                        {
                            string name = pro.Name;
                            object value = pro.GetValue(data);
                            jsonString += $"{name}={value}&";
                        }
                        jsonString = jsonString.TrimEnd('&');
                    }
                    byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
                    using (StreamContent sc = new StreamContent(new MemoryStream(bytes)))
                    {
                        sc.Headers.ContentLength = bytes.Length;
                        sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                        HttpResponseMessage response = await httpclient.PostAsync(url, sc);

                        var retString = "";
                        Stream myResponseStream = response.Content.ReadAsStreamAsync().Result;
                        StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                        retString = myStreamReader.ReadToEnd();
                        myStreamReader.Close();
                        myResponseStream.Close();
                        return new UnitResult<string>() { result = response.IsSuccessStatusCode, message = response.StatusCode + ";" + response.ReasonPhrase, obj = retString };
                    }
                };
            });
        }
        public async Task<UnitResult<string>> HttpClientPostForm<T>(string url, T data,Dictionary<string,string> header)
        {
            return await RetryTimesPolicyAsync<UnitResult<string>>("", async () =>
            {
                var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.None };

                using (var httpclient = new HttpClient(handler))
                {
                    foreach(var pair in header)
                    {
                        httpclient.DefaultRequestHeaders.Add(pair.Key, pair.Value);
                    }
                    string jsonString = "";
                    if (typeof(T) != typeof(string))
                    {
                        var properties = typeof(T).GetProperties();
                        foreach (var pro in properties)
                        {
                            string name = pro.Name;
                            object value = pro.GetValue(data);
                            jsonString += $"{name}={value}&";
                        }
                        jsonString = jsonString.TrimEnd('&');
                    }
                    byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
                    using (StreamContent sc = new StreamContent(new MemoryStream(bytes)))
                    {
                        sc.Headers.ContentLength = bytes.Length;
                        sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                        HttpResponseMessage response = await httpclient.PostAsync(url, sc);

                        var retString = "";
                        Stream myResponseStream = response.Content.ReadAsStreamAsync().Result;
                        StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                        retString = myStreamReader.ReadToEnd();
                        myStreamReader.Close();
                        myResponseStream.Close();
                        return new UnitResult<string>() { result = response.IsSuccessStatusCode, message = response.StatusCode + ";" + response.ReasonPhrase, obj = retString };
                    }
                };
            });
        }

        /// <summary>
        /// 以json形式post
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="isToken"></param>
        /// <returns></returns>
        public async Task<UnitResult<string>> HttpClientResultPostJson<T>(string url, T data, bool isToken = true, string tokenValue = null)
        {
            return await RetryTimesPolicyAsync<UnitResult<string>>("", async () =>
             {
                 var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.None };

                 using (var httpclient = new HttpClient(handler))
                 {
                     if (isToken)
                     {
                         var token = tokenValue;
                         if (tokenValue == null)
                         {
                             token = _httpContextAccessor.HttpContext.Items["token"].ToString();
                         }
                         httpclient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                     }
                     string jsonString = "";
                     if (typeof(T) != typeof(string))
                     {
                         var properties = typeof(T).GetProperties();
                         foreach (var pro in properties)
                         {
                             string name = pro.Name;
                             object value = pro.GetValue(data);
                             jsonString += $"{name}={value}&";
                         }
                         jsonString = jsonString.TrimEnd('&');
                     }
                     byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
                     using (StreamContent sc = new StreamContent(new MemoryStream(bytes)))
                     {
                         sc.Headers.ContentLength = bytes.Length;
                         sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                         HttpResponseMessage response = await httpclient.PostAsync(url, sc);
                         var retString = "";
                         Stream myResponseStream = response.Content.ReadAsStreamAsync().Result;
                         StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                         retString = myStreamReader.ReadToEnd();
                         myStreamReader.Close();
                         myResponseStream.Close();
                         return new UnitResult<string>() { result = response.IsSuccessStatusCode, message = response.StatusCode + ";" + response.ReasonPhrase, obj = retString };
                     }
                 };
             });
        }
        /// <summary>
        /// 以json形式put
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="isToken"></param>
        /// <returns></returns>
        public async Task<UnitResult<string>> HttpClientResultPutJson<T>(string url, T data, bool isToken = true, string tokenValue = null)
        {
            return await RetryTimesPolicyAsync<UnitResult<string>>("", async () =>
            {
                var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.None };

                using (var httpclient = new HttpClient(handler))
                {
                    if (isToken)
                    {
                        var token = tokenValue;
                        if (tokenValue == null)
                        {
                            token = _httpContextAccessor.HttpContext.Items["token"].ToString();
                        }
                        httpclient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }
                    string jsonString = "";
                    if (typeof(T) != typeof(string))
                    {
                        var properties = typeof(T).GetProperties();
                        foreach (var pro in properties)
                        {
                            string name = pro.Name;
                            object value = pro.GetValue(data);
                            jsonString += $"{name}={value}&";
                        }
                        jsonString = jsonString.TrimEnd('&');
                    }
                    else
                        jsonString = data.ToString();
                    byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
                    using (StreamContent sc = new StreamContent(new MemoryStream(bytes)))
                    {
                        sc.Headers.ContentLength = bytes.Length;
                        sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                        HttpResponseMessage response = await httpclient.PutAsync(url, sc);
                        var retString = "";
                        Stream myResponseStream = response.Content.ReadAsStreamAsync().Result;
                        StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                        retString = myStreamReader.ReadToEnd();
                        myStreamReader.Close();
                        myResponseStream.Close();
                        // return response.IsSuccessStatusCode;//StatusCode: 401, ReasonPhrase: 'Unauthorized',
                        return new UnitResult<string>() { result = response.IsSuccessStatusCode,message = response.StatusCode + ";" + response.ReasonPhrase, obj = retString };
                    }
                };
            });
        }

        /// <summary>
        /// 以json形式post
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="isToken"></param>
        /// <returns></returns>
        public async Task<UnitResult<string>> HttpClientPostJson<T>(string url, T data, bool isToken = true, string tokenValue = null,int timeOut=10)
        {
            return await RetryTimesPolicyAsync<UnitResult<string>>("", async () =>
            {
                var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.None };

                using (var httpclient = new HttpClient(handler))
                {
                    httpclient.Timeout = TimeSpan.FromSeconds(timeOut);
                    httpclient.DefaultRequestHeaders.Accept.Clear();
                    httpclient.DefaultRequestHeaders.Add("Accept", "application/json");
                    if (isToken)
                    {
                        var token = tokenValue;
                        if (tokenValue == null)
                        {
                            token = _httpContextAccessor.HttpContext.Items["token"] != null ?
                            _httpContextAccessor.HttpContext.Items["token"].ToString() : "";
                        }
                        httpclient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }
                    String userAgent = "Mozilla/5.0 (Windows NT 6.2; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.87 Safari/537.36";
                    httpclient.DefaultRequestHeaders.Add("User-Agent", userAgent);
                    string postData = ConvertJson(data);
                    HttpContent httpContent = new StringContent(postData);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    try
                    {
                        HttpResponseMessage response = await httpclient.PostAsync(url, httpContent);
                        if (response != null)
                        {
                            var retString = "";
                            Stream myResponseStream = response.Content.ReadAsStreamAsync().Result;
                            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                            retString = myStreamReader.ReadToEnd();
                            myStreamReader.Close();
                            myResponseStream.Close();
                            var msg = response.IsSuccessStatusCode ? response.StatusCode + ";" + response.ReasonPhrase :
                            retString + response.StatusCode + ";" + response.ReasonPhrase;
                            return new UnitResult<string>() { result = response.IsSuccessStatusCode, obj = retString, message = msg };
                        }
                        else
                        {
                            return new UnitResult<string>() { result = false, obj = "", message = $"url:{url},data:{postData} return null" };
                        }
                    }
                    catch(Exception ex)
                    {
                        return new UnitResult<string>() { result = false, obj = "", message = $"url:{url},data:{postData} error:{ex.Message};{ex.Source};{ex.StackTrace}"};
                    }
                };
            });
        }


        /// <summary>
        /// 以json形式put
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="isToken"></param>
        /// <returns></returns>
        public async Task<UnitResult<string>> HttpClientPutJson<T>(string url, T data, bool isToken = true, string tokenValue = null)
        {
            return await RetryTimesPolicyAsync<UnitResult<string>>("", async () =>
            {
                // var content = new FormUrlEncodedContent(Dic);

                var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.None };

                using (var httpclient = new HttpClient(handler))
                {
                    if (isToken)
                    {
                        var token = tokenValue;
                        if (tokenValue == null)
                        {
                            token = _httpContextAccessor.HttpContext.Items["token"].ToString();
                        }
                        httpclient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }
                    byte[] bytes = null;
                    if (data != null)
                    {
                        string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(data); ;
                        bytes = Encoding.UTF8.GetBytes(jsonString);
                    }
                    using (StreamContent sc = new StreamContent(bytes!=null?new MemoryStream(bytes):new MemoryStream()))
                    {
                        sc.Headers.ContentLength = bytes!=null? bytes.Length:0;
                        sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                        HttpResponseMessage response = await httpclient.PutAsync(url, sc);
                        var retString = "";
                        Stream myResponseStream = response.Content.ReadAsStreamAsync().Result;
                        StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                        retString = myStreamReader.ReadToEnd();
                        myStreamReader.Close();
                        myResponseStream.Close();
                        return new UnitResult<string>() { result = response.IsSuccessStatusCode, obj = retString, message = response.StatusCode + ";" + response.ReasonPhrase };
                    }
                };
            });
        }
        public string Get(string Url, string token = null)
        {
            //return RetryTimesPolicy<string>(Url, () =>
            // {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            //request.Proxy = null;
            request.Accept = "*/*";
            request.KeepAlive = true;
            request.Method = "GET";
            request.ContentType = "application/json; charset=utf-8";
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token);
            }
            // request.AutomaticDecompression = DecompressionMethods.GZip;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();

            myStreamReader.Close();
            myResponseStream.Close();

            if (response != null)
            {
                response.Close();
            }
            if (request != null)
            {
                request.Abort();
            }

            return retString;
            //});
        }

        public UnitResult<string> Post(string url, string data, string referer)
        {
            return RetryTimesPolicy<UnitResult<string>>(url, () =>
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;
                Stream myResponseStream = request.GetRequestStream();
                myResponseStream.Write(bytes, 0, bytes.Length);


                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                bool result = response.StatusCode == HttpStatusCode.OK;
                StreamReader myStreamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd(); 
                myStreamReader.Close();
                myResponseStream.Close();
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
                return new UnitResult<string>() { result = result, message = !result ? response.StatusCode + ":" + retString : "", obj = result ? retString : "" };
            });
        }
        public UnitResult<string> PostBasic(string url, string data,string basicHeader)
        {
            return RetryTimesPolicy<UnitResult<string>>(url, () =>
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;
                request.Headers.Add(HttpRequestHeader.Authorization, "Basic " + ConvertTo64(basicHeader));
                Stream myResponseStream = request.GetRequestStream();
                myResponseStream.Write(bytes, 0, bytes.Length);


                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                bool result = response.StatusCode == HttpStatusCode.OK;
                StreamReader myStreamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
                return new UnitResult<string>() { result = result, message = !result ? response.StatusCode + ":" + retString : "", obj = result ? retString : "" };
            });
        }
        public static string LocalIPAddress
        {
            get
            {
                UnicastIPAddressInformation mostSuitableIp = null;
                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (var network in networkInterfaces)
                {
                    if (network.OperationalStatus != OperationalStatus.Up)
                        continue;
                    var properties = network.GetIPProperties();
                    if (properties.GatewayAddresses.Count == 0)
                        continue;

                    foreach (var address in properties.UnicastAddresses)
                    {
                        if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                            continue;
                        if (IPAddress.IsLoopback(address.Address))
                            continue;
                        return address.Address.ToString();
                    }
                }
                return mostSuitableIp != null
                    ? mostSuitableIp.Address.ToString()
                    : "";
            }
        }
        public string ConvertJson<T>(T t)
        {
            return JsonConvert.SerializeObject(t, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() }).Replace("\r\n  ", "").Replace("\r\n", "");
        }
        protected string ConvertTo64(string value)
        {
            byte[] bytes = Encoding.Default.GetBytes(value);
            string str = Convert.ToBase64String(bytes);
            return str;
        }
    }
}
