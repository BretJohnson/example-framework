using System.Threading.Tasks;

namespace PreviewFramework;

public interface IExampleAppService
{
    public Task NavigateToExampleAsync(string componentName, string exampleName);

    public Task<string[]> GetUIComponentExamplesAsync(string componentName);
}
