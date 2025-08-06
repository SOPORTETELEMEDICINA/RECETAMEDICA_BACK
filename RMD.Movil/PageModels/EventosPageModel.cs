using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.Pacientes.Response;
using System.Collections.ObjectModel;

namespace RMD.Movil.PageModels;

public partial class EventosPageModel : ObservableObject
{
    private readonly IEventosSaludControllerService eventosService;

    public EventosPageModel(IEventosSaludControllerService eventosService)
    {
        this.eventosService = eventosService;
    }

    [ObservableProperty]
    private ObservableCollection<EventosSaludResponse> eventos = new();

    [RelayCommand]
    private async Task RegistrarNuevo()
    {
        await Shell.Current.GoToAsync("///RegistrarEventoSaludPage");


    }

    [RelayCommand]
    private async Task CargarEventos()
    {
        var result = await eventosService.GetAllEventosPacienteAsync();
        if (result.Toast == "success" || result.Toast == "info")
        {
            Eventos = new ObservableCollection<EventosSaludResponse>(result.Data ?? []);
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", result.Message, "OK");
        }
    }

    public async Task InitAsync()
    {
        await CargarEventos();
    }
}
