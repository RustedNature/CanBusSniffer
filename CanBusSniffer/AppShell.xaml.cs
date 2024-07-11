using CanBusSniffer.ViewModels;
using CanBusSniffer.Views;

namespace CanBusSniffer
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(BluetoothPage), typeof(BluetoothPage));
        }
    }
}