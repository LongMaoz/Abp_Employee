using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Linq;
using System.Reflection;

namespace ApiCore.EmployeeManagement.Converter;

/// <summary>
/// 自定义枚举Json转换
/// </summary>
public class CustomStringEnumConverter : StringEnumConverter
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <param name="serializer"></param>
    /// <exception cref="ArgumentException"></exception>
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        Type type = value.GetType();
        if (!type.IsEnum)
        {
            throw new ArgumentException("CustomStringEnumConverter can only be used with enum types.");
        }

        // 尝试获取枚举值上的JsonProperty属性
        string? enumText = value.GetType()
            .GetMember(value.ToString())
            .FirstOrDefault()?
            .GetCustomAttributes(typeof(JsonPropertyAttribute), false)
            .Cast<JsonPropertyAttribute>()
            .FirstOrDefault()?.PropertyName;

        // 如果没有JsonProperty属性，则将枚举值的第一个字母转换为小写
        if (string.IsNullOrEmpty(enumText))
        {
            enumText = value.ToString();
            if (!string.IsNullOrEmpty(enumText) && char.IsUpper(enumText[0]))
            {
                enumText = char.ToLowerInvariant(enumText[0]) + enumText.Substring(1);
            }
        }

        writer.WriteValue(enumText);
    }
}