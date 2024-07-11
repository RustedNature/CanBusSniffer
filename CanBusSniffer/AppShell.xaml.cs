using CanBusSniffer.Views;

namespace CanBusSniffer
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(BluetoothCV), typeof(BluetoothCV));
        }
    }
}