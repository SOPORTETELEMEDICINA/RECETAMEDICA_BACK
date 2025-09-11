using RMD.Data;
using RMD.Extensions;
using RMD.Interface.Receta;
using RMD.Shared.Models.Receta.Detalle.Interno;
using RMD.Shared.Models.Receta.Header.Internos;

namespace RMD.Service.Receta
{
    public class HelperRecetaServiceES : IHelperRecetaServiceES
    {
        private readonly RecetasDbContext _context;
        private readonly IHelperRecetaService _helperRecetaService;

        public HelperRecetaServiceES(
            RecetasDbContext context,
            IHelperRecetaService helperRecetaService)
        {
            _context = context;
            _helperRecetaService = helperRecetaService;

        }
        public async Task<List<DetalleDto>> PrepararPaquetesExactosESAsync(
            List<DetalleDto> detalles, List<PackageDetalleDto> paquetes)
        {
            // Paso 1: Filtrar detalles válidos (que tengan litros > 0)
            var detallesValidos = detalles
                .Where(d => d.TotalLitros.HasValue && d.TotalLitros > 0)
                .ToList();

            // Paso 2: Separar los de 30 días o menos
            var menoresA30Dias = detallesValidos
                .Where(d => d.DuracionEnDias > 0 && d.DuracionEnDias <= 30)
                .ToList();

            // Paso 3: Separar los de más de 30 y hasta 365 días
            var mayoresA30Dias = detallesValidos
                .Where(d => d.DuracionEnDias > 30 && d.DuracionEnDias <= 365)
                .ToList();

            // Paso 4: Dividir los de más de 30 días en bloques de 30 días
            var detallesSeparadosEnBloques = _helperRecetaService.SepararPorBloquesESAsync(mayoresA30Dias);

            // Paso 5: Si hay detalles separados en bloques, unirlos con los de ≤ 30 días
            var listaFinal = detallesSeparadosEnBloques.Any()
                ? _helperRecetaService.UnirDetallesParaCalculoES(detallesSeparadosEnBloques, menoresA30Dias)
                : menoresA30Dias;

            // Paso 6: Insertar en la tabla de Suertido Pendiente
            if (detallesSeparadosEnBloques.Any())
            {
                var detalleCronico = await _helperRecetaService.CalcularPaquetesExactosAsync(detallesSeparadosEnBloques, paquetes);
                await InsertarDetalleCronicoESAsync(detalleCronico);
            }

            //Paso 7: Retornamos la lista final de detalles preparados
            return listaFinal;
        }
        public string GenerarRecetaQR(DatosQRHeader datosQR)
        {
            var texto = $"{datosQR.IdReceta}|{datosQR.IdMedico}|{datosQR.IdGEMP}|{datosQR.IdSucursal}|{DateTime.Now:O}";
            var textoEncriptado = EncryptionHelper.Encrypt(texto);
            var liga = $"https://puntoventa.recetamedica.digital/receta?{textoEncriptado}";
            var qrBase64 = QRGenerator.GenerarQR(liga);
            return qrBase64;
        }
        private async Task InsertarDetalleCronicoESAsync(List<DetalleInHeader> detallesSeparadosEnBloques)
        {
            if (!detallesSeparadosEnBloques.Any())
                return;
            var primeros = detallesSeparadosEnBloques
                .GroupBy(b => new { b.MedicamentoId, b.MedicamentoType })
                .Select(g => g.OrderBy(x => x.PeriodoInicio).First().IdDetalleReceta)
                .ToHashSet();

            foreach (var bloque in detallesSeparadosEnBloques)
            {
                var cronico = new DetalleCronico
                {
                    Id = Guid.NewGuid(),
                    IdRecetaOrigen = bloque.IdReceta,
                    IdRecetaNueva = primeros.Contains(bloque.IdDetalleReceta) ? bloque.IdReceta : null,
                    MedicamentoId = bloque.MedicamentoId,
                    MedicamentoType = bloque.MedicamentoType,
                    UnidadDispensacionId = bloque.UnidadDispensacionId,
                    RutaAdministracionId = bloque.RutaAdministracionId,
                    CantidadDiaria = bloque.CantidadDiaria,
                    Indicacion = bloque.Indicacion,
                    IndicacionNombre = bloque.IndicacionNombre,
                    Frecuency = bloque.Frecuency,
                    IdFrecuencyType = bloque.IdFrecuencyType,
                    Observaciones = bloque.Observaciones,
                    Duracion = bloque.Duracion,
                    UnidadDuracion = bloque.UnidadDuracion,
                    PeriodoInicio = bloque.PeriodoInicio,
                    PeriodoTerminacion = bloque.PeriodoTerminacion,
                    IsNarcotic = bloque.IsNarcotic,
                    PsicoAnnexId = bloque.PsicoAnnexId
                };

                _context.DetalleCronico.Add(cronico);
            }

            await _context.SaveChangesAsync();
        }
        


    }
}
