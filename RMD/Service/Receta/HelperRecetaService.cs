using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Interface.Receta;
using RMD.Interface.Security;
using RMD.Interface.Vidal;
using RMD.Shared.Models.Receta.Detalle.Interno;
using RMD.Shared.Models.Receta.Header.Internos;
using RMD.Shared.Models.Receta.Header.Responses;
using RMD.Shared.Models.Vidal.Allergy;
using RMD.Shared.Models.Vidal.CIM10;
using RMD.Shared.Models.Vidal.Molecule;

namespace RMD.Service.Receta
{
    public class HelperRecetaService : IHelperRecetaService
    {
        private readonly RecetasDbContext _context;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly IDapperService _dapperService;
        private readonly ICIM10Service _cim10Service;
        private readonly IAllergyService _allergyService;
        private readonly IMoleculeService _moleculeService;

        public HelperRecetaService(
            RecetasDbContext context,
            ICatalogoNotificacionService catalogoNotificacionService,
            IDapperService dapperService,
            ICIM10Service cim10Service,
            IAllergyService allergyService,
            IMoleculeService moleculeService)
        {
            _context = context;
            _catalogoNotificacionService = catalogoNotificacionService;
            _dapperService = dapperService;
            _cim10Service = cim10Service;
            _allergyService = allergyService;
            _moleculeService = moleculeService;
        }

