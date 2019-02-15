
namespace Bska.Client.UI.ViewModels.StoreViewModel
{
    using System;
    using Microsoft.Practices.Unity;
    using Domain.Entity.AssetEntity;
    using Services;
    using Data.Service;
    using Client.API.UnitOfWork;
    using Domain.Entity.OrderEntity;
    using System.Collections.Generic;
    using Domain.Entity;
    using System.Linq;
    using System.Collections.ObjectModel;
    using Repository.Model;
    using System.Threading.Tasks;
    using Helper;
    using Common;
    using System.Windows.Input;
    using System.Windows.Controls;
    using API;
    using Client.API.Infrastructure;
    using System.Windows;
    using System.Data.Entity.Infrastructure;
    using Domain.Entity.AssetEntity.CommodityAsset;

    public sealed class StoreBillToDocumentViewModel : BaseViewModel
    {
        #region ctor

        public StoreBillToDocumentViewModel(IUnityContainer container,int storeBillId,Store store)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._orderService = _container.Resolve<IOrderService>(new ParameterOverride("repository", _unitOfWork.Repository<Order>()));
            this._movableAssetService = _container.Resolve<IMovableAssetService>(new ParameterOverride("repository", _unitOfWork.Repository<MovableAsset>()));
            this._commodityService = _container.Resolve<IMAssetCommodityService>(new ParameterOverride("repository", _unitOfWork.Repository<Commodity>()));
            this._storeBillService = _container.Resolve<IStoreBillService>(new ParameterOverride("repository", _unitOfWork.Repository<StoreBill>()));
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._employeeService = _container.Resolve<IEmployeeService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._collection = new ObservableCollection<MovableAssetModel>();
            this._selectedAssets = new HashSet<PortableAsset>();
            this._documentDictionary = new Dictionary<string, Document>();
            this.Store = store;
            this.initializObj(storeBillId);
            this.initializCommand();
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

        public List<Unit> Units
        {
            get { return GetValue(() => Units); }
            set
            {
                SetValue(() => Units, value);
            }
        }

        public Store Store
        {
            get { return GetValue(() => Store); }
            set
            {
                SetValue(() => Store, value);
            }
        }
        public ObservableCollection<MovableAssetModel> Collection
        {
            get { return _collection; }
        }

        public int Remain
        {
            get { return GetValue(() => Remain); }
            set
            {
                SetValue(() => Remain, value);
            }
        }

        public string DraftNo
        {
            get { return GetValue(() => DraftNo); }
            set
            {
                SetValue(() => DraftNo, value);
            }
        }

        public int CurrentNum
        {
            get { return GetValue(() => CurrentNum); }
            set
            {
                SetValue(() => CurrentNum, value);
            }
        }
        public List<string> RequestHistories
        {
            get { return GetValue(() => RequestHistories); }
            set
            {
                SetValue(() => RequestHistories, value);
            }
        }
        #endregion

        #region methods

        private async void initializObj(int storeBillId)
        {
            Units = _unitService.Queryable().ToList();
            CurrentBill = _storeBillService.Query(x => x.StoreBillId == storeBillId)
                .Include(x => x.MAssets).Include(x => x.Commodities).Select().Single();
            DraftNo = await this.initDraftNoAsync(DocumentType.StoreInternalDraft);
            await this.getRelatedAssetsAsync();
        }

