namespace PreviewFramework.Tooling;

public abstract class Example(string name, string? displayName) : ExampleBase(displayName)
{
    private readonly string _name = name;

    public override string Name => _name;
}
