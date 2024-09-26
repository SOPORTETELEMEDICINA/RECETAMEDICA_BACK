using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Extensions;
using RMD.Interface.Usuarios;
using RMD.Models.Usuarios;
using System.Data;
using System.Security.Claims;

namespace RMD.Service.Usuarios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly UsuariosDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _username;
        private readonly Guid _role;
        private readonly Guid _GEMP;
        private readonly Guid _IdSucursal;
        private readonly Guid _IdUsuario;
        private static readonly Guid SuperAdminId = Guid.Parse("7905213C-B0CB-4D42-A997-20094EF41F9C");
        private static readonly Guid MedicoId = Guid.Parse("DE5DFDDC-F6CC-4B7F-B805-286732501E57");
        private static readonly Guid ResponsableFarmaciaId = Guid.Parse("CD5FF082-0BF4-4848-BA0C-38EDA4921954");
        private static readonly Guid PacienteId = Guid.Parse("635A6E50-D6B0-4C0C-BC0A-96B617A21BEE");
        private static readonly Guid EmpleadoFarmaciaId = Guid.Parse("D0E20BC6-C1E8-42C9-B80C-A9C92D55D3E9");
        private static readonly Guid SupervisorSucursalesId = Guid.Parse("68C87CA4-2499-4A9C-B0DC-EEE99B7078BF");
        private readonly CifradoHelper _cifradoHelper;

        public UsuarioService(IHttpContextAccessor httpContextAccessor, UsuariosDBContext context, CifradoHelper cifradoHelper)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _cifradoHelper = cifradoHelper;
            _username = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            _role = Guid.TryParse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role), out var roleGuid) ? roleGuid : Guid.Empty;
            _GEMP = Guid.TryParse(_httpContextAccessor.HttpContext.User.FindFirst("GEMP")?.Value, out var gempGuid) ? gempGuid : Guid.Empty;
            _IdSucursal = Guid.TryParse(_httpContextAccessor.HttpContext.User.FindFirst("IdSucursal")?.Value, out var sucursalGuid) ? sucursalGuid : Guid.Empty;
            _IdUsuario = Guid.TryParse(_httpContextAccessor.HttpContext.User.FindFirst("IdUsuario")?.Value, out var usuarioGuid) ? usuarioGuid : Guid.Empty;

        }

        public async Task<Usuario?> GetUsuarioByIdAsync(Guid idUsuario)
        {
            var results = await _context.Usuarios
                .FromSqlRaw("EXEC Usuarios_GetUsuarioById @IdUsuario", new SqlParameter("@IdUsuario", idUsuario))
                .ToListAsync();

            return results.FirstOrDefault();
        }

        public async Task<IEnumerable<Usuario>> GetUsuariosByAsentamientoAsync(int idAsentamiento)
        {
            var results = await _context.Usuarios
                .FromSqlRaw("EXEC Usuarios_GetUsuariosByAsentamiento @IdAsentamiento", new SqlParameter("@IdAsentamiento", idAsentamiento))
                .ToListAsync();

            return results;
        }

        public async Task<IEnumerable<Usuario>> GetUsuariosByGEMPAsync(Guid idGEMP)
        {
            var results = await _context.Usuarios
                .FromSqlRaw("EXEC Usuarios_GetUsuariosByGEMP @IdGEMP", new SqlParameter("@IdGEMP", idGEMP))
                .ToListAsync();

            return results;
        }

        public async Task<IEnumerable<Usuario>> GetUsuariosBySucursalAsync(Guid idSucursal)
        {
            var results = await _context.Usuarios
                .FromSqlRaw("EXEC Usuarios_GetUsuariosBySucursal @IdSucursal", new SqlParameter("@IdSucursal", idSucursal))
                .ToListAsync();

            return results;
        }

        public async Task<IEnumerable<Usuario>> GetUsuariosByTipoUsuarioAsync(Guid idTipoUsuario)
        {
            var results = await _context.Usuarios
                .FromSqlRaw("EXEC Usuarios_GetUsuariosByTipoUsuario @IdTipoUsuario", new SqlParameter("@IdTipoUsuario", idTipoUsuario))
                .ToListAsync();

            return results;
        }

        public async Task<IEnumerable<UsuarioSucursal>> GetUsuarioSucursalByIdAsync(Guid idSucursal)
        {
            var results = await _context.UsuarioSucursales
                .FromSqlRaw("EXEC Usuarios_GetUsuarioSucursalById @IdSucursal", new SqlParameter("@IdSucursal", idSucursal))
                .ToListAsync();

            return results;
        }

        public async Task<(string Message, Guid IdUsuario)> AddUsuarioAsync(UsuarioCreate usuario, Guid idRol, string rol, Guid idGemp, Guid idSucursal)
        {
            try
            {
                // Cifrar la contraseña del usuario antes de enviarla al procedimiento almacenado
                if (!string.IsNullOrEmpty(usuario.Password))
                {
                    usuario.Password = _cifradoHelper.HashPassword(usuario.Password);
                }

                var parameter = new SqlParameter("@UsuarioData", SqlDbType.Structured)
                {
                    TypeName = "dbo.UsuarioCreateTableType",
                    Value = new List<UsuarioCreate> { usuario }.ToDataTable()
                };

                var idUsuarioParameter = new SqlParameter("@IdUsuario", SqlDbType.UniqueIdentifier)
                {
                    Value = _IdUsuario
                };

                var idRolParameter = new SqlParameter("@IdRol", SqlDbType.UniqueIdentifier)
                {
                    Value = idRol
                };

                var idGempParameter = new SqlParameter("@IdGemp", SqlDbType.UniqueIdentifier)
                {
                    Value = idGemp
                };

                var idSucursalParameter = new SqlParameter("@IdSucursal", SqlDbType.UniqueIdentifier)
                {
                    Value = idSucursal
                };

                var outputParameter = new SqlParameter("@OutputMessage", SqlDbType.NVarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };

                var generatedIdUsuarioParameter = new SqlParameter("@GeneratedIdUsuario", SqlDbType.UniqueIdentifier)
                {
                    Direction = ParameterDirection.Output
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC Usuarios_CreateUSR @UsuarioData, @IdUsuario, @IdRol, @IdGemp, @IdSucursal, @OutputMessage OUTPUT, @GeneratedIdUsuario OUTPUT",
                    parameter, idUsuarioParameter, idRolParameter, idGempParameter, idSucursalParameter, outputParameter, generatedIdUsuarioParameter
                );

                // Convertir el valor de salida a los tipos correspondientes
                var message = outputParameter.Value.ToString() ?? "Operación completada, pero no se recibió un mensaje de respuesta.";
                var generatedIdUsuario = (Guid)generatedIdUsuarioParameter.Value;

                return (message, generatedIdUsuario);
            }
            catch (SqlException ex)
            {
                return ($"Error al ejecutar el procedimiento almacenado: {ex.Message}", Guid.Empty);
            }
            catch (Exception ex)
            {
                return ($"Ocurrió un error inesperado: {ex.Message}", Guid.Empty);
            }
        }


        public async Task<(string mensaje, bool exito)> UpdateUsuarioAsync(Usuario usuario, Guid idUsuarioSolicitante)
        {
            try
            {
                // Cifrar la contraseña si fue provista
                if (!string.IsNullOrEmpty(usuario.Password))
                {
                    usuario.Password = _cifradoHelper.HashPassword(usuario.Password);
                }

                // Crear parámetros para el SP
                var usuarioParam = new SqlParameter("@UsuarioTable", SqlDbType.Structured)
                {
                    TypeName = "dbo.UsuarioTableType",
                    Value = new List<Usuario> { usuario }.ToDataTable() // Convertir a DataTable
                };

                var idUsuarioSolicitanteParam = new SqlParameter("@IdUsuarioSolicitante", idUsuarioSolicitante);
                var outputMessageParam = new SqlParameter("@OutputMessage", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };

                // Ejecutar el procedimiento almacenado
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC Usuarios_UpdateUsuario @UsuarioTable, @IdUsuarioSolicitante, @OutputMessage OUTPUT",
                    usuarioParam, idUsuarioSolicitanteParam, outputMessageParam
                );

                var mensaje = outputMessageParam.Value.ToString();
                var exito = mensaje.Contains("éxito");

                return (mensaje, exito);
            }
            catch (Exception ex)
            {
                return ($"Ocurrió un error inesperado: {ex.Message}", false);
            }
        }


        public async Task<bool> ValidateUserCredentialsAsync(string usr, string password)
        {
            try
            {
                string hashResult = null;

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "EXEC Usuarios_GetPasswordHashByUsername @Usr";
                    command.Parameters.Add(new SqlParameter("@Usr", usr));

                    _context.Database.OpenConnection();
                    var result = await command.ExecuteScalarAsync();
                    hashResult = result as string;
                }

                if (string.IsNullOrEmpty(hashResult))
                {
                    return false;
                }

                return _cifradoHelper.VerifyPassword(password, hashResult);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al validar las credenciales del usuario.", ex);
            }
        }

        public async Task<UsuarioDetalle?> GetUsuarioByUsernameAsync(string username)
        {
            var parameters = new SqlParameter[]
            {
                new("@Usr", username)
            };

            var results = await _context.UsuarioDetalle
                .FromSqlRaw("EXEC Usuarios_GetUsuarioByUsername @Usr", parameters)
                .ToListAsync();

            return results.FirstOrDefault();
        }

       

        public async Task<UsuarioDetalle> GetUsuarioByEmailAsync(string email)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
            new("@Email", email)
                };

                var results = await _context.UsuarioDetalle
                    .FromSqlRaw("EXEC Usuaruios_ValidarCorreoUsuario @Email", parameters)
                    .ToListAsync();

                return results.FirstOrDefault();
            }
            catch (SqlException ex)
            {
                // Aquí puedes registrar el error o lanzar una excepción personalizada
                // Puedes utilizar un logger si lo tienes configurado
                Console.WriteLine($"Error ejecutando el SP: {ex.Message}");

                // Puedes retornar null o lanzar una excepción para manejarlo en capas superiores
                return null;
            }
            catch (Exception ex)
            {
                // Para capturar cualquier otro tipo de error
                Console.WriteLine($"Error general: {ex.Message}");

                return null;
            }
        }

        // Implementación del método que ejecuta el SP con IdUsuario e IdRol
        public async Task<IEnumerable<UsuarioDetalle>> ObtenerUsuariosPorIdUsuarioYRolAsync(Guid idUsuario, Guid idRol)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new ("@IdUsuario", idUsuario),
                    new ("@IdTipoUsuario", idRol)
                };

                var resultados = await _context.UsuarioDetalle
                    .FromSqlRaw("EXEC Usuarios_ObtenerUsuarios @IdTipoUsuario, @IdUsuario", parameters)
                    .ToListAsync();


                return resultados;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener usuarios", ex);
            }
        }

        public async Task<string> CrearActualizarImagenFirmaAsync(UsuarioImagenRequest request)
        {
            var parametros = new[]
            {
            new SqlParameter("@IdUsuario", request.IdUsuario),
            new SqlParameter("@Imagen", (object)request.Imagen ?? DBNull.Value),
            new SqlParameter("@Firma", (object)request.Firma ?? DBNull.Value)
        };

            var outputMessage = new SqlParameter("@OutputMessage", SqlDbType.NVarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC UsuarioCrearActualizarImagenFirma @IdUsuario, @Imagen, @Firma, @OutputMessage OUTPUT",
                [.. parametros, outputMessage]
            );

            return outputMessage.Value.ToString();
        }

        public async Task<string?> ObtenerFirmaPorIdUsuarioAsync(Guid idUsuario)
        {
            var result = await _context.UsuarioImagenes
                .FromSqlRaw("EXEC ObtenerFirmaPorIdUsuario @IdUsuario", new SqlParameter("@IdUsuario", idUsuario))
                .Select(u => u.Firma)
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<string?> ObtenerImagenPorIdUsuarioAsync(Guid idUsuario)
        {
            var result = await _context.UsuarioImagenes
                .FromSqlRaw("EXEC ObtenerImagenPorIdUsuario @IdUsuario", new SqlParameter("@IdUsuario", idUsuario))
                .Select(u => u.Imagen)
                .FirstOrDefaultAsync();

            return result;
        }


        public async Task<IEnumerable<SucursalResponse>> GetSucursalesByUsuarioAsync(Guid idUsuario)
        {
            var param = new SqlParameter("@IdUsuario", SqlDbType.UniqueIdentifier)
            {
                Value = idUsuario
            };

            var sucursales = await _context.Set<SucursalResponse>()
                .FromSqlRaw("EXEC Usuarios_GetSucursalesByUsuario @IdUsuario", param)
                .ToListAsync();

            return sucursales;
        }

        public async Task<(string mensaje, bool exito)> InactivarUsuarioAsync(Guid idUsuario, Guid idUsuarioSolicitante)
        {
            try
            {
                var outputMessageParam = new SqlParameter("@OutputMessage", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                var idUsuarioParam = new SqlParameter("@IdUsuario", idUsuario);
                var idUsuarioSolicitanteParam = new SqlParameter("@IdUsuarioSolicitante", idUsuarioSolicitante);

                await _context.Database.ExecuteSqlRawAsync("EXEC Usuarios_InactivarUsuario @IdUsuario, @IdUsuarioSolicitante, @OutputMessage OUTPUT",
                    idUsuarioParam, idUsuarioSolicitanteParam, outputMessageParam);

                var mensaje = outputMessageParam.Value.ToString();
                var exito = mensaje.Contains("éxito");

                return (mensaje, exito);
            }
            catch (Exception ex)
            {
                return ($"Ocurrió un error inesperado: {ex.Message}", false);
            }
        }

        public async Task<(string mensaje, bool exito)> CambiarPasswordAsync(Guid idUsuario, string nuevaPassword)
        {
            try
            {
                // Cifrar la nueva contraseña
                var nuevaPasswordCifrada = _cifradoHelper.HashPassword(nuevaPassword);

                var outputMessageParam = new SqlParameter("@OutputMessage", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
                var idUsuarioParam = new SqlParameter("@IdUsuario", idUsuario);
                var nuevaPasswordParam = new SqlParameter("@NuevaPassword", nuevaPasswordCifrada);

                await _context.Database.ExecuteSqlRawAsync("EXEC Usuarios_CambiarPassword @IdUsuario, @NuevaPassword, @OutputMessage OUTPUT",
                    idUsuarioParam, nuevaPasswordParam, outputMessageParam);

                var mensaje = outputMessageParam.Value.ToString();
                var exito = mensaje.Contains("éxito");

                return (mensaje, exito);
            }
            catch (Exception ex)
            {
                return ($"Ocurrió un error inesperado: {ex.Message}", false);
            }
        }

    }


}

