using System;
using System.Collections.Generic;
using System.ComponentModel;
using PreviewFramework.App;

namespace PreviewFramework.App.Maui.ViewModels;

public class ExamplesViewModel // : INotifyPropertyChanged
{
    public static readonly UIComponentCategory UncategorizedCategory = new("Uncategorized");
    private static readonly Lazy<ExamplesViewModel> s_lazyInstance = new Lazy<ExamplesViewModel>(() => new ExamplesViewModel());

    public static ExamplesViewModel Instance => s_lazyInstance.Value;

    public IReadOnlyList<ExamplesItemViewModel> ExamplesItems { get; }
    public bool HasCategories { get; }

    private ExamplesViewModel()
    {
        var categories = new List<UIComponentCategory>();
        Dictionary<UIComponentCategory, List<UIComponentReflection>> uiComponentsByCategory = [];

        UIComponentsManagerReflection uiComponentsManager = MauiExampleApplication.Instance.GetUIComponentsManager();

        // Create a list of UIComponents for each category, including an "Uncategorized" category.
        foreach (UIComponentReflection uiComponent in uiComponentsManager.UIComponents)
        {
            UIComponentCategory? category = uiComponent.Category;

            category ??= UncategorizedCategory;

            if (!uiComponentsByCategory.TryGetValue(category, out List<UIComponentReflection>? uiComponentsForCategory))
            {
                categories.Add(category);
                uiComponentsForCategory = [];
                uiComponentsByCategory.Add(category, uiComponentsForCategory);
            }

            uiComponentsForCategory.Add(uiComponent);
        }

        // Sort the categories and components
        categories.Sort((category1, category2) => string.Compare(category1.Name, category2.Name, StringComparison.CurrentCultureIgnoreCase));
        foreach (List<UIComponentReflection> componentsForCategory in uiComponentsByCategory.Values)
        {
            componentsForCategory.Sort((component1, component2) => string.Compare(component1.DisplayName, component2.DisplayName, StringComparison.CurrentCultureIgnoreCase));
        }

        var examplesItems = new List<ExamplesItemViewModel>();

        HasCategories = categories.Count > 1;

        foreach (UIComponentCategory category in categories)
        {
            if (HasCategories)
            {
                examplesItems.Add(new UIComponentCategoryViewModel(category));
            }

            foreach (UIComponentReflection uiComponent in uiComponentsByCategory[category])
            {
                examplesItems.Add(new UIComponentViewModel(uiComponent));

                if (uiComponent.HasMultipleExamples)
                {
                    foreach (ExampleReflection example in uiComponent.Examples)
                    {
                        examplesItems.Add(new ExampleViewModel(uiComponent, example));
                    }
                }
            }
        }

        ExamplesItems = examplesItems;
    }

    public void NavigateToExample(UIComponentReflection uiComponent, ExampleReflection example)
    {
        MauiExampleApplication.Instance.ExampleNavigatorService.NavigateToExample(uiComponent, example);
    }
}
