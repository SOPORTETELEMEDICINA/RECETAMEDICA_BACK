using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage; // Preferences
using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.Pacientes.Response; // modelo de tu "Paciente" guardado
using RMD.Shared.Models.Receta.AlertaToma.Request;
using RMD.Shared.Models.Receta.AlertaToma.Response;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace RMD.Movil.PageModels
{
    public partial class AvisosMedicacionPageModel : ObservableObject
    {
        private readonly IAlertasProgramadasControllerService _alertasProgramadasService;
        private Guid _idPaciente;
        private const string PacientePrefKey = "Paciente";

        private DateTime _startOfWeek;

        public ObservableCollection<AlertaProgramadaResponse> Medicamentos { get; } = new();
        public ObservableCollection<DiaSemanaItem> Semana { get; } = new();

        [ObservableProperty] private string mesSemana = string.Empty;
        [ObservableProperty] private bool isPopupVisible;
        [ObservableProperty] private DiaSemanaItem? diaSeleccionado;

        public IAsyncRelayCommand LoadCommand { get; }
        public IRelayCommand AbrirCalendarioCommand { get; }
        public IRelayCommand<DiaSemanaItem> SeleccionarDiaCommand { get; }

        public AvisosMedicacionPageModel(IAlertasProgramadasControllerService alertasProgramadasService)
        {
            _alertasProgramadasService = alertasProgramadasService;

            CargarIdPacienteDesdePreferences();

            LoadCommand = new AsyncRelayCommand(CargarMedicamentosAsync);
            AbrirCalendarioCommand = new RelayCommand(() => IsPopupVisible = true);
            SeleccionarDiaCommand = new RelayCommand<DiaSemanaItem>(d => SeleccionarDia(d));

            _startOfWeek = GetStartOfWeek(DateTime.Today);
            ConstruirSemana(_startOfWeek);
            _ = CargarMedicamentosAsync();
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
            catch { /* swallow: deja _idPaciente = Guid.Empty */ }
        }

        public void SeleccionarFechaDesdePopup(DateTime fecha)
        {
            IsPopupVisible = false;
            _startOfWeek = GetStartOfWeek(fecha);
            ConstruirSemana(_startOfWeek);

            var match = Semana.FirstOrDefault(d => d.Date.Date == fecha.Date);
            if (match != null)
                SeleccionarDia(match);
        }

        private async Task CargarMedicamentosAsync()
        {
            try
            {
                if (_idPaciente == Guid.Empty)
                {
                    System.Diagnostics.Debug.WriteLine("IdPaciente no disponible en Preferences.");
                    Medicamentos.Clear();
                    return;
                }

                var request = new GetAlertasProgramadasRequest { IdPaciente = _idPaciente };
                var result = await _alertasProgramadasService.GetAlertasProgramadasByIdPacienteAsync(request);

                Medicamentos.Clear();
                if (result?.Data != null && result.Data.Count > 0)
                    foreach (var alerta in result.Data)
                        Medicamentos.Add(alerta);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando alertas: {ex.Message}");
            }
        }

        private static DateTime GetStartOfWeek(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-diff).Date;
        }

        private void ConstruirSemana(DateTime inicioSemana)
        {
            Semana.Clear();
            MesSemana = inicioSemana.ToString("MMMM yyyy", new CultureInfo("es-ES")).ToUpper();

            for (int i = 0; i < 7; i++)
                Semana.Add(new DiaSemanaItem(inicioSemana.AddDays(i)));

            var hoy = Semana.FirstOrDefault(d => d.Date.Date == DateTime.Today);
            if (hoy != null)
            {
                SeleccionarDia(hoy, actualizarPropiedad: false);
                DiaSeleccionado = hoy;
            }
        }

        private void SeleccionarDia(DiaSemanaItem? dia, bool actualizarPropiedad = true)
        {
            if (dia == null) return;
            foreach (var d in Semana) d.IsSelected = false;
            dia.IsSelected = true;
            if (actualizarPropiedad) DiaSeleccionado = dia;
        }

        partial void OnDiaSeleccionadoChanged(DiaSemanaItem? value)
        {
            if (value != null) SeleccionarDia(value, actualizarPropiedad: false);
        }
    }

    public partial class DiaSemanaItem : ObservableObject
    {
        public DiaSemanaItem(DateTime date)
        {
            Date = date.Date;
            DayName = date.ToString("ddd", new CultureInfo("es-ES")).ToUpper();
            DayNumber = date.ToString("dd");
            IsToday = date.Date == DateTime.Today;
        }
        public DateTime Date { get; }
        public string DayName { get; }
        public string DayNumber { get; }
        public bool IsToday { get; }
        [ObservableProperty] private bool isSelected;
    }
}
