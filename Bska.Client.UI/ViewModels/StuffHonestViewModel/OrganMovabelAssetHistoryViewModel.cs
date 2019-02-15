
namespace Bska.Client.UI.ViewModels.StuffHonestViewModel
{
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Helper;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Data.Entity;
    using Bska.Client.UI.Controls.CustomGridView.Querying;

    public sealed class OrganMovabelAssetHistoryViewModel : BaseViewModel
    {
        #region ctor

        public OrganMovabelAssetHistoryViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitOfWork=_container.Resolve<IUnitOfWorkAsync>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._items = new ObservableCollection<Tuple<MAssetCurState, string>>();
            this._collection = new ObservableCollection<MovableAssetModel>();
            this.initalizObj();
            this.initializCommand();
        }

        #endregion

        #region properties

        public Window Window
        {
            get { return GetValue(() => Window); }
            set
            {
                SetValue(() => Window, value);
            }
        }
        
        public ObservableCollection<MovableAssetModel> Collection
        {
            get { return _collection; }
            set
            {
                _collection = value;
                OnPropertyChanged("Collection");
            }
        }
        
        public List<Unit> Units
        {
            get { return GetValue(() => Units); }
            set { SetValue(() => Units, value); }
        }

        public MovableAssetModel Selected
        {
            get { return GetValue(() => Selected); }
            set
            {
                SetValue(() => Selected, value);
            }
        }

        public ObservableCollection<Tuple<MAssetCurState,string>> Items
        {
           get { return _items; }
        }

        public List<Tuple<MAssetCurState, string>> SelectedStates
        {
            get { return GetValue(() => SelectedStates); }
            set
            {
                SetValue(() => SelectedStates, value);
            }
        }

        public Dictionary<string, object> StuffTypes
        {
            get { return GetValue(() => StuffTypes); }
            set
            {
                SetValue(() => StuffTypes, value);
            }
        }

        public Dictionary<string, object> SelectedStuffType
        {
            get { return GetValue(() => SelectedStuffType); }
            set
            {
                SetValue(() => SelectedStuffType, value);
            }
        }
        
        public QueryController QueryControllerItem
        {
            get { return GetValue(() => QueryControllerItem); }
            set
            {
                SetValue(() => QueryControllerItem, value);
            }
        }
        #endregion

        #region methods

        private void initalizObj()
        {
             Enum.GetValues(typeof(MAssetCurState)).Cast<MAssetCurState>().ForEach(s=>
             {
                 _items.Add(new Tuple<MAssetCurState, string>(s,s.GetDescription()));
             });
            SelectedStates = new List<Tuple<MAssetCurState, string>>();
            this.Units = _unitService.Queryable().ToList();
            StuffTypes = new Dictionary<string, object> {{ StuffType.UnConsumption.GetDescription(),(int) StuffType.UnConsumption }
            ,{ StuffType.OrderConsumption.GetDescription(),(int)StuffType.OrderConsumption},{ StuffType.Installable.GetDescription(),(int)StuffType.Installable},{ StuffType.Belonging.GetDescription(),(int)StuffType.Belonging}};

            SelectedStuffType = new Dictionary<string, object> {{ StuffType.UnConsumption.GetDescription(), (int)StuffType.UnConsumption }
            ,{ StuffType.OrderConsumption.GetDescription(),(int)StuffType.OrderConsumption},{ StuffType.Installable.GetDescription(),(int)StuffType.Installable},{ StuffType.Belonging.GetDescription(),(int)StuffType.Belonging}};

        }
        
