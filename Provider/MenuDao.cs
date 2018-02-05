using System.Collections.Generic;
using System.Data;
using SiteServer.Plugin;
using SS.Home.Core;
using SS.Home.Model;

namespace SS.Home.Provider
{
    public static class MenuDao
    {
        public const string TableName = "ss_home_menu";

        public static List<TableColumn> Columns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(MenuInfo.Id),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(MenuInfo.ParentId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(MenuInfo.Taxis),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(MenuInfo.Title),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(MenuInfo.Url),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(MenuInfo.IsOpenWindow),
                DataType = DataType.Boolean
            }
        };

        private static DatabaseType DatabaseType => Main.Instance.DatabaseType;
        private static string ConnectionString => Main.Instance.ConnectionString;
        private static IDataApi DataApi => Main.Instance.DataApi;

        public static int Insert(MenuInfo menuInfo)
        {
            string sqlString = $@"INSERT INTO {TableName}
(
    {nameof(MenuInfo.ParentId)}, 
    {nameof(MenuInfo.Taxis)}, 
    {nameof(MenuInfo.Title)}, 
    {nameof(MenuInfo.Url)}, 
    {nameof(MenuInfo.IsOpenWindow)}
) VALUES (
    @{nameof(MenuInfo.ParentId)}, 
    @{nameof(MenuInfo.Taxis)}, 
    @{nameof(MenuInfo.Title)}, 
    @{nameof(MenuInfo.Url)}, 
    @{nameof(MenuInfo.IsOpenWindow)}
)";

            menuInfo.Taxis = GetMaxTaxis(menuInfo.ParentId) + 1;
            var parameters = new[]
            {
                DataApi.GetParameter($"@{nameof(MenuInfo.ParentId)}", menuInfo.ParentId),
                DataApi.GetParameter($"@{nameof(MenuInfo.Taxis)}", menuInfo.Taxis),
                DataApi.GetParameter($"@{nameof(MenuInfo.Title)}", menuInfo.Title),
                DataApi.GetParameter($"@{nameof(MenuInfo.Url)}", menuInfo.Url),
                DataApi.GetParameter($"@{nameof(MenuInfo.IsOpenWindow)}", menuInfo.IsOpenWindow)
            };

            return DataApi.ExecuteNonQueryAndReturnId(TableName, nameof(MenuInfo.Id), ConnectionString, sqlString, parameters);
        }

        public static void Update(MenuInfo menuInfo)
        {
            string sqlString = $@"UPDATE {TableName} SET
                {nameof(MenuInfo.ParentId)} = @{nameof(MenuInfo.ParentId)}, 
                {nameof(MenuInfo.Taxis)} = @{nameof(MenuInfo.Taxis)}, 
                {nameof(MenuInfo.Title)} = @{nameof(MenuInfo.Title)}, 
                {nameof(MenuInfo.Url)} = @{nameof(MenuInfo.Url)}, 
                {nameof(MenuInfo.IsOpenWindow)} = @{nameof(MenuInfo.IsOpenWindow)}
            WHERE {nameof(MenuInfo.Id)} = @{nameof(MenuInfo.Id)}";

            var parameters = new[]
            {
                DataApi.GetParameter($"@{nameof(MenuInfo.ParentId)}", menuInfo.ParentId),
                DataApi.GetParameter($"@{nameof(MenuInfo.Taxis)}", menuInfo.Taxis),
                DataApi.GetParameter($"@{nameof(MenuInfo.Title)}", menuInfo.Title),
                DataApi.GetParameter($"@{nameof(MenuInfo.Url)}", menuInfo.Url),
                DataApi.GetParameter($"@{nameof(MenuInfo.IsOpenWindow)}", menuInfo.IsOpenWindow),
                DataApi.GetParameter($"@{nameof(MenuInfo.Id)}", menuInfo.Id)
            };

            DataApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
        }

        public static void Delete(int id)
        {
            string sqlString = $"DELETE FROM {TableName} WHERE {nameof(MenuInfo.Id)} = @{nameof(MenuInfo.Id)}";

            var parameters = new []
            {
                DataApi.GetParameter($"@{nameof(MenuInfo.Id)}", id)
            };

            DataApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
        }

