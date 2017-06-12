using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace ServiceFabric.Utils.Logging
{
    public class SqlErrorStore : IErrorStore
    {
        string _connectionString;
        public SqlErrorStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> AddAsync(Error error)
        {
            using (var dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();

                var result = await dbConnection.ExecuteAsync(
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

                dbConnection.Close();

                return result;
            }
        }
    }
}