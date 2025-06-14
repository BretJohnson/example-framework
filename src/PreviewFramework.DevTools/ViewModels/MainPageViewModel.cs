using PreviewFramework.DevTools.ViewModels;
using PreviewFramework.DevTools.ViewModels.NavTree;
using PreviewFramework.Tooling;
using Microsoft.UI.Xaml.Data;

namespace PreviewFramework.DevTools.Presentation;

[Bindable]
public partial class MainPageViewModel : ObservableObject
{
    private readonly INavigator _navigator;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<NavTreeItemViewModel> _navTreeItems = [];

    public MainPageViewModel(IOptions<AppConfig> appInfo, INavigator navigator)
    {
        DevToolsManager.Instance.MainPageViewModel = this;

        _navigator = navigator;
        Title = "Examples";

        // Initialize commands
        PlayCommand = new RelayCommand(Play);
        SettingsCommand = new RelayCommand(Settings);

        // Initialize sample data
        InitializeNavTreeItems();
    }

    public string Title { get; }

    public ICommand PlayCommand { get; }

    public ICommand SettingsCommand { get; }

    private void InitializeNavTreeItems()
    {
        NavTreeItems.Clear();

        foreach (UIComponent uiComponent in DevToolsManager.Instance.UIComponentsManager.UIComponents)
        {
            NavTreeItems.Add(new UIComponentViewModel(uiComponent));
        }
    }

#if LATER
    private void InitializeNavTreeItemsWithSampleData()
    {
        NavTreeItems.Clear();

        // Introduction item
        NavTreeItems.Add(new UIComponentViewModel("Introduction", "📄"));

        // APPLICATION section
        var applicationSection = new SectionItemViewModel("APPLICATION", "");
        applicationSection.IsExpanded = true;

        // ProductCard
        applicationSection.Children.Add(new UIComponentViewModel("ProductCard", "🧩"));

        // Documentation with sub-items
        var documentationItem = new UIComponentViewModel("Documentation", "📋", new ObservableCollection<NavTreeItemViewModel>
        {
            new ExampleViewModel("Default", "📄"),
            new ExampleViewModel("Expanded", "📄"),
            new ExampleViewModel("Added to cart", "📄")
        });
        documentationItem.IsExpanded = true;
        documentationItem.IsSelected = true; // This matches the blue highlight in screenshot
        applicationSection.Children.Add(documentationItem);

        // Other application items
        applicationSection.Children.Add(new UIComponentViewModel("Dashboard", "🧩"));
        applicationSection.Children.Add(new UIComponentViewModel("Homepage", "🧩"));
        applicationSection.Children.Add(new UIComponentViewModel("User Profile", "🧩"));
        applicationSection.Children.Add(new UIComponentViewModel("Sign In", "🧩"));

        NavTreeItems.Add(applicationSection);

        // DESIGN SYSTEM section
        var designSystemSection = new SectionItemViewModel("DESIGN SYSTEM", "");
        designSystemSection.IsExpanded = true;

        designSystemSection.Children.Add(new UIComponentViewModel("ActivityList", "📁"));
        designSystemSection.Children.Add(new UIComponentViewModel("Form", "☐"));
        designSystemSection.Children.Add(new UIComponentViewModel("Avatar", "🧩"));
        designSystemSection.Children.Add(new UIComponentViewModel("Button", "🧩"));
        designSystemSection.Children.Add(new UIComponentViewModel("Footer", "🧩"));
        designSystemSection.Children.Add(new UIComponentViewModel("Header", "🧩"));
        designSystemSection.Children.Add(new UIComponentViewModel("Pagination", "🧩"));

        NavTreeItems.Add(designSystemSection);
    }
#endif

    private void Play()
    {
        bool success = DevToolsManager.Instance.Run();

        if (!success)
        {
            // TODO: Show error message to user (could use a notification service or dialog)
            // For now, this will be logged by the DevToolsManager
        }
    }

    private void Settings()
    {
        // TODO: Implement settings functionality
    }

    partial void OnSearchTextChanged(string value)
    {
        // TODO: Implement search filtering
    }
}