        public async Task<ResponseFromService<(List<DetalleDto> Detalles, List<PackageDetalleDto> Paquetes)>> GetDatosParaTimbradoAsync(Guid idReceta, Guid idUsuario)
        {
            try
            {
                var parameters = new { IdReceta = idReceta, IdUsuario = idUsuario };
               
                using var multi = await _dapperService.QueryMultipleAsync(
                    "[Receta].[GetDatosParaTimbrado]", parameters);

                var detalles = multi.Read<DetalleDto>().ToList();
                var paquetes = multi.Read<PackageDetalleDto>().ToList();

                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                return ResponseFromService<(List<DetalleDto>, List<PackageDetalleDto>)>.Success((detalles, paquetes), notificacion);
            }
            catch (Exception ex)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<(List<DetalleDto>, List<PackageDetalleDto>)>.Exeption(ex, notificacion);
            }
        }
        public Task<List<DetalleInHeader>> CalcularPaquetesExactosAsync(
            List<DetalleDto> detalles, List<PackageDetalleDto> paquetes)
        {
            var nuevosDetalles = new List<DetalleInHeader>();

            // Paso 1: Recorrer cada medicamento de la receta
            foreach (var detalle in detalles)
            {
                var litrosNecesarios = detalle.TotalLitros ?? 0;
                if (litrosNecesarios <= 0) continue; // Si no hay litros requeridos, salta

                // Paso 2: Filtrar paquetes compatibles por ProductId y que tengan contenido líquido
                var paquetesCompatibles = paquetes
                    .Where(p => p.ProductId == detalle.MedicamentoId && (p.TotalContenidoL ?? 0) > 0)
                    .OrderByDescending(p => p.TotalContenidoL ?? 0)
                    .ToList();
                var mejorCombinacion = new List<(PackageDetalleDto Paquete, int Cantidad)>();
                decimal menorExceso = decimal.MaxValue;

                // Paso 3: Buscar la mejor combinación de paquetes (menos exceso posible)
                for (int i = 0; i < paquetesCompatibles.Count; i++)
                {
                    var combinacionActual = new List<(PackageDetalleDto, int)>();
                    decimal total = 0;

                    for (int j = i; j < paquetesCompatibles.Count; j++)
                    {
                        var paquete = paquetesCompatibles[j];
                        int cantidad = 0;

                        // Paso 4: Sumar paquetes hasta alcanzar o pasar los litros requeridos
                        while (total < litrosNecesarios)
                        {
                            var litros = paquete.TotalContenidoL ?? 0;
                            if (litros <= 0) break;

                            total += litros;
                            cantidad++;

                            if (cantidad > 20) break; // Seguridad para evitar loop infinito
                        }


                        if (cantidad > 0)
                            combinacionActual.Add((paquete, cantidad));

                        if (total >= litrosNecesarios)
                            break;
                    }

                    // Paso 5: Evaluar si esta combinación tiene menos exceso que la anterior
                    var exceso = total - litrosNecesarios;
                    if (exceso >= 0 && exceso < menorExceso)
                    {
                        mejorCombinacion = combinacionActual;
                        menorExceso = exceso;

                        if (exceso == 0)
                            break; // No hay mejor combinación posible
                    }
                }

                // Paso 6: Registrar la combinación elegida como nuevos detalles de receta (tipo PACKAGE)
                foreach (var (paquete, cantidad) in mejorCombinacion)
                {
                    nuevosDetalles.Add(new DetalleInHeader
                    {
                        IdDetalleReceta = Guid.NewGuid(),
                        IdReceta = detalle.IdReceta,
                        MedicamentoId = paquete.IdPackage,
                        MedicamentoType = "PACKAGE",
                        CantidadDiaria = detalle.CantidadDiaria,
                        UnidadDispensacionId = detalle.UnidadDispensacionId,
                        RutaAdministracionId = detalle.RutaAdministracionId,
                        Indicacion = detalle.Indicacion,
                        Duracion = detalle.Duracion,
                        UnidadDuracion = detalle.UnidadDuracion,
                        PeriodoInicio = DateTime.Now.Date,
                        PeriodoTerminacion = detalle.PeriodoTerminacion ?? DateTime.Now.Date.AddDays(detalle.DuracionEnDias),
                        IndicacionNombre = detalle.IndicacionNombre,
                        Frecuency = detalle.Frecuency,
                        IdFrecuencyType = detalle.IdFrecuencyType,
                        Observaciones = detalle.Observaciones,
                        PackageQty = cantidad,
                        CantidadSurtida = 0,
                        IsNarcotic = detalle.IsNarcotic,
                        PsicoAnnexId = detalle.PsicoAnnexId
                    });
                }
            }

            // Paso 7: Retornar los nuevos detalles
            return Task.FromResult(nuevosDetalles);
        }
        public List<DetalleDto> SepararPorBloquesESAsync(List<DetalleDto> mayoresA30Dias)
        {
            // Paso 1: Inicializar lista que almacenará los nuevos detalles divididos por bloques
            List<DetalleDto> detallesSeparadosEnBloques = new List<DetalleDto>();

            // Paso 2: Recorrer cada detalle cuya duración es mayor a 30 días
            foreach (var detalle in mayoresA30Dias)
            {
                // Paso 2.1: Calcular el número total de bloques de 30 días que se necesitan
                var bloques = (int)Math.Ceiling(detalle.DuracionEnDias / 30m);
                var diasRestantes = detalle.DuracionEnDias;
                var litrosTotales = detalle.TotalLitros ?? 0;
                var litrosPorDia = detalle.DuracionEnDias > 0 ? litrosTotales / detalle.DuracionEnDias : 0;
                var fechaInicio = detalle.PeriodoInicio;

                // Paso 2.2: Generar cada bloque de 30 días (o menos si es el último)
                for (int i = 0; i < bloques; i++)
                {
                    // Paso 2.2.1: Determinar la duración exacta de este bloque (30 o lo que reste)
                    var diasEnEsteBloque = Math.Min(30, diasRestantes);

                    // Paso 2.2.2: Calcular los litros necesarios para este bloque
                    var litrosEnEsteBloque = litrosPorDia * diasEnEsteBloque;

                    // Paso 2.2.3: Construir un nuevo detalle de receta para el bloque actual
                    detallesSeparadosEnBloques.Add(new DetalleDto
                    {
                        IdDetalleReceta = Guid.NewGuid(),
                        IdReceta = detalle.IdReceta,
                        MedicamentoId = detalle.MedicamentoId,
                        MedicamentoType = detalle.MedicamentoType,
                        CantidadDiaria = detalle.CantidadDiaria,
                        UnidadDispensacionId = detalle.UnidadDispensacionId,
                        RutaAdministracionId = detalle.RutaAdministracionId,
                        Indicacion = detalle.Indicacion,
                        Duracion = diasEnEsteBloque,
                        UnidadDuracion = detalle.UnidadDuracion,

                        // Fecha de inicio y fin de este bloque
                        PeriodoInicio = fechaInicio,
                        PeriodoTerminacion = fechaInicio.AddDays(diasEnEsteBloque - 1),

                        IndicacionNombre = detalle.IndicacionNombre,
                        Frecuency = detalle.Frecuency,
                        IdFrecuencyType = detalle.IdFrecuencyType,
                        Observaciones = detalle.Observaciones,
                        DosisPorDia = detalle.DosisPorDia,
                        TotalDosis = detalle.DosisPorDia * diasEnEsteBloque,
                        TotalLitros = litrosEnEsteBloque,
                        Unidad = detalle.Unidad,
                        Conversion = detalle.Conversion,
                        Denominator = detalle.Denominator,
                        Numerator = detalle.Numerator,
                        Summary = detalle.Summary,
                        Name = detalle.Name,
                        IdProduct = detalle.IdProduct,
                        IdVmp = detalle.IdVmp,
                        GalenicForm = detalle.GalenicForm,
                        GalenicName = detalle.GalenicName,
                        AtcCode = detalle.AtcCode,
                        AtcCodeMJ = detalle.AtcCodeMJ,
                        Pictogram = detalle.Pictogram,
                        IsNarcotic = detalle.IsNarcotic,
                        PsicoAnnexId = detalle.PsicoAnnexId,
                        Monodrug = detalle.Monodrug,
                        Surtido = false,
                        FechaSurtido = null
                    });

                    // Paso 2.2.4: Actualizar días restantes y avanzar la fecha de inicio al siguiente bloque
                    diasRestantes -= diasEnEsteBloque;
                    fechaInicio = fechaInicio.AddDays(diasEnEsteBloque);
                }
            }

            // Paso 3: Retornar la lista completa de detalles divididos en bloques
            return detallesSeparadosEnBloques;
        }
        public List<DetalleDto> UnirDetallesParaCalculoES(
            List<DetalleDto> bloques, List<DetalleDto> menoresA30Dias)
        {
            // Paso 5.1: Agrupar los bloques por MedicamentoId y MedicamentoType
            var primerosBloques = bloques
                .GroupBy(b => new { b.MedicamentoId, b.MedicamentoType })
                .Select(g => g.OrderBy(x => x.PeriodoInicio).First())
                .ToList();

            // Paso 5.2: Unir los primeros bloques con los tratamientos de ≤ 30 días
            return primerosBloques.Concat(menoresA30Dias).ToList();
        }
        public async Task<Guid> GetMedicoIdOrThrowAsync(Guid idUsuarioClaim)
        {
            try
            {
                var idMedico = await _context.Medicos
                    .Where(m => m.IdUsuario == idUsuarioClaim)
                    .Select(m => m.IdMedico)
                    .FirstOrDefaultAsync();

                if (idMedico == Guid.Empty)
                    throw new UnauthorizedAccessException();

                return idMedico;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR en GetMedicoIdOrThrowAsync: " + ex);
                throw;
            }
        }
        public async Task<ResponseFromService<bool>> ActualizarPacienteDesdeReceta(
            Guid idPaciente, string alergias, string molecules, string patologias, 
            DateTime fechaUltimaModificacion)
        {
            try
            {
                var pacienteExistente = await _context.Pacientes
                    .Where(p => p.IdPaciente == idPaciente)
                    .FirstOrDefaultAsync();
                if (pacienteExistente != null)
                {
                    pacienteExistente.Alergias = string.IsNullOrEmpty(alergias) ? "" : alergias;
                    pacienteExistente.Molecules = string.IsNullOrEmpty(molecules) ? "" : molecules;
                    pacienteExistente.Patologias = string.IsNullOrEmpty(patologias) ? "" : patologias;
                    pacienteExistente.FechaUltimaModificacion = fechaUltimaModificacion;
                    await _context.SaveChangesAsync();
                }
                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                return ResponseFromService<bool>.Success(true, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, error);
            }
        }
        public HeaderCounltResponse TransformRecetaSQLToRecetaGet(HeaderBySQL receta)
        {
            return new HeaderCounltResponse
            {
                IdReceta = receta.IdReceta,
                Folio = receta.Folio,
                IdMedico = receta.IdMedico,
                NombresMedico = receta.NombresMedico,
                PrimerApellidoMedico = receta.PrimerApellidoMedico,
                SegundoApellidoMedico = receta.SegundoApellidoMedico,
                Universidad = receta.Universidad,
                CedulaGeneral = receta.CedulaGeneral,
                Especialidad = receta.Especialidad,
                CedulaEspecialidad = receta.CedulaEspecialidad,
                IdPaciente = receta.IdPaciente,
                NombresPaciente = receta.NombresPaciente,
                PrimerApellidoPaciente = receta.PrimerApellidoPaciente,
                SegundoApellidoPaciente = receta.SegundoApellidoPaciente,
                IdTipoIdentificacion = receta.IdTipoIdentificacion,
                TipoIdentificacion = receta.TipoIdentificacion,
                NumeroIdentificacion = receta.NumeroIdentificacion,
                FechaNacimientoPaciente = receta.FechaNacimientoPaciente,
                PacPeso = receta.PacPeso,
                PacTalla = receta.PacTalla,
                PacEmbarazo = receta.PacEmbarazo,
                PacSemAmenorrea = receta.PacSemAmenorrea,
                PacLactancia = receta.PacLactancia,
                PacCreatinina = receta.PacCreatinina,
                Alergias = string.IsNullOrEmpty(receta.Alergias)
                    ? new List<RequestSearchAllergy>()
                    : _allergyService.ParseAllergies(receta.Alergias),
                Molecules = string.IsNullOrEmpty(receta.Molecules)
                    ? new List<RequestSearchMolecules>()
                    : _moleculeService.ParseMolecules(receta.Molecules),
                Patologias = string.IsNullOrEmpty(receta.Patologias)
                    ? new List<RequestSearchCIM10>()
                    : _cim10Service.ParseCIM10(receta.Patologias),
                IdSucursal = receta.IdSucursal,
                Sucursal = receta.Sucursal,
                IdGEMP = receta.IdGEMP,
                GrupoEmpresarial = receta.GrupoEmpresarial,
                FechaCreacion = receta.FechaCreacion,
                FechaUltimaModificacion = receta.FechaUltimaModificacion,
                Estatus = receta.Estatus,
                Descripcion = receta.Descripcion,
                Timbrada = receta.Timbrada
            };
        }
        public Header_UpdatePacienteParsedRequest ParsePacienteToParsed(Header_UpdatePacienteRequest receta)
        {
            return new Header_UpdatePacienteParsedRequest
            {
                IdReceta = receta.IdReceta,
                IdMedico = receta.IdMedico,
                IdPaciente = receta.IdPaciente,
                NombresPaciente = receta.NombresPaciente,
                PrimerApellidoPaciente = receta.PrimerApellidoPaciente,
                SegundoApellidoPaciente = receta.SegundoApellidoPaciente,
                IdTipoIdentificacion = receta.IdTipoIdentificacion,
                TipoIdentificacion = receta.TipoIdentificacion,
                NumeroIdentificacion = receta.NumeroIdentificacion,
                EdadPaciente = receta.EdadPaciente,
                PacPeso = receta.PacPeso,
                Genero = receta.Genero,
                PacTalla = receta.PacTalla,
                PacEmbarazo = receta.PacEmbarazo,
                PacSemAmenorrea = receta.PacSemAmenorrea,
                PacLactancia = receta.PacLactancia,
                PacCreatinina = receta.PacCreatinina,
                Alergias = string.IsNullOrEmpty(receta.Alergias) ? new() : _allergyService.ParseAllergies(receta.Alergias),
                Molecules = string.IsNullOrEmpty(receta.Molecules) ? new() : _moleculeService.ParseMolecules(receta.Molecules),
                Patologias = string.IsNullOrEmpty(receta.Patologias) ? new() : _cim10Service.ParseCIM10(receta.Patologias),
                IdGEMP = receta.IdGEMP,
                NombreGrupoEmpresarial = receta.NombreGrupoEmpresarial,
                IdSucursal = receta.IdSucursal,
                Sucursal = receta.Sucursal,
                IdAsentamiento = receta.IdAsentamiento,
                NombreAsentamiento = receta.NombreAsentamiento,
                IdTipoAsentamiento = receta.IdTipoAsentamiento,
                TipoAsentamiento = receta.TipoAsentamiento,
                IdCP = receta.IdCP,
                CodigoPostal = receta.CodigoPostal,
                IdEntidad = receta.IdEntidad,
                IdMunicipio = receta.IdMunicipio,
                NoMunicipio = receta.NoMunicipio,
                Municipio = receta.Municipio,
                IdCiudad = receta.IdCiudad,
                Ciudad = receta.Ciudad,
                Estado = receta.Estado,
                Abreviatura = receta.Abreviatura,
                Fecha = receta.Fecha,
                EstatusReceta = receta.EstatusReceta
            };
        }
        public string ConvertirListaAString(List<int> lista)
        {
            return string.Join(",", lista);
        }
        public string ObtenerValorDesdeToken(string token, string claimType)
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim?.Value ?? string.Empty;
        }

    }
}
