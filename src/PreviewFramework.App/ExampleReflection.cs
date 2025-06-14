using System;

namespace PreviewFramework.App;

public abstract class ExampleReflection : ExampleBase
{
    private readonly Type? _uiComponentType;
    //private Dictionary<string, ImageSnapshot?>? _snapshotsByEnvironment;

    public ExampleReflection(ExampleAttribute exampleAttribute) : base(exampleAttribute.DisplayName)
    {
        _uiComponentType = exampleAttribute.UIComponentType;
    }

    public ExampleReflection(Type uiComponentType) : base(null)
    {
        _uiComponentType = uiComponentType;
    }

    /// <summary>
    /// Create an instance of the example. Normally this returns an instance of a UI framework control/page, suitable
    /// for display.
    /// </summary>
    /// <returns>instantiated example</returns>
    public abstract object Create();

    public Type UIComponentType
    {
        get
        {
            if (_uiComponentType != null)
            {
                return _uiComponentType;
            }

            Type? defaultUIComponentType = DefaultUIComponentType;
            if (defaultUIComponentType == null)
                throw new InvalidOperationException($"No DefaultUIComponentType specified for example: {Name}");
            else return defaultUIComponentType;
        }
    }

    /// <summary>
    /// Default component type (when there is one), e.g. based on the method return type. If there's no default
    /// type, this will be null.
    /// </summary>
    public abstract Type? DefaultUIComponentType { get; }
}
