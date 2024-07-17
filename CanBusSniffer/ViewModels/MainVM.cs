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
            BluetoothService.CanFrameBatchParsed += OnCanFrameParsed;
        }

        private void OnCanFrameParsed(object? sender, List<CanFrame> frames)
        {


            // Update the UI thread in batches
            MainThread.BeginInvokeOnMainThread(() =>
            {
                foreach (var frame in frames)
                {
                    CanFrames.Add(frame);
                }
            });

        }



        [RelayCommand]
        private async Task OpenBluetoothMenueAsync() => await Shell.Current.GoToAsync(nameof(BluetoothPage));
    }
}