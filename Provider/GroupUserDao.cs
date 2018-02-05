using System.Collections.Generic;
using SiteServer.Plugin;
using SS.Home.Model;

namespace SS.Home.Provider
{
    public static class GroupUserDao
    {
        public const string TableName = "ss_home_group_user";

        public static List<TableColumn> Columns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(GroupUserInfo.Id),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(GroupUserInfo.GroupId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(GroupUserInfo.UserName),
                DataType = DataType.VarChar,
                DataLength = 200
            }
        };

        private static string ConnectionString => Main.Instance.ConnectionString;
        private static IDataApi DataApi => Main.Instance.DataApi;

        public static void Insert(int groupId, string userName)
        {
            var sqlString = $@"INSERT INTO {TableName} (
                {nameof(GroupUserInfo.GroupId)}, {nameof(GroupUserInfo.UserName)}
            ) VALUES (
                @{nameof(GroupUserInfo.GroupId)}, @{nameof(GroupUserInfo.UserName)}
            )";

            var parameters = new[]
            {
                DataApi.GetParameter($"@{nameof(GroupUserInfo.GroupId)}", groupId),
                DataApi.GetParameter($"@{nameof(GroupUserInfo.UserName)}", userName)
            };

            DataApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
        }

        public static void Delete(int groupId)
        {
            var sqlString = $"DELETE FROM {TableName} WHERE {nameof(GroupUserInfo.GroupId)} = @{nameof(GroupUserInfo.GroupId)}";
            var parameters = new []
			{
                DataApi.GetParameter($"@{nameof(GroupUserInfo.GroupId)}", groupId)
            };

            DataApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
        }

        public static List<string> GetUserNameList(int groupId)
        {
            if (groupId == 0) return new List<string>();

            var list = new List<string>();

            var sqlString = $"SELECT {nameof(GroupUserInfo.UserName)} FROM {TableName} WHERE {nameof(GroupUserInfo.GroupId)} = @{nameof(GroupUserInfo.GroupId)} ORDER BY {nameof(GroupUserInfo.Id)}";

            var parameters = new[]
            {
                DataApi.GetParameter($"@{nameof(GroupUserInfo.GroupId)}", groupId)
            };

            using (var conn = DataApi.GetConnection(ConnectionString))
            {
                conn.Open();
                using (var rdr = DataApi.ExecuteReader(conn, sqlString, parameters))
                {
                    while (rdr.Read())
                    {
                        if (!rdr.IsDBNull(0))
                        {
                            list.Add(rdr.GetString(0));
                        }
                    }
                    rdr.Close();
                }
            }

            return list;
        }

        public static bool IsExists(int groupId, string userName)
        {
            var exists = false;

            var sqlString = $"SELECT {nameof(GroupUserInfo.Id)} FROM {TableName} WHERE {nameof(GroupUserInfo.GroupId)} = @{nameof(GroupUserInfo.GroupId)} AND {nameof(GroupUserInfo.UserName)} = @{nameof(GroupUserInfo.UserName)}";

            var parameters = new []
			{
                DataApi.GetParameter($"@{nameof(GroupUserInfo.GroupId)}", groupId),
                DataApi.GetParameter($"@{nameof(GroupUserInfo.UserName)}", userName)
            };

            using (var conn = DataApi.GetConnection(ConnectionString))
            {
                conn.Open();
                using (var rdr = DataApi.ExecuteReader(conn, sqlString, parameters))
                {
                    if (rdr.Read() && !rdr.IsDBNull(0))
                    {
                        exists = true;
                    }
                    rdr.Close();
                }
            }

            return exists;
        }

        public static int GetGroupId(string userName)
        {
            var groupId = 0;

            var sqlString = $"SELECT {nameof(GroupUserInfo.GroupId)} FROM {TableName} WHERE {nameof(GroupUserInfo.UserName)} = @{nameof(GroupUserInfo.UserName)}";

            var parameters = new[]
            {
                DataApi.GetParameter($"@{nameof(GroupUserInfo.UserName)}", userName)
            };

            using (var conn = DataApi.GetConnection(ConnectionString))
            {
                conn.Open();
                using (var rdr = DataApi.ExecuteReader(conn, sqlString, parameters))
                {
                    if (rdr.Read() && !rdr.IsDBNull(0))
                    {
                        groupId = rdr.GetInt32(0);
                    }
                    rdr.Close();
                }
            }

            return groupId;
        }
    }
}
