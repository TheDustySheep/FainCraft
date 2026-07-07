namespace FainCraft.Common.Networking.Packets;

[AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
internal class PacketTypeAttribute : Attribute
{
    public PacketType PacketType { get; }

    public PacketTypeAttribute(PacketType packetType)
    {
        PacketType = packetType;
    }
}
