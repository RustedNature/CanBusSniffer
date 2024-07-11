using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanBusSniffer.Models
{
    internal class CanFrame
    {
        public int FrameID { get; set; }
        public int DataLengthContent { get; set; }
        public byte[] Data { get; set; } = new byte[8];

        public CanFrame(int frameID, int dataLengthContent, byte[] data)
        {
            FrameID = frameID;
            DataLengthContent = dataLengthContent;
            for (int i = 0; i < DataLengthContent; i++)
            {
                Data[i] = data[i];
            }
        }

        public override string ToString()
        {
            string r = string.Empty;
            r += $"{FrameID:X}: ";
            for (int i = 0; i < DataLengthContent; i++)
            {
                r += $"{Data[i]}";
            }

            r += "\n";

            return r;
        }
    }
}