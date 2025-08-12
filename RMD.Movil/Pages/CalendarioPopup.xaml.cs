using System.Globalization;

namespace RMD.Movil.Pages
{
    public partial class CalendarioPopup : ContentView
    {
        public event Action<DateTime>? FechaSeleccionada;

        private DateTime mesActual;

        public CalendarioPopup()
        {
            InitializeComponent();
            mesActual = DateTime.Today;
            DibujarCalendario();
        }

        private void DibujarCalendario()
        {
            DiasGrid.Children.Clear();

            // Título mes y año
            LblMesAnio.Text = mesActual
                .ToString("MMMM yyyy", new CultureInfo("es-ES"))
                .ToUpper();

            // Encabezado días (domingo primero)
            string[] diasSemana = { "D", "L", "M", "M", "J", "V", "S" };
            for (int i = 0; i < 7; i++)
            {
                var lbl = new Label
                {
                    Text = diasSemana[i],
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontFamily = "Roboto",
                    FontSize = 14,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#6f61ee")
                };
                DiasGrid.Children.Add(lbl);
                Grid.SetRow(lbl, 0);
                Grid.SetColumn(lbl, i);
            }

            // Calcular primer día del mes y offset
            var primerDiaMes = new DateTime(mesActual.Year, mesActual.Month, 1);
            int offset = (int)primerDiaMes.DayOfWeek; // Domingo = 0
            int diasMes = DateTime.DaysInMonth(mesActual.Year, mesActual.Month);

            int fila = 1;
            int columna = offset;

            // Pintar días
            for (int dia = 1; dia <= diasMes; dia++)
            {
                var fecha = new DateTime(mesActual.Year, mesActual.Month, dia);

                var btn = new Button
                {
                    Text = dia.ToString("00"),
                    BackgroundColor = Colors.Transparent,
                    TextColor = Colors.Black,
                    FontFamily = "Roboto",
                    CornerRadius = 20,
                    FontSize = 9,
                    WidthRequest = 40,
                    HeightRequest = 40,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    CommandParameter = fecha
                };

                // Resaltar día actual
                if (fecha.Date == DateTime.Today.Date)
                {
                    btn.BackgroundColor = Color.FromArgb("#6f61ee");
                    btn.TextColor = Colors.White;
                }

                btn.Clicked += (s, e) =>
                {
                    if (s is Button b && b.CommandParameter is DateTime f)
                    {
                        FechaSeleccionada?.Invoke(f);
                        IsVisible = false;
                    }
                };

                DiasGrid.Children.Add(btn);
                Grid.SetRow(btn, fila);
                Grid.SetColumn(btn, columna);

                columna++;
                if (columna > 6)
                {
                    columna = 0;
                    fila++;
                }
            }
        }

        // Botones de navegación
        private void BtnAnterior_Clicked(object sender, EventArgs e)
        {
            mesActual = mesActual.AddMonths(-1);
            DibujarCalendario();
        }

        private void BtnSiguiente_Clicked(object sender, EventArgs e)
        {
            mesActual = mesActual.AddMonths(1);
            DibujarCalendario();
        }

        private void BtnCancelar_Clicked(object sender, EventArgs e)
        {
            IsVisible = false;
        }
    }
}
