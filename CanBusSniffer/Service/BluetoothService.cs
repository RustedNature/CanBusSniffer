using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace CanBusSniffer.Service
{
    public class BluetoothService
    {
        private readonly IBluetoothLE _bluetoothLE;
        private readonly IAdapter _adapter;

        public event EventHandler<IDevice> DeviceDiscovered;

        public event EventHandler<IDevice> DeviceConnected;

        public BluetoothService()
        {
            _bluetoothLE = CrossBluetoothLE.Current;
            _adapter = _bluetoothLE.Adapter;
            _adapter.DeviceDiscovered += OnDeviceDiscovered;
            _adapter.DeviceConnected += OnDeviceConnected;
        }

        private void OnDeviceConnected(object? sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            DeviceConnected?.Invoke(sender, e.Device);
        }

        private void OnDeviceDiscovered(object? sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            DeviceDiscovered?.Invoke(sender, e.Device);
        }

        public bool IsBluetoothLEOn()
        {
            return _bluetoothLE.IsOn;
        }

        public async Task<bool> RequestAndroidPermissions()
        {
            return await RequestLocationWhenInUsePermission() && await RequestBluetoothPermission();
        }

        private async Task<bool> RequestBluetoothPermission()
        {
            var status = await CheckStatusAsync<Bluetooth>();
            if (status != PermissionStatus.Granted)
            {
                status = await RequestAsync<Bluetooth>();
            }
            return status == PermissionStatus.Granted;
        }

        private async Task<bool> RequestLocationWhenInUsePermission()
        {
            var status = await CheckStatusAsync<LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await RequestAsync<LocationWhenInUse>();
            }

            return status == PermissionStatus.Granted;
        }

        public async Task ScanAsync()
        {
            await _adapter.StartScanningForDevicesAsync();
        }

        public async Task ConnectToDeviceAsync(IDevice device)
        {
            await _adapter.ConnectToDeviceAsync(device);
        }
    }
}