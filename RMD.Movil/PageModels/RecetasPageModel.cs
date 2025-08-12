using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RMD.Movil.Core.Service.Interfaces;
using RMD.Movil.PageModels.Controls; // BasePageModel
using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Models.Pacientes.Response;
using RMD.Shared.Models.Receta.Header.Request;
using RMD.Shared.Models.Receta.Header.Responses;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Linq;
using RMD.Movil.Services.Pdf;

namespace RMD.Movil.PageModels
{
    public partial class RecetasPageModel : BasePageModel
    {
        private readonly IRecetaControllerService _recetasService;
        private readonly IPdfService _pdfService;

        private const string PacientePrefKey = "Paciente";

        public RecetasPageModel(IRecetaControllerService recetasService, IPdfService pdfService)
        {
            _recetasService = recetasService;
            _pdfService = pdfService;

            ActivasTextColor = Color.FromArgb("#9d69ca");
            VencidasTextColor = Color.FromArgb("#69728b");
            // 👇 ahora aceptamos object? y parseamos dentro
            MostrarQrCommand = new AsyncRelayCommand<object?>(OnMostrarQrAsync);
            DescargarPdfCommand = new AsyncRelayCommand<object?>(OnDescargarPdfAsync);
            MostrarDetalleCommand = new AsyncRelayCommand<object?>(OnMostrarDetalleAsync);
        }
        public IAsyncRelayCommand<object?> MostrarDetalleCommand { get; }

        // en el constructor
        [ObservableProperty]
        private ObservableCollection<HeaderTextPlainResponse> recetas = new();

        [ObservableProperty]
        private ObservableCollection<HeaderTextPlainResponse> recetasFiltradas = new();

        [ObservableProperty]
        private bool isActivasTab = true;

        [ObservableProperty]
        private Color activasTextColor;

        [ObservableProperty]
        private Color vencidasTextColor;

        public IAsyncRelayCommand<object?> MostrarQrCommand { get; }
        public IAsyncRelayCommand<object?> DescargarPdfCommand { get; }

        [RelayCommand]
        private void SwitchTab(string tab)
        {
            IsActivasTab = tab == "Activas";
            ActivasTextColor = IsActivasTab ? Color.FromArgb("#9d69ca") : Color.FromArgb("#69728b");
            VencidasTextColor = IsActivasTab ? Color.FromArgb("#69728b") : Color.FromArgb("#9d69ca");
            Filtrar();
        }

        public async Task InitAsync()
        {
            try
            {
                if (IsBusy) return;
                IsBusy = true;

                var idPaciente = ResolverIdPacienteDesdePreferences();
                await CargarRecetasAsync(idPaciente);
                Filtrar();
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

        private void Filtrar()
        {
            // Activas: 1 Activa, 2 Surtida, 5 Surtida Parcial
            // Vencidas/Canceladas: 3 Cancelada, 4 Vencida, 6 Vencida Surtida Parcial
            if (IsActivasTab)
            {
                RecetasFiltradas = new ObservableCollection<HeaderTextPlainResponse>(
                    Recetas
                        .Where(r => r.Estatus == 1 || r.Estatus == 2 || r.Estatus == 5)
                        .OrderByDescending(r => r.FechaCreacion));
            }
            else
            {
                RecetasFiltradas = new ObservableCollection<HeaderTextPlainResponse>(
                    Recetas
                        .Where(r => r.Estatus == 3 || r.Estatus == 4 || r.Estatus == 6)
                        .OrderByDescending(r => r.FechaCreacion));
            }
        }

        private Guid ResolverIdPacienteDesdePreferences()
        {
            var pacienteJson = Preferences.Default.Get<string?>(PacientePrefKey, null);
            if (string.IsNullOrWhiteSpace(pacienteJson))
                throw new Exception("No hay información de paciente en Preferences.");

            PacienteConsultaResponse? pac;
            try
            {
                pac = JsonSerializer.Deserialize<PacienteConsultaResponse>(pacienteJson);
            }
            catch
            {
                throw new Exception("No se pudo leer el Paciente guardado.");
            }

            if (pac == null || pac.IdPaciente == Guid.Empty)
                throw new Exception("Paciente inválido en Preferences.");

            return pac.IdPaciente;
        }

        private async Task CargarRecetasAsync(Guid idPaciente)
        {
            var filtro = new HeaderFilterByPacienteRequest { IdPaciente = idPaciente };

            ResponseFromService<List<HeaderTextPlainResponse>> result =
                await _recetasService.GetRecetasByIdPacienteAsync(filtro);

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

        // === Helpers de parámetros ===
        private static bool TryGetGuid(object? param, out Guid id)
        {
            id = Guid.Empty;
            if (param is Guid g) { id = g; return true; }
            if (param is string s && Guid.TryParse(s, out var g2)) { id = g2; return true; }
            return false;
        }

        // === Comandos ===
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
    }
}
