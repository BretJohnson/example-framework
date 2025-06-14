namespace PreviewFramework;

public class RouteExample(string route)
{
    public string Route { get; } = route;
}

public class RouteExample<T>(string route) : RouteExample(route) where T : class
{
}
