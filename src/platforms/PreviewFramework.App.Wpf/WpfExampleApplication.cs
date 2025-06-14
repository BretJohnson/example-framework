using PreviewFramework;
using PreviewFramework.App;
using PreviewFramework.App.Wpf;

[assembly: ExampleApplicationClass(typeof(WpfExampleApplication))]

[assembly: PageUIComponentBaseType(WpfExampleApplication.WpfPlatformType, "System.Windows.Controls.Page")]
[assembly: PageUIComponentBaseType(WpfExampleApplication.WpfPlatformType, "System.Windows.Window")]
[assembly: ControlUIComponentBaseType(WpfExampleApplication.WpfPlatformType, "System.Windows.Media.Visual")]

namespace PreviewFramework.App.Wpf;

public class WpfExampleApplication : ExampleApplication
{
    public static WpfExampleApplication Instance => s_instance.Value;

    private static readonly Lazy<WpfExampleApplication> s_instance =
        new(() =>
        {
            var instance = new WpfExampleApplication();
            InitInstance(instance);
            return instance;
        });

    public const string WpfPlatformType = "WPF";

    private readonly Lazy<UIComponentsManagerReflection> _uiComponentsManager;

    private WpfExampleApplication()
    {
        _uiComponentsManager = new Lazy<UIComponentsManagerReflection>(
            () => new UIComponentsManagerReflection(ServiceProvider, AdditionalAppAssemblies,
            null));

        ExampleAppService = new WpfExampleAppService(this);
    }

    public WpfExampleAppService ExampleAppService { get; }

    public WpfExampleNavigatorService ExampleNavigatorService { get; set; } = new WpfExampleNavigatorService();

    public static void EnsureInitialized()
    {
        _ = Instance;
    }

    public override UIComponentsManagerReflection GetUIComponentsManager() => _uiComponentsManager.Value;

    public override ExampleAppService GetExampleAppService() => ExampleAppService;

}
