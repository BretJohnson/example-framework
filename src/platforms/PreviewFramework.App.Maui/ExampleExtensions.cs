using Microsoft.Maui.Controls;

namespace PreviewFramework.App.Maui;

public static class ExampleExtensions
{
    /// <summary>
    /// Creates a new instance of a view of type <typeparamref name="TView"/> and sets its binding context
    /// </summary>
    /// <typeparam name="TView">The type of the view to create. Must inherit from <see cref="View"/> and have a parameterless constructor.</typeparam>
    /// <param name="bindingContext">The binding context to assign to the created view.</param>
    /// <returns>A new instance of the view with the specified binding context.</returns>
    /// <remarks>
    /// This method simplifies the creation of control/page examples, setting the binding context as appropriate
    /// for the example data.
    /// </remarks>
    public static TView CreateViewWithBinding<TView>(object bindingContext) where TView : View, new()
    {
        TView view = new TView();
        view.BindingContext = bindingContext;
        return view;
    }

    public static TView CreateViewWithBindingToService<TView, TService>()
        where TView : View, new()
        where TService : class
    {
        TService service = MauiExampleApplication.Instance.GetRequiredService<TService>();
        return CreateViewWithBinding<TView>(service);
    }
}
