using System.Collections.ObjectModel;

namespace RMD.Movil.Views
{
    public partial class RecetasPage : ContentPage
    {
        public ObservableCollection<RecetaModel> Recetas { get; set; }

        public RecetasPage()
        {
            InitializeComponent();

            Recetas = new ObservableCollection<RecetaModel>
            {
                new RecetaModel
                {
                    Folio = "CEL-250304-002-001",
                    Doctor = "DRA. BRENDA ESTRADA\nMEDICINA DEL DOLOR",
                    Fecha = "2025–03–04",
                    Estado = "Surtida",
                    Color = "#21D1BE"
                },
                new RecetaModel
                {
                    Folio = "CEL-250506-001-001",
                    Doctor = "DR. MORENO ESCOBAR ERNESTO\nMED. GENERAL",
                    Fecha = "01/07/2025",
                    Estado = "Surtida Parcial",
                    Color = "#D2BBF2"
                }
            };

            RecetasCollection.ItemsSource = Recetas;
        }

        private void btnActivas_Clicked(object sender, EventArgs e)
        {
            // Aquí filtras por activas
        }

        private void btnVencidas_Clicked(object sender, EventArgs e)
        {
            // Aquí filtras por vencidas
        }
    }

    public class RecetaModel
    {
        public string Folio { get; set; }
        public string Doctor { get; set; }
        public string Fecha { get; set; }
        public string Estado { get; set; }
        public string Color { get; set; }
    }
}