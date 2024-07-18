using System.Diagnostics;
using System.Text;
using System.Timers;
using CanBusSniffer.Models;
using Newtonsoft.Json;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using static Microsoft.Maui.ApplicationModel.Permissions;
using Timer = System.Timers.Timer;

namespace CanBusSniffer.Service;

public class BluetoothService
{
    private const int BatchIntervalsMs = 1000;
    private readonly object _batchLock = new();
    private readonly Timer _batchTimer;
    private readonly List<CanFrame> _canFrameBatch = new();

    public BluetoothService()
    {
        _batchTimer = new Timer(BatchIntervalsMs);
        _batchTimer.Elapsed += OnBatchTimerElapsed;
        BluetoothLe = CrossBluetoothLE.Current;
        Adapter = BluetoothLe.Adapter;
        Adapter.DeviceDiscovered += OnDeviceDiscovered;
        Adapter.DeviceConnected += OnDeviceConnected;
    }

    private IBluetoothLE BluetoothLe { get; }

    private IAdapter Adapter { get; }

    private IDevice? ConnectedDevice { get; set; }

    private ICharacteristic? Characteristic { get; set; }

    public event EventHandler<IDevice>? DeviceDiscovered;

    public event EventHandler<IDevice>? DeviceConnected;

    public event EventHandler<string>? DataReceived;

    public event EventHandler<List<CanFrame>>? CanFrameBatchParsed;

    private void OnBatchTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        List<CanFrame> canFrames;
        lock (_batchLock)
        {
            canFrames = [.._canFrameBatch];
            _canFrameBatch.Clear();
        }

        CanFrameBatchParsed?.Invoke(sender, canFrames);
    }

    public async Task ScanAsync()
    {
        await Adapter.StartScanningForDevicesAsync();
    }

    public async Task StopScanAsync()
    {
        await Adapter.StopScanningForDevicesAsync();
    }

    private async Task StopReceivingData()
    {
        await Characteristic.StopUpdatesAsync();
    }

    private async Task CloseConnection()
    {
        await Adapter.DisconnectDeviceAsync(ConnectedDevice);
    }

    private void OnDeviceConnected(object? sender, DeviceEventArgs e)
    {
        DeviceConnected?.Invoke(sender, e.Device);
    }

    private void OnDeviceDiscovered(object? sender, DeviceEventArgs e)
    {
        DeviceDiscovered?.Invoke(sender, e.Device);
    }

    public bool IsBluetoothLEOn()
    {
        return BluetoothLe.IsOn;
    }

    public static async Task<bool> RequestAndroidPermissions()
    {
        return await RequestLocationWhenInUsePermission() && await RequestBluetoothPermission();
    }

    private static async Task<bool> RequestBluetoothPermission()
    {
        var status = await CheckStatusAsync<Bluetooth>();
        if (status != PermissionStatus.Granted) status = await RequestAsync<Bluetooth>();
        return status == PermissionStatus.Granted;
    }

    private static async Task<bool> RequestLocationWhenInUsePermission()
    {
        var status = await CheckStatusAsync<LocationWhenInUse>();
        if (status != PermissionStatus.Granted) status = await RequestAsync<LocationWhenInUse>();

        return status == PermissionStatus.Granted;
    }

    public async Task ConnectToDeviceAsync(IDevice device)
    {
        await Adapter.ConnectToDeviceAsync(device);
        ConnectedDevice = device;
        await RequestMtuAsync(200);
        await DiscoverServicesAsync();
    }

    private async Task RequestMtuAsync(int mtuSize)
    {
        try
        {
            var negotiatedMtu = await ConnectedDevice!.RequestMtuAsync(mtuSize);
            Debug.WriteLine($"Negotiated MTU size: {negotiatedMtu}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to request MTU: {ex.Message}");
        }
    }

    private async Task DiscoverServicesAsync()
    {
        var services = await ConnectedDevice!.GetServicesAsync();
        foreach (var service in services)
        {
            var characteristics = await service.GetCharacteristicsAsync();
            foreach (var c in characteristics)
                if (c.CanRead && c.CanUpdate)
                {
                    Characteristic = c;
                    break;
                }

            if (Characteristic != null) break;
        }

        await StartReceivingData();
    }

    private async Task StartReceivingData()
    {
        if (!_batchTimer.Enabled) _batchTimer.Start();
        if (Characteristic == null) throw new InvalidOperationException("No readable characteristic found");

        Characteristic.ValueUpdated += (s, e) =>
        {
            var receivedData = Encoding.UTF8.GetString(e.Characteristic.Value);
            DataReceived?.Invoke(this, receivedData);
            ParseCanFrame(receivedData);
        };

        await Characteristic.StartUpdatesAsync();
    }

    private void ParseCanFrame(string frameData)
    {
        try
        {
            var frame = JsonConvert.DeserializeObject<CanFrame>(frameData)!;
            Debug.WriteLine($"Successfully parsed: {frame}");
            lock (_batchLock)
            {
                _canFrameBatch.Add(frame);
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error parsing CAN frame: {e.Message}");
            Debug.WriteLine($"Stack trace: {e.StackTrace}");
        }
    }

    private async void Cleanup()
    {
        await StopReceivingData();
        await CloseConnection();
        _batchTimer.Stop();
        _batchTimer.Elapsed -= OnBatchTimerElapsed;
    }
}