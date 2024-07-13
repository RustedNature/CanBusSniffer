using CanBusSniffer.Models;
using CanBusSniffer.Service;
using CanBusSniffer.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CanBusSniffer.ViewModels
{
    public partial class MainVM : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<CanFrame> canFrames = new();

        public BluetoothService BluetoothService { get; set; }

        public MainVM(BluetoothService bluetoothService)
        {
            BluetoothService = bluetoothService;
            BluetoothService.CanFrameParsed += OnCanFrameParsed;
        }

        private void OnCanFrameParsed(object? sender, CanFrame e)
        {
            
                CanFrames.Add(e);
                Debug.WriteLine("CAN Frame parsed and added to collection");
           
        }

        [RelayCommand]
        private async Task OpenBluetoothMenueAsync() => await Shell.Current.GoToAsync(nameof(BluetoothPage));
    }
}