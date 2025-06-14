namespace PreviewFramework;

public class UIComponentExamplePair<TUIComponent, TExample>(TUIComponent uiComponent, TExample example)
    where TExample : ExampleBase
    where TUIComponent : UIComponentBase<TExample>
{
    public TUIComponent UIComponent => uiComponent;

    public TExample Example => example;
}
