using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace ServiceFabric.Utils.Http.Error
{
    public class SqlErrorStore : IErrorStore
    {
        private readonly IDbConnection _dbConnection;

        public SqlErrorStore(string connectionString)
        {
            _dbConnection = new SqlConnection(connectionString);
        }

        public async Task<int> AddAsync(Error error)
        {
            _dbConnection.Open();

            var result = await _dbConnection.ExecuteAsync(
                "INSERT INTO [dbo].[Error] (Id, ApplicationName, ApplicationVersion, MachineName, " +
                "Created, Type, Host, Url, HttpMethod, HttpStatusCode, IpAddress, Source, " +
                "Message, Detail, Sql, ErrorHash, FullJson) " +
                "VALUES (@Id, @ApplicationName, @ApplicationVersion, @MachineName, @Created, @Type, " +
                "@Host, @Url, @HttpMethod, @HttpStatusCode, @IpAddress, @Source, @Message, @Detail, " +
                "@Sql, @ErrorHash, @FullJson)",
                new
                {
                    error.Id,
                    error.ApplicationName,
                    error.ApplicationVersion,
                    error.MachineName,
                    error.Created,
                    error.Type,
                    error.Host,
                    error.Url,
                    error.HttpMethod,
                    error.HttpStatusCode,
                    error.IpAddress,
                    error.Source,
                    error.Message,
                    error.Detail,
                    error.Sql,
                    error.ErrorHash,
                    error.FullJson
                });

            _dbConnection.Close();

            return result;
        }
    }
}