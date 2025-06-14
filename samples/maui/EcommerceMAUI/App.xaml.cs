using EcommerceMAUI.Views;
using PreviewFramework.App;
using PreviewFramework.App.Maui;

namespace EcommerceMAUI;

public partial class App : Application
{
    public App()
    {
#if EXAMPLES
        MauiExampleApplication.EnsureInitialized();
#endif

        InitializeComponent();
        Current.UserAppTheme = AppTheme.Light;
        MainPage = new LoginView();
    }

    public static void Initialize()
    {
        // This method is called to initialize the app.
        // You can add any initialization code here if needed.
    }
}
