
namespace Bska.Client.UI.ViewModels.AssetViewModel
{
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.UI.API;
    using Bska.Client.UI.CustomAttributes;
    using Bska.Client.UI.Helper;
    using Bska.Client.Common;
    using Bska.Client.UI.ViewModels.AccessoriesViewModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using System.Linq;
    public sealed class InCommodityViewModel : BaseDetailsViewModel<InCommidity>
    {
        #region ctor

        public InCommodityViewModel(InCommidity currentEntity)
            : base(currentEntity)
        {
            _seedDataApi = new SeedDataHelper();
            this._floors = new List<string>();
            this.initalizObj();
        }

        #endregion

        #region properties
        public List<string> Floors
        {
            get { return _floors; }
            set
            {
                _floors = value;
                OnPropertyChanged("Floors");
            }
        }
        public Int64 AssetId
        {
            get { return CurrentEntity.AssetId; }
        }

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

        [Required(ErrorMessage = "تعداد الزامی است")]
        [PositiveNumber(ErrorMessage = "تعداد باید بزرگتر از صفر باشد")]
        public Double Num
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
        
        public String Floor
        {
            get { return CurrentEntity.Floor; }
            set
            {
                CurrentEntity.Floor = value;
                OnPropertyChanged("Floor");
                this.initOldLabels();
            }
        }
        
        public int? OldLabel
        {
            get { return CurrentEntity.OldLabel; }
            set
            {
                CurrentEntity.OldLabel = value;
                OnPropertyChanged("OldLabel");
                this.checkOldLabels();
            }
        }

