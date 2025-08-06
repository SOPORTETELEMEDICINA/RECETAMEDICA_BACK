using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Interface.Security;
using RMD.Shared.Models.Receta.Header.Internos;

namespace RMD.Service.ServiciosInternos
{
    public class CronRecetaService: ICronRecetaService
    {
        private readonly RecetasDbContext _context;
        private readonly int _diasLimite = 3; // Puedes ajustar este valor según sea necesario
        private readonly FolioHelper _folioHelper;

        public CronRecetaService(
            RecetasDbContext context,
            FolioHelper folioHelper)
        {
            _context = context;
            _folioHelper = folioHelper;
        }
        public async Task<bool> ObtenerDetalleCronicoESAsync()
        {
            var fechaLimite = DateTime.Now.Date.AddDays(_diasLimite);

            var _detalleCronicoList = await _context.DetalleCronico
                .Where(dc => dc.PeriodoInicio > fechaLimite)
                .ToListAsync();

            if (!_detalleCronicoList.Any())
                return true;

            var recetasOrigen = _detalleCronicoList
                .Select(x => x.IdRecetaOrigen)
                .Where(_ => true)
                .Distinct()
                .ToList();
            if (!recetasOrigen.Any())
                return true;

            var recetasHeader = await _context.ConsultaRecetas
                .Where(r => recetasOrigen.Contains(r.IdReceta))
                .ToListAsync();

            if (!recetasHeader.Any())
                return true;

            foreach (var idReceta in recetasOrigen)
            {
                var detallesAsociados = _detalleCronicoList
                    .Where(d => d.IdRecetaOrigen == idReceta)
                    .ToList();

                var header = recetasHeader
                    .FirstOrDefault(r => r.IdReceta == idReceta);

                if (header == null || !detallesAsociados.Any())
                    continue;

                var narcoticos = detallesAsociados
                    .Where(d => d.IsNarcotic == true)
                    .ToList();

                var psicotropos = detallesAsociados
                    .Where(d => !string.IsNullOrWhiteSpace(d.PsicoAnnexId))
                    .ToList();

                var normales = detallesAsociados
                    .Where(d => d.IsNarcotic != true && string.IsNullOrWhiteSpace(d.PsicoAnnexId))
                    .ToList();
                if (normales.Any())
                {
                    await ProcesarDetallesPorTipo(header, normales);
                }
                if (narcoticos.Any())
                {
                    foreach (var narcotico in narcoticos)
                    {
                        await ProcesarDetallesPorTipo(header, new List<DetalleCronico> { narcotico });
                    }
                }
                if (psicotropos.Any())
                {
                    foreach (var psicotropo in psicotropos)
                    {
                        await ProcesarDetallesPorTipo(header, new List<DetalleCronico> { psicotropo });
                    }
                }
                // Aquí puedes continuar con la lógica de creación de nuevas recetas
                // usando `header` y `detallesAsociados`
            }
            return true;

        }

        private async Task ProcesarDetallesPorTipo(Header header, List<DetalleCronico> detalles)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var nuevoIdReceta = Guid.NewGuid();
                var fechaCreacion = DateTime.Now;

                foreach (var detalle in detalles)
                    detalle.IdRecetaNueva = nuevoIdReceta;

                var folio = _folioHelper.GenerarFolioAsync(header.IdGEMP, fechaCreacion, header.IdSucursal).ToString();

                var nuevoHeader = new Header
                {
                    IdReceta = nuevoIdReceta,
                    IdMedico = header.IdMedico,
                    IdPaciente = header.IdPaciente,
                    PacPeso = header.PacPeso,
                    PacTalla = header.PacTalla,
                    PacEmbarazo = header.PacEmbarazo,
                    PacSemAmenorrea = header.PacSemAmenorrea,
                    PacLactancia = header.PacLactancia,
                    PacCreatinina = header.PacCreatinina,
                    Alergias = header.Alergias,
                    Molecules = header.Molecules,
                    Patologias = header.Patologias,
                    IdSucursal = header.IdSucursal,
                    IdGEMP = header.IdGEMP,
                    FechaCreacion = fechaCreacion,
                    FechaUltimaModificacion = fechaCreacion,
                    Timbrada = true,
                    Folio = folio
                };

                _context.ConsultaRecetas.Add(nuevoHeader);

                foreach (var detalle in detalles)
                {
                    var nuevoDetalle = new DetalleInHeader
                    {
                        IdDetalleReceta = Guid.NewGuid(),
                        IdReceta = nuevoIdReceta,
                        MedicamentoId = detalle.MedicamentoId,
                        MedicamentoType = detalle.MedicamentoType,
                        UnidadDispensacionId = detalle.UnidadDispensacionId,
                        RutaAdministracionId = detalle.RutaAdministracionId,
                        CantidadDiaria = detalle.CantidadDiaria,
                        Indicacion = detalle.Indicacion ?? "",
                        IndicacionNombre = detalle.IndicacionNombre ?? "",
                        Frecuency = detalle.Frecuency,
                        IdFrecuencyType = detalle.IdFrecuencyType,
                        Observaciones = detalle.Observaciones ?? "",
                        Duracion = detalle.Duracion,
                        UnidadDuracion = detalle.UnidadDuracion,
                        PeriodoInicio = detalle.PeriodoInicio,
                        PeriodoTerminacion = detalle.PeriodoTerminacion
                    };

                    _context.DetalleRecetas.Add(nuevoDetalle);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
