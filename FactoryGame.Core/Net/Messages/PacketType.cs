namespace FactoryGame.Core.Net.Messages;

public enum PacketType : byte
{
    Handshake = 0,
    Disconnect = 1,
}