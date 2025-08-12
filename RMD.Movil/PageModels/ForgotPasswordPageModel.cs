using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.Login;
using System.Net.Mail;

namespace RMD.Movil.PageModels;

public partial class ForgotPasswordPageModel : ObservableObject
{
    private readonly IAuthControllerService _auth;

    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private bool isBusy;
    public bool IsNotBusy => !IsBusy;

    [ObservableProperty] private string emailError = string.Empty;
    public bool HasEmailError => !string.IsNullOrWhiteSpace(EmailError);

    public ForgotPasswordPageModel(IAuthControllerService auth)
    {
        _auth = auth;
    }

    [RelayCommand]
    private async Task Back() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task Send()
    {
        // Limpiar error previo
        EmailError = string.Empty;
        OnPropertyChanged(nameof(HasEmailError));

        // 1) Reglas de validación
        if (string.IsNullOrWhiteSpace(Email))
        {
            EmailError = "El correo es obligatorio.";
            OnPropertyChanged(nameof(HasEmailError));
            await Shell.Current.DisplayAlert("Validación", EmailError, "OK");
            return;
        }
        if (!IsValidEmail(Email))
        {
            EmailError = "Formato de correo inválido.";
            OnPropertyChanged(nameof(HasEmailError));
            await Shell.Current.DisplayAlert("Validación", EmailError, "OK");
            return;
        }

        // 2) Llamada al endpoint
        try
        {
            IsBusy = true;
            OnPropertyChanged(nameof(IsNotBusy));

            var req = new ForgotPasswordRequest { Email = Email };
            var resp = await _auth.ForgotPasswordAsync(req);

            var toast = resp.Toast?.ToLower() ?? "error";
            var mensaje = (resp.Descripcion?.Any() == true)
                ? string.Join("\n", resp.Descripcion)
                : (string.IsNullOrWhiteSpace(resp.Message) ? "Solicitud enviada." : resp.Message);

            if (toast is "success" or "info")
                await Shell.Current.DisplayAlert("Listo", mensaje, "OK");
            else
                await Shell.Current.DisplayAlert("Error", mensaje, "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
            OnPropertyChanged(nameof(IsNotBusy));
        }
    }

    private static bool IsValidEmail(string email)
    {
        try { _ = new MailAddress(email); return true; }
        catch { return false; }
    }
}
