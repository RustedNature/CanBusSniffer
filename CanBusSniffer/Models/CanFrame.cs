using Newtonsoft.Json;

namespace CanBusSniffer.Models;

public class CanFrame
{
    public CanFrame(uint frameID, int dataLengthContent, byte[] data)
    {
        FrameID = frameID;
        DataLengthContent = dataLengthContent;
        for (var i = 0; i < DataLengthContent; i++) Data[i] = data[i];
    }

    public CanFrame()
    {
    }

    [JsonProperty("id")] public uint FrameID { get; set; }

    [JsonProperty("dlc")] public int DataLengthContent { get; set; }

    [JsonProperty("data")] public List<byte> Data { get; set; } = new();

    public override string ToString()
    {
        var r = string.Empty;
        r += $"ID: 0x{FrameID:X} Data: ";
        for (var i = 0; i < DataLengthContent; i++) r += $"{Data[i]:X} ";

        return r;
    }
}