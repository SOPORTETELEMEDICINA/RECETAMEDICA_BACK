using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.Pacientes.Response;
using RMD.Shared.Models.Receta.AlertaToma.Request;
using RMD.Shared.Models.Receta.AlertaToma.Response;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;
using System.Diagnostics;

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

        // Solo para depuración (puedes ocultarla en UI o dejarla para Output)
        [ObservableProperty] private string? debugUltimaRespuesta;

        public IAsyncRelayCommand LoadCommand { get; }

        public AvisosMedicacionPageModel(IAlertasProgramadasControllerService alertasProgramadasService)
        {
            _alertasProgramadasService = alertasProgramadasService;

            CargarIdPacienteDesdePreferences();

            LoadCommand = new AsyncRelayCommand(async () =>
            {
                if (Semana.Count == 0)
                    ConstruirSemanaDesde(DateTime.Today);

                await CargarMedicamentosEnFechaAsync(DateTime.Today);
            });

            // Inicializa semana (hoy + 6)
            ConstruirSemanaDesde(DateTime.Today);
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

                // --- Debug visible en Output y opcionalmente en UI ---
                var opcionesJson = new JsonSerializerOptions { WriteIndented = true };
                debugUltimaRespuesta = JsonSerializer.Serialize(resp, opcionesJson);
                Debug.WriteLine($"[Avisos] {fecha:yyyy-MM-dd} -> {debugUltimaRespuesta}");

                foreach (var a in resp?.Data ?? Enumerable.Empty<AlertaProgramadaResponse>())
                    Medicamentos.Add(a);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error cargando alertas por fecha: {ex.Message}");
                debugUltimaRespuesta = $"ERROR: {ex.Message}";
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
    }
}
