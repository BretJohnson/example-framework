﻿namespace ExampleFramework.App.Maui;

public static class MauiApplicationExtensions
{
    public static void EnableExamplesMode(this Application application)
    {
        MauiExamplesApplication.Init(application);
    }
}
