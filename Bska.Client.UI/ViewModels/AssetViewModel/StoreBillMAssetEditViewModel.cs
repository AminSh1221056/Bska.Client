
namespace Bska.Client.UI.ViewModels.AssetViewModel
{
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Linq;

    public sealed class StoreBillMAssetEditViewModel : BaseViewModel
    {
        #region ctor

        public StoreBillMAssetEditViewModel(IUnityContainer container,StoreBill storeBill)
        {
            this._container = container;
            this._navigationService = _container.Resolve<INavigationService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._storeBill = storeBill;
            this.initializObj();
            this.initializCommands();
        }

        #endregion

        #region properties

        public string Description
        {
            get { return GetValue(() => Description); }
            set
            {
                SetValue(() => Description, value);
            }
        }

        public List<MovableAssetModel> AssetList
        {
            get { return GetValue(() => AssetList); }
            set
            {
                SetValue(() => AssetList, value);
                if (AssetList != null)
                    this.Assets = new CollectionViewSource { Source = AssetList }.View;
            }
        }

        public MovableAssetModel Selected
        {
            get { return GetValue(() => Selected); }
            set
            {
                SetValue(() => Selected, value);
            }
        }

        public List<Unit> Units
        {
            get { return GetValue(() => Units); }
            set
            {
                SetValue(() => Units, value);
            }
        }

        public ICollectionView Assets { get; private set; }

        public String SearchCriteria
        {
            get { return GetValue(() => SearchCriteria); }
            set
            {
                SetValue(() => SearchCriteria, value);
                this.movableAssetFilters();
            }
        }
        #endregion

        #region methods

        private void initializObj()
        {
            Description = string.Format("ویرایش اموال قبض انبار با شماره {0}", _storeBill.StoreBillNo);
            
            this.Units = _unitService.Queryable().ToList();
        }

        private void movableAssetFilters()
        {
            Assets.Filter = (obj) =>
            {
                var asset = obj as MovableAssetModel;
                return asset.Name.Contains(SearchCriteria) || asset.Label.ToString() == SearchCriteria;
            };
        }
        private void showMAssetDetails(IList<object> parameters)
        {
            var mAsset = parameters[0] as MovableAssetModel;
            if (mAsset == null) return;
            var window = parameters[1] as Window;
            if (window == null) return;
            this.Selected = mAsset;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardHideWindow", window);
            bool isEditable = true;
            if (mAsset.IsCompietion != Common.CompietionState.NotReported)
            {
                isEditable = false;
            }
            if (string.Equals(mAsset.MAssetType, "مصرفی"))
            {
                var viewModel = new CommodityDetailsViewModel(_container, mAsset.AssetId, null,isEditable);
                _navigationService.ShowCommodityDetailsWindow(viewModel);
            }
            else
            {
                var viewModel = new MovableAssetDetailsViewModel(_container, mAsset.AssetId,null,isEditable);
                _navigationService.ShowMAssetDetailsWindow(viewModel);
            }
            StoryboardManager.PlayStoryboard("StoryboardShowWindow", window);
            Mouse.SetCursor(Cursors.Arrow);
        }
        #endregion

        #region commands

        public ICommand DetailsCommand { get; private set; }

        private void initializCommands()
        {
            DetailsCommand = new MvvmCommand(
                (parameter) => { this.showMAssetDetails(parameter as IList<object>); },
                (parameter) => { return true; }
                );
        }
        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly INavigationService _navigationService;
        private readonly IUnitService _unitService;
        private readonly StoreBill _storeBill;

        #endregion

    }
}
