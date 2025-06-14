using System;
using System.Linq;
using System.Threading.Tasks;

namespace PreviewFramework.App;

public abstract class ExampleAppService(ExampleApplication exampleApplication) : IExampleAppService
{
    public abstract Task NavigateToExampleAsync(string uiComponentName, string exampleName);

    protected UIComponentReflection GetUIComponent(string uiComponentName)
    {
        return exampleApplication.GetUIComponentsManager().GetUIComponent(uiComponentName) ??
            throw new UIComponentNotFoundException($"UIComponent {uiComponentName} not found");
    }

    protected UIComponentReflection? GetUIComponentIfExists(string uiComponentName)
    {
        return exampleApplication.GetUIComponentsManager().GetUIComponent(uiComponentName);
    }

    public Task<string[]> GetUIComponentExamplesAsync(string componentName)
    {
        UIComponentReflection? component = GetUIComponentIfExists(componentName);
        if (component is null)
        {
            return Task.FromResult(Array.Empty<string>());
        }

        string[] exampleNames = component.Examples.Select(example => example.Name).ToArray();
        return Task.FromResult(exampleNames);
    }

    protected UIComponentExamplePairReflection GetUIComponentExamplePair(string uiComponentName, string exampleName)
    {
        UIComponentReflection uiComponent = GetUIComponent(uiComponentName);
        ExampleReflection example = uiComponent.GetExample(exampleName) ?? throw new ExampleNotFoundException($"Example {exampleName} not found for UIComponent {uiComponentName}");
        return new UIComponentExamplePairReflection(uiComponent, example);
    }
}
