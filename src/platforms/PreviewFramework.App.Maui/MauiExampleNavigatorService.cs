using System;
using System.Threading.Tasks;
using PreviewFramework.App;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace PreviewFramework.App.Maui;

public class MauiExampleNavigatorService
{
    public bool NavigateAnimationsEnabled { get; set; } = false;

    public virtual void NavigateToExample(UIComponentReflection uiComponent, ExampleReflection example)
    {
        _ = NavigateToExampleAsync(uiComponent, example);
    }

    public virtual async Task NavigateToExampleAsync(UIComponentReflection uiComponent, ExampleReflection example)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            object? exampleUI = example.Create();

            if (uiComponent.Kind == UIComponentKind.Control)
            {
                ContentPage controlsPage = new ContentPage
                {
                    Content = (View)exampleUI
                };

                await Application.Current!.MainPage!.Navigation.PushAsync(controlsPage, NavigateAnimationsEnabled);
            }
            else
            {
                if (exampleUI is RouteExample routeExample)
                {
                    Window? mainWindow = Application.Current!.Windows[0];
                    Shell? shell = mainWindow?.Page as Shell;

                    if (shell is null)
                    {
                        throw new InvalidOperationException("Main window doesn't use Shell");
                    }

                    await shell.GoToAsync(routeExample.Route, NavigateAnimationsEnabled);
                }
                else if (exampleUI is ContentPage contentPage)
                {
                    //MauiExamplesApplication.Instance.Application.MainPage = contentPage;
                    await Application.Current!.MainPage!.Navigation.PushAsync(contentPage, NavigateAnimationsEnabled);
                }
            }
        });
    }
}