        private Task getRelatedAssetsAsync()
        {
            _collection.Clear();
            var ts = new Task(() =>
              {
                  CurrentBill.MAssets.ForEach(x =>
                  {
                      var reservers = _movableAssetService.GetReserveHistories(x.AssetId).First();
                      bool isRowEnable = false;

                      if (reservers.Status == MAssetReserveStatus.Reserved)
                      {
                          isRowEnable = true;
                      }

                      var ma = new MovableAssetModel
                          {
                              AssetId = x.AssetId,
                              MAssetType = x.ToString("t", null),
                              UnitId = x.UnitId,
                              Name = x.Name,
                              Num = x.Num,
                              kalaUid = x.KalaUid,
                              KalaNo = x.KalaNo,
                              Label = x.Label,
                              InsertDate = x.InsertDate,
                              IsRowEnabled = isRowEnable,
                              ReserveStatus=reservers.Status
                          };
                          DispatchService.Invoke(() =>
                          {
                              _collection.Add(ma);
                          });

                      });

                 CurrentBill.Commodities.ForEach(co =>
                  {
                      var reservers = _commodityService.GetReserveHistories(co.AssetId).First();
                      bool isRowEnable = false;

                      if (reservers.Status == MAssetReserveStatus.Reserved)
                      {
                          isRowEnable = true;
                      }

                      var coma = new MovableAssetModel
                      {
                          AcqType = StateOwnership.Purchase,
                          AssetId = co.AssetId,
                          InsertDate = co.InsertDate,
                          CurState = MAssetCurState.AtOperation,
                          IsCompietion = CompietionState.NotReported,
                          IsConfirmed = false,
                          IsInStore = true,
                          kalaUid = co.KalaUid,
                          Label = null,
                          MAssetType = "مصرفی",
                          Name = co.Name,
                          Num = co.Num,
                          UnitId = co.UnitId,
                          IsRowEnabled = isRowEnable,
                          ReserveStatus = reservers.Status
                      };
                      DispatchService.Invoke(() =>
                      {
                          _collection.Add(coma);
                      });
                  });
              });
            ts.Start();
            return ts;
        }

        private Task<String> initDraftNoAsync(DocumentType docType)
        {
            var ts = new Task<string>(() =>
            {
                string draftNo = "";
                int temp;
                var docs = _movableAssetService.GetDocuments(false).ToList();
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
            });
            ts.Start();
            return ts;
        }

        private void SelcectMAsset(object parameter)
        {
            Mouse.SetCursor(Cursors.Wait);

            var checkbox = parameter as CheckBox;
            if (checkbox == null) return;
            var selectedAsset = checkbox.Tag as MovableAssetModel;
            if (selectedAsset == null) return;

            PortableAsset mAsset = null;
            if (string.Equals(selectedAsset.MAssetType, "مصرفی"))
            {
                var ma = CurrentBill.Commodities.Single(x => x.AssetId == selectedAsset.AssetId);
                _commodityService.GetReserveHistories(ma.AssetId).ForEach(rs =>
                {
                    if (!ma.CommodityAssetReserveHistories.Contains(rs))
                    {
                        ma.CommodityAssetReserveHistories.Add(rs);
                    }
                });
                mAsset = ma;
            }
            else
            {
                var ma = CurrentBill.MAssets.Single(x => x.AssetId == selectedAsset.AssetId);
                _movableAssetService.GetReserveHistories(ma.AssetId).ForEach(rs =>
                {
                    if (!ma.MovableAssetReserveHistories.Contains(rs))
                    {
                        ma.MovableAssetReserveHistories.Add(rs);
                    }
                });
                mAsset = ma;
            }

            if (checkbox.IsChecked == true)
            {
                _selectedAssets.Add(mAsset);
            }
            else
            {
                if (_selectedAssets.Contains(mAsset))
                {
                    _selectedAssets.Remove(mAsset);
                }
            }

            CurrentNum = _selectedAssets.Count;
            Mouse.SetCursor(Cursors.Arrow);
        }

        private async void SubmitIndent()
        {
            if (_selectedAssets.Count <= 0)
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoRowSelected);
                return;
            }

            bool confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                Mouse.SetCursor(Cursors.Wait);

                AccountDocumentMaster accountMaster = null;
                Document document = null;
                
