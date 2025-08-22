using Microsoft.Maui.Controls.Shapes;
using System.Globalization;

namespace RMD.Movil.Pages
{
    public partial class CalendarioPopup : ContentView
    {
        public CalendarioPopup()
        {
            InitializeComponent();
            DibujarCalendario();
        }

        private void DibujarCalendario()
        {
            DiasGrid.Children.Clear();
            DiasGrid.RowDefinitions.Clear();
            DiasGrid.ColumnDefinitions.Clear();

            // mostrar HOY + 6 días
            DateTime hoy = DateTime.Today;
            var semana = Enumerable.Range(0, 7).Select(offset => hoy.AddDays(offset)).ToList();

            // título mes
            LblMesAnio.Text = hoy.ToString("MMMM yyyy", new CultureInfo("es-ES")).ToUpper();

            // layout: 7 columnas, 2 filas (nombre + número)
            for (int i = 0; i < 7; i++)
                DiasGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            DiasGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // fila nombre
            DiasGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // fila número

            for (int i = 0; i < semana.Count; i++)
            {
                DateTime fecha = semana[i];

                // nombre del día (LUN, MAR, ...)
                var lblNombre = new Label
                {
                    Text = fecha.ToString("ddd", new CultureInfo("es-ES")).ToUpper(),
                    FontSize = 12,
                    TextColor = fecha.Date == hoy.Date ? Colors.White : Colors.Black,
                    FontAttributes = fecha.Date == hoy.Date ? FontAttributes.Bold : FontAttributes.None,
                    HorizontalOptions = LayoutOptions.Center
                };

                // contenedor tipo píldora
                var border = new Border
                {
                    StrokeShape = new RoundRectangle { CornerRadius = 20 },
                    StrokeThickness = 0,
                    Padding = new Thickness(12, 8),
                    BackgroundColor = fecha.Date == hoy.Date ? Color.FromArgb("#6f61ee") : Colors.Transparent,
                    HorizontalOptions = LayoutOptions.Center
                };

                var lblDia = new Label
                {
                    Text = fecha.Day.ToString(),
                    FontSize = 14,
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = fecha.Date == hoy.Date ? Colors.White : Colors.Black,
                    FontAttributes = fecha.Date == hoy.Date ? FontAttributes.Bold : FontAttributes.None
                };


                border.Content = lblDia;

                DiasGrid.Children.Add(lblNombre);
                Grid.SetRow(lblNombre, 0);
                Grid.SetColumn(lblNombre, i);

                DiasGrid.Children.Add(border);
                Grid.SetRow(border, 1);
                Grid.SetColumn(border, i);
            }
        }
    }
}
