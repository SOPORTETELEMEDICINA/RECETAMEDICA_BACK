using System.Xml;
using System.Xml.Linq;
using RMD.Data;
using RMD.Extensions;
using RMD.Interface.Medicos;
using RMD.Interface.Notificaciones;
using RMD.Models.Medicos;
using RMD.Models.Responses;

namespace RMD.Service.Medicos
{
    public class MedicoService : IMedicoService
    {
        private readonly MedicosDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _appId;
        private readonly string _appKey;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public MedicoService(
            MedicosDbContext context,
            HttpClient httpClient,
            IConfiguration configuration,
            ICatalogoNotificacionService catalogoNotificacionService)
        {
            _context = context;
            _httpClient = httpClient;
            _baseUrl = configuration["VidalApi:BaseUrl"] ?? throw new ArgumentNullException(nameof(configuration), "BaseUrl not configured");
            _appId = configuration["VidalApi:AppId"] ?? throw new ArgumentNullException(nameof(configuration), "AppId not configured");
            _appKey = configuration["VidalApi:AppKey"] ?? throw new ArgumentNullException(nameof(configuration), "AppKey not configured");
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        public async Task<ResponseFromService<MedicoConsultaRequest>> GetMedicoByIdUsuarioAsync(Guid idUsuario)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Medicos_GetMedicoByIdUsuario", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);

                using var reader = await command.ExecuteReaderAsync();

                // 1) Leer código de notificación
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<MedicoConsultaRequest>.Failure(notificacion);

                // 2) Leer datos del médico
                MedicoConsultaRequest medico = null;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    medico = MedicoConsultaRequest.FromDataReader(reader);
                }
                else
                {
                    // Si no viene fila, devolvemos failure con la misma notificación
                    return ResponseFromService<MedicoConsultaRequest>.Failure(notificacion);
                }

                return ResponseFromService<MedicoConsultaRequest>.Success(medico, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<MedicoConsultaRequest>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<MedicoConsultaRequest>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<MedicoConsultaRequest>> GetMedicoByIdMedicoAsync(Guid idMedico)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Medicos_GetMedicoByIdMedico", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdMedico", idMedico);

                using var reader = await command.ExecuteReaderAsync();

                // 1) Leer código de notificación
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<MedicoConsultaRequest>.Failure(notificacion);

                // 2) Leer datos del médico
                MedicoConsultaRequest medico = null;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    medico = MedicoConsultaRequest.FromDataReader(reader);
                }
                else
                {
                    return ResponseFromService<MedicoConsultaRequest>.Failure(notificacion);
                }

                return ResponseFromService<MedicoConsultaRequest>.Success(medico, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<MedicoConsultaRequest>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<MedicoConsultaRequest>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<IEnumerable<MedicoConsultaRequest>>> GetMedicosBySucursalAsync(Guid idSucursal)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Medicos_GetMedicosBySucursal", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdSucursal", idSucursal);

                using var reader = await command.ExecuteReaderAsync();

                // 1) Leer código de notificación
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Failure(notificacion);

