using System;

namespace PreviewFramework.App;

public class ExampleNotFoundException : Exception
{
    public ExampleNotFoundException(string message) : base(message)
    {
    }
}
