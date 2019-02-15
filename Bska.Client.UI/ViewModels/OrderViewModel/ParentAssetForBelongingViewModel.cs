
namespace Bska.Client.UI.ViewModels.OrderViewModel
{
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Bska.Client.UI.Helper;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Data.Entity;
    using System.Collections.ObjectModel;
    using Bska.Client.Common;
    public sealed class ParentAssetForBelongingViewModel : BaseViewModel
    {
        #region ctor

        public ParentAssetForBelongingViewModel(IUnityContainer container,PersonModel currentPerson,Store currentStore,List<UnConsumption> availabelAssets,bool isForTrust=false)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>();
            this._mAssets = new ObservableCollection<UnConsumption>();
            this.CurrentPerson = currentPerson;
            this.CurrentStore = currentStore;
            this.initializObj(availabelAssets,isForTrust);
            this.initializCommands();
        }

        #endregion

        #region properties

        public PersonModel CurrentPerson
        {
            get { return GetValue(() => CurrentPerson); }
            set
            {
                SetValue(() => CurrentPerson, value);
            }
        }

        public Store CurrentStore
        {
            get { return GetValue(() => CurrentStore); }
            set
            {
                SetValue(() => CurrentStore, value);
            }
        }

        public ObservableCollection<UnConsumption> MAssets
        {
            get { return _mAssets; }
        }

        public UnConsumption Selected
        {
            get { return GetValue(() => Selected); }
            set
            {
                SetValue(() => Selected, value);
            }
        }

        public String Desc1
        {
            get { return GetValue(() => Desc1); }
            set
            {
                SetValue(() => Desc1, value);
            }
        }

        public String Desc2
        {
            get { return GetValue(() => Desc2); }
            set
            {
                SetValue(() => Desc2, value);
            }
        }

        #endregion

        #region methods

        private void initializObj(List<UnConsumption> availableAssets,bool isForTrust)
        {
            _mAssets.Clear();
            if (!isForTrust)
            {
                if (CurrentPerson != null)
                {
                    _movableAssetService.Queryable().SelectMany(x => x.Locations).Where(l => l.Status == LocationStatus.Active
                        && l.PersonId == CurrentPerson.NationalId).Select(l => l.MovableAsset).Include(ma => ma.Locations).OfType<UnConsumption>().ForEach(ma =>
                        {
                            _mAssets.Add(ma);
                        });

                    Desc1 = string.Format("نام پرسنل : {0}", CurrentPerson.FullName);
                    Desc2 = string.Format("کد ملی : {0}", CurrentPerson.NationalId);
                }
                else if (CurrentStore != null)
                {
                    _movableAssetService.Queryable().SelectMany(x => x.Locations).Where(l => l.Status == LocationStatus.StoreActive
                         && l.StoreId == CurrentStore.StoreId).Select(l => l.MovableAsset).Include(ma => ma.Locations).OfType<UnConsumption>().ForEach(ma =>
                         {
                             _mAssets.Add(ma);
                         });

                    Desc1 = string.Format("نام انبار : {0}", CurrentStore.Name);
                    Desc2 = string.Format("نوع انبار : {0}", CurrentStore.StoreType.GetDescription());
                }
            }

            if (availableAssets != null)
            {
                availableAssets.ForEach(ma =>
                {
                     _mAssets.Add(ma);
                });
            }
        }

        private void selectMAsset(IList<object> parameter)
        {
            var window=parameter[1] as Window;
            if (window==null) return;
            var mAsset = parameter[0] as UnConsumption;
            if (mAsset == null) return;
            this.Selected = mAsset;
            window.DialogResult = true;
        }

        #endregion

        #region commands

        public ICommand SelectCommand { get; private set; }

        private void initializCommands()
        {
            SelectCommand = new MvvmCommand(
                (parameter) => { this.selectMAsset(parameter as IList<object>); },
                (parameter) => { return true; }
                );
        }
        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly ObservableCollection<UnConsumption> _mAssets;

        #endregion
    }
}
