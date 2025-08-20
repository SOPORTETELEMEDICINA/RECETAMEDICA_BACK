using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.Pacientes.Response;
using RMD.Shared.Models.Receta.AlertaToma.Request;
using RMD.Shared.Models.Receta.AlertaToma.Response;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;

namespace RMD.Movil.PageModels
{
    public partial class AvisosMedicacionPageModel : ObservableObject
    {
        private readonly IAlertasProgramadasControllerService _alertasProgramadasService;
        private Guid _idPaciente;
        private const string PacientePrefKey = "Paciente";

        public ObservableCollection<AlertaProgramadaResponse> Medicamentos { get; } = new();
        public ObservableCollection<DiaSemanaItem> Semana { get; } = new();

        [ObservableProperty] private string mesSemana = string.Empty;
        [ObservableProperty] private DiaSemanaItem? diaSeleccionado;

        public IAsyncRelayCommand LoadCommand { get; }
        public IRelayCommand<DiaSemanaItem> SeleccionarDiaCommand { get; }

        public AvisosMedicacionPageModel(IAlertasProgramadasControllerService alertasProgramadasService)
        {
            _alertasProgramadasService = alertasProgramadasService;

            CargarIdPacienteDesdePreferences();

            SeleccionarDiaCommand = new RelayCommand<DiaSemanaItem>(d => SeleccionarDia(d));
            LoadCommand = new AsyncRelayCommand(async () =>
            {
                if (Semana.Count == 0)
                    ConstruirSemanaDesde(DateTime.Today);

                var fecha = DiaSeleccionado?.Date ?? DateTime.Today;
                await CargarMedicamentosEnFechaAsync(fecha);
            });

            ConstruirSemanaDesde(DateTime.Today);
            var hoy = Semana.FirstOrDefault();
            if (hoy != null) DiaSeleccionado = hoy; // dispara carga
        }

        // Llamado desde el popup (si tu XAML aún lo usa)
        public void SeleccionarFechaDesdePopup(DateTime fecha)
        {
            ConstruirSemanaDesde(fecha.Date);

            var match = Semana.FirstOrDefault(d => d.Date == fecha.Date) ?? Semana.First();
            DiaSeleccionado = match; // setter dispara carga
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

        private void ConstruirSemanaDesde(DateTime inicio)
        {
            Semana.Clear();
            MesSemana = inicio.ToString("MMMM yyyy", new CultureInfo("es-ES")).ToUpper();

            for (int i = 0; i < 7; i++)
                Semana.Add(new DiaSemanaItem(inicio.AddDays(i)));
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
            if (value == null) return;
            SeleccionarDia(value, actualizarPropiedad: false);
            _ = CargarMedicamentosEnFechaAsync(value.Date);
        }

        private async Task CargarMedicamentosEnFechaAsync(DateTime fecha)
        {
            try
            {
                Medicamentos.Clear();
                if (_idPaciente == Guid.Empty) return;

                var req = new GetAlertasProgramadasEnFechaRequest
                {
                    IdPaciente = _idPaciente,
                    Fecha = fecha
                };

                var resp = await _alertasProgramadasService.GetAlertasProgramadasEnFechaAsync(req);
                foreach (var a in resp?.Data ?? Enumerable.Empty<AlertaProgramadaResponse>())
                    Medicamentos.Add(a);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando alertas por fecha: {ex.Message}");
            }
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
