using CanBusSniffer.ViewModels;
using CanBusSniffer.Views;

namespace CanBusSniffer;

public partial class MainPage : ContentPage
{
    public MainPage(MainVm mainVm, MainCv mainCv)
    {
        InitializeComponent();
        BindingContext = mainVm;
        Content = mainCv;
    }
}