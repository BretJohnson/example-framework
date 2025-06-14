using System;

namespace PreviewFramework;

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class ExampleApplicationClassAttribute(Type exampleApplicationClass) : Attribute
{
    public static string TypeFullName { get; } = NameUtilities.NormalizeTypeFullName(typeof(ExampleApplicationClassAttribute));

    public Type ExampleApplicationClass { get; } = exampleApplicationClass;
}
