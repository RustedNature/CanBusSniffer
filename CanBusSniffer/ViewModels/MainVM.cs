using CanBusSniffer.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CanBusSniffer.ViewModels
{
    public partial class MainVM : ObservableObject
    {
        [ObservableProperty]
        private int num = 0;

        public MainVM()
        {
        }

        [RelayCommand]
        private async Task OpenBluetoothMenueAsync()
        {
            if (Shell.Current is not null)
            {
                await Shell.Current.GoToAsync(nameof(BluetoothPage));
            }
            else
            {
                Debug.WriteLine("No instance of shell");
            }
        }
    }
}