using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using StreamJsonRpc;

namespace PreviewFramework.App;

public class AppServiceClientConnection(string connectionString)
{
    private readonly string _connectionString = connectionString;
    private TcpClient? _tcpClient;
    private JsonRpc? _rpc;
    private IExampleAppControllerService? _appControllerService;

    public async Task StartConnectionAsync(ExampleAppService appService)
    {
        // Parse _connectionString in the format "host:port"
        string[] parts = _connectionString.Split(':');
        if (parts.Length != 2)
            throw new FormatException($"Connection string '{_connectionString}' isn't in the format 'host:port'.");

        string host = parts[0];
        if (!int.TryParse(parts[1], out int port))
            throw new FormatException($"Connection string '{_connectionString}' port must be a valid integer.");

        _tcpClient = new TcpClient();
        await _tcpClient.ConnectAsync(host, port).ConfigureAwait(false);
        NetworkStream networkStream = _tcpClient.GetStream();

        _rpc = JsonRpc.Attach(networkStream, appService);

        _appControllerService = _rpc.Attach<IExampleAppControllerService>();

        ExampleApplication exampleApplication = ExampleApplication.GetInstance();
        await _appControllerService.RegisterAppAsync(exampleApplication.ProjectPath, exampleApplication.ApplicationName,
            exampleApplication.PlatformName).ConfigureAwait(false);
    }
}
