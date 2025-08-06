using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.Catalogo;
using RMD.Shared.Models.Pacientes.Request;
using RMD.Shared.Models.Pacientes.Response;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace RMD.Movil.PageModels;

public partial class RegistrarEventoSaludPageModel : ObservableObject
{
    private readonly ICatEventosSaludControllerService _catEventosService;
    private readonly IEventosSaludControllerService _eventosService;

    public RegistrarEventoSaludPageModel(ICatEventosSaludControllerService catEventosService, IEventosSaludControllerService eventosService)
    {
        _catEventosService = catEventosService;
        _eventosService = eventosService;
        FechaEvento = DateTime.Today;
        TiposEventos = new ObservableCollection<CatEventosDeSalud>();

        _ = CargarTiposEventosAsync();
    }

    [ObservableProperty]
    ObservableCollection<CatEventosDeSalud> tiposEventos;

    [ObservableProperty]
    CatEventosDeSalud tipoSeleccionado;

    [ObservableProperty]
    DateTime fechaEvento;

    [ObservableProperty]
    string descripcion;

    [RelayCommand]
    private async Task Guardar()
    {
        if (TipoSeleccionado is null || string.IsNullOrWhiteSpace(Descripcion))
        {
            await Shell.Current.DisplayAlert("Validación", "Todos los campos son obligatorios", "OK");
            return;
        }

        // Limpiar tabs, enters y espacios finales de la descripción
        var descripcionLimpia = Descripcion
            .Trim()
            .Replace("\t", "")
            .Replace("\r", "")
            .Replace("\n", "");

        // Obtener paciente desde Preferences
        var json = Preferences.Default.Get<string>("Paciente", null);
        if (string.IsNullOrWhiteSpace(json))
        {
            await Shell.Current.DisplayAlert("Error", "No se encontró información del paciente.", "OK");
            return;
        }

        var paciente = JsonSerializer.Deserialize<PacienteConsultaResponse>(json);
        if (paciente == null || paciente.IdPaciente == Guid.Empty)
        {
            await Shell.Current.DisplayAlert("Error", "El paciente no es válido.", "OK");
            return;
        }

        var evento = new EventosSaludRequest
        {
            IdEventoSalud = Guid.NewGuid(),
            IdPaciente = paciente.IdPaciente,
            Fecha = FechaEvento,
            EventoDeSalud = TipoSeleccionado.IdEvento,
            Descripcion = descripcionLimpia
        };

        var response = await _eventosService.CreateEventoPacienteAsync(evento);

        var toast = response.Toast?.ToLower() ?? "error";
        var mensaje = response.Descripcion?.Any() == true
            ? string.Join("\n", response.Descripcion)
            : response.Message;

        if ((toast is "success" or "info") && response.Data)
        {
            await Shell.Current.DisplayAlert("Éxito", mensaje, "OK");
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", mensaje, "OK");
        }
    }



    [RelayCommand]
    private async Task Cancelar()
    {
        await Shell.Current.GoToAsync("///EventosPage");
    }

    private async Task CargarTiposEventosAsync()
    {
        var response = await _catEventosService.GetAllEventosSaludAsync();
        if (response.Toast == "success" || response.Toast == "info")
        {
            TiposEventos = new ObservableCollection<CatEventosDeSalud>(response.Data);
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "No se pudieron cargar los tipos de evento.", "OK");
        }
    }
}
