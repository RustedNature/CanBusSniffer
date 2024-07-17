using CanBusSniffer.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.BLE.Abstractions.Contracts;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CanBusSniffer.ViewModels
{
    public partial class BluetoothConnectionVM : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<IDevice> discoveredBTDevices = new();

        [ObservableProperty]
        private string connectionStatus = string.Empty;

        [ObservableProperty]
        private bool isScanning = false;

        [ObservableProperty]
        private bool isConnecting = false;

        public BluetoothService BluetoothService { get; }

        public BluetoothConnectionVM(BluetoothService bluetoothService)
        {
            BluetoothService = bluetoothService;
            BluetoothService.DeviceConnected += OnDeviceConnected;
            BluetoothService.DeviceDiscovered += OnDeviceDiscovered;
        }

        private void OnDeviceConnected(object? sender, IDevice device)
        {
            Debug.WriteLine($"{device.Name} connected");
        }

        private void OnDeviceDiscovered(object? sender, IDevice device)
        {
            bool listDevice = !DiscoveredBTDevices.Any(d => d.Id == device.Id || device.Name is null || device.Name.Length <= 0);
            Debug.WriteLine($"List Device: {device.Name} = {listDevice}");
            if (listDevice)
            {
                DiscoveredBTDevices.Add(device);
            }
        }

        [RelayCommand]
        public async Task ScanAsync()
        {
            await CheckIsBluetoothOn();
            await CheckRequierdAndroidPermissions();

            if (IsScanning)
            {
                return;
            }

            IsScanning = true;

            DiscoveredBTDevices.Clear();

            try
            {
                await BluetoothService.ScanAsync();
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

        private async Task CheckRequierdAndroidPermissions()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                await BluetoothService.RequestAndroidPermissions();
            }
        }

        private async Task CheckIsBluetoothOn()
        {
            if (!BluetoothService.IsBluetoothLEOn())
            {
                await Shell.Current.DisplayAlert("Enable Bluetooth", "", "I will activate bluetooth");
                return;
            }
        }

        [RelayCommand]
        public async Task ConnectToDevice(IDevice d)
        {
            await StopDiscoverDevices();

            if (IsConnecting)
            {
                return;
            }
            try
            {
                IsConnecting = true;

                await BluetoothService.ConnectToDeviceAsync(d);
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

        private async Task StopDiscoverDevices()
        {
            if (IsScanning)
            {
                await BluetoothService.StopScanAsync();
            }
        }
    }
}