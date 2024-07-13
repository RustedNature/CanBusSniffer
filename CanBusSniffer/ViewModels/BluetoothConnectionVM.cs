using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CanBusSniffer.ViewModels
{
    public partial class BluetoothConnectionVM : ObservableObject
    {
        private readonly IBluetoothLE bluetoothLE;
        private readonly IAdapter adapter;

        [ObservableProperty]
        private ObservableCollection<IDevice> discoveredBTDevices = new();

        [ObservableProperty]
        private string connectionStatus = string.Empty;

        [ObservableProperty]
        private bool isScanning = false;

        [ObservableProperty]
        private bool isConnecting = false;

        public BluetoothConnectionVM()
        {
            bluetoothLE = CrossBluetoothLE.Current;
            adapter = bluetoothLE.Adapter;
            adapter.DeviceDiscovered += Adapter_DeviceDiscovered;
            adapter.DeviceConnected += Adapter_DeviceConnected;
        }

        private void Adapter_DeviceConnected(object? sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            Debug.WriteLine($"{e.Device.Name} connected");
        }

        private void Adapter_DeviceDiscovered(object? sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            bool listDevice = !DiscoveredBTDevices.Any(d => d.Id == e.Device.Id || e.Device.Name is null || e.Device.Name.Length <= 0);
            Debug.WriteLine($"List Device: {e.Device.Name} = {listDevice}");
            if (listDevice)
            {
                DiscoveredBTDevices.Add(e.Device);
            }
        }

        private static async Task<bool> RequestAndroidPermissions()
        {
            return await RequestLocationWhenInUsePermission() && await RequestBluetoothPermission();
        }

        private static async Task<bool> RequestBluetoothPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Bluetooth>();
            }
            return status == PermissionStatus.Granted;
        }

        private static async Task<bool> RequestLocationWhenInUsePermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            return status == PermissionStatus.Granted;
        }

        [RelayCommand]
        public async Task ScanAsync()
        {
            if (!bluetoothLE.IsOn)
            {
                await Shell.Current.DisplayAlert("Enable Bluetooth", "", "I will activate bluetooth");
                return;
            }
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                await RequestAndroidPermissions();
            }

            if (IsScanning)
            {
                return;
            }

            IsScanning = true;

            DiscoveredBTDevices.Clear();

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
                Debug.WriteLine($"Found {DiscoveredBTDevices.Count} DiscoveredBTDevices");
            }
        }

        [RelayCommand]
        public async Task ConnectToDevice(IDevice d)
        {
            string wantTo = await Shell.Current.DisplayPromptAsync("Connect", $"Do you want to connect to {d.Name}?");
            if (IsConnecting && wantTo != "OK")
            {
                return;
            }
            try
            {
                IsConnecting = true;

                await adapter.ConnectToDeviceAsync(d);
                OnPropertyChanged(nameof(d.State));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                ConnectionStatus = "Connection failed";
            }
            finally
            {
                IsConnecting = !IsConnecting;
            }
        }
    }
}