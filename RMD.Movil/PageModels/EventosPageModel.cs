using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RMD.Movil.Core.Service.Interfaces;
using System.Collections.ObjectModel;
using EventosSaludModel = RMD.Movil.Models.EventosSaludModel;

namespace RMD.Movil.PageModels;

public partial class EventosPageModel : ObservableObject
{
    private readonly IEventosSaludControllerService eventosService;

    public EventosPageModel(IEventosSaludControllerService eventosService)
    {
        this.eventosService = eventosService;
    }

    [ObservableProperty]
    private ObservableCollection<EventosSaludModel> eventos = new();

    [RelayCommand]
    private async Task RegistrarNuevo() => await Shell.Current.GoToAsync("RegistrarEventoSaludPage");

    [RelayCommand]
    private async Task CargarEventos()
    {
        var result = await eventosService.GetAllEventosPacienteAsync();
        if (result.Toast == "success" || result.Toast == "info")
        {
            var lista = await Task.Run(() =>
            {
                return (result.Data ?? []).Select(e =>
                {
                    var model = new EventosSaludModel
                    {
                        IdEventoSalud = e.IdEventoSalud,
                        IdPaciente = e.IdPaciente,
                        Fecha = e.Fecha,
                        EventoDeSalud = e.EventoDeSalud,
                        Descripcion = e.Descripcion,
                        NombreEvento = e.NombreEvento
                    };
                    model.CalcularColor();
                    return model;
                }).ToList();
            });

            Eventos = new ObservableCollection<EventosSaludModel>(lista);
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