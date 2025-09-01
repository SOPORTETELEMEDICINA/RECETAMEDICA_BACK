using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RMD.Movil.Core.Service.Interfaces;
using RMD.Movil.PageModels.Controls;
using RMD.Movil.Services.Pdf;
using RMD.Shared.Models.Pacientes.Response;
using RMD.Shared.Models.Receta.Detalle.Response;
using RMD.Shared.Models.Receta.Header.Request;
using RMD.Shared.Models.Receta.Header.Responses;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace RMD.Movil.PageModels
{
    public partial class RecetasPageModel : BasePageModel
    {
        private readonly IRecetaControllerService _recetasService;
        private readonly IDetalleRecetasControllerService _detalleService;
        private readonly IPdfService _pdfService;

        public IAsyncRelayCommand AbrirAlertaManualCommand { get; }
        public IAsyncRelayCommand<object?> MostrarQrCommand { get; }
        public IAsyncRelayCommand<object?> DescargarPdfCommand { get; }
        public IAsyncRelayCommand<object?> MostrarDetalleCommand { get; }

        private const string PacientePrefKey = "Paciente";
        private bool _medsLoaded;

        public RecetasPageModel(IRecetaControllerService recetasService,
                                IDetalleRecetasControllerService detalleService,
                                IPdfService pdfService)
        {
            _recetasService = recetasService;
            _detalleService = detalleService;
            _pdfService = pdfService;

            IsRecetasTab = true;
            RecetasTabTextColor = Color.FromArgb("#9d69ca");
            MedicamentosTabTextColor = Color.FromArgb("#69728b");

            MostrarQrCommand = new AsyncRelayCommand<object?>(OnMostrarQrAsync);
            DescargarPdfCommand = new AsyncRelayCommand<object?>(OnDescargarPdfAsync);
            MostrarDetalleCommand = new AsyncRelayCommand<object?>(OnMostrarDetalleAsync);
            AbrirAlertaManualCommand = new AsyncRelayCommand(OnAbrirAlertaManualAsync);
        }

        [ObservableProperty] private ObservableCollection<HeaderTextPlainResponse> recetas = new();
        [ObservableProperty] private ObservableCollection<DetalleResponse> medicamentos = new();

        [ObservableProperty] private bool isRecetasTab;
        [ObservableProperty] private Color recetasTabTextColor;
        [ObservableProperty] private Color medicamentosTabTextColor;

        [RelayCommand]
        private async Task SwitchTab(string tab)
        {
            var toRecetas = tab == "Recetas";
            IsRecetasTab = toRecetas;
            RecetasTabTextColor = toRecetas ? Color.FromArgb("#9d69ca") : Color.FromArgb("#69728b");
            MedicamentosTabTextColor = toRecetas ? Color.FromArgb("#69728b") : Color.FromArgb("#9d69ca");

            if (!toRecetas && !_medsLoaded)
            {
                await CargarMedicamentosAsync();
            }
        }

        public async Task InitAsync()
        {
            try
            {
                if (IsBusy) return;
                IsBusy = true;

                await CargarRecetasAsync();
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
            var pacienteJson = Preferences.Default.Get<string?>(PacientePrefKey, null);
            if (string.IsNullOrWhiteSpace(pacienteJson))
                throw new Exception("No hay información de paciente en Preferences.");

            var pac = JsonSerializer.Deserialize<PacienteConsultaResponse>(pacienteJson);
            if (pac == null || pac.IdPaciente == Guid.Empty)
                throw new Exception("Paciente inválido en Preferences.");

            return pac.IdPaciente;
        }

        private async Task CargarRecetasAsync()
        {
            var result = await _recetasService.GetRecetasByPacienteAsync(); // backend resuelve por token
            if (!EsOkToast(result.Toast))
            {
                await MostrarAlertPorRespuestaAsync(result);
                Recetas = new ObservableCollection<HeaderTextPlainResponse>();
                return;
            }

            var lista = (result.Data ?? new List<HeaderTextPlainResponse>())
                .OrderByDescending(x => x.FechaCreacion)
                .ToList();

            Recetas = new ObservableCollection<HeaderTextPlainResponse>(lista);
        }

        private async Task CargarMedicamentosAsync()
        {
            try
            {
                if (IsBusy) return;
                IsBusy = true;

                var resp = await _detalleService.GetDetalleByPacienteAsync(); // sin null

                if (!EsOkToast(resp.Toast))
                {
                    await MostrarAlertPorRespuestaAsync(resp);
                    Medicamentos = new ObservableCollection<DetalleResponse>();
                    return;
                }

                Medicamentos = new ObservableCollection<DetalleResponse>(resp.Data ?? new List<DetalleResponse>());
                _medsLoaded = true;
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

        // Helpers
        private static bool TryGetGuid(object? param, out Guid id)
        {
            id = Guid.Empty;
            if (param is Guid g) { id = g; return true; }
            if (param is string s && Guid.TryParse(s, out var g2)) { id = g2; return true; }
            return false;
        }

        // Comandos (tu lógica actual)
        private async Task OnMostrarQrAsync(object? param)
        {
            if (!TryGetGuid(param, out var idReceta))
            {
                await MostrarAlertAsync("Error", "IdReceta inválido.");
                return;
            }

            try
            {
                if (IsBusy) return;
                IsBusy = true;

                // 👇 enviamos string para coincidir con QueryProperty string
                await Shell.Current.GoToAsync("RecetaQRPage", new Dictionary<string, object>
                {
                    { "IdReceta", idReceta.ToString() }
                });
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

        private async Task OnDescargarPdfAsync(object? param)
        {
            if (!TryGetGuid(param, out var idReceta))
            {
                await MostrarAlertAsync("Error", "IdReceta inválido.");
                return;
            }

            try
            {
                if (IsBusy) return;
                IsBusy = true;

                var idPaciente = ResolverIdPacienteDesdePreferences();

                var req = new RecetaRequest
                {
                    IdReceta = idReceta,
                    IdPaciente = idPaciente
                };

                // Ahora el service devuelve HTML plano
                var html = await _recetasService.GetRecetaByIdRecetaAsync(req);

                if (string.IsNullOrWhiteSpace(html))
                {
                    await MostrarAlertAsync("Error", "La receta no devolvió contenido HTML.");
                    return;
                }

                var pdfPath = await _pdfService.SaveHtmlToPdfAsync(html, $"receta_{idReceta:N}");
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(pdfPath)
                });
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
        // handler
        private async Task OnMostrarDetalleAsync(object? param)
        {
            if (!TryGetGuid(param, out var idReceta))
            {
                await MostrarAlertAsync("Error", "IdReceta inválido.");
                return;
            }

            if (IsBusy) return;
            IsBusy = true;
            try
            {
                await Shell.Current.GoToAsync(nameof(DetalleRecetaPage),
                    new Dictionary<string, object> { { "IdReceta", idReceta.ToString() } });
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

        private async Task OnAbrirAlertaManualAsync()
        {
            try
            {
                if (IsBusy) return;
                IsBusy = true;

                await Shell.Current.GoToAsync(nameof(ActivarAlertaTomaManualPage));
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
    }
}
