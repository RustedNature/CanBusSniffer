using CanBusSniffer.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InTheHand.Bluetooth;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanBusSniffer.ViewModels
{
    public partial class BluetoothConnectionVM : ObservableObject
    {
        [ObservableProperty]
        private IReadOnlyCollection<BluetoothDevice> devices;

        private async Task<bool> RequestBluetoothPermissions()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                if (DeviceInfo.Platform == DevicePlatform.Android && DeviceInfo.Version.Major >= 12)
                {
                    var bluetoothScan = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();
                    if (bluetoothScan != PermissionStatus.Granted)
                    {
                        bluetoothScan = await Permissions.RequestAsync<Permissions.Bluetooth>();
                    }

                    var bluetoothConnect = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();
                    if (bluetoothConnect != PermissionStatus.Granted)
                    {
                        bluetoothConnect = await Permissions.RequestAsync<Permissions.Bluetooth>();
                    }

                    return status == PermissionStatus.Granted &&
                           bluetoothScan == PermissionStatus.Granted &&
                           bluetoothConnect == PermissionStatus.Granted;
                }

                return status == PermissionStatus.Granted;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error requesting permissions: {ex}");
                return false;
            }
        }

        [RelayCommand]
        public async Task NewScanAsync()
        {
            await RequestBluetoothPermissions();
            try
            {
                Debug.WriteLine("NewScanAsync started");
                Devices = await BluetoothConnection.ScanForBluetoothDevices();

                foreach (var item in Devices)
                {
                    Debug.WriteLine($"Found {item.Id} device");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in NewScanAsync: {ex}");
            }
        }

        [RelayCommand]
        public async void SelectDevice()
        {
        }
    }
}