
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;


namespace Config
{
    public class CustomContractResolver : CamelCasePropertyNamesContractResolver
    {
        ///// <summary>
        ///// 实现首字母小写
        ///// </summary>
        ///// <param name="propertyName"></param>
        ///// <returns></returns>
        //protected override string ResolvePropertyName(string propertyName)
        //{
        //    return propertyName.Substring(0, 1).ToLower() + propertyName.Substring(1);
        //}

        /// <summary>
        /// 对长整型做处理
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (objectType == typeof(long))
            {
                return new JsonConverterLong();
            }
            return base.ResolveContractConverter(objectType);
        }
    }
}
