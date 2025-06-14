using System;

namespace PreviewFramework.App;

public class UIComponentNotFoundException : Exception
{
    public UIComponentNotFoundException(string message) : base(message)
    {
    }
}
