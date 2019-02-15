
namespace Bska.Client.Domain.Concrete
{
    using System.Data.SqlClient;
    using System;
    using Bska.Client.Domain.Entity.StoredProcedures;
    using Common;
    using Entity;
    using System.Data;

    public partial class BskaContext : IBskaStoredProcedures
    {
        public int Update_OrderStatus(long OrderNo, int Status, String Description, string Decison)
        {
            var orderNo = new SqlParameter("@OrderNo", OrderNo);
            var status = new SqlParameter("@Status", Status);
            var des = new SqlParameter("@Description", Description);
            var decision = new SqlParameter("@Decision", Decison);
            return Database.ExecuteSqlCommand("Update_OrderStatus @OrderNo, @Status, @Description, @Decision", orderNo, status, des, decision);
        }

        public int Insert_Stuff(int KalaUID, string KalaNme, Boolean IsStuff, int Typ, int? ParentId, int floorOld, int floorNew)
        {
            var kalaId = new SqlParameter("@KalaUID", KalaUID);
            var kn = new SqlParameter("@KalaNme", KalaNme);
            var isSt = new SqlParameter("@IsStuff", IsStuff);
            var typ = new SqlParameter("@Typ", Typ);
            var PId = new SqlParameter("@ParentId", (object)ParentId ?? DBNull.Value);
            var flOld = new SqlParameter("@FloorOld", floorOld);
            var flNew = new SqlParameter("@FloorNew", floorNew);
            return Database.ExecuteSqlCommand("insert_stuff @KalaUID,@KalaNme,@IsStuff,@Typ,@FloorOld,@FloorNew,@ParentId", kalaId, kn, isSt, typ,flOld,flNew, PId);
        }

        public int Update_stuff(int KalaUID, string KalaNme, Boolean IsStuff, int Typ, int? ParentId, int floorOld, int floorNew)
        {
            var kalaId = new SqlParameter("@KalaUID", KalaUID);
            var kn = new SqlParameter("@KalaNme", KalaNme);
            var isSt = new SqlParameter("@IsStuff", IsStuff);
            var typ = new SqlParameter("@Typ", Typ);
            var PId = new SqlParameter("@ParentId", (object)ParentId ?? DBNull.Value);
            var flOld = new SqlParameter("@FloorOld", floorOld);
            var flNew = new SqlParameter("@FloorNew", floorNew);
            return Database.ExecuteSqlCommand("update_stuff @KalaUID,@KalaNme,@IsStuff,@Typ,@FloorOld,@FloorNew,@ParentId", kalaId, kn, isSt, typ, flOld, flNew, PId);
        }

        public void delete_allStuffs()
        {
            Database.ExecuteSqlCommand("delete_allStuffs");
        }

        public int MAssetUpdate_Confirmed()
        {
            return Database.ExecuteSqlCommand("MAssetUpdate_Confirmed");
        }

        public int MAsset_Delete(long assetId, int type, int? label)
        {
            var massetId = new SqlParameter("@AssetId", assetId);
            var assetType = new SqlParameter("@AssetType", type);
            var lbl = new SqlParameter("@label", (object)label ?? DBNull.Value);
            return Database.ExecuteSqlCommand("MAsset_Delete @AssetId,@AssetType,@label", massetId,assetType, lbl);
        }

        public int Insert_EventLog(EventLog eventLog)
        {
            var uId = new SqlParameter("@UserId", eventLog.UserId);
            var tp = new SqlParameter("@Type", eventLog.Type);
            var msg = new SqlParameter("@Message", (object)eventLog.Message ?? DBNull.Value);
            var ky = new SqlParameter("@key", (object)eventLog.Key ?? DBNull.Value);
            var date = new SqlParameter("@EntryDate", (object)eventLog.EntryDate?? DBNull.Value);
            return Database.ExecuteSqlCommand("Insert_EventLog @Type, @Key, @Message, @EntryDate, @UserId", tp,ky,msg,date,uId);
        }

        public int MAssetUpdate_Compietion(int type)
        {
            var typ = new SqlParameter("@Typ", type);
            return Database.ExecuteSqlCommand("MAssetUpdate_Compietion @Typ",typ);
        }

        private SqlConnection DatabaseConnection
        {
            get
            {
                return new SqlConnection(ConnectionHelper.CreateConnectionString());
            }
        }

        public int ExecuteNonQuery(string cmdText, CommandType cmdType, SqlParameter[] cmdParms)
        {
            SqlCommand cmd = DatabaseConnection.CreateCommand();
            using (DatabaseConnection)
            {
                CreateCommand(cmd, DatabaseConnection, null, cmdType, cmdText, cmdParms,null);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public SqlDataReader ExecuteReader(string cmdText, CommandType cmdType, SqlParameter[] cmdParms)
        {
            SqlCommand cmd = DatabaseConnection.CreateCommand();
            CreateCommand(cmd, DatabaseConnection, null, cmdType, cmdText, cmdParms,null);
            var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return rdr;
        }

        public object ExecuteScalar(string cmdText, CommandType cmdType, SqlParameter[] cmdParms)
        {
            SqlCommand cmd = DatabaseConnection.CreateCommand();
            CreateCommand(cmd, DatabaseConnection, null, cmdType, cmdText, cmdParms,null);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        private void CreateCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] commandParameters,DataTable dt,bool isTvp=false)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }

            cmd.CommandType = cmdType;
            if (isTvp)
            {
                SqlParameter tvpParam = cmd.Parameters.AddWithValue(dt.TableName, dt); //Needed TVP
                tvpParam.SqlDbType = SqlDbType.Structured; //tells ADO.NET we are passing TVP
            }
            else
            {
                if (commandParameters != null)
                {
                    AddSqlParameters(cmd, commandParameters);
                }
            }
        }

        private void AddSqlParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            foreach (SqlParameter p in commandParameters)
            {
                //check for derived output value with no value assigned
                if (p.Value == null)
                {
                    p.Value = DBNull.Value;
                }

                command.Parameters.Add(p);
            }
        }

        public int ExecuteNonQuery(string cmdText, CommandType cmdType, DataTable dt)
        {
            SqlCommand cmd = DatabaseConnection.CreateCommand();
            using (DatabaseConnection)
            {
                CreateCommand(cmd, DatabaseConnection, null, cmdType, cmdText, null,dt,true);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public int DeleteAll_Assets()
        {
            return Database.ExecuteSqlCommand("DeleteAll_Assets ");
        }

        public int UpdateStuffOnUpdateFromServer()
        {
            return Database.ExecuteSqlCommand("UpdateStuffOnUpdateFromServer");
        }
    }
}
