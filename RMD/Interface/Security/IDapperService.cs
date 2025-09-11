using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace RMD.Interface.Security
{
    public interface IDapperService
    {
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.StoredProcedure);
        Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.StoredProcedure);
        Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object? param = null, CommandType commandType = CommandType.StoredProcedure);
        Task<int> ExecuteAsync(string sql, object? param = null, CommandType commandType = CommandType.StoredProcedure);

        Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object? param, SqlConnection connection, SqlTransaction? transaction, CommandType commandType);

    }
}
