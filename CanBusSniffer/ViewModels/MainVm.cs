using System.Collections.ObjectModel;
using System.Diagnostics;
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

    private BluetoothService BluetoothService { get; }

    private void OnCanFrameParsed(object? sender, List<CanFrame> frames)
    {
        foreach (var frame in frames)
        {
            if (!CanFrames.Contains(frame))
            {
                CanFrames.Add(frame);
                Debug.WriteLine($"{frame.FrameId} is not in, added frame");
            }
            else
            {
                CanFrames[CanFrames.IndexOf(frame)] = frame;
                Debug.WriteLine($"{frame.FrameId} is already in, updated frame");
            }
        }

        Debug.WriteLine($"List holds {CanFrames.Count} items");
    }


    [RelayCommand]
    private async Task OpenBluetoothMenuAsync()
    {
        await Shell.Current.GoToAsync(nameof(BluetoothPage));
    }
}