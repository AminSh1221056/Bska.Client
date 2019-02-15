
namespace Bska.Client.UI.ViewModels.StuffHonestViewModel
{
    using Bska.Client.Common;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.UI.Helper;
    using Bska.Client.Repository.Model;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Bska.Client.API.Infrastructure;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using System.Collections.ObjectModel;
    using System.Windows.Controls;
    using System.ComponentModel;
    using System.Windows.Data;
    using System.Data.Entity.Infrastructure;
    using System.Windows;

    public sealed class AddProceedingViewModel : BaseViewModel
    {
        #region ctor

        public AddProceedingViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._proceedingService = _container.Resolve<IProceedingService>(new ParameterOverride("repository", _unitOfWork.Repository<Proceeding>()));
            this._storeService = _container.Resolve<IStoreService>();
            this._orderService = _container.Resolve<IOrderService>(new ParameterOverride("repository", _unitOfWork.Repository<Order>()));
            this._movableAssetService = _container.Resolve<IMovableAssetService>(new ParameterOverride("repository", _unitOfWork.Repository<MovableAsset>()));
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationServic = _container.Resolve<INavigationService>();
            this._personService = _container.Resolve<IPersonService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._collection = new ObservableCollection<ProceedingAssetModel>();
            this._selectedItems = new HashSet<ProceedingAssetModel>();
            this.AssetProceedingView = new CollectionViewSource { Source = Collection }.View;
            this._seedDataHelper = new SeedDataHelper();
            this.initializObj();
            this.initializCommands();
        }

        #endregion

        #region properties

        public Boolean IsOrderMAsset
        {
            get { return GetValue(() => IsOrderMAsset); }
            set
            {
                SetValue(() => IsOrderMAsset, value);
            }
        }

        public Boolean IsStoreMAsset
        {
            get { return GetValue(() => IsStoreMAsset); }
            set
            {
                SetValue(() => IsStoreMAsset, value);
            }
        }

        public Boolean IsRetiringMAsset
        {
            get { return GetValue(() => IsRetiringMAsset); }
            set
            {
                SetValue(() => IsRetiringMAsset, value);
            }
        }

        public Boolean IsAccidentMAsset
        {
            get { return GetValue(() => IsAccidentMAsset); }
            set
            {
                SetValue(() => IsAccidentMAsset, value);
            }
        }

        public Boolean RefundInStore
        {
            get { return GetValue(() => RefundInStore); }
            set
            {
                SetValue(() => RefundInStore, value);
            }
        }

        public Boolean RefundOutStore
        {
            get { return GetValue(() => RefundOutStore); }
            set
            {
                SetValue(() => RefundOutStore, value);
            }
        }

        public ProceedingDetailsViewModel ProceedingDetailsVM
        {
            get { return GetValue(() => ProceedingDetailsVM); }
            private set
            {
                SetValue(() => ProceedingDetailsVM, value);
            }
        }

        public ProceedingsType ProceedingType
        {
            get { return GetValue(() => ProceedingType); }
            set
            {
                SetValue(() => ProceedingType, value);
                this.initProcDetails();
            }
        }

        public Int32 Counter
        {
            get { return GetValue(() => Counter); }
            set
            {
                SetValue(() => Counter, value);
            }
        }

        public ObservableCollection<ProceedingAssetModel> Collection
        {
            get { return _collection; }
        }

