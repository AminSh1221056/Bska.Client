
namespace Bska.Client.UI.ViewModels.AssetViewModel
{
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.UI.CustomAttributes;
    using Bska.Client.UI.Helper;
    using Common;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    public sealed class CommodityViewModel : BaseDetailsViewModel<Commodity>
    {
        #region ctor

        public CommodityViewModel(Commodity currentEntity,List<Unit> units)
            : base(currentEntity)
        {
            _countryMaker = new List<Country>();
            _company = new List<Company>();
            _seedDataApi = new SeedDataHelper();
            this.Units = units;
            this.initalizObj();
        }

        #endregion

        #region properties

        public Int64 AssetId
        {
            get { return CurrentEntity.AssetId; }
        }

        [Required(ErrorMessage = "نعداد الزامی است")]
        [PositiveNumber(ErrorMessage = "تعداد الزامی است")]
        public double Num
        {
            get { return CurrentEntity.Num; }
            set
            {
                CurrentEntity.Num = value;
                ValidateProperty(value);
                OnPropertyChanged("Num");
            }
        }

        [Required(ErrorMessage = "واحد مال الزامی است")]
        [PositiveIntNumber(ErrorMessage = "واحد الزامی است")]
        public Int32 UnitId
        {
            get { return CurrentEntity.UnitId; }
            set
            {
                CurrentEntity.UnitId = value;
                ValidateProperty(value);
                OnPropertyChanged("UnitId");
            }
        }

        [Required(ErrorMessage = "قیمت مال الزامی است")]
        [PositiveNumber(ErrorMessage = "مقدار وارد شده صحیح نیست")]
        public Decimal Cost
        {
            get { return CurrentEntity.Cost; }
            set
            {
                CurrentEntity.Cost = value;
                ValidateProperty(value);
                OnPropertyChanged("Cost");
                this.initPrice();
            }
        }

        [Required(ErrorMessage = "قیمت واحد الزامی است")]
        [PositiveNumber(ErrorMessage = "مقدار وارد شده صحیح نیست")]
        public decimal UnitPrice
        {
            get { return _unitPrice; }
            set
            {
                _unitPrice = value;
                ValidateProperty(value);
                OnPropertyChanged("UnitPrice");
            }
        }

        public DateTime? DateOfBirth
        {
            get { return CurrentEntity.DateOfBirth; }
            set
            {
                CurrentEntity.DateOfBirth = value;
                OnPropertyChanged("DateOfBirth");
                this.initExpirationDate();
            }
        }

        public int ValidityDuration
        {
            get { return _numValidity; }
            set
            {
                _numValidity = value;
                OnPropertyChanged("ValidityDuration");
            }
        }

        public DateTime? ExpirationDate
        {
            get { return CurrentEntity.ExpirationDate; }
            set
            {
                CurrentEntity.ExpirationDate = value;
                OnPropertyChanged("ExpirationDate");
                this.initExpirationDate();
            }
        }

        public String Description
        {
            get { return CurrentEntity.Description; }
            set
            {
                CurrentEntity.Description = value;
                OnPropertyChanged("Description");
            }
        }

        public List<Country> CountryMakerList
        {
            get { return _countryMaker; }
            set
            {
                _countryMaker = value;
                OnPropertyChanged("CountryMakerList");
            }
        }

        public String CountryName
        {
            get { return CurrentEntity.Country; }
            set
            {
                CurrentEntity.Country = value;
                OnPropertyChanged("CountryName");
                GetCompanyByCountry(value);
            }
        }

        public string BatchNumber
        {
            get { return CurrentEntity.BatchNumber; }
            set
            {
                CurrentEntity.BatchNumber = value;
                OnPropertyChanged("BatchNumber");
            }
        }

        public List<Company> Company
        {
            get { return _company; }
            set
            {
                _company = value;
                OnPropertyChanged("Company");
            }
        }

        public String CompanyName
        {
            get { return CurrentEntity.Company; }
            set
            {
                CurrentEntity.Company = value;
                OnPropertyChanged("CompanyName");
            }
        }

        public List<Unit> Units
        {
            get { return GetValue(() => Units); }
            set
            {
                SetValue(() => Units, value);
                if (value != null)
                {
                    initializUnits();
                }
            }
        }

        public String UnitName
        {
            get { return GetValue(() => UnitName); }
            set
            {
                SetValue(() => UnitName, value);
            }
        }

        #endregion

        #region methods
        private void initalizObj()
        {
            CountryMakerList = _seedDataApi.GetCountry();
            if (!string.IsNullOrEmpty(this.CurrentEntity.Country)) GetCompanyByCountry(this.CurrentEntity.Country);
            this.initExpirationDate();
        }
        private void initializUnits()
        {
            if (Units.Count() > 0)
            {
                this.UnitId = Units.First().UnitId;
            }
        }
        private void GetCompanyByCountry(string countryName)
        {
            if (CountryMakerList != null)
            {
                var country = CountryMakerList.FirstOrDefault(x => x.CountryName == countryName);
                if (country != null)
                {
                    Company = _seedDataApi.GetCompany(country.CountryId);
                }
            }
        }

        private void initPrice()
        {
            if (Cost > 0)
            {
                decimal up = Convert.ToDecimal(Convert.ToDouble(Cost) / Num);
                if (UnitPrice !=up)
                {
                    UnitPrice =up;
                }
            }

            if (UnitPrice > 0)
            {
                decimal cs= Convert.ToDecimal(Convert.ToDouble(UnitPrice) * Num);
                if (Cost !=cs)
                {
                    Cost = cs;
                }
            }
        }

        private void initExpirationDate()
        {
            if (ExpirationDate.HasValue)
            {
                if (DateOfBirth.HasValue)
                {
                    if (ExpirationDate.Value < DateOfBirth.Value)
                    {
                        System.Windows.MessageBox.Show("تاریخ انقضا نامعتبر می باشد", "خطا",
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        return;
                    }
                    ValidityDuration = ExpirationDate.Value.Month - DateOfBirth.Value.Month;
                }
            }
        }

        #endregion

        #region commands
        #endregion

        #region fields

        SeedDataHelper _seedDataApi;
        List<Country> _countryMaker;
        List<Company> _company;
        int _numValidity;
        decimal _unitPrice;
        #endregion
    }
}
