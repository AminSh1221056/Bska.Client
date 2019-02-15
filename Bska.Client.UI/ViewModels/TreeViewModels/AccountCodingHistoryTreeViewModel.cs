
namespace Bska.Client.UI.ViewModels.TreeViewModels
{
    using Repository.Model;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Data.Service;
    using Helper;
    using System.Threading.Tasks;
    using Common;
    using Domain.Entity.AssetEntity;

    public sealed class AccountCodingHistoryTreeViewModel : BaseViewModel
    {
        #region ctor
        
        private AccountCodingHistoryTreeViewModel(AccountCodingModel codingDesign)
        {
            this._codingDesign = codingDesign;
        }

        public AccountCodingHistoryTreeViewModel(IUnityContainer container,AccountCodingModel codingDesign, AccountCodingHistoryTreeViewModel parent)
        {
            this._container = container;
            this._employeeService = _container.Resolve<IEmployeeService>();
            _codingDesign = codingDesign;
            IsExpanded = false;
            _parent = parent;
            _children =
              new ObservableCollection<AccountCodingHistoryTreeViewModel>(
                (_codingDesign.Childeren.Select(child => new AccountCodingHistoryTreeViewModel(container, child, this))).ToList());
            _children.Add(DummyChild);
        }

        #endregion

        #region properties

        public bool IsSelected
        {
            get { return GetValue(() => IsSelected); }
            set
            {
                SetValue(() => IsSelected, value);
            }
        }
        
        public bool IsEnabled
        {
            get { return GetValue(() => IsEnabled); }
            set
            {
                SetValue(() => IsEnabled, value);
            }
        }

        public bool IsExpanded
        {
            get { return GetValue(() => IsExpanded); }
            set
            {
                SetValue(() => IsExpanded, value);
                // Expand all the way up to the root.
                if (value)
                {
                    if(_parent!=null)
                    _parent.IsExpanded = true;
                    //this.getChilderenAsync();
                }
            }
        }

        public bool IsEditing
        {
            get { return GetValue(() => IsEditing); }
            set
            {
                SetValue(() => IsEditing, value);
            }
        }

