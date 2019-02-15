
namespace Bska.Client.UI.ViewModels
{
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.OleDb;
    using System.Windows.Input;
    using Microsoft.Practices.Unity;
    public sealed class ShowAccessFileViewModel : BaseViewModel
    {
        #region ctor

        public ShowAccessFileViewModel(IUnityContainer container,string tableName,string path)
        {
            this._container = container;
            this.TableName = tableName;
            _path = path;
            this._dialogService = _container.Resolve<IDialogService>();
            this.initializObj();
        }

        #endregion

        #region properties

        public String TableName
        {
            get { return GetValue(() => TableName); }
            set
            {
                SetValue(() => TableName, value);
            }
        }

        public DataTable Collection
        {
            get { return GetValue(() => Collection); }
            set
            {
                SetValue(() => Collection, value);
            }
        }

        public String SearchCriteria
        {
            get { return GetValue(() => SearchCriteria); }
            set
            {
                SetValue(() => SearchCriteria, value);
                this.searchObj();
            }
        }

        #endregion

        #region methods

        private void initializObj()
        {
            Mouse.SetCursor(Cursors.Wait);
            OleDbConnection con = new OleDbConnection();
            con.ConnectionString = @"Provider =Microsoft.ACE.OLEDB.12.0; Data Source = " + _path + "; Persist Security Info = False;";
            try
            {
                con.Open();
                OleDbCommand command = new OleDbCommand();
                command.CommandText = "select * from [" + TableName + "]";
                command.Connection = con;
                OleDbDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dtSchema = dr.GetSchemaTable();
                DataTable dt = new DataTable();
                // You can also use an ArrayList instead of List<> 
                List<DataColumn> listCols = new List<DataColumn>();
                if (dtSchema != null)
                {
                    foreach (DataRow drow in dtSchema.Rows)
                    {
                        string columnName = System.Convert.ToString(drow["ColumnName"]);
                        DataColumn column = new DataColumn(columnName, (Type)(drow["DataType"]));
                        column.Unique = (bool)drow["IsUnique"];
                        column.AllowDBNull = (bool)drow["AllowDBNull"];
                        column.AutoIncrement = (bool)drow["IsAutoIncrement"];
                        listCols.Add(column);
                        dt.Columns.Add(column);
                    }

                }

                // Read rows from DataReader and populate the DataTable 

                while (dr.Read())
                {
                    DataRow dataRow = dt.NewRow();
                    for (int i = 0; i < listCols.Count; i++)
                    {
                        dataRow[((DataColumn)listCols[i])] = dr[i];
                    }

                    dt.Rows.Add(dataRow);
                }
                Collection = dt;
            }
            catch (OleDbException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("'Microsoft.ACE.OLEDB.12.0'"))
                {
                    _dialogService.ShowAlert("توجه", "کامپوننت مورد نیاز برای نمایش فایل روی این سیستم نصب نشده است");
                }
                else
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }

            Mouse.SetCursor(Cursors.Arrow);
        }

        private void searchObj()
        {
            if (!string.IsNullOrEmpty(SearchCriteria))
            {
                if (string.Equals(TableName, "Arrival_hokmmasrafi"))
                {
                    Collection.DefaultView.RowFilter = "CONVERT(kalauid, System.String) LIKE '" + SearchCriteria + "%'";
                }
                else
                {
                    Collection.DefaultView.RowFilter = "lable =" + SearchCriteria + "";
                }
            }
            else
            {
                Collection.DefaultView.RowFilter = null;
            }
        }

        #endregion

        #region commands
        #endregion

        #region fields
        string _path;
        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        #endregion
    }
}
