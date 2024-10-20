using System.Windows.Input;

namespace ExampleFramework.App.Maui.ViewModels
{
    public class ExampleViewModel
    {
        public string Title => this.Example.DisplayName;

        public AppUIExample Example { get; }

        public ICommand TapCommand { get; }

        public ExampleViewModel(AppUIExample example)
        {
            this.Example = example;

            this.TapCommand = new Command(
                execute: () =>
                {
                    _ = ExamplesViewModel.Instance.ExampleNavigatorService.NavigateToExampleAsync(this.Example).ConfigureAwait(false);
                }
            );
        }
    }
}
