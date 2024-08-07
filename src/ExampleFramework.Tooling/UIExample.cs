﻿using System.Data;
using VisualTestUtils;

namespace ExampleFramework.Tooling;

public abstract class UIExample
{
    private readonly string? _title;
    private readonly Type? _uiComponentType;
    //private Dictionary<string, ImageSnapshot?>? _snapshotsByEnvironment;

    public UIExample(UIExampleAttribute uiExampleAttribute)
    {
        _title = uiExampleAttribute.Title;
        _uiComponentType = uiExampleAttribute.UIComponentType;
    }

    /// <summary>
    /// Create an instance of the example. Normally this returns an instance of a UI framework control/page, suitable
    /// for display.
    /// </summary>
    /// <returns>instantiated example</returns>
    public abstract object Create();

    /// <summary>
    /// Title is intended to be what's shown in the UI to identify the example. It can contain spaces and
    /// isn't necessarily unique. It defaults to the example method/class name but can be overridden by
    /// the developer.
    /// </summary>
    public string Title => _title ?? this.DefaultTitle;

    /// <summary>
    /// Default title based on the example method/class name, ignoring any title override.
    /// </summary>
    public abstract string DefaultTitle { get; }

    public Type UIComponentType
    {
        get
        {
            if (_uiComponentType != null)
                return _uiComponentType;

            Type? defaultUIComponentType = this.DefaultUIComponentType;
            if (defaultUIComponentType == null)
                throw new InvalidOperationException($"No DefaultUIComponentType specified for example: {this.FullName}");
            else return defaultUIComponentType;
        }
    }

    /// <summary>
    /// Default component type (when there is one), e.g. based on the method return type. If there's no default
    /// type, this will be null.
    /// </summary>
    public abstract Type? DefaultUIComponentType { get; }

    /// <summary>
    /// FullName is intended to be what's used by the code to identify the example. It's the example's
    /// full qualified method name.
    /// </summary>
    public abstract string FullName { get; }
}
