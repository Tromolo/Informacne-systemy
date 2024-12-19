using Microsoft.Data.SqlClient;

namespace VIS_projekt.Connect
{
    public static class DBConnector
    {
        public static SqlConnectionStringBuilder GetBuilder()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"DESKTOP-PC0QDA3";
            builder.InitialCatalog = "SQL";
            builder.IntegratedSecurity = true;
            builder.TrustServerCertificate = true;
            return builder;

        }
    }
}
