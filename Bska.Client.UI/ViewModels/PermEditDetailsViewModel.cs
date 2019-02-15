
namespace Bska.Client.UI.ViewModels
{
    using AccessoriesViewModels;
    using API;
    using Data.Service;
    using Domain.Entity;
    using Domain.Entity.AssetEntity;
    using Helper;
    using Microsoft.Practices.Unity;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    public sealed class PermEditDetailsViewModel : BaseViewModel
    {
        #region ctor

        public PermEditDetailsViewModel(IUnityContainer container,Int64 assetId,bool isEditable)
        {
            this._container = container;
            this._movableAssetService = _container.Resolve<IMovableAssetService>();
            this._stuffService = _container.Resolve<IStuffService>();
            this._dialogService = _container.Resolve<IDialogService>();
            seedDataApi = new SeedDataHelper();
            this._stuffHelper = new StuffHelper();
            this.IsEditable = isEditable;
            this.initializObj(assetId);
            this.initializCommands();
        }

        #endregion

        #region properties

        public Stuff CurrentStuff
        {
            get;
            private set;
        }

        public UnConsumption CurrentMovableAsset
        {
            get { return GetValue(() => CurrentMovableAsset); }
            set
            {
                SetValue(() => CurrentMovableAsset, value);
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
        
        public Int32 EditYear
        {
            get { return GetValue(() => EditYear); }
            set
            {
                SetValue(() => EditYear, value);
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

        public Boolean IsEditable
        {
            get { return GetValue(() => IsEditable); }
            set
            {
                SetValue(() => IsEditable, value);
            }
        }

        public string Uid1
        {
            get { return GetValue(() => Uid1); }
            set
            {
              SetValue(() => Uid1, value);
            }
        }

        public string Uid2
        {
            get { return GetValue(() => Uid2); }
            set
            {
                SetValue(() => Uid2, value);
            }
        }

        public string Uid3
        {
            get { return GetValue(() => Uid3); }
            set
            {
                SetValue(() => Uid3, value);
            }
        }

        public string Uid4
        {
            get { return GetValue(() => Uid4); }
            set
            {
                SetValue(() => Uid4, value);
            }
        }

        public string Desc1
        {
            get { return GetValue(() => Desc1); }
            set
            {
                SetValue(() => Desc1, value);
            }
        }

        public string Desc2
        {
            get { return GetValue(() => Desc2); }
            set
            {
                SetValue(() => Desc2, value);
            }
        }

        public string Desc3
        {
            get { return GetValue(() => Desc3); }
            set
            {
                SetValue(() => Desc3, value);
            }
        }

        public string Desc4
        {
            get { return GetValue(() => Desc4); }
            set
            {
                SetValue(() => Desc4, value);
            }
        }
        #endregion

        #region methods

        private void initializObj(Int64 assetId)
        {
            this.CurrentMovableAsset = _movableAssetService.Find(assetId) as UnConsumption;
            this.CurrentStuff = _stuffService.Query(s=>s.StuffId==CurrentMovableAsset.KalaUid).Include(s => s.Parent).Select().SingleOrDefault();
           string[] itemDescs= _stuffHelper.getreportDescByStuffId(CurrentStuff.Parent.StuffId).Split('|');
            this.Uid1 = itemDescs[4]+" "+CurrentMovableAsset.Uid1;
            this.Uid2 = itemDescs[5] + " " + CurrentMovableAsset.Uid1;
            this.Uid3 = itemDescs[6] + " " + CurrentMovableAsset.Uid1;
            this.Uid4 = itemDescs[7] + " " + CurrentMovableAsset.Uid1;

            this.Desc1 = itemDescs[0] + " " + CurrentMovableAsset.Desc1;
            this.Desc2 = itemDescs[1] + " " + CurrentMovableAsset.Desc2;
            this.Desc3 = itemDescs[2] + " " + CurrentMovableAsset.Desc3;
            this.Desc4 = itemDescs[3] + " " + CurrentMovableAsset.Desc4;
            CountryMakerList = seedDataApi.GetCountry();
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
                    if (!string.IsNullOrEmpty(this.CurrentMovableAsset.Desc3))
                    {
                        GetCompanyByCountry(this.CurrentMovableAsset.Desc3);
                    }
                    break;
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

        private void saveRelatedChanges(object parameter)
        {
            var window = parameter as Window;
            if (window != null)
            {
                bool confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
                if (confirm)
                {
                    window.DialogResult = true;
                }
            }
        }

        #endregion

        #region commands

        public ICommand SaveCommand { get; private set; }

        private void initializCommands()
        {
            SaveCommand = new MvvmCommand(
                (parameter) => { this.saveRelatedChanges(parameter); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IStuffService _stuffService;
        private readonly IDialogService _dialogService;
        List<Country> _countryMaker;
        List<Company> _company;
        SeedDataHelper seedDataApi;
        private StuffHelper _stuffHelper;

        #endregion
    }
}
