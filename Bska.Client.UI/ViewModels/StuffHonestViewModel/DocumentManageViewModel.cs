
namespace Bska.Client.UI.ViewModels.StuffHonestViewModel
{
    using Bska.Client.Domain.Entity;
    using Bska.Client.Common;
    using Bska.Client.UI.Helper;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Linq;
    using System.Collections.ObjectModel;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.UI.Services;
    using Bska.Client.Data.Service;
    using System.Windows.Input;
    using Bska.Client.UI.API;
    using Repository.Model;
    using System.Threading.Tasks;
    using System.ComponentModel;
    using System.Windows.Data;

    public sealed class DocumentManageViewModel : BaseViewModel
    {
        #region ctor

        public DocumentManageViewModel(IUnityContainer container,int docNo)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>();
            this._commodityService = _container.Resolve<IMAssetCommodityService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._storeBillService = _container.Resolve<IStoreBillService>();
            this._storeService = _container.Resolve<IStoreService>();
            this._personService = _container.Resolve<IPersonService>();
            this._storeBillCollection = new ObservableCollection<StoreBillModel>();
            this._documentCollection = new ObservableCollection<DocumentModel>();
            this._items = new ObservableCollection<Tuple<DocumentType, string>>();
            this.initalizObj(docNo);
            this.initalizCommand();
        }

        #endregion

        #region properties
        public Window Window
        {
            get;
            set;
        }

        public Boolean IsDocumentView
        {
            get;
            set;
        }

        public String BillNo
        {
            get { return GetValue(() => BillNo); }
            set
            {
                SetValue(() => BillNo, value);
            }
        }

        public String DraftNo
        {
            get { return GetValue(() => DraftNo); }
            set
            {
                SetValue(() => DraftNo, value);
            }
        }

        public String InitialBalanceNo
        {
            get { return GetValue(() => InitialBalanceNo); }
            set
            {
                SetValue(() => InitialBalanceNo, value);
            }
        }

        public String DocumentYear
        {
            get { return GetValue(() => DocumentYear); }
            set
            {
                SetValue(() => DocumentYear, value);
            }
        }

        public ObservableCollection<StoreBillModel> StoreBillCollection
        {
            get { return _storeBillCollection; }
            set
            {
                _storeBillCollection = value;
                OnPropertyChanged("StoreBillCollection");
            }
        }

        public ObservableCollection<DocumentModel> DocumentCollection
        {
            get { return _documentCollection; }
            set
            {
                _documentCollection = value;
                OnPropertyChanged("DocumentCollection");
            }
        }
        
        public StoreBillModel SelectedBill
        {
            get { return GetValue(() => SelectedBill); }
            set
            {
                SetValue(() => SelectedBill, value);
            }
        }

        public DocumentModel SelectedDocument
        {
            get { return GetValue(() => SelectedDocument); }
            set
            {
                SetValue(() => SelectedDocument, value);
            }
        }

        public ObservableCollection<Tuple<DocumentType,string>> Items
        {
            get { return _items; }
        }

        public List<Tuple<DocumentType, string>> SelectedDocTypes
        {
            get { return GetValue(() => SelectedDocTypes); }
            set
            {
                SetValue(() => SelectedDocTypes, value);
            }
        }
        public PersianDate FromDate
        {
            get { return GetValue(() => FromDate); }
            set
            {
                SetValue(() => FromDate, value);
            }
        }

        public PersianDate ToDate
        {
            get { return GetValue(() => ToDate); }
            set
            {
                SetValue(() => ToDate, value);
            }
        }

        public Dictionary<string, object> StoresList
        {
            get { return GetValue(() => StoresList); }
            set
            {
                SetValue(() => StoresList, value);
            }
        }

        public Dictionary<string, object> SelectedStores
        {
            get { return GetValue(() => SelectedStores); }
            set
            {
                SetValue(() => SelectedStores, value);
            }
        }
        #endregion

        #region methods

