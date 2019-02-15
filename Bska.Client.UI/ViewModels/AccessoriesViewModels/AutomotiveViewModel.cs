
namespace Bska.Client.UI.ViewModels.AccessoriesViewModels
{
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.UI.Helper;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    public sealed class AutomotiveViewModel : BaseDetailsViewModel<MovableAsset>
    {
        #region ctor

        public AutomotiveViewModel(MovableAsset currentEntity)
            : base(currentEntity)
        {
            seedDataApi = new SeedDataHelper();
            this.initializObj();
        }

        #endregion

        #region properties

        [Required(ErrorMessage = "شماره موتور الزامی است")]
        public String MotorNo
        {
            get { return CurrentEntity.Uid1; }
            set
            {
                CurrentEntity.Uid1 = value;
                ValidateProperty(value);
                OnPropertyChanged("MotorNo");
            }
        }

        [Required(ErrorMessage = "شماره شاسی الزامی است")]
        public String ChassisNo
        {
            get { return CurrentEntity.Uid2; }
            set
            {
                CurrentEntity.Uid2 = value;
                ValidateProperty(value);
                OnPropertyChanged("ChassisNo");
            }
        }

        public String Capacity
        {
            get { return CurrentEntity.Uid3; }
            set
            {
                CurrentEntity.Uid3 = value;
                OnPropertyChanged("Capacity");
            }
        }

        [Required(ErrorMessage = "شماره کمیسون ماده 2 الزامی است")]
        public String CommissionNo
        {
            get { return CurrentEntity.Uid4; }
            set
            {
                CurrentEntity.Uid4 = value;
                ValidateProperty(value);
                OnPropertyChanged("CommissionNo");
            }
        }

        public String Color
        {
            get { return CurrentEntity.Desc1; }
            set
            {
                CurrentEntity.Desc1 = value;
                OnPropertyChanged("Color");
            }
        }

        public String Fuel
        {
            get { return CurrentEntity.Desc2; }
            set
            {
                CurrentEntity.Desc2 = value;
                OnPropertyChanged("Fuel");
            }
        }

        [Required(ErrorMessage = "پلاک الزامی است")]
        public String Plaque
        {
            get { return CurrentEntity.Desc3; }
            set
            {
                CurrentEntity.Desc3 = value;
                ValidateProperty(value);
                OnPropertyChanged("Plaque");
            }
        }

        public List<CarDetails> CarDetailsList
        {
            get { return GetValue(() => CarDetailsList); }
            set
            {
                SetValue(() => CarDetailsList, value);
            }
        }

        [Required(ErrorMessage = "مشخصات اصلی الزامی است")]
        public CarDetails CarDetails
        {
            get { return _carDetails; }
            set
            {
                _carDetails = value;
                ValidateProperty(value);
                OnPropertyChanged("CarDetails");
                this.intiCarDetailsItems();
            }
        }
        
        public List<Country> Countries
        {
            get { return GetValue(() => Countries); }
            set
            {
                SetValue(() => Countries, value);
            }
        }

        [Required(ErrorMessage = "کشور سازنده الزامی است")]
        public Country CountryItem
        {
            get { return _country; }
            set
            {
                _country = value;
                ValidateProperty(value);
                OnPropertyChanged("CountryItem");
                GetCarCompanies();
            }
        }

        public List<Company> Companies
        {
            get { return GetValue(() => Companies); }
            set
            {
                SetValue(() => Companies, value);
            }
        }

        [Required(ErrorMessage = "کارخانه سازنده الزامی است")]
        public Company SelectedCompany
        {
            get { return _company; }
            set
            {
                _company = value;
                ValidateProperty(value);
                OnPropertyChanged("SelectedCompany");
                CompanySelecting();
            }
        }

        public CarType CarTypeItem
        {
            get { return GetValue(() => CarTypeItem); }
            set
            {
                SetValue(() => CarTypeItem, value);
                GetCarDetails();
            }
        }

        [Required(ErrorMessage = "VIN الزامی می باشد")]
        public String VIN
        {
            get { return CurrentEntity.Desc11; }
            set
            {
                CurrentEntity.Desc11 = value;
                ValidateProperty(value);
                OnPropertyChanged("VIN");
            }
        }

        public string BuildYear
        {
            get { return GetValue(() => BuildYear); }
            set
            {
                SetValue(() => BuildYear, value);
            }
        }

        public String Tip
        {
            get { return GetValue(() => Tip); }
            set
            {
                SetValue(() => Tip, value);
            }
        }

        public Boolean VINEnabled
        {
            get { return GetValue(() => VINEnabled); }
            set
            {
                SetValue(() => VINEnabled, value);
            }
        }

        #endregion

        #region methods

        private void initializObj()
        {
            Companies = seedDataApi.GetCarCompany(-1, false);
            var coutryIds = Companies.Select(x => x.CountryId.HasValue ? x.CountryId.Value : 0).ToList();
            Countries = seedDataApi.GetCarCountry(coutryIds);
            this.CountryItem = Countries.FirstOrDefault(x => x.CountryName == CurrentEntity.Desc3);
            this.SelectedCompany = Companies.FirstOrDefault(x => x.Name == CurrentEntity.Desc2);
            this.CarDetailsList = this.getRelatedCarDetails(-1, null, true);
            this.CarDetails = CarDetailsList.FirstOrDefault(x => x.SystemType == CurrentEntity.Desc8 && x.Tipe == CurrentEntity.Desc9 && x.Model == CurrentEntity.Desc10 && x.CarType== (CarType)Enum.Parse(typeof(CarType),CurrentEntity.Desc7));
            VINEnabled = true;
        }

        private List<CarDetails> getRelatedCarDetails(int companyId, string carType, bool allCars, List<int> comapnyId = null, bool companyRealted = false)
        {
            if (companyRealted)
            {
                if (CurrentEntity != null)
                    return seedDataApi.GetRelatedCarDetails(comapnyId)
                        .Where(x => CurrentEntity.Name.StartsWith(x.SystemType)).ToList();
                else
                    return seedDataApi.GetRelatedCarDetails(comapnyId);
            }

            if (CurrentEntity != null)
            {
               return seedDataApi.GetCarDetails(companyId, carType, allCars)
                    .Where(x => CurrentEntity.Name.StartsWith(x.SystemType)).ToList();
            }
             return seedDataApi.GetCarDetails(companyId, carType, allCars);
        }

        private void CompanySelecting()
        {
            if (SelectedCompany == null) return;
            if (CountryItem == null)
            {
                CountryItem = Countries.FirstOrDefault(x => x.CountryId == SelectedCompany.CountryId);
            }
        }

        private void GetCarCountry()
        {
            var countryIds = Countries.Select(x => x.CountryId).ToList();
            Companies = seedDataApi.GetRelatedCompanies(countryIds);
            var companyIds = Companies.Select(x => x.CompanyId).ToList();
            CarDetailsList = this.getRelatedCarDetails(-1, null, false, companyIds, true);
            this.CarDetails = null;
            this.CountryItem = null;
            this.SelectedCompany = null;
            if (!string.IsNullOrEmpty(CurrentEntity.Desc5))
            {
                CountryItem = Countries.FirstOrDefault(x => x.CountryName == CurrentEntity.Desc5);
            }
        }

        private void GetCarCompanies()
        {
            if (CountryItem == null) return;
            if (SelectedCompany != null)
            {
                if (SelectedCompany.CountryId == CountryItem.CountryId) return;
            }
            CurrentEntity.Desc3 = CountryItem.CountryName;
            Companies = seedDataApi.GetCarCompany(CountryItem.CountryId);
            this.CurrentEntity.Desc5 = CountryItem.CountryName;
            var companyIds = Companies.Select(x => x.CompanyId).ToList();
            CarDetailsList = this.getRelatedCarDetails(-1, null, false, companyIds, true);
            this.CarDetails = null;
            this.SelectedCompany = null;
            if (!string.IsNullOrEmpty(CurrentEntity.Desc6))
            {
                SelectedCompany = Companies.FirstOrDefault(x => x.Name == CurrentEntity.Desc6);
            }

            if (!string.IsNullOrEmpty(CurrentEntity.Desc7))
            {
                CarTypeItem = (CarType)Enum.Parse(typeof(CarType), CurrentEntity.Desc7);
            }
        }

        private void GetCarDetails()
        {
            if (SelectedCompany == null) return;
            if (CarDetails != null) return;
            CarDetailsList = this.getRelatedCarDetails(SelectedCompany.CompanyId, CarTypeItem.ToString(), false);
            this.CurrentEntity.Desc6 = SelectedCompany.Name;
            this.CarDetails = null;
            if (!string.IsNullOrEmpty(CurrentEntity.Desc8))
            {
                CarDetails = CarDetailsList.FirstOrDefault(x => x.CarType == CarTypeItem
                    && x.Tipe == CurrentEntity.Desc9 && x.SystemType == CurrentEntity.Desc8);
            }
        }

        private void intiCarDetailsItems()
        {
            if (CarDetails == null) return;
            this.CurrentEntity.Desc7 = CarDetails.CarType.ToString();
            this.CurrentEntity.Desc8 = CarDetails.SystemType;
            this.CurrentEntity.Desc9 = CarDetails.Tipe;
            this.CurrentEntity.Desc10 = CarDetails.Model;
            this.BuildYear = CarDetails.Model;
            int temp = -1;
            if(int.TryParse(CarDetails.Model,out temp))
            {
                if (temp < 1383)
                {
                    this.VINEnabled = false;
                    this.CommissionNo = "1383";
                }
                else
                {
                    this.VINEnabled = true;
                }
            }
            else
            {
                VINEnabled = true;
            }

            this.Tip = CarDetails.Tipe.ToString();
            if (CarTypeItem != CarDetails.CarType)
            {
                CarTypeItem = CarDetails.CarType;
            }
            SelectedCompany = Companies.FirstOrDefault(x => x.CompanyId == CarDetails.CompanyId);
            CountryItem = Countries.FirstOrDefault(x => x.CountryId == SelectedCompany.CountryId);
        }

        #endregion

        #region fields

        SeedDataHelper seedDataApi;
        Country _country;
        Company _company;
        CarDetails _carDetails;

        #endregion
    }
}
