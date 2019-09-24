using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MobileFueling.DB;
using Npgsql;

namespace MobileFueling.Api.Common.HealthChecks
{
    public class SqlConnectionHealthCheck : IHealthCheck
    {
        private static readonly string DefaultHealthQuery = "select 1";

        private string ConnectionString { get; }

        private string HealthQuery { get; }

        private int SqlType { get; }

        public SqlConnectionHealthCheck(string connectionString, int sqlType)
            : this(connectionString, sqlType, healthQuery: DefaultHealthQuery)
        {
        }

        public SqlConnectionHealthCheck(string connectionString, int sqlType, string healthQuery)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            HealthQuery = healthQuery;
            SqlType = sqlType;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                DbConnection connection = null;
                switch(SqlType)
                {
                    case 0:
                        connection = new SqlConnection(ConnectionString);
                        break;
                    case 1:
                        connection = new NpgsqlConnection(ConnectionString);
                        break;
                    default:
                        connection = new SqlConnection(ConnectionString);
                        break;
                }
                
                await connection.OpenAsync(cancellationToken);

                if (HealthQuery != null)
                {
                    var command = connection.CreateCommand();
                    command.CommandText = HealthQuery;

                    await command.ExecuteNonQueryAsync(cancellationToken);
                }

                connection.Dispose();
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(status: context.Registration.FailureStatus, exception: ex);
            }

            return HealthCheckResult.Healthy();
        }
    }
}