        private void initalizObj(int docNo)
        {
            Mouse.SetCursor(Cursors.Wait);
            StoresList = new Dictionary<string, object>();
            SelectedStores = new Dictionary<string, object>();

            if (UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.Manager ||
             UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.StuffHonest)
            {
                 _storeService.Queryable().ToList().ForEach(st=>
                 {
                     StoresList.Add(st.Name, st.StoreId);
                     SelectedStores.Add(st.Name, st.StoreId);
                 });
            }
            else if (UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.StoreLeader)
            {
                var storeRoles = _personService.GetRolesByUser(UserLog.UniqueInstance.LogedUser.UserId)
                    .Where(x => x.RoleType == PermissionsType.StoreLeader).Select(x => x.StoreId);
                _storeService.Queryable().Where(x => storeRoles.Contains(x.StoreId)).ToList().ForEach(st =>
                {
                    StoresList.Add(st.Name, st.StoreId);
                    SelectedStores.Add(st.Name, st.StoreId);
                });
            }
            FromDate = GlobalClass._Today.AddMonths(-(APPSettings.Default.SearchDateMonth)).PersianDateTime();
            ToDate = GlobalClass._Today.PersianDateTime();
            if (docNo == 2)
            {
                Enum.GetValues(typeof(DocumentType)).Cast<DocumentType>().ForEach(s =>
                {
                    _items.Add(new Tuple<DocumentType, string>(s, s.GetDescription()));
                });
                SelectedDocTypes = new List<Tuple<DocumentType, string>>();
                _searchNo = 2;
                this.GetDocumentsAsync();
            }
            else
            {
                _searchNo = 1;
                this.GetAllStoreBillsAsync();
            }

            Mouse.SetCursor(Cursors.Arrow);
        }
        
        private async void GetAllStoreBillsAsync()
        {
            Mouse.SetCursor(Cursors.Wait);
            this.SelectedDocument = null;
            IsDocumentView = false;

            _storeBillCollection.Clear();
            var sIds = SelectedStores.Select(s=>(int)s.Value).ToArray();
            DateTime date1 = this.FromDate.ToDateTime();
            DateTime date2 = this.ToDate.ToDateTime().AddDays(1);

            await Task.Run(() =>
            {
                var items = _storeBillService.Queryable().Where(sb => sIds.Contains(sb.StoreId.Value)
                && (sb.ArrivalDate>date1 && sb.ArrivalDate<=date2)).AsEnumerable()
               .Select(b=>new StoreBillModel
               {
                   AcqType=b.AcqType,
                   ArrivalDate=b.ArrivalDate,
                   SellerId=b.SellerId,
                   StoreBillId=b.StoreBillId,
                   StoreBillNo=b.StoreBillNo,
                   StoreId=b.StoreId,
                   StuffType=b.StuffType,
                   PersianArrivalDate=b.ArrivalDate.PersianDateTime()
               });

                if (!string.IsNullOrEmpty(DocumentYear))
                {
                    items = items.Where(m => m.ArrivalDate.PersianDateString().Split('/')[0].Contains(DocumentYear));
                }

                if (!string.IsNullOrEmpty(BillNo))
                {
                    items = items.Where(m => m.StoreBillNo.StartsWith(BillNo));
                }
                
                DispatchService.Invoke(() =>
                {
                    StoreBillCollection = new ObservableCollection<StoreBillModel>(items);
                });
            });
            
            Mouse.SetCursor(Cursors.Arrow);
        }

