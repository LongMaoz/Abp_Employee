using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Config
{
    public class BodyModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.BindingSource == BindingSource.Body && context.Metadata.ModelType.IsClass)
            {
                return new BodyModelBinder();
            }

            return null;
        }
    }

    internal class BodyModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            bindingContext.HttpContext.Request.EnableBuffering();
            using var reader = new StreamReader(bindingContext.HttpContext.Request.Body, encoding: Encoding.UTF8);
            var body = await reader.ReadToEndAsync();
            bindingContext.Result = ModelBindingResult.Success(JsonConvert.DeserializeObject(value: body, type: bindingContext.ModelType));
            bindingContext.HttpContext.Request.RouteValues["request_body"] = body;
        }
    }
}