        public int? OrganLabel
        {
            get { return CurrentEntity.OrganLabel; }
            set
            {
                CurrentEntity.OrganLabel = value;
                OnPropertyChanged("OrganLabel");
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

        public String Characteristic
        {
            get { return CurrentEntity.Desc1; }
            set
            {
                CurrentEntity.Desc1 = value;
                OnPropertyChanged("Characteristic");
            }
        }

        public String Size
        {
            get { return CurrentEntity.Desc2; }
            set
            {
                CurrentEntity.Desc2 = value;
                OnPropertyChanged("Size");
            }
        }

        public String Country
        {
            get { return CurrentEntity.Desc3; }
            set
            {
                CurrentEntity.Desc3 = value;
                OnPropertyChanged("Country");
            }
        }

        public String Company
        {
            get { return CurrentEntity.Desc4; }
            set
            {
                CurrentEntity.Desc4 = value;
                OnPropertyChanged("Company");
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
            get { return _companies; }
            set
            {
                _companies = value;
                OnPropertyChanged("Company");
            }
        }

        public List<int> LastOldLabels
        {
            get { return GetValue(() => LastOldLabels); }
            set
            {
                SetValue(() => LastOldLabels, value);
            }
        }

        public Boolean HaveNewOldLabel
        {
            get { return GetValue(() => HaveNewOldLabel); }
            set
            {
                SetValue(() => HaveNewOldLabel, value);
                if (value)
                {
                    this.CurrentEntity.FloorType = 707;
                    this.initOldFloors(707);
                }
            }
        }

        public Boolean HaveAgoOldLabel
        {
            get { return GetValue(() => HaveAgoOldLabel); }
            set
            {
                SetValue(() => HaveAgoOldLabel, value);
                if (value)
                {
                    this.CurrentEntity.FloorType = 704;
                    this.initOldFloors(704);
                }
            }
        }

        public Boolean IsRbNewOldEnable
        {
            get { return GetValue(() => IsRbNewOldEnable); }
            set
            {
                SetValue(() => IsRbNewOldEnable, value);
            }
        }

        public Boolean IsRbAgoOldEnable
        {
            get { return GetValue(() => IsRbAgoOldEnable); }
            set
            {
                SetValue(() => IsRbAgoOldEnable, value);
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

        #endregion

        #region methods

        private async void initalizObj()
        {
            await Task.Run(() =>
            {
                CountryMakerList = _seedDataApi.GetCountry();
                if (!string.IsNullOrEmpty(this.CurrentEntity.Desc3)) GetCompanyByCountry(this.CurrentEntity.Desc3);
            });
            if (APPSettings.Default.OldSystemFloorType == 701)
            {
                IsRbNewOldEnable = true;
                IsRbAgoOldEnable = true;
            }
            else if (APPSettings.Default.OldSystemFloorType == 704)
            {
                IsRbAgoOldEnable = true;
            }
            else
            {
                IsRbNewOldEnable = true;
            }

            if (CurrentEntity.FloorType.HasValue)
            {
                int floorType = CurrentEntity.FloorType.Value;
                if (floorType == 704)
                {
                    this.HaveAgoOldLabel = true;
                }
                else
                {
                    this.HaveNewOldLabel = true;
                }
            }
            else
            {
                this.HaveNewOldLabel = true;
            }
            _currentKey = this.CurrentEntity.Floor + "*" + this.CurrentEntity.FloorType;
        }
        private void initializUnits()
        {
            if (Units.Count() > 0)
            {
                this.UnitId = Units.First().UnitId;
            }
        }
        private void initOldFloors(int type)
        {
            var items = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13" };
            if (APPSettings.Default.OldSystemFloorType == 701)
            {
                if (type == 704)
                {
                    items.Add("14");
                }
            }
            else if (APPSettings.Default.OldSystemFloorType == 704)
            {
                items.Add("14");
            }
            Floors = items;
            this.initOldLabels();
        }

        private void GetCompanyByCountry(string countryName)
        {
            if (CountryMakerList != null)
            {
                var country = CountryMakerList.FirstOrDefault(x => x.CountryName == countryName);
                if (country != null)
                {
                    Companies = _seedDataApi.GetCompany(country.CountryId);
                }
            }
        }
        internal void CreateListener(BaseDetailsViewModel<MovableAsset> ChildviewModel)
        {
            ChangeListener.Create(ChildviewModel).PropertyChanged += new PropertyChangedEventHandler(myScheduleView_PropertyChanged);
        }

        internal void myScheduleView_PropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Country":
                    if (!string.IsNullOrEmpty(this.CurrentEntity.Desc3))
                    {
                        GetCompanyByCountry(this.CurrentEntity.Desc3);
                    }
                    break;
            }
        }

        private void checkOldLabels()
        {
            if (OldLabel.HasValue)
            {
                if (string.IsNullOrEmpty(this.CurrentEntity.Floor))
                {
                    System.Windows.MessageBox.Show("طبقه انتخاب نشده است", "خطا", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    return;
                }

                string errorMessage = "";
                if (_oldLabels!=null)
                {
                    var lastOldLabels = _oldLabels[Floor + "*" + CurrentEntity.FloorType];
                    int fol = OldLabel.Value;
                    if (lastOldLabels.Contains(fol))
                    {
                        errorMessage += "طبقه " + Floor + " " + "شامل برچسب با شماره " + fol + " " + "مي باشد" + Environment.NewLine;
                    }
                    if (errorMessage.Length > 0)
                    {
                        System.Windows.MessageBox.Show(errorMessage, "خطا", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        this.OldLabel = null;
                    }
                }
            }
        }
        
        private void initOldLabels()
        {
            if (_oldLabels == null) return;
            if (!string.IsNullOrEmpty(Floor))
            {
                string key = Floor + "*" + CurrentEntity.FloorType;
                if (_oldLabels.ContainsKey(key))
                {
                    this.LastOldLabels = _oldLabels[key].ToList();
                    this.checkOldLabels();
                }
                else LastOldLabels = null;
            }
        }

        #endregion

        #region fields

        SeedDataHelper _seedDataApi;
        List<Country> _countryMaker;
        List<Company> _companies;
        internal int _checkErrorNum = 0;
        List<string> _floors;
        internal Dictionary<string, List<int>> _oldLabels;
        string _currentKey;

        #endregion
    }
}
