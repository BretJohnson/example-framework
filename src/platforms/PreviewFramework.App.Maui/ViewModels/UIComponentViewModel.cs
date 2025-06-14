using System.Windows.Input;
using PreviewFramework.App;
using PreviewFramework.App.Maui.Utilities;
using Microsoft.Maui.Controls;

namespace PreviewFramework.App.Maui.ViewModels;

public class UIComponentViewModel : ExamplesItemViewModel
{
    public UIComponentViewModel(UIComponentReflection uiComponent)
    {
        UIComponent = uiComponent;

        TapCommand = new Command(
            execute: () =>
            {
                if (UIComponent.HasSingleExample)
                {
                    ExampleReflection example = UIComponent.Examples[0];
                    ExamplesViewModel.Instance.NavigateToExample(UIComponent, example);
                }
            }
        );
    }

    public UIComponentReflection UIComponent { get; }

    public ICommand TapCommand { get; }

    public string DisplayName => UIComponent.DisplayName;

    public string Icon => IconUtilities.GetIcon(UIComponent.Kind == UIComponentKind.Control ? "control" : "page");
}
