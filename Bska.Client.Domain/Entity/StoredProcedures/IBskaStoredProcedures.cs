
namespace Bska.Client.Domain.Entity.StoredProcedures
{
    using Bska.Client.API.EF6;
    using Common;
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public interface IBskaStoredProcedures
    {
        int Update_OrderStatus(Int64 OrderNo, int Status, String Description, string decison);
        int Insert_Stuff(int KalaUID, string KalaNme, Boolean IsStuff, int Typ, int? ParentId, int floorOld, int floorNew);
        int Update_stuff(int KalaUID, string KalaNme, Boolean IsStuff, int Typ, int? ParentId, int floorOld, int floorNew);
        void delete_allStuffs();
        int MAssetUpdate_Confirmed();
        int DeleteAll_Assets();
        int MAssetUpdate_Compietion(int type);
        int MAsset_Delete(Int64 assetId, int type, int? label);
        int Insert_EventLog(EventLog eventLog);
        int UpdateStuffOnUpdateFromServer();
        int ExecuteNonQuery(string cmdText, CommandType cmdType, SqlParameter[] cmdParms);
        int ExecuteNonQuery(string cmdText, CommandType cmdType, DataTable dt);
        SqlDataReader ExecuteReader(string cmdText, CommandType cmdType, SqlParameter[] cmdParms);
        object ExecuteScalar(string cmdText, CommandType cmdType, SqlParameter[] cmdParms);
    }
}
