
namespace Bska.Client.UI.ViewModels.MunitionViewModel
{
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Data.Entity.Infrastructure;
    using System.Windows.Input;

    public sealed class SupplierProFormaUploadViewModel : BaseDetailsViewModel<SupplierTrenderOffer>
    {
        #region ctor

        public SupplierProFormaUploadViewModel(IUnityContainer container, SupplierTrenderOffer currentEntity
            ,bool isConfirmed)
             : base(currentEntity)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._orderService = _container.Resolve<IOrderService>(new ParameterOverride("repository",_unitOfWork.Repository<Order>()));
            this.IsConfirmed = isConfirmed;
            this.initializCommands();
        }

        #endregion

        #region properties

        public byte[] ProForma
        {
            get { return CurrentEntity.ProForma; }
            set
            {
                CurrentEntity.ProForma = value;
                OnPropertyChanged("ProForma");
            }
        }

        public Boolean IsConfirmed
        {
            get { return GetValue(() => IsConfirmed); }
            set
            {
                SetValue(() => IsConfirmed, value);
            }
        }

        #endregion

        #region methods

        private void saveUploaded()
        {
            if (CurrentEntity != null)
            {
                bool confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
                if (confirm)
                {
                    Mouse.SetCursor(Cursors.Wait);
                    var subOrder = _orderService.GetSubOrder(this.CurrentEntity.SubOrderId.Value);
                    var OrderDetails = _orderService.GetOrderDetails(subOrder.OrderDetailsId.Value);
                    var order = _orderService.Find(OrderDetails.OrderId);
                    this.CurrentEntity.ObjectState = Client.API.Infrastructure.ObjectState.Modified;
                    subOrder.SupplierTrenderOffers.Add(this.CurrentEntity);
                    OrderDetails.SubOrders.Add(subOrder);
                    order.OrderDetails.Add(OrderDetails);

                    _orderService.InsertOrUpdateGraph(order);
                    try
                    {
                        _unitOfWork.SaveChanges();
                        _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
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

        public ICommand SaveCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }
        public ICommand ReportCommand { get; private set; }

        private void initializCommands()
        {
            SaveCommand = new MvvmCommand(
                (parameter) => { this.saveUploaded(); },
                (parameter) => { return !IsConfirmed; }
                ).AddListener<SupplierProFormaUploadViewModel>(this,su=>su.IsConfirmed);

            ReportCommand = new MvvmCommand(
                (parameter) => { },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDialogService _dialogService;
        private readonly IOrderService _orderService;

        #endregion

    }
}
