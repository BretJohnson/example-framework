using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace PreviewFramework.App.Maui.Utilities;

public static class IconUtilities
{
    /// <summary>
    /// Returns the appropriate icon based on the current application theme
    /// </summary>
    /// <returns>The appropriate icon for the current theme</returns>
    public static string GetIcon(string baseName)
    {
        string suffix = Application.Current?.RequestedTheme == AppTheme.Dark ? "dark" : "light";
        return $"ic_preview_{baseName}__{suffix}.png";
    }
}
