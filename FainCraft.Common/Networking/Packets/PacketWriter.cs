using System.Buffers;
using System.Buffers.Binary;
using System.Text;

namespace FainCraft.Common.Networking.Packets;
public class PacketWriter
{
    private readonly ArrayBufferWriter<byte> _buffer = new();

    public PacketWriter(PacketType type)
    {
        Write((byte)type);
    }

    public int Length => _buffer.WrittenCount;

    public ReadOnlySpan<byte> Data => _buffer.WrittenSpan;

    // Byte
    public void Write(byte value)
    {
        _buffer.GetSpan(1)[0] = value;
        _buffer.Advance(1);
    }

    public void Write(sbyte value)
    {
        Write((byte)value);
    }

    // Bool
    public void Write(bool value)
    {
        Write(value ? (byte)1 : (byte)0);
    }

    // Char
    public void Write(char value)
    {
        Write((byte)value);
    }

    // Short
    public void Write(ushort value)
    {
        BinaryPrimitives.WriteUInt16BigEndian(_buffer.GetSpan(2), value);
        _buffer.Advance(2);
    }

    public void Write(short value)
    {
        BinaryPrimitives.WriteInt16BigEndian(_buffer.GetSpan(2), value);
        _buffer.Advance(2);
    }

    // Int
    public void Write(uint value)
    {
        BinaryPrimitives.WriteUInt32BigEndian(_buffer.GetSpan(4), value);
        _buffer.Advance(4);
    }

    public void Write(int value)
    {
        BinaryPrimitives.WriteInt32BigEndian(_buffer.GetSpan(4), value);
        _buffer.Advance(4);
    }

    // Long
    public void Write(ulong value)
    {
        BinaryPrimitives.WriteUInt64BigEndian(_buffer.GetSpan(8), value);
        _buffer.Advance(8);
    }

    public void Write(long value)
    {
        BinaryPrimitives.WriteInt64BigEndian(_buffer.GetSpan(8), value);
        _buffer.Advance(8);
    }

    // Float
    public void Write(float value)
    {
        BinaryPrimitives.WriteSingleBigEndian(_buffer.GetSpan(4), value);
        _buffer.Advance(4);
    }

    // Double
    public void Write(double value)
    {
        BinaryPrimitives.WriteDoubleBigEndian(_buffer.GetSpan(8), value);
        _buffer.Advance(8);
    }

    // GUID
    public void Write(Guid value)
    {
        value.TryWriteBytes(_buffer.GetSpan(16));
        _buffer.Advance(16);
    }

    // Raw bytes
    public void Write(ReadOnlySpan<byte> bytes)
    {
        bytes.CopyTo(_buffer.GetSpan(bytes.Length));
        _buffer.Advance(bytes.Length);
    }

    // String
    public void Write(string value)
    {
        int byteCount = Encoding.UTF8.GetByteCount(value);

        if (byteCount > ushort.MaxValue)
            throw new InvalidOperationException("String too large.");

        Write((ushort)byteCount);

        Encoding.UTF8.GetBytes(value, _buffer.GetSpan(byteCount));
        _buffer.Advance(byteCount);
    }

    // Packet
    public Packet Build()
    {
        return new Packet(_buffer.WrittenSpan.ToArray());
    }

    // Array
    public byte[] ToArray()
    {
        return _buffer.WrittenSpan.ToArray();
    }
}
