using System;
using Microsoft.Maui;
using Microsoft.UI.Input;
using Windows.System;
using Windows.UI.Core;

namespace PreviewFramework.App.Maui;

public partial class MauiExampleApplication
{
    private void AddKeyboardHandling()
    {
        // Register a handler for the window
        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping("GlobalKeyboardEvents", (handler, window) =>
        {
            if (handler.PlatformView is Microsoft.UI.Xaml.Window winUIWindow)
            {
                // Access the content and attach events
                if (winUIWindow.Content is Microsoft.UI.Xaml.Controls.Panel panel)
                {
                    panel.KeyDown += OnGlobalKeyDown;
                }
            }
        });
    }

    private static void OnGlobalKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        bool isCtrlDown = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down);
        bool isShiftDown = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down);

        // Handle global keyboard shortcuts
        if (e.Key == VirtualKey.Z && isCtrlDown && isShiftDown)
        {
            Instance.ShowExampleUIWindow();
            e.Handled = true;
        }
    }
}
