using CommunityToolkit.Mvvm.ComponentModel;

namespace RMD.Movil.Models;

public partial class EventosSaludModel : ObservableObject
{
    public Guid IdEventoSalud { get; set; }
    public Guid IdPaciente { get; set; }
    public DateTime Fecha { get; set; }
    public int EventoDeSalud { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string NombreEvento { get; set; } = string.Empty;

    private Color _colorEvento = Colors.Gray;

    public Color ColorEvento
    {
        get => _colorEvento;
        set => SetProperty(ref _colorEvento, value);
    }

    public void CalcularColor()
    {
        ColorEvento = EventoDeSalud switch
        {
            1 => Color.FromArgb("#e5ddf3"),
            2 => Color.FromArgb("#39d1bf"),
            3 => Color.FromArgb("#f4d4c6"),
            4 => Color.FromArgb("#b39ddb"),
            5 => Color.FromArgb("#90caf9"),
            6 => Color.FromArgb("#ffb74d"),
            7 => Color.FromArgb("#ef5350"),
            8 => Color.FromArgb("#81c784"),
            9 => Color.FromArgb("#ce93d8"),
            10 => Color.FromArgb("#4dd0e1"),
            11 => Color.FromArgb("#7986cb"),
            12 => Color.FromArgb("#f06292"),
            13 => Color.FromArgb("#ff8a65"),
            14 => Color.FromArgb("#d4e157"),
            15 => Color.FromArgb("#aed581"),
            16 => Color.FromArgb("#ba68c8"),
            17 => Color.FromArgb("#f48fb1"),
            18 => Color.FromArgb("#ef9a9a"),
            19 => Color.FromArgb("#ff7043"),
            20 => Color.FromArgb("#cfd8dc"),
            _ => Colors.Gray
        };
    }
}