        public ProceedingAssetModel Selected
        {
            get { return GetValue(() => Selected); }
            set
            {
                SetValue(() => Selected,value);
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

        public ICollectionView AssetProceedingView { get; set; }

        public String SearchCriteria
        {
            get { return GetValue(() => SearchCriteria); }
            set
            {
                SetValue(() => SearchCriteria, value);
                this.searchigItems(1002);
            }
        }

        public List<OrganizationModel> OrganNames
        {
            get { return GetValue(() => OrganNames); }
            set
            {
                SetValue(() => OrganNames, value);
            }
        }
        
        public string SelectedOrgan
        {
            get { return GetValue(() => SelectedOrgan); }
            set
            {
                SetValue(() => SelectedOrgan, value);
                this.searchigItems(1001);
            }
        }

        public Decimal GlobalPrice
        {
            get { return GetValue(() => GlobalPrice); }
            set
            {
                SetValue(() => GlobalPrice, value);
            }
        }


        #endregion

        #region methods

        private void initializObj()
        {
            Units = _unitService.Queryable().ToList();
        }

        private void initProcDetails()
        {
            _collection.Clear();
            _selectedItems.Clear();
            Counter = 0;

            if (ProceedingType == ProceedingsType.AssetRetiring)
            {
                ProceedingDetailsVM = new ProceedingDetailsViewModel(new Proceeding())
                {
                    Stores= _storeService.Queryable().Where(s => s.StoreType == StoreType.Retiring).ToList()
                };
            }
            else if (ProceedingType == ProceedingsType.DefinitiveTransfer
                || ProceedingType == ProceedingsType.StateTransfer)
            {
                ProceedingDetailsVM = new ProceedingDetailsViewModel(new Proceeding());
            }
            else if (ProceedingType == ProceedingsType.TrustTransfer)
            {
                ProceedingDetailsVM = new ProceedingTrustTansferDetailsViewModel(new Proceeding())
                {
                     Date1 = PersianDate.Today, Date2 = new PersianDate(PersianDate.Today.Year+1, 12, 29)
                };
            }
            else
            {
                ProceedingDetailsVM = new ProceedingDetailsViewModel(new Proceeding());
            }

            ProceedingDetailsVM.Desc1 = ProceedingDetailsVM.Desc2 = ProceedingDetailsVM.Desc3 = ProceedingDetailsVM.Desc4
                = ProceedingDetailsVM.Desc5 = ProceedingDetailsVM.Desc6 = "";

            if (OrganNames != null)
            {
                OrganNames.Clear();
            }
        }

        private void searchigItems(int searchType)
        {
            if (searchType == 1001)
            {
                if (string.IsNullOrEmpty(SelectedOrgan))
                {
                    return;
                }

                if (ProceedingType == ProceedingsType.ReturnFromTrust
                    || ProceedingType == ProceedingsType.RefundTrust)
                {
                    AssetProceedingView.Filter = (obj) =>
                    {
                        var item = obj as ProceedingAssetModel;
                        return item.RecipetNo == SelectedOrgan;
                    };
                }

                this.ProceedingDetailsVM.Desc1 =SelectedOrgan;
            }
            else
                AssetProceedingView.Filter = (obj) =>
                {
                    var item = obj as ProceedingAssetModel;
                    return item.Label.ToString() == SearchCriteria
                        || item.Name.Contains(SearchCriteria);
                };
        }

        private void SearchAvailableAssetsForProceeding()
        {
            Mouse.SetCursor(Cursors.Wait);
            _collection.Clear();
            OrderType otype = OrderType.Procceding;
            bool checkCompietion = true;
            bool checkDivanEnable = false;
            if (ProceedingType == ProceedingsType.Fire || ProceedingType == ProceedingsType.Flood
            || ProceedingType == ProceedingsType.Earthquake
            || ProceedingType == ProceedingsType.Accident
            || ProceedingType == ProceedingsType.Theft)
            {
                checkCompietion = false;
            }
            else if (ProceedingType == ProceedingsType.AssetRetiring
                || ProceedingType==ProceedingsType.ReturnFromRetiring)
            {
                checkCompietion = false;
            }
            else if(ProceedingType==ProceedingsType.Delete
                ||ProceedingType==ProceedingsType.SpecialLicencing
                || ProceedingType == ProceedingsType.BudgetLicencing)
            {
                if(APPSettings.Default.AccidentProccedingConfirm !=2003)
                    checkDivanEnable = true;
            }
            else if (ProceedingType == ProceedingsType.DefinitiveTransfer 
                || ProceedingType==ProceedingsType.StateTransfer
                || ProceedingType==ProceedingsType.TrustTransfer)
            {
                OrganNames = _seedDataHelper.GetOrganizations().ToList();
            }

            List<MovableAsset> searchingItems = new List<MovableAsset>();
            if (IsOrderMAsset)
            {
                _orderService.GetOrderdAssetsForProceeding(otype, checkCompietion,ProceedingType).ForEach(ma =>
                {
                    if (ma.AssetProceedings.Count > 0)
                    {
                        if (ma.AssetProceedings.All(ap => ap.State != AssetProceedingState.InProgress))
                        {
                            searchingItems.Add(ma);
                        }
                    }
                    else
                    {
                        searchingItems.Add(ma);
                    }
                });
            }

            if (IsStoreMAsset)
            {
                _movableAssetService.GetStoreMovableAssetForProcceding(checkCompietion).Where(x=>x.ISConfirmed).ToList()
                    .ForEach(ma =>
                    {
                        if (ma is Belonging)
                        {
                            var belong = ma as Belonging;
                            if (belong.ParentMAsset == null)
                            {
                                belong.ParentMAsset = _movableAssetService.GetBelongingParnet(belong.AssetId);
                                if (belong.ParentMAsset == null)
                                {
                                    searchingItems.Add(belong);
                                }
                            }
                        }
                        else if (ma is UnConsumption)
                        {
                            searchingItems.Add(ma);
                        }
                    });
            }

            if (IsRetiringMAsset)
            {
                _movableAssetService.GetUnConsuptionAssetToLabelInRetiringStore(true,checkCompietion).ForEach(ma =>
                {
                   searchingItems.Add(ma);
                });
            }

            if (IsAccidentMAsset)
            {
                _movableAssetService.GetUnConsuptionAssetToAccident(true).ForEach(ma =>
                {
                    searchingItems.Add(ma);
                });
            }

            if (ProceedingType==ProceedingsType.ReturnFromTrust)
            {
                var organName = new HashSet<string>();
                SelectedOrgan = null;
                _proceedingService.Queryable().Where(x => x.Type == ProceedingsType.TrustTransfer && x.State == ProceedingState.CompletedConfirm).Select(p => p.Desc1).ToList().ForEach(p =>
                {
                    if (!organName.Contains(p)) organName.Add(p);
                });
                OrganNames = _seedDataHelper.GetOrganizations().Where(x => organName.Contains(x.BudgetNo.ToString())).ToList();
                _movableAssetService.GetTrustAssetToReturntFromTrust().ForEach(ma =>
                {
                    var proc = ma.AssetProceedings.Select(x => x.Proceeding).FirstOrDefault();
                    if (proc != null)
                    {
                        ma.StoreBill.Desc1 = proc.Desc1;
                    }
                    searchingItems.Add(ma);
                });
            }

            if (ProceedingType == ProceedingsType.RefundTrust)
            {
                var organName = new HashSet<string>();
                SelectedOrgan = null;
                if (RefundOutStore)
                {
                    _movableAssetService.getMovableAssetToRefundTrust().ForEach(ma =>
                    {
                        if (ma.StoreBill != null)
                        {
                            if (!organName.Contains(ma.StoreBill.Desc1.Trim()))
                            organName.Add(ma.StoreBill.Desc1.Trim());
                        }
                        searchingItems.Add(ma);
                    });
                }

                if (RefundInStore)
                {
                    _movableAssetService.getMovableAssetToRefundTrust(true).ForEach(ma =>
                    {
                        if (ma.StoreBill != null)
                        {
                            if (!organName.Contains(ma.StoreBill.Desc1.Trim()))
                                organName.Add(ma.StoreBill.Desc1.Trim());
                        }
                        searchingItems.Add(ma);
                    });
                }

                OrganNames = _seedDataHelper.GetOrganizations().Where(x => organName.Contains(x.BudgetNo.ToString())).ToList();
            }

            Boolean addMa = true;
            Boolean divanEnable = false;
            searchingItems.ForEach(ma =>
            {
                addMa = false;
                if (ma is Belonging)
                {
                    var b=(Belonging)ma;
                    if (b.ParentMAsset==null)
                    {
                        b.ParentMAsset = _movableAssetService.GetBelongingParnet(b.AssetId);
                    }

                    if (b.ParentMAsset == null)
                    {
                        addMa = true;
                    }
                    else
                    {
                        addMa = true;
                    }
                }
                else
                {
                    addMa = true;
                }

                if (checkDivanEnable)
                {
                    var loc = _movableAssetService.GetLastLocation(ma.AssetId);
                    if (loc.Status != LocationStatus.Retiring)
                    {
                        divanEnable = true;
                    }
                    else
                    {
                        divanEnable =false;
                    }
                }

                if (addMa)
                {
                    _collection.Add(new ProceedingAssetModel
                    {
                        AssetId = ma.AssetId,
                        IsEditableLicense = false,
                        IsEditablePrice = true,
                        IsEditableDivan=divanEnable,
                        AccidentDivanNo =null,
                        IsOrganFault = false,
                        Label = ma.Label??ma.OrganLabel,
                        LicenseNumber =null,
                        Name = ma.Name,
                        Num = ma.Num,
                        Price = ma.Cost,
                        RecipetNo = ma.StoreBill != null ? ma.StoreBill.Desc1 : null,
                        State = AssetProceedingState.InProgress,
                        UnitId = ma.UnitId,
                        IsSelected = false,
                        IsSelectable = true
                    });
                }
            });

            Mouse.SetCursor(Cursors.Arrow);
        }

        private void selectItems(object item)
        {
            var ch = item as CheckBox;
            var selectingItem = ch.Tag as ProceedingAssetModel;
            if (selectingItem != null)
            {
                if (ch.IsChecked == true)
                {
                    if (selectingItem.Price <= 0)
                    {
                        _dialogService.ShowAlert("توجه", "قیمت مال صحیح نیست");
                        ch.IsChecked = false;
                        return;
                    }

                    if (ProceedingType == ProceedingsType.RefundTrust || ProceedingType==ProceedingsType.ReturnFromTrust)
                    {
                        if (!string.IsNullOrEmpty(_currentRefundTrustOrgan))
                        {
                            if (!string.Equals(selectingItem.RecipetNo, _currentRefundTrustOrgan, StringComparison.CurrentCulture))
                            {
                                _dialogService.ShowAlert("توجه", "سازمان امانت دهنده این مال با سازمان امانت دهنده اموال اتنخاب شده متفاوت است");
                                ch.IsChecked = false;
                                return;
                            }
                        }
                        else
                        {
                            _currentRefundTrustOrgan = selectingItem.RecipetNo;
                        }
                    }

                    if (selectingItem.IsEditableDivan)
                    {
                        if (string.IsNullOrWhiteSpace(selectingItem.AccidentDivanNo))
                        {
                            _dialogService.ShowAlert("توجه", "شماره رای دیوان عدالت اداری الزامی است");
                            ch.IsChecked = false;
                            return;
                        }
                    }

                    if (selectingItem.IsOrganFault)
                    {
                        if (string.IsNullOrWhiteSpace(selectingItem.RecipetNo))
                        {
                            _dialogService.ShowAlert("توجه", "شماره فیش واریزی با توجه به مقصر بودن الزامی است");
                            ch.IsChecked = false;
                            return;
                        }
                    }

                    Boolean hasBelonging = _movableAssetService.ContainBelongings(selectingItem.AssetId);
                    if (hasBelonging)
                    {
                        bool confirm = _dialogService.AskConfirmation("پرسش", "این مال دارای اموال متعلقه است.اموال متعلقه نیز شامل این صورت جلسه خواهند شد .آیا می خواهید ادامه دهید");
                        if (!confirm)
                        {
                            ch.IsChecked = false;
                            return;
                        }
                    }

                    if (!_selectedItems.Contains(selectingItem))
                    {
                        _selectedItems.Add(selectingItem);
                        selectingItem.IsSelected = true;
                    }
                }
                else
                {
                    if (_selectedItems.Contains(selectingItem))
                    {
                        _selectedItems.Remove(selectingItem);
                        selectingItem.IsSelected = false;
                    }
                }
            }

            Counter = _selectedItems.Count;
            if (Counter == 0)
            {
                _currentRefundTrustOrgan = null;
            }
        }

        private void addProceeding()
        {
            if (_selectedItems.Count <= 0)
            {
                _dialogService.ShowError("خطا", ErrorMessages.Default.NoRowSelected);
                return;
            }

            if (detailsHaveError())
            {
                _dialogService.ShowError("خطا", ErrorMessages.Default.InputInvalid);
                return;
            }
            decimal? salePrice = null;
            var entity = ProceedingDetailsVM.CurrentEntity;
            var proceeding = new Proceeding
            {
                Desc1=entity.Desc1,
                Desc2=entity.Desc2,
                Desc3=entity.Desc3,
                Desc4=entity.Desc4,
                Desc5=entity.Desc5,
                Desc6=entity.Desc6,
            };

            if (ProceedingType == ProceedingsType.TrustTransfer)
            {
                var vmtr = ProceedingDetailsVM as ProceedingTrustTansferDetailsViewModel;

                if (vmtr.Date1 > vmtr.Date2)
                {
                    _dialogService.ShowAlert("توجه", "تاریخ امانت دادن نمیتواند بزرگتر از تاریخ برگشت از امانی باشد");
                    return;
                }

                if (vmtr.Date2 > new PersianDate(PersianDate.Today.Year+1,12,29))
                {
                    _dialogService.ShowAlert("توجه", "نهایت تاریخ امانت دادن اسفند سال بعد می باشد");
                    return;
                }
            }
            else if (ProceedingType == ProceedingsType.Sale)
            {
                if (GlobalPrice > 0)
                {
                    salePrice = GlobalPrice / _selectedItems.Count;
                }
            }
            else if (ProceedingType == ProceedingsType.RefundTrust || ProceedingType == ProceedingsType.ReturnFromTrust)
            {
                proceeding.Desc1 = SelectedOrgan;
            }

            Boolean confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                Mouse.SetCursor(Cursors.Wait);
                proceeding.Type = ProceedingType;
                proceeding.ObjectState = ObjectState.Added;
                proceeding.State = ProceedingState.ManagerConfirming;
                proceeding.ProceedingDate = GlobalClass._Today;
                proceeding.ProcIdentity = Guid.NewGuid();
                proceeding.Description = "نام سازمان: " + UserLog.UniqueInstance.LogedEmployee.Name + "*" + "ثبت صورت جلسه توسط امین اموال سازمان مذکور به نام " + UserLog.UniqueInstance.LogedUser.FullName
                    + " " + "در تاریخ " + GlobalClass._Today.PersianDateString();
                _selectedItems.ForEach(apm =>
                {
                    var newAp = new AssetProceeding
                    {
                        AccidentDivanNo=apm.AccidentDivanNo,
                        IsOrganFault=apm.IsOrganFault,
                        LicenseNumber=null,
                        ObjectState=ObjectState.Added,
                        Price=salePrice??apm.Price,
                        State=AssetProceedingState.InProgress,
                        RecipetNo=apm.RecipetNo,
                        AssetId=apm.AssetId,
                    };

                    if (ProceedingType == ProceedingsType.EditRequest)
                    {
                        newAp.TempYear = apm.TempYear;
                        newAp.TempDesc1 = apm.TempDesc1;
                        newAp.TempDesc2 = apm.TempDesc2;
                        newAp.TempDesc3 = apm.TempDesc3;
                        newAp.TempDesc4 = apm.TempDesc4;
                        newAp.TempUid1 = apm.TempUid1;
                        newAp.TempUid2 = apm.TempUid2;
                        newAp.TempUid3 = apm.TempUid3;
                        newAp.TempUid4 = apm.TempUid4;
                    }
                    proceeding.AssetProceedings.Add(newAp);
                    _collection.Remove(apm);
                });
                _proceedingService.InsertOrUpdateGraph(proceeding);
                try
                {
                    _unitOfWork.SaveChanges();
                    _selectedItems.Clear();
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

        private void showMAssetDetails(IList<object> parameter)
        {
            if (!(parameter[1] is Window)) return;
            var window = parameter[1] as Window;
            var model = parameter[0] as ProceedingAssetModel;
            if (model != null)
            {
                Mouse.SetCursor(Cursors.Wait);
                this.Selected = model;
                var viewModel = new MovableAssetDetailsViewModel(_container, model.AssetId);
                StoryboardManager.PlayStoryboard("StoryboardHideWindow", window);
                _navigationServic.ShowMAssetDetailsWindow(viewModel);
                StoryboardManager.PlayStoryboard("StoryboardShowWindow", window);
                Mouse.SetCursor(Cursors.Arrow);
            }
        }

        private void showPermEditAssetWindow(IList<object> parameter)
        {
            if (!(parameter[1] is Window)) return;
            var window = parameter[1] as Window;
            var model = parameter[0] as ProceedingAssetModel;
            if (model != null)
            {
                Mouse.SetCursor(Cursors.Wait);
                this.Selected = model;
                StoryboardManager.PlayStoryboard("StoryboardHideWindow", window);
                var viewModel = new PermEditDetailsViewModel(_container, model.AssetId,true);
                viewModel.EditYear = model.TempYear ?? 0;
                viewModel.CurrentMovableAsset.Desc1 = model.TempDesc1;
                viewModel.CurrentMovableAsset.Desc2 = model.TempDesc2;
                viewModel.CurrentMovableAsset.Desc3 = model.TempDesc3;
                viewModel.CurrentMovableAsset.Desc4 = model.TempDesc4;
                viewModel.CurrentMovableAsset.Uid1 = model.TempUid1;
                viewModel.CurrentMovableAsset.Uid2 = model.TempUid2;
                viewModel.CurrentMovableAsset.Uid3 = model.TempUid3;
                viewModel.CurrentMovableAsset.Uid4 = model.TempUid4;
                var editwindow=_navigationServic.ShowPermEditWindow(viewModel);
                if (editwindow.DialogResult == true)
                {
                    model.TempYear = viewModel.EditYear;
                    model.TempDesc1 = viewModel.CurrentMovableAsset.Desc1;
                    model.TempDesc2 = viewModel.CurrentMovableAsset.Desc2;
                    model.TempDesc3 = viewModel.CurrentMovableAsset.Desc3;
                    model.TempDesc4 = viewModel.CurrentMovableAsset.Desc4;
                    model.TempUid1 = viewModel.CurrentMovableAsset.Uid1;
                    model.TempUid2 = viewModel.CurrentMovableAsset.Uid2;
                    model.TempUid3 = viewModel.CurrentMovableAsset.Uid3;
                    model.TempUid4 = viewModel.CurrentMovableAsset.Uid4;
                }
                StoryboardManager.PlayStoryboard("StoryboardShowWindow", window);
                Mouse.SetCursor(Cursors.Arrow);
            }
        }

        private Boolean detailsHaveError()
        {
            bool haveError = false;
            switch (ProceedingType)
            {
                case ProceedingsType.Fire:
                case ProceedingsType.Flood:
                case ProceedingsType.Earthquake:
                case ProceedingsType.Accident:
                case ProceedingsType.Theft:
                    if (string.IsNullOrWhiteSpace(ProceedingDetailsVM.Desc1))
                    {
                        haveError = true;
                    }
                    break;
                case ProceedingsType.Sale:
                    if (string.IsNullOrWhiteSpace(ProceedingDetailsVM.Desc1))
                    {
                        haveError = true;
                    }
                    break;
                case ProceedingsType.DefinitiveTransfer:
                case ProceedingsType.StateTransfer:
                    if (string.IsNullOrWhiteSpace(ProceedingDetailsVM.Desc1)
                         || string.IsNullOrWhiteSpace(ProceedingDetailsVM.Desc2)
                         || string.IsNullOrWhiteSpace(ProceedingDetailsVM.Desc3)
                        || string.IsNullOrWhiteSpace(ProceedingDetailsVM.Desc4)
                        || string.IsNullOrWhiteSpace(ProceedingDetailsVM.Desc5))
                    {
                        haveError = true;
                    }
                    break;
                case ProceedingsType.TrustTransfer:
                    if (string.IsNullOrWhiteSpace(ProceedingDetailsVM.Desc1)
                        || string.IsNullOrWhiteSpace(ProceedingDetailsVM.Desc2))
                    {
                        haveError = true;
                    }
                    break;
                case ProceedingsType.Delete:
                case ProceedingsType.SpecialLicencing:
                case ProceedingsType.BudgetLicencing:
                case ProceedingsType.RefundTrust:
                case ProceedingsType.ReturnFromTrust:
                case ProceedingsType.EditRequest:
                case ProceedingsType.ReturnFromRetiring:
                    haveError = false;
                    break;
                case ProceedingsType.AssetRetiring:
                    if (ProceedingDetailsVM.SelectedStore == null)
                    {
                        haveError = true;
                    }
                    else
                    {
                        ProceedingDetailsVM.Desc1 = ProceedingDetailsVM.SelectedStore.StoreId.ToString();
                    }
                    break;
                default:
                    haveError = true;
                    break;
            }
            return haveError;
        }

        #endregion

        #region commands

        public ICommand SearchCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand SelectCommand { get; private set; }
        public ICommand MAssetDetailsCommand { get; private set; }
        public ICommand MAssetEditCommand { get; private set; }
        private void initializCommands()
        {
            SearchCommand = new MvvmCommand(
                (parameter) => { this.SearchAvailableAssetsForProceeding(); },
                (parameter) => { return true; }
                );

            SelectCommand = new MvvmCommand(
                (parameter) => { this.selectItems(parameter); },
                (parameter) => { return true; }
                );

            SaveCommand = new MvvmCommand(
                 (parameter) => { this.addProceeding(); },
                (parameter) => { return true; }
                );

            MAssetDetailsCommand = new MvvmCommand(
                 (parameter) => { this.showMAssetDetails(parameter as IList<object>); },
                (parameter) => { return true; }
                );

            MAssetEditCommand = new MvvmCommand(
                (parameter) => { this.showPermEditAssetWindow(parameter as IList<object>); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IProceedingService _proceedingService;
        private readonly IStoreService _storeService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationServic;
        private readonly IPersonService _personService;
        private readonly ObservableCollection<ProceedingAssetModel> _collection;
        private readonly IOrderService _orderService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IUnitService _unitService;
        private readonly HashSet<ProceedingAssetModel> _selectedItems;
        private string _currentRefundTrustOrgan;
        private readonly SeedDataHelper _seedDataHelper;

        #endregion
    }
}
