using CanBusSniffer.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanBusSniffer.ViewModels
{
    public partial class BluetoothConnectionVM : ObservableObject
    {
        private readonly IBluetoothLE bluetoothLE;
        private readonly IAdapter adapter;

        [ObservableProperty]
        private ObservableCollection<IDevice> devices = new();

        [ObservableProperty]
        private bool isScanning = false;

        public BluetoothConnectionVM()
        {
            bluetoothLE = CrossBluetoothLE.Current;
            adapter = bluetoothLE.Adapter;
            adapter.DeviceDiscovered += Adapter_DeviceDiscovered;
        }

        private async void Adapter_DeviceDiscovered(object? sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            if (!Devices.Any(d => d.Id == e.Device.Id))
            {
                Devices.Add(e.Device);
            }
        }

        private static async Task<bool> RequestPermissions()
        {
            return await RequestLocationWhenInUse() && await RequestBluetooth();
        }

        private static async Task<bool> RequestBluetooth()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Bluetooth>();
            }
            return status == PermissionStatus.Granted;
        }

        private static async Task<bool> RequestLocationWhenInUse()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            return status == PermissionStatus.Granted;
        }

        [RelayCommand]
        public async Task NewScanAsync()
        {
#if __ANDROID__ || ANDROID

            if (!await RequestPermissions())
            {
                await Shell.Current.DisplayAlert("Need Permissons", "Please grant permission for Location when in use and Bluetooth at least", "Back");
                return;
            }
#endif
            if (IsScanning)
            {
                return;
            }

            IsScanning = true;

            Devices.Clear();

            try
            {
                await adapter.StartScanningForDevicesAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Unable to scan: {ex.Message}", "Ok");
            }
            finally
            {
                IsScanning = false;
                Debug.WriteLine($"Found {Devices.Count} devices");
            }
        }

        [RelayCommand]
        public async void SelectDevice()
        {
            throw new NotImplementedException();
        }
    }
}