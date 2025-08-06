using RMD.Shared.Models.Receta.Header.Internos;
using System.Text;

namespace RMD.Extensions
{
    public static class RecetaPdfExtension
    {
        public static string GenerarHtmlReceta(Header_PacienteRequest paciente, List<DetalleInternoRequest> detalles, Receta_FormatoHTMLRequest formato, string qr)
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

            // 2. ¿Mostrar columna 'Cantidad'?
            bool mostrarCantidad = detalles.Any(d => d.MedicamentoType == "PACKAGE");

            // 3. Encabezado de la tabla de medicamentos
            var encabezadoHtml = new StringBuilder();
            encabezadoHtml.Append("<thead><tr>");
            if (mostrarCantidad)
                encabezadoHtml.Append("<th>Cantidad</th>");
            encabezadoHtml.Append(@"<th>Medicamento</th>
                <th>Dosis</th>
                <th>Unidad</th>
                <th>Vía</th>
                <th>Frecuencia</th>
                <th>Periodo</th>
                <th>Recomendaciones</th>");
            encabezadoHtml.Append("</tr></thead>");

            // Generar las filas de la tabla de medicamentos
            var medicamentosHtml = new StringBuilder();
            foreach (var detalle in detalles)
            {
                var cantidadHtml = detalle.MedicamentoType == "PACKAGE"
                   ? $"<td>{detalle.Cantidad}</td>"
                   : "";
                medicamentosHtml.Append($@"
                <tr>
                    {cantidadHtml}
                    <td>{detalle.MedicamentoNombre}</td>
                    <td>{detalle.CantidadDiaria}</td>
                    <td>{detalle.UnidadDispensacion}</td>
                    <td>{detalle.RutaAdministracion}</td>
                    <td>CADA {detalle.Frecuency} {detalle.FrecuencyType}</td>
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
                .Replace("{{LogoBase64}}", formato.Logo)
                .Replace("{{Fecha}}", paciente.Fecha != DateTime.MinValue ? paciente.Fecha.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy"))

             // Médico
                .Replace("{{NombreMedico}}", $"{paciente.NombresMedico} {paciente.PrimerApellidoMedico} {paciente.SegundoApellidoMedico}".Trim())
                .Replace("{{CedulaGeneral}}", paciente.CedulaGeneral)
                .Replace("{{Universidad}}", paciente.Universidad)
                .Replace("{{EspecialidadMedico}}", paciente.Especialidad)
                .Replace("{{CedulaEspecialidadMedico}}", paciente.CedulaEspecialidad)
                .Replace("{{EspecialidadHtml}}", especialidadHtml.ToString())
                .Replace("{{TelefonoMedico}}", paciente.Movil)
                .Replace("{{EmailMedico}}", paciente.Email)

                // Paciente
                .Replace("{{NombrePaciente}}", $"{paciente.NombresPaciente} {paciente.PrimerApellidoPaciente} {paciente.SegundoApellidoPaciente}".Trim())
                .Replace("{{SexoPaciente}}", paciente.Genero)
                .Replace("{{EdadPaciente}}", paciente.EdadPaciente.ToString() + " Años")
                .Replace("{{PacPeso}}", paciente.PacPeso.ToString("F2"))
                .Replace("{{PacTalla}}", paciente.PacTalla.ToString("F2"))
                .Replace("{{TipoIdentificacion}}", paciente.TipoIdentificacion)
                .Replace("{{NumeroIdentificacion}}", paciente.NumeroIdentificacion)
                .Replace("{{Diagnosticos}}", diagnosticosFormateados)

                // Firma
                .Replace("{{FirmaBase64}}", paciente.Firma)

                // Detalles receta
                .Replace("{{EncabezadoMedications}}", encabezadoHtml.ToString())  // ⬅️ ESTA ES LA QUE TE FALTA
                .Replace("{{Medications}}", medicamentosHtml.ToString())

                // Extras
                .Replace("{{DireccionMedico}}", paciente.Domicilio)
                .Replace("{{Horario}}", paciente.Horario)
                .Replace("{{Folio}}", paciente.Folio)
                .Replace("{{QRData}}", qr);


            return htmlFinal;
        }
    }
}
