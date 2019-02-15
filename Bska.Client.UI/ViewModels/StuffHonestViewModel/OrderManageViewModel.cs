
namespace Bska.Client.UI.ViewModels.StuffHonestViewModel
{
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.Services;
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.UI.API;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Domain.Entity;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.ViewModels.OrderViewModel;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows;
    using System.Windows.Controls;
    using System.Data.Entity.Infrastructure;
    using PersonDetailsInfoViewModels;
    using System.Threading.Tasks;
    using System.Threading;

    public sealed class OrderManageViewModel : BaseViewModel
    {
        #region ctor

        public OrderManageViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._orderService = _container.Resolve<IOrderService>(new ParameterOverride("repository", _unitOfWork.Repository<Order>()));
            this._unitService = _container.Resolve<IUnitService>();
            this._storeService = _container.Resolve<IStoreService>();
            this._employeeService = _container.Resolve<IEmployeeService>();
            this._storeBillService = _container.Resolve<IStoreBillService>();
            this._personService = _container.Resolve<IPersonService>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>(new ParameterOverride("repository", _unitOfWork.Repository<MovableAsset>()));
            this._commodityService = _container.Resolve<IMAssetCommodityService>();
            this._orderCollection = new ObservableCollection<OrderModel>();
            this.OrderFilteredView = new CollectionViewSource { Source = OrderCollection }.View;
            this._collection = new ObservableCollection<OrderDetails>();
            this._disCollection = new ObservableCollection<MovableAsset>();
            this._mAssetToStoreDictionary = new Dictionary<long, Store>();
            this._unitHelper = new UnitHelper();
            this._recivedTypes = new Dictionary<string, object>();
            this.initializObj();
            this.initializCommand();
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

        public String SearchCriteria
        {
            get { return GetValue(() => SearchCriteria); }
            set
            {
                SetValue(() => SearchCriteria, value);
                this.SearchOrder();
            }
        }

        public ObservableCollection<OrderModel> OrderCollection
        {
            get { return _orderCollection; }
        }

        public ICollectionView OrderFilteredView { get; set; }

        public OrderModel OMSelected
        {
            get { return GetValue(() => OMSelected); }
            set
            {
                SetValue(() => OMSelected, value);
                this.getOrderDetails();
            }
        }

        public Boolean InternalVisible
        {
            get { return GetValue(() => InternalVisible); }
            set
            {
                SetValue(() => InternalVisible, value);
            }
        }

        public Boolean OrderHistoryVisible
        {
            get { return GetValue(() => OrderHistoryVisible); }
            set
            {
                SetValue(() => OrderHistoryVisible, value);
            }
        }

        public Boolean DisplacementVisible
        {
            get { return GetValue(() => DisplacementVisible); }
            set
            {
                SetValue(() => DisplacementVisible, value);
            }
        }

        public ObservableCollection<OrderDetails> Collection
        {
            get { return _collection; }
        }

        public OrderDetails Selected
        {
            get { return GetValue(() => Selected); }
            set
            {
                SetValue(() => Selected, value);
            }
        }

        public ObservableCollection<MovableAsset> DisCollection
        {
            get { return _disCollection; }
        }

