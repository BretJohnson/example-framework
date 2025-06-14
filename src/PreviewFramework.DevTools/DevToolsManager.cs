using PreviewFramework.Tooling;
using System.Diagnostics;

namespace PreviewFramework.DevTools;

/// <summary>
/// Singleton manager class for global application state for the PreviewFramework DevTools app.
/// </summary>
public class DevToolsManager
{
    private static readonly Lazy<DevToolsManager> s_instance = new(() => new DevToolsManager());
    private readonly ILogger<DevToolsManager> _logger;
    private IServiceProvider? _serviceProvider;
    private UIComponentsManager _uiComponentsManager;
    private string? _projectPath;

    /// <summary>
    /// Gets the singleton instance of the DevToolsManager.
    /// </summary>
    public static DevToolsManager Instance => s_instance.Value;

    /// <summary>
    /// Gets the UIComponentsManager instance.
    /// </summary>
    public UIComponentsManager UIComponentsManager => _uiComponentsManager;

    /// <summary>
    /// Get the current project path, if there is one.
    /// </summary>
    public string? ProjectPath => _projectPath;

    /// <summary>
    /// Private constructor to enforce singleton pattern.
    /// </summary>
    private DevToolsManager()
    {
        // Create a default logger if not provided through Initialize
        _logger = LoggerFactory.Create(builder => builder.AddDebug()).CreateLogger<DevToolsManager>();
        _logger.LogInformation("DevToolsManager instance created");

        _projectPath = @"Q:\\src\\example-framework\\samples\\maui\\EcommerceMAUI\\EcommerceMAUI.csproj";

        _uiComponentsManager = UIComponentsManager.CreateFromProjectAsync(_projectPath).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Gets a value indicating whether the manager has been initialized.
    /// </summary>
    public bool IsInitialized { get; private set; }

    public MainPageViewModel MainPageViewModel { get; set; } = null!;

    /// <summary>
    /// Gets the service provider for dependency resolution.
    /// </summary>
    public IServiceProvider ServiceProvider => _serviceProvider ??
        throw new InvalidOperationException("DevToolsManager has not been initialized. Call Initialize before using.");

    /// <summary>
    /// Gets or sets the current theme (light/dark).
    /// </summary>
    public string CurrentTheme { get; set; } = "Light";

    /// <summary>
    /// Initializes the DevToolsManager with services from the application.
    /// </summary>
    /// <param name="serviceProvider">The application's service provider.</param>
    public void Initialize(IServiceProvider serviceProvider)
    {
        if (IsInitialized)
        {
            _logger.LogWarning("DevToolsManager already initialized");
            return;
        }

        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        IsInitialized = true;

        // Get a better logger from DI
        ILoggerFactory? loggerFactory = serviceProvider.GetService<ILoggerFactory>();
        if (loggerFactory is not null)
        {
            // Replace the default logger with one from DI
            ILogger<DevToolsManager> logger = loggerFactory.CreateLogger<DevToolsManager>();
            logger.LogInformation("DevToolsManager initialized with application services");
        }

        _logger.LogInformation("DevToolsManager initialized");
    }

    /// <summary>
    /// Gets a service of type T from the service provider.
    /// </summary>
    /// <typeparam name="T">The type of service to get.</typeparam>
    /// <returns>The service instance.</returns>
    public T GetService<T>() where T : notnull
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException("DevToolsManager has not been initialized. Call Initialize before using.");
        }

        T? service = ServiceProvider.GetService<T>();
        if (service is null)
        {
            throw new InvalidOperationException($"Service of type {typeof(T).Name} could not be resolved.");
        }

        return service;
    }

    /// <summary>
    /// Tries to get a service of type T from the service provider.
    /// </summary>
    /// <typeparam name="T">The type of service to get.</typeparam>
    /// <param name="service">The output service instance if found.</param>
    /// <returns>True if the service was found, false otherwise.</returns>
    public bool TryGetService<T>(out T? service) where T : class
    {
        if (!IsInitialized)
        {
            service = null;
            return false;
        }

        service = ServiceProvider.GetService<T>();
        return service is not null;
    }

    /// <summary>
    /// Runs the project by launching the csproj file.
    /// </summary>
    /// <returns>True if the process started successfully, false otherwise.</returns>
    public bool Run()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{_projectPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            var process = Process.Start(startInfo);
            if (process == null)
            {
                _logger.LogError("Failed to start the process");
                return false;
            }

            _logger.LogInformation("Project started successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running the project");
            return false;
        }
    }

    /// <summary>
    /// Updates the project path and reloads the UIComponentsManager.
    /// </summary>
    /// <param name="newProjectPath">The new project path to use.</param>
    /// <returns>True if the project was loaded successfully, false otherwise.</returns>
    public async Task<bool> UpdateProjectPathAsync(string newProjectPath)
    {
        if (string.IsNullOrWhiteSpace(newProjectPath))
        {
            _logger.LogError("Project path cannot be null or empty");
            return false;
        }

        if (!File.Exists(newProjectPath))
        {
            _logger.LogError("Project file does not exist: {ProjectPath}", newProjectPath);
            return false;
        }

        try
        {
            _logger.LogInformation("Updating project path to: {ProjectPath}", newProjectPath);

            // Update the project path
            _projectPath = newProjectPath;

            // Reload the UIComponentsManager with the new project
            _uiComponentsManager = await UIComponentsManager.CreateFromProjectAsync(_projectPath);

            _logger.LogInformation("Project path updated successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating project path to: {ProjectPath}", newProjectPath);
            return false;
        }
    }
}
