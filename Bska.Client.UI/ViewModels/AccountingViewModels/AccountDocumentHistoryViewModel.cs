
namespace Bska.Client.UI.ViewModels.AccountingViewModels
{
    using Data.Service;
    using Microsoft.Practices.Unity;
    using Services;
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Input;
    using System.Linq;
    using TreeViewModels;
    using Repository.Model;
    using System.Collections.Generic;
    using Helper;
    using Domain.Entity;
    using Common;
    using API;
    using System.Threading.Tasks;
    using System.ComponentModel;
    using System.Windows.Data;
    using Domain.Entity.AssetEntity;

    public sealed class AccountDocumentHistoryViewModel : BaseViewModel
    {
        #region ctor

        public AccountDocumentHistoryViewModel(IUnityContainer container)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this._employeeService = _container.Resolve<IEmployeeService>();
            this._firstGeneration = new ObservableCollection<AccountCodingHistoryTreeViewModel>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._collection = new ObservableCollection<AccountDocumentDetailsModel>();
            this._masterAccounts = new ObservableCollection<AccountDocumentMaster>();
            this.AccountDocFilteredView = new CollectionViewSource { Source = MasterCollection }.View;
            this.initializObj();
            this.initializCommand();
        }

        #endregion

        #region properties

        public Window Window { get; set; }

        public AccountCodingHistoryTreeViewModel SelectedNode
        {
            get { return GetValue(() => SelectedNode); }
            set
            {
                SetValue(() => SelectedNode, value);
               // this.getAccountDocumetnsAsync();
            }
        }
        
        public ObservableCollection<AccountDocumentMaster> MasterCollection
        {
            get { return _masterAccounts; }
        }

        public ObservableCollection<AccountCodingHistoryTreeViewModel> FirstGeneration
        {
            get { return _firstGeneration; }
        }

        public bool IsEditable
        {
            get { return GetValue(() => IsEditable); }
            set
            {
                SetValue(() => IsEditable, value);
            }
        }

