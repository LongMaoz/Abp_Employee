using Domain.Shared.Attributes;
using System;
using System.Reflection;

namespace Domain.Shared.Extends;

public static class EnumExtensions
{
    public static string GetString(this Enum value)
    {
        Type type = value.GetType();
        string name = Enum.GetName(type, value);
        if (name != null)
        {
            FieldInfo? field = type.GetField(name);
            if (field != null)
            {
                if (Attribute.GetCustomAttribute(field,
                        typeof(EnumStringAttribute)) is EnumStringAttribute attr)
                {
                    return attr.StringValue;
                }
            }
        }
        return null;
    }
}