using System.Diagnostics;
using CanBusSniffer.Models;
using Newtonsoft.Json;

namespace CanBusSniffer.Service;

public static class CanFrameParser
{
    public static Task ParseCanFrame(string frameData, List<CanFrame> canFrameBatch)
    {
        try
        {
            var frame = JsonConvert.DeserializeObject<CanFrame>(frameData)!;
            Debug.WriteLine($"Successfully parsed: {frame}");

            canFrameBatch.Add(frame);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error parsing CAN frame: {e.Message}");
        }

        return Task.CompletedTask;
    }
}