using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using RMD.Interface.Security;

namespace RMD.Service.ServiciosInternos
{
    public class DapperService : IDapperService
    {
        private readonly string _connectionString;

        public DapperService(string connectionString)
        {
            _connectionString = connectionString;
        }

        private async Task<SqlConnection> GetOpenConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.StoredProcedure)
        {
            using var connection = await GetOpenConnectionAsync();
            return await connection.QueryAsync<T>(sql, param, commandType: commandType);
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.StoredProcedure)
        {
            using var connection = await GetOpenConnectionAsync();
            return await connection.QueryFirstOrDefaultAsync<T>(sql, param, commandType: commandType);
        }

        public async Task<int> ExecuteAsync(string sql, object? param = null, CommandType commandType = CommandType.StoredProcedure)
        {
            using var connection = await GetOpenConnectionAsync();
            return await connection.ExecuteAsync(sql, param, commandType: commandType);
        }

        public async Task<SqlMapper.GridReader> QueryMultipleAsync(
            string sql,
            object? param = null,
            CommandType commandType = CommandType.StoredProcedure)
        {
            var connection = await GetOpenConnectionAsync();
            return await connection.QueryMultipleAsync(sql, param, commandType: commandType);
        }

        public async Task<SqlMapper.GridReader> QueryMultipleAsync(
            string sql,
            object? param,
            SqlConnection connection,
            SqlTransaction? transaction,
            CommandType commandType)
        {
            return await connection.QueryMultipleAsync(sql, param, transaction, null, commandType);
        }


    }
}
