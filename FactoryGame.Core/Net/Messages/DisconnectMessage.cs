namespace FactoryGame.Core.Net.Messages;

public class DisconnectMessage : NetMessage
{
    public override PacketType Type => PacketType.Disconnect;
    public string Reason { get; }
    
    public DisconnectMessage(string reason = "No reason provided.")
    {
        Reason = reason;
    }

    protected override void Write(BinaryWriter writer)
    {
        writer.Write(Reason);
    }

    public static DisconnectMessage Read(BinaryReader reader)
    {
        var reason = reader.ReadString();
        return new DisconnectMessage(reason);
    }
}