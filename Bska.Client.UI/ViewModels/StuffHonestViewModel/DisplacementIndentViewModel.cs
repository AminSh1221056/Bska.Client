
namespace Bska.Client.UI.ViewModels.StuffHonestViewModel
{
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Data.Entity;
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using System.Data.Entity.Infrastructure;
    using System.Windows;
    using Domain.Entity;
    using System.Data.Entity.Validation;

    public sealed class InternalTransferViewModel : BaseViewModel
    {
        #region ctor

        public InternalTransferViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._orderService = _container.Resolve<IOrderService>(new ParameterOverride("repository", _unitOfWork.Repository<Order>()));
            this._movableAssetService = _container.Resolve<IMovableAssetService>();
            this._employeeService = _container.Resolve<IEmployeeService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._selectedAssets = new HashSet<MovableAsset>();
            this._suborders=new List<SubOrder>();
            this.initializCommand();
        }

        #endregion

        #region properties

        public OrderDetails CurrentOrderDetails
        {
            get { return GetValue(() => CurrentOrderDetails); }
            set
            {
                SetValue(() => CurrentOrderDetails, value);
                this.initalizObj();
            }
        }

        public Double Remain
        {
            get { return GetValue(() => Remain); }
            set
            {
                SetValue(() => Remain, value);
            }
        }

        public Double CurrentNum
        {
            get { return GetValue(() => CurrentNum); }
            set
            {
                SetValue(() => CurrentNum, value);
            }
        }

        public List<MovableAsset> DisCollection
        {
            get { return GetValue(() => DisCollection); }
            set
            {
                SetValue(() => DisCollection, value);
            }
        }

        public MovableAsset DisSelected
        {
            get { return GetValue(() => DisSelected); }
            set
            {
                SetValue(() => DisSelected, value);
            }
        }

        public Double IndentNum
        {
            get { return GetValue(() => IndentNum); }
            set
            {
                SetValue(() => IndentNum, value);
            }
        }

        #endregion

        #region methods

        private async void initalizObj()
        {
           this.DisCollection=await initDisAssetAsync();
        }

        private Task<List<MovableAsset>> initDisAssetAsync()
        {
            var ts = new Task<List<MovableAsset>>(() =>
            {
                var disList = new List<MovableAsset>();
                if (CurrentOrderDetails.StuffType == StuffType.OrderConsumption)
                {
                    _orderService.Queryable().Where(o => o.OrderType == OrderType.InternalTransfer
                         && o.Status == OrderStatus.StuffHonest).SelectMany(o => o.MovableAssets).OfType<InCommidity>().ToList().ForEach(ma =>
                     {
                         if (ma.Name.Equals(CurrentOrderDetails.StuffName))
                         {
                             var loc = _movableAssetService.GetMovedRequestLocation(ma.AssetId);
                             if (loc != null)
                             {
                                 disList.Add(ma);
                             }
                         }
                     });
                }
                else if (CurrentOrderDetails.StuffType == StuffType.Belonging)
                {
                    _orderService.Queryable().Where(o => o.OrderType == OrderType.InternalTransfer
                       && o.Status == OrderStatus.StuffHonest).SelectMany(o => o.MovableAssets).OfType<Belonging>().Include(ma=>ma.ParentMAsset).ToList().ForEach(ma =>
                       {
                           if (ma.Name.Equals(CurrentOrderDetails.StuffName))
                           {
                               var loc = _movableAssetService.GetMovedRequestLocation(ma.AssetId);
                               if (loc != null)
                               {
                                   disList.Add(ma);
                               }
                           }
                       });
                }
                else
                {
                    _orderService.Queryable().Where(o => o.OrderType == OrderType.InternalTransfer
                        && o.Status == OrderStatus.StuffHonest).SelectMany(o => o.MovableAssets).OfType<UnConsumption>().Include(ma=>ma.Belongings).ToList().ForEach(ma =>
                        {
                            if (ma.Name.Equals(CurrentOrderDetails.StuffName))
                            {
                                var loc = _movableAssetService.GetMovedRequestLocation(ma.AssetId);
                                if (loc != null)
                                {
                                    disList.Add(ma);
                                }
                            }
                        });
                }
                return disList;
            });
            ts.Start();
            return ts;
        }