        private async void GetDocumentsAsync()
        {
            Mouse.SetCursor(Cursors.Wait);
            this.SelectedBill = null;
            IsDocumentView = true;
            _documentCollection.Clear();
            var sIds = SelectedStores.Select(s => s.Value).ToArray();
            DateTime date1 = this.FromDate.ToDateTime();
            DateTime date2 = this.ToDate.ToDateTime().AddDays(1);
            await Task.Run(() =>
            {
                var items = _movableAssetService.GetAllDocuments().Where(sb => sIds.Contains(sb.StoreId.Value)
                && (sb.DocumentDate > date1 && sb.DocumentDate <= date2)).Select(d=>new DocumentModel
                {
                    Desc1=d.Desc1,
                    DocumentDate=d.DocumentDate,
                    DocumentId=d.DocumentId,
                    DocumentType=d.DocumentType,
                    StoreId=d.StoreId,
                    Transferee=d.Transferee,
                    StoreName=d.Store.Name,
                    PersianDocumentDate=d.DocumentDate.PersianDateTime()
                });

                if (SelectedDocTypes.Count > 0 && !SelectedDocTypes.Any(v => v.Item1 == DocumentType.None))
                {
                    var types = SelectedDocTypes.Select(d => d.Item1).ToList();
                    items = items.Where(d => types.Contains(d.DocumentType));
                }

                if (!string.IsNullOrEmpty(DocumentYear))
                {
                    items = items.Where(d => d.DocumentDate.PersianDateString()
                        .Split('/')[0].Contains(DocumentYear));
                }

                if (!string.IsNullOrEmpty(DraftNo))
                {
                    items = items.Where(d => d.Desc1.StartsWith(DraftNo));
                }
                
                DispatchService.Invoke(() =>
                {
                    DocumentCollection = new ObservableCollection<DocumentModel>(items);
                });

            });

            Mouse.SetCursor(Cursors.Arrow);
        }

