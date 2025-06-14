using System;

namespace PreviewFramework;

/// <summary>
/// An attribute that specifies this is an example, for a control or other UI.
/// Examples can be shown in a gallery viewer, doc, etc.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ExampleAttribute : Attribute
{
    public static string TypeFullName => NameUtilities.NormalizeTypeFullName(typeof(ExampleAttribute));

    /// <summary>
    /// Optional title for the example, determining how it appears in navigation UI.
    /// "/" delimiters can be used to indicate hierarchy.
    /// </summary>
    public string? DisplayName { get; }

    public Type? UIComponentType { get; }

    public ExampleAttribute()
    {
    }

    public ExampleAttribute(string? displayName = null, Type? uiComponent = null)
    {
        DisplayName = displayName;
        UIComponentType = uiComponent;
    }

    public ExampleAttribute(Type uiComponent)
    {
        UIComponentType = uiComponent;
    }
}
