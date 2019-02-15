
namespace Bska.Client.UI.Helper
{
    using ADOX;
    using System;
    using System.IO;
    using System.Data;
    using System.Data.OleDb;
    public class AccessDatabaseHelper
    {
        ADOX.Catalog cat = null;
        string conStr;
        public AccessDatabaseHelper()
        {
            cat = new ADOX.Catalog();
        }

        private void CreateDatabaseIfNotExit(string filePath)
        {
            string filename = filePath;
            conStr = "Provider=Microsoft.Jet.OLEDB.4.0;";
            conStr += "Data Source=" + filename + ";Jet OLEDB:Engine Type=5";
            if (!File.Exists(filePath))
            {
                cat.Create(conStr);
            }
            else
            {
                cat.let_ActiveConnection(conStr);
            }
        }

        public bool CreateTable(string filePath,int tbNo)
        {
            this.CreateDatabaseIfNotExit(filePath);
            Table nTable = new Table();
            if (tbNo==1001)
            {
                nTable.Name = "Arrival";
                this.createArrivalTable(nTable);
            }
            else if (tbNo == 1002)
            {
                nTable.Name = "Arrival_hokmmasrafi";
                this.createArrivalTable(nTable);
            }
            else if (tbNo == 1003)
            {
                nTable.Name = "TransmitIn";
                this.createTransmitIn(nTable);
            }
            else if (tbNo == 1004)
            {
                nTable.Name = "TransmitOut";
                this.createTransmitOut(nTable);
            }
            else if (tbNo == 1005)
            {
                nTable.Name = "Perm";
                this.createPerm(nTable);
            }
            else if (tbNo == 1006)
            {
                nTable.Name = "PermEdit";
                this.createPermEdit(nTable);
            }
            else
            {
                return false;
            }

            try
            {
                try
                {
                    cat.Tables.Delete(nTable.Name);
                }
                catch (Exception)
                {
                    //...nothing
                }

                cat.Tables.Append(nTable);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(nTable);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(cat.Tables);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(cat.ActiveConnection);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(cat);
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error on create Access table" + "." + ex.Message);
            }
        }

        public Boolean insertToTable(int tbNo,DataTable dt)
        {
            if (tbNo == 1001)
            {
                this.AddToArrivalTable(dt,"Arrival");
            }
            else if (tbNo == 1002)
            {
                this.AddToArrivalTable(dt, "Arrival_hokmmasrafi");
            }
            else if (tbNo == 1003)
            {
                this.AddToTransmitInTable(dt, "TransmitIn");
            }
            else if (tbNo == 1004)
            {
                this.AddToToTransmitOutTable(dt, "TransmitOut");
            }
            else if (tbNo == 1005)
            {
                this.AddToPermTable(dt, "Perm");
            }
            else if (tbNo == 1006)
            {
                this.AddToPermEditTable(dt, "PermEdit");
            }
            else
            {
                return false;
            }
            return true;
        }

