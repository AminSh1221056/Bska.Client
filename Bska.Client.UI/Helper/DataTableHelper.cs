using System;
using System.Data;

namespace Bska.Client.UI.Helper
{
    public class DataTableHelper
    {
        public DataTable getStuffTable()
        {
            DataTable dt = new DataTable("StuffType");
            dt.Columns.Add("KalaUID", typeof(int));
            dt.Columns.Add("KalaNo", typeof(string));
            dt.Columns.Add("GS1", typeof(string));
            dt.Columns.Add("KalaNme", typeof(string));
            dt.Columns.Add("IsStuff", typeof(Boolean));
            dt.Columns.Add("Typ", typeof(int));
            dt.Columns.Add("FloorOld", typeof(int));
            dt.Columns.Add("FloorNew", typeof(int));
            dt.Columns.Add("parentId", typeof(int));
            return dt;
        }

        public DataTable getUnitTable()
        {
            DataTable dt = new DataTable("UnitType");
            dt.Columns.Add("UnitId", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("StuffId", typeof(int));
            dt.Columns.Add("MathType", typeof(int));
            dt.Columns.Add("MathNum", typeof(int));
            dt.Columns.Add("ParentId", typeof(int));
            return dt;
        }

        public DataTable getAccountCodingTable()
        {
            DataTable dt = new DataTable("@AccountCodingType");
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("AccountCode", typeof(string));
            dt.Columns.Add("EmployeeId", typeof(int));
            dt.Columns.Add("ParentId", typeof(int));
            return dt;
        }

        public DataTable getDepricationTable()
        {
            DataTable dt = new DataTable("DepricationType");
            dt.Columns.Add("StuffId", typeof(int));
            dt.Columns.Add("DepricationNum", typeof(int));
            dt.Columns.Add("Type", typeof(int));
            dt.Columns.Add("FactorLowest", typeof(int));
            return dt;
        }

        public DataTable getCountryTable()
        {
            DataTable dt = new DataTable("CountryType");
            dt.Columns.Add("CountryId", typeof(int));
            dt.Columns.Add("CountryName", typeof(string));
            dt.Columns.Add("CarCorporationId", typeof(int));
            return dt;
        }

        public DataTable getCompanyTable()
        {
            DataTable dt = new DataTable("CompanyType");

            dt.Columns.Add("CompanyId", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("CountryId", typeof(int));
            dt.Columns.Add("IsCarCompany", typeof(bool));
            return dt;
        }

        public DataTable getInsurancecompanyTable()
        {
            DataTable dt = new DataTable("InsuranceCompanyType");
            dt.Columns.Add("InsuranceId", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            return dt;
        }

        public DataTable getCarDetailsTable()
        {
            DataTable dt = new DataTable("CarDetailsType");
            dt.Columns.Add("CarDetailsId", typeof(int));
            dt.Columns.Add("CarType", typeof(int));
            dt.Columns.Add("SystemType", typeof(string));
            dt.Columns.Add("Tipe", typeof(string));
            dt.Columns.Add("Model", typeof(string));
            dt.Columns.Add("CompanyId", typeof(int));
            return dt;
        }

        public DataTable getOrganizationModelTable()
        {
            DataTable dt = new DataTable("OrganizationModelType");
            dt.Columns.Add("EmployeeId", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("BudgetNo", typeof(int));
            return dt;
        }

        public DataTable getProvinceTable()
        {
            DataTable dt = new DataTable("ProvinceType");
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            return dt;
        }

        public DataTable getEstateOrganTable()
        {
            DataTable dt = new DataTable("EstateOrganType");
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("ProvinceId", typeof(string));
            return dt;
        }
    }
}
