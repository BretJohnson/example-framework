using Microsoft.UI.Xaml.Data;

namespace PreviewFramework.DevTools.ViewModels;

[Bindable]
public abstract partial class NavTreeItemViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isExpanded;

    [ObservableProperty]
    private bool _isSelected;

    public NavTreeItemViewModel()
    {
        // Initialize commands
        ToggleExpandCommand = new RelayCommand(ToggleExpand);
        SelectItemCommand = new RelayCommand(SelectItem);
    }

    public string Content => DisplayName;

    public abstract string DisplayName { get; }
    public abstract string Icon { get; }
    public virtual ObservableCollection<NavTreeItemViewModel>? Children { get; }
    public virtual bool HasChildren => Children?.Count > 0;
    public virtual bool IsExpandable => HasChildren;

    public ICommand ToggleExpandCommand { get; }
    public ICommand SelectItemCommand { get; }

    private void ToggleExpand()
    {
        if (IsExpandable)
        {
            IsExpanded = !IsExpanded;
        }
    }

    private void SelectItem()
    {
        MainPageViewModel mainPageViewMode = DevToolsManager.Instance.MainPageViewModel;





        // Find the root item to handle deselection of previously selected items
        NavTreeItemViewModel? root = FindRootOfTree();
        if (root != null)
        {
            DeselectAllInTree(root);
        }

        // Select this item
        IsSelected = true;
    }

    private NavTreeItemViewModel? FindRootOfTree()
    {
        // In a real implementation, we would navigate to the root of the tree
        // This is a placeholder - in a production app, we'd need a more robust way
        // to find the root item or reference to the MainPageViewModel

        // For now, this is a simplified implementation
        return this;
    }

    private void DeselectAllInTree(NavTreeItemViewModel item)
    {
        // Deselect the current item
        if (item != this)
        {
            item.IsSelected = false;
        }

        // Recursively deselect all children
        if (item.Children != null)
        {
            foreach (var child in item.Children)
            {
                DeselectAllInTree(child);
            }
        }
    }
}
