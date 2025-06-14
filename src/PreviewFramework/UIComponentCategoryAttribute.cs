using System;

namespace PreviewFramework;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class UIComponentCategoryAttribute : Attribute
{
    /// <summary>
    /// Optional title for the example, determining how it appears in navigation UI.
    /// "/" delimiters can be used to indicate hierarchy.
    /// </summary>
    public string Name { get; }
    public Type[] UIComponentTypes { get; }

    public UIComponentCategoryAttribute(string name, params Type[] uiComponents)
    {
        Name = name;
        UIComponentTypes = uiComponents;
    }
}
