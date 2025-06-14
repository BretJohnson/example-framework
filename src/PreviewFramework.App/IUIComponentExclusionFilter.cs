using System;
using System.Reflection;

namespace PreviewFramework.App;

public interface IUIComponentExclusionFilter
{
    public bool ExcludeAssembly(Assembly assembly);

    public bool ExcludeType(Type type);
}
