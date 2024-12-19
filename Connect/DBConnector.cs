using Microsoft.Data.SqlClient;

namespace VIS_projekt.Connect
{
    public static class DBConnector
    {
        public static SqlConnectionStringBuilder GetBuilder()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"XXXXXX";
            builder.InitialCatalog = "XXXX";
            builder.IntegratedSecurity = true;
            builder.TrustServerCertificate = true;
            return builder;

        }
    }
}
