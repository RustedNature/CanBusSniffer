using System.Collections.ObjectModel;
using CanBusSniffer.Models;
using CanBusSniffer.Service;
using CanBusSniffer.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CanBusSniffer.ViewModels;

public partial class MainVm : ObservableObject
{
    [ObservableProperty] private ObservableCollection<CanFrame> _canFrames = new();

    public MainVm(BluetoothService bluetoothService)
    {
        BluetoothService = bluetoothService;
        BluetoothService.CanFrameBatchParsed += OnCanFrameParsed;
    }

    private BluetoothService BluetoothService { get; set; }

    private void OnCanFrameParsed(object? sender, List<CanFrame> frames)
    {
        // Update the UI thread in batches
        MainThread.BeginInvokeOnMainThread(() =>
        {
            foreach (var frame in frames) CanFrames.Add(frame);
        });
    }


    [RelayCommand]
    private async Task OpenBluetoothMenuAsync()
    {
        await Shell.Current.GoToAsync(nameof(BluetoothPage));
    }
}