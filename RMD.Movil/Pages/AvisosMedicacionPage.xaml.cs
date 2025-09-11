using System;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using RMD.Movil.Core.Service.Interfaces;
using RMD.Movil.PageModels;
using RMD.Shared.Models.Receta.AlertaToma.Response;

namespace RMD.Movil.Pages
{
    public partial class AvisosMedicacionPage : ContentPage
    {
        private readonly AvisosMedicacionPageModel _vm;

        public AvisosMedicacionPage()
        {
            InitializeComponent();

            var alertasService = App.Services.GetRequiredService<IAlertasProgramadasControllerService>();
            var alertaTomaService = App.Services.GetRequiredService<IAlertaTomaControllerService>();

            _vm = new AvisosMedicacionPageModel(alertasService, alertaTomaService);
            BindingContext = _vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (_vm.LoadCommand.CanExecute(null))
                await _vm.LoadCommand.ExecuteAsync(null);
        }

        // Tap en píldoras del calendario
        private async void OnDayTapped(object sender, EventArgs e)
        {
            if (sender is not BindableObject b || b.BindingContext is not DiaSemanaItem item)
                return;

            if (_vm.SeleccionarFechaCommand.CanExecute(item.Date))
                await _vm.SeleccionarFechaCommand.ExecuteAsync(item.Date);
        }

        // Plan B: tap del ícono si el Command del DataTemplate no se resolviera
        private async void OnIconTapped(object sender, EventArgs e)
        {
            if (sender is not BindableObject b || b.BindingContext is not AlertaProgramadaResponse item)
                return;

            if (_vm.ToggleAlertaCommand.CanExecute(item))
                await _vm.ToggleAlertaCommand.ExecuteAsync(item);
        }
    }

    public class GuidToColorConverter : IValueConverter
    {
        private static readonly Color[] Palette = new[]
        {
            Color.FromArgb("#073B60"),
            Color.FromArgb("#00A5B8"),
            Color.FromArgb("#2563EB"),
            Color.FromArgb("#22C55E"),
            Color.FromArgb("#84CC16"),
            Color.FromArgb("#F59E0B"),
            Color.FromArgb("#ED8A60"),
            Color.FromArgb("#EF4444"),
            Color.FromArgb("#EC4899"),
            Color.FromArgb("#9D69CA"),
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Guid g;
            if (value is Guid gid) g = gid;
            else if (value is string s && Guid.TryParse(s, out var parsed)) g = parsed;
            else return Palette[0];

            var idx = Math.Abs(g.GetHashCode()) % Palette.Length;
            return Palette[idx];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