        private void showMAssetDetails(object parameter)
        {
            var mAsset = parameter as MovableAsset;
            if (mAsset == null) return;
            this.DisSelected = mAsset;
            Mouse.SetCursor(Cursors.Wait);
            var viewModel = new MovableAssetDetailsViewModel(_container,mAsset.AssetId);
            _navigationService.ShowMAssetDetailsWindow(viewModel);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void selectAsset(object parameter)
        {
            var ch = parameter as CheckBox;
            var mAsset = ch.Tag as MovableAsset;
            if (mAsset == null) return;
            this.DisSelected = mAsset;
            if (ch.IsChecked == true)
            {
                if (Remain<=0)
                {
                    _dialogService.ShowAlert("توجه", "شما به مقدار باقیمانده از درخواست مال تامیین کرده اید");
                    ch.IsChecked = false;
                    return;
                }

                if (!_selectedAssets.Contains(mAsset))
                {
                    _selectedAssets.Add(mAsset);
                    Remain--;
                    CurrentNum++;
                    IndentNum++;
                }
            }
            else
            {
                if (_selectedAssets.Contains(mAsset))
                {
                    _selectedAssets.Remove(mAsset);
                    Remain++;
                    CurrentNum--;
                    IndentNum--;
                }
            }
        }

        private void confirmInternalTransfer(object parameter)
        {
            if (_selectedAssets.Count <= 0)
            {
                _dialogService.ShowAlert("نوجه", "هیچ مالی انتخاب نشده است");
                return;
            }

            var window = parameter as Window;
            if (window == null) return;
            bool confirm = _dialogService.AskConfirmation("پرسش", "سفارش اموال با جابه جایی داخلی به طور مستقیم تحویل پرسنل درخواست کننده میشود.آیا میخواهید ادامه دهید");
             if (confirm)
             {
                 var order = _orderService.Query(x => x.OrderId == CurrentOrderDetails.OrderId)
                              .Include(o => o.OrderDetails).Include(p => p.Person).Select().Single();
                 var orderDetails = order.OrderDetails.Single(od => od.OrderDetialsId == CurrentOrderDetails.OrderDetialsId);
                 order.ObjectState = ObjectState.Modified;
                 orderDetails.ObjectState = ObjectState.Modified;
                 bool belongingHasError = false;
                var accountMaster = new AccountDocumentMaster
                {
                    AccountCover = "1",
                    AccountDate = GlobalClass._Today,
                    EmployeeId = UserLog.UniqueInstance.LogedEmployee.EmployeeId,
                    ObjectState = ObjectState.Added,
                };

                _selectedAssets.ForEach(ma =>
                 {
                     var loc = _movableAssetService.GetLocation(ma.AssetId, false);
                     var sBill = _movableAssetService.GetStoreBill(ma.AssetId);
                     loc.ObjectState = ObjectState.Modified;
                     loc.Status = LocationStatus.DeActive;
                     loc.ReturnDate = GlobalClass._Today;

                     AccountDocumentType acountType = AccountDocumentType.None;

                     if (sBill.AcqType != StateOwnership.Trust)
                     {
                         acountType = AccountDocumentType.UnitsToUnits;
                     }

                     var newLocation = new Location
                     {
                         InsertDate = GlobalClass._Today,
                         ObjectState = ObjectState.Added,
                         OrganizId = orderDetails.OrganizId.Value,
                         Status = LocationStatus.Active,
                         StrategyId = orderDetails.StrategyId.Value,
                         PersonId = order.Person.NationalId,
                         AccountDocumentType = acountType
                     };

                     var subOrder = new SubOrder
                     {
                         Num = 1,
                         ObjectState = ObjectState.Added,
                         State = SubOrderState.Deliviry,
                         Type = SubOrderType.Displacement,
                         UnitId = ma.UnitId,
                         Identity = ma.AssetId.ToString(),
                     };

                     if (ma is UnConsumption)
                     {
                         if (ma is Belonging)
                         {
                             var bMAsset = ma as Belonging;
                             if (bMAsset.ParentMAsset == null)
                             {
                                 bMAsset.ParentMAsset = _movableAssetService.GetBelongingParnet(bMAsset.AssetId);
                             }

                             if (!CurrentOrderDetails.BelongingParentLable.HasValue)
                             {
                                 belongingHasError = true;
                                 return;
                             }

                             var newParent = _movableAssetService.Find(CurrentOrderDetails.BelongingParentLable.Value) as UnConsumption;
                             var parentLoc = _movableAssetService.GetLocation(newParent.AssetId, false);
                             newLocation.OrganizId = parentLoc.OrganizId;
                             newLocation.StrategyId = parentLoc.StrategyId;
                             newLocation.PersonId = parentLoc.PersonId;
                             bMAsset.ParentMAsset = newParent;
                             bMAsset.ObjectState = ObjectState.Modified;
                             bMAsset.ModeifiedDate = GlobalClass._Today;
                         }
                         else
                         {
                             var unconsum = ma as UnConsumption;
                             if (unconsum.Belongings.Any())
                             {
                                 unconsum.Belongings.ForEach(b =>
                                 {
                                     var bloc = _movableAssetService.GetLocation(b.AssetId, false);
                                     bloc.ObjectState = ObjectState.Modified;
                                     bloc.Status = LocationStatus.DeActive;
                                     bloc.ReturnDate = GlobalClass._Today;
                                     var bSbill = _movableAssetService.GetStoreBill(b.AssetId);
                                     b.Locations.Add(bloc);
                                     AccountDocumentType bacountType = AccountDocumentType.None;
                                     if (bSbill.AcqType == StateOwnership.Trust)
                                     {
                                         bacountType = AccountDocumentType.EscrowToTrust;
                                     }
                                     else
                                     {
                                         bacountType = AccountDocumentType.UnitsToUnits;
                                     }

                                     var bnewLocation = new Location
                                     {
                                         InsertDate = GlobalClass._Today,
                                         ObjectState = ObjectState.Added,
                                         OrganizId = orderDetails.OrganizId.Value,
                                         Status = LocationStatus.Active,
                                         StrategyId = orderDetails.StrategyId.Value,
                                         PersonId = order.Person.NationalId,
                                         AccountDocumentType = bacountType
                                     };

                                     b.Locations.Add(bnewLocation);
                                 });
                             }

                             this.setAccountDocumentDetails(unconsum, newLocation, loc, accountMaster);
                         }
                     }

                     orderDetails.SubOrders.Add(subOrder);
                     _suborders.Add(subOrder);
                     ma.Locations.Add(loc);
                     ma.Locations.Add(newLocation);
                     order.MovableAssets.Add(ma);
                 });

                 if (belongingHasError)
                 {
                     _dialogService.ShowError("خطا", "اطلاعات در مورد درخواست اموال متعلقه اشتباه است.لطفا به مدیر سیستم مراجعه بفرمایید");
                     return;
                 }

                 if (Remain == 0)
                 {
                     if (_selectedAssets.Count == orderDetails.Num)
                     {
                         orderDetails.State = OrderDetailsState.Deliviry;
                     }
                     else
                     {
                         orderDetails.State = OrderDetailsState.SubOrder;
                     }

                     var oldHistory = _orderService.GetUserHistories(CurrentOrderDetails.OrderDetialsId)
                               .SingleOrDefault(ou => !ou.UserDecision);
                     if (oldHistory != null)
                     {
                         oldHistory.ObjectState = ObjectState.Modified;
                         oldHistory.Description = "سفارش درخواست توسط کاربر با نام:" + " " + UserLog.UniqueInstance.LogedUser.FullName + " " +
                                   "در تاریخ:" + " " + GlobalClass._Today.PersianDateString();
                         oldHistory.Identity = "Indent";
                         oldHistory.UserDecision = true;
                         orderDetails.OrderUserHistories.Add(oldHistory);
                     }
                 }

                 if (order.OrderDetails.Where(od => !od.IsReject).All(od => od.State == OrderDetailsState.SubOrder))
                 {
                     order.Status = OrderStatus.SubOrder;
                 }
                 else if (order.OrderDetails.Where(od => !od.IsReject).All(od => od.State == OrderDetailsState.Deliviry))
                 {
                     order.Status = OrderStatus.Deliviry;
                     order.DueDate = GlobalClass._Today;
                 }

                 _orderService.InsertOrUpdateGraph(order);
                 try
                 {
                     Mouse.SetCursor(Cursors.Wait);
                     _unitOfWork.SaveChanges();
                     _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                     Mouse.SetCursor(Cursors.Arrow);
                     window.DialogResult = true;
                 }
                 catch (DbUpdateException ex)
                 {
                     _dialogService.ShowError("خطا", ex.Message);
                 }
                catch (Exception)
                 {
                     throw;
                 }
             }
        }

        private void setAccountDocumentDetails(UnConsumption asset, Location loc,Location oldLoc, AccountDocumentMaster accountMaster)
        {
            var ma = asset;
            if (ma != null && loc != null)
            {
                if (loc.AccountDocumentType == AccountDocumentType.UnitsToUnits)
                {
                    List<Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>> currentAccountCodings = new List<Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>>();
                    var accountCodings = _employeeService.GetAccountCodings();
                    var parentcode = accountCodings.FirstOrDefault(x => x.Parent == null);

                    string desc = "نامشخص";
                    string code = "0";
                    if (loc.AccountDocumentType == AccountDocumentType.UnitsToUnits)
                    {
                        var organization = _employeeService.GetParentNode(1).FirstOrDefault(x => x.BuidldingDesignId == oldLoc.OrganizId);
                        if (organization != null)
                        {
                            var getItem = this.GetHirecharyNode(organization);
                            desc = getItem.Item1;
                            code = getItem.Item2;
                        }
                        currentAccountCodings.Add(
                           new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.UnitsDeliviry),
                           "Creditor", desc, code, ma.Cost, ma));

                        var organization1 = _employeeService.GetParentNode(1).FirstOrDefault(x => x.BuidldingDesignId == loc.OrganizId);
                        if (organization1 != null)
                        {
                            var getItem = this.GetHirecharyNode(organization1);
                            desc = getItem.Item1;
                            code = getItem.Item2;
                        }
                        currentAccountCodings.Add(
                           new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.UnitsDeliviry),
                           "Debtor", desc, code, ma.Cost, ma));
                    }

