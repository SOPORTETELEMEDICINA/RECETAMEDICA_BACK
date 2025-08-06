using RMD.Movil.PageModels.Controls;

namespace RMD.Movil.Pages.Controls;

public partial class UserInfoComponent : ContentView
{
    public UserInfoComponent()
    {
        InitializeComponent();
        BindingContext = new UserInfoComponentModel();
    }
}