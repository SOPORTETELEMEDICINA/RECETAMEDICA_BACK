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

            // Generar dinámicamente las etiquetas para Especialidad y Cédula Especialidad
            var especialidadHtml = new StringBuilder();
            if (!string.IsNullOrEmpty(paciente.Especialidad) && !string.IsNullOrEmpty(paciente.CedulaEspecialidad))
            {
                especialidadHtml.Append($"<p>Especialidad: {paciente.Especialidad}</p>");
                especialidadHtml.Append($"<p>Cédula Especialidad: {paciente.CedulaEspecialidad}</p>");
            }


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

            // Agregar marca de agua si el estatus de la receta es diferente a "Activo" o "Surtido Parcialmente"
            var watermarkHtml = string.Empty;
            if (!string.Equals(paciente.EstatusReceta, "Activo", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(paciente.EstatusReceta, "Surtido Parcialmente", StringComparison.OrdinalIgnoreCase))
            {
                watermarkHtml = $"<div class='watermark'>{paciente.EstatusReceta}</div>";
            }

            // Reemplazar los placeholders en la plantilla
            var htmlFinal = formato.Formato
                .Replace("{{Watermark}}", watermarkHtml)
                .Replace("{{LogoBase64}}", formato.Logo ?? string.Empty)
                .Replace("{{Fecha}}", paciente.Fecha != DateTime.MinValue ? paciente.Fecha.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy"))

                // Información del médico
                .Replace("{{NombreMedico}}", $"{paciente.NombresMedico} {paciente.PrimerApellidoMedico} {paciente.SegundoApellidoMedico}".Trim())
                .Replace("{{CedulaGeneral}}", paciente.CedulaGeneral)
                .Replace("{{Universidad}}", paciente.Universidad)
                 .Replace("{{EspecialidadMedico}}", paciente.Especialidad ?? string.Empty)
                .Replace("{{CedulaEspecialidadMedico}}", paciente.CedulaEspecialidad ?? string.Empty)
                .Replace("{{EspecialidadHtml}}", especialidadHtml.ToString()) // ✅ Convertir a string

                .Replace("{{TelefonoMedico}}", paciente.Movil ?? string.Empty)
                .Replace("{{EmailMedico}}", paciente.Email ?? string.Empty)

                // Información del paciente
                .Replace("{{NombrePaciente}}", $"{paciente.NombresPaciente} {paciente.PrimerApellidoPaciente} {paciente.SegundoApellidoPaciente}".Trim())
                .Replace("{{SexoPaciente}}", paciente.Genero ?? string.Empty)
                .Replace("{{EdadPaciente}}", paciente.EdadPaciente.ToString() + " Años")
                .Replace("{{PacPeso}}", paciente.PacPeso.ToString("F2"))
                .Replace("{{PacTalla}}", paciente.PacTalla.ToString("F2"))

                // Nuevo: Identificación del paciente
                .Replace("{{TipoIdentificacion}}", paciente.TipoIdentificacion ?? "N/A")
                .Replace("{{NumeroIdentificacion}}", paciente.NumeroIdentificacion ?? "N/A")

                // Diagnóstico
                .Replace("{{Diagnosticos}}", diagnosticosFormateados)

                // Firma del médico
                .Replace("{{FirmaBase64}}", paciente.Firma ?? string.Empty)

                // Detalles de los medicamentos
                .Replace("{{Medications}}", medicamentosHtml.ToString())

                // Información adicional
                .Replace("{{DireccionMedico}}", paciente.Domicilio ?? string.Empty)
                .Replace("{{Horario}}", paciente.Horario ?? "N/A")

                // Nuevo: Folio y Código QR
                .Replace("{{Folio}}", paciente.Folio ?? "N/A")
                .Replace("{{QRData}}", paciente.QRData ?? string.Empty);

            return htmlFinal;
        }
    }
}
