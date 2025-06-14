using System;
using PreviewFramework;
using PreviewFramework.App.Maui;
using PreviewFramework.App.Maui.Pages;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;

#if !MICROSOFT_PREVIEW_IN_TAP
[assembly: ExampleApplicationClass(typeof(MauiExampleApplication))]

[assembly: PageUIComponentBaseType(MauiExampleApplication.MauiPlatformType, "Microsoft.Maui.Controls.Page")]
[assembly: ControlUIComponentBaseType(MauiExampleApplication.MauiPlatformType, "Microsoft.Maui.Controls.View")]
#endif

namespace PreviewFramework.App.Maui;

public partial class MauiExampleApplication : ExampleApplication
{
    public static MauiExampleApplication Instance => s_instance.Value;

    private static readonly Lazy<MauiExampleApplication> s_instance =
        new(() =>
        {
            var instance = new MauiExampleApplication();
            InitInstance(instance);
            return instance;
        });

    public const string MauiPlatformType = "MAUI";

    private readonly Lazy<UIComponentsManagerReflection> _uiComponentsManager;

    private MauiExampleApplication()
    {
        // Use application default IServiceProvider, if available
        IElement? applicationElement = Application.Current;
        ServiceProvider = applicationElement?.Handler?.MauiContext?.Services;

        _uiComponentsManager = new Lazy<UIComponentsManagerReflection>(
            () => new UIComponentsManagerReflection(ServiceProvider, AdditionalAppAssemblies,
            new MauiUIComponentExclusionFilter()));

        ExampleAppService = new MauiExampleAppService(this);

        ApplicationName = AppInfo.Current.Name;
        PlatformName = DeviceInfo.Current.Platform.ToString();

#if WINDOWS
        AddKeyboardHandling();
#endif
    }

    public MauiExampleAppService ExampleAppService { get; }

    public MauiExampleNavigatorService ExampleNavigatorService { get; set; } = new MauiExampleNavigatorService();

    public Window? ExampleUIWindow { get; private set; }

    public static void EnsureInitialized()
    {
        _ = Instance;
    }

    public override UIComponentsManagerReflection GetUIComponentsManager() => _uiComponentsManager.Value;

    public override ExampleAppService GetExampleAppService() => ExampleAppService;

    public override string ApplicationName { get; set; }

    public override string PlatformName { get; set; }

    public void AddExampleUIShellItem(Shell shell, string title = "Examples", string? icon = null)
    {
        var examplesShellContent = new ShellContent
        {
            Title = title,
            Icon = icon,
            Route = "UIExamples",
            ContentTemplate = new DataTemplate(typeof(ExamplesPage))
        };

        shell.Items.Add(examplesShellContent);
    }

    public void ShowExampleUIWindow()
    {
        if (ExampleUIWindow is null)
        {
            ExampleUIWindow = new Window(new ExamplesPage());
            ExampleUIWindow.Title = "UI Examples";
            ExampleUIWindow.Destroying += ExampleUIWindow_Destroying;

            ExampleUIWindow.Width = 320;
            ExampleUIWindow.Height = 500;

            Window mainWindow = Application.Current!.Windows[0];
            if (mainWindow is not null)
            {
                // Position the window just left of the top left corner of the app window, but ensuring
                // it's fully on the screen
                ExampleUIWindow.X = double.Max(mainWindow.X - ExampleUIWindow.Width - 5, 0);
                ExampleUIWindow.Y = double.Max(mainWindow.Y, 0);
            }

            Application.Current?.OpenWindow(ExampleUIWindow);
        }
    }

    private void ExampleUIWindow_Destroying(object? sender, EventArgs e)
    {
        ExampleUIWindow = null;
    }
}
