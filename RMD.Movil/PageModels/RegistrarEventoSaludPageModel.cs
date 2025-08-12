using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.Catalogo;
using RMD.Shared.Models.Pacientes.Request;
using RMD.Shared.Models.Pacientes.Response;
using RMD.Shared.Models.Receta.Detalle.Request;
using RMD.Shared.Models.Receta.Detalle.Response;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace RMD.Movil.PageModels;

public partial class RegistrarEventoSaludPageModel : ObservableObject
{
    private readonly ICatEventosSaludControllerService _catEventosService;
    private readonly IEventosSaludControllerService _eventosService;
    private readonly IDetalleRecetasControllerService _detalleService;

    public RegistrarEventoSaludPageModel(
        ICatEventosSaludControllerService catEventosService,
        IEventosSaludControllerService eventosService,
        IDetalleRecetasControllerService detalleService)
    {
        _catEventosService = catEventosService;
        _eventosService = eventosService;
        _detalleService = detalleService;

        FechaEvento = DateTime.Today;
        TiposEventos = new ObservableCollection<CatEventosDeSalud>();
        MedicamentosActivos = new ObservableCollection<MedicamentoActivoResponse>();

        _ = InitAsync(); // ← ahora iniciamos ambas cargas aquí
    }

    [ObservableProperty]
    ObservableCollection<CatEventosDeSalud> tiposEventos;

    [ObservableProperty]
    CatEventosDeSalud tipoSeleccionado;

    [ObservableProperty]
    DateTime fechaEvento;

    [ObservableProperty]
    string descripcion;

    [ObservableProperty]
    bool mostrarCampos;

    [ObservableProperty]
    bool mostrarMedicamentos;

    [ObservableProperty]
    ObservableCollection<MedicamentoActivoResponse> medicamentosActivos = new();

    [ObservableProperty]
    MedicamentoActivoResponse medicamentoSeleccionado;

    // Carga inicial: tipos + meds, y filtrado del IdEvento=14 si no hay meds
    private async Task InitAsync()
    {
        // Puedes hacerlo en paralelo:
        var tiposTask = _catEventosService.GetAllEventosSaludAsync();
        var medsTask = _detalleService.GetSoloMedicamentosActivosAsync();

        await Task.WhenAll(tiposTask, medsTask);

        var tiposResp = tiposTask.Result;
        var medsResp = medsTask.Result;

        if (tiposResp.Toast is "success" or "info")
        {
            var tipos = (tiposResp.Data ?? Enumerable.Empty<CatEventosDeSalud>()).ToList();

            var hayMeds = medsResp.Toast is "success" or "info" && medsResp.Data?.Any() == true;
            if (hayMeds)
            {
                MedicamentosActivos = new ObservableCollection<MedicamentoActivoResponse>(medsResp.Data!);
                MostrarMedicamentos = false; // solo se muestra cuando seleccionen el tipo 14
            }
            else
            {
                // No hay medicamentos: eliminar el tipo 14 de la lista
                tipos = tipos.Where(t => t.IdEvento != 14).ToList();

                // Si lo tenían seleccionado por algún motivo, lo limpiamos
                if (TipoSeleccionado?.IdEvento == 14)
                    TipoSeleccionado = null;

                MostrarMedicamentos = false;
            }

            TiposEventos = new ObservableCollection<CatEventosDeSalud>(tipos);
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "No se pudieron cargar los tipos de evento.", "OK");
        }
    }

    // Ya no volvemos a llamar al servicio aquí; usamos lo que cargamos en InitAsync
    partial void OnTipoSeleccionadoChanged(CatEventosDeSalud value)
    {
        MostrarCampos = value is not null;

        if (value?.IdEvento == 14)
        {
            // Mostrar picker solo si HAY medicamentos cargados
            MostrarMedicamentos = MedicamentosActivos?.Any() == true;
        }
        else
        {
            MostrarMedicamentos = false;
        }
    }

    // ... el resto de tus usings

    [RelayCommand]
    private async Task Guardar()
    {
        if (TipoSeleccionado is null || string.IsNullOrWhiteSpace(Descripcion))
        {
            await Shell.Current.DisplayAlert("Validación", "Todos los campos son obligatorios", "OK");
            return;
        }

        // Limpia la descripción del editor
        var descripcionLimpia = Descripcion
            .Trim()
            .Replace("\t", "")
            .Replace("\r", "")
            .Replace("\n", "");

        
        // Paciente desde preferencias
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

        // *** RAMA ESPECIAL PARA EVENTO 14 (Reacción a medicamento) ***
        if (TipoSeleccionado.IdEvento == 14)
        {
            // Debe haber un medicamento seleccionado
            if (MedicamentoSeleccionado is null)
            {
                await Shell.Current.DisplayAlert("Validación",
                    "Debes seleccionar el medicamento que provocó la reacción.", "OK");
                return;
            }
            // Agrega fecha a la descripción (elige el formato que prefieras)
            var descripcionConFecha = $"{FechaEvento:dd/MM/yyyy} - {descripcionLimpia}";

            // Arma el request para el endpoint de reacción
            var req = new DetalleRequest
            {
                IdDetalleReceta = MedicamentoSeleccionado.IdDetalleReceta,
                IdReceta = MedicamentoSeleccionado.IdReceta,
                MedicamentoId = MedicamentoSeleccionado.Drug,      // int
                MedicamentoType = MedicamentoSeleccionado.DrugType,  // string
                Descripcion = descripcionConFecha
            };

            var respReaccion = await _detalleService.CreateUpdateReaccionAsync(req);

            var toastRx = respReaccion.Toast?.ToLower() ?? "error";
            var mensajeRx = respReaccion.Descripcion?.Any() == true
                ? string.Join("\n", respReaccion.Descripcion)
                : (string.IsNullOrWhiteSpace(respReaccion.Message) ? "Operación completada." : respReaccion.Message);

            if (toastRx is "success" or "info")
            {
                await Shell.Current.DisplayAlert("Éxito", mensajeRx, "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", mensajeRx, "OK");
            }

            return; // importante: no sigas a la rama normal
        }

        // *** RAMA NORMAL PARA CUALQUIER OTRO TIPO DE EVENTO ***
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
    private async Task Back() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task Cancelar() => await Shell.Current.GoToAsync("..");
}
