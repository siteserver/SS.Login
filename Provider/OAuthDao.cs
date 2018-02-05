using System.Collections.Generic;
using SiteServer.Plugin;
using SS.Login.Model;

namespace SS.Login.Provider
{
    public static class OAuthDao
    {
        public const string TableName = "ss_oauth";

        public static List<TableColumn> Columns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(OAuthInfo.Id),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(OAuthInfo.UserName),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(OAuthInfo.Source),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(OAuthInfo.UniqueId),
                DataType = DataType.VarChar,
                DataLength = 200
            }
        };

        private static string ConnectionString => Main.Instance.ConnectionString;
        private static IDataApi DataApi => Main.Instance.DataApi;

        public static int Insert(OAuthInfo loginInfo)
        {
            string sqlString = $@"INSERT INTO {TableName}
(
    {nameof(OAuthInfo.UserName)}, 
    {nameof(OAuthInfo.Source)}, 
    {nameof(OAuthInfo.UniqueId)}
) VALUES (
    @{nameof(OAuthInfo.UserName)}, 
    @{nameof(OAuthInfo.Source)},
    @{nameof(OAuthInfo.UniqueId)}
)";

            var parameters = new[]
            {
                DataApi.GetParameter($"@{nameof(OAuthInfo.UserName)}", loginInfo.UserName),
                DataApi.GetParameter($"@{nameof(OAuthInfo.Source)}", loginInfo.Source),
                DataApi.GetParameter($"@{nameof(OAuthInfo.UniqueId)}", loginInfo.UniqueId)
            };

            return DataApi.ExecuteNonQueryAndReturnId(TableName, nameof(OAuthInfo.Id), ConnectionString, sqlString, parameters);
        }

        public static void Delete(string userName, string source)
        {
            string sqlString =
                $"DELETE FROM {TableName} WHERE {nameof(OAuthInfo.UserName)} = @{nameof(OAuthInfo.UserName)} AND {nameof(OAuthInfo.Source)} = @{nameof(OAuthInfo.Source)}";

            var parameters = new[]
            {
                DataApi.GetParameter($"@{nameof(OAuthInfo.UserName)}", userName),
                DataApi.GetParameter($"@{nameof(OAuthInfo.Source)}", source)
            };

            DataApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
        }

        public static string GetUserName(string source, string uniqueId)
        {
            var userName = string.Empty;

            string sqlString =
                $"SELECT {nameof(OAuthInfo.UserName)} FROM {TableName} WHERE {nameof(OAuthInfo.Source)} = @{nameof(OAuthInfo.Source)} AND {nameof(OAuthInfo.UniqueId)} = @{nameof(OAuthInfo.UniqueId)}";

            var parameters = new[]
            {
                DataApi.GetParameter($"@{nameof(OAuthInfo.Source)}", source),
                DataApi.GetParameter($"@{nameof(OAuthInfo.UniqueId)}", uniqueId)
            };

            using (var conn = DataApi.GetConnection(ConnectionString))
            {
                conn.Open();
                using (var rdr = DataApi.ExecuteReader(conn, sqlString, parameters))
                {
                    if (rdr.Read() && !rdr.IsDBNull(0))
                    {
                        userName = rdr.GetString(0);
                    }
                    rdr.Close();
                }
            }

            return userName;
        }
    }
}
