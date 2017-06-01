using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ServiceFabric.Utils.Http.Logging
{
    /// <summary>
    /// Logs a <see cref="Request"/> or <see cref="Response"/> to an MSSQL Database.
    /// </summary>
    public class SqlLogStore : ILogStore
    {
        private readonly IDbConnection _dbConnection;

        /// <summary>
        /// Creates a new instance of <see cref="SqlLogStore"/> with the specified <paramref name="connectionString"/>.
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlLogStore(string connectionString)
        {
            _dbConnection = new SqlConnection(connectionString);
        }

        /// <summary>
        /// Add a <see cref="Request"/> to the database.
        /// </summary>
        /// <param name="request">The <see cref="Request"/>.</param>
        /// <returns>True on success; otherwise false</returns>
        public async Task<bool> AddAsync(Request request)
        {
            _dbConnection.Open();

            var result = await _dbConnection.ExecuteAsync(
                @"INSERT INTO [dbo].[Request] 
                (Id, Username, ApplicationName, ApplicationVersion, MachineName, Created, Url, HttpMethod, IpAddress, Body, QueryString, FullJson) 
                VALUES 
                (@Id, @Username, @ApplicationName, @ApplicationVersion, @MachineName, @Created, @Url, @HttpMethod, @IpAddress, @Body, @QueryString, @FullJson)",
                new
                {
                    request.Id,
                    request.Username,
                    request.ApplicationName,
                    request.ApplicationVersion,
                    request.MachineName,
                    request.Created,
                    request.Url,
                    request.HttpMethod,
                    request.IpAddress,
                    request.Body,
                    QueryString = request.QueryString.ToString(),
                    request.FullJson
                });

            _dbConnection.Close();
            return result > 0;
        }

        /// <summary>
        /// Add a <see cref="Response"/> to the database.
        /// </summary>
        /// <param name="response">The <see cref="Response"/>.</param>
        /// <returns>True on success; otherwise false</returns>
        public async Task<bool> AddAsync(Response response)
        {
            _dbConnection.Open();

            var result = await _dbConnection.ExecuteAsync(
                @"INSERT INTO [dbo].[Response] 
                (Id, RequestId, ApplicationName, ApplicationVersion, MachineName, Created, HttpStatusCode, ElapsedMilliseconds, FullJson) 
                VALUES 
                (@Id, @RequestId, @ApplicationName, @ApplicationVersion, @MachineName, @Created, @HttpStatusCode, @ElapsedMilliseconds, @FullJson)",
                new
                {
                    response.Id,
                    response.RequestId,
                    response.ApplicationName,
                    response.ApplicationVersion,
                    response.MachineName,
                    response.Created,
                    response.HttpStatusCode,
                    response.ElapsedMilliseconds,
                    response.FullJson
                });

            _dbConnection.Close();
            return result > 0;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (null == _dbConnection)
                return;

            if (_dbConnection.State != ConnectionState.Closed)
            {
                _dbConnection.Close();
            }

            _dbConnection.Dispose();
        }
    }
}
