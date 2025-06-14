namespace PreviewFramework.App.Maui.ViewModels;

public class UIComponentCategoryViewModel : ExamplesItemViewModel
{
    public string Name { get; }

    public UIComponentCategoryViewModel(UIComponentCategory category)
    {
        Name = category.Name;
    }
}
