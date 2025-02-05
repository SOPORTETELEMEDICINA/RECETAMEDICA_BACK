using RMD.Models.PuntoVenta;
using RMD.Models.Recetas;
using System.Text;

namespace RMD.Extensions
{
    public static class RecetaPdfExtension
    {
        public static string GenerarHtmlReceta(Receta_PacienteRequest paciente, List<Receta_RecetaDetalleRequest> detalles, Receta_FormatoRequest formato)
        {
            if (formato == null || string.IsNullOrEmpty(formato.Formato))
            {
                throw new ArgumentException("El formato de la receta no está disponible.");
            }

            // Validaciones para EspecialidadMedico y CedulaMedico
            var especialidadMedico = string.IsNullOrEmpty(paciente.Especialidad) ? "Médico General" : paciente.Especialidad;
            var cedulaMedico = string.IsNullOrEmpty(paciente.CedulaEspecialidad) ? paciente.CedulaGeneral : paciente.CedulaEspecialidad;

            // Generar las filas de la tabla de medicamentos
            var medicamentosHtml = new StringBuilder();
            foreach (var detalle in detalles)
            {
                medicamentosHtml.Append($@"
                <tr>
                    <td>{detalle.MedicamentoNombre}</td>
                    <td>{detalle.CantidadDiaria}</td>
                    <td>{detalle.UnidadDispensacion}</td>
                    <td>{detalle.RutaAdministracion}</td>
                    <td>{detalle.Frecuencia}</td>
                    <td>{detalle.Duracion} {detalle.UnidadDuracion}</td>
                    <td class=""recommendations"">{detalle.Observaciones}</td>
                </tr>");
            }
            // Procesar diagnósticos
            var diagnosticosFormateados = string.Empty;

            if (!string.IsNullOrEmpty(paciente.Diagnosticos))
            {
                diagnosticosFormateados = string.Join(
                    "<br>",
                    paciente.Diagnosticos
                        .Split(',')
                        .Select(diagnostico =>
                        {
                            var parts = diagnostico.Split('-');
                            return parts.Length == 2
                                ? $"<strong>{parts[0]}</strong>&emsp;{parts[1]}"
                                : diagnostico;
                        })
                );
            }
            // Reemplazar los placeholders en la plantilla
            var htmlFinal = formato.Formato
                .Replace("{{LogoBase64}}", formato.Logo ?? string.Empty)
                .Replace("{{Fecha}}", paciente.Fecha != DateTime.MinValue ? paciente.Fecha.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy"))
                // Información del médico
                .Replace("{{NombreMedico}}", $"{paciente.NombresMedico} {paciente.PrimerApellidoMedico} {paciente.SegundoApellidoMedico}".Trim())
                .Replace("{{CedulaMedico}}", cedulaMedico)
                .Replace("{{EspecialidadMedico}}", especialidadMedico)
                .Replace("{{TelefonoMedico}}", paciente.Movil ?? string.Empty)
                .Replace("{{EmailMedico}}", paciente.Email ?? string.Empty)

                // Información del paciente
                .Replace("{{NombrePaciente}}", $"{paciente.NombresPaciente} {paciente.PrimerApellidoPaciente} {paciente.SegundoApellidoPaciente}".Trim())
                .Replace("{{SexoPaciente}}", paciente.Genero ?? string.Empty)
                .Replace("{{EdadPaciente}}", paciente.EdadPaciente.ToString() + " Años")
                .Replace("{{PacPeso}}", paciente.PacPeso.ToString("F2"))
                .Replace("{{PacTalla}}", paciente.PacTalla.ToString("F2"))
                .Replace("{{Diagnosticos}}", diagnosticosFormateados)

                // Firma del médico
                .Replace("{{FirmaBase64}}", paciente.Firma ?? string.Empty)

                // Detalles de los medicamentos
                .Replace("{{Medications}}", medicamentosHtml.ToString())

                // Información adicional
                .Replace("{{DireccionMedico}}", paciente.Domicilio ?? string.Empty)
                .Replace("{{Horario}}", paciente.Horario ?? "N/A");

            return htmlFinal;
        }
    }
}
