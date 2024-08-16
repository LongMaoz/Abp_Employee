using System;
namespace Domain.Shared.Attributes;


[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public class EnumStringAttribute(string? value) : Attribute
{
    public string? StringValue { get; } = value;
}