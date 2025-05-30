﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExampleFramework.App;

public class UIComponentsManagerReflection : UIComponentsManagerBase<UIComponentReflection, ExampleReflection>
{
    private readonly IServiceProvider? _serviceProvider = null;
    private readonly IUIComponentExclusionFilter? _exclusionFilter = null;

    /// <summary>
    /// Discover all the UI components and associated examples in the the app, via reflection.
    /// UI component / examples can be defined explicitly, using the [Example] attribute, or
    /// implicitly auto-generated, when there's for a default constructor or a constructor that can
    /// be resolved via Dependency Injection.
    /// <paramref name="serviceProvider">An optional <see cref="IServiceProvider"/> instance for dependency injection.</paramref>
    public UIComponentsManagerReflection(IServiceProvider? serviceProvider, IEnumerable<string> additionalAppAssemblies,
        IUIComponentExclusionFilter? exclusionFilter)
    {
        _serviceProvider = serviceProvider;
        _exclusionFilter = exclusionFilter;

        IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic);
        foreach (Assembly assembly in assemblies)
        {
            AddUIComponentBaseClassesFromAssembly(assembly);
        }

        AddUIComponentsFromAssembly(Assembly.GetEntryAssembly());

        HashSet<string> additionalAppAssemblesSet = new(additionalAppAssemblies, StringComparer.OrdinalIgnoreCase);
        foreach (Assembly assembly in assemblies)
        {
            string name = assembly.GetName().Name;
            if (additionalAppAssemblesSet.Contains(name))
            {
                AddUIComponentsFromAssembly(assembly);
            }
        }
    }

    public void AddUIComponentBaseClassesFromAssembly(Assembly assembly)
    {
        foreach (PageUIComponentBaseTypeAttribute pageUIComponentAttribute in assembly.GetCustomAttributes<PageUIComponentBaseTypeAttribute>())
        {
            _pageUIComponentBaseTypes.AddBaseType(pageUIComponentAttribute.Platform, pageUIComponentAttribute.BaseType);
        }

        foreach (ControlUIComponentBaseTypeAttribute controlUIComponentAttribute in assembly.GetCustomAttributes<ControlUIComponentBaseTypeAttribute>())
        {
            _controlUIComponentBaseTypes.AddBaseType(controlUIComponentAttribute.Platform, controlUIComponentAttribute.BaseType);
        }
    }

    public void AddUIComponentsFromAssembly(Assembly assembly)
    {
        IEnumerable<UIComponentCategoryAttribute> uiComponentCategoryAttributes = assembly.GetCustomAttributes<UIComponentCategoryAttribute>();
        foreach (UIComponentCategoryAttribute uiComponentCategoryAttribute in uiComponentCategoryAttributes)
        {
            UIComponentCategory category = GetOrAddCategory(uiComponentCategoryAttribute.Name);

            foreach (Type type in uiComponentCategoryAttribute.UIComponentTypes)
            {
                UIComponentReflection component = GetOrAddUIComponent(type);
                component.InitCategory(category);
            }
        }

        Type[] types = assembly.GetExportedTypes();
        foreach (Type type in types)
        {
            if (_exclusionFilter?.ExcludeType(type) == true)
            {
                continue;
            }

            ExampleAttribute? typeExampleAttribute = type.GetCustomAttribute<ExampleAttribute>(false);
            if (typeExampleAttribute != null)
            {
                AddExample(new ExampleClassReflection(typeExampleAttribute, type));
            }

            MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (MethodInfo method in methods)
            {
                ExampleAttribute? exampleAttribute = method.GetCustomAttribute<ExampleAttribute>(false);

                if (exampleAttribute != null)
                {
                    AddExample(new ExampleStaticMethodReflection(exampleAttribute, method));
                }
            }

            if (CanBeAutoGeneratedExample(type))
            {
                UIComponentReflection? uiComponent = GetUIComponent(type.FullName);

                // If the type doesn't have any examples and this class can be an auto-generated example, then add that
                if ((uiComponent == null || uiComponent.Examples.Count == 0) && CanBeAutoGeneratedExample(type))
                {
                    uiComponent ??= GetOrAddUIComponent(type);

                    var example = new ExampleClassReflection(type, isAutoGenerated: true);
                    AddExample(example);
                }
            }
        }
    }

    private UIComponentKind InferUIComponentKind(Type type)
    {
        Type baseType = type.BaseType;
        while (baseType != null)
        {
            if (IsUIComponentBaseType(baseType.FullName, out UIComponentKind kind))
            {
                return kind;
            }

            baseType = baseType.BaseType;
        }

        return UIComponentKind.Unknown;
    }

    private bool CanBeAutoGeneratedExample(Type type)
    {
        if (type.IsAbstract)
        {
            return false;
        }

        UIComponentKind kind = InferUIComponentKind(type);

        if (kind == UIComponentKind.Unknown)
        {
            return false;
        }

        // Abstract classes and interfaces cannot be directly instantiated. Also don't try to auto
        // create examples for non-public types.
        if (type.IsNotPublic || type.IsAbstract || type.IsInterface)
        {
            return false;
        }

        // Check all public constructors
        ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                               .Where(c => c.IsPublic)
                               .ToArray();

        if (constructors.Length == 0)
            return false;

        foreach (ConstructorInfo? constructor in constructors)
        {
            ParameterInfo[] parameters = constructor.GetParameters();

            // Parameterless constructor can be auto-generated
            if (parameters.Length == 0)
                return true;

            // Check if all parameters can be resolved via Dependency Injection

            if (_serviceProvider is null)
            {
                continue;
            }

            bool allParametersResolvable = true;
            foreach (ParameterInfo param in parameters)
            {
                try
                {
                    var service = _serviceProvider.GetService(param.ParameterType);
                    if (service is null && !param.IsOptional)
                    {
                        allParametersResolvable = false;
                        break;
                    }
                }
                catch
                {
                    allParametersResolvable = false;
                    break;
                }
            }

            if (allParametersResolvable)
                return true;
        }

        return false;
    }

    public UIComponentReflection GetOrAddUIComponent(Type type, UIComponentKind? kind = null)
    {
        string name = type.FullName;

        UIComponentReflection? component = GetUIComponent(name);
        if (component == null)
        {
            kind ??= InferUIComponentKind(type);

            component = new UIComponentReflection(type, kind.Value, null);
            AddUIComponent(component);
        }

        return component;
    }

    public void AddExample(ExampleReflection example)
    {
        UIComponentReflection component = GetOrAddUIComponent(example.UIComponentType);
        component.AddExample(example);
    }
}
