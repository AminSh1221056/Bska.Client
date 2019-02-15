
namespace Bska.Client.UI.Helper
{
    using Bska.Client.Domain.Entity;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Linq;
    using System.Linq;
    using Domain.Entity.StoredProcedures;
    using Domain.Concrete;
    using System.Data;
    using System.Data.SqlClient;
    using System.Security.Cryptography.X509Certificates;

    public class SeedDataHelper
    {
        private readonly IBskaStoredProcedures _bskaProcedure;
        public SeedDataHelper()
        {
            this._bskaProcedure = new BskaContext();
        }

        public List<Country> GetCountry()
        {
            List<Country> countries = null;
            string command = "Select * From Production.Country";
            using(IDataReader reader = _bskaProcedure.ExecuteReader(command,CommandType.Text,null))
            {
              countries=reader.Select(r => r.FromDataReaderCountry()).ToList();
            }
            return countries;
        }

        public List<Company> GetCompany(int countryId)
        {
            string command = "Select CountryId,Name,CompanyId From Production.Company "+
                "Where CountryId=@countryId And IsCarCompany=0";
            List<Company> companies = null;
            SqlParameter param = new SqlParameter("countryId", countryId);
            using (IDataReader reader = _bskaProcedure.ExecuteReader(command, CommandType.Text, new SqlParameter[] { param }))
            {
                companies = reader.Select(r => r.FromDataReaderCompany()).ToList();
            }
            return companies;
        }

        public List<InsuranceCompany> GetInsuranceComapny()
        {
            string command = "select InsuranceId,Name from Production.InsuranceCompany";
            List<InsuranceCompany> insuramceCo = null;
            using (IDataReader reader = _bskaProcedure.ExecuteReader(command, CommandType.Text, null))
            {
                insuramceCo = reader.Select(r => r.FromDataReaderInsuranceCo()).ToList();
            }
            return insuramceCo;
        }
        
        public List<Country> GetCarCountry(List<int> countryIds)
        {
            Int32[] allIds = countryIds.ToArray();
            string command = string.Format("select CountryId,CountryName,CarCorporationId from Production.Country where CountryId in ({0})", string.Join(", ", allIds));
            List<Country> countries = null;
            using (IDataReader reader = _bskaProcedure.ExecuteReader(command, CommandType.Text, null))
            {
                countries = reader.Select(r => r.FromDataReaderCountry()).ToList();
            }
            return countries;
        }

        public List<Company> GetCarCompany(int countryId,bool byCountry=true)
        {
            string command = "";
            if (!byCountry)
            {
                command = "select CountryId,Name,CompanyId from Production.Company where IsCarCompany=1";
            }
            else
            {
                command = "select CountryId,Name,CompanyId from Production.Company where IsCarCompany=1 And CountryId=@CountryId";
            }
            List<Company> companies = null;
            SqlParameter param = new SqlParameter("countryId", countryId);
            using (IDataReader reader = _bskaProcedure.ExecuteReader(command, CommandType.Text, new SqlParameter[] { param }))
            {
                companies = reader.Select(r => r.FromDataReaderCompany()).ToList();
            }
            return companies;
        }

        public List<Company> GetRelatedCompanies(List<int> countryIds)
        {
            Int32[] allIds = countryIds.ToArray();
            string command = string.Format("select CountryId,Name,CompanyId from Production.Company where CountryId in ({0}) And IsCarCompany=1", string.Join(", ", allIds));
            List<Company> companies = null;
            using (IDataReader reader = _bskaProcedure.ExecuteReader(command, CommandType.Text, null))
            {
                companies = reader.Select(r => r.FromDataReaderCompany()).ToList();
            }
            return companies;
        }

        public List<CarDetails> GetCarDetails(int companyId, String carType,Boolean getAll=false)
        {
            string command = "";
            if (getAll)
            {
                command = "select CarDetailsId,CarType,CompanyId,Model,SystemType,Tipe from Production.CarDetails";
            }
            else
            {
                command = "select CarDetailsId,CarType,CompanyId,Model,SystemType,Tipe from Production.CarDetails where CompanyId=@companyId And CarType=@carType";
            }

            List<CarDetails> carDetails = null;
            SqlParameter param = new SqlParameter("companyId", companyId);
            SqlParameter param1 = new SqlParameter("carType", carType);
            using (IDataReader reader = _bskaProcedure.ExecuteReader(command, CommandType.Text, new SqlParameter[] { param,param1 }))
            {
                carDetails = reader.Select(r => r.FromDataReaderCarDetails()).ToList();
            }
            return carDetails;
        }

