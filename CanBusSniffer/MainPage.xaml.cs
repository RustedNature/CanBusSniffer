using CanBusSniffer.ViewModels;
using CanBusSniffer.Views;

namespace CanBusSniffer
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainVM mainVM, MainCV mainCV)
        {
            InitializeComponent();
            BindingContext = mainVM;
            Content = mainCV;
        }
    }
}