        public string Name
        {
            get { return _codingDesign.Name; }
            set
            {
                if (_codingDesign.Name != value)
                {
                    _codingDesign.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public string Code
        {
            get { return _codingDesign.AccountCode; }
            set
            {
                if (_codingDesign.AccountCode != value)
                {
                    _codingDesign.AccountCode = value;
                    OnPropertyChanged("Code");
                }
            }
        }

        public ObservableCollection<AccountCodingHistoryTreeViewModel> Children
        {
            get { return _children; }
        }

        public AccountCodingModel AccountDocumentCodingCurrent
        {
            get { return _codingDesign; }
        }

        public AccountCodingHistoryTreeViewModel Parent
        {
            get { return this._parent ?? null; }
        }
        public bool HasDummyChild
        {
            get { return this.Children.Count == 1 && this.Children[0] == DummyChild; }
        }
        #endregion

        #region methods

        //private async void getChilderenAsync()
        //{
        //    if(HasDummyChild)
        //    this._children.Remove(DummyChild);

        //    if (this.Children.Count <= 0)
        //    {
        //        var ts = new Task(() =>
        //          {
        //              if (this.AccountDocumentCodingCurrent.CertainAccountType ==CertainAccountsType.None)
        //              {
        //                  var items = _employeeService.GetAccountCodingsByParent(this.AccountDocumentCodingCurrent.Id);
        //                  items.ForEach(chd =>
        //                  {
        //                      var md = new AccountCodingModel
        //                      {
        //                          AccountCode = chd.AccountCode,
        //                          Id = chd.ID,
        //                          Name = chd.Name,
        //                      };
        //                      DispatchService.Invoke(() =>
        //                      {
        //                          this.Children.Add(new AccountCodingHistoryTreeViewModel(_container, md, this));
        //                      });
        //                  });
        //              }
        //              else
        //              {
        //                  if (this.AccountDocumentCodingCurrent.CertainAccountType == CertainAccountsType.ReachedAssetBuy)
        //                  {
        //                      var sellers = _employeeService.GetSellers();
        //                      sellers.ForEach(chd =>
        //                      {
        //                          var md = new AccountCodingModel
        //                          {
        //                              AccountCode = chd.Coding,
        //                              CertainAccountType =CertainAccountsType.Tafsili,
        //                              Id = chd.SellerId,
        //                              Name = chd.Name,
        //                              TotalAccountType =AccountingDescrtiption.None,
        //                          };
        //                          DispatchService.Invoke(() =>
        //                          {
        //                              this.Children.Add(new AccountCodingHistoryTreeViewModel(_container, md, this));
        //                          });
        //                      });
        //                  }
        //                  else if (this.AccountDocumentCodingCurrent.CertainAccountType == CertainAccountsType.ReachedAssetTransfer)
        //                  {
        //                      var seedDataHelper = new SeedDataHelper();
        //                      var organs = seedDataHelper.GetOrganizations();
        //                      var storeBillService = _container.Resolve<IStoreBillService>();
        //                      var bills = storeBillService.Queryable().Where(x => x.AcqType == StateOwnership.GovCompanyRecived).Select(x => x.Desc1).ToList();
        //                      organs.Where(x=> bills.Contains(x.BudgetNo.ToString())).ForEach(chd =>
        //                      {
        //                          var md = new AccountCodingModel
        //                          {
        //                              AccountCode = chd.BudgetNo.ToString(),
        //                              CertainAccountType = CertainAccountsType.Tafsili,
        //                              Id = chd.EmployeeId,
        //                              Name = chd.Name,
        //                              TotalAccountType = AccountingDescrtiption.None,
        //                          };
        //                          DispatchService.Invoke(() =>
        //                          {
        //                              this.Children.Add(new AccountCodingHistoryTreeViewModel(_container, md, this));
        //                          });
        //                      });
        //                  }
        //                  else if(this.AccountDocumentCodingCurrent.CertainAccountType == CertainAccountsType.TrustOrganizationReciver)
        //                  {
        //                      var seedDataHelper = new SeedDataHelper();
        //                      var organs = seedDataHelper.GetOrganizations();
        //                      var proceedingService = _container.Resolve<ProceedingService>();
        //                      var procs = proceedingService.Queryable().Where(x => x.Type == ProceedingsType.TrustTransfer && x.State==ProceedingState.CompletedConfirm).Select(x => x.Desc1).ToList();
        //                      organs.Where(x => procs.Contains(x.BudgetNo.ToString())).ForEach(chd =>
        //                      {
        //                          var md = new AccountCodingModel
        //                          {
        //                              AccountCode = chd.BudgetNo.ToString(),
        //                              CertainAccountType = CertainAccountsType.Tafsili,
        //                              Id = chd.EmployeeId,
        //                              Name = chd.Name,
        //                              TotalAccountType = AccountingDescrtiption.None,
        //                          };
        //                          DispatchService.Invoke(() =>
        //                          {
        //                              this.Children.Add(new AccountCodingHistoryTreeViewModel(_container, md, this));
        //                          });
        //                      });
        //                  }
        //                  else if (this.AccountDocumentCodingCurrent.CertainAccountType == CertainAccountsType.ReachedTrustOrganizationSender)
        //                  {
        //                      var seedDataHelper = new SeedDataHelper();
        //                      var organs = seedDataHelper.GetOrganizations();
        //                      var storeBillService = _container.Resolve<IStoreBillService>();
        //                      var bills = storeBillService.Queryable().Where(x => x.AcqType == StateOwnership.Trust).Select(x => x.Desc1).ToList();
        //                      organs.Where(x => bills.Contains(x.BudgetNo.ToString())).ForEach(chd =>
        //                      {
        //                          var md = new AccountCodingModel
        //                          {
        //                              AccountCode = chd.BudgetNo.ToString(),
        //                              CertainAccountType = CertainAccountsType.Tafsili,
        //                              Id = chd.EmployeeId,
        //                              Name = chd.Name,
        //                              TotalAccountType = AccountingDescrtiption.None,
        //                          };
        //                          DispatchService.Invoke(() =>
        //                          {
        //                              this.Children.Add(new AccountCodingHistoryTreeViewModel(_container, md, this));
        //                          });
        //                      });
        //                  }
        //                  else if (this.AccountDocumentCodingCurrent.CertainAccountType == CertainAccountsType.SendTransfer)
        //                  {
        //                      var seedDataHelper = new SeedDataHelper();
        //                      var organs = seedDataHelper.GetOrganizations();
        //                      var proceedingService = _container.Resolve<ProceedingService>();
        //                      var procs = proceedingService.Queryable().Where(x => (x.Type == ProceedingsType.StateTransfer
        //                         || x.Type == ProceedingsType.DefinitiveTransfer) && x.State == ProceedingState.CompletedConfirm).Select(x => x.Desc1).ToList();
        //                      organs.Where(x => procs.Contains(x.BudgetNo.ToString())).ForEach(chd =>
        //                      {
        //                          var md = new AccountCodingModel
        //                          {
        //                              AccountCode = chd.BudgetNo.ToString(),
        //                              CertainAccountType = CertainAccountsType.Tafsili,
        //                              Id = chd.EmployeeId,
        //                              Name = chd.Name,
        //                              TotalAccountType = AccountingDescrtiption.None,
        //                          };
        //                          DispatchService.Invoke(() =>
        //                          {
        //                              this.Children.Add(new AccountCodingHistoryTreeViewModel(_container, md, this));
        //                          });
        //                      });
        //                  }
        //                  else if (this.AccountDocumentCodingCurrent.CertainAccountType == CertainAccountsType.Escrow)
        //                  {
        //                      var seedDataHelper = new SeedDataHelper();
        //                      var organs = seedDataHelper.GetOrganizations();
        //                      var proceedingService = _container.Resolve<ProceedingService>();
        //                      var procs = proceedingService.Queryable().Where(x => (x.Type == ProceedingsType.RefundTrust) && x.State == ProceedingState.CompletedConfirm).Select(x => x.Desc1).ToList();
        //                      organs.Where(x => procs.Contains(x.BudgetNo.ToString())).ForEach(chd =>
        //                      {
        //                          var md = new AccountCodingModel
        //                          {
        //                              AccountCode = chd.BudgetNo.ToString(),
        //                              CertainAccountType = CertainAccountsType.Tafsili,
        //                              Id = chd.EmployeeId,
        //                              Name = chd.Name,
        //                              TotalAccountType = AccountingDescrtiption.None,
        //                          };
        //                          DispatchService.Invoke(() =>
        //                          {
        //                              this.Children.Add(new AccountCodingHistoryTreeViewModel(_container, md, this));
        //                          });
        //                      });
        //                  }
        //                  else if (this.AccountDocumentCodingCurrent.CertainAccountType==CertainAccountsType.RetiringAssetType
        //                  || this.AccountDocumentCodingCurrent.CertainAccountType==CertainAccountsType.StockAssetBuyAndType
        //                  || this.AccountDocumentCodingCurrent.CertainAccountType==CertainAccountsType.DisasterAssetType)
        //                  {
        //                      var mAssetService = _container.Resolve<IMovableAssetService>();
        //                      var stuffs = mAssetService.Queryable().OfType<UnConsumption>().Select(x => new
        //                      {
        //                          x.Name,
        //                          x.KalaUid,
        //                      }).DistinctBy(x=>x.KalaUid).AsEnumerable();
                            
        //                      stuffs.ForEach(chd =>
        //                      {
        //                          var md = new AccountCodingModel
        //                          {
        //                              AccountCode = chd.KalaUid.ToString(),
        //                              CertainAccountType = CertainAccountsType.Tafsili,
        //                              Id = chd.KalaUid,
        //                              Name = chd.Name,
        //                              TotalAccountType = AccountingDescrtiption.None,
        //                          };
        //                          DispatchService.Invoke(() =>
        //                          {
        //                              this.Children.Add(new AccountCodingHistoryTreeViewModel(_container, md, this));
        //                          });
        //                      });
        //                  }
        //                  else if (this.AccountDocumentCodingCurrent.CertainAccountType == CertainAccountsType.UnitsDeliviry)
        //                  {
        //                      var organizations = _employeeService.GetParentNode(1).Where(x => x.ParentNode == null).ToList();
        //                      organizations.ForEach(chd =>
        //                      {
        //                          var md = new AccountCodingModel
        //                          {
        //                              AccountCode = chd.BuidldingDesignId.ToString(),
        //                              CertainAccountType = CertainAccountsType.UnitsChilderen,
        //                              Id = chd.BuidldingDesignId,
        //                              Name = chd.Name,
        //                              TotalAccountType = AccountingDescrtiption.None,
        //                          };
        //                          DispatchService.Invoke(() =>
        //                          {
        //                              this.Children.Add(new AccountCodingHistoryTreeViewModel(_container, md, this));
        //                          });
        //                      });
        //                  }
        //                  else if (this.AccountDocumentCodingCurrent.CertainAccountType == CertainAccountsType.UnitsChilderen)
        //                  {
        //                      var organizations = _employeeService.GetChilderen(1, this.AccountDocumentCodingCurrent.Id);
        //                      organizations.ForEach(chd =>
        //                      {
        //                          var md = new AccountCodingModel
        //                          {
        //                              AccountCode = chd.BuidldingDesignId.ToString(),
        //                              CertainAccountType = CertainAccountsType.UnitsChilderen,
        //                              Id = chd.BuidldingDesignId,
        //                              Name = chd.Name,
        //                              TotalAccountType = AccountingDescrtiption.None,
        //                          };
        //                          DispatchService.Invoke(() =>
        //                          {
        //                              this.Children.Add(new AccountCodingHistoryTreeViewModel(_container, md, this));
        //                          });
        //                      });
        //                  }
        //              }
        //          });
        //        ts.Start();
        //        await ts;
        //    }
        //}

        public bool NameContainsText(string text)
        {
            if (String.IsNullOrEmpty(text) || String.IsNullOrEmpty(this.Name))
                return false;

            return this.Name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }
        #endregion

        #region commands
        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IEmployeeService _employeeService;
        private readonly ObservableCollection<AccountCodingHistoryTreeViewModel> _children;
        private readonly AccountCodingHistoryTreeViewModel _parent;
        private readonly AccountCodingModel _codingDesign;
        static readonly AccountCodingHistoryTreeViewModel DummyChild=new AccountCodingHistoryTreeViewModel(new AccountCodingModel());

        #endregion
    }
}
