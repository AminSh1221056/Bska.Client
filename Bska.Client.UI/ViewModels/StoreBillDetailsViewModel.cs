
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Domain.Entity.AssetEntity;
    using Data.Service;
    using Microsoft.Practices.Unity;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Bska.Client.UI.Helper;
    using System.Collections.Generic;
    using System.Windows.Input;
    using Services;
    using API;

    public sealed class StoreBillDetailsViewModel : BaseViewModel
    {
        #region ctor

        public StoreBillDetailsViewModel(IUnityContainer container,StoreBill currentBill,bool isEditableOrder)
        {
            this._container = container;
            this.CurrentBill = currentBill;
            this._sellerService = _container.Resolve<ISellerService>();
            this._storeBillService = _container.Resolve<IStoreBillService>();
            this._personService = _container.Resolve<IPersonService>();
            this._storeService = _container.Resolve<IStoreService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this.IsEditableOrder = isEditableOrder;
            this.initializObj();
            this.initializCommands();
        }

        #endregion

        #region properties

        public StoreBill CurrentBill
        {
            get { return GetValue(() => CurrentBill); }
            set
            {
                SetValue(() => CurrentBill, value);
            }
        }

        public StoreBillViewModel StoreBillVM
        {
            get;
            private set;
        }

        public StoreBillEditViewModel StoreBillEditVm
        {
            get;
            private set;
        }
        public  Boolean IsEditableOrder
        {
            get { return GetValue(() => IsEditableOrder); }
            set
            {
                SetValue(() => IsEditableOrder, value);
            }
        }

        public string SupplierName
        {
            get { return GetValue(() => SupplierName); }
            set
            {
                SetValue(() => SupplierName, value);
            }
        }
        #endregion

        #region methods

        private void initializObj()
        {
            StoreBillVM = new StoreBillViewModel(CurrentBill) { AcqTyp=CurrentBill.AcqType};
            StoreBillEditVm = new StoreBillEditViewModel(_container, CurrentBill.StoreBillId);
            if (CurrentBill.AcqType == Common.StateOwnership.Purchase)
            {
                var seller = _sellerService.Find(CurrentBill.SellerId);
                StoreBillVM.Sellers = new List<Domain.Entity.Seller> { seller};
                StoreBillVM.SelectedSeller = StoreBillVM.Sellers.FirstOrDefault();
            }

            var indent = _storeBillService.Queryable()
                    .Where(sb => sb.StoreBillId == CurrentBill.StoreBillId)
                    .SelectMany(x => x.SupplierIndents).FirstOrDefault();

            if (indent!=null)
            {
                var supplier = _personService.GetUser(indent.SupplierId);
                SupplierName = supplier.FullName;
            }
            else
            {
                SupplierName = "نامشخص";
            }
        }

        private async void getSellers()
        {
            await Task.Run(() =>
            {
                var sellers = _sellerService.Queryable().ToList();
                DispatchService.Invoke(() =>
                {
                   
                });
            });
        }

        private void report()
        {
            Mouse.SetCursor(Cursors.Wait);
            string searchItem ="";
            var viewModel = new ReportViewModel();
            var bill = CurrentBill;
            string spName = SupplierName;
            string desc2 = "";
            string desc3 = "امین اموال : نامشخص";
            string desc4 = "انباردار : نامشخس";
            string desc5 = "ذی حساب : نامشخص";
            
            if (bill.AcqType == Common.StateOwnership.Purchase)
            {
                var seller = _sellerService.Find(bill.SellerId);
                if (seller != null)
                {
                    desc2 = $"فروشنده : {seller.Name} {seller.Lastname}";
                }
            }
            var stuffHUser = _personService.GetUniqueUserToPersonByPermission(Common.PermissionsType.StuffHonest);
            if (stuffHUser != null)
            {
                desc3 = $"امین اموال : {stuffHUser.FullName}";
            }

            if (bill.StoreId.HasValue)
            {
                var storeLeader = _storeService.GetUserForStore(bill.StoreId.Value);
                if (storeLeader != null)
                {
                    desc4 = $"انباردار:{storeLeader.FullName}";
                }
            }

            var accounting = _personService.GetUniqueUserToPersonByPermission(Common.PermissionsType.Accountant);
            if (accounting != null)
            {
                desc5 = $"ذی حساب:{accounting.FullName}";
            }
            int masterId = _storeBillService.getRelatedAccountMasterId(bill.StoreBillId);
            viewModel.StoreBillReport(bill.StoreBillNo, searchItem, spName, desc2,masterId, desc3, desc4, desc5);
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);
        }
        #endregion

        #region commands

        public ICommand ReportCommand
        {
            get;private set;
        }

        private void initializCommands()
        {
            ReportCommand = new MvvmCommand(
                (parameter) => { this.report(); },
                (parameter) => { return true; }
                );
        }
        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly ISellerService _sellerService;
        private readonly IPersonService _personService;
        private readonly IStoreService _storeService;
        private readonly IStoreBillService _storeBillService;
        private readonly INavigationService _navigationService;

        #endregion

    }
}