        private void SearchMovableAssetByStoreBill(IList<object> parameters)
        {
            Mouse.SetCursor(Cursors.Wait);

            var storeBill = parameters[0] as StoreBillModel;
            if (storeBill == null) return;
            string s1 = "";
            string s2 = "";
            var win = parameters[1] as Window;
            if (this.Window == null && win != null)
            {
                s1 = "StoryboardHideWindow";
                s2 = "StoryboardShowWindow";
            }
            else
            {
                win = Window;
                s1 = "StoryboardFadeOut";
                s2 = "StoryboardFadeIn";
            }
            SelectedBill = storeBill;
            List<MovableAssetModel> mAssets = null;
            if (SelectedBill.StuffType == StuffType.Consumable)
            {
                mAssets = _commodityService.GetCommodityByBillId(storeBill.StoreBillId).ToList();
            }
            else
            {
                mAssets = _movableAssetService.GetMovableAssetByStoreBill(storeBill.StoreBillId).ToList();
            }

            StoryboardManager.PlayStoryboard(s1, win);
            var viewModel = new MAssetListViewModel(_container, 1002, storeBill.StoreBillId,storeBill.StoreBillNo,0);
            viewModel.AssetList = mAssets;
            var window = _navigationService.ShowMAssetListWindow(viewModel);
            StoryboardManager.PlayStoryboard(s2, win);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void SerachMovableAssetByDocument(IList<object> parameters)
        {
            Mouse.SetCursor(Cursors.Wait);
            var document = parameters[0] as DocumentModel;
            if (document == null) return;
            string s1 = "";
            string s2 = "";
            var win = parameters[1] as Window;
            if (this.Window == null && win != null)
            {
                s1 = "StoryboardHideWindow";
                s2 = "StoryboardShowWindow";
            }
            else
            {
                win = Window;
                s1 = "StoryboardFadeOut";
                s2 = "StoryboardFadeIn";
            }
            SelectedDocument = document;
            var mAssets = _movableAssetService.GetMovableAssetByDocument(document.DocumentId).ToList();
            mAssets.AddRange(_commodityService.GetCommodityByDocId(document.DocumentId));

            StoryboardManager.PlayStoryboard(s1, win);
            var viewModel = new MAssetListViewModel(_container, 1006, SelectedDocument.DocumentId,document.Desc1,(int)document.DocumentType);
            viewModel.AssetList = mAssets;
            var window = _navigationService.ShowMAssetListWindow(viewModel);
            StoryboardManager.PlayStoryboard(s2, win);
            Mouse.SetCursor(Cursors.Arrow);
        }
        
        private void showStoreBillDetails(IList<object> parameters)
        {
            var storeBill = parameters[0] as StoreBillModel;
            if (storeBill == null) return;
            SelectedBill = storeBill;
            string s1 = "";
            string s2 = "";
            var win = parameters[1] as Window;
            if (this.Window == null && win!=null)
            {
                s1 = "StoryboardHideWindow";
                s2 = "StoryboardShowWindow";
            }
            else
            {
                win = Window;
               s1 = "StoryboardFadeOut";
               s2 = "StoryboardFadeIn";
            }
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard(s1, win);
            var bill = _storeBillService.Find(storeBill.StoreBillId);
            var viewModel = new StoreBillDetailsViewModel(_container,bill,true);
            _navigationService.ShowStoreBillDetailsWindow(viewModel);
            Mouse.SetCursor(Cursors.Arrow);
            StoryboardManager.PlayStoryboard(s2, win);

        }
        
        private void ReportListDocument()
        {
            Mouse.SetCursor(Cursors.Wait);
            DateTime? pDate1 = default(DateTime?);
            DateTime? pDate2 = default(DateTime?);
            int temp = 0;
            if (!string.IsNullOrWhiteSpace(DocumentYear))
            {
                if (DocumentYear.Length == 2)
                {
                    DocumentYear = PersianDate.Today.Year.ToString().Substring(0, 2) + DocumentYear;
                }

                if (int.TryParse(DocumentYear, out temp))
                {
                    pDate1 = new PersianDate(temp, 1, 1).ToDateTime();
                    pDate2 = new PersianDate(temp, 12, 29).ToDateTime();
                }
            }

            var viewModel = new ReportViewModel();
            if (IsDocumentView)
            {
                viewModel.AllDocumentsReport(SelectedDocTypes.Select(x => ((int)x.Item1).ToString()).ToArray(), 0);
            }
            else
            {
                viewModel.AllBillsReport(null,0,pDate1,pDate2,BillNo);
            }

            _navigationService.ShowReportViewWindow(viewModel);
            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region commands

        public ICommand SearchCommand { get; private set; }
        public ICommand DetailsCommand { get; private set; }
        public ICommand BillDetailsCommand { get; private set; }
        public ICommand ReportCommand { get; private set; }
        public ICommand ReportDetailsCommand { get; private set; }
        public ICommand DoubleClickListViewItemCommand { get; private set; }
        private void initalizCommand()
        {
            SearchCommand = new MvvmCommand(
                (parameter) =>
                {
                    if (_searchNo == 1)
                    {
                        this.GetAllStoreBillsAsync();
                    }
                    else
                    {
                        this.GetDocumentsAsync();
                    }
                },
                (parameter) =>
                {
                    return true;
                }
                );

            DetailsCommand = new MvvmCommand(
               (parameter) =>
               {
                   if (_searchNo==1) this.SearchMovableAssetByStoreBill(parameter as IList<object>);
                   else this.SerachMovableAssetByDocument(parameter as IList<object>);
               },
               (parameter) =>
               {
                   return true;
               }
               );

            BillDetailsCommand = new MvvmCommand(
                (parameter) => { this.showStoreBillDetails(parameter as IList<object>); },
                (parameter) => { return true; }
                );
            
            ReportCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.ReportListDocument();
                },
               (parameter) =>
               {
                   return true;
               }
                );

            DoubleClickListViewItemCommand = new MvvmCommand(
            (parameter) => 
            {
                if (SelectedBill != null) showStoreBillDetails(new List<object> { this.SelectedBill, this.Window });
                else SerachMovableAssetByDocument(new List<object> { this.SelectedDocument,this.Window});
            },
            (parameter) => { return true; }
            );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IMAssetCommodityService _commodityService;
        private readonly IStoreService _storeService;
        private readonly INavigationService _navigationService;
        private readonly IStoreBillService _storeBillService;
        private readonly IPersonService _personService;
        private ObservableCollection<StoreBillModel> _storeBillCollection;
        private ObservableCollection<DocumentModel> _documentCollection;
        private readonly ObservableCollection<Tuple<DocumentType, string>> _items;
        int _searchNo = 1;

        #endregion
    }
}
