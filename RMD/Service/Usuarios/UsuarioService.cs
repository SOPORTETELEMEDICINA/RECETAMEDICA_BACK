using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Extensions;
using RMD.Interface.Security;
using RMD.Interface.Usuarios;
using RMD.Shared.Models.Usuarios;
using System.Data;

namespace RMD.Service.Usuarios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly CatalogoDbContext _catalogo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        //private readonly Guid _IdUsuario;
        private readonly CifradoHelper _cifradoHelper;
        private readonly IDapperService _dapperService;

        public UsuarioService(
            IHttpContextAccessor httpContextAccessor,
            CatalogoDbContext catalogo,
            CifradoHelper cifradoHelper,
            ICatalogoNotificacionService catalogoNotificacionService,
            IDapperService dapperService)  // Se inyecta la cadena
        {
            _catalogo = catalogo;
            _cifradoHelper = cifradoHelper;
            _httpContextAccessor = httpContextAccessor;
            _catalogoNotificacionService = catalogoNotificacionService;
            _dapperService = dapperService;
        }       
        public async Task<ResponseFromService<IEnumerable<RequestUsuario>>> GetUsuariosByGEMPAsync(Guid idGEMP)
        {
            try
            {
                // 1. Obtener los valores del token.
                var idUsuarioSolicitante = _httpContextAccessor.HttpContext.User.FindFirstValue("IdUsuario");
                var idRolSolicitante = _httpContextAccessor.HttpContext.User.FindFirstValue("IdRol");

                // 2. Validar que se puedan convertir a Guid.
                if (!Guid.TryParse(idUsuarioSolicitante, out var parsedIdUsuario) ||
                    !Guid.TryParse(idRolSolicitante, out var parsedIdRol))
                {
                    var notificacionfail = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                    return ResponseFromService<IEnumerable<RequestUsuario>>.Failure(notificacionfail);
                }

                // 3. Ejecutar SP con Dapper.
                var parameters = new
                {
                    IdGEMP = idGEMP,
                    IdUsuarioSolicitante = parsedIdUsuario,
                    IdRolSolicitante = parsedIdRol
                };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "Usuarios_GetUsuariosByGEMP",
                    parameters
                );

                // 4. Leer el código de notificación del primer result set.
                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                // 5. Si la notificación indica error, salir.
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<RequestUsuario>>.Failure(notificacion);

                // 6. Leer usuarios del segundo result set.
                var usuarios = multi.Read<RequestUsuario>().ToList();

                // 7. Retornar respuesta exitosa.
                return ResponseFromService<IEnumerable<RequestUsuario>>.Success(usuarios, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<RequestUsuario>>.Exeption(ex, error);
            }
        }        
        public async Task<ResponseFromService<string>> CambiarPasswordAsync(Guid idUsuario, string nuevaPassword)
        {
            try
            {
                var nuevaPasswordCifrada = _cifradoHelper.HashPassword(nuevaPassword);

                var parameters = new
                {
                    IdUsuario = idUsuario,
                    NuevaPassword = nuevaPasswordCifrada
                };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "Usuarios_CambiarPassword",
                    parameters
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                {
                    return ResponseFromService<string>.Failure(notificacion);
                }

                return ResponseFromService<string>.Success(notificacion.Descripcion, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }        
        public async Task<ResponseFromService<IEnumerable<RequestUsuario>>> GetUsuariosBySucursalAsync(Guid idSucursal)
        {
            try
            {
                // 1. Obtener los valores del token
                var idUsuarioSolicitante = _httpContextAccessor.HttpContext.User.FindFirstValue("IdUsuario");
                var idRolSolicitante = _httpContextAccessor.HttpContext.User.FindFirstValue("IdRol");

                // 2. Validar que los valores del token puedan convertirse a Guid
                if (!Guid.TryParse(idUsuarioSolicitante, out var parsedIdUsuario) ||
                    !Guid.TryParse(idRolSolicitante, out var parsedIdRol))
                {
                    var error = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                    return ResponseFromService<IEnumerable<RequestUsuario>>.Failure(error);
                }

                // 3. Ejecutar SP con Dapper
                var parameters = new
                {
                    IdSucursal = idSucursal,
                    IdUsuarioSolicitante = parsedIdUsuario,
                    IdRolSolicitante = parsedIdRol
                };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "Usuarios_GetUsuariosBySucursal",
                    parameters
                );

                // 4. Leer código de notificación
                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                // 5. Verificar si hay error
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<RequestUsuario>>.Failure(notificacion);

                // 6. Leer usuarios del segundo result set
                var usuarios = multi.Read<RequestUsuario>().ToList();

                // 7. Retornar resultado
                return ResponseFromService<IEnumerable<RequestUsuario>>.Success(usuarios, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<RequestUsuario>>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<object>> AddUsuarioAsync(UsuarioCreate usuario, Guid idRol, string rol, Guid idGemp, Guid idSucursal)
        {
            try
            {
                // Validar asentamiento
                var asentamientoExiste = await _catalogo.CatAsentamientos
                    .AnyAsync(a => a.IdAsentamiento == usuario.IdAsentamiento);

                if (!asentamientoExiste)
                {
                    var error = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ASENTAMIENTO_NO_ENCONTRADO");
                    return ResponseFromService<object>.Failure(error);
                }

                // Cifrar password si aplica
                if (!string.IsNullOrEmpty(usuario.Password))
                {
                    usuario.Password = _cifradoHelper.HashPassword(usuario.Password);
                }

                // Preparar parámetros
                var tableParam = new SqlParameter("@UsuarioData", SqlDbType.Structured)
                {
                    TypeName = "dbo.UsuarioCreateTableType",
                    Value = new List<UsuarioCreate> { usuario }.ToDataTable()
                };

                var parametros = new DynamicParameters();
                parametros.Add("@UsuarioData", tableParam.Value, DbType.Object);
                parametros.Add("@IdRol", idRol);
                parametros.Add("@IdGemp", idGemp);
                parametros.Add("@IdSucursal", idSucursal);

                // Ejecutar SP con Dapper
                using var multi = await _dapperService.QueryMultipleAsync("Usuarios_CreateUSR", parametros);

                // Leer código de notificación
                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<object>.Failure(notificacion);

                // Leer Id generado
                var idCreado = multi.ReadFirstOrDefault<Guid>();

                return ResponseFromService<object>.Success(idCreado, notificacion);
            }
            catch (SqlException ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<object>.Exeption(ex, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<object>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<string>> UpdateUsuarioAsync(Usuario usuario, Guid idUsuarioSolicitante)
        {
            try
            {
                // Validar asentamiento
                var asentamientoExiste = await _catalogo.CatAsentamientos
                    .AnyAsync(a => a.IdAsentamiento == usuario.IdAsentamiento);

                if (!asentamientoExiste)
                {
                    var error = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("CATALOGOS", "ASENTAMIENTO_NO_ENCONTRADO");
                    return ResponseFromService<string>.Failure(error);
                }

                // Validar usuario
                if (usuario.IdUsuario == Guid.Empty)
                {
                    var error = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                    return ResponseFromService<string>.Failure(error);
                }

                // Cifrar password si aplica
                usuario.Password = !string.IsNullOrEmpty(usuario.Password)
                    ? _cifradoHelper.HashPassword(usuario.Password)
                    : null;

                // Preparar parámetros
                var usuarioParam = new SqlParameter("@UsuarioTable", SqlDbType.Structured)
                {
                    TypeName = "dbo.UsuarioTableType",
                    Value = new List<Usuario> { usuario }.ToDataTable()
                };

                var parametros = new DynamicParameters();
                parametros.Add("@UsuarioTable", usuarioParam.Value, DbType.Object);
                parametros.Add("@IdUsuarioSolicitante", idUsuarioSolicitante);

                // Ejecutar SP
                using var multi = await _dapperService.QueryMultipleAsync("Usuarios_UpdateUsuario", parametros);

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<string>.Failure(notificacion);

                return ResponseFromService<string>.Success(null, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<UsuarioDetalle>> GetUsuarioByUsernameAsync(string username)
        {
            try
            {
                var parameters = new { Usr = username };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "Usuarios_GetUsuarioByUsername",
                    parameters
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<UsuarioDetalle>.Failure(notificacion);

                var usuarioDetalle = multi.Read<UsuarioDetalle>().FirstOrDefault();

                return ResponseFromService<UsuarioDetalle>.Success(usuarioDetalle, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<UsuarioDetalle>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<UsuarioDetalle>> GetUsuarioPacienteByUsernameAsync(string username)
        {
            try
            {
                var parameters = new { Usr = username };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "Usuarios_GetUsuarioPacienteByUsername",
                    parameters
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<UsuarioDetalle>.Failure(notificacion);

                var usuarioDetalle = multi.Read<UsuarioDetalle>().FirstOrDefault();

                return ResponseFromService<UsuarioDetalle>.Success(usuarioDetalle, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<UsuarioDetalle>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<IEnumerable<UsuarioDetalle>>> ObtenerUsuariosPorIdUsuarioYRolAsync(Guid idUsuario, Guid idRol)
        {
            try
            {
                var parameters = new
                {
                    IdUsuario = idUsuario,
                    IdTipoUsuario = idRol
                };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "Usuarios_ObtenerUsuarios",
                    parameters
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<UsuarioDetalle>>.Failure(notificacion);

                var usuarios = multi.Read<UsuarioDetalle>().ToList();

                return ResponseFromService<IEnumerable<UsuarioDetalle>>.Success(usuarios, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<UsuarioDetalle>>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<string>> InactivarUsuarioAsync(Guid idUsuario, Guid idUsuarioSolicitante)
        {
            try
            {
                var parameters = new
                {
                    IdUsuario = idUsuario,
                    IdUsuarioSolicitante = idUsuarioSolicitante
                };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "Usuarios_InactivarUsuario",
                    parameters
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<string>.Failure(notificacion);

                return ResponseFromService<string>.Success("OK", notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<string?>> ObtenerFirmaPorIdUsuarioAsync(Guid idUsuario)
        {
            try
            {
                var parameters = new { IdUsuario = idUsuario };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "Usuarios_ObtenerFirmaPorIdUsuario",
                    parameters
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<string?>.Failure(notificacion);

                var firma = multi.Read<string?>().FirstOrDefault();

                return ResponseFromService<string?>.Success(firma, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string?>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<string>> CrearActualizarImagenFirmaAsync(UsuarioImagenRequest request)
        {
            if ((string.IsNullOrEmpty(request.Imagen) && string.IsNullOrEmpty(request.Firma)))
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return ResponseFromService<string>.Failure(error);
            }

            try
            {
                var parameters = new
                {
                    request.IdUsuario,
                    Imagen = string.IsNullOrEmpty(request.Imagen) ? null : request.Imagen,
                    Firma = string.IsNullOrEmpty(request.Firma) ? null : request.Firma
                };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "Usuarios_CrearActualizarImagenFirma",
                    parameters
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<string>.Failure(notificacion);

                return ResponseFromService<string>.Success(notificacion.Descripcion, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<string>> EliminarFirmaAsync(Guid idUsuario)
        {
            try
            {
                var parameters = new { IdUsuario = idUsuario };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "Usuarios_EliminarFirma",
                    parameters
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<string>.Failure(notificacion);

                return ResponseFromService<string>.Success(notificacion.Descripcion, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<string?>> ObtenerImagenPorIdUsuarioAsync(Guid idUsuario)
        {
            try
            {
                var parameters = new { IdUsuario = idUsuario };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "Usuarios_ObtenerImagenPorIdUsuario",
                    parameters
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<string?>.Failure(notificacion);

                var imagen = multi.Read<string?>().FirstOrDefault();

                return ResponseFromService<string?>.Success(imagen, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string?>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<string>> EliminarImagenAsync(Guid idUsuario)
        {
            try
            {
                var parameters = new { IdUsuario = idUsuario };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "Usuarios_EliminarImagen",
                    parameters
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<string>.Failure(notificacion);

                return ResponseFromService<string>.Success("Imagen eliminada con éxito.", notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<bool>> ValidateUserCredentialsAsync(string usr, string password)
        {
            try
            {
                var parameters = new { Usr = usr };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "Usuarios_GetPasswordHashByUsername",
                    parameters
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<bool>.Failure(notificacion);

                var hash = multi.Read<string?>().FirstOrDefault();
                var valido = !string.IsNullOrEmpty(hash) && _cifradoHelper.VerifyPassword(password, hash);

                return ResponseFromService<bool>.Success(valido, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<UsuarioDetalle>> GetUsuarioByEmailAsync(string email)
        {
            try
            {
                var parameters = new { Email = email };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "Usuaruios_ValidarCorreoUsuario",
                    parameters
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<UsuarioDetalle>.Failure(notificacion);

                var usuario = multi.Read<UsuarioDetalle>().FirstOrDefault();

                return ResponseFromService<UsuarioDetalle>.Success(usuario, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<UsuarioDetalle>.Exeption(ex, error);
            }
        }        
    }
}
