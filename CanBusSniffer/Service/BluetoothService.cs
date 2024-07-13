using CanBusSniffer.Models;
using Newtonsoft.Json;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System.Diagnostics;
using System.Text;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace CanBusSniffer.Service
{
    public class BluetoothService
    {
        private readonly IBluetoothLE _bluetoothLE;
        private readonly IAdapter _adapter;
        private IDevice? _connectedDevice;
        private ICharacteristic _characteristic;

        public IBluetoothLE BluetoothLE => _bluetoothLE;

        public IAdapter Adapter => _adapter;

        public IDevice? ConnectedDevice { get => _connectedDevice; set => _connectedDevice = value; }
        public ICharacteristic Characteristic { get => _characteristic; set => _characteristic = value; }

        public event EventHandler<IDevice> DeviceDiscovered;

        public event EventHandler<IDevice> DeviceConnected;

        public event EventHandler<string> DataReceived;

        public event EventHandler<CanFrame> CanFrameParsed;

        public BluetoothService()
        {
            _bluetoothLE = CrossBluetoothLE.Current;
            _adapter = BluetoothLE.Adapter;
            _adapter.DeviceDiscovered += OnDeviceDiscovered;
            _adapter.DeviceConnected += OnDeviceConnected;
        }

        public async Task ScanAsync() => await Adapter.StartScanningForDevicesAsync();

        public async Task StopScanAsync() => await Adapter.StopScanningForDevicesAsync();

        public async Task StopReceivingData() => await Characteristic.StopUpdatesAsync();

        public async Task CloseConnection() => await Adapter.DisconnectDeviceAsync(ConnectedDevice);

        private void OnDeviceConnected(object? sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e) => DeviceConnected?.Invoke(sender, e.Device);

        private void OnDeviceDiscovered(object? sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e) => DeviceDiscovered?.Invoke(sender, e.Device);

        public bool IsBluetoothLEOn() => BluetoothLE.IsOn;

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
                int negotiatedMtu = await ConnectedDevice!.RequestMtuAsync(mtuSize);
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
            foreach (var sevice in services)
            {
                var characteristics = await sevice.GetCharacteristicsAsync();
                foreach (var c in characteristics)
                {
                    if (c.CanRead && c.CanUpdate)
                    {
                        Characteristic = c;
                        break;
                    }
                }
                if (Characteristic != null)
                {
                    break;
                }
            }
            await StartReceivingData();
        }

        public async Task StartReceivingData()
        {
            if (Characteristic == null)
            {
                throw new InvalidOperationException("No readable characteristic found");
            }

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
                CanFrame frame = JsonConvert.DeserializeObject<CanFrame>(frameData)!;
                Debug.WriteLine($"Successfully parsed: {frame}");

                CanFrameParsed.Invoke(this, frame);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error parsing CAN frame: {e.Message}");
                Debug.WriteLine($"Stack trace: {e.StackTrace}");
            }
        }
    }
}