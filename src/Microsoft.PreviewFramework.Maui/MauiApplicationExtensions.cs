﻿namespace Microsoft.PreviewFramework.Maui;

public static class MauiApplicationExtensions
{
    public static void EnablePreviewMode(this Application application)
    {
        MauiPreviewsApplication.Init(application);
    }
}
