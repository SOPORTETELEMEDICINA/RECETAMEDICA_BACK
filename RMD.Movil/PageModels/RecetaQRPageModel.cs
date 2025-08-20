using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RMD.Movil.Core.Service.Interfaces;
using RMD.Movil.PageModels.Controls;

namespace RMD.Movil.PageModels
{
    public partial class RecetaQRPageModel : BasePageModel
    {
        private readonly IRecetaControllerService _recetasService;

        public RecetaQRPageModel(IRecetaControllerService recetasService)
        {
            _recetasService = recetasService;

            CloseCommand = new AsyncRelayCommand(OnCloseAsync);
        }

        [ObservableProperty]
        private ImageSource? qrImageSource;

        public IAsyncRelayCommand CloseCommand { get; }

        public async Task InitAsync(Guid idReceta)
        {
            try
            {
                if (IsBusy) return;
                IsBusy = true;

                var resp = await _recetasService.GetQRByIdRecetaAsync(idReceta);
                if (!EsOkToast(resp.Toast) || string.IsNullOrWhiteSpace(resp.Data))
                {
                    await MostrarAlertPorRespuestaAsync(resp);
                    return;
                }

                QrImageSource = Base64ToImageSource(resp.Data);
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

        private static ImageSource? Base64ToImageSource(string base64)
        {
            try
            {
                var clean = base64;
                var comma = base64.IndexOf(',');
                if (comma >= 0)
                    clean = base64[(comma + 1)..];

                var bytes = Convert.FromBase64String(clean);
                return ImageSource.FromStream(() => new MemoryStream(bytes));
            }
            catch
            {
                return null;
            }
        }

        private async Task OnCloseAsync()
        {
            await Shell.Current.GoToAsync(".."); // pop
        }
    }
}
