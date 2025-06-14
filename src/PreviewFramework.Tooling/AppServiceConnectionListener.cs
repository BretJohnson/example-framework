using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace PreviewFramework.Tooling;

public class AppServiceConnectionListener : IDisposable
{
    public const int DefaultPort = 54242;

    private readonly TcpListener _listener;
    private readonly ConcurrentDictionary<AppServiceServerConnection, AppServiceServerConnection> _connections = [];

    public AppServiceConnectionListener()
    {
        try
        {
            _listener = new TcpListener(IPAddress.Any, DefaultPort);
            _listener.Start();
        }
        catch (SocketException)
        {
            // If the default port is in use, fall back to an arbitrary port
            _listener = new TcpListener(IPAddress.Any, 0);
            _listener.Start();
        }
    }

    public void StartListening(CancellationToken cancellationToken = default)
    {
        // Get the port number
        int port = ((IPEndPoint)_listener.LocalEndpoint).Port;

        // Write the connection settings JSON file
        ConnectionSettingsJson.WriteConnectionSettingsJson(port);

        Task.Run(() => ListenLoopAsync(cancellationToken), cancellationToken);
    }

    internal void AddConnection(AppServiceServerConnection connection)
    {
        if (!_connections.TryAdd(connection, connection))
        {
            throw new InvalidOperationException("Connection unexpected already added to listeners");
        }
    }

    internal void RemoveConnection(AppServiceServerConnection connection)
    {
        _connections.TryRemove(connection, out _);
    }

    private async Task ListenLoopAsync(CancellationToken cancellationToken)
    {
        while (_listener is not null && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Throws if cancellationToken is canceled before or during the wait
                Task<TcpClient> acceptTask = _listener.AcceptTcpClientAsync(cancellationToken).AsTask();
                Task completedTask = await Task.WhenAny(acceptTask, Task.Delay(Timeout.Infinite, cancellationToken));
                if (completedTask == acceptTask)
                {
                    TcpClient tcpClient = acceptTask.Result;

                    AppServiceServerConnection appServiceConnection = new AppServiceServerConnection(this, tcpClient);

                    // Handle each client connection asynchronously (fire and forget)
                    _ = appServiceConnection.HandleConnectionAsync();
                }
                else
                {
                    // Cancellation requested
                    break;
                }
            }
            catch (ObjectDisposedException)
            {
                // Listener was stopped, exit loop
                break;
            }
            catch (OperationCanceledException)
            {
                // Cancellation requested, exit loop
                break;
            }
            catch (Exception ex)
            {
                // Log or handle error as needed
            }
        }
    }

    public int Port =>
        _listener?.LocalEndpoint is IPEndPoint ipEndpoint ? ipEndpoint.Port : -1;

    public void Dispose()
    {
        _listener.Stop();

        GC.SuppressFinalize(this);
    }
}
