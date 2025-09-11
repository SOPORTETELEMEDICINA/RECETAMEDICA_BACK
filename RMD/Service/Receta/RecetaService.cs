using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RMD.Data;
using RMD.Extensions;
using RMD.Interface.Receta;
using RMD.Interface.Security;
using RMD.Shared.Models.Receta.Header.Internos;
using RMD.Shared.Models.Receta.Header.Request;
using RMD.Shared.Models.Receta.Header.Responses;
using System.Data;

namespace RMD.Service.Receta
{
    public class RecetaService : IRecetaService
    {
        private readonly RecetasDbContext _context;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        //private readonly string _connectionString;
        private readonly IDapperService _dapperService;
        private readonly IHelperRecetaService _helperRecetaService;
        private readonly IHelperRecetaServiceMX _helperRecetaServiceMX;
        private readonly IHelperRecetaServiceES _helperRecetaServiceES;
        private readonly FolioHelper _folioHelper;
        private readonly string _country;

        public RecetaService(
            RecetasDbContext context,
            ICatalogoNotificacionService catalogoNotificacionService,
            IDapperService dapperService,
            IHelperRecetaService helperRecetaService,
            IHelperRecetaServiceMX helperRecetaServiceMx,
            IHelperRecetaServiceES helperRecetaServiceES,
            IConfiguration configuration,
            FolioHelper folioHelper)
        {
            _context = context;
            _catalogoNotificacionService = catalogoNotificacionService;
            _dapperService = dapperService;
            _country = configuration["Country"] ?? "MX";
            _folioHelper = folioHelper;
            _helperRecetaService = helperRecetaService;
            _helperRecetaServiceMX = helperRecetaServiceMx;
            _helperRecetaServiceES = helperRecetaServiceES;
        }
        /// <summary>
        /// Obtiene los datos completos de una receta médica por su ID y el ID del paciente,
        /// y genera un HTML con el formato de la receta.
        /// </summary>
        /// <param name="idReceta">Identificador único de la receta</param>
        /// <param name="idPaciente">Identificador único del paciente</param>
        /// <returns>Respuesta con contenido HTML de la receta o error correspondiente</returns>
        public async Task<ResponseFromService<string>> GetRecetaByIdRecetaAsync(Guid idReceta, Guid idPaciente)
        {
            try
            {
                // Ejecuta el SP con Dapper y obtiene múltiples result sets:
                // 1. Código de notificación
                // 2. Datos del paciente con la receta
                // 3. Detalles de los medicamentos
                // 4. Formato personalizado para el PDF
                using var multi = await _dapperService.QueryMultipleAsync(
                    "[Receta].[GetRecetaByIdReceta]",
                    new { IdReceta = idReceta, IdPaciente = idPaciente }
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<string>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<string>.Success(string.Empty, notificacion);

                var receta = multi.Read<Header_PacienteRequest>().FirstOrDefault();
                var detalles = multi.Read<DetalleInternoRequest>().ToList();
                var formato = multi.Read<Receta_FormatoHTMLRequest>().FirstOrDefault();

               
                var datosQR = new DatosQRHeader
                {
                    IdReceta = idReceta,
                    IdMedico = receta.IdMedico,
                    IdGEMP = receta.IdGEMP,
                    IdSucursal = receta.IdSucursal
                };
                string qrformato = _country switch
                {
                    "MX" => _helperRecetaServiceMX.GenerarRecetaQR(datosQR),
                    "ES" => _helperRecetaServiceES.GenerarRecetaQR(datosQR),
                    _ => string.Empty
                };

                var htmlContent = RecetaPdfExtension.GenerarHtmlReceta(receta, detalles, formato, qrformato);

                return ResponseFromService<string>.Success(htmlContent, notificacion);
            }
            catch (Exception ex)
            {
                // En caso de excepción, devuelve una notificación de error general
                var errorNotificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, errorNotificacion);
            }
        }        
        /// <summary>
        /// Timbra una receta médica si cumple con las condiciones necesarias.
        /// Realiza validaciones de negocio, ejecuta SPs, actualiza entidad y maneja todo dentro de una transacción.
        /// </summary>
        /// <param name="request">Request con el Id de la receta a timbrar</param>
        /// <param name="idUsuarioClaim">Id del usuario autenticado</param>
        /// <returns>Lista de IDs de recetas resultantes si se timbra exitosamente</returns>
        public async Task<ResponseFromService<List<Guid>>> TimbrarAsync(QRRequest request, Guid idUsuarioClaim)
        {
            // 1. Se obtiene una conexión SQL directa y se abre manualmente
            using var connection = new SqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync();

            // 2. Se inicia una transacción SQL explícita
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // 3. Se obtiene el ID del médico desde el contexto de usuario autenticado
                var idMedico = await _helperRecetaService.GetMedicoIdOrThrowAsync(idUsuarioClaim);

                // 4. Se vincula EF Core a la conexión y transacción manual (para que SaveChangesAsync funcione correctamente)
                _context.Database.SetDbConnection(connection);
                _context.Database.UseTransaction((SqlTransaction)transaction);

                // 5. Validación de existencia y estado de la receta
                var receta = await _context.ConsultaRecetas
                    .FirstOrDefaultAsync(r => r.IdReceta == request.IdReceta && r.Timbrada == false);

                if (receta == null)
                {
                    await transaction.RollbackAsync();
                    return ResponseFromService<List<Guid>>.Failure(
                        await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CONSULTA", "RECETA_YA_TIMBRADA"));
                }

                // 6. Validación: solo el médico dueño puede timbrar su receta
                if (receta.IdMedico != idMedico)
                {
                    await transaction.RollbackAsync();
                    return ResponseFromService<List<Guid>>.Success(
                        new List<Guid> { request.IdReceta },
                        await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CONSULTA", "NO_TIMBRAR"));
                }

                // 7. Se consultan los datos para timbrado (detalles + paquetes)
                var datos = (await _helperRecetaService.GetDatosParaTimbradoAsync(request.IdReceta, idUsuarioClaim)).Data;

                // 8. Lógica condicional si el país es España: ajuste específico de paquetes
                switch (_country)
                {
                    case "ES":
                        datos.Detalles = await _helperRecetaServiceES.PrepararPaquetesExactosESAsync(datos.Detalles, datos.Paquetes);
                        break;
                    // case "MX": ...
                }

                // 9. Calcular los paquetes exactos que se usarán para el timbrado
                var detalleReceta = await _helperRecetaService.CalcularPaquetesExactosAsync(datos.Detalles, datos.Paquetes);

                // 10. Serializar los detalles a JSON para enviarlos al SP
                var detalleJson = JsonSerializer.Serialize(detalleReceta.Select(d => new
                {
                    d.IdDetalleReceta,
                    d.IdReceta,
                    d.MedicamentoId,
                    d.MedicamentoType,
                    d.CantidadDiaria,
                    d.UnidadDispensacionId,
                    d.RutaAdministracionId,
                    d.Indicacion,
                    d.Duracion,
                    d.UnidadDuracion,
                    PeriodoInicio = d.PeriodoInicio.ToString("yyyy-MM-dd"),
                    PeriodoTerminacion = d.PeriodoTerminacion?.ToString("yyyy-MM-dd"),
                    d.IndicacionNombre,
                    d.Frecuency,
                    d.IdFrecuencyType,
                    d.Observaciones,
                    d.PackageQty,
                    d.CantidadSurtida,
                    d.IsNarcotic,
                    d.PsicoAnnexId
                }));

                // 11. Ejecutar el SP que inserta los datos de timbrado y devuelve IDs de nuevas recetas
                using var multi = await _dapperService.QueryMultipleAsync(
                    "[Receta].[Detalle_InsertTimbrado]",
                    new { request.IdReceta, DetalleRecetaJson = detalleJson },
                    connection: connection,
                    transaction: (SqlTransaction)transaction,
                    commandType: CommandType.StoredProcedure
                );


                // 12. Primer resultset: Código de notificación
                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                // 13. Validación de error del SP
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                {
                    await transaction.RollbackAsync();
                    return ResponseFromService<List<Guid>>.Failure(notificacion);
                }

                // 14. Segundo resultset: Lista de IDs de recetas generadas (pueden ser varias si se separa por tipo de medicamento)
                var recetasFinales = multi.Read<Guid>().ToList();

                // 15. Actualización del estado de la receta original
                receta.Timbrada = true;
                receta.FechaUltimaModificacion = DateTime.Now;
                receta.FechaCreacion = DateTime.Now;
                receta.Folio = await _folioHelper.GenerarFolioAsync(receta.IdGEMP, receta.FechaCreacion, receta.IdSucursal);

                // 16. Guardar cambios en la base de datos vía EF Core
                await _context.SaveChangesAsync();

                // 17. Confirmar la transacción completa
                await transaction.CommitAsync();

                // 18. Devolver éxito con IDs de recetas generadas
                return ResponseFromService<List<Guid>>.Success(recetasFinales, notificacion);
            }
            catch (SqlException sqlEx)
            {
                // Manejo de errores SQL: rollback + notificación genérica
                await transaction.RollbackAsync();
                var err = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<List<Guid>>.Exeption(sqlEx, err);
            }
            catch (Exception ex)
            {
                // Manejo de errores generales: rollback + notificación genérica
                await transaction.RollbackAsync();
                var err = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<List<Guid>>.Exeption(ex, err);
            }
        }        
        /// <summary>
        /// Obtiene los datos necesarios para actualizar una receta, incluyendo los datos del paciente y el detalle de medicamentos.
        /// </summary>
        /// <param name="idReceta">Identificador único de la receta.</param>
        /// <param name="idPaciente">Identificador del paciente asociado a la receta.</param>
        /// <param name="idMedico">Identificador del médico que emitió la receta.</param>
        /// <returns>
        /// Un <see cref="ResponseFromService{T}"/> que contiene un DTO con los datos de la receta y su detalle si fue exitosa la consulta,
        /// o una notificación de error si algo falló.
        /// </returns>
        public async Task<ResponseFromService<HeaderUpdateResponse>> GetRecetaUpdatgeByIdRecetaAsync(Guid idReceta, Guid idPaciente, Guid idMedico)
        {
            try
            {
                // Ejecuta el procedimiento almacenado y obtiene múltiples conjuntos de resultados
                using var multi = await _dapperService.QueryMultipleAsync(
                    "[Receta].[GetRecetaUpdateByIdReceta]",
                    new { IdReceta = idReceta, IdPaciente = idPaciente, IdMedico = idMedico }
                );

                // 1. Primer result set: Código de notificación de estado
                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                {
                    return ResponseFromService<HeaderUpdateResponse>.Failure(notificacion);
                }

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                {
                    return ResponseFromService<HeaderUpdateResponse>.Success(new HeaderUpdateResponse(), notificacion);
                }

                // 2. Segundo result set: Información principal de la receta (datos del paciente)
                var receta = multi.Read<Header_UpdatePacienteRequest>().FirstOrDefault();

                // 3. Tercer result set: Detalle de medicamentos de la receta
                var detalles = multi.Read<Detalle_UpdateResponse>().ToList();

                // Procesa los datos del paciente para convertirlos al formato esperado
                var recetaList = _helperRecetaService.ParsePacienteToParsed(receta);

                // Si se obtuvo una receta válida, retornar la respuesta exitosa
                return ResponseFromService<HeaderUpdateResponse>.Success(new HeaderUpdateResponse
                {
                    Receta = recetaList,
                    Detalles = detalles
                }, notificacion);

            }
            catch (Exception ex)
            {
                // Manejo de excepciones generales: retorna respuesta con tipo excepción
                var errorNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<HeaderUpdateResponse>.Exeption(ex, errorNotificacion);
            }
        }
        /// <summary>
        /// Consulta recetas filtradas por rol, grupo empresarial, sucursal y/o rango de fechas.
        /// </summary>
        /// <param name="roleId">Rol del usuario solicitante.</param>
        /// <param name="idGEMP">ID del grupo empresarial (GEMP).</param>
        /// <param name="idSucursal">ID de la sucursal (opcional).</param>
        /// <param name="folio">Folio de la receta (opcional, si se usa, se ignoran fechas).</param>
        /// <param name="startDate">Fecha de inicio del filtro (se usa si no hay folio).</param>
        /// <param name="endDate">Fecha de fin del filtro (se usa si no hay folio).</param>
        /// <param name="dateFilter">Campo por el cual se filtra la fecha (ej. "FechaCreacion").</param>
        /// <returns>Listado de recetas filtradas o error en caso de fallo.</returns>
        public async Task<ResponseFromService<IEnumerable<HeaderTextPlainResponse>>> GetFilteredRecetasAsync(
            string roleId, Guid idGEMP, Guid? idSucursal, string? folio, DateTime? startDate, DateTime? endDate, string dateFilter)
        {
            try
            {
                var parameters = new
                {
                    RoleId = roleId,
                    IdGEMP = idGEMP,
                    IdSucursal = idSucursal,
                    Folio = string.IsNullOrWhiteSpace(folio) ? null : folio,
                    StartDate = string.IsNullOrWhiteSpace(folio) ? startDate : null,
                    EndDate = string.IsNullOrWhiteSpace(folio) ? endDate : null,
                    DateFilter = string.IsNullOrWhiteSpace(folio) ? dateFilter : null
                };

                using var multi = await _dapperService.QueryMultipleAsync("[Receta].[GetFilteredRecetas]", parameters);
                var codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<HeaderTextPlainResponse>>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<IEnumerable<HeaderTextPlainResponse>>.Success(new List<HeaderTextPlainResponse>(), notificacion);


                var recetas = multi.Read<HeaderTextPlainResponse>().ToList();

                  return notificacion.ToastType.ToUpper() == "ERROR"
                    ? ResponseFromService<IEnumerable<HeaderTextPlainResponse>>.Failure(notificacion)
                    : ResponseFromService<IEnumerable<HeaderTextPlainResponse>>.Success(recetas, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<HeaderTextPlainResponse>>.Exeption(ex, error);
            }
        }
        /// <summary>
        /// Consulta recetas filtradas específicamente para un médico, por grupo empresarial, sucursal y fechas.
        /// </summary>
        /// <param name="idUsuario">ID del médico solicitante.</param>
        /// <param name="idGEMP">ID del grupo empresarial.</param>
        /// <param name="idSucursal">ID de la sucursal (opcional).</param>
        /// <param name="folio">Folio de la receta (opcional).</param>
        /// <param name="startDate">Fecha de inicio del filtro (si no se usa folio).</param>
        /// <param name="endDate">Fecha de fin del filtro (si no se usa folio).</param>
        /// <param name="dateFilter">Campo por el cual se filtra la fecha.</param>
        /// <returns>Listado de recetas o error en caso de fallo.</returns>
        public async Task<ResponseFromService<IEnumerable<HeaderTextPlainResponse>>> GetFilteredRecetasByIdMedicoAsync(
            Guid idUsuario, Guid idGEMP, Guid? idSucursal, string? folio, DateTime? startDate, DateTime? endDate, string dateFilter)
        {
            try
            {
                var parameters = new
                {
                    IdUsuario = idUsuario,
                    IdGEMP = idGEMP,
                    IdSucursal = idSucursal,
                    Folio = string.IsNullOrWhiteSpace(folio) ? null : folio,
                    StartDate = string.IsNullOrWhiteSpace(folio) ? startDate : null,
                    EndDate = string.IsNullOrWhiteSpace(folio) ? endDate : null,
                    DateFilter = string.IsNullOrWhiteSpace(folio) ? dateFilter : null
                };

                using var multi = await _dapperService.QueryMultipleAsync("[Receta].[GetFilteredRecetasByIdMedico]", parameters);
                var codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                {
                    return ResponseFromService<IEnumerable<HeaderTextPlainResponse>>.Failure(notificacion);
                }
                var recetas = multi.Read<HeaderTextPlainResponse>().ToList();

                return notificacion.ToastType.ToUpper() == "ERROR"
                    ? ResponseFromService<IEnumerable<HeaderTextPlainResponse>>.Failure(notificacion)
                    : ResponseFromService<IEnumerable<HeaderTextPlainResponse>>.Success(recetas, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<HeaderTextPlainResponse>>.Exeption(ex, error);
            }
        }
        /// <summary>
        /// Consulta todas las recetas relacionadas a un paciente, opcionalmente filtradas por rango de fechas.
        /// </summary>
        /// <param name="idUsuario">ID del usuario que consulta (por seguridad o auditoría).</param>
        /// <param name="idPaciente">ID del paciente dueño de las recetas.</param>
        /// <param name="startDate">Fecha inicial del filtro (opcional).</param>
        /// <param name="endDate">Fecha final del filtro (opcional).</param>
        /// <param name="dateFilter">Campo de filtro (ej. "FechaCreacion").</param>
        /// <returns>Listado de recetas del paciente o error si ocurre fallo.</returns>
        public async Task<ResponseFromService<List<HeaderTextPlainResponse>>> GetRecetasByIdPacienteAsync(
            Guid idUsuario, Guid idPaciente, DateTime? startDate, DateTime? endDate, string dateFilter)
        {
            try
            {
                var parameters = new
                {
                    IdPaciente = idPaciente,
                    IdUsuario = idUsuario,
                    StartDate = startDate,
                    EndDate = endDate,
                    DateFilter = string.IsNullOrWhiteSpace(dateFilter) ? null : dateFilter
                };

                using var multi = await _dapperService.QueryMultipleAsync("[Receta].[GetRecetasByIdPaciente]", parameters);
                var codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<List<HeaderTextPlainResponse>>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<List<HeaderTextPlainResponse>>.Success(new List<HeaderTextPlainResponse>(), notificacion);


                var recetas = multi.Read<HeaderTextPlainResponse>().ToList();

                return ResponseFromService<List<HeaderTextPlainResponse>>.Success(recetas, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<List<HeaderTextPlainResponse>>.Exeption(ex, error);
            }
        }        
        /// <summary>
        /// Obtiene el ID del paciente asociado a un usuario específico.
        /// </summary>
        /// <param name="idUsuario">ID del usuario del cual se desea obtener el paciente relacionado.</param>
        /// <returns>
        /// Un objeto <see cref="ResponseFromService{T}"/> conteniendo el ID del paciente (Guid?)
        /// o un error si ocurre alguna excepción o falla lógica.
        /// </returns>
        public async Task<ResponseFromService<Guid?>> GetIdPacienteByUsuarioAsync(Guid idUsuario)
        {
            try
            {
                // 1. Preparar los parámetros del SP
                var parameters = new { IdUsuario = idUsuario };

                // 2. Ejecutar el procedimiento almacenado con Dapper
                using var multi = await _dapperService.QueryMultipleAsync("Recetas_GetPacienteByIdUsuario", parameters);

                // 3. Primer resultset: código de notificación (para saber si fue exitoso o no)
                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<Guid?>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<Guid?>.Success(null, notificacion);

                // 4. Segundo resultset: ID del paciente asociado al usuario (puede ser null si no se encontró)
                Guid idPaciente = multi.ReadFirstOrDefault<Guid>();

                
                return ResponseFromService<Guid?>.Success(idPaciente, notificacion);
            }
            catch (Exception ex)
            {
                // 7. En caso de excepción, retornar mensaje de error controlado
                var errorNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<Guid?>.Exeption(ex, errorNotificacion);
            }
        }
        /// <summary>
        /// Obtiene el código QR generado dinámicamente para una receta y paciente específico.
        /// </summary>
        /// <param name="idReceta">ID de la receta.</param>
        /// <param name="idPaciente">ID del paciente asociado a la receta.</param>
        /// <returns>
        /// Un <see cref="ResponseFromService{T}"/> que contiene un string con el QR generado (en formato base64).
        /// Retorna error si la receta no existe o la notificación indica fallo.
        /// </returns>
        public async Task<ResponseFromService<string>> GetQRAsync(Guid idReceta, Guid idPaciente)
        {
            try
            {
                var parameters = new { IdReceta = idReceta, IdPaciente = idPaciente };

                var datosQR = await _dapperService
                    .QueryFirstOrDefaultAsync<DatosQRHeader>("[Receta].[GetQRByIdReceta]", parameters);

                // Si no hay datos, entonces el SP devolvió la notificación QR_NO_ENCONTRADO
                if (datosQR == null)
                {
                    var notFound = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("RECETASSP", "QR_NO_ENCONTRADO");
                    return ResponseFromService<string>.Failure(notFound);
                }

                // Generar el QR con los datos de la receta
                var qrformato = _helperRecetaServiceMX.GenerarRecetaQR(datosQR);

                var notificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("RECETASSP", "QR_ENCONTRADO");

                return ResponseFromService<string>.Success(qrformato, notificacion);
            }
            catch (Exception ex)
            {
                var errorNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, errorNotificacion);
            }
        }

        /// <summary>
        /// Obtiene todas las recetas generadas por un médico en una sucursal específica.
        /// </summary>
        /// <param name="idMedico">Identificador del médico.</param>
        /// <param name="idSucursal">Identificador de la sucursal.</param>
        /// <returns>
        /// Una respuesta estándar que incluye la lista de recetas encontradas para el médico,
        /// o un mensaje de error en caso de fallo.
        /// </returns>
        public async Task<ResponseFromService<IEnumerable<HeaderCounltResponse>>> ObtenerRecetasPorMedicoAsync(Guid idMedico, Guid idSucursal)
        {
            try
            {
                // 1. Preparar los parámetros requeridos por el SP.
                var parameters = new { IdMedico = idMedico, IdSucursal = idSucursal };

                // 2. Ejecutar el procedimiento almacenado que retorna múltiples resultados.
                using var multi = await _dapperService.QueryMultipleAsync(
                    "[Receta].[RecetasPorMedico]",
                    parameters
                );

                // 3. Leer el código de notificación del primer result set (estatus de la operación).
                int codigoNotificacion = multi.ReadFirstOrDefault<int>();

                // 4. Consultar la notificación correspondiente en el catálogo.
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                // 5. Validar si la notificación indica error, abortar si es así.
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<HeaderCounltResponse>>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<IEnumerable<HeaderCounltResponse>>.Success(new List<HeaderCounltResponse>(), notificacion);

                // 6. Leer el segundo result set: recetas en formato SQL.
                var recetasSQL = multi.Read<HeaderBySQL>().ToList();

                // 7. Mapear las recetas desde el modelo SQL a DTO de respuesta.
                var recetas = recetasSQL.Select(_helperRecetaService.TransformRecetaSQLToRecetaGet);

                // 8. Retornar respuesta exitosa con los datos transformados.
                return ResponseFromService<IEnumerable<HeaderCounltResponse>>.Success(recetas, notificacion);
            }
            catch (SqlException sqlEx)
            {
                // 9. Manejo de excepciones SQL.
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<HeaderCounltResponse>>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                // 10. Manejo de cualquier otro tipo de excepción.
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<HeaderCounltResponse>>.Exeption(ex, error);
            }
        }
        /// <summary>
        /// Consulta una receta médica por su ID y el usuario solicitante, incluyendo sus detalles.
        /// </summary>
        /// <param name="idReceta">ID único de la receta.</param>
        /// <param name="idUsuario">ID del usuario que solicita la consulta.</param>
        /// <returns>Objeto con los datos de la receta y sus detalles si se encuentra, o un error.</returns>
        public async Task<ResponseFromService<HeaderAndDetalleResponse>> ConsultarRecetaAsync(Guid idReceta, Guid idUsuario)
        {
            try
            {
                // 1. Ejecutar el procedimiento almacenado que devuelve la receta y sus detalles.
                var parameters = new { IdReceta = idReceta, IdUsuario = idUsuario };
                using var multi = await _dapperService.QueryMultipleAsync("[Receta].[ConsultarReceta]", parameters);

                // 2. Leer el código de notificación desde el primer resultset.
                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                // 3. Si hay error en la notificación, retornar fallo.
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<HeaderAndDetalleResponse>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<HeaderAndDetalleResponse>.Success(new HeaderAndDetalleResponse(), notificacion);

                // 4. Leer la receta del segundo resultset.
                var recetaSql = multi.Read<HeaderBySQL>().FirstOrDefault();

                // 5. Leer los detalles del tercer resultset.
                var detalles = multi.Read<DetalleRecetaGet>().ToList();

                // 6. Transformar la receta al modelo de respuesta.
                var receta = _helperRecetaService.TransformRecetaSQLToRecetaGet(recetaSql);

                // 7. Armar el payload y devolver éxito.
                var payload = new HeaderAndDetalleResponse
                {
                    Receta = receta,
                    Detalles = detalles
                };

                return ResponseFromService<HeaderAndDetalleResponse>.Success(payload, notificacion);
            }
            catch (SqlException sqlEx)
            {
                // Manejo de excepciones SQL
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<HeaderAndDetalleResponse>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                // Manejo de excepciones generales
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<HeaderAndDetalleResponse>.Exeption(ex, error);
            }
        }
        /// <summary>
        /// Registra una nueva receta médica en base a los datos proporcionados.
        /// </summary>
        /// <param name="request">Modelo con la información del paciente y las líneas de prescripción.</param>
        /// <param name="token">Token JWT con los datos de sesión del usuario.</param>
        /// <returns>ID de la nueva receta creada o error.</returns>
        public async Task<ResponseFromService<Guid>> RegistrarRecetaAsync(HeaderRequest request, string token)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Obtener datos del token: GEMP, Sucursal y Usuario
                var idGEMP = _helperRecetaService.ObtenerValorDesdeToken(token, "GEMP");
                var idSucursal = _helperRecetaService.ObtenerValorDesdeToken(token, "IdSucursal");
                var idUsuario = Guid.Parse(_helperRecetaService.ObtenerValorDesdeToken(token, "IdUsuario"));

                // 2. Validación de datos críticos del token
                if (string.IsNullOrEmpty(idGEMP) || string.IsNullOrEmpty(idSucursal))
                {
                    var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                    return ResponseFromService<Guid>.Failure(error);
                }

                // 3. Obtener el Id del médico desde el usuario
                var idMedico = await _context.Medicos
                    .Where(m => m.IdUsuario == idUsuario)
                    .Select(m => m.IdMedico)
                    .FirstOrDefaultAsync();

                if (idMedico == Guid.Empty)
                {
                    var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("RECETAS", "NO_MEDICO");
                    return ResponseFromService<Guid>.Failure(error);
                }

                // 4. Validar datos del paciente antes de insertar
                if (request.Paciente.Peso < 0 || request.Paciente.Peso > 999.99m
                    || request.Paciente.Talla < 0 || request.Paciente.Talla > 999.99m
                    || (request.Paciente.Creatinina.HasValue && (request.Paciente.Creatinina < 0 || request.Paciente.Creatinina > 999.99m)))
                {
                    var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                    return ResponseFromService<Guid>.Failure(error);
                }

                // 5. Setear el contexto de sesión del usuario en SQL Server
                var connection = _context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();
                command.CommandText = "EXEC sp_set_session_context @key=N'UserId', @value=@userId";
                command.Parameters.Add(new SqlParameter("@userId", idUsuario));
                await command.ExecuteNonQueryAsync();

                // 6. Crear el objeto receta principal
                var nuevaReceta = new Header
                {
                    IdReceta = Guid.NewGuid(),
                    IdMedico = idMedico,
                    IdPaciente = request.IdPaciente,
                    PacPeso = request.Paciente.Peso,
                    PacTalla = request.Paciente.Talla,
                    PacEmbarazo = request.Paciente.Embarazo,
                    PacSemAmenorrea = request.Paciente.SemanasAmenorrea ?? 0,
                    PacLactancia = request.Paciente.Lactancia,
                    PacCreatinina = request.Paciente.Creatinina ?? 0m,
                    Alergias = _helperRecetaService.ConvertirListaAString(request.Paciente.Alergias),
                    Molecules = _helperRecetaService.ConvertirListaAString(request.Paciente.Moleculas),
                    Patologias = _helperRecetaService.ConvertirListaAString(request.Paciente.Patologias),
                    IdSucursal = Guid.Parse(idSucursal),
                    IdGEMP = Guid.Parse(idGEMP),
                    FechaCreacion = DateTime.Now,
                    FechaUltimaModificacion = DateTime.Now
                };

                _context.ConsultaRecetas.Add(nuevaReceta);
                await _context.SaveChangesAsync();

                // 7. Insertar los detalles (líneas de prescripción)
                foreach (var detalle in request.PrescriptionLines)
                {
                    if (detalle.PeriodoInicio > detalle.PeriodoTerminacion)
                    {
                        var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                        return ResponseFromService<Guid>.Failure(error);
                    }

                    var nuevoDetalle = new DetalleInHeader
                    {
                        IdDetalleReceta = Guid.NewGuid(),
                        IdReceta = nuevaReceta.IdReceta,
                        MedicamentoId = detalle.IdMedicamento,
                        MedicamentoType = detalle.TipoMedicamento,
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

                // 8. Guardar todo y confirmar la transacción
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                return ResponseFromService<Guid>.Success(nuevaReceta.IdReceta, success);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                var inner = ex.InnerException?.Message ?? ex.Message;

                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                error.Mensaje = inner;

                return ResponseFromService<Guid>.Exeption(ex, error);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<Guid>.Exeption(ex, error);
            }
        }
        /// <summary>
        /// Actualiza los datos de una receta existente, validando su estado y permisos.
        /// </summary>
        /// <param name="req">Modelo con los datos actualizados de la receta y del paciente.</param>
        /// <param name="idUsuario">ID del usuario que realiza la operación.</param>
        /// <returns>Resultado con éxito o error según el proceso.</returns>
        public async Task<ResponseFromService<bool>> ActualizarRecetaAsync(HeaderUpdateRequest req, Guid idUsuario)
        {
            try
            {
                // 1. Obtener la receta desde la base de datos.
                var receta = await _context.ConsultaRecetas.FirstOrDefaultAsync(r => r.IdReceta == req.IdReceta);
                if (receta == null)
                    return ResponseFromService<bool>.Failure(
                        await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CONSULTA", "NO_ENCONTRADA"));

                // 2. Validar si la receta ya fue timbrada.
                if (receta.Timbrada)
                    return ResponseFromService<bool>.Failure(
                        await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CONSULTA", "RECETA_YA_TIMBRADA"));

                // 3. Validar si el usuario es el médico dueño de la receta.
                var idMedico = await _helperRecetaService.GetMedicoIdOrThrowAsync(idUsuario);
                if (receta.IdMedico != idMedico)
                    return ResponseFromService<bool>.Failure(
                        await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("CONSULTA", "NO_PERMITIDO"));

                // 4. Serializar datos de receta y detalles a formato JSON.
                var recetaJson = JsonSerializer.Serialize(new
                {
                    req.IdReceta,
                    req.IdPaciente,
                    req.IdMedico,
                    PacPeso = req.Paciente.Peso,
                    PacTalla = req.Paciente.Talla,
                    PacEmbarazo = req.Paciente.Embarazo,
                    PacSemAmenorrea = req.Paciente.SemanasAmenorrea ?? 0,
                    PacLactancia = req.Paciente.Lactancia,
                    PacCreatinina = req.Paciente.Creatinina ?? 0m,
                    Alergias = _helperRecetaService.ConvertirListaAString(req.Paciente.Alergias),
                    Molecules = _helperRecetaService.ConvertirListaAString(req.Paciente.Moleculas),
                    Patologias = _helperRecetaService.ConvertirListaAString(req.Paciente.Patologias),
                    receta.IdSucursal,
                    receta.IdGEMP
                });

                var detallesJson = JsonSerializer.Serialize(req.PrescriptionLines);

                // 5. Ejecutar SP de actualización de receta.
                await _dapperService.ExecuteAsync(
                    "EXEC [Receta].[ActualizarReceta] @RecetaJson, @DetallesJson",
                    new { RecetaJson = recetaJson, DetallesJson = detallesJson },
                    CommandType.Text
                );
                // 6. Actualizar los datos del paciente con las listas crónicas convertidas a string.
                var alergias = _helperRecetaService.ConvertirListaAString(req.AlergiasCronicas ?? new());
                var molecules = _helperRecetaService.ConvertirListaAString(req.MoleculasCronicas ?? new());
                var patologias = _helperRecetaService.ConvertirListaAString(req.PatologiasCronicas ?? new());

                await _helperRecetaService.ActualizarPacienteDesdeReceta(
                    req.IdPaciente, alergias, molecules, patologias, DateTime.Now
                );

                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                return ResponseFromService<bool>.Success(true, notificacion);
            }
            catch (Exception ex)
            {
                var err = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, err);
            }
        }
        /// <summary>
        /// Marca una receta como cancelada (Estatus = 3).
        /// </summary>
        /// <param name="idReceta">ID de la receta a cancelar.</param>
        /// <returns>Resultado indicando si se realizó la operación correctamente.</returns>
        public async Task<ResponseFromService<bool>> EliminarRecetaAsync(Guid idReceta)
        {
            try
            {
                // 1. Buscar la receta en la base de datos.
                var receta = await _context.RecetasSql.FirstOrDefaultAsync(r => r.IdReceta == idReceta);
                if (receta == null)
                {
                    var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NO_ENCONTRADO");
                    return ResponseFromService<bool>.Failure(notificacion);
                }

                // 2. Cambiar estatus a cancelada.
                receta.Estatus = 3;
                receta.FechaUltimaModificacion = DateTime.Now;
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                return ResponseFromService<bool>.Success(true, success);
            }
            catch (Exception ex)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, notificacion);
            }
        }
       
       
       
        

    }
}

