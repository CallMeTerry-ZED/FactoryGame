using LiteNetLib;
using LiteNetLib.Utils;
using FactoryGame.Core.Log;
using FactoryGame.Core.Net;
using FactoryGame.Core.Net.Messages;
using FactoryGame.Core.Player;

namespace FactoryGame.Client;

public class ClientNet : INetEventListener, IDisposable
{
    private readonly NetManager _netManager;
    private NetPeer? _server;
    private string _pendingPlayerName = "Player";
    
    public bool IsConnected => _server != null;
    public Player? LocalPlayer { get; private set; }

    public ClientNet()
    {
        _netManager = new NetManager(this)
        {
            AutoRecycle = true,
        };
    }

    public void Connect(string host, string playerName)
    {
        _netManager.Start();

        var writer = new NetDataWriter();
        writer.Put(NetProtocol.AppId);
        _netManager.Connect(host, NetProtocol.Port, writer);

        Logger.Info($"ClientNet connecting to {host}:{NetProtocol.Port} as '{playerName}'...");

        // Send handshake once connected — stored for use in OnPeerConnected
        _pendingPlayerName = playerName;
    }

    public void Poll() => _netManager.PollEvents();

    public void Disconnect()
    {
        _server?.Disconnect();
        _netManager.Stop();
        Logger.Info("ClientNet disconnected.");
    }

    // LiteNetLib callbacks
    public void OnPeerConnected(NetPeer peer)
    {
        _server = peer;
        Logger.Info($"Connected to server at {peer.Address}.");

        // Send handshake immediately
        Send(new HandshakeMessage(_pendingPlayerName, NetProtocol.Version));
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        _server = null;
        Logger.Warn($"Disconnected from server. Reason: {disconnectInfo.Reason}");
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
    {
        try
        {
            var data = reader.GetRemainingBytes();
            var message = NetMessage.Deserialize(data);

            switch (message)
            {
                case HandshakeMessage handshake:
                    // Create our local player now that server confirmed us
                    LocalPlayer = new Player(0, _pendingPlayerName);
                    Logger.Info($"Handshake confirmed by server (version={handshake.Version}). Ready!");
                    Logger.Info($"Local player created: {LocalPlayer}");
                    break;
                
                case DisconnectMessage disconnect:
                    Logger.Warn($"Server rejected connection: {disconnect.Reason}");
                    break;
                
                default:
                    Logger.Warn($"Unhandled message type: {message.Type}");
                    break;
            }
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to deserialize packet.", ex);
        }
    }

    public void OnConnectionRequest(ConnectionRequest request) => request.Reject();

    private void Send(NetMessage message)
    {
        if (_server == null) return;
        var data = message.Serialize();
        var writer = new NetDataWriter();
        writer.Put(data);
        _server.Send(writer, DeliveryMethod.ReliableOrdered);
    }

    public void OnNetworkError(System.Net.IPEndPoint endPoint, System.Net.Sockets.SocketError socketError) => Logger.Error($"Network error from {endPoint}: {socketError}");

    public void OnNetworkReceiveUnconnected(System.Net.IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public void Dispose() => Disconnect();
}