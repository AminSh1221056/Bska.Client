
using Bska.Client.Common;
using System;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;

namespace Bska.Client.Domain.Concrete
{
    public class ConnectionHelper
    {
        internal static string CreateConnectionString()
        {
            const string appName = "EntityFramework";
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = Settings.Default.dataSource;
            sqlBuilder.InitialCatalog = Settings.Default.initialCatalog;
            sqlBuilder.MultipleActiveResultSets = true;
            if (!Settings.Default.security)
            {
                sqlBuilder.IntegratedSecurity = false;
                sqlBuilder.UserID = Settings.Default.username;
                sqlBuilder.Password = "1221056@Am"; //GlobalClass.DecryptStringAES(Settings.Default.password, "66Ak679Du4V3qo92");
            }
            else
            {
                sqlBuilder.IntegratedSecurity = true;
            }

            sqlBuilder.ConnectTimeout = Settings.Default.timeOut;
            sqlBuilder.ApplicationName = appName;
            return sqlBuilder.ConnectionString;
        }

        public static DateTime GetCurrentDateTime()
        {
            DateTime dt = DateTime.MinValue;
            var objConn = new SqlConnection(ConnectionHelper.CreateConnectionString());
            try
            {
                objConn.Open();
                string commandText = "SELECT GETDATE() AS Today";
                SqlCommand myCommand = new SqlCommand(commandText, objConn);
                dt = (DateTime)myCommand.ExecuteScalar();
                GlobalClass._Today = dt;
                objConn.Close();
            }
            catch (SqlException)
            {
                dt = DateTime.Now;
                objConn.Close();
            }
            return dt;
        }

        public static bool CheckConnection()
        {
            SqlConnection con = new SqlConnection(ConnectionHelper.CreateConnectionString());
            try
            {
                con.Open();
                con.Close();
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }
    }
}
