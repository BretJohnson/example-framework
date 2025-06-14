using ExampleFramework.Tooling;

namespace ExampleFramework.DevTools.ViewModels.NavTree;

public class ExampleViewModel(Example example) : NavTreeItemViewModel
{
    public override string DisplayName => example.DisplayName;

    public override string Icon => "";

    public override ObservableCollection<NavTreeItemViewModel>? Children => null;
}
