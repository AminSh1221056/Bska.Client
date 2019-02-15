
namespace Bska.Client.UI.ViewModels.AssetViewModel
{
    using Bska.Client.Domain.Entity.AssetEntity;
    using CustomAttributes;
    using Domain.Entity;
    using Helper;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    public sealed class InstallableViewModel : BaseDetailsViewModel<Installable>
    {
        #region ctor

        public InstallableViewModel(Installable currentEntity)
            : base(currentEntity)
        {
            seedDataApi = new SeedDataHelper();
            this.initializObj();
        }

        #endregion

        #region properties

        [Required(ErrorMessage = "نام مال الزامی است")]
        public String Name
        {
            get { return CurrentEntity.Name; }
            set
            {
                CurrentEntity.Name = value;
                ValidateProperty(value);
                OnPropertyChanged("Name");
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

        public String Description
        {
            get { return CurrentEntity.Description; }
            set
            {
                CurrentEntity.Description = value;
                OnPropertyChanged("Description");
            }
        }

        [Required(ErrorMessage = "کیفیت الزامی است")]
        public String Quality
        {
            get { return CurrentEntity.Quality; }
            set
            {
                CurrentEntity.Quality = value;
                ValidateProperty(value);
                OnPropertyChanged("Quality");
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

        public List<Company> Companies
        {
            get { return _company; }
            set
            {
                _company = value;
                OnPropertyChanged("Companies");
            }
        }

        public String Country
        {
            get { return CurrentEntity.Desc3; }
            set
            {
                CurrentEntity.Desc3 = value;
                OnPropertyChanged("Country");
                if (!string.IsNullOrWhiteSpace(value))
                {
                    this.GetCompanyByCountry(value);
                }
            }
        }

        public String Company
        {
            get { return CurrentEntity.Desc2; }
            set
            {
                CurrentEntity.Desc2 = value;
                OnPropertyChanged("Company");
            }
        }

        public String Capacity
        {
            get { return CurrentEntity.Desc1; }
            set
            {
                CurrentEntity.Desc1 = value;
                OnPropertyChanged("Capacity");
            }
        }

        public String Size
        {
            get { return CurrentEntity.Desc4; }
            set
            {
                CurrentEntity.Desc4 = value;
                OnPropertyChanged("Size");
            }
        }

        public String Model
        {
            get { return CurrentEntity.Desc5; }
            set
            {
                CurrentEntity.Desc5 = value;
                OnPropertyChanged("Model");
            }
        }
        public List<Unit> Units
        {
            get { return GetValue(() => Units); }
            set
            {
                SetValue(() => Units, value);
                initializUnits();
            }
        }

        [Required(ErrorMessage = "برچسب الزامی است")]
        [PositiveNumber(ErrorMessage = "مقدار وارد شده صحیح نیست")]
        public int? Label
        {
            get { return CurrentEntity.Label; }
            set
            {
                CurrentEntity.Label = value;
                ValidateProperty(value);
                OnPropertyChanged("Label");
            }
        }

        public List<int> Labels
        {
            get { return GetValue(() => Labels); }
            set
            {
                SetValue(() => Labels, value);
            }
        }

        #endregion

        #region methods

        private void initializObj()
        {
            CountryMakerList = seedDataApi.GetCountry();
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
                    Companies = seedDataApi.GetCompany(country.CountryId);
                }
            }
        }

        #endregion

        #region commands
        #endregion

        #region fields

        List<Country> _countryMaker;
        List<Company> _company;
        SeedDataHelper seedDataApi;

        #endregion
    }
}
