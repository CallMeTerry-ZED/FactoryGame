using LiteNetLib;
using LiteNetLib.Utils;
using FactoryGame.Core.Log;
using FactoryGame.Core.Net;
using FactoryGame.Core.Net.Messages;
using FactoryGame.Core.Player;

namespace FactoryGame.Server;

public class ServerNet : INetEventListener, IDisposable
{
    private readonly NetManager _netManager;
    private readonly Dictionary<int, NetPeer> _peers = new();
    private readonly Dictionary<int, Player> _players = new();
    
    public int PlayerCount => _players.Count;

    public ServerNet()
    {
        _netManager = new NetManager(this)
        {
            AutoRecycle = true,
        };
    }

    public void Start()
    {
        _netManager.Start(NetProtocol.Port);
        Logger.Info($"Server started on port {NetProtocol.Port}");
    }

    // Called each server tick to poll for incoming packets
    public void Poll() => _netManager.PollEvents();

    public void Stop()
    {
        _netManager.Stop();
        Logger.Info($"Server stopped on port {NetProtocol.Port}");
    }

    // LiteNetLib callbacks
    public void OnConnectionRequest(ConnectionRequest request)
    {
        if (_peers.Count >= NetProtocol.MaxPlayers)
        {
            Logger.Warn("Max players reached. Join request rejected.");
            request.Reject();
            return;
        }

        // Validate the app id so random connections get dropped hopefully
        request.AcceptIfKey(NetProtocol.AppId);
    }

    public void OnPeerConnected(NetPeer peer)
    {
        _peers[peer.Id] = peer;
        Logger.Info($"Peer connected: {peer.Address} (id={peer.Id}) [{_peers.Count}/{NetProtocol.MaxPlayers}]");
        // Player gets created properly once we receive their handshake with their name
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        if (_players.TryGetValue(peer.Id, out var player))
            Logger.Info($"Player '{player.Name}' disconnected. Reason: {disconnectInfo.Reason}");

        _peers.Remove(peer.Id);
        _players.Remove(peer.Id);
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
                    Logger.Info($"Handshake from '{handshake.PlayerName}' (version={handshake.Version})");

                    if (handshake.Version != NetProtocol.Version)
                    {
                        Logger.Warn($"Version mismatch — client={handshake.Version} server={NetProtocol.Version}");
                        Send(peer, new DisconnectMessage($"Version mismatch. Server is {NetProtocol.Version}."));
                        peer.Disconnect();
                        return;
                    }

                    // Create the player now that we have their name
                    var newPlayer = new Player(peer.Id, handshake.PlayerName);
                    _players[peer.Id] = newPlayer;
                    Logger.Info($"Player '{newPlayer.Name}' registered (id={newPlayer.Id}).");

                    Send(peer, new HandshakeMessage("Server", NetProtocol.Version));
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

    private void Send(NetPeer peer, NetMessage message)
    {
        var data = message.Serialize();
        var writer = new NetDataWriter();
        writer.Put(data);
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
    }

    // Unused LiteNetLib callbacks — required by interface
    public void OnNetworkError(System.Net.IPEndPoint endPoint, System.Net.Sockets.SocketError socketError) => Logger.Error($"Network error from {endPoint}: {socketError}");
    
    public void OnNetworkReceiveUnconnected(System.Net.IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public IEnumerable<string> GetPlayerNames() => _players.Values.Select(p => p.Name);
    
    public void Dispose() => Stop();
}