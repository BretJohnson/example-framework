using PreviewFramework.App.Maui.ViewModels;
using Microsoft.Maui.Controls;

namespace PreviewFramework.App.Maui.Pages;

public class ExampleItemDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate? UIComponentTemplate { get; set; }
    public DataTemplate? ExampleTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container) =>
        (item is UIComponentViewModel) ? UIComponentTemplate! : ExampleTemplate!;
}
