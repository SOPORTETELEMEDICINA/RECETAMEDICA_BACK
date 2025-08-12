using RMD.Movil.PageModels.Controls;

namespace RMD.Movil.Pages.Controls
{
    public partial class BottomNavBar : ContentView
    {
        private readonly BottomNavBarModel _vm = new();

        public BottomNavBar()
        {
            InitializeComponent();
            BindingContext = _vm;
            _vm.Refresh(); // opcional

            // En ContentView usa Unloaded para liberar
            Unloaded += (_, __) => _vm.Dispose();
        }
    }
}