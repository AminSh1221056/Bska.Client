using Bska.Client.Common;
using Bska.Client.Domain.Entity;
using Bska.Client.Domain.Entity.StoredProcedures;
using Bska.Client.UI.Helper;
using Microsoft.Practices.Unity;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Bska.Client.UI.API.Client
{
    public class BaseInfoService : IBaseInfoService
    {
        #region ctor

        public BaseInfoService(IUnityContainer container)
        {
            this._container = container;
            this._helper = new WebAPIHelper();
            this._dataTableHelper = new DataTableHelper();
            this._beskaProcedures = _container.Resolve<IBskaStoredProcedures>();
        }

        #endregion

        public Task<int> saveStuffAsync(string apiUrl, IProgress<double> progress = null)
        {
            var stuffTask = new Task<int>(() =>
            {
                var result = _helper.GetTItems<StuffHelper[]>(Settings.Default.APIServiceURL + apiUrl);
                if (result != null)
                {
                    if (progress != null)
                    {
                        progress.Report(_currentProgress + 10);
                    }

                    int totalCount = result.Count();
                    //_beskaProcedures.delete_allStuffs();
                    var dt = _dataTableHelper.getStuffTable();
                    int cStuffId = 0;
                    for (int i = 0; i < totalCount; i++)
                    {
                        if(int.TryParse(result[i].StuffId,out cStuffId))
                        {
                            dt.Rows.Add(cStuffId, result[i].StuffId.ToString(), DBNull.Value,
                           result[i].Name, result[i].IsStuff, result[i].StuffType, (object)result[i].FloorNew ?? DBNull.Value,
                           (object)result[i].FloorOld ?? DBNull.Value, (object)result[i].parentId ?? DBNull.Value);
                        }
                    }

                    if (progress != null)
                    {
                        progress.Report(_currentProgress + 35);
                    }
                   return _beskaProcedures.ExecuteNonQuery("dbo.StuffInsert", CommandType.StoredProcedure, dt);
                  //  return _beskaProcedures.UpdateStuffOnUpdateFromServer();
                }
                return -1;
            });
            stuffTask.Start();
            return stuffTask;
        }

        public Task<int> saveUnitAsync(string apiUrl, IProgress<double> progress = null)
        {
            var unitTask = new Task<int>(() =>
            {
                var result = _helper.GetTItems<UnitModel[]>(Settings.Default.APIServiceURL + apiUrl);
                if (result != null)
                {
                    int ok = _beskaProcedures.ExecuteNonQuery("Delete Production.Unit", CommandType.Text, cmdParms: null);
                    int totalCount = result.Count();
                    var dt = _dataTableHelper.getUnitTable();
                    for (int i = 0; i < totalCount; i++)
                    {
                        dt.Rows.Add(result[i].UnitId, result[i].Name, (int)result[i].StuffType, result[i].MathType,
                            (object)result[i].MathNum ?? DBNull.Value,
                     (object)result[i].ParentId ?? DBNull.Value);
                    }
                    if (progress != null)
                    {
                        progress.Report(_currentProgress + 10);
                    }
                    return _beskaProcedures.ExecuteNonQuery("dbo.UnitInsert", CommandType.StoredProcedure, dt);
                }
                return -1;
            });
            unitTask.Start();
            return unitTask;
        }

        //public Task<int> saveAccounCodingAsync(string apiUrl, IProgress<double> progress = null)
        //{
        //    var ts = new Task<int>(() =>
        //    {
        //        var result = _helper.GetTItems<AccountDocCodingModel[]>(Settings.Default.APIServiceURL + apiUrl);
        //        if (result != null)
        //        {
        //            int ok = _beskaProcedures.ExecuteNonQuery("Delete Production.AccountDocumentCoding", CommandType.Text, cmdParms: null);
        //            int totalCount = result.Count();
        //            var dt = _dataTableHelper.getAccountCodingTable();
        //            for (int i = 0; i < totalCount; i++)
        //            {
        //                dt.Rows.Add(result[i].Id,result[i].Name, result[i].AccountCode,UserLog.UniqueInstance.LogedEmployee.EmployeeId,(object)result[i].ParentId ?? DBNull.Value);
        //            }
        //            _beskaProcedures.ExecuteNonQuery("dbo.AccountCodingInsert", CommandType.StoredProcedure, dt);
        //        }
                
        //        if (progress != null)
        //        {
        //            progress.Report(_currentProgress + 5);
        //        }
        //        return 1;
        //    });
        //    ts.Start();
        //    return ts;
        //}

        public Task<int> saveCountryAsync(string countryUrl,string companyUrl, IProgress<double> progress = null)
        {
            var ts = new Task<int>(() =>
            {
                var result = _helper.GetTItems<Country[]>(Settings.Default.APIServiceURL + countryUrl);
                if (result != null)
                {
                    int ok = _beskaProcedures.ExecuteNonQuery("Delete Production.Country", CommandType.Text, cmdParms: null);
                    int totalCount = result.Count();
                    var dt = _dataTableHelper.getCountryTable();
                    for (int i = 0; i < totalCount; i++)
                    {
                        dt.Rows.Add(result[i].CountryId, result[i].CountryName, (object)result[i].CarCorporationId ?? DBNull.Value);
                    }
                    _beskaProcedures.ExecuteNonQuery("dbo.CountryInsert", CommandType.StoredProcedure, dt);
                }

                var result1 = _helper.GetTItems<Company[]>(Settings.Default.APIServiceURL + companyUrl);
                if (result1 != null)
                {
                    int ok = _beskaProcedures.ExecuteNonQuery("Delete Production.Company", CommandType.Text, cmdParms: null);
                    int totalCount = result1.Count();
                    var dt = _dataTableHelper.getCompanyTable();
                    for (int i = 0; i < totalCount; i++)
                    {
                        dt.Rows.Add(result1[i].CompanyId, result1[i].Name, (object)result1[i].CountryId ?? DBNull.Value,
                            result1[i].IsCarCompany);
                    }
                    _beskaProcedures.ExecuteNonQuery("dbo.CompanyInsert", CommandType.StoredProcedure, dt);
                }

                if (progress != null)
                {
                    progress.Report(_currentProgress + 5);
                }
                return 1;
            });
            ts.Start();
            return ts;
        }

        public Task<int> saveInsuranceAsync(string apiUrl, IProgress<double> progress = null)
        {
            var ts = new Task<int>(() =>
            {
                var result = _helper.GetTItems<InsuranceCompany[]>(Settings.Default.APIServiceURL +apiUrl);
                if (result != null)
                {
                    _beskaProcedures.ExecuteNonQuery("Delete Production.InsuranceCompany", CommandType.Text, cmdParms: null);
                    int totalCount = result.Count();
                    var dt = _dataTableHelper.getInsurancecompanyTable();
                    for (int i = 0; i < totalCount; i++)
                    {
                        dt.Rows.Add(result[i].InsuranceId, result[i].Name);
                    }
                    _beskaProcedures.ExecuteNonQuery("dbo.InsuranceCompanyInsert", CommandType.StoredProcedure, dt);
                }
                if (progress != null)
                {
                    progress.Report(_currentProgress + 5);
                }
                return 1;
            });
            ts.Start();
            return ts;
        }

        public Task<int> saveCarCompanyAsync(string apiUrl, IProgress<double> progress = null)
        {
            var ts = new Task<int>(() =>
            {
                var result = _helper.GetTItems<CarDetails[]>(Settings.Default.APIServiceURL + apiUrl);
                if (result != null)
                {
                    _beskaProcedures.ExecuteNonQuery("Delete Production.CarDetails", CommandType.Text, cmdParms: null);
                    int totalCount = result.Count();
                    var dt = _dataTableHelper.getCarDetailsTable();
                    for (int i = 0; i < totalCount; i++)
                    {
                        dt.Rows.Add(result[i].CarDetailsId, (int)result[i].CarType, result[i].SystemType, result[i].Tipe,
                            result[i].Model, (object)result[i].CompanyId ?? DBNull.Value);
                    }
                    _beskaProcedures.ExecuteNonQuery("dbo.CarDetailsInsert", CommandType.StoredProcedure, dt);
                }
                if (progress != null)
                {
                    progress.Report(_currentProgress + 5);
                }
                return 1;
            });
            ts.Start();
            return ts;
        }

        public Task<int> saveOrganAsync(string apiUrl, IProgress<double> progress = null)
        {
            var task = new Task<int>(() =>
            {
                var result = _helper.GetTItems<OrganizationModel[]>(Settings.Default.APIServiceURL + apiUrl);
                if (result != null)
                {
                    _beskaProcedures.ExecuteNonQuery("Delete Production.Organization", CommandType.Text, cmdParms: null);
                    int totalCount = result.Count();
                    var dt = _dataTableHelper.getOrganizationModelTable();
                    for (int i = 0; i < totalCount; i++)
                    {
                        dt.Rows.Add(result[i].EmployeeId, result[i].Name, result[i].BudgetNo);
                    }
                    _beskaProcedures.ExecuteNonQuery("dbo.OrganizationModelInsert", CommandType.StoredProcedure, dt);
                }
                if (progress != null)
                {
                    progress.Report(_currentProgress + 5);
                }
                return 1;
            });
            task.Start();
            return task;
        }

        public Task<int> saveStateAsync(string provinceUrl, string townshipUrl, string zoneUrl, string cityUrl, 
            IProgress<double> progress = null)
        {
            var task = new Task<int>(() =>
            {
                var result = _helper.GetTItems<Province[]>(Settings.Default.APIServiceURL + provinceUrl);
                if (result != null)
                {
                    _beskaProcedures.ExecuteNonQuery("Delete Production.Province", CommandType.Text, cmdParms: null);
                    int totalCount = result.Count();
                    var dt = _dataTableHelper.getProvinceTable();
                    for (int i = 0; i < totalCount; i++)
                    {
                        dt.Rows.Add(result[i].ID, result[i].Name);
                    }
                    _beskaProcedures.ExecuteNonQuery("dbo.ProvinceInsert", CommandType.StoredProcedure, dt);
                }

                var result1 = _helper.GetTItems<TwonShip[]>(Settings.Default.APIServiceURL + townshipUrl);
                if (result1 != null)
                {
                    _beskaProcedures.ExecuteNonQuery("Delete Production.TwonShip", CommandType.Text, cmdParms: null);
                    int totalCount = result1.Count();
                    var dt = _dataTableHelper.getProvinceTable();
                    for (int i = 0; i < totalCount; i++)
                    {
                        dt.Rows.Add(result1[i].ID, result1[i].Name);
                    }
                    _beskaProcedures.ExecuteNonQuery("dbo.TwonShipInsert", CommandType.StoredProcedure, dt);
                }

                var result2 = _helper.GetTItems<Zone[]>(Settings.Default.APIServiceURL + zoneUrl);
                if (result2 != null)
                {
                    _beskaProcedures.ExecuteNonQuery("Delete Production.Zone", CommandType.Text, cmdParms: null);
                    int totalCount = result2.Count();
                    var dt = _dataTableHelper.getProvinceTable();
                    for (int i = 0; i < totalCount; i++)
                    {
                        dt.Rows.Add(result2[i].ID, result2[i].Name);
                    }
                    _beskaProcedures.ExecuteNonQuery("dbo.ZoneInsert", CommandType.StoredProcedure, dt);
                }

                var result3 = _helper.GetTItems<City[]>(Settings.Default.APIServiceURL + cityUrl);
                if (result3 != null)
                {
                    _beskaProcedures.ExecuteNonQuery("Delete Production.City", CommandType.Text, cmdParms: null);
                    int totalCount = result3.Count();
                    var dt = _dataTableHelper.getProvinceTable();
                    for (int i = 0; i < totalCount; i++)
                    {
                        dt.Rows.Add(result3[i].ID, result3[i].Name);
                    }
                    _beskaProcedures.ExecuteNonQuery("dbo.CityInsert", CommandType.StoredProcedure, dt);
                }
                if (progress != null)
                {
                    progress.Report(_currentProgress + 15);
                }
                return 1;
            });
            task.Start();
            return task;
        }

        public Task<int> saveEstateOrganAsync(string apiUrl, IProgress<double> progress = null)
        {
            var task = new Task<int>(() =>
            {
                var result = _helper.GetTItems<EstateOrgan[]>(Settings.Default.APIServiceURL + apiUrl);
                if (result != null)
                {
                    _beskaProcedures.ExecuteNonQuery("Delete Production.EstateOrgan", CommandType.Text, cmdParms: null);
                    int totalCount = result.Count();
                    var dt = _dataTableHelper.getEstateOrganTable();
                    for (int i = 0; i < totalCount; i++)
                    {
                        dt.Rows.Add(result[i].Name, result[i].ProvinceId);
                    }
                    _beskaProcedures.ExecuteNonQuery("dbo.EstateOrganInsert", CommandType.StoredProcedure, dt);
                }
                if (progress != null)
                {
                    progress.Report(_currentProgress + 5);
                }
                return 1;
            });
            task.Start();
            return task;
        }

        #region fields

        private readonly WebAPIHelper _helper;
        private readonly DataTableHelper _dataTableHelper;
        private readonly IBskaStoredProcedures _beskaProcedures;
        private readonly IUnityContainer _container;
        private double _currentProgress;
        public double CurrentProgress { get => _currentProgress; set => _currentProgress=value; }

        #endregion
    }
}
