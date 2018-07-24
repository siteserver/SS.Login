namespace SS.Login.Provider
{
    public static class Dao
    {
        public static int GetIntResult(string sqlString)
        {
            var count = 0;

            using (var conn = LoginPlugin.Instance.DatabaseApi.GetConnection(LoginPlugin.Instance.ConnectionString))
            {
                conn.Open();
                using (var rdr = LoginPlugin.Instance.DatabaseApi.ExecuteReader(conn, sqlString))
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