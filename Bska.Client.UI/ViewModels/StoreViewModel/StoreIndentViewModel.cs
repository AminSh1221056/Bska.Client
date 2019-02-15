
namespace Bska.Client.UI.ViewModels.StoreViewModel
{
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.Services;
    using Bska.Client.UI.Helper;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.API.Infrastructure;
    using Bska.Client.UI.API;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.ComponentModel;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Controls;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using System.Data.Entity.Infrastructure;
    using Domain.Entity.AssetEntity.CommodityAsset;
    using System.Data.Entity;

    public sealed class StoreIndentViewModel : BaseViewModel
    {
        #region ctor

        public StoreIndentViewModel(IUnityContainer container,SubOrderModel currentSod,int storeId)
        {
            this._container = container;
            this._storeDesignId = new List<int>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._storeService = _container.Resolve<IStoreService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._orderService = _container.Resolve<IOrderService>(new ParameterOverride("repository", _unitOfWork.Repository<Order>()));
            this._movableAssetService = _container.Resolve<IMovableAssetService>(new ParameterOverride("repository", _unitOfWork.Repository<MovableAsset>()));
            this._commodityService = _container.Resolve<IMAssetCommodityService>(new ParameterOverride("repository", _unitOfWork.Repository<Commodity>()));
            this._employeeService = _container.Resolve<IEmployeeService>();
            this._selectedCollection = new ObservableCollection<Tuple<Int64, Int64, PortableAsset>>();
            this._firistGeneration = new ObservableCollection<StoreTreeViewModel>();
            this._collection = new ObservableCollection<MovableAssetModel>();
            this.MAssetView = new CollectionViewSource { Source = _collection}.View;
            this._remainedOfOrder = new Dictionary<long, double>();
            this._unitHelper = new UnitHelper();
            this._labels = new HashSet<int>();
            initializObj(storeId,currentSod);
            this.initializCommand();
        }

        #endregion

        #region properties

