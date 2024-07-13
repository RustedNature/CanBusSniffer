using CanBusSniffer.Service;
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
        private BluetoothService _bluetoothService;

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
            _bluetoothService = new BluetoothService();
            _bluetoothService.DeviceConnected += OnDeviceConnected;
            _bluetoothService.DeviceDiscovered += OnDeviceDiscovered;
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
            if (!_bluetoothService.IsBluetoothLEOn())
            {
                await Shell.Current.DisplayAlert("Enable Bluetooth", "", "I will activate bluetooth");
                return;
            }
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                await _bluetoothService.RequestAndroidPermissions();
            }

            if (IsScanning)
            {
                return;
            }

            IsScanning = true;

            DiscoveredBTDevices.Clear();

            try
            {
                await _bluetoothService.ScanAsync();
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

                await _bluetoothService.ConnectToDeviceAsync(d);
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