                Boolean belongingHasError = false;
                string msg = "خطا درمورد اموال متعلقه در سیستم رخ داد.لطفا به مدیر سیستم مراجعه کنید";
                string key = "";
                HashSet<Order> orders = new HashSet<Order>();
                _selectedAssets.ForEach(mo =>
                {
                    Order order = null;
                    OrderDetails currentOrderDetails = null;
                    var subOrder = _orderService.GetSubOrderBySupplierIndent(mo.IndentId.Value);
                    if (mo is Commodity)
                    {
                        var commodityAsset = mo as Commodity;

                        order = _orderService.Query(o => o.Commodities.Any(m => m.AssetId == mo.AssetId))
                         .Include(o => o.OrderDetails).Include(p => p.Person).Select().Single();

                        currentOrderDetails = order.OrderDetails.First(x => x.OrderDetialsId == subOrder.OrderDetailsId);
                        
                        key= order.Person.NationalId + "-" + currentOrderDetails.OrganizId.ToString()
                        + "-" + currentOrderDetails.StrategyId.ToString();

                        if (!_documentDictionary.ContainsKey(key))
                        {
                            document = new Document
                            {
                                Desc1 = this.DraftNo,
                                DocumentDate = GlobalClass._Today,
                                Desc2 = this.Store.StoreId.ToString(),
                                DocumentType = DocumentType.StoreInternalDraft,
                                ObjectState = ObjectState.Added,
                                StoreId = Store.StoreId,
                                Transferee = $"{order.Person.FirstName} {order.Person.LastName}"
                            };
                            accountMaster = document.AccountDocument;
                            _documentDictionary.Add(key, document);
                        }
                        else
                        {
                            document = _documentDictionary[key];
                        }

                        commodityAsset.PlaceOfUses.Add(new PlaceOfUse
                        {
                            ObjectState = ObjectState.Added,
                            Document = document,
                            Num = mo.Num,
                            OrganizId = currentOrderDetails.OrganizId.Value,
                            StrategtyId = currentOrderDetails.StrategyId.Value,
                            UnitId = mo.UnitId,
                            InsertDate = GlobalClass._Today,
                            PersonId = order.Person.NationalId,
                        });
                        var reservHistory = commodityAsset.CommodityAssetReserveHistories
                        .FirstOrDefault(co => co.Status == MAssetReserveStatus.Reserved);
                        if (reservHistory != null)
                        {
                            reservHistory.ObjectState = ObjectState.Modified;
                            reservHistory.Description +="/"+"ثبت حواله انبار و خروج از رزرو";
                            reservHistory.Status = MAssetReserveStatus.UnReserved;
                            commodityAsset.CommodityAssetReserveHistories.Add(reservHistory);
                        }
                        _commodityService.InsertOrUpdateGraph(commodityAsset);
                    }
                    else
                    {
                        var mAsset = mo as MovableAsset;

                        order = _orderService.Query(o => o.MovableAssets.Any(m => m.AssetId == mo.AssetId))
                        .Include(o => o.OrderDetails).Include(p => p.Person).Select().Single();

                        currentOrderDetails = order.OrderDetails.First(x => x.OrderDetialsId == subOrder.OrderDetailsId);

                        key = order.Person.NationalId + "-" + currentOrderDetails.OrganizId.ToString()
                       + "-" + currentOrderDetails.StrategyId.ToString();

                        if (!_documentDictionary.ContainsKey(key))
                        {
                            document = new Document
                            {
                                Desc1 = this.DraftNo,
                                DocumentDate = GlobalClass._Today,
                                Desc2 = this.Store.StoreId.ToString(),
                                DocumentType = DocumentType.StoreInternalDraft,
                                ObjectState = ObjectState.Added,
                                StoreId = Store.StoreId,
                                Transferee = $"{order.Person.FirstName} {order.Person.LastName}"
                            };
                            accountMaster = document.AccountDocument;
                            _documentDictionary.Add(key, document);
                        }
                        else
                        {
                            document = _documentDictionary[key];
                        }

                        MovableAsset selected = mo as MovableAsset;
                        var location = _movableAssetService.GetLocation(selected.AssetId, isInStore: true);

                        location.ObjectState = ObjectState.Modified;
                        location.Status = LocationStatus.StoreDeActive;
                        location.MovedRequestDate = GlobalClass._Today;
                        location.ReturnDate = GlobalClass._Today;

                        Location newLocation = null;
                        newLocation = new Location
                        {
                            InsertDate = GlobalClass._Today,
                            OrganizId = currentOrderDetails.OrganizId.Value,
                            PersonId = order.Person.NationalId,
                            Status = LocationStatus.Active,
                            StrategyId = currentOrderDetails.StrategyId.Value,
                            ObjectState = ObjectState.Added,
                            StoreId = location.StoreId,
                            StoreAddressId = location.StoreAddressId,
                            AccountDocumentType = AccountDocumentType.StockToUnits,
                        };
                        
                        selected.Documetns.Add(document);

                        if (selected is UnConsumption)
                        {
                            foreach (var b in ((UnConsumption)selected).Belongings)
                            {
                                var blocation = _movableAssetService.GetLocation(b.AssetId, true);
                                blocation.ObjectState = ObjectState.Modified;
                                blocation.Status = LocationStatus.StoreDeActive;
                                blocation.MovedRequestDate = GlobalClass._Today;
                                blocation.ReturnDate = GlobalClass._Today;

                                Location bnewLocation = null;

                                bnewLocation = new Location
                                {
                                    InsertDate = GlobalClass._Today,
                                    OrganizId = currentOrderDetails.OrganizId.Value,
                                    PersonId = order.Person.NationalId,
                                    Status = LocationStatus.Active,
                                    StrategyId = currentOrderDetails.StrategyId.Value,
                                    ObjectState = ObjectState.Added,
                                    StoreId = location.StoreId,
                                    StoreAddressId = location.StoreAddressId,
                                    AccountDocumentType = AccountDocumentType.StockToUnits,
                                };
                                b.Documetns.Add(document);

                                b.Locations.Add(blocation);
                                b.Locations.Add(bnewLocation);
                            }

                            if (accountMaster == null)
                            {
                                accountMaster = new AccountDocumentMaster
                                {
                                    AccountCover = "1",
                                    AccountDate = GlobalClass._Today,
                                    EmployeeId = UserLog.UniqueInstance.LogedEmployee.EmployeeId,
                                    ObjectState = ObjectState.Added,
                                };

                                document.AccountDocument = accountMaster;
                            }

                            this.setAccountDocumentDetails(selected as UnConsumption, newLocation, accountMaster);
                        }
                        else if (selected is Belonging)
                        {
                            var parent = _movableAssetService.Queryable().OfType<UnConsumption>()
                                    .SingleOrDefault(x => x.AssetId == currentOrderDetails.BelongingParentLable);
                            if (parent == null)
                            {
                                belongingHasError = true;
                            }
                           ((Belonging)selected).ParentMAsset = parent;
                        }

                        selected.Locations.Add(location);
                        selected.Locations.Add(newLocation);
                        var reservHistory = selected.MovableAssetReserveHistories
                      .FirstOrDefault(co => co.Status == MAssetReserveStatus.Reserved);
                        if (reservHistory != null)
                        {
                            reservHistory.ObjectState = ObjectState.Modified;
                            reservHistory.Description += "/" + "ثبت حواله انبار و خروج از رزرو";
                            reservHistory.Status = MAssetReserveStatus.UnReserved;
                            selected.MovableAssetReserveHistories.Add(reservHistory);
                        }
                        _movableAssetService.InsertOrUpdateGraph(selected);
                        
                    }

                    _orderService.GetSubOrders(currentOrderDetails.OrderDetialsId).ForEach(sb =>
                    {
                        if (!currentOrderDetails.SubOrders.Contains(sb))
                        {
                            currentOrderDetails.SubOrders.Add(sb);
                        }
                    });

                    if (currentOrderDetails.SubOrders.All(so => so.State == SubOrderState.Deliviry)
                    && currentOrderDetails.State == OrderDetailsState.SubOrder)
                    {
                        currentOrderDetails.State = OrderDetailsState.Deliviry;
                        var history = new OrderUserHistory
                        {
                            UserDecision = true,
                            Description = "تحویل درخواست.توسط کاربر با نام:" + " " + UserLog.UniqueInstance.LogedUser.FullName + " " +
                              "در تاریخ:" + " " + GlobalClass._Today.PersianDateString(),
                            ObjectState = ObjectState.Added,
                            UserId = UserLog.UniqueInstance.LogedUser.UserId,
                        };
                        currentOrderDetails.OrderUserHistories.Add(history);
                        currentOrderDetails.ObjectState = ObjectState.Modified;
                    }
                    
                    if (!orders.Contains(order))
                    {
                        orders.Add(order);
                    }
                });

