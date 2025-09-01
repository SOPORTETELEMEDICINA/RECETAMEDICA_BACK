using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.Pacientes.Response;
using RMD.Shared.Models.Receta.AlertaToma.Request;
using RMD.Shared.Models.Receta.AlertaToma.Response;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace RMD.Movil.PageModels
{
    public partial class AvisosMedicacionPageModel : ObservableObject
    {
        private readonly IAlertasProgramadasControllerService _alertasProgramadasService;
        private readonly IAlertaTomaControllerService _alertaTomaService;

        private Guid _idPaciente;
        private const string PacientePrefKey = "Paciente";

        // Observable para poder reemplazar la instancia y forzar refresh del CollectionView
        [ObservableProperty]
        private ObservableCollection<AlertaProgramadaResponse> medicamentos = new();

        public ObservableCollection<DiaSemanaItem> Semana { get; } = new();

        [ObservableProperty] private string mesSemana = string.Empty;
        [ObservableProperty] private DateTime selectedDate = DateTime.Today;
        [ObservableProperty] private bool isBusy;

        public IAsyncRelayCommand LoadCommand { get; }
        public IAsyncRelayCommand AbrirAlertaManualCommand { get; }
        public IAsyncRelayCommand<DateTime> SeleccionarFechaCommand { get; }
        public IAsyncRelayCommand<AlertaProgramadaResponse> ToggleAlertaCommand { get; }
        public IAsyncRelayCommand ReloadCommand { get; }

        private bool _isNavigating;

        public AvisosMedicacionPageModel(
            IAlertasProgramadasControllerService alertasProgramadasService,
            IAlertaTomaControllerService alertaTomaService)
        {
            _alertasProgramadasService = alertasProgramadasService;
            _alertaTomaService = alertaTomaService;

            CargarIdPacienteDesdePreferences();
            ConstruirSemanaDesde(DateTime.Today);

            LoadCommand = new AsyncRelayCommand(async () =>
            {
                MarcarSeleccion(SelectedDate);
                await ReloadAsync();
            });

            ReloadCommand = new AsyncRelayCommand(ReloadAsync);
            AbrirAlertaManualCommand = new AsyncRelayCommand(OnAbrirAlertaManualAsync);
            SeleccionarFechaCommand = new AsyncRelayCommand<DateTime>(OnSeleccionarFechaAsync);
            ToggleAlertaCommand = new AsyncRelayCommand<AlertaProgramadaResponse>(OnToggleAlertaAsync);
        }

        private void CargarIdPacienteDesdePreferences()
        {
            try
            {
                var json = Preferences.Default.Get<string?>(PacientePrefKey, null);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    var paciente = JsonSerializer.Deserialize<PacienteConsultaResponse>(json);
                    if (paciente != null && paciente.IdPaciente != Guid.Empty)
                        _idPaciente = paciente.IdPaciente;
                }
            }
            catch { }
        }

        private async Task OnAbrirAlertaManualAsync()
        {
            if (_isNavigating) return;
            _isNavigating = true;
            try
            {
                await Shell.Current.GoToAsync(nameof(RMD.Movil.Pages.ActivarAlertaTomaManualPage));
            }
            finally
            {
                _isNavigating = false;
            }
        }

        private void ConstruirSemanaDesde(DateTime inicio)
        {
            Semana.Clear();
            MesSemana = inicio.ToString("MMMM yyyy", new CultureInfo("es-ES")).ToUpper();

            for (int i = 0; i < 7; i++)
            {
                var d = new DiaSemanaItem(inicio.AddDays(i))
                {
                    IsSelected = inicio.AddDays(i).Date == SelectedDate.Date
                };
                Semana.Add(d);
            }
        }

        private void MarcarSeleccion(DateTime fecha)
        {
            var f = fecha.Date;
            foreach (var d in Semana)
                d.IsSelected = (d.Date.Date == f);
        }

        private async Task OnSeleccionarFechaAsync(DateTime fecha)
        {
            var inicio = DateTime.Today.Date;
            var fin = inicio.AddDays(6);
            var f = fecha.Date;

            if (f < inicio || f > fin)
                return;

            SelectedDate = f;
            MarcarSeleccion(SelectedDate);
            await ReloadAsync();
        }

        // ========= Punto único de recarga =========
        public async Task ReloadAsync()
        {
            var fecha = SelectedDate.Date;
            if (_idPaciente == Guid.Empty)
            {
                await MainThread.InvokeOnMainThreadAsync(() => Medicamentos = new());
                return;
            }

            try
            {
                IsBusy = true;

                var req = new GetAlertasProgramadasEnFechaRequest
                {
                    IdPaciente = _idPaciente,
                    Fecha = fecha
                };

                var resp = await _alertasProgramadasService.GetAlertasProgramadasEnFechaAsync(req);
                var lista = resp?.Data?.ToList() ?? new List<AlertaProgramadaResponse>();

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Medicamentos = new ObservableCollection<AlertaProgramadaResponse>(lista);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error recargando alertas: {ex.Message}");
                await MainThread.InvokeOnMainThreadAsync(() => Medicamentos = new());
            }
            finally
            {
                IsBusy = false;
            }
        }

        // ===== Tap en ícono -> confirmar y desactivar, luego recargar =====
        private async Task OnToggleAlertaAsync(AlertaProgramadaResponse? item)
        {
            if (item is null) return;

            if (item.IdAlerta == Guid.Empty)
            {
                await MostrarAlertAsync("Alerta", "No se encontró el identificador de la alerta.");
                return;
            }

            var esManual = item.TipoAlerta == 2;
            var msg = $"¿Deseas desactivar el aviso de medicación{(esManual ? " manual" : "")} para:\n{item.Medicamento}?";

            var ok = await MostrarConfirmAsync("Desactivar alerta", msg, "Sí, desactivar", "Cancelar");
            if (!ok) return;

            try
            {
                if (IsBusy) return;
                IsBusy = true;

                if (esManual)
                {
                    var reqManual = new DesactivarAlertaManualRequest { IdAlertaTomaManual = item.IdAlerta };
                    var resp = await _alertaTomaService.DesactivarAlertaManualAsync(reqManual);
                    if (!EsOkToast(resp.Toast))
                    {
                        await MostrarAlertAsync("Alerta", resp.Message ?? "No se pudo desactivar la alerta manual.");
                        return;
                    }
                }
                else
                {
                    var req = new DesactivarAlertaTomaRequest { IdAlertaToma = item.IdAlerta };
                    var resp = await _alertaTomaService.DesactivarAlertaTomaAsync(req);
                    if (!EsOkToast(resp.Toast))
                    {
                        await MostrarAlertAsync("Alerta", resp.Message ?? "No se pudo desactivar la alerta.");
                        return;
                    }
                }

                await MostrarAlertAsync("Listo", "La alerta fue desactivada correctamente.");
                await ReloadAsync(); // recarga todo desde el backend
            }
            catch (Exception ex)
            {
                await MostrarAlertAsync("Error", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        // ----------------------------
        // Helpers (toast + UI dialogs)
        // ----------------------------
        private static bool EsOkToast(string? toast)
            => !string.IsNullOrWhiteSpace(toast) &&
               (toast.Equals("success", StringComparison.OrdinalIgnoreCase)
                || toast.Equals("ok", StringComparison.OrdinalIgnoreCase));

        // Evita Application.Current.MainPage (obsoleto): usa Shell o ventana activa
        private static Page? GetCurrentPage()
        {
            var shellPage = Shell.Current?.CurrentPage;
            if (shellPage is not null) return shellPage;

            var window = Application.Current?.Windows.FirstOrDefault(w => w?.Page is not null);
            return window?.Page;
        }

        private static Task MostrarAlertAsync(string titulo, string mensaje)
        {
            return MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var page = GetCurrentPage();
                if (page is null) return;
                await page.DisplayAlert(titulo, mensaje, "OK");
            });
        }

        private static Task<bool> MostrarConfirmAsync(string titulo, string mensaje, string aceptar = "Aceptar", string cancelar = "Cancelar")
        {
            return MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var page = GetCurrentPage();
                if (page is null) return false;
                return await page.DisplayAlert(titulo, mensaje, aceptar, cancelar);
            });
        }
    }

    public partial class DiaSemanaItem : ObservableObject
    {
        public DiaSemanaItem(DateTime date)
        {
            Date = date.Date;
            DayName = date.ToString("ddd", new CultureInfo("es-ES")).ToUpper();
            DayNumber = date.ToString("dd");
            IsToday = date.Date == DateTime.Today.Date;
        }

        public DateTime Date { get; }
        public string DayName { get; }
        public string DayNumber { get; }
        public bool IsToday { get; }

        [ObservableProperty] private bool isSelected;
    }
}
