using System.Threading.Tasks;
using PreviewFramework.App;

namespace PreviewFramework.App.Maui;

public class MauiExampleAppService(MauiExampleApplication mauiExampleApplication) : ExampleAppService(mauiExampleApplication)
{
    public override async Task NavigateToExampleAsync(string uiComponentName, string exampleName)
    {
        UIComponentExamplePairReflection uiComponentExamplePair = GetUIComponentExamplePair(uiComponentName, exampleName);
        await MauiExampleApplication.Instance.ExampleNavigatorService.
            NavigateToExampleAsync(uiComponentExamplePair.UIComponent, uiComponentExamplePair.Example).ConfigureAwait(false);
    }
}