                // 2) Leer lista de médicos
                var lista = new List<MedicoConsultaRequest>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(MedicoConsultaRequest.FromDataReader(reader));
                    }
                }

                return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Success(lista, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<IEnumerable<MedicoConsultaRequest>>> GetMedicosByGEMPAsync(Guid idGEMP)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Medicos_GetMedicosByGEMP", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdGEMP", idGEMP);

                using var reader = await command.ExecuteReaderAsync();

                // 1) Leer código de notificación
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Failure(notificacion);

                // 2) Leer lista de médicos
                var lista = new List<MedicoConsultaRequest>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(MedicoConsultaRequest.FromDataReader(reader));
                    }
                }

                return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Success(lista, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<IEnumerable<MedicoConsultaRequest>>> GetMedicoByNameAsync(string nombreBusqueda)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Medicos_GetMedicoByName", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@NombreBusqueda", nombreBusqueda);

                using var reader = await command.ExecuteReaderAsync();

                // 1) Leer código de notificación
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Failure(notificacion);

                // 2) Leer lista de médicos
                var lista = new List<MedicoConsultaRequest>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(MedicoConsultaRequest.FromDataReader(reader));
                    }
                }

                return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Success(lista, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<bool>> CreateMedicoAsync(MedicoCreate medico, Guid idRol)
        {
            try
            {
                // 1) Preparar conexión y comando
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();
                using var command = new SqlCommand("Medicos_CreateMedico", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // 2) Parámetros
                var medicoTable = new List<MedicoCreate> { medico }.ToDataTable();
                command.Parameters.Add(new SqlParameter("@MedicoTable", SqlDbType.Structured)
                {
                    TypeName = "dbo.MedicoCreateTableType",
                    Value = medicoTable
                });
                command.Parameters.Add(new SqlParameter("@IdRol", idRol));

                // 3) Ejecutar y leer código de notificación
                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                // 4) Si es error, devolvemos Failure
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<bool>.Failure(notificacion);

                // 5) Interpretar resultado (true sólo si función es MEDICO_CREADO)
                bool created = notificacion.Funcion.Equals("MEDICO_CREADO", StringComparison.OrdinalIgnoreCase);
                return ResponseFromService<bool>.Success(created, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> UpdateMedicoAsync(Medico medico, Guid idUsuarioSolicitante)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Medicos_UpdateMedico", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // 1) tabla de parámetros
                var medicoTable = new List<Medico> { medico }.ToDataTable();
                command.Parameters.Add(new SqlParameter("@MedicoTable", SqlDbType.Structured)
                {
                    TypeName = "dbo.MedicoTableType",
                    Value = medicoTable
                });

                // 2) parámetro del solicitante
                command.Parameters.AddWithValue("@IdUsuarioSolicitante", idUsuarioSolicitante);

                // 3) ejecutar y leer código de notificación
                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                // 4) si es error, devolvemos failure
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<string>.Failure(notificacion);

                // 5) caso exitoso, retornamos la descripción de la notificación
                return ResponseFromService<string>.Success(notificacion.Descripcion, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<bool>> DeleteMedicoAsync(Guid idMedico, Guid idUsuarioSolicitante)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Medicos_DeleteMedico", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdMedico", idMedico);
                command.Parameters.AddWithValue("@IdUsuarioSolicitante", idUsuarioSolicitante);

                using var reader = await command.ExecuteReaderAsync();

                // 1) Leer código de notificación
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                {
                    return ResponseFromService<bool>.Failure(notificacion);
                }

                // 2) No hay segundo result set: el SP ya hizo el borrado lógico
                return ResponseFromService<bool>.Success(true, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<IEnumerable<PacientePorSucursalListModel>>> GetPacientesBySucursalListAsync(Guid idUsuario)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Medicos_GetPacientesBySucursal", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);

                using var reader = await command.ExecuteReaderAsync();

                // 1) Leer código de notificación
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<IEnumerable<PacientePorSucursalListModel>>.Failure(notificacion);

                // 2) Leer segundo conjunto: fila por fila
                var resultados = new List<PacientePorSucursalListModel>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        // Parseo de historial de patologías
                        var patologias = new List<PatologiaModel>();
                        var rawPatologias = reader.GetString(reader.GetOrdinal("Patologias"));
                        if (!string.IsNullOrEmpty(rawPatologias) && rawPatologias.Contains("vidal://cim10/code/"))
                        {
                            var codes = rawPatologias
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(s => s.Trim())
                                .Where(s => s.StartsWith("vidal://cim10/code/"));

                            foreach (var code in codes)
                            {
                                var vidalId = code.Substring(code.LastIndexOf('/') + 1);
                                var vidalName = await GetVidalNameAsync(vidalId);
                                patologias.Add(new PatologiaModel { VidalId = code, VidalName = vidalName });
                            }
                        }

                        resultados.Add(new PacientePorSucursalListModel
                        {
                            IdUsuario = reader.GetGuid(reader.GetOrdinal("IdUsuario")),
                            IdPaciente = reader.GetGuid(reader.GetOrdinal("IdPaciente")),
                            IdGEMP = reader.GetGuid(reader.GetOrdinal("IdGEMP")),
                            LogoGEMP = reader.GetString(reader.GetOrdinal("LogoGEMP")),
                            IdSucursal = reader.GetGuid(reader.GetOrdinal("IdSucursal")),
                            Nombres = reader.GetString(reader.GetOrdinal("Nombres")),
                            PrimerApellido = reader.GetString(reader.GetOrdinal("PrimerApellido")),
                            SegundoApellido = reader.GetString(reader.GetOrdinal("SegundoApellido")),
                            IdTipoIdentificacion = reader.GetInt32(reader.GetOrdinal("IdTipoIdentificacion")),
                            TipoIdentificacion = reader.GetString(reader.GetOrdinal("TipoIdentificacion")),
                            NumeroIdentificacion = reader.GetString(reader.GetOrdinal("NumeroIdentificacion")),
                            FechaNacimiento = reader.GetString(reader.GetOrdinal("FechaNacimiento")),
                            Edad = reader.GetInt32(reader.GetOrdinal("Edad")),
                            IdEntidadNacimiento = reader.GetInt32(reader.GetOrdinal("IdEntidadNacimiento")),
                            Genero = reader.GetString(reader.GetOrdinal("Genero")),
                            Alergias = reader.IsDBNull(reader.GetOrdinal("Alergias"))
                                                       ? new List<string>()
                                                       : reader.GetString(reader.GetOrdinal("Alergias")).Split(',').ToList(),
                            Molecules = reader.IsDBNull(reader.GetOrdinal("Molecules"))
                                                       ? new List<string>()
                                                       : reader.GetString(reader.GetOrdinal("Molecules")).Split(',').ToList(),
                            Patologias = patologias,
                            Movil = reader.GetString(reader.GetOrdinal("Movil")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Domicilio = reader.GetString(reader.GetOrdinal("Domicilio")),
                            IdAsentamiento = reader.GetInt32(reader.GetOrdinal("IdAsentamiento")),
                            Asentamiento = reader.GetString(reader.GetOrdinal("Asentamiento")),
                            IdTipoAsentamiento = reader.GetInt32(reader.GetOrdinal("IdTipoAsentamiento")),
                            TipoAsentamiento = reader.GetString(reader.GetOrdinal("TipoAsentamiento")),
                            IdCP = reader.GetInt32(reader.GetOrdinal("IdCP")),
                            CodigoPostal = reader.GetString(reader.GetOrdinal("CodigoPostal")),
                            IdMunicipio = reader.GetInt32(reader.GetOrdinal("IdMunicipio")),
                            NoMunicipio = reader.GetInt16(reader.GetOrdinal("NoMunicipio")),
                            Municipio = reader.GetString(reader.GetOrdinal("Municipio")),
                            IdCiudad = reader.GetInt32(reader.GetOrdinal("IdCiudad")),
                            Ciudad = reader.GetString(reader.GetOrdinal("Ciudad")),
                            IdEntidad = reader.GetInt32(reader.GetOrdinal("IdEntidad")),
                            Estado = reader.GetString(reader.GetOrdinal("Estado")),
                            Abreviatura = reader.GetString(reader.GetOrdinal("Abreviatura")),
                            Status = reader.GetString(reader.GetOrdinal("Status"))
                        });
                    }
                }

                // 3) Devolver respuesta exitosa con la notificación leída
                return ResponseFromService<IEnumerable<PacientePorSucursalListModel>>.Success(resultados, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<PacientePorSucursalListModel>>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<PacientePorSucursalListModel>>.Exeption(ex, error);
            }
        }

        private async Task<string> GetVidalNameAsync(string code)
        {
            try
            {
                var apiUrl = $"{_baseUrl}/pathologies?filter=CIM10&code={code}&app_id={_appId}&app_key={_appKey}";
                var response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                var xmlContent = await response.Content.ReadAsStringAsync();
                var vidalName = ParseVidalNameFromXml(xmlContent);

                return vidalName;
            }
            catch (HttpRequestException httpEx)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                throw new ApplicationException($"{notificacion.Descripcion} Detalle (HTTP): {httpEx.Message}");
            }
            catch (XmlException xmlEx)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                throw new ApplicationException($"{notificacion.Descripcion} Detalle (XML): {xmlEx.Message}");
            }
            catch (Exception ex)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                throw new ApplicationException($"{notificacion.Descripcion} Detalle: {ex.Message}");
            }
        }

        private string ParseVidalNameFromXml(string xmlContent)
        {
            try
            {
                var document = XDocument.Parse(xmlContent);
                var name = document.Descendants(XName.Get("name", "http://api.vidal.net/-/spec/vidal-api/1.0/"))
                                   .Select(x => x.Value)
                                   .FirstOrDefault();
                return name ?? string.Empty;
            }
            catch (XmlException xmlEx)
            {
                var notificacion = _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA").Result;
                throw new ApplicationException($"{notificacion.Descripcion} Detalle (XML): {xmlEx.Message}");
            }
            catch (Exception ex)
            {
                var notificacion = _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA").Result;
                throw new ApplicationException($"{notificacion.Descripcion} Detalle: {ex.Message}");
            }
        }
    }
}
