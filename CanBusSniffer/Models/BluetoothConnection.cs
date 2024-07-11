using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InTheHand.Bluetooth;

namespace CanBusSniffer.Models
{
    internal static class BluetoothConnection
    {
        public static async Task<IReadOnlyCollection<BluetoothDevice>> ScanForBluetoothDevices()
        {
            var deviceList = await Bluetooth.ScanForDevicesAsync();

            return deviceList;
        }

        public static async void ConnectWithBluetoothDevice(BluetoothDevice device)
        {
            if (device is not null)
            {
                await device.Gatt.ConnectAsync();

                if (device.Gatt.IsConnected)
                {
                    Debug.WriteLine($"Device {device.Name} connected");
                }
                else
                {
                    Debug.WriteLine($"Device {device.Name} not connected");
                }
            }
        }
    }
}