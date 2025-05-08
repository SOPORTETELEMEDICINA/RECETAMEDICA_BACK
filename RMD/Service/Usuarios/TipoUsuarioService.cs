using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Extensions;
using RMD.Interface.Notificaciones;
using RMD.Interface.Usuarios;
using RMD.Models.Responses;
using RMD.Models.Usuarios;

namespace RMD.Service.Usuarios
{
    public class TipoUsuarioService : ITipoUsuarioService
    {
        private readonly UsuariosDBContext _context;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly string _connectionString;

        public TipoUsuarioService(
            UsuariosDBContext context,
            ICatalogoNotificacionService catalogoNotificacionService,
            string connectionString)
        {
            _context = context;
            _catalogoNotificacionService = catalogoNotificacionService;
            _connectionString = connectionString;
        }

        public async Task<ResponseFromService<IEnumerable<TipoUsuario>>> GetAllTipoUsuarioAsync()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Cat_GetAllTipoUsuario", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using var reader = await command.ExecuteReaderAsync();

                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                {
                    return ResponseFromService<IEnumerable<TipoUsuario>>.Failure(notificacion);
                }

                var tipos = new List<TipoUsuario>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tipos.Add(TipoUsuario.FromDataReader(reader));
                    }
                }

                return ResponseFromService<IEnumerable<TipoUsuario>>.Success(tipos, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<TipoUsuario>>.Exeption(ex, error);
            }
        }


        public async Task<ResponseFromService<TipoUsuario>> GetTipoUsuarioByIdAsync(Guid id)
        {
            try
            {
                var parameter = new SqlParameter("@IdTipoUsuario", id);

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Cat_GetTipoUsuarioById", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(parameter);

                using var reader = await command.ExecuteReaderAsync();

                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                {
                    return ResponseFromService<TipoUsuario>.Failure(notificacion);
                }

                TipoUsuario tipo = null;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    tipo = TipoUsuario.FromDataReader(reader);
                }

                return ResponseFromService<TipoUsuario>.Success(tipo, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<TipoUsuario>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> CreateTipoUsuarioAsync(TipoUsuario tipoUsuario)
        {
            try
            {
                var parameter = new SqlParameter("@TipoUsuario", SqlDbType.Structured)
                {
                    TypeName = "dbo.TipoUsuarioType",
                    Value = new List<TipoUsuario> { tipoUsuario }.ToDataTable()
                };

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Cat_CreateTipoUsuario", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(parameter);

                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                return notificacion.ToastType.ToUpperInvariant() == "ERROR"
                    ? ResponseFromService<string>.Failure(notificacion)
                    : ResponseFromService<string>.Success(notificacion.Descripcion, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> UpdateTipoUsuarioAsync(TipoUsuario tipoUsuario)
        {
            try
            {
                var parameter = new SqlParameter("@TipoUsuario", SqlDbType.Structured)
                {
                    TypeName = "dbo.TipoUsuarioType",
                    Value = new List<TipoUsuario> { tipoUsuario }.ToDataTable()
                };

                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("sp_Cat_UpdateTipoUsuario", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(parameter);

                using var reader = await command.ExecuteReaderAsync();
                // Recibe el código de notificación retornado por el SP
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                return notificacion.ToastType.ToUpperInvariant() == "ERROR"
                    ? ResponseFromService<string>.Failure(notificacion)
                    : ResponseFromService<string>.Success(notificacion.Descripcion, notificacion);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }

    }
}
