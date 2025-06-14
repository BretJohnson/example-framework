namespace PreviewFramework.DevTools.ViewModels.NavTree;

public class SectionItemViewModel(string displayName, string icon = "") : NavTreeItemViewModel
{
    public override string DisplayName { get; } = displayName;

    public override string Icon { get; } = icon;

    public override ObservableCollection<NavTreeItemViewModel> Children { get; } = new ObservableCollection<NavTreeItemViewModel>();
}