        public List<CarDetails> GetRelatedCarDetails(List<int> companyIds)
        {
            Int32[] allIds = companyIds.ToArray();
            string command = string.Format("select CarDetailsId,CarType,CompanyId,Model,SystemType,Tipe from Production.CarDetails where CompanyId in ({0})", string.Join(", ", allIds));
            List<CarDetails> carDetails = null;
            using (IDataReader reader = _bskaProcedure.ExecuteReader(command, CommandType.Text, null))
            {
                carDetails = reader.Select(r => r.FromDataReaderCarDetails()).ToList();
            }
            return carDetails;
        }

        public List<OrganizationModel> GetOrganizations()
        {
            string command = "select BudgetNo,Name from Production.Organization";
            List<OrganizationModel> organization = null;
            if (UserLog.UniqueInstance.LogedEmployee == null) return null;
            int bgc = UserLog.UniqueInstance.LogedEmployee.BudgetNo;
            using (IDataReader reader = _bskaProcedure.ExecuteReader(command, CommandType.Text, null))
            {
                organization = reader.Select(r => r.FromDataReaderOrganization())
                    .Where(e=>e.BudgetNo!=bgc).ToList();
            }
            return organization;
        }

        public List<Province> GetProvinces()
        {
            string command = "select ID,Name from Production.Province";
            List<Province> Provinces = null;
            using (IDataReader reader = _bskaProcedure.ExecuteReader(command, CommandType.Text, null))
            {
                Provinces = reader.Select(r => r.FromDataReaderProvince()).ToList();
            }
            return Provinces;
        }

        public List<TwonShip> GetTwonShips(String provinceId)
        {
            if (String.IsNullOrEmpty(provinceId))
            {
                return null;
            }

            string command = "select ID,Name from Production.TwonShip Where ID LIKE '"+provinceId+"%'";
            List<TwonShip> items = null;
            using (IDataReader reader = _bskaProcedure.ExecuteReader(command, CommandType.Text, null))
            {
                items = reader.Select(r => r.FromDataReaderTwonShip()).ToList();
            }
            return items;
        }

        public List<Zone> GetZones(String twonshipId)
        {
            if (String.IsNullOrEmpty(twonshipId))
            {
                return null;
            }

            string command = "select ID,Name from Production.Zone Where ID LIKE '"+twonshipId+"%'";
            SqlParameter param = new SqlParameter("twonshipId", twonshipId);
            List<Zone> items = null;
            using (IDataReader reader = _bskaProcedure.ExecuteReader(command, CommandType.Text, new SqlParameter[] { param }))
            {
                items = reader.Select(r => r.FromDataReaderZone()).ToList();
            }
            return items;
        }

        public List<City> GetCities(String zoneId)
        {
            if (String.IsNullOrEmpty(zoneId))
            {
                return null;
            }

            string command = "select ID,Name from Production.City Where ID LIKE '%"+zoneId+"%'";
            SqlParameter param = new SqlParameter("zoneId", zoneId);
            List<City> items = null;
            using (IDataReader reader = _bskaProcedure.ExecuteReader(command, CommandType.Text, new SqlParameter[] { param }))
            {
                items = reader.Select(r => r.FromDataReaderCity()).ToList();
            }
            return items;
        }

        public List<EstateOrgan> GetEstateOrganbyProvinceId(string provinceId)
        {
            if (String.IsNullOrEmpty(provinceId))
            {
                return null;
            }

            string command = "select Id,Name,ProvinceId from Production.EstateOrgan Where ProvinceId LIKE '%" + provinceId + "%'";
            List<EstateOrgan> items = null;
            using (IDataReader reader = _bskaProcedure.ExecuteReader(command, CommandType.Text, null))
            {
                items = reader.Select(r => r.FromDataReaderEstateOrgan()).ToList();
            }
            return items;
        }
        
        public static X509Certificate2 GetX509Certificate()
        {
            X509Certificate2 cer = new X509Certificate2();
            cer.Import("wwwbskapcom.pfx", "1221056@Am", X509KeyStorageFlags.PersistKeySet);
            return cer;
        }
    }
}
