namespace FactoryGame.Core.Net.Messages;

public enum PacketType : byte
{
    Handshake = 0,
    Disconnect = 1,
    PlayerPosition = 2,
    PlayerState    = 3,
}