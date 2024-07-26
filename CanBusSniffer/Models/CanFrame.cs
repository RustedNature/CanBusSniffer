using Newtonsoft.Json;

namespace CanBusSniffer.Models;

public class CanFrame : IEquatable<CanFrame>
{
    private const uint MaxDataLength = 8;

    public CanFrame(uint frameId, int dataLengthContent, List<byte> data)
    {
        FrameId = frameId;
        DataLengthContent = dataLengthContent;

        for (var i = 0; i < MaxDataLength; i++)
        {
            if (i < DataLengthContent)
            {
                Data[i] = data[i];
            }
            else
            {
                Data[i] = 0;
            }
        }
    }

    [JsonProperty("id")] public uint FrameId { get; }

    [JsonProperty("dlc")] public int DataLengthContent { get; set; }

    [JsonProperty("data")] public byte[] Data { get; set; } = new byte[8];


    public override string ToString()
    {
        var r = string.Empty;
        r += $"ID: 0x{FrameId:X} Data: ";
        for (var i = 0; i < DataLengthContent; i++) r += $"{Data[i]:X} ";

        return r;
    }


    public bool Equals(CanFrame? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return FrameId == other.FrameId;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CanFrame)obj);
    }

    public override int GetHashCode()
    {
        return (int)FrameId;
    }
}