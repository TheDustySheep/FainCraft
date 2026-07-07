using FainCraft.Common.Networking.Messages;
using System.Buffers.Binary;
using System.Text;

namespace FainCraft.Common.Networking.Packets;
public class Packet
{
    public int Position { get; private set; } = 1;
    public int Remaining => _data.Length - Position;
    public int Length => _data.Length;
    public PacketType Type { get; }

    private readonly byte[] _data;
    public ReadOnlySpan<byte> Data => _data;

    // Constructors
    public Packet(byte[] data)
    {
        if (data.Length == 0)
            throw new ArgumentException("Packet cannot be empty.", nameof(data));

        _data = data;
        Type = (PacketType)_data[0];
    }

    public Packet(PacketType type, byte[] payload)
    {
        Type = type;

        _data = new byte[payload.Length + 1];
        _data[0] = (byte)type;
        Array.Copy(payload, 0, _data, 1, payload.Length);
    }

    public Packet(PacketType type)
    {
        Type = type;
        _data = new byte[] { (byte)type };
    }

    public INetworkMessage ReadMessage()
    {
        return NetworkMessageRegistry.ReadMessage(this);
    }

    #region Data Reading Methods
    // Byte array
    public ReadOnlySpan<byte> ReadBytes(int count)
    {
        if (Position + count > _data.Length)
            throw new InvalidOperationException($"Attempted to read {count} bytes past packet end.");

        var result = _data.AsSpan(Position, count);
        Position += count;
        return result;
    }
    public byte[] ReadByteArray(int count) => ReadBytes(count).ToArray();

    // Byte
    public byte ReadByte()
    {
        if (Position >= _data.Length)
            throw new InvalidOperationException("Attempted to read past packet end.");

        return _data[Position++];
    }
    public sbyte ReadSByte() => (sbyte)ReadByte();

    // Bool
    public bool ReadBool() => ReadByte() != 0;

    // Char
    public char ReadChar() => (char)ReadUShort();

    // Short
    public ushort ReadUShort() => BinaryPrimitives.ReadUInt16BigEndian(ReadBytes(2));
    public short ReadShort() => BinaryPrimitives.ReadInt16BigEndian(ReadBytes(2));

    // Int
    public uint ReadUInt() => BinaryPrimitives.ReadUInt32BigEndian(ReadBytes(4));
    public int ReadInt() => BinaryPrimitives.ReadInt32BigEndian(ReadBytes(4));

    // Long
    public ulong ReadULong() => BinaryPrimitives.ReadUInt64BigEndian(ReadBytes(8));
    public long ReadLong() => BinaryPrimitives.ReadInt64BigEndian(ReadBytes(8));


    // Float
    public float ReadFloat() => BinaryPrimitives.ReadSingleBigEndian(ReadBytes(4));

    // Double
    public double ReadDouble() => BinaryPrimitives.ReadDoubleBigEndian(ReadBytes(8));

    // GUID
    public Guid ReadGuid() => new(ReadBytes(16));

    // String
    public string ReadString()
    {
        ushort length = ReadUShort();
        string result = Encoding.UTF8.GetString(
            _data,
            Position,
            length);

        Position += length;
        return result;
    }
    #endregion
}
