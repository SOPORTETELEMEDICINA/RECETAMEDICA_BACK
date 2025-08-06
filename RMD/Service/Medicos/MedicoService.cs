using Dapper;
using Microsoft.Data.SqlClient;
using RMD.Extensions;
using RMD.Interface.Medicos;
using RMD.Interface.Security;
using RMD.Shared.Models.Medicos;
using System.Xml;
using System.Xml.Linq;

namespace RMD.Service.Medicos
{
    public class MedicoService : IMedicoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _appId;
        private readonly string _appKey;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly IDapperService _dapperService;

        public MedicoService(
            HttpClient httpClient,
            IConfiguration configuration,
            ICatalogoNotificacionService catalogoNotificacionService,
            IDapperService dapperService,
            byte[] commonKey)
        {
            _httpClient = httpClient;
            _catalogoNotificacionService = catalogoNotificacionService;
            _dapperService = dapperService;

            var country = configuration["Country"]?.ToUpper() ?? "MX";
            var prefix = country == "ES" ? "VidalApiEs" : "VidalApi";

            _baseUrl = configuration[$"{prefix}:BaseUrl"] ?? throw new ArgumentNullException(nameof(configuration), "BaseUrl not configured");

            var encryptedAppId = configuration[$"{prefix}:AppId"] ?? throw new ArgumentNullException(nameof(configuration), "AppId not configured");
            var encryptedAppKey = configuration[$"{prefix}:AppKey"] ?? throw new ArgumentNullException(nameof(configuration), "AppKey not configured");

            _appId = EncryptionHelper.Decrypt(encryptedAppId, commonKey);
            _appKey = EncryptionHelper.Decrypt(encryptedAppKey, commonKey);
        }

