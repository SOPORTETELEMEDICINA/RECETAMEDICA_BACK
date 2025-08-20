namespace RMD.OneSignal.Models;

public sealed class ScheduleOptions
{
    // Envío inmediato si es null
    public DateTimeOffset? SendAfter { get; set; }

    // Para “paquete del día”: varias horas en el día
    public IEnumerable<DateTimeOffset>? MultipleTimes { get; set; }

    // Timezone OneSignal
    public string? TimeZone { get; set; }
}