                    string acNo = "";
                    decimal deboter = 0;
                    decimal creditor = 0;
                    string vdesc = "";
                    currentAccountCodings.ForEach(adc =>
                    {
                        acNo = parentcode.AccountCode + "-" + adc.Item1.Parent.AccountCode + "-" + adc.Item1.AccountCode + "-" +GlobalClass.CheckAccountCode(adc.Item4);
                        
                        if (string.Equals(adc.Item2, "Creditor"))
                        {
                            creditor = adc.Item5;
                            deboter = 0;
                            vdesc = "<<" + adc.Item3 + ">>";
                        }
                        else if (string.Equals(adc.Item2, "Debtor"))
                        {
                            deboter = adc.Item5;
                            creditor = 0;
                            vdesc = "((" + adc.Item3 + "))";
                        }

                        var newDetails = new AccountDocumentDetails
                        {
                            Debtor = deboter,
                            Creditor = creditor,
                            Description = vdesc,
                            AccountNo = acNo,
                            ObjectState = ObjectState.Added,
                            AccountDocumentMaster = accountMaster,
                        };
                        adc.Item6.AccountDocumentDetails.Add(newDetails);
                    });
                }
            }
        }

        private Tuple<string, string> GetHirecharyNode(EmployeeDesign item)
        {
            String _nodeName = "";
            string _coding = "";
            if (item.ParentNode != null)
            {
                var getItem = this.GetHirecharyNode(item.ParentNode);
                _nodeName += getItem.Item1;
                _coding += getItem.Item2;
                _nodeName += "**";
            }
            _coding += item.BuidldingDesignId.ToString();
            _nodeName += item.Name;

            return new Tuple<string, string>(_nodeName, _coding);
        }

        #endregion

        #region commands

        public ICommand DetailsCommand { get; private set; }
        public ICommand SelectCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }
        private void initializCommand()
        {
            DetailsCommand = new MvvmCommand(
                (parameter) => { this.showMAssetDetails(parameter); },
                (parameter) => { return true; }
                );

            SelectCommand = new MvvmCommand(
                (parameter) => { this.selectAsset(parameter); },
                (parameter) => { return true; }
                );

            ConfirmCommand = new MvvmCommand(
                (parameter) => { this.confirmInternalTransfer(parameter); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IOrderService _orderService;
        private readonly IEmployeeService _employeeService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly HashSet<MovableAsset> _selectedAssets;
        private readonly IUnitOfWorkAsync _unitOfWork;
        internal List<SubOrder> _suborders;
        #endregion
    }
}
