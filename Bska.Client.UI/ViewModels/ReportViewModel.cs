
namespace Bska.Client.UI.ViewModels
{
    using Microsoft.Reporting.WinForms;
    using System;
    using System.Linq;
    using System.Data;
    using System.Windows.Forms.Integration;
    using Bska.Client.Common;
    using Bska.Client.UI.Helper;
    using System.Windows.Media;
    using System.Collections.Generic;
    using Bska.Client.Repository.Model;
    using System.Reflection;

    public sealed class ReportViewModel : BaseViewModel
    {
        #region ctor

        public ReportViewModel()
        {
            Viewer = new WindowsFormsHost();
            Viewer.FlowDirection = System.Windows.FlowDirection.RightToLeft;
            _objReportViewer = new ReportViewer();
            _objReportViewer.ProcessingMode = ProcessingMode.Remote;
            if (Settings.Default.NETTypeIsDomain)
            {
                _objReportViewer.ServerReport.ReportServerCredentials.NetworkCredentials =
                    new System.Net.NetworkCredential(Settings.Default.SSRSUsername, Settings.Default.SSRSPassword, Settings.Default.SSRSDomainName);
            }
            _objReportViewer.ServerReport.ReportServerUrl = new Uri(Settings.Default.ReportServerUrl);
            _objReportViewer.ShowParameterPrompts = true;
            _objReportViewer.PromptAreaCollapsed = false;
            _objReportViewer.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            _objReportViewer.SetDisplayMode(DisplayMode.Normal);
            _objReportViewer.ZoomMode = ZoomMode.PageWidth;
            this._serverName = Settings.Default.dataSource;
            this._databaseName= Settings.Default.initialCatalog;
            this._userName = Settings.Default.username;
            this._password = GlobalClass.DecryptStringAES(Settings.Default.password, "66Ak679Du4V3qo92");
        }

        //client initalization
        public ReportViewModel(bool isClient)
        {
            Viewer = new WindowsFormsHost();
            Viewer.FlowDirection = System.Windows.FlowDirection.RightToLeft;
            _objReportViewer = new ReportViewer();
            _objReportViewer.ProcessingMode = ProcessingMode.Local;
            _objReportViewer.ShowParameterPrompts = true;
            _objReportViewer.PromptAreaCollapsed = false;
            _objReportViewer.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            _objReportViewer.SetDisplayMode(DisplayMode.Normal);
            _objReportViewer.ZoomMode = ZoomMode.PageWidth;
        }
        #endregion

        #region properties

        public WindowsFormsHost Viewer
        {
            get { return GetValue(() => Viewer); }
            set
            {
                SetValue(() => Viewer, value);
            }
        }

        public String ReportTitle
        {
            get { return GetValue(() => ReportTitle); }
            set
            {
                SetValue(() => ReportTitle, value);
            }
        }

        #endregion

        #region methods

