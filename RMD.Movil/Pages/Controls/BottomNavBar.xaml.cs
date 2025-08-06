using RMD.Movil.PageModels.Controls;

namespace RMD.Movil.Pages.Controls;
public partial class BottomNavBar : ContentView
{
    public BottomNavBar()
    {
        InitializeComponent();
        BindingContext = new BottomNavBarModel();
    }
}