using CanBusSniffer.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CanBusSniffer.ViewModels
{
    public partial class MainPageVM : ObservableObject
    {
        [ObservableProperty]
        private int num = 0;

        public MainPageVM()
        {
        }

        [RelayCommand]
        private async Task OpenBluetoothMenueAsync()
        {
            Num++;
        }
    }
}