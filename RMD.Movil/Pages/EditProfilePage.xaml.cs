using RMD.Movil.Core.Service.Interfaces;
using RMD.Movil.PageModels;

namespace RMD.Movil.Pages;

public partial class EditProfilePage : ContentPage
{
    public EditProfilePage()
    {
        InitializeComponent();

        var usuarios = App.Services.GetService<IUsuariosControllerService>();
        if (usuarios is null)
            throw new Exception("No se pudo resolver IUsuariosControllerService");

        BindingContext = new EditProfilePageModel(usuarios);
    }
}