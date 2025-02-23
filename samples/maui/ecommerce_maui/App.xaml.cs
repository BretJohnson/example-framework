﻿using EcommerceMAUI.Views;
using Microsoft.PreviewFramework.Maui;

namespace EcommerceMAUI;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        Current.UserAppTheme = AppTheme.Light;
        MainPage = new LoginView();

#if EXAMPLES
        this.EnableExamplesMode();
#endif
    }
}
