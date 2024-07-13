using Newtonsoft.Json;

namespace CanBusSniffer.Models
{
    public class CanFrame
    {
        [JsonProperty("id")]
        public long FrameID { get; set; }

        [JsonProperty("dlc")]
        public int DataLengthContent { get; set; }

        [JsonProperty("data")]
        public List<byte> Data { get; set; } = new();

        public CanFrame(int frameID, int dataLengthContent, byte[] data)
        {
            FrameID = frameID;
            DataLengthContent = dataLengthContent;
            for (int i = 0; i < DataLengthContent; i++)
            {
                Data[i] = data[i];
            }
        }

        public CanFrame()
        {
        }

        public override string ToString()
        {
            string r = string.Empty;
            r += $"ID: 0x{FrameID:X} Data: ";
            for (int i = 0; i < DataLengthContent; i++)
            {
                r += $"{Data[i]:X} ";
            }

            return r;
        }
    }
}