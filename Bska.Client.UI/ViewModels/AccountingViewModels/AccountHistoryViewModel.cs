
namespace Bska.Client.UI.ViewModels
{
    using API;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.Helper;
    using Microsoft.Practices.Unity;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using System.Threading.Tasks;
    using Domain.Entity;
    using Common;

    public sealed class AccountHistoryViewModel : BaseViewModel
    {
        #region ctor

        public AccountHistoryViewModel(IUnityContainer container,PortableAsset currentAsset)
        {
            this._container = container;
            this._movableAssetService = _container.Resolve<IMovableAssetService>();
            this._commodityService = _container.Resolve<IMAssetCommodityService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._employeeService = _container.Resolve<IEmployeeService>();
            this.CurrentAsset = currentAsset;
            this.initializObj();
            this.initalizCommand();
        }

        #endregion

        #region properties

        public PortableAsset CurrentAsset
        {
            get { return GetValue(() => CurrentAsset); }
            private set
            {
                SetValue(() => CurrentAsset, value);
            }
        }

        public String AccountType
        {
            get { return GetValue(() => AccountType); }
            set
            {
                SetValue(() => AccountType, value);
                this.initLists();
            }
        }

        public List<Document> Documents
        {
            get { return GetValue(() => Documents); }
            set
            {
                SetValue(() => Documents, value);
            }
        }

        public AccountDocumentMaster CurrentDocument
        {
            get { return GetValue(() => CurrentDocument); }
            set
            {
                SetValue(() => CurrentDocument, value);
            }
        }

        public List<AccountDocumentDetailsModel> AccountDocuments
        {
            get { return GetValue(() => AccountDocuments); }
            set
            {
                SetValue(() => AccountDocuments, value);
            }
        }

        public AccountDocumentDetailsModel CurrentDetailsAccount
        {
            get { return GetValue(() => CurrentDetailsAccount); }
            set
            {
                SetValue(() => CurrentDetailsAccount, value);
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
        #endregion

        #region methods

        private void initializObj()
        {
            AccountType = "A00A";
        }

        private async void initLists()
        {
            if (string.IsNullOrEmpty(AccountType)) return;
            if (AccountType == "A00A")
            {
                if(CurrentAsset is Commodity)
                {
                    Documents = _commodityService.GetDocuments(CurrentAsset.AssetId).ToList();
                }
                else
                {
                    Documents = ((MovableAsset)CurrentAsset).Documetns.ToList();
                }
            }
            else
            {
                if (CurrentAsset is UnConsumption)
                {
                    CurrentDocument = _movableAssetService.GetLastAccountDocuemntForAsset(CurrentAsset.AssetId);
                    if (CurrentDocument != null)
                    {
                        var item = await this.setAccountDetailsAsync();
                        this.AccountDocuments = item.Item1;
                        DebtorDescription = $"جمع بدهکار : {item.Item2.ToString("N0")}";
                        CreditorDescription = $"جمع بستانکار : {item.Item3.ToString("N0")}";
                        CurrentDetailsAccount = null;
                    }
                }
            }
        }

        private Task<Tuple<List<AccountDocumentDetailsModel>, decimal, decimal>> setAccountDetailsAsync()
        {
            var ts = new Task<Tuple<List<AccountDocumentDetailsModel>, decimal, decimal>>(() =>
              {
                  var lsitem =new List<AccountDocumentDetailsModel>();
                  var coings = _employeeService.GetAccountCodings().ToList();
                  decimal debtor = 0;
                  decimal creditor = 0;
                  CurrentDocument.AccountDocumentDetails.ForEach(acd =>
                  {
                      string[] splitCoding = acd.AccountNo.Split('-');
                      string code =splitCoding[2];
                      string totalCode= splitCoding[1];
                      var cod = coings.FirstOrDefault(x => x.AccountCode == code);
                      var totalCod = coings.FirstOrDefault(x => x.AccountCode == totalCode);
                      var item = new AccountDocumentDetailsModel
                      {
                          AccountNo=acd.AccountNo,
                          AssetId=acd.AssetId,
                          Creditor=acd.Creditor,
                          Debtor=acd.Debtor,
                          ID=acd.ID,
                          MasterId=acd.MasterId,
                          Description=acd.Description
                      };
                      debtor += acd.Debtor;
                      creditor += acd.Creditor;
                      if (acd.AssetId == CurrentAsset.AssetId)
                      {
                          item.IsCurent = true;
                      }

                      if (cod != null)
                      {
                          item.TotalAccount = totalCod.Name;
                      }
                      else
                      {
                          item.TotalAccount = "ناشناخته";
                      }
                      lsitem.Add(item);
                  });
                 
                  return new Tuple<List<AccountDocumentDetailsModel>, decimal, decimal>(lsitem,debtor,creditor);
              });
            ts.Start();
            return ts;
        }
        
        private void reports()
        {
            Mouse.SetCursor(Cursors.Wait);

            var viewModel = new ReportViewModel();
            if (CurrentAsset is MovableAsset)
            {
                var asset = CurrentAsset as MovableAsset;
                if (string.Equals(AccountType, "A00A", StringComparison.InvariantCulture))
                {
                    viewModel.mAssetItemsReports(asset.Name, asset.GetType().Name, asset.AssetId, asset.Label, 4);
                }
                else
                {
                    viewModel.AccountDocMainGroupedReport(CurrentDocument.ID, CurrentDocument.AccountDate.PersianDateString());
                }
            }
            else
            {

            }
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region commands
        public ICommand ReportCommand { get; private set; }

        private void initalizCommand()
        {
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
        private readonly IMovableAssetService _movableAssetService;
        private readonly IMAssetCommodityService _commodityService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IEmployeeService _employeeService;

        #endregion
    }
}
