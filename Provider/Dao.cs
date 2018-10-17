using SiteServer.Plugin;

namespace SS.Login.Provider
{
    public static class Dao
    {
        public static int GetIntResult(string sqlString)
        {
            var count = 0;

            using (var conn = Context.DatabaseApi.GetConnection(Context.ConnectionString))
            {
                conn.Open();
                using (var rdr = Context.DatabaseApi.ExecuteReader(conn, sqlString))
                {
                    if (rdr.Read() && !rdr.IsDBNull(0))
                    {
                        count = rdr.GetInt32(0);
                    }
                    rdr.Close();
                }
            }
            return count;
        }
    }
}