        public static MenuInfo GetMenuInfo(int menuId)
        {
            if (menuId == 0) return null;
            
            MenuInfo menuInfo = null;

            string sqlString = $@"SELECT
    {nameof(MenuInfo.Id)},
    {nameof(MenuInfo.ParentId)},
    {nameof(MenuInfo.Taxis)}, 
    {nameof(MenuInfo.Title)}, 
    {nameof(MenuInfo.Url)}, 
    {nameof(MenuInfo.IsOpenWindow)}
    FROM {TableName} 
    WHERE {nameof(MenuInfo.Id)} = @{nameof(MenuInfo.Id)}";

            var parms = new []
            {
                DataApi.GetParameter($"@{nameof(MenuInfo.Id)}", menuId)
            };

            using (var rdr = DataApi.ExecuteReader(ConnectionString, sqlString, parms))
            {
                if (rdr.Read())
                {
                    menuInfo = GetMenuInfo(rdr);
                }
                rdr.Close();
            }

            return menuInfo;
        }

        public static List<MenuInfo> GetMenuInfoList(int parentId)
        {
            var list = new List<MenuInfo>();

            string sqlString =
                $"SELECT {nameof(MenuInfo.Id)}, {nameof(MenuInfo.ParentId)}, {nameof(MenuInfo.Taxis)}, {nameof(MenuInfo.Title)}, {nameof(MenuInfo.Url)}, {nameof(MenuInfo.IsOpenWindow)} FROM {TableName} WHERE {nameof(MenuInfo.ParentId)} = @{nameof(MenuInfo.ParentId)} ORDER BY {nameof(MenuInfo.Taxis)}, {nameof(MenuInfo.Id)}";

            var parameters = new[]
            {
                DataApi.GetParameter($"@{nameof(MenuInfo.ParentId)}", parentId)
            };

            using (var rdr = DataApi.ExecuteReader(ConnectionString, sqlString, parameters))
            {
                while (rdr.Read())
                {
                    list.Add(GetMenuInfo(rdr));
                }
                rdr.Close();
            }

            return list;
        }

        public static bool UpdateTaxisToUp(int parentId, int menuId)
        {
            var sqlString = Utils.GetTopSqlString(DatabaseType, TableName, "Id, Taxis", $"WHERE ((Taxis > (SELECT Taxis FROM {TableName} WHERE Id = {menuId})) AND ParentId ={parentId}) ORDER BY Taxis", 1);

            var higherId = 0;
            var higherTaxis = 0;

            using (var rdr = DataApi.ExecuteReader(ConnectionString, sqlString))
            {
                if (rdr.Read())
                {
                    higherId = rdr.GetInt32(0);
                    higherTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(menuId);

            if (higherId != 0)
            {
                SetTaxis(menuId, higherTaxis);
                SetTaxis(higherId, selectedTaxis);
                return true;
            }
            return false;
        }

        public static bool UpdateTaxisToDown(int parentId, int menuId)
        {
            var sqlString = Utils.GetTopSqlString(DatabaseType, TableName, "Id, Taxis", $"WHERE ((Taxis < (SELECT Taxis FROM {TableName} WHERE (Id = {menuId}))) AND ParentId = {parentId}) ORDER BY Taxis DESC", 1);

            var lowerId = 0;
            var lowerTaxis = 0;

            using (var rdr = DataApi.ExecuteReader(ConnectionString, sqlString))
            {
                if (rdr.Read())
                {
                    lowerId = rdr.GetInt32(0);
                    lowerTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(menuId);

            if (lowerId != 0)
            {
                SetTaxis(menuId, lowerTaxis);
                SetTaxis(lowerId, selectedTaxis);
                return true;
            }
            return false;
        }

        private static int GetMaxTaxis(int parentId)
        {
            string sqlString =
                $"SELECT MAX(Taxis) FROM {TableName} WHERE {nameof(MenuInfo.ParentId)} = {parentId}";
            return Dao.GetIntResult(sqlString);
        }

        private static int GetTaxis(int menuId)
        {
            string sqlString = $"SELECT Taxis FROM {TableName} WHERE ({nameof(MenuInfo.Id)} = {menuId})";
            return Dao.GetIntResult(sqlString);
        }

        private static void SetTaxis(int menuId, int taxis)
        {
            string sqlString = $"UPDATE {TableName} SET Taxis = {taxis} WHERE {nameof(MenuInfo.Id)} = {menuId}";
            DataApi.ExecuteNonQuery(ConnectionString, sqlString);
        }

        private static MenuInfo GetMenuInfo(IDataRecord rdr)
        {
            if (rdr == null) return null;

            var menuInfo = new MenuInfo();

            var i = 0;
            menuInfo.Id = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            menuInfo.ParentId = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            menuInfo.Taxis = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
            i++;
            menuInfo.Title = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            menuInfo.Url = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
            i++;
            menuInfo.IsOpenWindow = !rdr.IsDBNull(i) && rdr.GetBoolean(i);

            return menuInfo;
        }
    }
}