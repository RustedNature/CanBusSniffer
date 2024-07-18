using System.Collections.ObjectModel;
using System.Diagnostics;
using CanBusSniffer.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.BLE.Abstractions.Contracts;

namespace CanBusSniffer.ViewModels;

public partial class BluetoothConnectionVm : ObservableObject
{
    [ObservableProperty] private string _connectionStatus = string.Empty;

    [ObservableProperty] private ObservableCollection<IDevice> _discoveredBtDevices = new();

    [ObservableProperty] private bool _isConnecting;

    [ObservableProperty] private bool _isScanning;

    public BluetoothConnectionVm(BluetoothService bluetoothService)
    {
        BluetoothService = bluetoothService;
        BluetoothService.DeviceConnected += OnDeviceConnected;
        BluetoothService.DeviceDiscovered += OnDeviceDiscovered;
    }

    private BluetoothService BluetoothService { get; }

    private static void OnDeviceConnected(object? sender, IDevice device)
    {
        Debug.WriteLine($"{device.Name} connected");
    }

    private void OnDeviceDiscovered(object? sender, IDevice device)
    {
        var listDevice =
            !DiscoveredBtDevices.Any(d => d.Id == device.Id || device.Name is null || device.Name.Length <= 0);
        Debug.WriteLine($"List Device: {device.Name} = {listDevice}");
        if (listDevice) DiscoveredBtDevices.Add(device);
    }

    [RelayCommand]
    private async Task ScanAsync()
    {
        await CheckIsBluetoothOn();
        await CheckRequiredAndroidPermissions();

        if (IsScanning) return;

        IsScanning = true;

        DiscoveredBtDevices.Clear();

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
            Debug.WriteLine($"Found {DiscoveredBtDevices.Count} DiscoveredBTDevices");
        }
    }

    private static async Task CheckRequiredAndroidPermissions()
    {
        if (DeviceInfo.Platform == DevicePlatform.Android) await BluetoothService.RequestAndroidPermissions();
    }

    private async Task CheckIsBluetoothOn()
    {
        if (!BluetoothService.IsBluetoothLEOn())
        {
            await Shell.Current.DisplayAlert("Enable Bluetooth", "", "I will activate bluetooth");
        }
    }

    [RelayCommand]
    private async Task ConnectToDevice(IDevice d)
    {
        await StopDiscoverDevices();

        if (IsConnecting) return;
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
        if (IsScanning) await BluetoothService.StopScanAsync();
    }
}