        #region repors items
        public void EmployeeDetailsReport()
        {
            ReportParameter[] parameters = new ReportParameter[6];
            parameters[0] = new ReportParameter("serverName", this._serverName);
            parameters[1] = new ReportParameter("databaseName", this._databaseName);
            parameters[2] = new ReportParameter("userName", this._userName);
            parameters[3] = new ReportParameter("passWord", this._password);
            parameters[4] = new ReportParameter("employeeName", UserLog.UniqueInstance.LogedEmployee.Name);
            parameters[5] = new ReportParameter("parentName", UserLog.UniqueInstance.LogedEmployee.ParentName);

            _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/EmployeeDetails";
            try
            {
                _objReportViewer.ServerReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void MAssetListReport(String OrganizPath, String StrategyPath
            , Int32 queryLevel, Int32 orgId, Int32 strId, string personId, string PersonName,String[] type,
            DateTime? pdate1, DateTime? pdate2, string queryString,bool IsGrouping)
        {
            ReportParameter[] parameters = new ReportParameter[17];
            if (IsGrouping)
            {
                _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/MAssetGroupListReport";
            }
            else
            {
                _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/MAssetListReport";
            }

            parameters[0] = new ReportParameter("OrganizPath", OrganizPath);
            parameters[1] = new ReportParameter("StrategyPath", StrategyPath);
            parameters[2] = new ReportParameter("queryLevel", queryLevel.ToString());
            parameters[3] = new ReportParameter("orgId", orgId.ToString());
            parameters[4] = new ReportParameter("strId", strId.ToString());

            parameters[5] = new ReportParameter("PersonName", PersonName);
            parameters[6] = new ReportParameter("personId", personId);

            parameters[7] = new ReportParameter("AType", type);
            parameters[8] = new ReportParameter("employeeName", UserLog.UniqueInstance.LogedEmployee.Name);
            parameters[9] = new ReportParameter("parentName", UserLog.UniqueInstance.LogedEmployee.ParentName);

            string date1 = null;
            if (pdate1.HasValue)
            {
                date1 = pdate1.Value.ToString("MM/dd/yyyy");
            }

            string date2 = null;
            if (pdate2.HasValue)
            {
                date2 = pdate2.Value.ToString("MM/dd/yyyy");
            }

            parameters[10] = new ReportParameter("fromDate", date1);
            parameters[11] = new ReportParameter("toDate", date2);
            parameters[12] = new ReportParameter("queryString", queryString);

            //connection string credential
            parameters[13] = new ReportParameter("serverName", this._serverName);
            parameters[14] = new ReportParameter("databaseName", this._databaseName);
            parameters[15] = new ReportParameter("userName", this._userName);
            parameters[16] = new ReportParameter("passWord", this._password);

            try
            {
                _objReportViewer.ServerReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void MAssetStoreListReport(Int32 queryLevel, Int32 storeId, Int32 stdId, string storeName, string storeAddress
           , String[] type, DateTime? pdate1, DateTime? pdate2, string queryString, bool IsGrouping)
        {
            ReportParameter[] parameters = new ReportParameter[15];
            if (IsGrouping)
            {
                _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/MAssetStoreGroupListReport";
            }
            else
            {
                _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/MAssetStoreListReport";
            }
            parameters[0] = new ReportParameter("queryLevel", queryLevel.ToString());
            parameters[1] = new ReportParameter("stdId", stdId.ToString());
            parameters[2] = new ReportParameter("storeId", storeId.ToString());
            parameters[3] = new ReportParameter("storeName", storeName);
            parameters[4] = new ReportParameter("storeAddress", storeAddress);
            parameters[5] = new ReportParameter("AType", type);
            parameters[6] = new ReportParameter("employeeName", UserLog.UniqueInstance.LogedEmployee.Name);
            parameters[7] = new ReportParameter("parentName", UserLog.UniqueInstance.LogedEmployee.ParentName);

            string date1 = null;
            if (pdate1.HasValue)
            {
                date1 = pdate1.Value.ToString("MM/dd/yyyy");
            }

            string date2 = null;
            if (pdate2.HasValue)
            {
                date2 = pdate2.Value.ToString("MM/dd/yyyy");
            }

            parameters[8] = new ReportParameter("fromDate", date1);
            parameters[9] = new ReportParameter("toDate", date2);
            parameters[10] = new ReportParameter("queryString", queryString);

            //connection string credential
            parameters[11] = new ReportParameter("serverName", this._serverName);
            parameters[12] = new ReportParameter("databaseName", this._databaseName);
            parameters[13] = new ReportParameter("userName", this._userName);
            parameters[14] = new ReportParameter("passWord", this._password);
            try
            {
                _objReportViewer.ServerReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DocumentReport(string docNo,int DocumentType,string queryString,string transfree,int? accountNo
            ,string desc1,string desc2,string desc3)
        {
            _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/DocumentReport";
            ReportParameter[] parameters = new ReportParameter[14];
            parameters[0] = new ReportParameter("docNo", docNo);
            parameters[1] = new ReportParameter("employeeName", UserLog.UniqueInstance.LogedEmployee.Name);
            parameters[2] = new ReportParameter("parentName", UserLog.UniqueInstance.LogedEmployee.ParentName);
            parameters[3] = new ReportParameter("DocumentType", DocumentType.ToString());
            parameters[4] = new ReportParameter("queryString", queryString);
            parameters[5] = new ReportParameter("Transfree", transfree);
            parameters[6] = new ReportParameter("desc1", desc1);
            parameters[7] = new ReportParameter("desc2", desc2);
            parameters[8] = new ReportParameter("desc3", desc3);

            string accno = null;
            if (accountNo.HasValue)
            {
                accno = accountNo.Value.ToString();
            }

            parameters[9] = new ReportParameter("accountNo", accno);

            //connection string credential
            parameters[10] = new ReportParameter("serverName", this._serverName);
            parameters[11] = new ReportParameter("databaseName", this._databaseName);
            parameters[12] = new ReportParameter("userName", this._userName);
            parameters[13] = new ReportParameter("passWord", this._password);
            try
            {
                _objReportViewer.ServerReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void StoreBillReport(string billNo,string queryString,string desc1,string desc2,int accountNo
            ,string desc3,string desc4,string desc5)
        {
            _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/BillReport";
            ReportParameter[] parameters = new ReportParameter[14];
            parameters[0] = new ReportParameter("billNo", billNo);
            parameters[1] = new ReportParameter("employeeName", UserLog.UniqueInstance.LogedEmployee.Name);
            parameters[2] = new ReportParameter("parentName", UserLog.UniqueInstance.LogedEmployee.ParentName);
            parameters[3] = new ReportParameter("queryString", queryString);
            parameters[4] = new ReportParameter("desc1", desc1);
            parameters[5] = new ReportParameter("desc2", desc2);
            parameters[6] = new ReportParameter("accountNo", accountNo.ToString());
            parameters[7] = new ReportParameter("desc3", desc3);
            parameters[8] = new ReportParameter("desc4", desc4);
            parameters[9] = new ReportParameter("desc5", desc5);

            //connection string credential
            parameters[10] = new ReportParameter("serverName", this._serverName);
            parameters[11] = new ReportParameter("databaseName", this._databaseName);
            parameters[12] = new ReportParameter("userName", this._userName);
            parameters[13] = new ReportParameter("passWord", this._password);
            try
            {
                _objReportViewer.ServerReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch(System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void AllBillsReport(int? AcqType,int? storeId,DateTime? pdate1,DateTime? pdate2,string billNo)
        {
            _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/StoreBillsReport";

            string acqtype = null;
            if (AcqType.HasValue)
            {
                acqtype = acqtype.ToString();
            }

            string stid = null;
            if (storeId.HasValue)
            {
                stid = storeId.ToString();
            }

            string date1 = null;
            if (pdate1.HasValue)
            {
                date1 = pdate1.Value.ToString("MM/dd/yyyy");
            }

            string date2 = null;
            if (pdate2.HasValue)
            {
                date2 = pdate2.Value.ToString("MM/dd/yyyy");
            }
            ReportParameter[] parameters = new ReportParameter[11];
            parameters[0] = new ReportParameter("AcqType", acqtype);
            parameters[1] = new ReportParameter("employeeName", UserLog.UniqueInstance.LogedEmployee.Name);
            parameters[2] = new ReportParameter("parentName", UserLog.UniqueInstance.LogedEmployee.ParentName);
            parameters[3] = new ReportParameter("storeId", stid);
            parameters[4] = new ReportParameter("date1", date1);
            parameters[5] = new ReportParameter("date2", date2);
            parameters[6] = new ReportParameter("billNo", billNo);

            //connection string credential
            parameters[7] = new ReportParameter("serverName", this._serverName);
            parameters[8] = new ReportParameter("databaseName", this._databaseName);
            parameters[9] = new ReportParameter("userName", this._userName);
            parameters[10] = new ReportParameter("passWord", this._password);
            try
            {
                _objReportViewer.ServerReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AllDocumentsReport(string[] documentType,int? storeId)
        {
            _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/DocumentListReport";
            
            string stid = null;
            if (storeId.HasValue)
            {
                stid = storeId.ToString();
            }

            ReportParameter[] parameters = new ReportParameter[8];
            parameters[0] = new ReportParameter("documentType",documentType);
            parameters[1] = new ReportParameter("employeeName", UserLog.UniqueInstance.LogedEmployee.Name);
            parameters[2] = new ReportParameter("parentName", UserLog.UniqueInstance.LogedEmployee.ParentName);
            parameters[3] = new ReportParameter("storeId", stid);

            //connection string credential
            parameters[4] = new ReportParameter("serverName", this._serverName);
            parameters[5] = new ReportParameter("databaseName", this._databaseName);
            parameters[6] = new ReportParameter("userName", this._userName);
            parameters[7] = new ReportParameter("passWord", this._password);
            try
            {
                _objReportViewer.ServerReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void mAssetHistoryReports(string[] curState,string[] stuffTypes,string queryString)
        {
            _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/MAssetHistoryReport";
            ReportParameter[] parameters = new ReportParameter[9];
          
            parameters[0] = new ReportParameter("curState", curState);
            parameters[1] = new ReportParameter("employeeName", UserLog.UniqueInstance.LogedEmployee.Name);
            parameters[2] = new ReportParameter("parentName", UserLog.UniqueInstance.LogedEmployee.ParentName);
            parameters[3] = new ReportParameter("assetType",stuffTypes);
            parameters[4] = new ReportParameter("queryString", queryString);

            //connection string credential
            parameters[5] = new ReportParameter("serverName", this._serverName);
            parameters[6] = new ReportParameter("databaseName", this._databaseName);
            parameters[7] = new ReportParameter("userName", this._userName);
            parameters[8] = new ReportParameter("passWord", this._password);
            try
            {
                _objReportViewer.ServerReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AccountDocumentListReport(int accountDocType,string[] accountTypes)
        {
            _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/AccountDocumentListReport";
            ReportParameter[] parameters = new ReportParameter[7];
            parameters[0] = new ReportParameter("AccountType", accountDocType.ToString());
            parameters[1] = new ReportParameter("employeeName", UserLog.UniqueInstance.LogedEmployee.Name);
            parameters[2] = new ReportParameter("parentName", UserLog.UniqueInstance.LogedEmployee.ParentName);

            //connection string credential
            parameters[3] = new ReportParameter("serverName", this._serverName);
            parameters[4] = new ReportParameter("databaseName", this._databaseName);
            parameters[5] = new ReportParameter("userName", this._userName);
            parameters[6] = new ReportParameter("passWord", this._password);
            try
            {
                _objReportViewer.ServerReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public void mAssetItemsReports(string assetName, string type, Int64 assetId, Int32? label,int itemsType)
        {
            if (itemsType == 1)
            {
                _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/MAssetSplitsReport";
            }
            else if (itemsType == 2)
            {
                _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/MAssetOrdersReport";
            }
            else if (itemsType == 3)
            {
                _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/MAssetProceedingsReport";
            }
            else if (itemsType == 4)
            {
                _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/MAssetDocumentsReport";
            }
            else if (itemsType == 5)
            {
                _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/MAssetAccountDocumentsReport";
            }

            string mLabel = null;
            if (label.HasValue)
            {
                mLabel = label.ToString();
            }
            ReportParameter[] parameters = new ReportParameter[10];
            parameters[0] = new ReportParameter("mAssetType", type);
            parameters[1] = new ReportParameter("employeeName", UserLog.UniqueInstance.LogedEmployee.Name);
            parameters[2] = new ReportParameter("parentName", UserLog.UniqueInstance.LogedEmployee.ParentName);
            parameters[3] = new ReportParameter("mAssetId", assetId.ToString());
            parameters[4] = new ReportParameter("mAssetName", assetName);
            parameters[5] = new ReportParameter("mAssetLabel", mLabel);

            //connection string credential
            parameters[6] = new ReportParameter("serverName", this._serverName);
            parameters[7] = new ReportParameter("databaseName", this._databaseName);
            parameters[8] = new ReportParameter("userName", this._userName);
            parameters[9] = new ReportParameter("passWord", this._password);
            try
            {
                _objReportViewer.ServerReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AccountDocumentBookByAcqTypeReport(string accountType,Int32 bookType,Int32 acqType,string[] accountTypes)
        {
            _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/AccountDocumentBookByAcqTypeReport";
            ReportParameter[] parameters = new ReportParameter[10];
            parameters[0] = new ReportParameter("AccountType", accountType);
            parameters[1] = new ReportParameter("employeeName", UserLog.UniqueInstance.LogedEmployee.Name);
            parameters[2] = new ReportParameter("parentName", UserLog.UniqueInstance.LogedEmployee.ParentName);
            parameters[3] = new ReportParameter("bookType", bookType.ToString());
            parameters[4] = new ReportParameter("acqType", acqType.ToString());
            parameters[5] = new ReportParameter("AccountTypes", accountTypes);

            //connection string credential
            parameters[6] = new ReportParameter("serverName", this._serverName);
            parameters[7] = new ReportParameter("databaseName", this._databaseName);
            parameters[8] = new ReportParameter("userName", this._userName);
            parameters[9] = new ReportParameter("passWord", this._password);
            try
            {
                _objReportViewer.ServerReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void MAssetDetailsReport(Int64 assetId,string paramDesc)
        {
            _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/MAssetDetailsReport";
            string[] items = paramDesc.Split('|');
            if (items.Length == 8)
            {
                ReportParameter[] parameters = new ReportParameter[15];
                parameters[0] = new ReportParameter("employeeName", UserLog.UniqueInstance.LogedEmployee.Name);
                parameters[1] = new ReportParameter("parentName", UserLog.UniqueInstance.LogedEmployee.ParentName);
                parameters[2] = new ReportParameter("mAssetId", assetId.ToString());
                parameters[3] = new ReportParameter("desc1", items[0]);
                parameters[4] = new ReportParameter("desc2", items[1]);
                parameters[5] = new ReportParameter("desc3", items[2]);
                parameters[6] = new ReportParameter("desc4", items[3]);
                parameters[7] = new ReportParameter("uid1", items[4]);
                parameters[8] = new ReportParameter("uid2", items[5]);
                parameters[9] = new ReportParameter("uid3", items[6]);
                parameters[10] = new ReportParameter("uid4", items[7]);

                //connection string credential
                parameters[11] = new ReportParameter("serverName", this._serverName);
                parameters[12] = new ReportParameter("databaseName", this._databaseName);
                parameters[13] = new ReportParameter("userName", this._userName);
                parameters[14] = new ReportParameter("passWord", this._password);
                try
                {
                    _objReportViewer.ServerReport.SetParameters(parameters);
                    _objReportViewer.RefreshReport();
                    Viewer.Child = _objReportViewer;
                }
                catch (System.Net.WebException ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void OrderConfirmReport(Int64 orderId,string managerName,string stuffHonestName,string orderType)
        {
            _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/OrderConfirmReport";
            ReportParameter[] parameters = new ReportParameter[10];
            parameters[0] = new ReportParameter("employeeName", UserLog.UniqueInstance.LogedEmployee.Name);
            parameters[1] = new ReportParameter("parentName", UserLog.UniqueInstance.LogedEmployee.ParentName);
            parameters[2] = new ReportParameter("ManagerName", managerName);
            parameters[3] = new ReportParameter("StuffHonestName", stuffHonestName);
            parameters[4] = new ReportParameter("orderId", orderId.ToString());
            parameters[5] = new ReportParameter("orderType", orderType);

            //connection string credential
            parameters[6] = new ReportParameter("serverName", this._serverName);
            parameters[7] = new ReportParameter("databaseName", this._databaseName);
            parameters[8] = new ReportParameter("userName", this._userName);
            parameters[9] = new ReportParameter("passWord", this._password);
            try
            {
                _objReportViewer.ServerReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void OrderUserHistoryReports(Int64 orderDetailsId)
        {
            _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/OrderUserHistoryReport";
            ReportParameter[] parameters = new ReportParameter[7];
            parameters[0] = new ReportParameter("employeeName", UserLog.UniqueInstance.LogedEmployee.Name);
            parameters[1] = new ReportParameter("parentName", UserLog.UniqueInstance.LogedEmployee.ParentName);
            parameters[2] = new ReportParameter("orderDetailsId", orderDetailsId.ToString());

            //connection string credential
            parameters[3] = new ReportParameter("serverName", this._serverName);
            parameters[4] = new ReportParameter("databaseName", this._databaseName);
            parameters[5] = new ReportParameter("userName", this._userName);
            parameters[6] = new ReportParameter("passWord", this._password);
            try
            {
                _objReportViewer.ServerReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ProceedingListReport(string[] pType,string query)
        {
            _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/ProceedingListReport";
            ReportParameter[] parameters = new ReportParameter[8];
            parameters[0] = new ReportParameter("employeeName", UserLog.UniqueInstance.LogedEmployee.Name);
            parameters[1] = new ReportParameter("parentName", UserLog.UniqueInstance.LogedEmployee.ParentName);
            parameters[2] = new ReportParameter("pType", pType);
            parameters[3] = new ReportParameter("queryString", query);

            //connection string credential
            parameters[4] = new ReportParameter("serverName", this._serverName);
            parameters[5] = new ReportParameter("databaseName", this._databaseName);
            parameters[6] = new ReportParameter("userName", this._userName);
            parameters[7] = new ReportParameter("passWord", this._password);
            try
            {
                _objReportViewer.ServerReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void MAssetInitialListReport(string title,string filtersearch,int queryLevel=401)
        {
            _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/InitialMAssetReport";
          
            ReportParameter[] parameters = new ReportParameter[9];
            parameters[0] = new ReportParameter("employeeName", UserLog.UniqueInstance.LogedEmployee.Name);
            parameters[1] = new ReportParameter("parentName", UserLog.UniqueInstance.LogedEmployee.ParentName);
            parameters[2] = new ReportParameter("title", title);
            parameters[3] = new ReportParameter("queryLevel", queryLevel.ToString());
            parameters[4] = new ReportParameter("filterSearch", filtersearch);

            //connection string credential
            parameters[5] = new ReportParameter("serverName", this._serverName);
            parameters[6] = new ReportParameter("databaseName", this._databaseName);
            parameters[7] = new ReportParameter("userName", this._userName);
            parameters[8] = new ReportParameter("passWord", this._password);
            try
            {
                _objReportViewer.ServerReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void MunitionIndentReport(string[] AType, string queryString,bool isGrouping)
        {
            _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/MunitionIndentReport";

            ReportParameter[] parameters = new ReportParameter[9];
            parameters[0] = new ReportParameter("employeeName", UserLog.UniqueInstance.LogedEmployee.Name);
            parameters[1] = new ReportParameter("parentName", UserLog.UniqueInstance.LogedEmployee.ParentName);
            parameters[2] = new ReportParameter("AType", AType);
            parameters[3] = new ReportParameter("queryString", queryString);
            parameters[4] = new ReportParameter("isGrouping", isGrouping.ToString());

            //connection string credential
            parameters[5] = new ReportParameter("serverName", this._serverName);
            parameters[6] = new ReportParameter("databaseName", this._databaseName);
            parameters[7] = new ReportParameter("userName", this._userName);
            parameters[8] = new ReportParameter("passWord", this._password);
            try
            {
                _objReportViewer.ServerReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void MeterBillListReport(string queryString,string[] mType)
        {
            _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/MeterBillListReport";

            ReportParameter[] parameters = new ReportParameter[8];
            parameters[0] = new ReportParameter("employeeName", UserLog.UniqueInstance.LogedEmployee.Name);
            parameters[1] = new ReportParameter("parentName", UserLog.UniqueInstance.LogedEmployee.ParentName);
            parameters[2] = new ReportParameter("mType", mType);
            parameters[3] = new ReportParameter("queryString", queryString);

            //connection string credential
            parameters[4] = new ReportParameter("serverName", this._serverName);
            parameters[5] = new ReportParameter("databaseName", this._databaseName);
            parameters[6] = new ReportParameter("userName", this._userName);
            parameters[7] = new ReportParameter("passWord", this._password);
            try
            {
                _objReportViewer.ServerReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SupplierIndentReport(string[] AType, string queryString, bool isGrouping,string supplierName,int supplierId,bool allSubOrder)
        {
            _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/SupplierIndentListReport";

            ReportParameter[] parameters = new ReportParameter[12];
            parameters[0] = new ReportParameter("employeeName", UserLog.UniqueInstance.LogedEmployee.Name);
            parameters[1] = new ReportParameter("parentName", UserLog.UniqueInstance.LogedEmployee.ParentName);
            parameters[2] = new ReportParameter("AType", AType);
            parameters[3] = new ReportParameter("queryString", queryString);
            parameters[4] = new ReportParameter("isGrouping", isGrouping.ToString());
            parameters[5] = new ReportParameter("supplierName", supplierName);
            parameters[6] = new ReportParameter("supplierId", supplierId.ToString());
            parameters[7] = new ReportParameter("allSubOrder", allSubOrder.ToString());

            //connection string credential
            parameters[8] = new ReportParameter("serverName", this._serverName);
            parameters[9] = new ReportParameter("databaseName", this._databaseName);
            parameters[10] = new ReportParameter("userName", this._userName);
            parameters[11] = new ReportParameter("passWord", this._password);
            try
            {
                _objReportViewer.ServerReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void MAssetBelongingListReport(Int64 parnetAssetId,string parentAssetName)
        {
            _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/ParentAssetBelongingListReport";

            ReportParameter[] parameters = new ReportParameter[8];
            parameters[0] = new ReportParameter("employeeName", UserLog.UniqueInstance.LogedEmployee.Name);
            parameters[1] = new ReportParameter("parentName", UserLog.UniqueInstance.LogedEmployee.ParentName);
            parameters[2] = new ReportParameter("parentId", parnetAssetId.ToString());
            parameters[3] = new ReportParameter("parentStuffName", parentAssetName);

            //connection string credential
            parameters[4] = new ReportParameter("serverName", this._serverName);
            parameters[5] = new ReportParameter("databaseName", this._databaseName);
            parameters[6] = new ReportParameter("userName", this._userName);
            parameters[7] = new ReportParameter("passWord", this._password);
            try
            {
                _objReportViewer.ServerReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK,System.Windows.MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AccountdocMainListReport(string titleDescription,string accountNo,int? masterId)
        {
            _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/AccountDocumentMainListReport";
            ReportParameter[] parameters = new ReportParameter[9];
            parameters[0] = new ReportParameter("employeeName", UserLog.UniqueInstance.LogedEmployee.Name);
            parameters[1] = new ReportParameter("parentName", UserLog.UniqueInstance.LogedEmployee.ParentName);
            parameters[2] = new ReportParameter("titleDescription", titleDescription);
            parameters[3] = new ReportParameter("accountNo", accountNo);

            string mId = null;
            if (masterId.HasValue)
            {
                 mId = masterId.ToString();
            }

            parameters[4] = new ReportParameter("masterId", mId);
            //connection string credential
            parameters[5] = new ReportParameter("serverName", this._serverName);
            parameters[6] = new ReportParameter("databaseName", this._databaseName);
            parameters[7] = new ReportParameter("userName", this._userName);
            parameters[8] = new ReportParameter("passWord", this._password);
            try
            {
                _objReportViewer.ServerReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AccountDocMainGroupedReport(int masterId,string accountDate)
        {
            _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Reports/AccountDocumentMainGroupedReport";
            ReportParameter[] parameters = new ReportParameter[8];
            parameters[0] = new ReportParameter("employeeName", UserLog.UniqueInstance.LogedEmployee.Name);
            parameters[1] = new ReportParameter("parentName", UserLog.UniqueInstance.LogedEmployee.ParentName);
            parameters[2] = new ReportParameter("masterId", masterId.ToString());
            parameters[3] = new ReportParameter("accountDate", accountDate);

            //connection string credential
            parameters[4] = new ReportParameter("serverName", this._serverName);
            parameters[5] = new ReportParameter("databaseName", this._databaseName);
            parameters[6] = new ReportParameter("userName", this._userName);
            parameters[7] = new ReportParameter("passWord", this._password);
            try
            {
                _objReportViewer.ServerReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region help items

        public void showMainWindowHelp()
        {
            _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Help/MainWindowHelp";
            ReportParameter[] parameters = new ReportParameter[4];
            parameters[0] = new ReportParameter("serverName", this._serverName);
            parameters[1] = new ReportParameter("databaseName", this._databaseName);
            parameters[2] = new ReportParameter("userName", this._userName);
            parameters[3] = new ReportParameter("passWord", this._password);

            try
            {
                _objReportViewer.ServerReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void showGlobalSinglePageHelp(string reportCode)
        {
            _objReportViewer.ServerReport.ReportPath = "/Bska.Client.Help/GlobalSinglePageHelp";
            ReportParameter[] parameters = new ReportParameter[5];
            parameters[0] = new ReportParameter("serverName", this._serverName);
            parameters[1] = new ReportParameter("databaseName", this._databaseName);
            parameters[2] = new ReportParameter("userName", this._userName);
            parameters[3] = new ReportParameter("passWord", this._password);
            parameters[4] = new ReportParameter("reportCode", reportCode);

            try
            {
                _objReportViewer.ServerReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region client Reports

        public void reportBookHistoryClient(System.Collections.IEnumerable items)
        {
            ReportParameter[] parameters = new ReportParameter[2];
            // _objReportViewer.LocalReport.ReportPath = @"\Reports\Client\AssetBookHistoryReport.rdlc";
            ReportDataSource reportDataSource1 = new ReportDataSource();
            this._itemsBindingSource = new System.Windows.Forms.BindingSource();
            this._itemsBindingSource.DataSource = items;

            reportDataSource1.Name = "DataSet1";
            reportDataSource1.Value = this._itemsBindingSource;
            this._objReportViewer.LocalReport.DataSources.Add(reportDataSource1);
            this._objReportViewer.LocalReport.ReportPath =AppDomain.CurrentDomain.BaseDirectory
                + @"/Reports/Client/AssetBookHistoryReport.rdlc";

            parameters[0] = new ReportParameter("employeeName", UserLog.UniqueInstance.LogedEmployee.Name);
            parameters[1] = new ReportParameter("parentName", UserLog.UniqueInstance.LogedEmployee.ParentName);

            try
            {
                _objReportViewer.LocalReport.SetParameters(parameters);
                _objReportViewer.RefreshReport();
                Viewer.Child = _objReportViewer;
            }
            catch (System.Net.WebException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK
                    , System.Windows.MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #endregion

        #region commands
        #endregion

        #region fields

        private readonly ReportViewer _objReportViewer;
        private System.Windows.Forms.BindingSource _itemsBindingSource;
        private readonly string _serverName;
        private readonly string _databaseName;
        private readonly string _userName;
        private readonly string _password;

        #endregion
    }
}
