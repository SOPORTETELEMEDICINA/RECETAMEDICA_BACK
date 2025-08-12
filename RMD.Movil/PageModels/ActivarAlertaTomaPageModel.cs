using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RMD.Movil.Core.Service.Interfaces;
using RMD.Movil.PageModels.Controls;
// Ajusta el namespace del request según tu Shared Models
using RMD.Shared.Models.Receta.AlertaToma.Request;

namespace RMD.Movil.PageModels
{
    public partial class ActivarAlertaTomaPageModel : BasePageModel
    {
        private readonly IAlertaTomaControllerService _alertaService;

        // Parámetros de la receta/detalle
        public Guid IdReceta { get; set; }
        public Guid IdDetalleReceta { get; set; }
        public int MedicamentoId { get; set; }
        public string MedicamentoType { get; set; } = string.Empty;

        [ObservableProperty]
        private DateTime fechaSeleccionada = DateTime.Today;

        [ObservableProperty]
        private TimeSpan horaSeleccionada = DateTime.Now.TimeOfDay;

        public IAsyncRelayCommand CancelarCommand { get; }
        public IAsyncRelayCommand AceptarCommand { get; }

        public ActivarAlertaTomaPageModel(IAlertaTomaControllerService alertaService)
        {
            _alertaService = alertaService;
            CancelarCommand = new AsyncRelayCommand(() => Shell.Current.GoToAsync(".."));
            AceptarCommand = new AsyncRelayCommand(OnAceptarAsync);
        }

        private async Task OnAceptarAsync()
        {
            try
            {
                if (IsBusy) return;
                IsBusy = true;

                var fechaHora = FechaSeleccionada.Date + HoraSeleccionada;

                var req = new ActivarAlertaTomaRequest
                {
                    IdReceta = IdReceta,
                    IdDetalleReceta = IdDetalleReceta,
                    MedicamentoId = MedicamentoId,
                    MedicamentoType = MedicamentoType,
                    FechaHoraPrimerToma = fechaHora
                };

                var resp = await _alertaService.ActivarAlertaTomaAsync(req);

                if (!EsOkToast(resp.Toast))
                {
                    await MostrarAlertPorRespuestaAsync(resp);
                    return;
                }

                await MostrarAlertAsync("Listo", resp.Message ?? "Alerta activada.");
                await Shell.Current.GoToAsync("..");
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
