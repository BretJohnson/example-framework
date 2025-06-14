using System;
using System.Reflection;
using PreviewFramework.App;

namespace PreviewFramework.App.Maui;

public class MauiUIComponentExclusionFilter : IUIComponentExclusionFilter
{
    public bool ExcludeAssembly(Assembly assembly)
    {
        string? name = assembly.GetName().Name;
        return name != null && (name.StartsWith("Microsoft.Maui.") || name == "Microsoft.Maui");
    }

    public bool ExcludeType(Type type) => false;
}
