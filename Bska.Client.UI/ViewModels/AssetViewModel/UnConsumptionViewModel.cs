
namespace Bska.Client.UI.ViewModels.AssetViewModel
{
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.UI.API;
    using Bska.Client.UI.CustomAttributes;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.ViewModels.AccessoriesViewModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    public sealed class UnConsumptionViewModel : BaseDetailsViewModel<UnConsumption>
    {
        #region ctor

        public UnConsumptionViewModel(UnConsumption currentEntity) :
            base(currentEntity)
        {
            seedDataApi = new SeedDataHelper();
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

        public AutomativeSportsViewModel AutomativeSportsModel
        {
            get;
            set;
        }

        public AutomotiveViewModel AutomotiveModel
        {
            get;
            set;
        }

        public CameraViewModel CameraModel
        {
            get;
            set;
        }

        public CDViewModel CDModel
        {
            get;
            set;
        }

        public ElectricViewModel ElectricModel
        {
            get;
            set;
        }

        public ComputerViewModel ComputerModel
        {
            get;
            set;
        }

        public HandmadeCarpetViewModel HandmadeCarpetModel
        {
            get;
            set;
        }

        public NonElectricViewModel NonElectricModel
        {
            get;
            set;
        }

        public PrintedBooksViewModel PrintedBooksModel
        {
            get;
            set;
        }

        public SportViewModel SportModel
        {
            get;
            set;
        }

        public ToolViewModel ToolModel
        {
            get;
            set;
        }

        public VideoAudioViewModel VideoAudioModel
        {
            get;
            set;
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

        public int? FloorType
        {
            get { return CurrentEntity.FloorType; }
            set
            {
                CurrentEntity.FloorType = value;
                OnPropertyChanged("FloorType");
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

        public int? Label
        {
            get { return CurrentEntity.Label; }
        }

        public List<int> Labels
        {
            get { return GetValue(() => Labels); }
            set
            {
                SetValue(() => Labels, value);
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

        public List<Unit> Units
        {
            get { return GetValue(() => Units); }
            set
            {
                SetValue(() => Units, value);
                this.initializUnits();
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

        public List<Company> Company
        {
            get { return _company; }
            set
            {
                _company = value;
                OnPropertyChanged("Company");
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

        public Boolean OldLabelEditable
        {
            get { return GetValue(() => OldLabelEditable); }
            set
            {
                SetValue(() => OldLabelEditable, value);
            }
        }

        #endregion

        #region methods
        private void initalizObj()
        {
            CountryMakerList = seedDataApi.GetCountry();
            if (!string.IsNullOrEmpty(this.CurrentEntity.Desc3))
                GetCompanyByCountry(this.CurrentEntity.Desc3);
           

            if (CurrentEntity.FloorType.HasValue)
            {
                int floorType = CurrentEntity.FloorType.Value;
                if (floorType == 704)
                {
                    this.HaveAgoOldLabel = true;
                    IsRbAgoOldEnable = true;
                }
                else
                {
                    IsRbNewOldEnable = true;
                    this.HaveNewOldLabel = true;
                }
            }
            else
            {
                if (APPSettings.Default.OldSystemFloorType == 701)
                {
                    IsRbNewOldEnable = true;
                    IsRbAgoOldEnable = true;
                    this.HaveNewOldLabel = true;
                }
                else if (APPSettings.Default.OldSystemFloorType == 704)
                {
                    IsRbAgoOldEnable = true;
                    this.HaveAgoOldLabel = true;
                }
                else
                {
                    IsRbNewOldEnable = true;
                    this.HaveNewOldLabel = true;
                }
            }

            _currentKey = this.CurrentEntity.Floor + "*" + this.CurrentEntity.FloorType;
        }

        private void initOldFloors(int type)
        {
            var items = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13" };
            if (APPSettings.Default.OldSystemFloorType == 701)
            {
                if (type==704)
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

        private void initializUnits()
        {
            if (Units.Count() > 0)
            {
                this.UnitId = Units.First().UnitId;
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

                if (_oldLabels != null)
                {
                    string errorMessage = "";
                    if (Labels.Count() > 0)
                    {
                        if (_oldLabels.ContainsKey(Floor+"*"+CurrentEntity.FloorType))
                        {
                            var lastOldLabels = _oldLabels[Floor + "*" + CurrentEntity.FloorType];
                            int fol = OldLabel.Value;
                            foreach (int l in Labels)
                            {
                                if (lastOldLabels.Contains(fol))
                                {
                                    errorMessage += "طبقه " + Floor + " " + "شامل برچسب با شماره " + fol + " " + "مي باشد" + Environment.NewLine;
                                    break;
                                }
                                fol++;
                            }
                        }
                    }
                    else
                    {
                        errorMessage += "تعداد وارد نشده است" + Environment.NewLine;
                    }

                    if (errorMessage.Length > 0)
                    {
                        System.Windows.MessageBox.Show(errorMessage,"خطا",System.Windows.MessageBoxButton.OK,System.Windows.MessageBoxImage.Error);
                        this.OldLabel =null;
                    }
                }
            }
        }

        private void GetCompanyByCountry(string countryName)
        {
            if (CountryMakerList != null)
            {
                var country = CountryMakerList.FirstOrDefault(x => x.CountryName == countryName);
                if (country != null)
                {
                    Company = seedDataApi.GetCompany(country.CountryId);
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

        public Boolean CheckErrors()
        {
            if (this.HasErrors) return false;
            else
            {
                Boolean _hasErrors = false;
                switch (_checkErrorNum)
                {
                    case 1:
                        _hasErrors = this.ElectricModel.HasErrors;
                        break;
                    case 2:
                        _hasErrors = this.PrintedBooksModel.HasErrors;
                        break;
                    case 3:
                        _hasErrors = this.AutomotiveModel.HasErrors;
                        break;
                    case 4:
                        _hasErrors = this.VideoAudioModel.HasErrors;
                        break;
                    case 5:
                        _hasErrors = this.ComputerModel.HasErrors;
                        break;
                    case 6:
                        _hasErrors = this.HandmadeCarpetModel.HasErrors;
                        break;
                    case 7:
                        _hasErrors = this.CDModel.HasErrors;
                        break;
                    case 8:
                        _hasErrors = this.SportModel.HasErrors;
                        break;
                    case 9:
                        _hasErrors = this.AutomativeSportsModel.HasErrors;
                        break;
                    case 10:
                        _hasErrors = this.CameraModel.HasErrors;
                        break;
                    case 11:
                        _hasErrors = this.ToolModel.HasErrors;
                        break;
                    case 12:
                        _hasErrors = this.NonElectricModel.HasErrors;
                        break;
                    default:
                        _hasErrors = true;
                        break;
                }
                return !_hasErrors;
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

        List<Country> _countryMaker;
        List<Company> _company;
        internal int _checkErrorNum = 0;
        SeedDataHelper seedDataApi;
        internal Dictionary<string,List<int>> _oldLabels;
        List<string> _floors;
        string _currentKey;

        #endregion
    }
}