        public Boolean IsMinCost
        {
            get { return GetValue(() => IsMinCost); }
            set
            {
                SetValue(() => IsMinCost, value);
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

        public Decimal MinCost
        {
            get { return GetValue(() => MinCost); }
            set
            {
                SetValue(() => MinCost, value);
            }
        }

        public string SearchText
        {
            get { return GetValue(() => SearchText); }
            set
            {
                SetValue(() => SearchText, value);
            }
        }

        public ObservableCollection<AccountDocumentDetailsModel> Collection
        {
            get { return _collection; }
            set
            {
                _collection = value;
                OnPropertyChanged("Collection");
            }
        }
        public ICollectionView AccountDocFilteredView { get; set; }
        public AccountDocumentDetailsModel SelectedDoc
        {
            get { return GetValue(() => SelectedDoc); }
            set
            {
                SetValue(() => SelectedDoc, value);
            }
        }

        public String DebtorDescription
        {
            get { return GetValue(() => DebtorDescription); }
            set
            {
                SetValue(() => DebtorDescription, value);
            }
        }

        public String CreditorDescription
        {
            get { return GetValue(() => CreditorDescription); }
            set
            {
                SetValue(() => CreditorDescription, value);
            }
        }
        
        public string SearchAccount
        {
            get { return GetValue(() => SearchAccount); }
            set
            {
                SetValue(() => SearchAccount, value);
                this.searchMasterAccounts();
            }
        }

        public AccountDocumentMaster AccountDocSelected
        {
            get { return GetValue(() => AccountDocSelected); }
            set
            {
                SetValue(() => AccountDocSelected, value);
            }
        }
        #endregion

        #region methods

        private void initializObj()
        {
            Mouse.SetCursor(Cursors.Wait);
            this.IsEditable=IsMinCost = false;
            FromDate = PersianDate.Today.AddDays(-30);
            ToDate = PersianDate.Today.AddDays(1);
            MinCost = 0;
            this.SelectedNode = null;
            _firstGeneration.Clear();
            var items = new List<AccountCodingModel>();
            _employeeService.GetAccountCodings().Where(x=>x.Parent==null).ToList().ForEach(nd =>
            {
                var newnode = new AccountCodingModel
                {
                    AccountCode=nd.AccountCode,
                    CertainAccountType=nd.CertainAccountType,
                    Id=nd.ID,
                    Name=nd.Name,
                    TotalAccountType=nd.TotalAccountType,
                };
                newnode.Parent = null;
                items.Add(newnode);
            });
            var item = items.Where(x => x.Parent == null).FirstOrDefault();
            var rootNod = new AccountCodingHistoryTreeViewModel(_container, item, null) { IsSelected=true};
            _firstGeneration.Add(rootNod);
            this.getMasterAccountsAsync();
            this.SelectedNode = rootNod;
            Mouse.SetCursor(Cursors.Arrow);
        }
        
        private async void getMasterAccountsAsync()
        {
            await Task.Run(() =>
            {
                _employeeService.GetAccountMaster().ForEach(am =>
                {
                    DispatchService.Invoke(() =>
                    {
                        _masterAccounts.Add(am);
                    });
                });
            });
        }

        private async void getAccountDocumetnsAsync()
        {
            if (SelectedNode == null) return;
            string treeCode = this.createCode(SelectedNode);
            var splitCode = treeCode.Split('-');
            if (splitCode.Count() == 4)
            {
               treeCode=splitCode[0]+"-"+splitCode[1]+"-"+splitCode[2]+"-"+GlobalClass.CheckAccountCode(treeCode.Split('-').Last());
            }
            Collection = new ObservableCollection<AccountDocumentDetailsModel>
                (new List<AccountDocumentDetailsModel> { new AccountDocumentDetailsModel() });
            _collection.Clear();

            DebtorDescription = "";
            CreditorDescription = "";
            await Task.Run(() =>
            {
                var coings = _employeeService.GetAccountCodings().ToList();
                _accounts = _employeeService.GetAccountDetails(treeCode, FromDate.ToDateTime(), ToDate.ToDateTime()).ToList();

                if (MinCost > 0)
                {
                    if (IsMinCost)
                    {
                        _accounts = _accounts.Where(x => x.MAsset.Cost >= MinCost);
                    }
                    else
                    {
                        _accounts = _accounts.Where(x => x.MAsset.Cost <= MinCost);
                    }
                }

                if (AccountDocSelected != null)
                {
                    _accounts = _accounts.Where(x => x.MasterId == AccountDocSelected.ID);
                }

                decimal creditor = 0;
                decimal debtor = 0;
                _accounts.ForEach(acd =>
                {
                    string[] splitCoding = acd.AccountNo.Split('-');
                    string code = splitCoding[2];
                    string totalCode = splitCoding[1];
                    var cod = coings.FirstOrDefault(x => x.AccountCode == code);
                    var totalCod = coings.FirstOrDefault(x => x.AccountCode == totalCode);
                    var item = new AccountDocumentDetailsModel
                    {
                        AccountNo = acd.AccountNo,
                        AssetId = acd.AssetId,
                        ID = acd.ID,
                        MasterId = acd.MasterId,
                        Description = acd.Description,
                        AssetLabel =acd.MAsset.Label,
                        AssetName=acd.MAsset.Name,
                        Creditor=acd.Creditor,
                        Debtor=acd.Debtor,
                    };
                    creditor += acd.Creditor;
                    debtor += acd.Debtor;
                    if (cod != null)
                    {
                        item.TotalAccount = totalCod.Name;
                    }
                    else
                    {
                        item.TotalAccount = "ناشناخته";
                    }

                    DispatchService.Invoke(() =>
                    {
                        _collection.Add(item);
                        DebtorDescription = $"جمع بدهکار : {debtor.ToString("N0")}";
                        CreditorDescription = $"جمع بستانکار : {creditor.ToString("N0")}";
                    });
                });
            });
        }

        private string createCode(AccountCodingHistoryTreeViewModel item)
        {
            string treeCode ="";
            if (item.Parent != null)
            {
                treeCode += this.createCode(item.Parent);
                if(item.Parent.AccountDocumentCodingCurrent.CertainAccountType!=CertainAccountsType.UnitsChilderen)
                treeCode += "-";
                
            }
            treeCode += item.AccountDocumentCodingCurrent.AccountCode;
            return treeCode;
        }

        private void searchMasterAccounts()
        {
            if (string.IsNullOrWhiteSpace(SearchAccount))
            {
                this.AccountDocFilteredView.Filter = null;
                return;
            }

            this.AccountDocFilteredView.Filter = (obj) =>
            {
                AccountDocumentMaster item = obj as AccountDocumentMaster;
                return item.ID.ToString() == SearchAccount;
            };
        }
        
        void PerformSearch()
        {
            if (_matchingStoreEnumerator == null || !_matchingStoreEnumerator.MoveNext())
            {
                this.VerifyMatchingPeopleEnumerator();
            }

            var store = _matchingStoreEnumerator.Current;

            if (store == null)
                return;

            // Ensure that this person is in view.
            if (store.Parent != null)
            {
                store.Parent.IsExpanded = true;
            }

            store.IsSelected = true;
        }

        void VerifyMatchingPeopleEnumerator()
        {
            try
            {
                var matches = this.FindParnetMatches(SearchText, _firstGeneration);

                _matchingStoreEnumerator = matches.GetEnumerator();

                if (!_matchingStoreEnumerator.MoveNext())
                {
                    _dialogService.ShowAlert("دوباره سعی کنید", "هیچ شاخه ای پیدا نشد");
                }
            }

            catch (NullReferenceException) { }
            catch (Exception) { throw; }
        }

        IEnumerable<AccountCodingHistoryTreeViewModel> FindParnetMatches(string searchText, IEnumerable<AccountCodingHistoryTreeViewModel> allaccount)
        {
            foreach (var account in allaccount)
            {
                if (account.NameContainsText(searchText))
                    yield return account;

                foreach (AccountCodingHistoryTreeViewModel child in account.Children)
                    foreach (AccountCodingHistoryTreeViewModel match in this.FindMatches(searchText, child))
                        yield return match;
            }
        }

        IEnumerable<AccountCodingHistoryTreeViewModel> FindMatches(string searchText, AccountCodingHistoryTreeViewModel account)
        {

            if (account.NameContainsText(searchText))
                yield return account;

            if(account.Children!=null)
            foreach (AccountCodingHistoryTreeViewModel child in account.Children)
                foreach (AccountCodingHistoryTreeViewModel match in this.FindMatches(searchText, child))
                    yield return match;
        }

        private String GetHirecharyNode(AccountDocumentCoding item)
        {
            String _nodeName = "";

            if (item.Parent != null)
            {
                _nodeName += this.GetHirecharyNode(item.Parent);
                _nodeName += "--";
            }

            _nodeName += item.Name;

            return _nodeName;
        }

        private void reportDocs()
        {
            if (SelectedNode == null) return;
            Mouse.SetCursor(Cursors.Wait);
            string treeCode = this.createCode(SelectedNode);
            string title = "";
            string[] splitCode = treeCode.Split('-');
            var coings = _employeeService.GetAccountCodings().ToList();
            string code = "";
            if (splitCode.Count() == 1)
            {
                code = splitCode[0];
                var cod = coings.FirstOrDefault(x => x.AccountCode == code);
                title= this.GetHirecharyNode(cod);
            }
            else if (splitCode.Count() == 2)
            {
                code = splitCode[1];
                var cod = coings.FirstOrDefault(x => x.AccountCode == code);
                title = this.GetHirecharyNode(cod);
            }
            else if (splitCode.Count() >= 3)
            {
                code = splitCode[2];
                var cod = coings.FirstOrDefault(x => x.AccountCode == code);
                title = this.GetHirecharyNode(cod);
            }

            if (splitCode.Count() == 4)
            {
                treeCode = splitCode[0] + "-" + splitCode[1] +"-"+ splitCode[2] + "-" + GlobalClass.CheckAccountCode(splitCode[3]);
            }

            int? mId = null;
            if (AccountDocSelected != null)
            {
                mId = AccountDocSelected.ID;
            }
            
            var viewModel = new ReportViewModel();
            viewModel.AccountdocMainListReport(title, treeCode,mId);
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showMAssetDetails(object parameter)
        {
            var item = parameter as AccountDocumentDetailsModel;
            if (item.AssetId == null) return;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", Window);
            this.SelectedDoc = item;
            var viewModel = new MovableAssetDetailsViewModel(_container, item.AssetId.Value);
            var window = _navigationService.ShowMAssetDetailsWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showMainDoc(object parameter)
        {
            var item = parameter as AccountDocumentMaster;
            if (item == null) return;
            Mouse.SetCursor(Cursors.Wait);
            this.AccountDocSelected = item;
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", Window);
            var viewModel = new MainAccountDocViewModel(_container,item);
            var window = _navigationService.ShowAccountDocMainWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", Window);
            Mouse.SetCursor(Cursors.Arrow);

        }

        private void redirectToRelatedMethod(object parameter)
        {
            if(parameter is AccountDocumentMaster)
            {
                showMainDoc(parameter);
            }
            else if(parameter is AccountDocumentDetailsModel)
            {
                this.showMAssetDetails(parameter);
            }
        }

        #endregion

        #region commands

        public ICommand SearchCommand { get; private set; }
        public ICommand ReportCommand { get; private set; }
        public ICommand AccountTreeSearchCommand { get; private set; }
        public ICommand DetailsCommand { get; private set; }
        public ICommand MainDocCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand DoubleClickListViewItemCommand { get; private set; }

        private void initializCommand()
        {
            AccountTreeSearchCommand = new MvvmCommand(
               (parameter) =>
               {
                   this.PerformSearch();
               },
               (parameter) =>
               {
                   return true;
               }
               );

            SearchCommand = new MvvmCommand(
                 (parameter) =>
                 {
                     this.getAccountDocumetnsAsync();
                 },
               (parameter) =>
               {
                   return true;
               }
                );

            ReportCommand = new MvvmCommand(
                (parameter) => { this.reportDocs(); },
                (parameter) => { return true; }
                );

            DetailsCommand = new MvvmCommand(
                (parameter) => { this.showMAssetDetails(parameter); },
                (parameter) => { return true; }
                );

            MainDocCommand= new MvvmCommand(
                (parameter) => { this.showMainDoc(parameter); },
                (parameter) => { return true; }
                );

            RefreshCommand = new MvvmCommand(
                (parameter) => { this.AccountDocSelected = null; },
                (parameter) => { return true; }
                );

            DoubleClickListViewItemCommand = new MvvmCommand(
                (parameter) => { this.redirectToRelatedMethod(parameter); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IEmployeeService _employeeService;
        private readonly ObservableCollection<AccountCodingHistoryTreeViewModel> _firstGeneration;
        IEnumerator<AccountCodingHistoryTreeViewModel> _matchingStoreEnumerator;
        private ObservableCollection<AccountDocumentDetailsModel> _collection;
        private IEnumerable<AccountDocumentDetails> _accounts;
        private readonly ObservableCollection<AccountDocumentMaster> _masterAccounts;
        #endregion

    }
}
