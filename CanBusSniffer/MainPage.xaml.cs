using CanBusSniffer.ViewModels;

namespace CanBusSniffer
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageVM();
        }
    }
}