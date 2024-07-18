using CanBusSniffer.ViewModels;

namespace CanBusSniffer.Views;

public partial class BluetoothPage : ContentPage
{
    public BluetoothPage(BluetoothConnectionVm vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}