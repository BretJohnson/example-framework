using System;

namespace PreviewFramework;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class ControlUIComponentBaseTypeAttribute(string platform, string baseType) : Attribute
{
    public static string TypeFullName => NameUtilities.NormalizeTypeFullName(typeof(ControlUIComponentBaseTypeAttribute));

    public string Platform { get; } = platform;

    public string BaseType { get; } = baseType;
}
