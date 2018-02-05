using System.Collections.Generic;
using System.Data;
using SiteServer.Plugin;
using SS.Home.Model;

namespace SS.Home.Provider
{
    public static class GroupDao
    {
        public const string TableName = "ss_home_group";

        public static List<TableColumn> Columns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(GroupInfo.Id),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(GroupInfo.GroupName),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(GroupInfo.IsWriting),
                DataType = DataType.Boolean
            },
            new TableColumn
            {
                AttributeName = nameof(GroupInfo.WritingAdmin),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(GroupInfo.LastWritingSiteId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(GroupInfo.LastWritingChannelId),
                DataType = DataType.Integer
            }
        };

        private static string ConnectionString => Main.Instance.ConnectionString;
        private static IDataApi DataApi => Main.Instance.DataApi;

        public static int Insert(GroupInfo groupInfo)
        {
            var sqlString = $@"INSERT INTO {TableName} (
                {nameof(GroupInfo.GroupName)}, 
                {nameof(GroupInfo.IsWriting)}, 
                {nameof(GroupInfo.WritingAdmin)},
                {nameof(GroupInfo.LastWritingSiteId)},
                {nameof(GroupInfo.LastWritingChannelId)}
            ) VALUES (
                @{nameof(GroupInfo.GroupName)}, 
                @{nameof(GroupInfo.IsWriting)}, 
                @{nameof(GroupInfo.WritingAdmin)},
                @{nameof(GroupInfo.LastWritingSiteId)}, 
                @{nameof(GroupInfo.LastWritingChannelId)}
            )";

            var parameters = new []
			{
                DataApi.GetParameter($"@{nameof(GroupInfo.GroupName)}", groupInfo.GroupName),
                DataApi.GetParameter($"@{nameof(GroupInfo.IsWriting)}", groupInfo.IsWriting),
                DataApi.GetParameter($"@{nameof(GroupInfo.WritingAdmin)}", groupInfo.WritingAdmin),
                DataApi.GetParameter($"@{nameof(GroupInfo.LastWritingSiteId)}", groupInfo.LastWritingSiteId),
                DataApi.GetParameter($"@{nameof(GroupInfo.LastWritingChannelId)}", groupInfo.LastWritingChannelId)
            };

            return DataApi.ExecuteNonQueryAndReturnId(TableName, nameof(GroupInfo.Id), ConnectionString, sqlString, parameters);
        }

        public static void Update(GroupInfo groupInfo)
        {
            var sqlString = $@"UPDATE {TableName} SET 
                {nameof(GroupInfo.GroupName)} = @{nameof(GroupInfo.GroupName)}, 
                {nameof(GroupInfo.IsWriting)} = @{nameof(GroupInfo.IsWriting)}, 
                {nameof(GroupInfo.WritingAdmin)} = @{nameof(GroupInfo.WritingAdmin)},
                {nameof(GroupInfo.LastWritingSiteId)} = @{nameof(GroupInfo.LastWritingSiteId)},
                {nameof(GroupInfo.LastWritingChannelId)} = @{nameof(GroupInfo.LastWritingChannelId)}
            WHERE {nameof(GroupInfo.Id)} = @{nameof(GroupInfo.Id)}";

            var parameters = new []
			{
                DataApi.GetParameter($"@{nameof(GroupInfo.GroupName)}", groupInfo.GroupName),
                DataApi.GetParameter($"@{nameof(GroupInfo.IsWriting)}", groupInfo.IsWriting),
                DataApi.GetParameter($"@{nameof(GroupInfo.WritingAdmin)}", groupInfo.WritingAdmin),
                DataApi.GetParameter($"@{nameof(GroupInfo.LastWritingSiteId)}", groupInfo.LastWritingSiteId),
                DataApi.GetParameter($"@{nameof(GroupInfo.LastWritingChannelId)}", groupInfo.LastWritingChannelId),
                DataApi.GetParameter($"@{nameof(GroupInfo.Id)}", groupInfo.Id)
			};

            DataApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
        }

        public static void Delete(int groupId)
        {
            var sqlString = $"DELETE FROM {TableName} WHERE Id = @Id";
            var parameters = new []
			{
				DataApi.GetParameter($"@{nameof(GroupInfo.Id)}", groupId)
			};

            DataApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
        }

        public static List<GroupInfo> GetGroupInfoList()
        {
            var list = new List<GroupInfo>();

            var sqlString = $@"SELECT 
    {nameof(GroupInfo.Id)}, 
    {nameof(GroupInfo.GroupName)}, 
    {nameof(GroupInfo.IsWriting)}, 
    {nameof(GroupInfo.WritingAdmin)},
    {nameof(GroupInfo.LastWritingSiteId)},
    {nameof(GroupInfo.LastWritingChannelId)}
FROM {TableName} ORDER BY {nameof(GroupInfo.Id)}";

            using (var conn = DataApi.GetConnection(ConnectionString))
            {
                conn.Open();
                using (var rdr = DataApi.ExecuteReader(conn, sqlString))
                {
                    while (rdr.Read())
                    {
                        var groupInfo = GetGroupInfo(rdr);
                        list.Add(groupInfo);
                    }
                    rdr.Close();
                }
            }

            return list;
        }

        public static GroupInfo GetGroupInfo(int groupId)
        {
            if (groupId <= 0) return null;

            GroupInfo groupInfo = null;

            var sqlString = $@"SELECT 
    {nameof(GroupInfo.Id)}, 
    {nameof(GroupInfo.GroupName)}, 
    {nameof(GroupInfo.IsWriting)}, 
    {nameof(GroupInfo.WritingAdmin)},
    {nameof(GroupInfo.LastWritingSiteId)},
    {nameof(GroupInfo.LastWritingChannelId)}
FROM {TableName} WHERE {nameof(GroupInfo.Id)} = @{nameof(GroupInfo.Id)}";

            var parameters = new []
            {
                DataApi.GetParameter($"@{nameof(GroupInfo.Id)}", groupId)
            };

            using (var conn = DataApi.GetConnection(ConnectionString))
            {
                conn.Open();
                using (var rdr = DataApi.ExecuteReader(conn, sqlString, parameters))
                {
                    if (rdr.Read())
                    {
                        groupInfo = GetGroupInfo(rdr);
                    }
                    rdr.Close();
                }
            }

            return groupInfo;
        }

        public static bool IsExists(string groupName)
        {
            var exists = false;

            var sqlString = $"SELECT {nameof(GroupInfo.Id)} FROM {TableName} WHERE {nameof(GroupInfo.GroupName)} = @{nameof(GroupInfo.GroupName)}";

            var parameters = new []
			{
                DataApi.GetParameter($"@{nameof(GroupInfo.GroupName)}", groupName)
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

        private static GroupInfo GetGroupInfo(IDataRecord rdr)
        {
            if (rdr == null) return null;

            var groupInfo = new GroupInfo();

            var i = 0;
            groupInfo.Id = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            groupInfo.GroupName = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            groupInfo.IsWriting = !rdr.IsDBNull(i) && rdr.GetBoolean(i);
            i++;
            groupInfo.WritingAdmin = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            groupInfo.LastWritingSiteId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            groupInfo.LastWritingChannelId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);

            return groupInfo;
        }
    }
}
