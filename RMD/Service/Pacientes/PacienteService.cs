using RMD.Interface.Pacientes;
using RMD.Interface.Security;
using RMD.Interface.Vidal;
using RMD.Shared.Models.Pacientes;
using RMD.Shared.Models.Pacientes.Request;
using RMD.Shared.Models.Pacientes.Response;

namespace RMD.Service.Pacientes
{
    public class PacienteService(
        ICatalogoNotificacionService catalogoNotificacionService,
           IDapperService dapperService,
           ICIM10Service cim10Service,
           IAllergyService allergyService,
           IMoleculeService moleculeService,
           IConfiguration configuration,
           IHelperPacienteServiceES helperPacienteServiceES) : IPacienteService
    {
        private readonly ICatalogoNotificacionService _catalogoNotificacionService = catalogoNotificacionService;
        private readonly IDapperService _dapperService = dapperService;
        private readonly IHelperPacienteServiceES _helperPacienteServiceES = helperPacienteServiceES;
        private readonly string _country = configuration["Country"] ?? "MX";
        public async Task<ResponseFromService<PacienteConsultaResponse>> GetPacienteByIdUsuarioAsync(Guid idUsuario)
        {
            try
            {
                var parameters = new { IdUsuario = idUsuario };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "Paciente_GetPacienteByIdUsuario",
                    parameters
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<PacienteConsultaResponse>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<PacienteConsultaResponse>.Success(new PacienteConsultaResponse(), notificacion);

                var usuarioPaciente = multi.Read<UsuarioPaciente>().FirstOrDefault();

                var paciente = usuarioPaciente is not null
                    ? await MapToPacienteConsultaRequest(usuarioPaciente)
                    : null;

                return ResponseFromService<PacienteConsultaResponse>.Success(paciente, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<PacienteConsultaResponse>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<PacienteConsultaResponse>> GetPacienteByIdPacienteAsync(Guid idPaciente)
        {
            try
            {
                var parameters = new { IdPaciente = idPaciente };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "Paciente_GetPacienteByIdPaciente",
                    parameters
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<PacienteConsultaResponse>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<PacienteConsultaResponse>.Success(new PacienteConsultaResponse(), notificacion);

                var usuarioPaciente = multi.Read<UsuarioPaciente>().FirstOrDefault();
                var paciente = usuarioPaciente is not null
                    ? await MapToPacienteConsultaRequest(usuarioPaciente)
                    : null;

                return ResponseFromService<PacienteConsultaResponse>.Success(paciente, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<PacienteConsultaResponse>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<IEnumerable<PacienteConsultaResponse>>> GetPacienteByNameAsync(string nombreBusqueda, Guid idGemp)
        {
            try
            {
                var parameters = new
                {
                    NombreBusqueda = nombreBusqueda,
                    IdGemp = idGemp
                };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "Paciente_GetPacienteByName",
                    parameters
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<PacienteConsultaResponse>>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<IEnumerable<PacienteConsultaResponse>>.Success(new List<PacienteConsultaResponse>(), notificacion);

                var lista = new List<PacienteConsultaResponse>();
                var pacientes = multi.Read<UsuarioPaciente>().ToList();

                foreach (var usuarioPaciente in pacientes)
                {
                    var paciente = await MapToPacienteConsultaRequest(usuarioPaciente);
                    lista.Add(paciente);
                }

                return ResponseFromService<IEnumerable<PacienteConsultaResponse>>.Success(lista, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<PacienteConsultaResponse>>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<bool>> CreatePacienteAsync(PacienteCreateConListasRequest pacienteRequest, Guid idUsuarioSolicitante)
        {
            try
            {
                int result = await EjecutarStoredProcedureAsync("Pacientes_CrearPaciente", pacienteRequest, idUsuarioSolicitante);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(result);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<bool>.Failure(notificacion);

                return ResponseFromService<bool>.Success(true, notificacion);
            }
            catch (Exception ex)
            {
                var notificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, notificacion);
            }
        }
        public async Task<ResponseFromService<bool>> UpdatePacienteAsync(PacienteConListasRequest pacienteRequest, Guid idUsuarioSolicitante)
        {
            try
            {
                int codigoNotificacion = await EjecutarStoredProcedureAsync("Pacientes_UpdatePaciente", pacienteRequest, idUsuarioSolicitante);
                var spNotificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                
                if (spNotificacion.ToastType.ToUpperInvariant() == "SUCCESS" ||
                    spNotificacion.ToastType.ToUpperInvariant() == "INFO")
                {
                    return ResponseFromService<bool>.Success(true, spNotificacion);
                }
                else
                {
                    return ResponseFromService<bool>.Failure(spNotificacion);
                }
            }
            catch (Exception ex)
            {
                var notificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, notificacion);
            }
        }
        public async Task<ResponseFromService<bool>> EliminarPacienteAsync(Guid idPaciente, Guid idUsuarioSolicitante)
        {
            try
            {
                // Ejecuta el SP y obtiene el código de notificación devuelto.
                int codigo = await EjecutarStoredProcedureAsync("Pacientes_EliminarPaciente", idPaciente, idUsuarioSolicitante);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigo);

                if (notificacion.ToastType.ToUpperInvariant() == "SUCCESS" ||
                    notificacion.ToastType.ToUpperInvariant() == "INFO")
                {
                    return ResponseFromService<bool>.Success(true, notificacion);
                }
                else
                {
                    return ResponseFromService<bool>.Failure(notificacion);
                }
            }
            catch (Exception ex)
            {
                var errorNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, errorNotificacion);
            }
        }
        public async Task<ResponseFromService<IEnumerable<EntidadNacimientoResponse>>> GetEntidadesFederativasAsync()
        {
            try
            {
                using var multi = await _dapperService.QueryMultipleAsync(
                    "Paciente_GetEntidadesFederativas"
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<EntidadNacimientoResponse>>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<IEnumerable<EntidadNacimientoResponse>>.Success(new List<EntidadNacimientoResponse>(), notificacion);

                var lista = multi.Read<EntidadNacimientoResponse>().ToList();

                return ResponseFromService<IEnumerable<EntidadNacimientoResponse>>.Success(lista, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<EntidadNacimientoResponse>>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<IEnumerable<PacienteConsultaResponse>>> GetPacientesBySucursalAsync(Guid idSucursal)
        {
            try
            {
                var parameters = new { IdSucursal = idSucursal };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "Paciente_GetPacientesBySucursal",
                    parameters
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<PacienteConsultaResponse>>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<IEnumerable<PacienteConsultaResponse>>.Success(new List<PacienteConsultaResponse>(), notificacion);

                var pacientesRaw = multi.Read<UsuarioPaciente>().ToList();
                var lista = new List<PacienteConsultaResponse>();

                foreach (var usuarioPaciente in pacientesRaw)
                {
                    var paciente = await MapToPacienteConsultaRequest(usuarioPaciente);
                    lista.Add(paciente);
                }

                return ResponseFromService<IEnumerable<PacienteConsultaResponse>>.Success(lista, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<PacienteConsultaResponse>>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<IEnumerable<PacienteConsultaResponse>>> GetPacientesByGEMPAsync(Guid idGemp)
        {
            try
            {
                var parameters = new { IdGemp = idGemp };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "Paciente_GetPacientesByGEMP",
                    parameters
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<PacienteConsultaResponse>>.Failure(notificacion);


                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<IEnumerable<PacienteConsultaResponse>>.Success(new List<PacienteConsultaResponse>(), notificacion);

                var pacientesRaw = multi.Read<UsuarioPaciente>().ToList();
                var lista = new List<PacienteConsultaResponse>();

                foreach (var usuarioPaciente in pacientesRaw)
                {
                    var paciente = await MapToPacienteConsultaRequest(usuarioPaciente);
                    lista.Add(paciente);
                }

                return ResponseFromService<IEnumerable<PacienteConsultaResponse>>.Success(lista, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<PacienteConsultaResponse>>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<IEnumerable<PacienteConsultaResponse>>> GetPacientesByMedicoAsync(Guid idMedico)
        {
            try
            {
                var parameters = new { IdMedico = idMedico };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "Paciente_GetPacientesByMedico",
                    parameters
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<PacienteConsultaResponse>>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<IEnumerable<PacienteConsultaResponse>>.Success(new List<PacienteConsultaResponse>(), notificacion);

                var pacientesRaw = multi.Read<UsuarioPaciente>().ToList();
                var lista = new List<PacienteConsultaResponse>();

                foreach (var usuarioPaciente in pacientesRaw)
                {
                    var paciente = await MapToPacienteConsultaRequest(usuarioPaciente);
                    lista.Add(paciente);
                }

                return ResponseFromService<IEnumerable<PacienteConsultaResponse>>.Success(lista, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<PacienteConsultaResponse>>.Exeption(ex, error);
            }
        }
        private async Task<int> EjecutarStoredProcedureAsync(string procedimiento, object pacienteRequest, Guid idUsuarioSolicitante)
        {
            try
            {
                object parametros;

                if (pacienteRequest is Guid idPaciente)
                {
                    parametros = new
                    {
                        IdPaciente = idPaciente,
                        IdUsuarioSolicitante = idUsuarioSolicitante
                    };
                }
                else
                {
                    var pacienteJson = JsonSerializer.Serialize(pacienteRequest);
                    parametros = new
                    {
                        PacienteJson = pacienteJson,
                        IdUsuarioSolicitante = idUsuarioSolicitante
                    };
                }

                using var multi = await _dapperService.QueryMultipleAsync(procedimiento, parametros);

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                return codigoNotificacion;
            }
            catch (Exception)
            {
                var notificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return notificacion.CodigoNotificacion;
            }
        }
        private async Task<PacienteConsultaResponse> MapToPacienteConsultaRequest(UsuarioPaciente paciente)
        {
            try
            {
                var alergias = await allergyService.ObtenerAlergiasPorIdsAsync(paciente.Alergias);
                var molecules = await moleculeService.ObtenerMoleculesPorIdsAsync(paciente.Molecules);
                var patologias = await cim10Service.ObtenerCIM10PorIdsAsync(paciente.Patologias);

                return new PacienteConsultaResponse
                {
                    IdUsuario = paciente.IdUsuario,
                    IdTipoUsuario = paciente.IdTipoUsuario,
                    IdPaciente = paciente.IdPaciente,
                    IdGEMP = paciente.IdGEMP,
                    GrupoEmpresarial = paciente.GrupoEmpresarial,
                    IdSucursal = paciente.IdSucursal,
                    Sucursal = paciente.Sucursal,
                    Usr = paciente.Usr,
                    Nombres = paciente.Nombres,
                    PrimerApellido = paciente.PrimerApellido,
                    SegundoApellido = paciente.SegundoApellido,
                    IdTipoIdentificacion = paciente.IdTipoIdentificacion,
                    TipoIdentificacion = paciente.TipoIdentificacion,
                    NumeroIdentificacion = paciente.NumeroIdentificacion,
                    FechaNacimiento = paciente.FechaNacimiento,
                    Edad = paciente.Edad,
                    IdEntidadNacimiento = paciente.IdEntidadNacimiento,
                    EntidadNacimiento = paciente.EntidadNacimiento,
                    Genero = paciente.Genero,
                    Movil = paciente.Movil,
                    Email = paciente.Email,
                    Domicilio = paciente.Domicilio,
                    IdAsentamiento = paciente.IdAsentamiento,
                    Asentamiento = paciente.Asentamiento,
                    IdTipoAsentamiento = paciente.IdTipoAsentamiento,
                    TipoAsentamiento = paciente.TipoAsentamiento,
                    IdCP = paciente.IdCP,
                    CodigoPostal = paciente.CodigoPostal,
                    IdMunicipio = paciente.IdMunicipio,
                    NoMunicipio = paciente.NoMunicipio,
                    Municipio = paciente.Municipio,
                    IdCiudad = paciente.IdCiudad,
                    Ciudad = paciente.Ciudad,
                    IdEntidad = paciente.IdEntidad,
                    Estado = paciente.Estado,
                    Abreviatura = paciente.Abreviatura,
                    Firma = paciente.Firma,
                    Imagen = paciente.Imagen,
                    Alergias = alergias,
                    Molecules = molecules,
                    Patologias = patologias
                };
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                throw new ApplicationException($"{error.Descripcion}. Detalle: {ex.Message}");
            }
        }
        public async Task<ResponseFromService<string>> GenerarQRParaPacienteAsync(Guid idPaciente)
        {
            ResponseFromService<string> repositoryQR = new();
            return _country switch
            {
                "ES" => await _helperPacienteServiceES.GenerarQRParaPacienteAsync(idPaciente),
                _ => repositoryQR 
            };

        }
        //public async Task<ResponseFromService<Guid?>> GetIdPacienteByUsuarioAsync(Guid idUsuario)
        //{
        //    try
        //    {
        //        var parameters = new { IdUsuario = idUsuario };

        //        using var multi = await _dapperService.QueryMultipleAsync(
        //            "Recetas_GetPacienteByIdUsuario",
        //            parameters
        //        );

        //        int codigoNotificacion = multi.ReadFirstOrDefault<int>();
        //        var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

        //        if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
        //            return ResponseFromService<Guid?>.Failure(notificacion);

        //        var idPaciente = multi.Read<Guid?>().FirstOrDefault();

        //        return ResponseFromService<Guid?>.Success(idPaciente, notificacion);
        //    }
        //    catch (Exception ex)
        //    {
        //        var error = await _catalogoNotificacionService
        //            .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
        //        return ResponseFromService<Guid?>.Exeption(ex, error);
        //    }
        //}
    }
}
