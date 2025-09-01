using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RMD.Movil.Core.Service.Interfaces;
using RMD.Movil.PageModels.Controls;
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
        private readonly IAlertaTomaControllerService _alertaTomaService;

        // Collection para el CollectionView
        [ObservableProperty] private ObservableCollection<DetalleItemUI> detallesUI = new();

        // SelectedItem del CollectionView (dispara activar/desactivar)
        [ObservableProperty] private DetalleItemUI? selectedItem;

        public IAsyncRelayCommand BackCommand { get; }
        public IAsyncRelayCommand<DetalleItemUI> ActivarAlertaCommand { get; }

        public DetalleRecetaPageModel(
            IDetalleRecetasControllerService detallesService,
            IAlertasProgramadasControllerService alertasProgService,
            IAlertaTomaControllerService alertaTomaService)
        {
            _detallesService = detallesService;
            _alertasProgService = alertasProgService;
            _alertaTomaService = alertaTomaService;

            BackCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync(".."));
            ActivarAlertaCommand = new AsyncRelayCommand<DetalleItemUI>(OnActivarAlertaAsync);
        }

        // Tap en item:
        // - Si YA tiene alerta activa => confirmar y DESACTIVAR
        // - Si NO tiene => abrir modal de ACTIVAR
        partial void OnSelectedItemChanged(DetalleItemUI? value)
        {
            if (value is null) return;

            if (value.EsAlertaActiva)
            {
                _ = OnConfirmarDesactivarAsync(value);
                SelectedItem = null;
                return;
            }

            _ = OnActivarAlertaAsync(value);
            SelectedItem = null;
        }

        public async Task InitAsync(Guid idReceta)
        {
            try
            {
                if (IsBusy) return;
                IsBusy = true;

                // 1) Obtener detalles surtidos
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

                // 2) Consultar alertas activas del paciente
                var idPaciente = ResolverIdPacienteDesdePreferences();
                var filtro = new GetAlertasProgramadasRequest { IdPaciente = idPaciente };
                var respAlertas = await _alertasProgService.GetAlertasProgramadasByIdPacienteAsync(filtro);
                var alertas = respAlertas.Data ?? new List<AlertaProgramadaResponse>();

                // Conjunto (IdReceta, IdDetalleReceta) activos
                var activos = alertas
                    .Select(a => (a.IdReceta.GetValueOrDefault(), a.IdDetalleReceta.GetValueOrDefault()))
                    .Where(t => t.Item1 != Guid.Empty && t.Item2 != Guid.Empty)
                    .ToHashSet();

                // Índice para extraer IdAlerta / TipoAlerta
                var indexAlertas = alertas
                    .Where(a => a.IdReceta.HasValue && a.IdReceta.Value != Guid.Empty
                             && a.IdDetalleReceta.HasValue && a.IdDetalleReceta.Value != Guid.Empty)
                    .ToDictionary(a => (a.IdReceta!.Value, a.IdDetalleReceta!.Value), a => a);

                // 3) Mapear a UI con marca de "activa" + IdAlerta
                var ui = detalles.Select(d =>
                {
                    var esActiva = activos.Contains((d.IdReceta, d.IdDetalleReceta));

                    Guid? idAlerta = null;
                    int? tipoAlerta = null;

                    if (esActiva && indexAlertas.TryGetValue((d.IdReceta, d.IdDetalleReceta), out var alerta))
                    {
                        idAlerta = alerta.IdAlerta;     // GUID del modelo compartido
                        tipoAlerta = alerta.TipoAlerta; // 1=Normal, 2=Manual (no usado aquí)
                    }

                    return new DetalleItemUI(d)
                    {
                        EsAlertaActiva = esActiva,
                        IdAlerta = idAlerta,
                        TipoAlerta = tipoAlerta
                    };
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

        private async Task OnActivarAlertaAsync(DetalleItemUI? item)
        {
            if (item is null) return;

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

        // ====== DESACTIVAR ALERTA (normal) ======
        private async Task OnConfirmarDesactivarAsync(DetalleItemUI item)
        {
            try
            {
                if (item.IdAlerta is null || item.IdAlerta == Guid.Empty)
                {
                    await MostrarAlertAsync("Alerta", "No se encontró el identificador de la alerta vinculada.");
                    return;
                }

                var ok = await MostrarConfirmAsync(
                    "Desactivar alerta",
                    $"¿Deseas desactivar la alerta para:\n{item.MedicamentoNombre}?",
                    "Sí, desactivar", "No");

                if (!ok) return;

                await DesactivarAlertaAsync(item);
            }
            catch (Exception ex)
            {
                await MostrarAlertAsync("Error", ex.Message);
            }
        }

        private async Task DesactivarAlertaAsync(DetalleItemUI item)
        {
            try
            {
                if (IsBusy) return;
                IsBusy = true;

                var req = new DesactivarAlertaTomaRequest
                {
                    IdAlertaToma = item.IdAlerta!.Value
                };

                var resp = await _alertaTomaService.DesactivarAlertaTomaAsync(req);
                if (!EsOkToast(resp.Toast))
                {
                    await MostrarAlertPorRespuestaAsync(resp);
                    return;
                }

                // Éxito: limpiar estado visual y vínculo
                item.EsAlertaActiva = false;
                item.IdAlerta = null;

                await MostrarAlertAsync("Alerta", "La alerta fue desactivada correctamente.");
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

        // Helper local (no depende de BasePageModel)
        private Task<bool> MostrarConfirmAsync(
            string titulo,
            string mensaje,
            string textoAceptar = "Sí",
            string textoCancelar = "No")
        {
            return MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var page = Shell.Current?.CurrentPage ?? Application.Current?.MainPage;
                if (page is null) return false;

                return await page.DisplayAlert(titulo, mensaje, textoAceptar, textoCancelar);
            });
        }
    }

    // ============================
    // UI Model (Item de la lista)
    // ============================
    public partial class DetalleItemUI : ObservableObject
    {
        public DetalleResponse Source { get; }
        public DetalleItemUI(DetalleResponse source) => Source = source;

        [ObservableProperty] private bool esAlertaActiva;

        // NUEVOS: campos del modelo de alerta (para poder desactivar)
        private Guid? _idAlerta;
        public Guid? IdAlerta
        {
            get => _idAlerta;
            set => SetProperty(ref _idAlerta, value);
        }

        private int? _tipoAlerta; // 1=Normal, 2=Manual (opcional)
        public int? TipoAlerta
        {
            get => _tipoAlerta;
            set => SetProperty(ref _tipoAlerta, value);
        }

        // Passthroughs al modelo original
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

        // === Estilo (verde cuando hay alerta activa) ===
        public Color CardStrokeColor => EsAlertaActiva ? Color.FromArgb("#22C55E") : Color.FromArgb("#E0E3EB");
        public Color CardBackgroundColor => EsAlertaActiva ? Color.FromArgb("#ECFDF5") : Colors.White;
        public Color BadgeBgColor => EsAlertaActiva ? Color.FromArgb("#22C55E") : Colors.Transparent;
        public bool MostrarBadge => EsAlertaActiva;
        public double CardOpacity => 1.0;

        partial void OnEsAlertaActivaChanged(bool value)
        {
            OnPropertyChanged(nameof(CardStrokeColor));
            OnPropertyChanged(nameof(CardBackgroundColor));
            OnPropertyChanged(nameof(BadgeBgColor));
            OnPropertyChanged(nameof(MostrarBadge));
            OnPropertyChanged(nameof(CardOpacity));
        }
    }
}
