
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;
    using System.Linq;
    using Bska.Client.Common;

    public sealed class MovableAssetSplitViewModel : BaseViewModel
    {
        #region ctor

        public MovableAssetSplitViewModel(IUnityContainer container)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>();
            this._employeeSrvice = _container.Resolve<IEmployeeService>();
            this._personService = _container.Resolve<IPersonService>();
            this._storeService = _container.Resolve<IStoreService>();
            this._navigationService = _container.Resolve<INavigationService>();
        }

        #endregion

        #region properties

        public MovableAsset CurrentMovableAsset
        {
            get { return GetValue(() => CurrentMovableAsset); }
            set
            {
                SetValue(() => CurrentMovableAsset, value);
                if (value != null)
                {
                    this.initalizObj();
                    this.initalizCommand();
                }
            }
        }
        public bool BuildingEnabled
        {
            get { return GetValue(() => BuildingEnabled); }
            set
            {
                SetValue(() => BuildingEnabled, value);
            }
        }

        public bool StoreEnabled
        {
            get { return GetValue(() => StoreEnabled); }
            set
            {
                SetValue(() => StoreEnabled, value);
            }
        }

        public bool OtherEnabled
        {
            get { return GetValue(() => OtherEnabled); }
            set
            {
                SetValue(() => OtherEnabled, value);
            }
        }

        public List<Location> Locations
        {
            get { return GetValue(() => Locations); }
            set
            {
                SetValue(() => Locations, value);
            }
        }

        public Location SelectedItem
        {
            get { return GetValue(() => SelectedItem); }
            set
            {
                SetValue(() => SelectedItem, value);
            }
        }
        public string LocationName
        {
            get { return GetValue(() => LocationName); }
            set
            {
                SetValue(() => LocationName, value);
            }
        }

        public String BuildingName
        {
            get { return GetValue(() => BuildingName); }
            set
            {
                SetValue(() => BuildingName, value);
            }
        }

        public string OrganizationName
        {
            get { return GetValue(() => OrganizationName); }
            set
            {
                SetValue(() => OrganizationName, value);
            }
        }

        public string StrategyName
        {
            get { return GetValue(() => StrategyName); }
            set
            {
                SetValue(() => StrategyName, value);
            }
        }

        public String StoreAddressName
        {
            get { return GetValue(() => StoreAddressName); }
            set
            {
                SetValue(() => StoreAddressName, value);
            }
        }

        public string PersonName
        {
            get { return GetValue(() => PersonName); }
            set
            {
                SetValue(() => PersonName, value);
            }
        }

        public string StoreName
        {
            get { return GetValue(() => StoreName); }
            set
            {
                SetValue(() => StoreName, value);
            }
        }

        #endregion

        #region methods

        private void initalizObj()
        {
            this.StoreEnabled = true;
            this.BuildingEnabled = true;
            this.OtherEnabled = true;

            if (CurrentMovableAsset != null)
                Locations = CurrentMovableAsset.Locations.ToList();
        }

        private void GetLocationDetails(object location)
        {
            var currentLocation = location as Location;
            if (currentLocation == null)
            {
                _dialogService.ShowInfo("نوجه", "ورودی نامعتبر می باشد");
                return;
            }

            Mouse.SetCursor(Cursors.Wait);
            SelectedItem = currentLocation;
            if (currentLocation.Status == LocationStatus.StoreActive || currentLocation.Status == LocationStatus.StoreDeActive || 
                currentLocation.Status==LocationStatus.Retiring || currentLocation.Status==LocationStatus.RetiringDeActive)
            {
                LocationName = "انبار";
                StoreEnabled = false;
                BuildingEnabled = true;
                this.OtherEnabled = true;
                var store = _storeService.Find(currentLocation.StoreId);
                if (store != null)
                {
                    StoreName = store.Name;
                    var storeDesign = _storeService.GetParentNode(store.StoreId)
                        .Where(x => x.StoreDesignId == currentLocation.StoreAddressId).SingleOrDefault();
                    if (storeDesign != null)
                    {
                        StoreAddressName = GetStoreHirecharyNode(storeDesign);
                    }
                }
            }
            else if (currentLocation.Status == LocationStatus.Active || currentLocation.Status == LocationStatus.MovedRequest || currentLocation.Status==LocationStatus.DeActive)
            {
                var person = _personService.Queryable().SingleOrDefault(p=>p.NationalId==currentLocation.PersonId);
                LocationName = "خارج از انبار";
                BuildingEnabled = false;
                StoreEnabled = true;
                this.OtherEnabled = true;

                BuildingName = "555";
                var organiz =_employeeSrvice.GetParentNode(1).Where(x => x.BuidldingDesignId
                        == currentLocation.OrganizId).SingleOrDefault();
                if (organiz != null)
                {
                    OrganizationName = GetHirecharyNode(organiz);
                }

                var strategy = _employeeSrvice.GetParentNode(2).Where(x => x.BuidldingDesignId
                        == currentLocation.StrategyId).SingleOrDefault();
                if (strategy != null)
                {
                    StrategyName = GetHirecharyNode(strategy);
                }

                if (person != null)
                {
                    PersonName = string.Format("{0} {1}", person.FirstName, person.LastName);
                }
            }
            else
            {
                LocationName = "عملیات اداری اموال";
                StoreEnabled = true;
                BuildingEnabled = true;
                this.OtherEnabled = false;
            }

            Mouse.SetCursor(Cursors.Arrow);
        }

        private String GetHirecharyNode(EmployeeDesign item)
        {
            String _nodeName = "";

            if (item.ParentNode != null)
            {
                _nodeName += this.GetHirecharyNode(item.ParentNode);
                _nodeName += "***";
            }

            _nodeName += item.Name;

            return _nodeName;
        }

        private String GetStoreHirecharyNode(StoreDesign item)
        {
            String _nodeName = "";

            if (item.ParentNode != null)
            {
                _nodeName += this.GetStoreHirecharyNode(item.ParentNode);
                _nodeName += "***";
            }

            _nodeName += item.Name;

            return _nodeName;
        }

        private void reports()
        {
            Mouse.SetCursor(Cursors.Wait);

            var viewModel = new ReportViewModel();
            viewModel.mAssetItemsReports(CurrentMovableAsset.Name,CurrentMovableAsset.GetType().Name,CurrentMovableAsset.AssetId,CurrentMovableAsset.Label,1);
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region commands

        public ICommand DetailsCommand { get; private set; }
        public ICommand ReportCommand { get; private set; }
        private void initalizCommand()
        {
            DetailsCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.GetLocationDetails(parameter);
                },
                (parameter) =>
                {
                    return true;
                }
                );

            ReportCommand = new MvvmCommand(
                (parameter) => { this.reports(); },
                (parameter) =>
                {
                    return true;
                }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IStoreService _storeService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IPersonService _personService;
        private readonly IEmployeeService _employeeSrvice;

        #endregion
    }
}
