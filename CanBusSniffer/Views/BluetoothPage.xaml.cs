using CanBusSniffer.ViewModels;

namespace CanBusSniffer.Views;

public partial class BluetoothPage : ContentPage
{
    public BluetoothPage(BluetoothConnectionVM vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}