        public async Task<ResponseFromService<MedicoConsultaRequest>> GetMedicoByIdUsuarioAsync(Guid idUsuario)
        {
            try
            {
                using var multi = await _dapperService.QueryMultipleAsync(
                    "Medicos_GetMedicoByIdUsuario",
                    new { IdUsuario = idUsuario }
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<MedicoConsultaRequest>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<MedicoConsultaRequest>.Success(new MedicoConsultaRequest(), notificacion);

                var medico = multi.ReadFirstOrDefault<MedicoConsultaRequest>();
                if (medico == null)
                    return ResponseFromService<MedicoConsultaRequest>.Failure(notificacion);

                return ResponseFromService<MedicoConsultaRequest>.Success(medico, notificacion);
            }
            catch (SqlException ex)
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<MedicoConsultaRequest>.Exeption(ex, notif);
            }
            catch (Exception ex)
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<MedicoConsultaRequest>.Exeption(ex, notif);
            }
        }
        public async Task<ResponseFromService<MedicoConsultaRequest>> GetMedicoByIdMedicoAsync(Guid idMedico)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@IdMedico", idMedico);

                using var multi = await _dapperService.QueryMultipleAsync("Medicos_GetMedicoByIdMedico", parameters);

                int codigoNotificacion = await multi.ReadFirstOrDefaultAsync<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<MedicoConsultaRequest>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<MedicoConsultaRequest>.Success(new MedicoConsultaRequest(), notificacion);

                var medico = await multi.ReadFirstOrDefaultAsync<MedicoConsultaRequest>();

                return medico is null
                    ? ResponseFromService<MedicoConsultaRequest>.Failure(notificacion)
                    : ResponseFromService<MedicoConsultaRequest>.Success(medico, notificacion);
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
                using var multi = await _dapperService.QueryMultipleAsync(
                    "Medicos_GetMedicosBySucursal",
                    new { IdSucursal = idSucursal }
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Success(new List<MedicoConsultaRequest>(), notificacion);

                var medicos = multi.Read<MedicoConsultaRequest>().ToList();
                return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Success(medicos, notificacion);
            }
            catch (SqlException ex)
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Exeption(ex, notif);
            }
            catch (Exception ex)
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Exeption(ex, notif);
            }
        }
        public async Task<ResponseFromService<IEnumerable<MedicoConsultaRequest>>> GetMedicosByGEMPAsync(Guid idGEMP)
        {
            try
            {
                using var multi = await _dapperService.QueryMultipleAsync(
                    "Medicos_GetMedicosByGEMP",
                    new { IdGEMP = idGEMP }
                );
                
                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Failure(notificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Success(new List<MedicoConsultaRequest>(), notificacion);

                var medicos = multi.Read<MedicoConsultaRequest>().ToList();
                return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Success(medicos, notificacion);
            }
            catch (SqlException ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Exeption(ex, error);
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
                using var multi = await _dapperService.QueryMultipleAsync(
                    "Medicos_GetMedicoByName",
                    new { NombreBusqueda = nombreBusqueda }
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Success(new List<MedicoConsultaRequest>(), notificacion);

                var lista = multi.Read<MedicoConsultaRequest>().ToList();
                return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Success(lista, notificacion);
            }
            catch (SqlException ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<MedicoConsultaRequest>>.Exeption(ex, error);
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
                var table = new List<MedicoCreate> { medico }.ToDataTable();

                var parametros = new
                {
                    MedicoTable = table.AsTableValuedParameter("dbo.MedicoCreateTableType"),
                    IdRol = idRol
                };

                using var multi = await _dapperService.QueryMultipleAsync("Medicos_CreateMedico", parametros);
                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<bool>.Failure(notificacion);

                bool creado = notificacion.Funcion.Equals("MEDICO_CREADO", StringComparison.OrdinalIgnoreCase);
                return ResponseFromService<bool>.Success(creado, notificacion);
            }
            catch (SqlException ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, error);
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
                var table = new List<Medico> { medico }.ToDataTable();

                var parametros = new
                {
                    MedicoTable = table.AsTableValuedParameter("dbo.MedicoTableType"),
                    IdUsuarioSolicitante = idUsuarioSolicitante
                };

                using var multi = await _dapperService.QueryMultipleAsync("Medicos_UpdateMedico", parametros);
                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<string>.Failure(notificacion);

                return ResponseFromService<string>.Success(notificacion.Descripcion, notificacion);
            }
            catch (SqlException ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
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
                var parametros = new
                {
                    IdMedico = idMedico,
                    IdUsuarioSolicitante = idUsuarioSolicitante
                };

                using var multi = await _dapperService.QueryMultipleAsync("Medicos_DeleteMedico", parametros);
                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<bool>.Failure(notificacion);

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
                var parameters = new { IdUsuario = idUsuario };
                using var multi = await _dapperService.QueryMultipleAsync("Medicos_GetPacientesBySucursal", parameters);

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<PacientePorSucursalListModel>>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<IEnumerable<PacientePorSucursalListModel>>.Success(new List<PacientePorSucursalListModel>(), notificacion);

                var data = multi.Read<dynamic>().ToList();

                var lista = new List<PacientePorSucursalListModel>();
                foreach (var row in data)
                {
                    var patologias = new List<PatologiaModel>();
                    string rawPatologias = row.Patologias;
                    if (!string.IsNullOrEmpty(rawPatologias) && rawPatologias.Contains("vidal://cim10/code/"))
                    {
                        var codes = rawPatologias.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                 .Select(s => s.Trim())
                                                 .Where(s => s.StartsWith("vidal://cim10/code/"));

                        foreach (var code in codes)
                        {
                            var vidalId = code.Substring(code.LastIndexOf('/') + 1);
                            var vidalName = await GetVidalNameAsync(vidalId);
                            patologias.Add(new PatologiaModel { VidalId = code, VidalName = vidalName });
                        }
                    }

                    lista.Add(new PacientePorSucursalListModel
                    {
                        IdUsuario = row.IdUsuario,
                        IdPaciente = row.IdPaciente,
                        IdGEMP = row.IdGEMP,
                        LogoGEMP = row.LogoGEMP,
                        IdSucursal = row.IdSucursal,
                        Nombres = row.Nombres,
                        PrimerApellido = row.PrimerApellido,
                        SegundoApellido = row.SegundoApellido,
                        IdTipoIdentificacion = row.IdTipoIdentificacion,
                        TipoIdentificacion = row.TipoIdentificacion,
                        NumeroIdentificacion = row.NumeroIdentificacion,
                        FechaNacimiento = row.FechaNacimiento,
                        Edad = row.Edad,
                        IdEntidadNacimiento = row.IdEntidadNacimiento,
                        Genero = row.Genero,
                        Alergias = string.IsNullOrEmpty(row.Alergias) ? new List<string>() : row.Alergias.Split(',').ToList(),
                        Molecules = string.IsNullOrEmpty(row.Molecules) ? new List<string>() : row.Molecules.Split(',').ToList(),
                        Patologias = patologias,
                        Movil = row.Movil,
                        Email = row.Email,
                        Domicilio = row.Domicilio,
                        IdAsentamiento = row.IdAsentamiento,
                        Asentamiento = row.Asentamiento,
                        IdTipoAsentamiento = row.IdTipoAsentamiento,
                        TipoAsentamiento = row.TipoAsentamiento,
                        IdCP = row.IdCP,
                        CodigoPostal = row.CodigoPostal,
                        IdMunicipio = row.IdMunicipio,
                        NoMunicipio = row.NoMunicipio,
                        Municipio = row.Municipio,
                        IdCiudad = row.IdCiudad,
                        Ciudad = row.Ciudad,
                        IdEntidad = row.IdEntidad,
                        Estado = row.Estado,
                        Abreviatura = row.Abreviatura,
                        Status = row.Status
                    });
                }

                return ResponseFromService<IEnumerable<PacientePorSucursalListModel>>.Success(lista, notificacion);
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