        private void createArrivalTable(Table nTable)
        {
            nTable.Columns.Append("kalauid", DataTypeEnum.adInteger);
            nTable.Columns.Append("Curstate", DataTypeEnum.adInteger);
            nTable.Columns.Append("acqtyp", DataTypeEnum.adInteger);
            nTable.Columns.Append("anbrsdno", DataTypeEnum.adInteger);
            nTable.Columns.Append("anbhvlno", DataTypeEnum.adInteger);
            nTable.Columns.Append("custuid", DataTypeEnum.adInteger);
            nTable.Columns.Append("lable", DataTypeEnum.adInteger);
            nTable.Columns.Append("amvuid", DataTypeEnum.adInteger);
            nTable.Columns.Append("namuid", DataTypeEnum.adVarWChar,20);
            nTable.Columns.Append("Year", DataTypeEnum.adInteger);
            nTable.Columns.Append("Desc", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Cost", DataTypeEnum.adCurrency,15);
            nTable.Columns.Append("transfer", DataTypeEnum.adInteger);
            nTable.Columns.Append("Uid1", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Uid2", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Uid3", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Uid4", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Desc1", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Desc2", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Desc3", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Desc4", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("couid", DataTypeEnum.adInteger);
        }

        private void createPerm(Table nTable)
        {
            nTable.Columns.Append("kalauid", DataTypeEnum.adInteger);
            nTable.Columns.Append("Curstate", DataTypeEnum.adInteger);
            nTable.Columns.Append("acqtyp", DataTypeEnum.adInteger);
            nTable.Columns.Append("anbrsdno", DataTypeEnum.adInteger);
            nTable.Columns.Append("anbhvlno", DataTypeEnum.adInteger);
            nTable.Columns.Append("custuid", DataTypeEnum.adInteger);
            nTable.Columns.Append("lable", DataTypeEnum.adInteger);
            nTable.Columns.Append("amvuid", DataTypeEnum.adInteger);
            nTable.Columns.Append("namuid", DataTypeEnum.adVarWChar, 20);
            nTable.Columns.Append("Year", DataTypeEnum.adInteger);
            nTable.Columns.Append("Desc", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Cost", DataTypeEnum.adCurrency);
            nTable.Columns.Append("transfer", DataTypeEnum.adInteger);
            nTable.Columns.Append("Uid1", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Uid2", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Uid3", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Uid4", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Desc1", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Desc2", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Desc3", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Desc4", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("optyp", DataTypeEnum.adInteger);
            nTable.Columns.Append("bossnme", DataTypeEnum.adWChar, 20);
            nTable.Columns.Append("bosstkh", DataTypeEnum.adVarWChar, 10);
            nTable.Columns.Append("dareenme", DataTypeEnum.adVarWChar, 20);
            nTable.Columns.Append("dareetkh", DataTypeEnum.adVarWChar, 10);
            nTable.Columns.Append("othernme", DataTypeEnum.adVarWChar, 20);
            nTable.Columns.Append("othertkh", DataTypeEnum.adVarWChar, 10);
            nTable.Columns.Append("Trnscust", DataTypeEnum.adInteger);
            nTable.Columns.Append("Divannamno", DataTypeEnum.adVarWChar, 20);
            nTable.Columns.Append("divannamtkh", DataTypeEnum.adVarWChar, 10);
            nTable.Columns.Append("divanray", DataTypeEnum.adInteger);
            nTable.Columns.Append("mdtamnt", DataTypeEnum.adInteger);
            nTable.Columns.Append("ComisunNo", DataTypeEnum.adInteger);
        }

        private void createPermEdit(Table nTable)
        {
            nTable.Columns.Append("kalauid", DataTypeEnum.adInteger);
            nTable.Columns.Append("Curstate", DataTypeEnum.adInteger);
            nTable.Columns.Append("custuid", DataTypeEnum.adInteger);
            nTable.Columns.Append("lable", DataTypeEnum.adInteger);
            nTable.Columns.Append("bossnme", DataTypeEnum.adVarWChar, 20);
            nTable.Columns.Append("bosstkh", DataTypeEnum.adVarWChar, 10);
            nTable.Columns.Append("namuid", DataTypeEnum.adVarWChar, 20);
            nTable.Columns.Append("Year", DataTypeEnum.adInteger);
            nTable.Columns.Append("Desc", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Cost", DataTypeEnum.adCurrency);
            nTable.Columns.Append("Uid1", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Uid2", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Uid3", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Uid4", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Desc1", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Desc2", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Desc3", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Desc4", DataTypeEnum.adVarWChar, 50);
        }

        private void createTransmitOut(Table nTable)
        {
            nTable.Columns.Append("custuid", DataTypeEnum.adInteger);
            nTable.Columns.Append("lable", DataTypeEnum.adInteger);
            nTable.Columns.Append("amvuid", DataTypeEnum.adInteger);
            nTable.Columns.Append("namuid", DataTypeEnum.adVarWChar, 20);
            nTable.Columns.Append("acttyp", DataTypeEnum.adInteger);
            nTable.Columns.Append("permuid", DataTypeEnum.adInteger);
            nTable.Columns.Append("Trnscust", DataTypeEnum.adInteger);
            nTable.Columns.Append("fishno", DataTypeEnum.adVarWChar,15);
            nTable.Columns.Append("fishtkh", DataTypeEnum.adVarWChar,10);
            nTable.Columns.Append("hesabno", DataTypeEnum.adVarWChar, 20);
            nTable.Columns.Append("mablagh", DataTypeEnum.adInteger);
            nTable.Columns.Append("Foroshcost", DataTypeEnum.adInteger);
            nTable.Columns.Append("srtjlstkh", DataTypeEnum.adInteger);
        }

        private void createTransmitIn(Table nTable)
        {
            nTable.Columns.Append("kalauid", DataTypeEnum.adInteger);
            nTable.Columns.Append("Curstate", DataTypeEnum.adInteger);
            nTable.Columns.Append("acqtyp", DataTypeEnum.adInteger);
            nTable.Columns.Append("anbrsdno", DataTypeEnum.adInteger);
            nTable.Columns.Append("anbhvlno", DataTypeEnum.adInteger);
            nTable.Columns.Append("custuid", DataTypeEnum.adInteger);
            nTable.Columns.Append("lable", DataTypeEnum.adInteger);
            nTable.Columns.Append("amvuid", DataTypeEnum.adInteger);
            nTable.Columns.Append("namuid", DataTypeEnum.adVarWChar, 20);
            nTable.Columns.Append("Year", DataTypeEnum.adInteger);
            nTable.Columns.Append("Desc", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Cost", DataTypeEnum.adCurrency);
            nTable.Columns.Append("Uid1", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Uid2", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Uid3", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Uid4", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Desc1", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Desc2", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Desc3", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("Desc4", DataTypeEnum.adVarWChar, 50);
            nTable.Columns.Append("acttyp", DataTypeEnum.adInteger);
            nTable.Columns.Append("trnscust", DataTypeEnum.adInteger);
            nTable.Columns.Append("bfrlable", DataTypeEnum.adInteger);
            nTable.Columns.Append("permuid", DataTypeEnum.adInteger);
            nTable.Columns.Append("srtjlstkh", DataTypeEnum.adVarWChar, 10);
        }

        private void AddToArrivalTable(DataTable dT,string table)
        {
            OleDbConnection OleConn = new OleDbConnection(conStr);
            OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * From " + table, OleConn);
            OleDbCommandBuilder cmdBuilder = new OleDbCommandBuilder(adapter);

            adapter.InsertCommand = new OleDbCommand(String.Format("INSERT INTO {0} ([kalauid], [Curstate], [acqtyp], [anbrsdno], [anbhvlno], [custuid], [lable], [amvuid], [namuid], [Year], [Desc], [Cost], [transfer], [Uid1], [Uid2], [Uid3], [Uid4], [Desc1], [Desc2], [Desc3], [Desc4], [couid]) Values (@kalauid,@Curstate,@acqtyp,@anbrsdno,@anbhvlno,@custuid,@lable,@amvuid,@namuid,@Year,@Desc,@Cost,@transfer,@Uid1,@Uid2,@Uid3,@Uid4,@Desc1,@Desc2,@Desc3,@Desc4,@couid);", table), OleConn);

            adapter.InsertCommand.Parameters.Add("@kalauid", OleDbType.Integer,0, "kalauid");
            adapter.InsertCommand.Parameters.Add("@Curstate", OleDbType.Integer, 0, "Curstate");
            adapter.InsertCommand.Parameters.Add("@acqtyp", OleDbType.Integer, 0, "acqtyp");
            adapter.InsertCommand.Parameters.Add("@anbrsdno", OleDbType.Integer, 0, "anbrsdno");
            adapter.InsertCommand.Parameters.Add("@anbhvlno", OleDbType.Integer, 0, "anbhvlno");
            adapter.InsertCommand.Parameters.Add("@custuid", OleDbType.Integer, 0, "custuid");
            adapter.InsertCommand.Parameters.Add("@lable", OleDbType.Integer, 0, "lable");
            adapter.InsertCommand.Parameters.Add("@amvuid", OleDbType.Integer,0, "amvuid");
            adapter.InsertCommand.Parameters.Add("@namuid", OleDbType.VarWChar, 20, "namuid");
            adapter.InsertCommand.Parameters.Add("@Year", OleDbType.Integer, 0, "Year");
            adapter.InsertCommand.Parameters.Add("@Desc", OleDbType.VarWChar, 50, "Desc");
            adapter.InsertCommand.Parameters.Add("@Cost", OleDbType.Currency, 15, "Cost");
            adapter.InsertCommand.Parameters.Add("@transfer", OleDbType.Integer,0, "transfer");
            adapter.InsertCommand.Parameters.Add("@Uid1", OleDbType.VarWChar, 50, "Uid1");
            adapter.InsertCommand.Parameters.Add("@Uid2", OleDbType.VarWChar, 50, "Uid2");
            adapter.InsertCommand.Parameters.Add("@Uid3", OleDbType.VarWChar, 50, "Uid3");
            adapter.InsertCommand.Parameters.Add("@Uid4", OleDbType.VarWChar, 50, "Uid4");
            adapter.InsertCommand.Parameters.Add("@Desc1", OleDbType.VarWChar, 50, "Desc1");
            adapter.InsertCommand.Parameters.Add("@Desc2", OleDbType.VarWChar, 50, "Desc2");
            adapter.InsertCommand.Parameters.Add("@Desc3", OleDbType.VarWChar, 50, "Desc3");
            adapter.InsertCommand.Parameters.Add("@Desc4", OleDbType.VarWChar, 50, "Desc4");
            adapter.InsertCommand.Parameters.Add("@couid", OleDbType.Integer, 0, "couid");
            adapter.InsertCommand.Connection.Open();
           
            adapter.Update(dT);
            adapter.InsertCommand.Connection.Close();
        }

        private void AddToPermTable(DataTable dt, string table)
        {
            OleDbConnection OleConn = new OleDbConnection(conStr);
            OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * From " + table, OleConn);
            OleDbCommandBuilder cmdBuilder = new OleDbCommandBuilder(adapter);
            adapter.InsertCommand = new OleDbCommand(String.Format("INSERT INTO {0} ([kalauid], [Curstate], [acqtyp], [anbrsdno], [anbhvlno], [custuid], [lable], [amvuid], [namuid], [Year], [Desc], [Cost], [transfer], [Uid1], [Uid2], [Uid3], [Uid4], [Desc1], [Desc2], [Desc3], [Desc4], [optyp], [bossnme], [bosstkh], [dareenme], [dareetkh], [othernme], [othertkh], [Trnscust], [Divannamno], [divannamtkh], [divanray], [mdtamnt], [ComisunNo]) Values (@kalauid,@Curstate,@acqtyp,@anbrsdno,@anbhvlno,@custuid,@lable,@amvuid,@namuid,@Year,@Desc,@Cost,@transfer,@Uid1,@Uid2,@Uid3,@Uid4,@Desc1,@Desc2,@Desc3,@Desc4,@optyp,@bossnme,@bosstkh,@dareenme,@dareetkh,@othernme,@othertkh,@Trnscust,@Divannamno,@divannamtkh,@divanray,@mdtamnt,@ComisunNo);", table), OleConn);

            adapter.InsertCommand.Parameters.Add("@kalauid", OleDbType.Integer, 0, "kalauid");
            adapter.InsertCommand.Parameters.Add("@Curstate", OleDbType.Integer,0, "Curstate");
            adapter.InsertCommand.Parameters.Add("@acqtyp", OleDbType.Integer, 0, "acqtyp");
            adapter.InsertCommand.Parameters.Add("@anbrsdno", OleDbType.Integer, 0, "anbrsdno");
            adapter.InsertCommand.Parameters.Add("@anbhvlno", OleDbType.Integer,0, "anbhvlno");
            adapter.InsertCommand.Parameters.Add("@custuid", OleDbType.Integer, 0, "custuid");
            adapter.InsertCommand.Parameters.Add("@lable", OleDbType.Integer, 0, "lable");
            adapter.InsertCommand.Parameters.Add("@amvuid", OleDbType.Integer, 0, "amvuid");
            adapter.InsertCommand.Parameters.Add("@namuid", OleDbType.VarWChar, 20, "namuid");
            adapter.InsertCommand.Parameters.Add("@Year", OleDbType.Integer,0, "Year");
            adapter.InsertCommand.Parameters.Add("@Desc", OleDbType.VarWChar, 50, "Desc");
            adapter.InsertCommand.Parameters.Add("@Cost", OleDbType.Currency, 0, "Cost");
            adapter.InsertCommand.Parameters.Add("@transfer", OleDbType.Integer, 0, "transfer");
            adapter.InsertCommand.Parameters.Add("@Uid1", OleDbType.VarWChar, 50, "Uid1");
            adapter.InsertCommand.Parameters.Add("@Uid2", OleDbType.VarWChar, 50, "Uid2");
            adapter.InsertCommand.Parameters.Add("@Uid3", OleDbType.VarWChar, 50, "Uid3");
            adapter.InsertCommand.Parameters.Add("@Uid4", OleDbType.VarWChar, 50, "Uid4");
            adapter.InsertCommand.Parameters.Add("@Desc1", OleDbType.VarWChar, 50, "Desc1");
            adapter.InsertCommand.Parameters.Add("@Desc2", OleDbType.VarWChar, 50, "Desc2");
            adapter.InsertCommand.Parameters.Add("@Desc3", OleDbType.VarWChar, 50, "Desc3");
            adapter.InsertCommand.Parameters.Add("@Desc4", OleDbType.VarWChar, 50, "Desc4");
            adapter.InsertCommand.Parameters.Add("@optyp", OleDbType.Integer, 0, "optyp");
            adapter.InsertCommand.Parameters.Add("@bossnme", OleDbType.VarWChar, 20, "bossnme");
            adapter.InsertCommand.Parameters.Add("@bosstkh", OleDbType.VarWChar, 10, "bosstkh");
            adapter.InsertCommand.Parameters.Add("@dareenme", OleDbType.VarWChar, 20, "dareenme");
            adapter.InsertCommand.Parameters.Add("@dareetkh", OleDbType.VarWChar, 10, "dareetkh");
            adapter.InsertCommand.Parameters.Add("@othernme", OleDbType.VarWChar, 20, "othernme");
            adapter.InsertCommand.Parameters.Add("@othertkh", OleDbType.VarWChar, 10, "othertkh");
            adapter.InsertCommand.Parameters.Add("@Trnscust", OleDbType.Integer, 0, "Trnscust");
            adapter.InsertCommand.Parameters.Add("@Divannamno", OleDbType.VarWChar, 20, "Divannamno");
            adapter.InsertCommand.Parameters.Add("@divannamtkh", OleDbType.VarWChar, 10, "divannamtkh");
            adapter.InsertCommand.Parameters.Add("@divanray", OleDbType.Integer, 0, "divanray");
            adapter.InsertCommand.Parameters.Add("@mdtamnt", OleDbType.Integer, 0, "mdtamnt");
            adapter.InsertCommand.Parameters.Add("@ComisunNo", OleDbType.Integer,0, "ComisunNo");
            adapter.InsertCommand.Connection.Open();

            adapter.Update(dt);
            adapter.InsertCommand.Connection.Close();
        }

        private void AddToPermEditTable(DataTable dt,string table)
        {
            OleDbConnection OleConn = new OleDbConnection(conStr);
            OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * From " + table, OleConn);
            OleDbCommandBuilder cmdBuilder = new OleDbCommandBuilder(adapter);
            adapter.InsertCommand = new OleDbCommand(String.Format("INSERT INTO {0} ([kalauid], [Curstate], [custuid], [lable], [bossnme], [bosstkh], [namuid], [Year], [Desc], [Cost], [Uid1], [Uid2], [Uid3], [Uid4], [Desc1], [Desc2], [Desc3], [Desc4]) Values (@kalauid,@Curstate,@custuid,@lable,@bossnme,@bosstkh,@namuid,@Year,@Desc,@Cost,@Uid1,@Uid2,@Uid3,@Uid4,@Desc1,@Desc2,@Desc3,@Desc4);", table), OleConn);

            adapter.InsertCommand.Parameters.Add("@kalauid", OleDbType.Integer, 0, "kalauid");
            adapter.InsertCommand.Parameters.Add("@Curstate", OleDbType.Integer, 0, "Curstate");
            adapter.InsertCommand.Parameters.Add("@custuid", OleDbType.Integer, 0, "custuid");
            adapter.InsertCommand.Parameters.Add("@lable", OleDbType.Integer, 0, "lable");
            adapter.InsertCommand.Parameters.Add("@bossnme", OleDbType.VarWChar, 20, "bossnme");
            adapter.InsertCommand.Parameters.Add("@bosstkh", OleDbType.VarWChar, 10, "bosstkh");
            adapter.InsertCommand.Parameters.Add("@namuid", OleDbType.VarWChar, 20, "namuid");
            adapter.InsertCommand.Parameters.Add("@Year", OleDbType.Integer, 0, "Year");
            adapter.InsertCommand.Parameters.Add("@Desc", OleDbType.VarWChar, 50, "Desc");
            adapter.InsertCommand.Parameters.Add("@Cost", OleDbType.Currency, 0, "Cost");
            adapter.InsertCommand.Parameters.Add("@Uid1", OleDbType.VarWChar, 50, "Uid1");
            adapter.InsertCommand.Parameters.Add("@Uid2", OleDbType.VarWChar, 50, "Uid2");
            adapter.InsertCommand.Parameters.Add("@Uid3", OleDbType.VarWChar, 50, "Uid3");
            adapter.InsertCommand.Parameters.Add("@Uid4", OleDbType.VarWChar, 50, "Uid4");
            adapter.InsertCommand.Parameters.Add("@Desc1", OleDbType.VarWChar, 50, "Desc1");
            adapter.InsertCommand.Parameters.Add("@Desc2", OleDbType.VarWChar, 50, "Desc2");
            adapter.InsertCommand.Parameters.Add("@Desc3", OleDbType.VarWChar, 50, "Desc3");
            adapter.InsertCommand.Parameters.Add("@Desc4", OleDbType.VarWChar, 50, "Desc4");
            adapter.InsertCommand.Connection.Open();

            adapter.Update(dt);
            adapter.InsertCommand.Connection.Close();
        }

        private void AddToToTransmitOutTable(DataTable dt, string table)
        {
            OleDbConnection OleConn = new OleDbConnection(conStr);
            OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * From " + table, OleConn);
            OleDbCommandBuilder cmdBuilder = new OleDbCommandBuilder(adapter);
            adapter.InsertCommand = new OleDbCommand(String.Format("INSERT INTO {0} ([custuid], [lable], [amvuid], [namuid], [acttyp], [permuid], [Trnscust], [fishno], [fishtkh], [hesabno], [mablagh], [Foroshcost], [srtjlstkh]) Values (@custuid,@lable,@amvuid,@namuid,@acttyp,@permuid,@Trnscust,@fishno,@fishtkh,@hesabno,@mablagh,@Foroshcost,@srtjlstkh);", table), OleConn);

            adapter.InsertCommand.Parameters.Add("@custuid", OleDbType.Integer,0, "custuid");
            adapter.InsertCommand.Parameters.Add("@lable", OleDbType.Integer,0, "lable");
            adapter.InsertCommand.Parameters.Add("@amvuid", OleDbType.Integer, 0, "amvuid");
            adapter.InsertCommand.Parameters.Add("@namuid", OleDbType.VarWChar, 20, "namuid");
            adapter.InsertCommand.Parameters.Add("@acttyp", OleDbType.Integer, 0, "acttyp");
            adapter.InsertCommand.Parameters.Add("@permuid", OleDbType.Integer,0, "permuid"); 
            adapter.InsertCommand.Parameters.Add("@Trnscust", OleDbType.Integer, 0, "Trnscust");
            adapter.InsertCommand.Parameters.Add("@fishno", OleDbType.VarWChar, 15, "fishno");
            adapter.InsertCommand.Parameters.Add("@fishtkh", OleDbType.VarWChar, 10, "fishtkh");
            adapter.InsertCommand.Parameters.Add("@hesabno", OleDbType.VarWChar, 20, "hesabno");
            adapter.InsertCommand.Parameters.Add("@mablagh", OleDbType.Integer, 0, "mablagh");
            adapter.InsertCommand.Parameters.Add("@Foroshcost", OleDbType.Integer, 0, "Foroshcost");
            adapter.InsertCommand.Parameters.Add("@srtjlstkh", OleDbType.Integer, 0, "srtjlstkh");
            adapter.InsertCommand.Connection.Open();

            adapter.Update(dt);
            adapter.InsertCommand.Connection.Close();
        }

        private void AddToTransmitInTable(DataTable dt, string table)
        {
            OleDbConnection OleConn = new OleDbConnection(conStr);
            OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * From " + table, OleConn);
            OleDbCommandBuilder cmdBuilder = new OleDbCommandBuilder(adapter);

            adapter.InsertCommand = new OleDbCommand(String.Format("INSERT INTO {0} ([kalauid], [Curstate], [acqtyp], [anbrsdno], [anbhvlno], [custuid], [lable], [amvuid], [namuid], [Year], [Desc], [Cost], [Uid1], [Uid2], [Uid3], [Uid4], [Desc1], [Desc2], [Desc3], [Desc4], [acttyp], [trnscust], [bfrlable], [permuid], [srtjlstkh]) Values (@kalauid,@Curstate,@acqtyp,@anbrsdno,@anbhvlno,@custuid,@lable,@amvuid,@namuid,@Year,@Desc,@Cost,@Uid1,@Uid2,@Uid3,@Uid4,@Desc1,@Desc2,@Desc3,@Desc4,@acttyp,@trnscust,@bfrlable,@permuid,@srtjlstkh);", table), OleConn);

            adapter.InsertCommand.Parameters.Add("@kalauid", OleDbType.Integer, 0, "kalauid");
            adapter.InsertCommand.Parameters.Add("@Curstate", OleDbType.Integer, 0, "Curstate");
            adapter.InsertCommand.Parameters.Add("@acqtyp", OleDbType.Integer, 0, "acqtyp");
            adapter.InsertCommand.Parameters.Add("@anbrsdno", OleDbType.Integer, 0, "anbrsdno");
            adapter.InsertCommand.Parameters.Add("@anbhvlno", OleDbType.Integer, 0, "anbhvlno");
            adapter.InsertCommand.Parameters.Add("@custuid", OleDbType.Integer, 0, "custuid");
            adapter.InsertCommand.Parameters.Add("@lable", OleDbType.Integer, 0, "lable");
            adapter.InsertCommand.Parameters.Add("@amvuid", OleDbType.Integer, 0, "amvuid");
            adapter.InsertCommand.Parameters.Add("@namuid", OleDbType.VarWChar, 20, "namuid");
            adapter.InsertCommand.Parameters.Add("@Year", OleDbType.Integer, 0, "Year");
            adapter.InsertCommand.Parameters.Add("@Desc", OleDbType.VarWChar, 50, "Desc");
            adapter.InsertCommand.Parameters.Add("@Cost", OleDbType.Currency, 0, "Cost");
            adapter.InsertCommand.Parameters.Add("@Uid1", OleDbType.VarWChar, 50, "Uid1");
            adapter.InsertCommand.Parameters.Add("@Uid2", OleDbType.VarWChar, 50, "Uid2");
            adapter.InsertCommand.Parameters.Add("@Uid3", OleDbType.VarWChar, 50, "Uid3");
            adapter.InsertCommand.Parameters.Add("@Uid4", OleDbType.VarWChar, 50, "Uid4");
            adapter.InsertCommand.Parameters.Add("@Desc1", OleDbType.VarWChar, 50, "Desc1");
            adapter.InsertCommand.Parameters.Add("@Desc2", OleDbType.VarWChar, 50, "Desc2");
            adapter.InsertCommand.Parameters.Add("@Desc3", OleDbType.VarWChar, 50, "Desc3");
            adapter.InsertCommand.Parameters.Add("@Desc4", OleDbType.VarWChar, 50, "Desc4");
            adapter.InsertCommand.Parameters.Add("@acttyp", OleDbType.Integer,0, "acttyp");
            adapter.InsertCommand.Parameters.Add("@trnscust", OleDbType.Integer,0, "trnscust");
            adapter.InsertCommand.Parameters.Add("@bfrlable", OleDbType.Integer, 0, "bfrlable");
            adapter.InsertCommand.Parameters.Add("@permuid", OleDbType.Integer, 10, "permuid");
            adapter.InsertCommand.Parameters.Add("@srtjlstkh", OleDbType.VarWChar, 10, "srtjlstkh");
            adapter.InsertCommand.Connection.Open();

            adapter.Update(dt);
            adapter.InsertCommand.Connection.Close();
        }

        public DataTable creatDataTable(string tbName,int tbNo)
        {
            DataTable dt = new DataTable(tbName);
            if (tbNo == 1001 || tbNo == 1002)
            {
                dt.Columns.Add("kalauid", typeof(Int32));
                dt.Columns.Add("Curstate", typeof(Int32));
                dt.Columns.Add("acqtyp", typeof(int));
                dt.Columns.Add("anbrsdno", typeof(int));
                dt.Columns.Add("anbhvlno", typeof(long));
                dt.Columns.Add("custuid", typeof(int));
                dt.Columns.Add("lable", typeof(int));
                dt.Columns.Add("amvuid", typeof(int));
                dt.Columns.Add("namuid", typeof(string));
                dt.Columns.Add("Year", typeof(int));
                dt.Columns.Add("Desc", typeof(string));
                dt.Columns.Add("Cost", typeof(decimal));
                dt.Columns.Add("transfer", typeof(int));
                dt.Columns.Add("Uid1", typeof(string));
                dt.Columns.Add("Uid2", typeof(string));
                dt.Columns.Add("Uid3", typeof(string));
                dt.Columns.Add("Uid4", typeof(string));
                dt.Columns.Add("Desc1", typeof(string));
                dt.Columns.Add("Desc2", typeof(string));
                dt.Columns.Add("Desc3", typeof(string));
                dt.Columns.Add("Desc4", typeof(string));
                dt.Columns.Add("couid", typeof(int));
            }
            else if (tbNo == 1005)
            {
                dt.Columns.Add("kalauid", typeof(Int32));
                dt.Columns.Add("Curstate", typeof(Int32));
                dt.Columns.Add("acqtyp", typeof(int));
                dt.Columns.Add("anbrsdno", typeof(int));
                dt.Columns.Add("anbhvlno", typeof(long));
                dt.Columns.Add("custuid", typeof(int));
                dt.Columns.Add("lable", typeof(int));
                dt.Columns.Add("amvuid", typeof(int));
                dt.Columns.Add("namuid", typeof(string));
                dt.Columns.Add("Year", typeof(int));
                dt.Columns.Add("Desc", typeof(string));
                dt.Columns.Add("Cost", typeof(decimal));
                dt.Columns.Add("transfer", typeof(int));
                dt.Columns.Add("Uid1", typeof(string));
                dt.Columns.Add("Uid2", typeof(string));
                dt.Columns.Add("Uid3", typeof(string));
                dt.Columns.Add("Uid4", typeof(string));
                dt.Columns.Add("Desc1", typeof(string));
                dt.Columns.Add("Desc2", typeof(string));
                dt.Columns.Add("Desc3", typeof(string));
                dt.Columns.Add("Desc4", typeof(string));
                dt.Columns.Add("optyp", typeof(int));
                dt.Columns.Add("bossnme", typeof(string));
                dt.Columns.Add("bosstkh", typeof(string));
                dt.Columns.Add("dareenme", typeof(string));
                dt.Columns.Add("dareetkh", typeof(string));
                dt.Columns.Add("othernme", typeof(string));
                dt.Columns.Add("othertkh", typeof(string));
                dt.Columns.Add("Trnscust", typeof(int));
                dt.Columns.Add("Divannamno", typeof(string));
                dt.Columns.Add("divannamtkh", typeof(string));
                dt.Columns.Add("divanray", typeof(int));
                dt.Columns.Add("mdtamnt", typeof(int));
                dt.Columns.Add("ComisunNo", typeof(int));
            }
            else if (tbNo == 1003)
            {
                dt.Columns.Add("kalauid", typeof(Int32));
                dt.Columns.Add("Curstate", typeof(Int32));
                dt.Columns.Add("acqtyp", typeof(int));
                dt.Columns.Add("anbrsdno", typeof(int));
                dt.Columns.Add("anbhvlno", typeof(long));
                dt.Columns.Add("custuid", typeof(int));
                dt.Columns.Add("lable", typeof(int));
                dt.Columns.Add("amvuid", typeof(int));
                dt.Columns.Add("namuid", typeof(string));
                dt.Columns.Add("Year", typeof(int));
                dt.Columns.Add("Desc", typeof(string));
                dt.Columns.Add("Cost", typeof(decimal));
                dt.Columns.Add("Uid1", typeof(string));
                dt.Columns.Add("Uid2", typeof(string));
                dt.Columns.Add("Uid3", typeof(string));
                dt.Columns.Add("Uid4", typeof(string));
                dt.Columns.Add("Desc1", typeof(string));
                dt.Columns.Add("Desc2", typeof(string));
                dt.Columns.Add("Desc3", typeof(string));
                dt.Columns.Add("Desc4", typeof(string));
                dt.Columns.Add("acttyp", typeof(int));
                dt.Columns.Add("trnscust", typeof(int));
                dt.Columns.Add("bfrlable", typeof(int));
                dt.Columns.Add("permuid", typeof(int));
                dt.Columns.Add("srtjlstkh", typeof(string));
            }
            else if (tbNo == 1004)
            {
                dt.Columns.Add("custuid", typeof(int));
                dt.Columns.Add("lable", typeof(int));
                dt.Columns.Add("amvuid", typeof(int));
                dt.Columns.Add("namuid", typeof(string));
                dt.Columns.Add("acttyp", typeof(int));
                dt.Columns.Add("permuid", typeof(int));
                dt.Columns.Add("Trnscust", typeof(int));
                dt.Columns.Add("fishno", typeof(string));
                dt.Columns.Add("fishtkh", typeof(string));
                dt.Columns.Add("hesabno", typeof(string));
                dt.Columns.Add("mablagh", typeof(int));
                dt.Columns.Add("Foroshcost", typeof(int));
                dt.Columns.Add("srtjlstkh", typeof(int));
            }
            else if (tbNo == 1006)
            {
                dt.Columns.Add("kalauid", typeof(Int32));
                dt.Columns.Add("Curstate", typeof(Int32));
                dt.Columns.Add("custuid", typeof(int));
                dt.Columns.Add("lable", typeof(string));
                dt.Columns.Add("bossnme", typeof(string));
                dt.Columns.Add("bosstkh", typeof(string));
                dt.Columns.Add("namuid", typeof(string));
                dt.Columns.Add("Year", typeof(int));
                dt.Columns.Add("Desc", typeof(string));
                dt.Columns.Add("Cost", typeof(decimal));
                dt.Columns.Add("Uid1", typeof(string));
                dt.Columns.Add("Uid2", typeof(string));
                dt.Columns.Add("Uid3", typeof(string));
                dt.Columns.Add("Uid4", typeof(string));
                dt.Columns.Add("Desc1", typeof(string));
                dt.Columns.Add("Desc2", typeof(string));
                dt.Columns.Add("Desc3", typeof(string));
                dt.Columns.Add("Desc4", typeof(string));
            }
            return dt;
        }

       // public static AccessDatabaseHelper UniqueInstance
       // {
       //     get { return SingletonCreator.uniqueInstance; }
       // }

       // class SingletonCreator
       // {
        //    static SingletonCreator() { }
         //   internal static readonly AccessDatabaseHelper uniqueInstance = new AccessDatabaseHelper();
        //}
    }
}
