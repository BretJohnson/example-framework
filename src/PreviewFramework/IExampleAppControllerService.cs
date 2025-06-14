using System.Threading.Tasks;

namespace PreviewFramework;

public interface IExampleAppControllerService
{
    public Task RegisterAppAsync(string? projectPath, string applicationName, string platformName);
}
