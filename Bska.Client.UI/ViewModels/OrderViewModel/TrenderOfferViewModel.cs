
namespace Bska.Client.UI.ViewModels.OrderViewModel
{
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Input;

    public sealed class TrenderOfferViewModel : BaseViewModel
    {
        #region ctor

        public TrenderOfferViewModel(IUnityContainer container,Int64 subOrderId,bool isEditable)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._orderService = _container.Resolve<IOrderService>(new ParameterOverride("repository", _unitOfWork.Repository<Order>()));
            this._personService = _container.Resolve<IPersonService>();
            this._sellerService = _container.Resolve<ISellerService>();
            IsVisible = isEditable;
            this.initializObj(subOrderId);
            this.initializCommands();
        }

        #endregion

        #region properties

        public List<Users> Suppliers
        {
            get { return GetValue(() => Suppliers); }
            set
            {
                SetValue(() => Suppliers, value);
            }
        }

        public List<SupplierTrenderOffer> TrenderCollection
        {
            get { return GetValue(() => TrenderCollection); }
            set
            {
                SetValue(() => TrenderCollection, value);
            }
        }

        public SupplierTrenderOffer SelectedTrender
        {
            get { return GetValue(() => SelectedTrender); }
            set
            {
                SetValue(() => SelectedTrender, value);
            }
        }

        public Boolean IsEditable
        {
            get { return GetValue(() => IsEditable); }
            set
            {
                SetValue(() => IsEditable, value);
            }
        }

        public Boolean IsVisible
        {
            get { return GetValue(() => IsVisible); }
            set
            {
                SetValue(() => IsVisible, value);
            }
        }

        public byte[] CurrentProForma
        {
            get { return GetValue(() => CurrentProForma); }
            set
            {
                SetValue(() => CurrentProForma, value);
            }
        }

        public List<Seller> Sellers
        {
            get { return GetValue(() => Sellers); }
            set
            {
                SetValue(() => Sellers, value);
            }
        }

        public Seller SelectedSeller
        {
            get { return GetValue(() => SelectedSeller); }
            set
            {
                SetValue(() => SelectedSeller, value);
            }
        }

        #endregion

        #region methods

        private void initializObj(long subOrderId)
        {
            Suppliers = _personService.GetUsersByPermission(Common.PermissionsType.Supplier).ToList();
            Sellers = _sellerService.Queryable().ToList();
            SelectedSeller = Sellers.FirstOrDefault();
            TrenderCollection = _orderService.GetTrenderOffersBySubOrder(subOrderId).ToList();
            IsEditable = true;
            this.checkEditable();
        }

        private void checkEditable()
        {
            if (TrenderCollection.Any(t => t.ISEnableTrender))
            {
                this.IsEditable = false;
            }
        }

        private void showProforma(object parameter)
        {
            var tr = parameter as SupplierTrenderOffer;
            if (tr == null) return;
            this.SelectedTrender = tr;
            this.CurrentProForma = tr.ProForma;
        }

        private void setCurrentSelected(object parameter)
        {
            RadioButton rb = parameter as RadioButton;
            if (rb.IsChecked == true)
            {
                var tr = rb.Tag as SupplierTrenderOffer;
                if (tr == null) return;
                this.SelectedTrender = tr;
                this._currentSelectedId = tr.Id;
            }
        }
        private void setIsCurrent()
        {
            var tr = this.TrenderCollection.SingleOrDefault(t => t.Id == _currentSelectedId);
            if (tr != null)
            {
                if (tr.ProForma == null)
                {
                    _dialogService.ShowAlert("توجه", "هیچ پیش فاکتوری هنوز برای این مناقصه ثبت نشده است");
                    return;
                }

                bool confirm = _dialogService.AskConfirmation("پرسش", "این مناقصه تکمیل شده و دیگر قابل ویرایش نیست.یک درخواست خرید نیز به کارپرداز فرستاده می شود.آیا می خواهید ادامه دهید");
                if (confirm)
                {
                    tr.ObjectState = ObjectState.Modified;
                    tr.ISEnableTrender = true;
                    Mouse.SetCursor(Cursors.Wait);
                    var subOrder = _orderService.GetSubOrder(tr.SubOrderId.Value);

                    var spI = new SupplierIndent
                    {
                        ObjectState=ObjectState.Added,
                        Num=subOrder.Num,
                        Remain=subOrder.Remain,
                        UnitId=subOrder.UnitId,
                        SupplierId=tr.SupplierId,
                        State=Common.SupplierIndentState.Ongoing,
                        SellerId=SelectedSeller?.SellerId
                    };
                    subOrder.SupplierTrenderOffers.Add(tr);
                    subOrder.SupplierIndents.Add(spI);

                    var OrderDetails = _orderService.GetOrderDetails(subOrder.OrderDetailsId.Value);
                    var order = _orderService.Find(OrderDetails.OrderId);
                    OrderDetails.SubOrders.Add(subOrder);
                    order.OrderDetails.Add(OrderDetails);

                    _orderService.InsertOrUpdateGraph(order);
                    try
                    {
                        _unitOfWork.SaveChanges();
                        _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                        this.checkEditable();
                    }
                    catch (DbUpdateException ex)
                    {
                        _dialogService.ShowError("خطا", ex.Message);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    Mouse.SetCursor(Cursors.Arrow);
                }
            }
        }
        #endregion

        #region commands

        public ICommand ProFormaCommand { get; private set; }
        public ICommand SelectCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        private void initializCommands()
        {
            ProFormaCommand = new MvvmCommand(
                (parameter) => { this.showProforma(parameter); },
                (parameter) => { return true; }
                );

            SelectCommand = new MvvmCommand(
                (parameter) => { this.setCurrentSelected(parameter); },
                (parameter) => { return true; }
                );

            SaveCommand = new MvvmCommand(
                (parameter) => { this.setIsCurrent(); },
                (parameter) => { return IsEditable; }
                ).AddListener<TrenderOfferViewModel>(this,t=>t.IsEditable);
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly IPersonService _personService;
        private readonly ISellerService _sellerService;
        private readonly IDialogService _dialogService;
        private int _currentSelectedId;

        #endregion
    }
}
