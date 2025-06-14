using PreviewFramework.App.Maui.ViewModels;
using Microsoft.Maui.Controls;

namespace PreviewFramework.App.Maui.Pages;

public partial class ExamplesPage : ContentPage
{
    public ExamplesPage()
    {
        InitializeComponent();
        BindingContext = ExamplesViewModel.Instance;
    }

    // Currently, when this source is embedded in the Visual Studio tap assemblies, then the MAUI .xaml source generator
    // isn't called. In that scenario, in app UI shouldn't work and the tap defines MICROSOFT_PREVIEW_IN_TAP so it compiles
#if MICROSOFT_PREVIEW_IN_TAP
    private void InitializeComponent()
    {
        throw new System.InvalidOperationException("This method should never be called.");
    }
#endif
}
