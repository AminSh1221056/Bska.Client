
namespace Bska.Client.UI.ViewModels.StuffHonestViewModel
{
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Data.Entity;

    public sealed class ProceedingViewModel : BaseViewModel
    {
        #region ctor

        public ProceedingViewModel(IUnityContainer container)
        {
            this._container = container;
            this. _dialogService = _container.Resolve<IDialogService>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>();
            this._proceedingService = _container.Resolve<IProceedingService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._Proceddings = new ObservableCollection<Proceeding>();
            this.ProceedingFilteredView = new CollectionViewSource { Source = Proceedings }.View;
            _items = new Dictionary<string, object>();
            this.initializObj();
            this.initalizCommand();
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

        public Proceeding Selected
        {
            get { return GetValue(() => Selected); }
            set
            {
                SetValue(() => Selected, value);
            }
        }

        public ICollectionView ProceedingFilteredView { get; set; }
        public ObservableCollection<Proceeding> Proceedings
        {
            get { return _Proceddings; }
        }
        
        public MovableAssetModel SelectedAsset
        {
            get { return GetValue(() => SelectedAsset); }
            set
            {
                SetValue(() => SelectedAsset, value);
            }
        }
        
        public Dictionary<string, object> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }

        public Dictionary<string, object> SelectedItems
        {
            get { return GetValue(() => SelectedItems); }
            set
            {
                SetValue(() => SelectedItems, value);
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
        public String SearchCriteria
        {
            get { return GetValue(() => SearchCriteria); }
            set
            {
                SetValue(() => SearchCriteria, value);
                this.searchProcs(1);
            }
        }

        #endregion

        #region methods

        private async void initializObj()
        {
            Mouse.SetCursor(Cursors.Wait);
            Enum.GetValues(typeof(ProceedingsType)).Cast<ProceedingsType>().ForEach(s =>
            {
                _items.Add(s.GetDescription(),s);
            });
            _items.Remove("کل صورت جلسات");
            SelectedItems = new Dictionary<string, object>();
            SelectedItems.Add("All", "All");
            await this.getProceedingAsync();
            Mouse.SetCursor(Cursors.Arrow);
        }

        private Task getProceedingAsync()
        {
            _Proceddings.Clear();
            Task ts = new Task(()=>
            {
                var procs = _proceedingService.Queryable().Include(p => p.AssetProceedings).OrderByDescending(o => o.ProceedingId).AsNoTracking().ToList();
                procs.ForEach(p =>
                {
                    DispatchService.Invoke(() =>
                    {
                        _Proceddings.Add(p);
                    });
                });
            });
            ts.Start();
            return ts;
        }

        private async void showProceedingDetails(object parameter)
        {
            var proc = parameter as Proceeding;
            if (proc == null) return;
            Selected = proc;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new ProceedingInformationViewModel(_container, proc.ProceedingId);
            var window=_navigationService.ShowProceedingDetailsWindow(viewModel);
            if (window.DialogResult == true)
            {
               await this.getProceedingAsync();
            }
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }
        
        private void searchProcs(int filterTyp)
        {

            if (filterTyp == 1)
            {
                ProceedingFilteredView.Filter = (obj) =>
                {
                    var proc = obj as Proceeding;
                    return proc.ProceedingId.ToString().StartsWith(SearchCriteria);
                };
            }
            else
            {
                if (SelectedItems.ContainsKey("All"))
                {
                    ProceedingFilteredView.Filter = null;
                }
                else
                {
                    ProceedingFilteredView.Filter = (obj) =>
                    {
                        var proc = obj as Proceeding;
                        return SelectedItems.ContainsValue(proc.Type);
                    };
                }
            }
        }

        private void showProcAssets(object parameter)
        {
            var proc = parameter as Proceeding;
            if (proc == null) return;
            Selected = proc;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new MAssetListViewModel(_container,1001,proc.ProceedingId);
            Int64[] assetIds = proc.AssetProceedings.Select(x => x.AssetId).ToArray();
            viewModel.AssetList = _movableAssetService.Queryable().Where(x => assetIds.Contains(x.AssetId)).AsEnumerable().Select(x => new MovableAssetModel
            {
                AssetId = x.AssetId,
                CurState = x.CurState,
                InsertDate = x.InsertDate,
                Name = x.Name,
                Num = x.Num,
                MAssetType = x.ToString("T", null),
                UnitId = x.UnitId,
                Label = x.Label,
                kalaUid = x.KalaUid,
            }).ToList();
            var window = _navigationService.ShowMAssetListWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void reports()
        {
            Mouse.SetCursor(Cursors.Wait);

            var viewModel = new ReportViewModel();
            List<string> _sItmes = new List<string>();
            if (SelectedItems.ContainsKey("All"))
            {
                _sItmes.Add("17000");
            }
            else
            {
                SelectedItems.ForEach(x =>
                {
                    ProceedingsType temp;
                    if (Enum.TryParse(x.Value.ToString(), out temp))
                    {
                        _sItmes.Add(((int)temp).ToString());
                    }

                });
            }
            string searchquery = null;
            if (!string.IsNullOrWhiteSpace(SearchCriteria))
            {
                searchquery = SearchCriteria;
            }
            viewModel.ProceedingListReport(_sItmes.ToArray(), searchquery);
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region commands

        public ICommand ProceedingDetailsCommand { get; private set; }
        public ICommand ProceedingMAssetCommand { get; private set; }
        public ICommand ReportCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }
        private void initalizCommand()
        {
            ProceedingDetailsCommand = new MvvmCommand(
                (parameter) => { this.showProceedingDetails(parameter); },
                (parameter) => { return true; });
            
            ProceedingMAssetCommand = new MvvmCommand(
                (parameter) => { this.showProcAssets(parameter); },
                (parameter) => { return true; }
                );
           
            ReportCommand = new MvvmCommand(
                (parameter) => { this.reports(); },
                (parameter) => { return true; }
                );

            SearchCommand = new MvvmCommand(
                  (parameter) => { this.searchProcs(2); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly ObservableCollection<Proceeding> _Proceddings;
        private readonly IProceedingService _proceedingService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IUnitService _unitService;
        private Dictionary<string, object> _items;

        #endregion
    }
}
