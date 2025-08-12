using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RMD.Movil.Core.Service.Interfaces;
using RMD.Movil.PageModels.Controls; // BasePageModel
using RMD.Shared.Models.Pacientes.Response;
using RMD.Shared.Models.Receta.AlertaToma.Request;
using RMD.Shared.Models.Receta.AlertaToma.Response;
using RMD.Shared.Models.Receta.Detalle.Response;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace RMD.Movil.PageModels
{
    public partial class DetalleRecetaPageModel : BasePageModel
    {
        private readonly IDetalleRecetasControllerService _detallesService;
        private readonly IAlertasProgramadasControllerService _alertasProgService;

        [ObservableProperty] private ObservableCollection<DetalleItemUI> detallesUI = new();

        public IAsyncRelayCommand BackCommand { get; }
        public IAsyncRelayCommand<DetalleItemUI> ActivarAlertaCommand { get; }

        public DetalleRecetaPageModel(
            IDetalleRecetasControllerService detallesService,
            IAlertasProgramadasControllerService alertasProgService)
        {
            _detallesService = detallesService;
            _alertasProgService = alertasProgService;

            BackCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync(".."));
            ActivarAlertaCommand = new AsyncRelayCommand<DetalleItemUI>(OnActivarAlertaAsync);
        }

        public async Task InitAsync(Guid idReceta)
        {
            try
            {
                if (IsBusy) return;
                IsBusy = true;

                // 1) Detalles (solo surtidos)
                var respDetalles = await _detallesService.GetDetallesByIdRecetaAsync(idReceta);
                if (!EsOkToast(respDetalles.Toast))
                {
                    await MostrarAlertPorRespuestaAsync(respDetalles);
                    DetallesUI = new();
                    return;
                }

                var detalles = (respDetalles.Data ?? new List<DetalleResponse>())
                    .Where(d => d.CantidadSurtida.HasValue && d.CantidadSurtida.Value > 0)
                    .ToList();

                // 2) Alertas programadas del paciente
                var idPaciente = ResolverIdPacienteDesdePreferences();
                var filtro = new GetAlertasProgramadasRequest { IdPaciente = idPaciente };
                var respAlertas = await _alertasProgService.GetAlertasProgramadasByIdPacienteAsync(filtro);

                var activos = (respAlertas.Data ?? new List<AlertaProgramadaResponse>())
                    .Select(a => (a.IdReceta.GetValueOrDefault(), a.IdDetalleReceta.GetValueOrDefault()))
                    .Where(t => t.Item1 != Guid.Empty && t.Item2 != Guid.Empty)
                    .ToHashSet();

                // 3) Mapear a UI y marcar activos
                var ui = detalles.Select(d => new DetalleItemUI(d)
                {
                    EsAlertaActiva = activos.Contains((d.IdReceta, d.IdDetalleReceta))
                }).ToList();

                DetallesUI = new ObservableCollection<DetalleItemUI>(ui);
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

        private Guid ResolverIdPacienteDesdePreferences()
        {
            var json = Preferences.Default.Get<string?>("Paciente", null)
                       ?? throw new Exception("No hay información de paciente en Preferences.");
            var pac = JsonSerializer.Deserialize<PacienteConsultaResponse>(json)
                      ?? throw new Exception("Paciente guardado inválido.");
            if (pac.IdPaciente == Guid.Empty) throw new Exception("IdPaciente inválido.");
            return pac.IdPaciente;
        }

        private async Task OnActivarAlertaAsync(DetalleItemUI item)
        {
            if (item is null || item.EsAlertaActiva)
                return;

            try
            {
                var nav = new Dictionary<string, object>
                {
                    ["IdReceta"] = item.IdReceta.ToString(),
                    ["IdDetalleReceta"] = item.IdDetalleReceta.ToString(),
                    ["MedicamentoId"] = item.MedicamentoId.ToString(),
                    ["MedicamentoType"] = item.MedicamentoType ?? string.Empty
                };

                await Shell.Current.GoToAsync(nameof(ActivarAlertaTomaPage), nav);
            }
            catch (Exception ex)
            {
                await MostrarAlertAsync("Navegación", ex.Message);
            }
        }
    }

    public partial class DetalleItemUI : ObservableObject
    {
        public DetalleResponse Source { get; }
        public DetalleItemUI(DetalleResponse source) => Source = source;

        [ObservableProperty] private bool esAlertaActiva;

        // Passthroughs
        public Guid IdReceta => Source.IdReceta;
        public Guid IdDetalleReceta => Source.IdDetalleReceta;
        public int MedicamentoId => Source.MedicamentoId;
        public string MedicamentoType => Source.MedicamentoType;
        public string MedicamentoNombre => Source.MedicamentoNombre;
        public int? CantidadSurtida => Source.CantidadSurtida;
        public string UnidadDispensacion => Source.UnidadDispensacion;
        public decimal CantidadDiaria => Source.CantidadDiaria;
        public int Frecuency => Source.Frecuency;
        public string? FrecuencyType => Source.FrecuencyType;
        public string? Indicacion => Source.Indicacion;

        // Estilo
        public Color CardStrokeColor => Color.FromArgb("#E0E3EB"); // borde gris SIEMPRE
        public Color CardBackgroundColor => EsAlertaActiva ? Color.FromArgb("#E6F7E6") : Colors.White;

        public Color BadgeBgColor => EsAlertaActiva ? Color.FromArgb("#22C55E") : Colors.Transparent;
        public bool MostrarBadge => EsAlertaActiva;

        public bool ItemIsEnabled => !EsAlertaActiva;

        partial void OnEsAlertaActivaChanged(bool value)
        {
            OnPropertyChanged(nameof(CardStrokeColor));
            OnPropertyChanged(nameof(CardBackgroundColor));
            OnPropertyChanged(nameof(BadgeBgColor));
            OnPropertyChanged(nameof(MostrarBadge));
            OnPropertyChanged(nameof(ItemIsEnabled));
        }
    }
}
