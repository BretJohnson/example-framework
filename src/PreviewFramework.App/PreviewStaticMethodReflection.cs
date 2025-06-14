﻿using System;
using System.Reflection;

namespace PreviewFramework.App;

public class PreviewStaticMethodReflection : PreviewReflection
{
    public MethodInfo MethodInfo { get; }

    public PreviewStaticMethodReflection(PreviewAttribute previewAttribute, MethodInfo methodInfo) : base(previewAttribute)
    {
        MethodInfo = methodInfo;
    }

    public override object Create()
    {
        if (MethodInfo.GetParameters().Length != 0)
            throw new InvalidOperationException($"Previews that take parameters aren't yet supported: {Name}");

        return MethodInfo.Invoke(null, null);
    }

    public override Type? DefaultUIComponentType
    {
        get
        {
            Type type = MethodInfo.ReturnType;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(RoutePreview<>))
            {
                return type.GetGenericArguments()[0];
            }

            return type;
        }
    }

    /// <summary>
    /// FullName is intended to be what's used by the code to identify the preview. It's the preview's
    /// full qualified method name.
    /// </summary>
    public override string Name => MethodInfo.DeclaringType.FullName + "." + MethodInfo.Name;
}