        public MovableAsset DisSelected
        {
            get { return GetValue(() => DisSelected); }
            set
            {
                SetValue(() => DisSelected, value);
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

        public List<OrderUserHistory> OrderUserHistories
        {
            get { return GetValue(() => OrderUserHistories); }
            set
            {
                SetValue(() => OrderUserHistories, value);
            }
        }
        public List<AnalizModel> Analizes
        {
            get { return GetValue(() => Analizes); }
            set
            {
                SetValue(() => Analizes, value);
            }
        }

        public List<Store> Stores
        {
            get { return GetValue(() => Stores); }
            set
            {
                SetValue(() => Stores, value);
            }
        }
        public Dictionary<string, object> RecivedTypes
        {
            get
            {
                return _recivedTypes;
            }
            set
            {
                _recivedTypes = value;
                OnPropertyChanged("RecivedTypes");
            }
        }

        public Dictionary<string, object> SelectedRecivedType
        {
            get { return GetValue(() => SelectedRecivedType); }
            set
            {
                SetValue(() => SelectedRecivedType, value);
            }
        }

        public Boolean IsEditableOrder
        {
            get { return GetValue(() => IsEditableOrder); }
            set
            {
                SetValue(() => IsEditableOrder, value);
            }
        }
        #endregion

        #region methods

        private void SearchOrder()
        {
            this.OrderFilteredView.Filter = ((obj) =>
            {
                var order = obj as OrderModel;
                if (order != null)
                {
                    return order.OrderId.ToString().StartsWith(SearchCriteria)
                        || order.PersonName.Contains(SearchCriteria);
                }
                return false;
            });
        }

        private  async void initializObj()
        {
            if (Thread.CurrentPrincipal.IsInRole("Manager"))
            {
                _recivedTypes.Add("درخواست های رسیده", "A001");
            }
            else
            {
                _recivedTypes.Add("درخواست های رسیده", "A001");
                _recivedTypes.Add("درخواست های در راه", "A002");
            }

            this.Units = _unitService.Queryable().ToList();
            SelectedRecivedType = new Dictionary<string, object>();
            SelectedRecivedType.Add("درخواست های رسیده", "A001");

            this.InternalVisible = true;
            await this.getRecivedOrdersAsync();
            filterOnRecivedType("درخواست های رسیده", true);
        }

        internal void filterOnRecivedType(string key, bool isChecked)
        {
            if (this.SelectedRecivedType.Count == _recivedTypes.Count)
            {
                this.OrderFilteredView.Filter = null;
                this.SearchCriteria = "";
            }
            else if (this.SelectedRecivedType.Count <= 0)
            {
                this.OrderFilteredView.Filter = (obj) =>
                {
                    var order = obj as OrderModel;
                    return order.NationalId == "-1";
                };
            }
            else
            {
                if (this.SelectedRecivedType.ContainsKey("درخواست های رسیده"))
                {
                    this.OrderFilteredView.Filter = (obj) =>
                    {
                        var order = obj as OrderModel;
                        return order.IsEditable;
                    };
                }
                else if (this.SelectedRecivedType.ContainsKey("درخواست های در راه"))
                {
                    this.OrderFilteredView.Filter = (obj) =>
                    {
                        var order = obj as OrderModel;
                        return !order.IsEditable;
                    };
                }
            }
        }
        private Task getRecivedOrdersAsync()
        {
            _orderCollection.Clear();
            bool isManger = false;
            if (Thread.CurrentPrincipal.IsInRole("Manager"))
            {
                isManger = true;
            }
            var ts = new Task(() =>
              {
                  _orderService.GetHonestRecivedOrders(UserLog.UniqueInstance.LogedUser.UserId, isManger).ToList().ForEach(o =>
                  {
                      DispatchService.Invoke(() =>
                      {
                          _orderCollection.Add(o);
                      });
                  });
              });
            ts.Start();
            return ts;
        }

        private void getOrderDetails()
        {
            if (OMSelected != null)
            {
                Mouse.SetCursor(Cursors.Wait);
                OrderUserHistories = null;
                Analizes = null;
                if (OMSelected.OrderType == OrderType.InternalRequest)
                {
                    _collection.Clear();
                    this.DisplacementVisible = false;
                    this.InternalVisible = true;
                    _orderService.GetHonestRecivedOrderDetails(OMSelected.OrderId).ToList().ForEach(od =>
                     {
                         _collection.Add(od);
                     });
                    this.Selected = _collection.FirstOrDefault();

                }
                else
                {
                    this.InternalVisible = false;
                    this.DisplacementVisible = true;
                    _currentOrder = _orderService.Query(x => x.OrderId == OMSelected.OrderId)
                        .Include(o => o.OrderDetails).Include(o => o.MovableAssets).Select().Single();
                    _disCollection.Clear();
                    _currentOrder.MovableAssets.ForEach(ma =>
                        {
                            var movedLoc = _movableAssetService.GetMovedRequestLocation(ma.AssetId);
                            if (movedLoc != null)
                                _disCollection.Add(ma);
                        });
                    OrderUserHistories = _orderService.GetUserHistories(_currentOrder.OrderDetails.First().OrderDetialsId)
                        .Where(ou => ou.UserDecision).ToList();
                    Stores = _storeService.Queryable().Where(x => x.StoreType != StoreType.Retiring).ToList();
                    _mAssetToStoreDictionary.Clear();
                }

                if (OMSelected.IsEditable)
                {
                    this.IsEditableOrder = true;
                }
                else
                {
                    this.IsEditableOrder = false;
                }

                Mouse.SetCursor(Cursors.Arrow);
            }
        }

        private void getOrderDetailsHistory(object parameter)
        {
            var od = parameter as OrderDetails;
            if (od == null) return;
            this.OrderHistoryVisible = true;
            this.Selected = od;
            this.OrderUserHistories = od.OrderUserHistories.Where(ou=>ou.UserDecision).ToList();
        }

        private void showIndentWindow(object parameter)
        {
            if (OMSelected == null) return;

            Mouse.SetCursor(Cursors.Wait);
            if (OMSelected.OrderType == OrderType.Displacement)
            {
                if (_mAssetToStoreDictionary.Count <= 0)
                {
                    _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoRowSelected);
                    return;
                }

                Boolean confirm = _dialogService.AskConfirmation("هشدار", ErrorMessages.Default.AskConfrimation);
                if (confirm)
                {
                    Mouse.SetCursor(Cursors.Wait);

                    Document trustDocument = null;
                    Document document = null;
                    AccountDocumentMaster accountMaster = null;
                    foreach (var item in _mAssetToStoreDictionary)
                    {
                        var mAsset = _disCollection.Single(x => x.AssetId == item.Key);
                        var storeBill = mAsset.StoreBill;
                        var oldLocation = mAsset.Locations.Single(l => l.Status == LocationStatus.MovedRequest);
                        oldLocation.Status = LocationStatus.DeActive;
                        oldLocation.ObjectState = ObjectState.Modified;
                        oldLocation.ReturnDate = GlobalClass._Today;
                        mAsset.Locations.Add(oldLocation);

                        var newLocation = new Location
                        {
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            StoreId = item.Value.StoreId,
                            StrategyId = oldLocation.StrategyId,
                            OrganizId = oldLocation.OrganizId,
                            Status = LocationStatus.StoreActive
                        };

                        if (storeBill.AcqType == StateOwnership.Trust)
                        {
                            newLocation.AccountDocumentType = AccountDocumentType.None;
                            if (trustDocument == null)
                            {
                                trustDocument = new Document
                                {
                                    DocumentType = DocumentType.ReturnToStoreTrustDraft,
                                    ObjectState = ObjectState.Added,
                                    DocumentDate = GlobalClass._Today,
                                    Desc1 = initDraftNo(DocumentType.ReturnToStoreTrustDraft),
                                    StoreId = item.Value.StoreId,
                                    Transferee = item.Value.Name
                                };
                            }
                            mAsset.Documetns.Add(trustDocument);
                        }
                        else
                        {
                            newLocation.AccountDocumentType = AccountDocumentType.UnitsToStock;
                            if (document == null)
                            {
                                document = new Document
                                {
                                    DocumentType = DocumentType.ReturnToStoreDraft,
                                    ObjectState = ObjectState.Added,
                                    DocumentDate = GlobalClass._Today,
                                    Desc1 = initDraftNo(DocumentType.ReturnToStoreDraft),
                                    StoreId = item.Value.StoreId,
                                    Transferee = item.Value.Name
                                };
                            }
                            mAsset.Documetns.Add(document);
                        }

                        if (mAsset is UnConsumption)
                        {
                            if (accountMaster == null)
                            {
                                accountMaster = new AccountDocumentMaster
                                {
                                    AccountCover = "1",
                                    AccountDate = GlobalClass._Today,
                                    EmployeeId = UserLog.UniqueInstance.LogedEmployee.EmployeeId,
                                    ObjectState = ObjectState.Added,
                                };
                            }
                            document.AccountDocument = accountMaster;
                            this.setAccountDocumentDetails(mAsset as UnConsumption, newLocation, accountMaster);
                            Boolean hasBelongings = _movableAssetService.ContainBelongings(mAsset.AssetId);
                            if (hasBelongings)
                            {
                                var belongs = _movableAssetService.GetBelongingsToLocation(mAsset.AssetId).ToList();
                                belongs.ForEach(b =>
                                {
                                    var bStoreBill = _movableAssetService.GetStoreBill(b.AssetId);
                                    var bOldLocation = b.Locations.Single(l => l.Status == LocationStatus.MovedRequest);
                                    bOldLocation.Status = LocationStatus.DeActive;
                                    bOldLocation.ObjectState = ObjectState.Modified;
                                    bOldLocation.ReturnDate = GlobalClass._Today;

                                    b.Locations.Add(bOldLocation);

                                    var bnewLocation = new Location
                                    {
                                        InsertDate = GlobalClass._Today,
                                        ObjectState = ObjectState.Added,
                                        StoreId = item.Value.StoreId,
                                        StrategyId = oldLocation.StrategyId,
                                        OrganizId = oldLocation.OrganizId,
                                        Status = LocationStatus.StoreActive
                                    };

                                    if (bStoreBill.AcqType == StateOwnership.Trust)
                                    {
                                        bnewLocation.AccountDocumentType = AccountDocumentType.None;
                                        if (trustDocument == null)
                                        {
                                            trustDocument = new Document
                                            {
                                                DocumentType = DocumentType.ReturnToStoreTrustDraft,
                                                ObjectState = ObjectState.Added,
                                                DocumentDate = GlobalClass._Today,
                                                Desc1 = initDraftNo(DocumentType.ReturnToStoreTrustDraft),
                                                StoreId = item.Value.StoreId,
                                                Transferee = item.Value.Name,
                                            };
                                        }
                                        b.Documetns.Add(trustDocument);
                                    }
                                    else
                                    {
                                        bnewLocation.AccountDocumentType = AccountDocumentType.UnitsToStock;
                                        if (document == null)
                                        {
                                            document = new Document
                                            {
                                                DocumentType = DocumentType.ReturnToStoreDraft,
                                                ObjectState = ObjectState.Added,
                                                DocumentDate = GlobalClass._Today,
                                                Desc1 = initDraftNo(DocumentType.ReturnToStoreDraft),
                                                StoreId = item.Value.StoreId,
                                                Transferee = item.Value.Name
                                            };
                                        }
                                        b.Documetns.Add(document);
                                    }
                                    b.Locations.Add(bnewLocation);
                                });
                            }
                        }
                        else if (mAsset is Belonging)
                        {
                            var bMAsset = mAsset as Belonging;
                            if (bMAsset.ParentMAsset == null)
                            {
                                bMAsset.ParentMAsset = _movableAssetService.GetBelongingParnet(bMAsset.AssetId);
                            }
                            bMAsset.ParentMAsset = null;
                            bMAsset.ObjectState = ObjectState.Modified;
                            bMAsset.ModeifiedDate = GlobalClass._Today;
                        }
                        mAsset.Locations.Add(newLocation);
                        _currentOrder.MovableAssets.Add(mAsset);
                        _disCollection.Remove(mAsset);
                    }

                    if (_disCollection.Count == 0)
                    {
                        var details = _currentOrder.OrderDetails.FirstOrDefault();
                        var newHistory = new OrderUserHistory
                        {
                            Description = "تحویل درخواست.توسط کاربر با نام:" + " " + UserLog.UniqueInstance.LogedUser.FullName + " " +
                              "در تاریخ:" + " " + GlobalClass._Today.PersianDateString(),
                            UserId = UserLog.UniqueInstance.LogedUser.UserId,
                            ObjectState = ObjectState.Added,
                            UserDecision = true,
                        };
                        details.State = OrderDetailsState.Deliviry;
                        details.OrderUserHistories.Add(newHistory);
                        details.ObjectState = ObjectState.Modified;
                        _currentOrder.Status = OrderStatus.Deliviry;
                        _currentOrder.DueDate = GlobalClass._Today;
                        _currentOrder.ObjectState = ObjectState.Modified;
                        OrderUserHistories = null;
                        _orderCollection.Remove(OMSelected);
                    }

                    try
                    {
                        _orderService.InsertOrUpdateGraph(_currentOrder);
                        _unitOfWork.SaveChanges();
                        _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                        _mAssetToStoreDictionary.Clear();
                        Mouse.SetCursor(Cursors.Arrow);
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
            else
            {
                if (Selected == null)
                {
                    Selected = parameter as OrderDetails;
                    if (Selected == null)
                    {
                        _dialogService.ShowAlert("توجه", "هیچ درخواستی انتخاب نشده است");
                        return;
                    }
                }

                if (Selected.IsReject)
                {
                    _dialogService.ShowAlert("توجه", "این درخواست رد شده و قابلیت سفارش ندارد");
                    return;
                }

                if (!IsEditableOrder)
                {
                    _dialogService.ShowAlert("توجه", "برای این درخواست شما هنوز مجوز سفارش را دریافت نکرده اید");
                    return;
                }

                StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
                var viewModel = new IndentViewModel(_container);
                viewModel.CurrentOrders = _collection.Where(v => !v.IsReject).ToList();
                viewModel.CurrentOrderDetails = Selected;
                _navigationService.ShowIndentWindow(viewModel);
                StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
                this.getOrderDetails();
            }
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void setAccountDocumentDetails(UnConsumption asset,Location loc,AccountDocumentMaster accountMaster)
        {
            var ma = asset;
            if (ma != null && loc!=null)
            {
                List<Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>> currentAccountCodings = new List<Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>>();
                var accountCodings = _employeeService.GetAccountCodings();
                var parentcode = accountCodings.FirstOrDefault(x => x.Parent == null);

                string desc = "نامشخص";
                string code = "0";
                if (loc.AccountDocumentType == AccountDocumentType.UnitsToStock)
                {
                    var organization = _employeeService.GetParentNode(1).FirstOrDefault(x=>x.BuidldingDesignId== loc.OrganizId);
                    if (organization != null)
                    {
                        var getItem = this.GetHirecharyNode(organization);
                        desc = getItem.Item1;
                        code = getItem.Item2;
                    }
                    currentAccountCodings.Add(
                       new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.UnitsDeliviry),
                       "Creditor", desc, code, ma.Cost, ma));

                    code = ma.KalaUid.ToString();
                    desc = ma.Name + "**" + ma.Cost.ToString("N0");
                    currentAccountCodings.Add(
                       new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.StockAssetBuyAndType),
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
                        AccountDocumentMaster= accountMaster,
                    };
                    adc.Item6.AccountDocumentDetails.Add(newDetails);
                });
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

        private void showOrderDetails(object parameter)
        {
            var od = parameter as OrderDetails;
            if (od == null) return;
            this.Selected = od;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new OrderDetailsManageViewModel(_container, false);
            viewModel.Units = this.Units;
            viewModel.CurrentOrderDetails = od;
            viewModel.AllOrderDetails = _collection.ToList();
            _navigationService.ShowOrderDetailsWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void addDisplacementToStore(object parameter)
        {
            var comboBox = parameter as ComboBox;
            if (comboBox == null) return;
            if (comboBox.SelectedIndex == -1) return;
            var mAsset = comboBox.Tag as MovableAsset;
            if (mAsset == null) return;
            Store selectedStore = comboBox.SelectedItem as Store;
            Boolean storeCompatibale = true;
           
            if (!storeCompatibale)
            {
                _dialogService.ShowAlert("توجه", "انبار انتخاب شده با نوع مال هم خوانی ندارد");
                comboBox.SelectedIndex = -1;
                return;
            }
            Mouse.SetCursor(Cursors.Wait);
            if (!_mAssetToStoreDictionary.ContainsKey(mAsset.AssetId))
            {
                mAsset.StoreBill = _movableAssetService.GetStoreBill(mAsset.AssetId);
                _movableAssetService.Queryable().Where(x => x.AssetId == mAsset.AssetId).SelectMany(x => x.Locations).ToList().ForEach(l =>
                {
                    if(!mAsset.Locations.Contains(l))
                    mAsset.Locations.Add(l);
                });
                _mAssetToStoreDictionary.Add(mAsset.AssetId, selectedStore);
            }
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showMAssetDetails(object parameter)
        {
            var mAsset = parameter as MovableAsset;
            if (mAsset == null) return;
            this.DisSelected = mAsset;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", Window);
            var viewModel = new MovableAssetDetailsViewModel(_container,mAsset.AssetId);
            _navigationService.ShowMAssetDetailsWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", Window);
            Mouse.SetCursor(Cursors.Arrow);
        }
        
        private String initDraftNo(DocumentType docType)
        {
            string draftNo = "";
            int temp;
            var docs = _movableAssetService.GetDocuments(false,false).ToList();
            if (docs.Count() > 0)
            {
                int maxVal = docs
                    .Select(d => int.TryParse(d.Desc1, out temp) ? temp : 0).Max();
                draftNo = (maxVal + 1).ToString();
            }
            else
            {
                draftNo = "1";
            }
            return draftNo;
        }

        private void showStoreUseableAsset(object parameter)
        {
            if (OMSelected == null)
            {
                _dialogService.ShowAlert("", "هیچ درخواستی انتخاب نشده است");
                return;
            }

            var analizM = parameter as AnalizModel;
            if (analizM == null)
            {
                analizM = new AnalizModel();
            }
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var requestStuffs = new List<Tuple<string, StuffType,int,string>>();

            if(OMSelected.OrderType==OrderType.InternalRequest)
            {
                _collection.GroupBy(x=>new {x.kalaNo}).ForEach(od =>
                {
                    requestStuffs.Add(new Tuple<string, StuffType, int,string>(od.First().StuffName, od.First().StuffType, od.First().KalaUid,od.Key.kalaNo));
                });
            }
            else
            {
                StuffType sType = StuffType.None;
                _disCollection.GroupBy(x => new { x.KalaNo }).ForEach(ds =>
                {
                    if (ds is Belonging) sType = StuffType.Belonging;
                    else if (ds is UnConsumption) sType = StuffType.UnConsumption;
                    else if (ds is InCommidity) sType = StuffType.OrderConsumption;
                    else if (ds is Installable) sType = StuffType.Installable;
                    requestStuffs.Add(new Tuple<string, StuffType, int,string>(ds.First().Name, sType, ds.First().KalaUid,ds.Key.KalaNo));
                });
            }
            var viewmodel = new StoreAssetDetailsViewModel(_container, requestStuffs,analizM);

            if (Selected != null)
            {
                viewmodel.SelectedStuff = requestStuffs.FirstOrDefault(v => v.Item3 == Selected.KalaUid);
            }
            _navigationService.ShowStoreAssetDetailsWindow(viewmodel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void ShowPersonDetails(object parameter)
        {
            if (OMSelected == null)
            {
                OrderModel om = parameter as OrderModel;
                OMSelected = om;
            }
            if (OMSelected == null)
            {
                _dialogService.ShowAlert("", "هیچ درخواستی انتخاب نشده است");
                return;
            }
            Mouse.SetCursor(Cursors.Wait);
            Order order = null;
            order = _orderService.Find(OMSelected.OrderId);
            var person = _personService.Find(order.PersonId);
            if (person == null) return;
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new PersonDetailsInfoViewModel(person.PersonId);
            _navigationService.ShowPersonDetailsWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private async void analizationOnOrderAsync(object parameter)
        {
            var od = parameter as OrderDetails;
            if (od == null) return;
            Selected = od;
            Analizes = null;
            OrderHistoryVisible = false;
            Task ts = null;
            DateTime fromDate = GlobalClass._Today.AddMonths(-6);
            DateTime toDate = GlobalClass._Today;
            if(OMSelected.OrderType==OrderType.InternalRequest)
            {
                if (Selected != null)
                {
                    double allMemo = 0;
                    int kalaUid = Selected.KalaUid;
                    var items = new List<AnalizModel>();
                    var ounit = Units.First(u => u.UnitId == Selected.UnitId);
                    if (Selected.StuffType == StuffType.Consumable)
                    {
                        ts = new Task(() =>
                        {
                            _commodityService.Queryable().Where(co => co.KalaUid == Selected.KalaUid && 
                            (co.InsertDate > fromDate && co.InsertDate <= toDate))
                             .Select(x=>new
                             {
                                 x.PlaceOfUses,
                                 x.Num,
                                 x.UnitId
                             }).ToList().ForEach(co =>
                             {
                                 var coU = Units.First(v => v.UnitId == co.UnitId);
                                 double coMemo = co.Num;
                                 if (co.PlaceOfUses.Count() > 0)
                                 {
                                     co.PlaceOfUses.ForEach(cop =>
                                     {
                                         double memo = 0;
                                         var copU = Units.First(u => u.UnitId == cop.UnitId);
                                         if (coU.Equals(copU))
                                         {
                                             memo += cop.Num;
                                         }
                                         else
                                         {
                                             Unit isOrderChild = null;
                                             Unit IsPropertyChild = null;

                                             if (coU != null)
                                             {
                                                 isOrderChild = _unitHelper.mainparentRecovery(coU);
                                             }

                                             if (copU != null)
                                             {
                                                 IsPropertyChild = _unitHelper.mainparentRecovery(copU);
                                             }

                                             if (isOrderChild.Equals(IsPropertyChild))
                                             {
                                                 Double orderval = _unitHelper.CalculateUnitNum(copU, cop.Num);
                                                 double propertyVal = _unitHelper.ReverseCalculateUnitNum(coU, orderval);
                                                 memo = propertyVal;
                                                
                                             }
                                         }
                                         coMemo -= memo;
                                     });
                                 }

                                 if (coU.Equals(ounit))
                                 {
                                     allMemo +=coMemo;
                                 }
                                 else
                                 {
                                     Unit isOrderChild = null;
                                     Unit IsPropertyChild = null;

                                     if (coU != null)
                                     {
                                         isOrderChild = _unitHelper.mainparentRecovery(coU);
                                     }

                                     if (ounit != null)
                                     {
                                         IsPropertyChild = _unitHelper.mainparentRecovery(ounit);
                                     }

                                     if (isOrderChild.Equals(IsPropertyChild))
                                     {
                                         Double orderval = _unitHelper.CalculateUnitNum(coU, coMemo);
                                         double propertyVal = _unitHelper.ReverseCalculateUnitNum(ounit, orderval);
                                         allMemo += propertyVal;
                                     }
                                 }
                             });

                            items.Add(new AnalizModel
                            {
                                Description = "موجودی حال حاضر",
                                Num = allMemo,
                                UnitName = ounit.Name,
                                Identity = AnalizModelIdentity.Stock
                            });
                            
                            items.Add(_storeBillService.GetStoreBillAnalized(kalaUid,Selected.StuffType, fromDate, toDate,true));
                            items.Add(_commodityService.GetInternalDocAnaliz(kalaUid, fromDate, toDate));
                            
                            _orderService.GetOrderDetailsByKalaUid(kalaUid,Selected.StuffType, fromDate, toDate,false)
                             .GroupBy(g => new { g.KalaUid, g.Status }).Where(g => g.Count() > 0).ForEach(og =>
                             {
                                 items.Add(new AnalizModel
                                 {
                                     Description = "درخواست ها با وضعیت " + og.Key.Status.GetDescription(),
                                     Num = og.Sum(b =>
                                     {
                                         double memo = 0;
                                         var punit = Units.First(t => t.UnitId == b.UnitId);
                                     if (punit.Equals(ounit))
                                         {
                                             memo = b.Num;
                                         }
                                     else
                                         {
                                             Unit isOrderChild = null;
                                             Unit IsPropertyChild = null;

                                             if (punit != null)
                                             {
                                                 isOrderChild = _unitHelper.mainparentRecovery(punit);
                                             }

                                             if (ounit != null)
                                             {
                                                 IsPropertyChild = _unitHelper.mainparentRecovery(ounit);
                                             }

                                             if (isOrderChild.Equals(IsPropertyChild))
                                             {
                                                 Double orderval = _unitHelper.CalculateUnitNum(punit, b.Num);
                                                 double propertyVal = _unitHelper.ReverseCalculateUnitNum(ounit, orderval);
                                                 memo = propertyVal;
                                             }
                                         }
                                        return memo;
                                     }),

                                     UnitName = ounit.Name,
                                     Identity = AnalizModelIdentity.Order,
                                     OrderStatus=og.Key.Status
                                 });
                             });
                            
                            DispatchService.Invoke(() =>
                            {
                                Analizes = items;
                            });
                        });
                    }
                    else
                    {
                        ts = new Task(() =>
                        {
                            items.Add(_movableAssetService.GetCurrentStoreByKalaUid(kalaUid,Selected.StuffType,false));
                            items.Add(_storeBillService.GetStoreBillAnalized(kalaUid,Selected.StuffType, fromDate, toDate,false));
                            items.Add(_movableAssetService.GetStoreInternalDocAnaliz(kalaUid,Selected.StuffType, fromDate, toDate,false));
                            _orderService.GetOrderDetailsByKalaUid(kalaUid,Selected.StuffType, fromDate, toDate,false)
                            .GroupBy(g => new { g.KalaUid, g.UnitId, g.Status }).Where(g => g.Count() > 0).ForEach(og =>
                            {
                                var u = Units.FirstOrDefault(un => un.UnitId == og.Key.UnitId);
                                items.Add(new AnalizModel
                                {
                                    Description = "درخواست ها با وضعیت " + og.Key.Status.GetDescription(),
                                    Num = og.Sum(b => b.Num),
                                    UnitName = u?.Name,
                                    Identity=AnalizModelIdentity.Order,
                                    OrderStatus = og.Key.Status
                                });
                            });

                            DispatchService.Invoke(() =>
                            {
                                Analizes = items;
                            });
                        });
                    }
                    
                }
            }

            if (ts != null)
            {
                ts.Start();
                await ts;
            }
        }

        #endregion

        #region commands
        public ICommand OrderDetailsHistoryCommand { get; private set; }
        public ICommand IndentCommand { get; private set; }
        public ICommand OrderDetailsCommand { get; private set; }
        public ICommand StoreSelectCommand { get; private set; }
        public ICommand DetailsCommand { get; private set; }
        public ICommand StoreDetailsCommand { get; private set; }
        public ICommand PersonDetailsCommand { get; private set; }
        public ICommand AnalizCommand { get; private set; }
        public ICommand DoubleClickListViewItemCommand { get; private set; }
        public ICommand DoubleClickListBoxItemCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        private void initializCommand()
        {
            OrderDetailsHistoryCommand = new MvvmCommand(
                (parameter) => { this.getOrderDetailsHistory(parameter); },
                (parameter) => { return true; }
                );

            AnalizCommand = new MvvmCommand(
                 (parameter) => { this.analizationOnOrderAsync(parameter); },
                (parameter) => { return true; }
                );

            IndentCommand = new MvvmCommand(
                (parameter) => { this.showIndentWindow(parameter); },
                (parameter) => { return IsEditableOrder; }
                ).AddListener<RecivedOrderViewModel>(this, th => th.IsEditableOrder);

            OrderDetailsCommand = new MvvmCommand(
                (parameter) => { this.showOrderDetails(parameter); },
                (parameter) => { return true; }
                );

            StoreSelectCommand = new MvvmCommand(
             (parameter) => { this.addDisplacementToStore(parameter); },
             (parameter) => { return true; }
             );

            DetailsCommand = new MvvmCommand(
               (parameter) => { this.showMAssetDetails(parameter); },
               (parameter) => { return true; }
               );

            StoreDetailsCommand = new MvvmCommand(
             (parameter) => { this.showStoreUseableAsset(null); },
             (parameter) => { return true; });

            PersonDetailsCommand = new MvvmCommand(
            (parameter) => { this.ShowPersonDetails(parameter); },
            (parameter) => { return true; });

            DoubleClickListViewItemCommand = new MvvmCommand(
                (parameter) => { this.showIndentWindow(parameter); },
                 (parameter) => { return true; }
                );

            DoubleClickListBoxItemCommand = new MvvmCommand(
                 (parameter) => { this.showStoreUseableAsset(parameter); },
             (parameter) => { return true; }
                );

            RefreshCommand = new MvvmCommand(
             async (parameter) =>
             {
               await  this.getRecivedOrdersAsync();
             },
             (parameter) => { return true; }
             );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IEmployeeService _employeeService;
        private readonly IOrderService _orderService;
        private readonly IUnitService _unitService;
        private readonly IStoreService _storeService;
        private readonly IPersonService _personService;
        private readonly IStoreBillService _storeBillService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IMAssetCommodityService _commodityService;
        private readonly ObservableCollection<OrderModel> _orderCollection;
        private readonly ObservableCollection<OrderDetails> _collection;
        private readonly ObservableCollection<MovableAsset> _disCollection;
        private readonly Dictionary<long,Store> _mAssetToStoreDictionary;
        private readonly UnitHelper _unitHelper;
        private Order _currentOrder;
        private Dictionary<string, object> _recivedTypes;

        #endregion
    }
}