        private async void getMAssetsAsync()
        {
            this.Collection = new ObservableCollection<MovableAssetModel>
                (new List<MovableAssetModel> { new MovableAssetModel() });
            _collection.Clear();
            if (SelectedStates == null) return;
            

            var ts = new Task(() =>
            {
                var query= _movableAssetService.Queryable().Include(ma => ma.StoreBill)
                    .Include(ma => ma.Locations).AsNoTracking();

                if (!SelectedStates.Any(v=>v.Item1==MAssetCurState.Noen))
                {
                    var states = SelectedStates.Select(x => x.Item1).ToList();
                    query = _movableAssetService.Queryable().Where(x => states.Contains(x.CurState))
                    .Include(ma => ma.StoreBill).Include(ma => ma.Locations).AsNoTracking();
                }
                
                if (query != null)
                {
                    var items = query.AsEnumerable().Select(x => new MovableAssetModel
                    {
                        AssetId = x.AssetId,
                        MAssetType = x.ToString("t", null),
                        UnitId = x.UnitId,
                        Name = x.Name,
                        Num = x.Num,
                        Label = x.Label,
                        PersianInsertDate = x.InsertDate.PersianDateTime(),
                        CurState = x.CurState,
                        AcqType = x.StoreBill.AcqType,
                        IsCompietion = x.ISCompietion,
                        kalaUid = x.KalaUid,
                        KalaNo = x.KalaNo,
                        IsConfirmed = x.ISConfirmed,
                        IsInStore = x.Locations.Any(l => l.Status == LocationStatus.StoreActive || l.Status == LocationStatus.Retiring),
                    }).Where(x => SelectedStuffType.ContainsKey(x.MAssetType) && !(string.Equals(x.MAssetType, "مصرفی")));

                    items.AsParallel().ForAll(it =>
                    {
                        DispatchService.Invoke(() =>
                        {
                            this._collection.Add(it);
                        });
                    });
                }
            });
            ts.Start();
            await ts;
        }

        private void showAssetDetailsWindow(object parameter)
        {
            var mAsset = parameter as MovableAssetModel;
            if (mAsset == null) return;

            Mouse.SetCursor(Cursors.Wait);
            this.Selected = mAsset;
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            bool isEditable = false;
            if (!mAsset.IsConfirmed)
            {
                isEditable = true;
            }

            var viewModel = new MovableAssetDetailsViewModel(_container, mAsset.AssetId, isEditable: isEditable);
            var window = _navigationService.ShowMAssetDetailsWindow(viewModel);
            if (window.DialogResult == true)
            {
                this.getMAssetsAsync();
            }
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);

            Mouse.SetCursor(Cursors.Arrow);
        }

        private void reports()
        {
            Mouse.SetCursor(Cursors.Wait);
            //List<string> lst = new List<string>();
            //SelectedStuffType.Values.ForEach(s =>
            //{
            //    lst.Add(s.ToString());
            //});
            if (QueryControllerItem != null)
            {
                var viewModel = new ReportViewModel(isClient:true);
                if (this.QueryControllerItem.filteredCollection != null)
                {
                    viewModel.reportBookHistoryClient(this.QueryControllerItem.filteredCollection);
                }
                else
                {
                    viewModel.reportBookHistoryClient(this.QueryControllerItem.ItemsSource);
                }

                _navigationService.ShowReportViewWindow(viewModel);
            }
            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region commands

        public ICommand SearchCommand { get; private set; }
        public ICommand MAssetDetailsCommand { get; private set; }
        public ICommand ReportCommand { get; private set; }
        public ICommand DoubleClickListViewItemCommand { get; private set; }
        private void initializCommand()
        {
            SearchCommand = new MvvmCommand(
               (parameter) => { this.getMAssetsAsync(); },
               (parameter) => { return true; }
               );

            MAssetDetailsCommand = new MvvmCommand(
                (parameter) => { this.showAssetDetailsWindow(parameter); },
               (parameter) => { return true; }
               );

            ReportCommand = new MvvmCommand(
                (parameter) => { this.reports(); },
                (parameter) => { return true; }
                );

            DoubleClickListViewItemCommand = new MvvmCommand(
            (parameter) => { this.showAssetDetailsWindow(parameter); },
            (parameter) => { return true; }
            );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IUnitService _unitService;
        private ObservableCollection<MovableAssetModel> _collection;
        private readonly ObservableCollection<Tuple<MAssetCurState,string>> _items;

        #endregion
    }
}
