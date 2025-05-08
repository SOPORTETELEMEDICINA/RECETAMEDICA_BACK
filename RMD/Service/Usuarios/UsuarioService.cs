using RMD.Data;
using RMD.Extensions;
using RMD.Interface.Notificaciones;
using RMD.Interface.Usuarios;
using RMD.Models.Responses;
using RMD.Models.Usuarios;

namespace RMD.Service.Usuarios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly UsuariosDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        //private readonly Guid _IdUsuario;
        private readonly CifradoHelper _cifradoHelper;
        private readonly string _connectionString;

        public UsuarioService(
           IHttpContextAccessor httpContextAccessor,
           UsuariosDBContext context,
           CifradoHelper cifradoHelper,
           ICatalogoNotificacionService catalogoNotificacionService,
           string connectionString)  // Se inyecta la cadena
        {
            _context = context;
            _cifradoHelper = cifradoHelper;
            _httpContextAccessor = httpContextAccessor;
            _catalogoNotificacionService = catalogoNotificacionService;
            _connectionString = connectionString;
        }

        // Helper para leer el código de error desde el primer conjunto

        public async Task<ResponseFromService<IEnumerable<RequestUsuario>>> GetUsuariosByGEMPAsync(Guid idGEMP)
        {
            try
            {
                // Obtener los valores del token.
                var idUsuarioSolicitante = _httpContextAccessor.HttpContext.User.FindFirstValue("IdUsuario");
                var idRolSolicitante = _httpContextAccessor.HttpContext.User.FindFirstValue("IdRol");

                // Validar que se puedan convertir a Guid.
                if (!Guid.TryParse(idUsuarioSolicitante, out var parsedIdUsuario) ||
                    !Guid.TryParse(idRolSolicitante, out var parsedIdRol))
                {
                    var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO"); // "PROBLEMAS AL VALIDAR LOS CAMPOS DEL TOKEN."
                    return ResponseFromService<IEnumerable<RequestUsuario>>.Failure(notificacion);
                }

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Usuarios_GetUsuariosByGEMP", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdGEMP", idGEMP);
                command.Parameters.AddWithValue("@IdUsuarioSolicitante", parsedIdUsuario);
                command.Parameters.AddWithValue("@IdRolSolicitante", parsedIdRol);

                using var reader = await command.ExecuteReaderAsync();

                // Leer el primer conjunto para obtener el Código de Notificación.
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var spNotificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (spNotificacion.ToastType.ToUpperInvariant() == "ERROR")
                {
                    return ResponseFromService<IEnumerable<RequestUsuario>>.Failure(spNotificacion);
                }

                // 2) Moverte al segundo result set (la lista de usuarios)
                if (await reader.NextResultAsync())
                {
                    var usuarios = new List<RequestUsuario>();
                    // 3) Iterar filas
                    while (await reader.ReadAsync())
                    {
                        usuarios.Add(RequestUsuario.FromDataReader(reader));
                    }
                    return ResponseFromService<IEnumerable<RequestUsuario>>.Success(usuarios, spNotificacion);
                }
                else
                {
                    // No vino result set de usuarios
                    return ResponseFromService<IEnumerable<RequestUsuario>>.Success(Enumerable.Empty<RequestUsuario>(), spNotificacion);
                }
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA"); // EXCEPCIÓN DETECTADA
                return ResponseFromService<IEnumerable<RequestUsuario>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> CambiarPasswordAsync(Guid idUsuario, string nuevaPassword)
        {
            try
            {
                var nuevaPasswordCifrada = _cifradoHelper.HashPassword(nuevaPassword);

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "Usuarios_CambiarPassword";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@IdUsuario", idUsuario));
                command.Parameters.Add(new SqlParameter("@NuevaPassword", nuevaPasswordCifrada));

                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);


                var spNotificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (spNotificacion.ToastType.ToUpperInvariant() == "ERROR")
                {
                    return ResponseFromService<string>.Failure(spNotificacion);
                }
                else
                {
                    return ResponseFromService<string>.Success(spNotificacion.Descripcion, spNotificacion);
                }

            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA"); // EXCEPCIÓN DETECTADA
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<IEnumerable<RequestUsuario>>> GetUsuariosBySucursalAsync(Guid idSucursal)
        {
            try
            {
                // Obtener los valores del token
                var idUsuarioSolicitante = _httpContextAccessor.HttpContext.User.FindFirstValue("IdUsuario");
                var idRolSolicitante = _httpContextAccessor.HttpContext.User.FindFirstValue("IdRol");

                // Validar que los valores del token puedan convertirse a Guid
                if (!Guid.TryParse(idUsuarioSolicitante, out var parsedIdUsuario) ||
                    !Guid.TryParse(idRolSolicitante, out var parsedIdRol))
                {
                    var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO"); // "PROBLEMAS AL VALIDAR LOS CAMPOS DEL TOKEN."
                    return ResponseFromService<IEnumerable<RequestUsuario>>.Failure(notificacion);
                }

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Usuarios_GetUsuariosBySucursal", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@IdSucursal", idSucursal);
                command.Parameters.AddWithValue("@IdUsuarioSolicitante", parsedIdUsuario);
                command.Parameters.AddWithValue("@IdRolSolicitante", parsedIdRol);

                using var reader = await command.ExecuteReaderAsync();

                // Leer el primer conjunto: se espera el Código de Notificación.
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var spNotificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (spNotificacion.ToastType.ToUpperInvariant() == "ERROR")
                {
                    return ResponseFromService<IEnumerable<RequestUsuario>>.Failure(spNotificacion);
                }                

                var usuarios = new List<RequestUsuario>();
                while (await reader.ReadAsync())
                {
                    usuarios.Add(RequestUsuario.FromDataReader(reader));
                }

                return ResponseFromService<IEnumerable<RequestUsuario>>.Success(usuarios, spNotificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA"); // EXCEPCIÓN DETECTADA
                return ResponseFromService<IEnumerable<RequestUsuario>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<object>> AddUsuarioAsync(UsuarioCreate usuario, Guid idRol, string rol, Guid idGemp, Guid idSucursal)
        {
            try
            {
                // Cifrar la contraseña si se proporciona.
                if (!string.IsNullOrEmpty(usuario.Password))
                {
                    usuario.Password = _cifradoHelper.HashPassword(usuario.Password);
                }

                // Preparar el parámetro de tabla.
                var parameter = new SqlParameter("@UsuarioData", SqlDbType.Structured)
                {
                    TypeName = "dbo.UsuarioCreateTableType",
                    Value = new List<UsuarioCreate> { usuario }.ToDataTable()
                };

                // Generar un nuevo Id para el usuario.
                Guid _IdUsuario = Guid.NewGuid();
                var idUsuarioParameter = new SqlParameter("@IdUsuario", SqlDbType.UniqueIdentifier) { Value = _IdUsuario };
                var idRolParameter = new SqlParameter("@IdRol", SqlDbType.UniqueIdentifier) { Value = idRol };
                var idGempParameter = new SqlParameter("@IdGemp", SqlDbType.UniqueIdentifier) { Value = idGemp };
                var idSucursalParameter = new SqlParameter("@IdSucursal", SqlDbType.UniqueIdentifier) { Value = idSucursal };

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Usuarios_CreateUSR", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(parameter);
                command.Parameters.Add(idUsuarioParameter);
                command.Parameters.Add(idRolParameter);
                command.Parameters.Add(idGempParameter);
                command.Parameters.Add(idSucursalParameter);

                using var reader = await command.ExecuteReaderAsync();

                // Leer el código de notificación (primer conjunto).
                int codigoError = await ValidationHelper.ReadErrorCodeAsync(reader);
                var spNotificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoError);
                if (spNotificacion.ToastType.ToUpperInvariant() == "ERROR")
                {
                    return ResponseFromService<object>.Failure(spNotificacion);
                }

                Guid generatedIdUsuario = Guid.Empty;
                if (await reader.ReadAsync())
                {
                    generatedIdUsuario = reader.GetGuid(0);
                }
                return ResponseFromService<object>.Success(generatedIdUsuario, spNotificacion);
            }
            catch (SqlException ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<object>.Exeption(ex, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<object>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> UpdateUsuarioAsync(Usuario usuario, Guid idUsuarioSolicitante)
        {
            try
            {
                // Validar que el usuario no sea nulo y tenga un Id válido.
                if (usuario == null || usuario.IdUsuario == Guid.Empty)
                {
                    var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                    return ResponseFromService<string>.Failure(notificacion);
                }

                // Si se proporcionó contraseña, cifrarla; si no, asignarla a null.
                if (!string.IsNullOrEmpty(usuario.Password))
                {
                    usuario.Password = _cifradoHelper.HashPassword(usuario.Password);
                }
                else
                {
                    usuario.Password = null;
                }

                // Crear el parámetro para la tabla del usuario.
                var usuarioParam = new SqlParameter("@UsuarioTable", SqlDbType.Structured)
                {
                    TypeName = "dbo.UsuarioTableType",
                    Value = new List<Usuario> { usuario }.ToDataTable()
                };

                var idUsuarioSolicitanteParam = new SqlParameter("@IdUsuarioSolicitante", idUsuarioSolicitante);

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Usuarios_UpdateUsuario", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(usuarioParam);
                command.Parameters.Add(idUsuarioSolicitanteParam);

                using var reader = await command.ExecuteReaderAsync();

                // Leer el código de notificación (primer conjunto).
                int codigoError = await ValidationHelper.ReadErrorCodeAsync(reader);
                var spNotificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoError);

                if (spNotificacion.ToastType.ToUpperInvariant() == "ERROR")
                {
                    return ResponseFromService<string>.Failure(spNotificacion);
                }
                else
                {
                    return ResponseFromService<string>.Success(null, spNotificacion);
                }
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<UsuarioDetalle>> GetUsuarioByUsernameAsync(string username)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Usuarios_GetUsuarioByUsername", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@Usr", username));

                using var reader = await command.ExecuteReaderAsync();

                // Leer el primer conjunto para obtener el código de notificación.
                int codigoError = await ValidationHelper.ReadErrorCodeAsync(reader);
                var spNotificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoError);
                if (spNotificacion.ToastType.ToUpperInvariant() == "ERROR")
                {
                    return ResponseFromService<UsuarioDetalle>.Failure(spNotificacion);
                }
                UsuarioDetalle usuarioDetalle = null;
            // Avanzar al segundo result set
            if (await reader.NextResultAsync())
            {
                if (await reader.ReadAsync())
                {
                    usuarioDetalle = UsuarioDetalle.FromDataReader((SqlDataReader)reader);
                }
            }
                return ResponseFromService<UsuarioDetalle>.Success(usuarioDetalle, spNotificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<UsuarioDetalle>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<UsuarioDetalle>> GetUsuarioPacienteByUsernameAsync(string username)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Usuarios_GetUsuarioPacienteByUsername", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@Usr", username));

                using var reader = await command.ExecuteReaderAsync();

                // Leer el primer conjunto para obtener el código de notificación.
                int codigoError = await ValidationHelper.ReadErrorCodeAsync(reader);
                var spNotificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoError);
                if (spNotificacion.ToastType.ToUpperInvariant() == "ERROR")
                {
                    return ResponseFromService<UsuarioDetalle>.Failure(spNotificacion);
                }
                UsuarioDetalle usuarioDetalle = null;
                // Avanzar al segundo result set
                if (await reader.NextResultAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        usuarioDetalle = UsuarioDetalle.FromDataReader((SqlDataReader)reader);
                    }
                }
                return ResponseFromService<UsuarioDetalle>.Success(usuarioDetalle, spNotificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<UsuarioDetalle>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<IEnumerable<UsuarioDetalle>>> ObtenerUsuariosPorIdUsuarioYRolAsync(Guid idUsuario, Guid idRol)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Usuarios_ObtenerUsuarios", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@IdUsuario", idUsuario));
                command.Parameters.Add(new SqlParameter("@IdTipoUsuario", idRol));

                using var reader = await command.ExecuteReaderAsync();

                // Leer código de notificación
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var spNotificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (spNotificacion.ToastType.ToUpperInvariant() == "ERROR")
                {
                    return ResponseFromService<IEnumerable<UsuarioDetalle>>.Failure(spNotificacion);
                }

                // Segundo conjunto: usuarios
                var usuarios = new List<UsuarioDetalle>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        usuarios.Add(UsuarioDetalle.FromDataReader((SqlDataReader)reader));
                    }
                }

                return ResponseFromService<IEnumerable<UsuarioDetalle>>.Success(usuarios, spNotificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<UsuarioDetalle>>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> InactivarUsuarioAsync(Guid idUsuario, Guid idUsuarioSolicitante)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Usuarios_InactivarUsuario", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                command.Parameters.AddWithValue("@IdUsuarioSolicitante", idUsuarioSolicitante);

                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);

                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<string>.Failure(notificacion);

                return ResponseFromService<string>.Success("OK", notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string?>> ObtenerFirmaPorIdUsuarioAsync(Guid idUsuario)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Usuarios_ObtenerFirmaPorIdUsuario", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);

                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<string?>.Failure(notificacion);

                if (!await reader.NextResultAsync())
                    return ResponseFromService<string?>.Success(null, notificacion);

                string? firma = null;
                if (await reader.ReadAsync())
                    firma = reader["Firma"] as string;

                return ResponseFromService<string?>.Success(firma, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string?>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> CrearActualizarImagenFirmaAsync(UsuarioImagenRequest request)
        {
            if (request == null || (string.IsNullOrEmpty(request.Imagen) && string.IsNullOrEmpty(request.Firma)))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return ResponseFromService<string>.Failure(notificacion);
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Usuarios_CrearActualizarImagenFirma", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdUsuario", request.IdUsuario);
                command.Parameters.AddWithValue("@Imagen", string.IsNullOrEmpty(request.Imagen) ? DBNull.Value : request.Imagen);
                command.Parameters.AddWithValue("@Firma", string.IsNullOrEmpty(request.Firma) ? DBNull.Value : request.Firma);

                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await  ValidationHelper.ReadErrorCodeAsync(reader);

                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<string>.Failure(notificacion);

                return ResponseFromService<string>.Success(notificacion.Descripcion, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> EliminarFirmaAsync(Guid idUsuario)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Usuarios_EliminarFirma", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);

                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await  ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<string>.Failure(notificacion);

                return ResponseFromService<string>.Success(notificacion.Descripcion, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string?>> ObtenerImagenPorIdUsuarioAsync(Guid idUsuario)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Usuarios_ObtenerImagenPorIdUsuario", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);

                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<string?>.Failure(notificacion);

                if (!await reader.NextResultAsync())
                    return ResponseFromService<string?>.Success(null, notificacion);

                string? imagen = null;
                if (await reader.ReadAsync())
                    imagen = reader.IsDBNull(0) ? null : reader.GetString(0);

                return ResponseFromService<string?>.Success(imagen, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string?>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> EliminarImagenAsync(Guid idUsuario)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Usuarios_EliminarImagen", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdUsuario", idUsuario);

                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<string>.Failure(notificacion);

                return ResponseFromService<string>.Success("Imagen eliminada con éxito.", notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<bool>> ValidateUserCredentialsAsync(string usr, string password)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Usuarios_GetPasswordHashByUsername", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@Usr", usr));

                using var reader = await command.ExecuteReaderAsync();

                // Leer código de notificación
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                {
                    return ResponseFromService<bool>.Failure(notificacion);
                }

                if (!await reader.NextResultAsync() || !await reader.ReadAsync())
                {
                    return ResponseFromService<bool>.Success(false, notificacion);
                }

                var hash = reader.IsDBNull(0) ? null : reader.GetString(0);
                var valido = !string.IsNullOrEmpty(hash) && _cifradoHelper.VerifyPassword(password, hash);
                return ResponseFromService<bool>.Success(valido, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<UsuarioDetalle>> GetUsuarioByEmailAsync(string email)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "Usuaruios_ValidarCorreoUsuario";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Email", email));

                using var reader = await command.ExecuteReaderAsync();

                // Leer el código de notificación
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var spNotificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (spNotificacion.ToastType.ToUpperInvariant() == "ERROR")
                {
                    return ResponseFromService<UsuarioDetalle>.Failure(spNotificacion);
                }

                UsuarioDetalle? usuario = null;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    usuario = UsuarioDetalle.FromDataReader((SqlDataReader)reader);
                }

                return ResponseFromService<UsuarioDetalle>.Success(usuario, spNotificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<UsuarioDetalle>.Exeption(ex, error);
            }
        }

    }
}