                if (belongingHasError)
                {
                    _dialogService.ShowError("توجه", msg);
                    return;
                }

                if (CurrentBill.StuffType == StuffType.Consumable)
                {
                    if (CurrentBill.Commodities.All(x => x.PlaceOfUses.Count>0))
                    {
                        CurrentBill.ObjectState = ObjectState.Modified;
                        _storeBillService.Update(CurrentBill);
                    }
                }
                else
                {
                    if (CurrentBill.MAssets.All(x => x.Locations.Any(l => l.Status == LocationStatus.Active)))
                    {
                        CurrentBill.ObjectState = ObjectState.Modified;
                        _storeBillService.Update(CurrentBill);
                    }
                }

                orders.ForEach(order =>
                {
                    if (order.OrderDetails.All(od => od.State == OrderDetailsState.Deliviry)
                    && order.Status != OrderStatus.Deliviry)
                    {
                        order.Status = OrderStatus.Deliviry;
                        order.DueDate = GlobalClass._Today;
                        order.ObjectState = ObjectState.Modified;
                    }
                    _orderService.InsertOrUpdateGraph(order);
                });

                try
                {
                    _unitOfWork.SaveChanges();
                    Mouse.SetCursor(Cursors.Arrow);
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    await this.getRelatedAssetsAsync();
                }
                catch (DbUpdateException ex)
                {
                    _dialogService.ShowError("خطا", ex.Message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private async void unReservedRequest()
        {
            if (_selectedAssets.Count <= 0)
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoRowSelected);
                return;
            }

            bool confirm = _dialogService.AskConfirmation("پرسش", "درخواست آزاد کردن این مال به مدیر فرستاده خواهد شد، و در صورت تایید مال به عنوان موجودی آزاد در انبار در خواهد آمد.آیا مطمئن به انجام این عملیات هستید");
            if (confirm)
            {
                _selectedAssets.ForEach(ma =>
                {
                    if(ma is Commodity)
                    {
                        var asset = ma as Commodity;
                        var reservHistory = asset.CommodityAssetReserveHistories
                     .FirstOrDefault(co => co.Status == MAssetReserveStatus.Reserved);
                        if (reservHistory != null)
                        {
                            reservHistory.ObjectState = ObjectState.Modified;
                            reservHistory.Description += "/" + "ثبت درخواست آزاد کردن مال برای انبار و خروج از رزرو";
                            reservHistory.Status = MAssetReserveStatus.UnReservedRequested;
                            asset.CommodityAssetReserveHistories.Add(reservHistory);
                        }
                        _commodityService.InsertOrUpdateGraph(asset);
                    }
                    else if(ma is MovableAsset)
                    {
                        var asset = ma as MovableAsset;
                       var reservHistory= asset.MovableAssetReserveHistories
                    .FirstOrDefault(co => co.Status == MAssetReserveStatus.Reserved);
                        if (reservHistory != null)
                        {
                            reservHistory.ObjectState = ObjectState.Modified;
                            reservHistory.Description +="/"+ "ثبت درخواست آزاد کردن مال برای انبار و خروج از رزرو";
                            reservHistory.Status = MAssetReserveStatus.UnReservedRequested;
                            asset.MovableAssetReserveHistories.Add(reservHistory);
                        }
                        _movableAssetService.InsertOrUpdateGraph(asset);
                    }
                   
                });

                try
                {
                    _unitOfWork.SaveChanges();
                    Mouse.SetCursor(Cursors.Arrow);
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    await this.getRelatedAssetsAsync();
                }
                catch (DbUpdateException ex)
                {
                    _dialogService.ShowError("خطا", ex.Message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void setAccountDocumentDetails(UnConsumption asset, Location loc, AccountDocumentMaster accountMaster)
        {
            var ma = asset;
            if (ma != null && loc != null)
            {
                List<Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>> currentAccountCodings = new List<Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>>();
                var accountCodings = _employeeService.GetAccountCodings();
                var parentcode = accountCodings.FirstOrDefault(x => x.Parent == null);

                string desc = "نامشخص";
                string code = "0";
                if (loc.AccountDocumentType == AccountDocumentType.StockToUnits)
                {
                    code = ma.KalaUid.ToString();
                    desc = ma.Name + "**" + ma.Cost.ToString("N0");
                    currentAccountCodings.Add(
                       new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.StockAssetBuyAndType),
                       "Creditor", desc, code, ma.Cost, ma));

                    var organization = _employeeService.GetParentNode(1)
                        .FirstOrDefault(x => x.BuidldingDesignId == loc.OrganizId);
                    if (organization != null)
                    {
                        var getItem = this.GetHirecharyNode(organization);
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
                    acNo = parentcode.AccountCode + "-" + adc.Item1.Parent.AccountCode + "-" + adc.Item1.AccountCode + "-" + GlobalClass.CheckAccountCode(adc.Item4);
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

        private void showDocWindow(bool isStoreBill)
        {
            Mouse.SetCursor(Cursors.Wait);
            var viewModel = new DocumentShowViewModel(_container, false);
            _navigationService.ShowDocumentShowWindow(viewModel);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showHitories(object parameter)
        {
            var selectedAsset = parameter as MovableAssetModel;
            if (selectedAsset == null)
            {
                return;
            }
            if (string.Equals(selectedAsset.MAssetType, "مصرفی"))
            {
                var reservers = _commodityService.GetReserveHistories(selectedAsset.AssetId).First();
                this.RequestHistories = reservers.Description.Split('/').ToList();
            }
            else
            {
                var reservers = _movableAssetService.GetReserveHistories(selectedAsset.AssetId).First();
                this.RequestHistories = reservers.Description.Split('/').ToList();
            }
        }
        #endregion

        #region commands

        public ICommand SelectCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand StoreDraftCommand { get; private set; }
        public ICommand UnReservedCommand { get; set; }
        public ICommand OrderDetailsHistoryCommand { get; private set; }
        private void initializCommand()
        {
            SelectCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.SelcectMAsset(parameter);
                },
                (parameter) =>
                {
                    return true;
                });

            SaveCommand = new MvvmCommand(
               (parameter) => { this.SubmitIndent(); },
               (parameter) => { return true; }
               );

            StoreDraftCommand = new MvvmCommand(
               (paramter) => { this.showDocWindow(false); },
               (paramter) => { return true; }
               );

            UnReservedCommand = new MvvmCommand(
               (paramter) => { this.unReservedRequest(); },
               (paramter) => { return true; }
               );

            OrderDetailsHistoryCommand = new MvvmCommand(
              (parameter) => { this.showHitories(parameter); },
             (parameter) => { return true; }
             );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDialogService _dialogService;
        private readonly IStoreBillService _storeBillService;
        private readonly INavigationService _navigationService;
        private readonly IOrderService _orderService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IMAssetCommodityService _commodityService;
        private readonly IEmployeeService _employeeService;
        private readonly IUnitService _unitService;
        private readonly ObservableCollection<MovableAssetModel> _collection;
        private readonly HashSet<PortableAsset> _selectedAssets;
        private readonly Dictionary<string, Document> _documentDictionary;

        #endregion
    }
}