        public SubOrderModel CurrentSubOrder
        {
            get { return GetValue(() => CurrentSubOrder); }
            set
            {
                SetValue(() => CurrentSubOrder, value);
                this.GetStoreMAssets();
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

        public List<Unit> Units
        {
            get { return GetValue(() => Units); }
            set
            {
                SetValue(() => Units, value);
            }
        }

        public Unit Unit
        {
            get { return GetValue(() => Unit); }
            set
            {
                SetValue(() => Unit, value);
            }
        }
        public ObservableCollection<StoreTreeViewModel> FirstGeneration
        {
            get { return _firistGeneration; }
        }

        public ObservableCollection<MovableAssetModel> Collection
        {
            get { return _collection; }
        }

        public ObservableCollection<Tuple<Int64, Int64, PortableAsset>> SelectedAssets
        {
            get { return _selectedCollection; }
        }

        public Double CurrentNum
        {
            get { return GetValue(() => CurrentNum); }
            set
            {
                SetValue(() => CurrentNum, value);
            }
        }

        public ICollectionView MAssetView { get; set; }

        public StoreTreeViewModel StoreDesignSelected
        {
            get { return GetValue(() => StoreDesignSelected); }
            set
            {
                SetValue(() => StoreDesignSelected, value);
                if (value != null)
                {
                    _storeDesignId.Clear();
                    GetAllStoreDesignId(value.StoreDesignCurrent);
                    FilterByStoreDesign();
                }
            }
        }

        [Required(ErrorMessage = "شماره حواله انبار الزامی است")]
        public String DraftNo
        {
            get { return _draftNo; }
            set
            {
                _draftNo = value;
                ValidateProperty(value);
                OnPropertyChanged("DraftNo");
            }
        }

        public String RemainDesc
        {
            get { return GetValue(() => RemainDesc); }
            set
            {
                SetValue(() => RemainDesc, value);
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

        public List<SubOrderModel> SubOrders
        {
            get { return GetValue(() => SubOrders); }
            set
            {
                SetValue(() => SubOrders, value);
            }
        }

        #endregion

        #region methods

        private async void initializObj(int sId, SubOrderModel currentSod)
        {
            Units = _unitService.Queryable().ToList();
            CurrentSubOrder = currentSod;
            if (CurrentSubOrder == null) return;
            int storeId = 0;
            int.TryParse(CurrentSubOrder.Identity, out storeId);
            if (storeId > 0)
            {
                Store = _storeService.Find(storeId);
                if (Store != null)
                {
                    _firistGeneration.Clear();
                    _storeService.GetParentNode(Store.StoreId).Where(x => x.ParentNode == null).ToList()
               .ForEach(x =>
               {
                   _rootNode = new StoreTreeViewModel(x, null, true);
                   _firistGeneration.Add(_rootNode);
               });
                }
            }
            RemainDesc = "باقیمانده:";
            this.SubOrders = await this.getRelatedSubOrdersAsync();
            DraftNo = await initDraftNoAsync(DocumentType.StoreInternalDraft);
        }

        private int generateLabel()
        {
            var maxLabel = 0;

            if (_labels.Count > 0)
            {
                maxLabel = _labels.Max();
            }
            else
            {
                var items = _movableAssetService.Queryable().OfType<UnConsumption>()
                    .Where(x => x.Label.HasValue).AsEnumerable();
                if (items.Count() > 0)
                {
                    maxLabel = items.Max(x => x.Label.Value);
                }
                _labels.Add(maxLabel);
            }
            return maxLabel;
        }

        private Task<List<SubOrderModel>> getRelatedSubOrdersAsync()
        {
            var ts = new Task<List<SubOrderModel>>(() =>
              {
                  return _orderService.GetRelatedSubOrders(Store.StoreId.ToString(), CurrentSubOrder.OrderDetailsId)
                  .ToList();
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
               var docs=_movableAssetService.GetDocuments(false).ToList();
               if (docs.Count() > 0)
               {
                   int maxVal = docs
                       .Select(d => int.TryParse(d.Desc1, out temp) ? temp : 0).Max();
                   draftNo = (maxVal + 1).ToString();
               }
               else
               {
                   draftNo ="1";
               }
               return draftNo;
            });
            ts.Start();
            return ts;
        }

        private String initDraftNo(DocumentType docType)
        {
            string draftNo = "";
            int temp;
            var docs = _movableAssetService.GetDocuments().ToList();
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

        private void GetStoreMAssets()
        {
            if (CurrentSubOrder == null) return;
            int storeId = 0;
            Unit = Units.SingleOrDefault(x => x.UnitId == CurrentSubOrder.UnitId);
            
            if (int.TryParse(CurrentSubOrder.Identity, out storeId))
            {
                _collection.Clear();
                var lstAssets = new List<MovableAssetModel>();
                if (CurrentSubOrder.StuffType == StuffType.Consumable)
                {
                  _commodityService.GetCommodityToDeliviryOrders(CurrentSubOrder.KalaUid,storeId,GlobalClass._Today).ToList().ForEach(co =>
                        {
                            if (co.PlaceOfUses.Count() > 0)
                            {
                                double memo = 0;
                                var coU = Units.First(v => v.UnitId == co.UnitId);
                                co.PlaceOfUses.ForEach(cop =>
                                {
                                    var copU = Units.First(u => u.UnitId == cop.UnitId);
                                    double coMemo = 0;
                                    if (coU.Equals(copU))
                                    {
                                        coMemo += cop.Num;
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
                                            coMemo = propertyVal;
                                        }
                                    }
                                    memo += coMemo;
                                });

                                if (!(memo >= co.Num))
                                {
                                    co.Num -= memo;

                                    var m = new MovableAssetModel
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
                                        ExpirationDate=co.ExpirationDate
                                    };
                                    lstAssets.Add(m);
                                }
                            }
                            else
                            {
                                var m = new MovableAssetModel
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
                                    ExpirationDate = co.ExpirationDate
                                };
                                lstAssets.Add(m);
                            }
                        });
                }
                else
                {
                    var mAssets = _movableAssetService.GetStoreMovableAssetWithoutProccedingByKalaUid(storeId, CurrentSubOrder.KalaUid);
                    mAssets.ForEach(m =>
                    {
                        lstAssets.Add(m);
                    });
                }

                lstAssets.ForEach(ma =>
                {
                    if (string.Equals(ma.MAssetType, "مصرفی"))
                    {
                        if (_selectedCollection.Select(co=>co.Item3).OfType<Commodity>().Any(v => v.AssetId == ma.AssetId))
                        {
                            Unit maU = Units.First(t => t.UnitId == ma.UnitId);
                            double cVal = 0;
                            _selectedCollection.Where(x => x.Item3.AssetId == ma.AssetId).ForEach(sl =>
                            {
                                Unit Ou = Units.First(t => t.UnitId == sl.Item3.UnitId);
                                if (Ou.Equals(maU))
                                {
                                    cVal += sl.Item3.Num;
                                }
                                else
                                {
                                    Unit isOrderChild = null;
                                    Unit IsPropertyChild = null;

                                    if (Ou != null)
                                    {
                                        isOrderChild = _unitHelper.mainparentRecovery(Ou);
                                    }

                                    if (maU != null)
                                    {
                                        IsPropertyChild = _unitHelper.mainparentRecovery(maU);
                                    }

                                    if (isOrderChild.Equals(IsPropertyChild))
                                    {
                                        Double orderval = _unitHelper.CalculateUnitNum(Ou, sl.Item3.Num);
                                        double propertyVal = _unitHelper.ReverseCalculateUnitNum(maU, orderval);
                                        cVal += propertyVal;
                                    }
                                    else
                                    {
                                        cVal += -1;
                                    }
                                }
                            });

                            if (!(cVal <= 0 || cVal >= ma.Num))
                            {
                                ma.Num -= cVal;
                                _collection.Add(ma);
                            }
                        }
                        else
                        {
                            _collection.Add(ma);
                        }
                    }
                    else
                    {
                        if (!_selectedCollection.Select(co => co.Item3).OfType<MovableAsset>().Any(v => v.AssetId == ma.AssetId))
                        {
                            _collection.Add(ma);
                        }
                    }
                });

                if (_remainedOfOrder.ContainsKey(CurrentSubOrder.SubOrderId))
                {
                    Remain = _remainedOfOrder[CurrentSubOrder.SubOrderId];
                    CurrentNum = CurrentSubOrder.Num - Remain;
                }
                else
                {
                    Remain = CurrentSubOrder.Remain;
                    CurrentNum = 0;
                }
            }
        }

        private void GetAllStoreDesignId(StoreDesign selectedItem)
        {
            if (selectedItem.ChildNode.Count > 0)
            {
                foreach (var k in selectedItem.ChildNode.AsParallel<StoreDesign>())
                {
                    this.GetAllStoreDesignId(k);
                }
            }
            _storeDesignId.Add(selectedItem.StoreDesignId);
        }
        
        private void FilterByStoreDesign()
        {
            MAssetView.Filter = (x =>
            {
                var item = x as MovableAssetModel;
                return item.StoreDesignId == StoreDesignSelected.StoreDesignCurrent.StoreDesignId;
            });
        }

        private void SelcectMAsset(object parameter)
        {
            Mouse.SetCursor(Cursors.Wait);

            var checkbox = parameter as CheckBox;
            if (checkbox == null) return;
            var selectedAsset = checkbox.Tag as MovableAssetModel;
            if (selectedAsset == null) return;

            PortableAsset mAsset = null;
           
            if (checkbox.IsChecked == true)
            {
                if (this.HasErrors)
                {
                    _dialogService.ShowError("خطا", "لطفا ورودی های خود را کنترل کنید");
                    return;
                }

                if (Remain<=0)
                {
                    _dialogService.ShowAlert("توجه", "شما به مقدار سفارش مال را تامیین کرده اید");
                    checkbox.IsChecked = false;
                    return;
                }

                double pVal = 0;
                if (string.Equals(selectedAsset.MAssetType,"مصرفی"))
                {
                    mAsset = _commodityService.Query(x => x.AssetId == selectedAsset.AssetId).Select().Single();
                    var coMa = _collection.First(v => v.AssetId == mAsset.AssetId);
                    mAsset.Num = coMa.Num;

                    var mAssetUnit = _unitService.Find(mAsset.UnitId);
                    if (Unit.Equals(mAssetUnit))
                    {
                        if (mAsset.Num > Remain)
                        {
                            Boolean confrim = _dialogService.AskConfirmation("پرسش", "به مقدار" + " " + Remain + " " + Unit.Name + " " + "از این مال کم خواهد شد.آیا می خواهید ادامه دهید");
                            if (!confrim)
                            {
                                checkbox.IsChecked = false;
                                return;
                            }
                            CurrentNum += Remain;
                            pVal = Remain;
                        }
                        else
                        {
                            if ((CurrentNum + mAsset.Num) > Remain)
                            {
                                double memo = (Remain - CurrentNum);
                                Boolean confrim = _dialogService.AskConfirmation("پرسش", "به مقدار" + " " + memo + " " + Unit.Name + " " + "از این مال کم خواهد شد.آیا می خواهید ادامه دهید");
                                if (!confrim)
                                {
                                    checkbox.IsChecked = false;
                                    return;
                                }

                                CurrentNum += memo;
                                pVal = memo;
                            }
                            else
                            {
                                CurrentNum += mAsset.Num;
                                pVal = mAsset.Num;
                            }
                        }
                    }
                    else
                    {
                        var orderUnit = Units.SingleOrDefault(x => x.UnitId == CurrentSubOrder.UnitId);
                        var propertyUnit = Units.SingleOrDefault(x => x.UnitId == mAsset.UnitId);

                        Unit isOrderChild = null;
                        Unit IsPropertyChild = null;

                        if (orderUnit != null)
                        {
                            isOrderChild = _unitHelper.mainparentRecovery(orderUnit);
                        }

                        if (propertyUnit != null)
                        {
                            IsPropertyChild = _unitHelper.mainparentRecovery(propertyUnit);
                        }

                        if (isOrderChild.Equals(IsPropertyChild))
                        {
                            Double orderval = _unitHelper.CalculateUnitNum(orderUnit, Remain);
                            Double propertyval = _unitHelper.CalculateUnitNum(propertyUnit, mAsset.Num);
                            if (propertyval > orderval)
                            {
                                Double memo = _unitHelper.ReverseCalculateUnitNum(propertyUnit, orderval);
                                Boolean confrim = _dialogService.AskConfirmation("پرسش", "به مقدار" + " " + memo + " " + propertyUnit.Name + " " + "از این مال کم خواهد شد.آیا می خواهید ادامه دهید");
                                if (!confrim)
                                {
                                    checkbox.IsChecked = false;
                                    return;
                                }
                                CurrentNum += Remain;
                                pVal = Remain;
                            }
                            else
                            {
                                Double val = _unitHelper.ReverseCalculateUnitNum(orderUnit, propertyval);
                                if ((CurrentNum + val) > Remain)
                                {
                                    double val1 = _unitHelper.CalculateUnitNum(orderUnit, (Remain - CurrentNum));
                                    double memo = _unitHelper.ReverseCalculateUnitNum(propertyUnit, val1);
                                    Boolean confrim = _dialogService.AskConfirmation("پرسش", "به مقدار" + " " + memo + " " + propertyUnit.Name + " " + "از این مال کم خواهد شد.آیا می خواهید ادامه دهید");
                                    if (!confrim)
                                    {
                                        checkbox.IsChecked = false;
                                        return;
                                    }

                                    CurrentNum += (Remain - CurrentNum);
                                    pVal = (Remain - CurrentNum);
                                }
                                else
                                {
                                    CurrentNum = CurrentNum + val;
                                    pVal = val;
                                }
                            }
                        }
                        else
                        {
                            _dialogService.ShowAlert("توجه", "واحد مال هیچ رابطه ای با واحد در خواست ندارد");
                            checkbox.IsChecked = false;
                            return;
                        }
                    }
                }
                else
                {
                    mAsset = _movableAssetService.Query(x => x.AssetId == selectedAsset.AssetId)
                        .Include(x => x.StoreBill).Select().Single();
                    var mAssetUnit = _unitService.Find(mAsset.UnitId);

                    if (string.Equals(selectedAsset.MAssetType,"غیرمصرفی"))
                    {
                        Boolean hasBelonging =false;
                        var unconsum = mAsset as UnConsumption;
                        hasBelonging = _movableAssetService.ContainBelongings(mAsset.AssetId);
                        if (hasBelonging)
                        {
                            Boolean belongingConfirm = _dialogService.AskConfirmation("پرسش", "این مال دارای اموال متعلقه می باشد.تمام اموال متعلقه نیز همراه مال انتقال میابند.آیا میخواهید ادامه دهید");
                            if (!belongingConfirm)
                            {
                                checkbox.IsChecked = false;
                                return;
                            }

                            var belongs = _movableAssetService.GetBelongingsToLocation(mAsset.AssetId);

                            belongs.ForEach(b =>
                            {
                                b.StoreBill = _movableAssetService.GetStoreBill(b.AssetId);
                                if (!b.Label.HasValue && b.StoreBill.AcqType != StateOwnership.Trust)
                                {
                                    int label = generateLabel() + 1;
                                    b.Label = label;
                                    _labels.Add(label);
                                    b.ObjectState = ObjectState.Modified;
                                }

                                if (!unconsum.Belongings.Contains(b))
                                {
                                    unconsum.Belongings.Add(b);
                                }
                            });
                        }
                        var ma = mAsset as MovableAsset;
                        if (!ma.Label.HasValue && ma.StoreBill.AcqType != StateOwnership.Trust)
                        {
                            int lable = generateLabel() + 1;
                            ma.Label = lable;
                            _labels.Add(lable);
                            ma.ObjectState = ObjectState.Modified;
                        }
                    }
                    else if(string.Equals(selectedAsset.MAssetType, "متعلقات"))
                    {
                        var parent = _movableAssetService.GetBelongingParnet(mAsset.AssetId);
                        if (parent != null)
                        {
                            Boolean askForFreeParent = _dialogService.AskConfirmation("توجه", "مال انتخاب شده متعلقه و دارای مال اصلی می باشد.آیا میخواهید از مال اصلی خود جدا و به مال اصلی در درخواست اضافه شود");
                            if (!askForFreeParent)
                            {
                                checkbox.IsChecked = false;
                                return;
                            }
                        }
                    }

                    CurrentNum += 1;
                    pVal = 1;
                }

                if (mAsset != null)
                {
                   addToList(mAsset, pVal);
                }
            }
            else
            {
                var item = _selectedCollection.First(x => x.Item1 == selectedAsset.AssetId && x.Item2 == CurrentSubOrder.SubOrderId);
                CurrentNum -= item.Item3.Num;
                Remain += item.Item3.Num;
                if (_remainedOfOrder.ContainsKey(CurrentSubOrder.SubOrderId))
                {
                    _remainedOfOrder[CurrentSubOrder.SubOrderId] = Remain;
                }

                _selectedCollection.Remove(item);
            }
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void addToList(PortableAsset sel,double val)
        {
            Mouse.SetCursor(Cursors.Wait);
           
            if (sel is MovableAsset)
            {
                var selected = sel as MovableAsset;
                _selectedCollection.Add(new Tuple<long, long, PortableAsset>(selected.AssetId, CurrentSubOrder.SubOrderId, selected));
            }
            else if (sel is Commodity)
            {
                var item = sel as Commodity;
                var selected = item.Clone();
                selected.Num = val;
                selected.UnitId = CurrentSubOrder.UnitId;
                selected.ObjectState = ObjectState.Unchanged;
                _selectedCollection.Add(new Tuple<long, long, PortableAsset>(selected.AssetId, CurrentSubOrder.SubOrderId, selected));
            }

            Remain -= val;

            if (_remainedOfOrder.ContainsKey(CurrentSubOrder.SubOrderId))
            {
                _remainedOfOrder[CurrentSubOrder.SubOrderId] = Remain;
            }
            else
            {
                _remainedOfOrder.Add(CurrentSubOrder.SubOrderId, Remain);
            }
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void SubmitIndent()
        {
            if (_selectedCollection.Count <= 0)
            {
                _dialogService.ShowAlert("توجه",ErrorMessages.Default.NoRowSelected);
                return;
            }

            Boolean confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                Mouse.SetCursor(Cursors.Wait);
                AccountDocumentMaster accountMaster = null;
                Document document = null;
                Document trustDocument = _currentTrustDoc;

                if (_currentDoc == null)
                {
                    document = new Document
                    {
                        Desc1 = this.DraftNo,
                        DocumentDate = GlobalClass._Today,
                        Desc2 = this.Store.StoreId.ToString(),
                        DocumentType = DocumentType.StoreInternalDraft,
                        ObjectState = ObjectState.Added,
                        StoreId = Store.StoreId,
                    };
                    _currentDoc = document;
                }
                else
                {
                    document = _currentDoc;
                    accountMaster = document.AccountDocument;
                }

                Boolean belongingHasError = false;
                var orders = new HashSet<Order>();
                _selectedCollection.ForEach(sl =>
                {
                    var csod = SubOrders.First(v => v.SubOrderId == sl.Item2);
                    var order = _orderService.Query(o => o.OrderId ==csod.OrderId)
               .Include(o => o.OrderDetails).Include(p => p.Person).Select().Single();

                    var subOrder = _orderService.GetSubOrders(csod.OrderDetailsId)
                        .Single(so => so.SubOrderId == sl.Item2);

                    var currentOrderDetails = order.OrderDetails.First(x => x.OrderDetialsId == subOrder.OrderDetailsId);
                    if (sl.Item3 is Commodity)
                    {
                       var com = _commodityService.Queryable().Where(v => v.AssetId == sl.Item3.AssetId).Include(v => v.PlaceOfUses).Single() ;
                       com.PlaceOfUses.Add( new PlaceOfUse
                        {
                           ObjectState=ObjectState.Added,
                            Document = document,
                            Num = sl.Item3.Num,
                            OrganizId = currentOrderDetails.OrganizId.Value,
                            StrategtyId = currentOrderDetails.StrategyId.Value,
                            UnitId = sl.Item3.UnitId,
                            InsertDate = GlobalClass._Today,
                            PersonId=order.Person.NationalId,
                       });

                        order.Commodities.Add(com);
                    }
                    else
                    {
                        MovableAsset selected = sl.Item3 as MovableAsset;
                        var location = _movableAssetService.GetLocation(selected.AssetId, isInStore: true);

                        location.ObjectState = ObjectState.Modified;
                        location.Status = LocationStatus.StoreDeActive;
                        location.MovedRequestDate = GlobalClass._Today;
                        location.ReturnDate = GlobalClass._Today;

                        Location newLocation = null;
                        if (selected.StoreBill.AcqType == StateOwnership.Trust)
                        {
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
                                AccountDocumentType = AccountDocumentType.None,
                            };

                            if (trustDocument == null)
                            {
                                trustDocument = new Document
                                {
                                    Desc1 = initDraftNo(DocumentType.InternalStoreTrustDraft),
                                    DocumentDate = GlobalClass._Today,
                                    DocumentType = DocumentType.InternalStoreTrustDraft,
                                    ObjectState = ObjectState.Added,
                                    StoreId = Store.StoreId,
                                    Transferee = $"{order.Person.FirstName} {order.Person.LastName}"
                                };
                                _currentTrustDoc = trustDocument;
                            }
                            selected.Documetns.Add(trustDocument);
                        }
                        else
                        {
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
                        }

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

                                if (b.StoreBill.AcqType == StateOwnership.Trust)
                                {
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
                                        AccountDocumentType = AccountDocumentType.None,
                                    };

                                    if (trustDocument == null)
                                    {
                                        trustDocument = new Document
                                        {
                                            Desc1 = initDraftNo(DocumentType.InternalStoreTrustDraft),
                                            DocumentDate = GlobalClass._Today,
                                            Desc2 = this.Store.StoreId.ToString(),
                                            DocumentType = DocumentType.InternalStoreTrustDraft,
                                            ObjectState = ObjectState.Added,
                                            StoreId = Store.StoreId
                                        };
                                    }
                                    b.Documetns.Add(trustDocument);
                                }
                                else
                                {
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
                                }
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

                        order.MovableAssets.Add(selected);
                    }

                    SubOrderState sbState = subOrder.State;
                    subOrder.Remain = _remainedOfOrder[subOrder.SubOrderId];
                    if (subOrder.Remain <= 0)
                    {
                        sbState = SubOrderState.Deliviry;
                    }

                    subOrder.State = sbState;
                    subOrder.ObjectState = ObjectState.Modified;
                    currentOrderDetails.SubOrders.Add(subOrder);

                    if (currentOrderDetails.SubOrders.All(so => so.State == SubOrderState.Deliviry)
                    && currentOrderDetails.State==OrderDetailsState.SubOrder)
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
                    }
                    currentOrderDetails.ObjectState = ObjectState.Modified;
                    order.OrderDetails.Add(currentOrderDetails);

                    if (order.OrderDetails.All(od => od.State == OrderDetailsState.Deliviry)
                    && order.Status!=OrderStatus.Deliviry)
                    {
                        order.Status = OrderStatus.Deliviry;
                        order.DueDate = GlobalClass._Today;
                        order.ObjectState = ObjectState.Modified;
                        document.Transferee = $"{order.Person.FirstName} {order.Person.LastName}";
                        orders.Add(order);
                    }
                });

                if (belongingHasError)
                {
                    _dialogService.ShowError("توجه", "خطا درمورد اموال متعلقه در سیستم رخ داد.لطفا به مدیر سیستم مراجعه کنید");
                    return;
                }

                try
                {
                    _orderService.InsertGraphRange(orders);
                    _unitOfWork.SaveChanges();
                    Mouse.SetCursor(Cursors.Arrow);
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
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

        private void showDocWindow()
        {
            Mouse.SetCursor(Cursors.Wait);
           // var window = parameter as Window;
            //if (window == null) return;
           // StoryboardManager.PlayStoryboard("StoryboardHideWindow", window);
            var doc = _movableAssetService
                    .GetDocument(DraftNo);
            var viewModel = new DocumentShowViewModel(_container, false);
            _navigationService.ShowDocumentShowWindow(viewModel);
           // StoryboardManager.PlayStoryboard("StoryboardShowWindow", window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region commands

        public ICommand SaveCommand { get; private set; }
        public ICommand SelectCommand { get; private set; }
        public ICommand StoreDraftCommand { get; private set; }
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
                (parameter) => { this.showDocWindow(); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IStoreService _storeService;
        private readonly IEmployeeService _employeeService;
        private readonly IUnitService _unitService;
        private readonly IOrderService _orderService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IMAssetCommodityService _commodityService;
        private readonly ObservableCollection<StoreTreeViewModel> _firistGeneration;
        private readonly ObservableCollection<MovableAssetModel> _collection;
        private readonly ObservableCollection<Tuple<Int64,Int64,PortableAsset>> _selectedCollection;
        private readonly List<Int32> _storeDesignId;
        private readonly Dictionary<Int64, Double> _remainedOfOrder;
        private readonly UnitHelper _unitHelper;
        private readonly HashSet<int> _labels;
        private string _draftNo;
        private Document _currentDoc;
        private StoreTreeViewModel _rootNode;
        private Document _currentTrustDoc;

        #endregion
    }
}
