
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.Services;
    using Bska.Client.UI.ViewModels.AssetViewModel;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;
    using System.Linq;
    using Bska.Client.API.UnitOfWork;
    using System.Data.Entity.Infrastructure;
    using Bska.Client.API.Infrastructure;
    using System.Transactions;
    using System.Windows;
    using System.Threading.Tasks;
    using Bska.Client.Domain.Entity.StoredProcedures;
    using System.Data.Entity;

    public sealed class MovableAssetDetailsViewModel : BaseViewModel
    {
        #region ctor

        public MovableAssetDetailsViewModel(IUnityContainer container, Int64 assetId,MovableAsset asset=null,
            Boolean isEditable = false,Boolean isRealAsset=true)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._employeeService = _container.Resolve<IEmployeeService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>(new ParameterOverride("repository", _unitOfWork.Repository<MovableAsset>()));
            this._storeBillService = _container.Resolve<IStoreBillService>(new ParameterOverride("repository", _unitOfWork.Repository<StoreBill>()));
            this._personService = _container.Resolve<IPersonService>();
            this._beskaProcedures = _container.Resolve<IBskaStoredProcedures>();
            this._stuffService = _container.Resolve<IStuffService>();
            this._storeService = _container.Resolve<IStoreService>();
            this._sellerService = _container.Resolve<ISellerService>();
            this._isEditable = isEditable;
            this._isRealasset = isRealAsset;
            this._stuffHelper = new StuffHelper();
            this.CurrentAsset = asset;
            this.initalizObj(assetId);
            this.initializCommands();
        }

        #endregion

        #region properties

        public String BuildingName
        {
            get { return GetValue(() => BuildingName); }
            set
            {
                SetValue(() => BuildingName, value);
            }
        }

        public String OrganizPath
        {
            get { return GetValue(() => OrganizPath); }
            set
            {
                SetValue(() => OrganizPath, value);
            }
        }

        public String StraegyPath
        {
            get { return GetValue(() => StraegyPath); }
            set
            {
                SetValue(() => StraegyPath, value);
            }
        }

        public string StoreBillNo
        {
            get { return GetValue(() => StoreBillNo); }
            set
            {
                SetValue(() => StoreBillNo, value);
            }
        }

        public string StoreBillType
        {
            get { return GetValue(() => StoreBillType); }
            set
            {
                SetValue(() => StoreBillType, value);
            }
        }

        public String StoreBillDate
        {
            get { return GetValue(() => StoreBillDate); }
            set
            {
                SetValue(() => StoreBillDate, value);
            }
        }

        public UnConsumptionViewModel UnConsumptionViewModel
        {
            get { return GetValue(() => UnConsumptionViewModel); }
            private set
            {
                SetValue(() => UnConsumptionViewModel, value);
            }
        }

        public InstallableViewModel InstallableViewModel
        {
            get { return GetValue(() => InstallableViewModel); }
            set
            {
                SetValue(() => InstallableViewModel, value);
            }
        }

        public BelongingViewModel BelongingViewModel
        {
            get { return GetValue(() => BelongingViewModel); }
            set
            {
                SetValue(() => BelongingViewModel, value);
            }
        }
        
        public InCommodityViewModel InCommodityViewModel
        {
            get { return GetValue(() => InCommodityViewModel); }
            private set
            {
                SetValue(() => InCommodityViewModel, value);
            }
        }

        public MovableAsset CurrentAsset
        {
            get { return GetValue(() => CurrentAsset); }
            set
            {
                SetValue(() => CurrentAsset, value);
            }
        }

        public List<MovableAsset> AllMAsset
        {
            get { return GetValue(() => AllMAsset); }
            set
            {
                SetValue(() => AllMAsset, value);
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

        public List<Stuff> Stuffs
        {
            get { return GetValue(() => Stuffs); }
            set
            {
                SetValue(() => Stuffs, value);
            }
        }

        public Stuff SelectedStuff
        {
            get { return GetValue(() => SelectedStuff); }
            set
            {
                SetValue(() => SelectedStuff, value);
                if (value != null)
                {
                    CurrentAsset.Name = SelectedStuff.Name;
                    CurrentAsset.KalaUid = SelectedStuff.StuffId;
                }
            }
        }

        public Int32 PStuffId
        {
            get;
            set;
        }

        public Boolean IsEditableAsset
        {
            get { return GetValue(() => IsEditableAsset); }
            set
            {
                SetValue(() => IsEditableAsset, value);
            }
        }

        public double Num
        {
            get { return GetValue(() => Num); }
            set
            {
                SetValue(() => Num, value);
            }
        }

        public Int32 Year
        {
            get { return GetValue(() => Year); }
            set
            {
                SetValue(() => Year, value);
            }
        }

        public Boolean HaveBelonging
        {
            get { return GetValue(() => HaveBelonging); }
            set
            {
                SetValue(() => HaveBelonging, value);
            }
        }

        public Int32 RequestPermitId
        {
            get { return GetValue(() => RequestPermitId); }
            set
            {
                if (value != RequestPermitId)
                {
                    SetValue(() => RequestPermitId, value);
                    this.getPersonPermitsAsync(null, value);
                }
            }
        }

        public string PersonName
        {
            get { return GetValue(() => PersonName); }
            set
            {
                SetValue(() => PersonName, value);
            }
        }

        public Boolean ChangeLocation
        {
            get { return GetValue(() => ChangeLocation); }
            set
            {
                SetValue(() => ChangeLocation, value);
            }
        }
        
        #endregion

        #region methods

        private async void initalizObj(Int64 assetId)
        {
            if (assetId > 0)
            {
                CurrentAsset = _movableAssetService.Queryable().Where(ma => ma.AssetId == assetId)
                   .Include(ma => ma.StoreBill).Include(ma => ma.Locations)
                   .Include(ma => ma.Documetns).SingleOrDefault();
            }

            if (CurrentAsset == null) return;
            ChangeLocation= HaveBelonging = false;
            AllMAsset = new List<MovableAsset> { CurrentAsset };

            if (CurrentAsset is UnConsumption)
            {
                _stufftype = StuffType.UnConsumption;
                UnConsumptionViewModel = new UnConsumptionViewModel(CurrentAsset as UnConsumption) { OldLabelEditable = true };
                this.Num = 1;
                if (CurrentAsset.StoreBill != null)
                {
                    this.Year = CurrentAsset.StoreBill.ArrivalDate.PersianDateTime().Year;
                }
                HaveBelonging = _movableAssetService.ContainBelongings(CurrentAsset.AssetId);
            }
            else if(CurrentAsset is Installable)
            {
                InstallableViewModel = new InstallableViewModel(CurrentAsset as Installable);
                this.Num = 1;
                _stufftype = StuffType.Installable;
            }
            else if(CurrentAsset is Belonging)
            {
                var belong = CurrentAsset as Belonging;
                BelongingViewModel = new BelongingViewModel(CurrentAsset as Belonging);
                if (belong.ParentMAsset == null)
                {
                    belong.ParentMAsset = _movableAssetService.GetBelongingParnet(CurrentAsset.AssetId);
                }
                BelongingViewModel.Parent = belong.ParentMAsset;
                this.Num = 1;
                _stufftype = StuffType.Belonging;
            }
            else if (CurrentAsset is InCommidity)
            {
                InCommodityViewModel = new InCommodityViewModel(CurrentAsset as InCommidity);
                this.Num = 1;
                _stufftype = StuffType.OrderConsumption;
            }

            var stuff = _stuffService.Query(x => x.KalaNo == CurrentAsset.KalaNo)
                     .Include(x => x.Parent).Select().SingleOrDefault();
            if (stuff != null)
            {
                if (stuff.Parent != null)
                {
                    PStuffId = stuff.Parent.StuffId;
                    this.Units = _unitService.Queryable().Where(x => x.StuffId == _stufftype || x.StuffId == StuffType.None).ToList();
                }
            }

            if (CurrentAsset.StoreBill != null)
            {
                StoreBillNo = "شماره قبض انبار : " + CurrentAsset.StoreBill.StoreBillNo.ToString();
                StoreBillType = "نوع قبض انبار : " + CurrentAsset.StoreBill.AcqType.GetDescription();
                StoreBillDate = "تاریخ : " + CurrentAsset.StoreBill.ArrivalDate.PersianDateString();
            }

            Location location = null;
            location = CurrentAsset.Locations.OrderBy(x=>x.LocationId).LastOrDefault();

            if (location != null)
            {
                if (location.Status == LocationStatus.Active || location.Status == LocationStatus.MovedRequest)
                {
                    CurrentAsset.IsInStore = false;
                    this.getPersonPermitsAsync(location,null);
                }
                else if (location.Status == LocationStatus.StoreActive)
                {
                    CurrentAsset.IsInStore = true;
                    var store = _storeService.Find(location.StoreId);
                    var storeAddress = _storeService.GetParentNode(location.StoreId)
                        .Where(x => x.StoreDesignId == location.StoreAddressId).SingleOrDefault();
                    BuildingName = "نام انبار : " + store.Name;
                    if (storeAddress != null)
                    {
                        OrganizPath = "آدرس انبار : " + storeGetHirecharyNode(storeAddress);
                    }
                }
                else
                {
                    BuildingName = "صورت جلسه";
                    OrganizPath = "مال از لیست موجودی سازمان خارج شده است";
                }
            }

            SelectedStuff = stuff;
            if (_isEditable)
            {
                if (!CurrentAsset.ISConfirmed)
                {
                    if (CurrentAsset.Documetns.Any(d => d.DocumentType == DocumentType.InitialBalance))
                    {
                        _isOldSystem = true;
                    }
                    
                    Stuffs = await this.getStuffAsync(_stufftype);
                    await this.getOldLabelsByFloorAsync();
                }
                else
                {
                    if (_isRealasset)
                    {
                        Stuffs = new List<Stuff> { SelectedStuff };
                    }
                    else
                    {
                        Stuffs = await this.getStuffAsync(_stufftype);
                    }
                }
                IsEditableAsset = true;
            }
            else
            {
                IsEditableAsset = false;
                Stuffs = new List<Stuff> { SelectedStuff };
            }
        }

        private Task<List<Stuff>> getStuffAsync(StuffType sType)
        {
          var ts=new Task<List<Stuff>>(() =>
            {
                if (sType == StuffType.OrderConsumption)
                {
                    return _stuffService.Query(s => (s.StuffType == StuffType.UnConsumption || s.StuffType==StuffType.OrderConsumption) && s.IsStuff == true).Include(s => s.Parent).Select().ToList();
                }
                else
                {
                    return _stuffService.Query(s => s.StuffType == sType && s.IsStuff == true).Include(s => s.Parent).Select().ToList();
                }
            });
            ts.Start();
            return ts;
        }

        private async void getPersonPermitsAsync(Location lastLocation,int? permitId)
        {
           await Task.Run(() =>
            {
                Person person = null;
                _permits= _personService.GetAllPermits().ToList();
                RequestPermit permit = null;
                if (permitId.HasValue && ChangeLocation)
                {
                    permit = _permits.FirstOrDefault(x => x.RequestPermitId == permitId.Value);
                    if (permit != null)
                    {
                        person = _personService.Find(permit?.PersonId);
                        var oldLoc = CurrentAsset.Locations.OrderBy(x => x.LocationId).LastOrDefault();
                        var newLoc = new Location
                        {
                            AccountDocumentType = oldLoc.AccountDocumentType,
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            StrategyId = permit.StrategyId,
                            OrganizId = permit.OrganizId,
                            PersonId = person.NationalId,
                            Status = oldLoc.Status,
                        };
                        _NewLocation = newLoc;
                    }
                    else
                    {
                        _dialogService.ShowAlert("توجه", "موقعیت با کد وارد شده یافت نشد");
                    }
                }
                else
                {
                    if (lastLocation != null)
                    {
                        person = _personService.Queryable().Where(x => x.NationalId == lastLocation.PersonId).Single();
                        permit = _permits.Where(x => x.OrganizId == lastLocation.OrganizId && x.StrategyId == lastLocation.StrategyId
                          && x.PersonId == person.PersonId).FirstOrDefault();
                    }
                }

                if (permit != null)
                {
                    if (RequestPermitId != permit.RequestPermitId)
                    {
                        var organiz = _employeeService.GetParentNode(1).Where(x => x.BuidldingDesignId
                                == permit.OrganizId).SingleOrDefault();


                        var strategy = _employeeService.GetParentNode(2).Where(x => x.BuidldingDesignId
                             == permit.StrategyId).SingleOrDefault();

                        DispatchService.Invoke(() =>
                        {
                            if (organiz != null)
                            {
                                OrganizPath = "سازمانی : " + GetHirecharyNode(organiz);
                            }

                            if (strategy != null)
                            {
                                StraegyPath = "استراتژیکی : " + GetHirecharyNode(strategy);
                            }
                            RequestPermitId = permit.RequestPermitId;
                            PersonName = $"{person.FirstName} {person.LastName}";
                        });
                    }
                }
            });
        }

        private String GetHirecharyNode(EmployeeDesign item)
        {
            String _nodeName = "";

            if (item.ParentNode != null)
            {
                _nodeName += this.GetHirecharyNode(item.ParentNode);
                _nodeName += "***";
            }

            _nodeName += item.Name;

            return _nodeName;
        }

        private string storeGetHirecharyNode(StoreDesign item)
        {
            String _nodeName = "";

            if (item.ParentNode != null)
            {
                _nodeName += this.storeGetHirecharyNode(item.ParentNode);
                _nodeName += "***";
            }

            _nodeName += item.Name;

            return _nodeName;
        }

        //...ambient transaction should not be used to EF...but there is exception
        private void editCurrentAsset(object parameter)
        {
            if (CurrentAsset == null) return;
            var window = parameter as Window;
            
            if(CurrentAsset is UnConsumption)
            {
                if (!this.UnConsumptionViewModel.CheckErrors())
                {
                    _dialogService.ShowError("توجه", ErrorMessages.Default.InputInvalid);
                    return;
                }
            }

            bool confirm = _dialogService.AskConfirmation("پرسش",ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                if (_isRealasset)
                {
                    var option = new TransactionOptions();
                    option.IsolationLevel=IsolationLevel.ReadCommitted;
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, option))
                    {
                        if (CurrentAsset is UnConsumption)
                        {
                            if (UnConsumptionViewModel.HasErrors)
                            {
                                _dialogService.ShowError("خطا", ErrorMessages.Default.InputInvalid);
                                return;
                            }

                            if (_isOldSystem)
                            {
                                if (CurrentAsset.Cost < APPSettings.Default.MStuffPrice)
                                {
                                    var unconsum = CurrentAsset as UnConsumption;
                                    var inComodity = unconsum.ToInCommidity();

                                    List<Location> locations = null;
                                    if (_NewLocation != null)
                                    {
                                        var oldLoc = CurrentAsset.Locations.OrderBy(l => l.LocationId).Last();
                                        locations = CurrentAsset.Locations.Where(x=>x.LocationId!=oldLoc.LocationId).ToList();
                                        locations.Add(_NewLocation);
                                    }
                                    else
                                    {
                                        locations = CurrentAsset.Locations.ToList();
                                    }
                                    var docs = CurrentAsset.Documetns.ToList();
                                    StoreBill sBill = _storeBillService.Queryable()
                                .Where(x => x.StoreBillNo == "2" && x.ArrivalDate.Year == CurrentAsset.StoreBill.ArrivalDate.Year
                                && x.StuffType == StuffType.OrderConsumption)
                                .SingleOrDefault();

                                    if (sBill == null)
                                    {
                                        sBill = new StoreBill
                                        {
                                            AcqType = StateOwnership.Purchase,
                                            ArrivalDate = CurrentAsset.StoreBill.ArrivalDate,
                                            ObjectState = ObjectState.Added,
                                            ModifiedDate = GlobalClass._Today,
                                            StoreBillNo = "2",
                                            StoreId = CurrentAsset.StoreBill.StoreId,
                                            Desc1 = CurrentAsset.StoreBill.Desc1,
                                            Desc2 = CurrentAsset.StoreBill.Desc2,
                                            Desc3 = CurrentAsset.StoreBill.Desc3,
                                            StuffType = StuffType.OrderConsumption,
                                        };
                                        inComodity.StoreBill = sBill;
                                    }
                                    else
                                    {
                                        inComodity.StoreBillId = sBill.StoreBillId;
                                    }
                                    
                                    locations.ForEach(l =>
                                    {
                                        inComodity.Locations.Add(new Location
                                        {
                                            AccountDocumentType = l.AccountDocumentType,
                                            InsertDate = l.InsertDate,
                                            MovedRequestDate = l.MovedRequestDate,
                                            ReturnDate = l.ReturnDate,
                                            ObjectState = ObjectState.Added,
                                            StrategyId = l.StrategyId,
                                            OrganizId = l.OrganizId,
                                            PersonId = l.PersonId,
                                            StoreId = l.StoreId,
                                            StoreAddressId = l.StoreAddressId,
                                            Status = l.Status,
                                        });
                                    });

                                    docs.ForEach(d =>
                                    {
                                        d.MovableAsset.Add(inComodity);
                                    });
                                    _beskaProcedures.MAsset_Delete(CurrentAsset.AssetId,(int)_stufftype, CurrentAsset.Label);
                                    inComodity.ObjectState = ObjectState.Added;
                                    _movableAssetService.InsertOrUpdateGraph(inComodity);
                                }
                                else
                                {
                                    if (_NewLocation != null)
                                    {
                                        var oldLoc = CurrentAsset.Locations.OrderBy(l => l.LocationId).Last();
                                        oldLoc.ObjectState = ObjectState.Deleted;
                                        CurrentAsset.Locations.Add(_NewLocation);
                                        CurrentAsset.Locations.Remove(oldLoc);
                                    }

                                    CurrentAsset.ObjectState = ObjectState.Modified;
                                    _movableAssetService.Update(CurrentAsset);
                                }
                            }
                            else
                            {
                                if (_NewLocation != null)
                                {
                                    var oldLoc = CurrentAsset.Locations.OrderBy(l => l.LocationId).Last();
                                    oldLoc.ObjectState = ObjectState.Deleted;
                                    CurrentAsset.Locations.Add(_NewLocation);
                                    CurrentAsset.Locations.Remove(oldLoc);
                                }

                                CurrentAsset.ObjectState = ObjectState.Modified;
                                _movableAssetService.Update(CurrentAsset);
                            }
                        }
                        else if (CurrentAsset is InCommidity)
                        {
                            if (InCommodityViewModel.HasErrors)
                            {
                                _dialogService.ShowError("خطا", ErrorMessages.Default.InputInvalid);
                                return;
                            }

                            if (_isOldSystem)
                            {
                                if (CurrentAsset.Cost >= APPSettings.Default.MStuffPrice)
                                {
                                    var unconsumption = ((InCommidity)CurrentAsset).ToUnconsumpton();
                                    var labels = _movableAssetService.Queryable().Where(x => x.Label.HasValue)
                                        .Select(x => x.Label.Value).ToList();
                                    if (labels.Count() > 0)
                                    {
                                        unconsumption.Label = labels.Max() + 1;
                                    }
                                    else
                                    {
                                        unconsumption.Label = 1;
                                    }

                                    List<Location> locations = null;
                                    if (_NewLocation != null)
                                    {
                                        var oldLoc = CurrentAsset.Locations.OrderBy(l => l.LocationId).Last();
                                        locations = CurrentAsset.Locations.Where(x => x.LocationId != oldLoc.LocationId).ToList();
                                        locations.Add(_NewLocation);
                                    }
                                    else
                                    {
                                        locations = CurrentAsset.Locations.ToList();
                                    }

                                    var docs = CurrentAsset.Documetns.ToList();
                                    StoreBill sBill = _storeBillService.Queryable()
                                  .Where(x => x.StoreBillNo == "1" && x.ArrivalDate.Year == CurrentAsset.StoreBill.ArrivalDate.Year
                                  && x.StuffType == StuffType.UnConsumption)
                                  .SingleOrDefault();
                                    AccountDocumentMaster storeBilldoc = null;
                                    int masterDocId = 0;
                                    var emp = _employeeService.Queryable().First();
                                    if (sBill == null)
                                    {
                                        sBill = new StoreBill
                                        {
                                            AcqType = StateOwnership.Purchase,
                                            ArrivalDate = CurrentAsset.StoreBill.ArrivalDate,
                                            ObjectState = ObjectState.Added,
                                            ModifiedDate = GlobalClass._Today,
                                            StoreBillNo = "1",
                                            StoreId = CurrentAsset.StoreBill.StoreId,
                                            Desc1 = CurrentAsset.StoreBill.Desc1,
                                            Desc2 = CurrentAsset.StoreBill.Desc2,
                                            Desc3 = CurrentAsset.StoreBill.Desc3,
                                            StuffType = StuffType.UnConsumption,
                                        };

                                         storeBilldoc= new AccountDocumentMaster
                                         {
                                             AccountDate = GlobalClass._Today,
                                             AccountCover = "1",
                                             ObjectState = ObjectState.Added,
                                             EmployeeId = emp.EmployeeId,
                                         };
                                        storeBilldoc.StoreBill = sBill;
                                    }
                                    else
                                    {
                                        masterDocId = _storeBillService.getRelatedAccountMasterId(sBill.StoreBillId);
                                    }

                                    unconsumption.StoreBill = sBill;

                                    locations.ForEach(l =>
                                    {
                                        unconsumption.Locations.Add(new Location
                                        {
                                            AccountDocumentType = l.AccountDocumentType,
                                            InsertDate = l.InsertDate,
                                            MovedRequestDate = l.MovedRequestDate,
                                            ReturnDate = l.ReturnDate,
                                            ObjectState = ObjectState.Added,
                                            StrategyId = l.StrategyId,
                                            OrganizId = l.OrganizId,
                                            PersonId = l.PersonId,
                                            StoreId = l.StoreId,
                                            StoreAddressId = l.StoreAddressId,
                                            Status = l.Status,
                                        });
                                    });

                                    docs.ForEach(d =>
                                    {
                                        d.MovableAsset.Add(unconsumption);
                                    });
                                    
                                    if (docs.Count > 0 && !CurrentAsset.IsInStore)
                                    {
                                        AccountDocumentMaster documentdoc = null;
                                        var doc = docs.Last();
                                        int docMasterId = _movableAssetService.GetRelatedAccountDocumentByDoc(doc.DocumentId);
                                        if (docMasterId == 0)
                                        {
                                             documentdoc= new AccountDocumentMaster
                                            {
                                                AccountDate = GlobalClass._Today,
                                                AccountCover = "1",
                                                ObjectState = ObjectState.Added,
                                                EmployeeId = emp.EmployeeId,
                                            };
                                            doc.AccountDocument = documentdoc;
                                        }
                                    }

                                    unconsumption.ObjectState = ObjectState.Added;
                                    _movableAssetService.InsertOrUpdateGraph(unconsumption);
                                    _beskaProcedures.MAsset_Delete(CurrentAsset.AssetId,(int)_stufftype, CurrentAsset.Label);
                                }
                                else
                                {
                                    if (_NewLocation != null)
                                    {
                                        var oldLoc = CurrentAsset.Locations.OrderBy(l => l.LocationId).Last();
                                        oldLoc.ObjectState = ObjectState.Deleted;
                                        CurrentAsset.Locations.Add(_NewLocation);
                                        CurrentAsset.Locations.Remove(oldLoc);
                                    }

                                    CurrentAsset.ObjectState = ObjectState.Modified;
                                   _movableAssetService.Update(CurrentAsset);
                                }
                            }
                            else
                            {
                                if (_NewLocation != null)
                                {
                                    var oldLoc = CurrentAsset.Locations.OrderBy(l => l.LocationId).Last();
                                    oldLoc.ObjectState = ObjectState.Deleted;
                                    CurrentAsset.Locations.Add(_NewLocation);
                                    CurrentAsset.Locations.Remove(oldLoc);
                                }

                                CurrentAsset.ObjectState = ObjectState.Modified;
                                _movableAssetService.Update(CurrentAsset);
                            }
                        }
                        else
                        {
                            if (_NewLocation != null)
                            {
                                var oldLoc = CurrentAsset.Locations.OrderBy(l => l.LocationId).Last();
                                oldLoc.ObjectState = ObjectState.Deleted;
                                CurrentAsset.Locations.Add(_NewLocation);
                                CurrentAsset.Locations.Remove(oldLoc);
                            }

                            CurrentAsset.ObjectState = ObjectState.Modified;
                            _movableAssetService.Update(CurrentAsset);
                        }

                        try
                        {
                            _unitOfWork.SaveChanges();
                            _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                            scope.Complete();
                            string msg = $"Id:{CurrentAsset.AssetId}--Label:{CurrentAsset.Label}--Name:{CurrentAsset.Name}--Num:{CurrentAsset.Num}--Price:{CurrentAsset.Cost}";
                            UserLog.UniqueInstance.AddLog(new EventLog
                            {
                                EntryDate = GlobalClass._Today,
                                Message = "Update Asset to specifications"+"="+msg,
                                ObjectState = ObjectState.Added,
                                Type = EventType.Update,
                                UserId = UserLog.UniqueInstance.LogedUser.UserId
                            });
                            window.DialogResult = true;
                        }
                        catch (TransactionAbortedException ex)
                        {
                            _dialogService.ShowError("TransactionAbortedException", ex.Message);
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
                    window.DialogResult = true;
                }
            }
        }
        
        private void showAddInfoWin(object parameter)
        {
            if (!(CurrentAsset is UnConsumption))
            {
                _dialogService.ShowAlert("توجه", " این نوع مال اطلاعات تکمیلی ندارد");
                return;
            }
            var unconsumAsset = CurrentAsset as UnConsumption;
            Mouse.SetCursor(Cursors.Wait);
            var curwindow = parameter as Window;
            StoryboardManager.PlayStoryboard("StoryboardHideWindow", curwindow);
            var viewModel = new AddInfoViewModel(unconsumAsset);
            _navigationService.ShowAddInfoWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardShowWindow", curwindow);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void deleteMasset(object parameter)
        {
            Window window = parameter as Window;
            if (window == null) return;
            bool confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                if (_isRealasset)
                {
                    var option = new TransactionOptions();
                    option.IsolationLevel = IsolationLevel.ReadCommitted;
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,option))
                    {
                        try
                        {
                            _beskaProcedures.MAsset_Delete(CurrentAsset.AssetId,(int)_stufftype, CurrentAsset.Label);
                            scope.Complete();
                            string msg = $"Id:{CurrentAsset.AssetId}--Label:{CurrentAsset.Label}--Name:{CurrentAsset.Name}--Num:{CurrentAsset.Num}--Price:{CurrentAsset.Cost}";
                            UserLog.UniqueInstance.AddLog(new EventLog
                            {
                                EntryDate = GlobalClass._Today,
                                Message = "Delete Asset to specifications" + "=" + msg,
                                ObjectState = ObjectState.Added,
                                Type = EventType.Update,
                                UserId = UserLog.UniqueInstance.LogedUser.UserId
                            });
                        }
                        catch (TransactionAbortedException ex)
                        {
                            _dialogService.ShowError("TransactionAbortedException", ex.Message);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                }
                else
                {
                    CurrentAsset.AssetId = -1001;
                }

                window.DialogResult = true;
            }
        }
        
        private Tuple<string, string> GetHirecharyNodeTuple(EmployeeDesign item)
        {
            String _nodeName = "";
            string _coding = "";
            if (item.ParentNode != null)
            {
                var getItem = this.GetHirecharyNodeTuple(item.ParentNode);
                _nodeName += getItem.Item1;
                _coding += getItem.Item2;
                _nodeName += "**";
            }
            _coding += item.BuidldingDesignId.ToString();
            _nodeName += item.Name;

            return new Tuple<string, string>(_nodeName, _coding);
        }

        private void ShowSplitWindow(object parameter)
        {
            var window = parameter as Window;
            if (window == null) return;
            StoryboardManager.PlayStoryboard("StoryboardHideWindow", window);
            Mouse.SetCursor(Cursors.Wait);
            var viewModel = new MovableAssetSplitViewModel(_container);
            viewModel.CurrentMovableAsset =CurrentAsset;
            _navigationService.ShowSplitWindow(viewModel);
            Mouse.SetCursor(Cursors.Arrow);
            StoryboardManager.PlayStoryboard("StoryboardShowWindow", window);
        }

        private void showOrderHistoryWindow(object parameter)
        {
            var window = parameter as Window;
            if (window == null) return;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardHideWindow", window);
            var viewModel = new OrderHistoryViewModel(_container);
            viewModel.CurrentAsset =new Repository.Model.MovableAssetModel
            {
                AssetId=CurrentAsset.AssetId,
                Name=CurrentAsset.Name,
                kalaUid=CurrentAsset.KalaUid,
            };
            viewModel.Orders = _movableAssetService.Query(x => x.AssetId == CurrentAsset.AssetId).Include(x => x.Orders)
                .Select().SelectMany(x => x.Orders).ToList();
            _navigationService.ShowOrderHistoryWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardShowWindow", window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showProceedingHistoryWindow(object parameter)
        {
            var window = parameter as Window;
            if (window == null) return;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardHideWindow", window);
            var viewModel = new ProceedingHistoryViewModel(_container, CurrentAsset);
            _navigationService.ShowProceedingHistoryWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardShowWindow", window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showDocumentHistoryWindow(object parameter)
        {
            //var window = parameter as Window;
            //if (window == null) return;
            //Mouse.SetCursor(Cursors.Wait);
            //StoryboardManager.PlayStoryboard("StoryboardHideWindow", window);
            //var viewModel =null;
            //_navigationService.ShowDocumentHistoryWindow(viewModel);
            //StoryboardManager.PlayStoryboard("StoryboardShowWindow", window);
            //Mouse.SetCursor(Cursors.Arrow);
        }

        private void showBelongingsWindow(object parameter)
        {
            var window = parameter as Window;
            if (window == null) return;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardHideWindow", window);
            var viewModel = new BelongingsListViewModel(_container, CurrentAsset as UnConsumption);
            _navigationService.ShowBelongingsWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardShowWindow", window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showMAssetCostWindow(object parameter)
        {
            var window = parameter as Window;
            if (window == null) return;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardHideWindow", window);
            var viewModel = new AssetCostViewModel(_container, CurrentAsset,_isRealasset);
            _navigationService.ShowMAssetCostWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardShowWindow", window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showInsuranceManageWindow(object parameter)
        {
            var window = parameter as Window;
            if (window == null) return;
            var unconsum = CurrentAsset as UnConsumption;
            if (unconsum == null)
            {
                _dialogService.ShowAlert("توجه", "عملیات نامعتبر می باشد");
                return;
            }
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardHideWindow", window);
            var viewModel = new InsuranceManageListViewModel(_container, unconsum, _isRealasset);
            _navigationService.ShowInsuranceManageWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardShowWindow", window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private Task getOldLabelsByFloorAsync()
        {
            Task ts=null;
            if (CurrentAsset is InCommidity)
            {
                if (InCommodityViewModel._oldLabels == null)
                {
                    ts = new Task(() =>
                    {
                        InCommodityViewModel._oldLabels = new Dictionary<string, List<int>>();
                        _movableAssetService.Queryable()
                              .Where(x => x.OldLabel.HasValue).ToLookup(x => x.Floor + "*" + x.FloorType, p => p.OldLabel.Value).ForEach(x =>
                              {
                                  InCommodityViewModel._oldLabels.Add(x.Key, x.ToList());
                              });
                    });
                    ts.Start();
                }
            }
            else if (CurrentAsset is UnConsumption)
            {

                if (UnConsumptionViewModel._oldLabels == null)
                {
                    ts = new Task(() =>
                    {
                        UnConsumptionViewModel._oldLabels = new Dictionary<string, List<int>>();
                        _movableAssetService.Queryable()
                              .Where(x => x.OldLabel.HasValue).ToLookup(x => x.Floor + "*" + x.FloorType, p => p.OldLabel.Value).ForEach(x =>
                              {
                                  UnConsumptionViewModel._oldLabels.Add(x.Key, x.ToList());
                              });
                    });
                    ts.Start();
                }
            }
            return ts ?? Task.FromResult(false);
        }

        private void detailsReport()
        {
            Mouse.SetCursor(Cursors.Wait);
            var viewModel = new ReportViewModel();
            viewModel.MAssetDetailsReport(CurrentAsset.AssetId,_stuffHelper.getreportDescByStuffId(PStuffId));
            _navigationService.ShowReportViewWindow(viewModel);
            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region commands

        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand AddInfoCommand { get; private set; }
        public ICommand SplitCommand { get; private set; }
        public ICommand OrderHistoryCommand { get; private set; }
        public ICommand ProceedingHistoryCommand { get; private set; }
        public ICommand DocumentHistoryCommand { get; private set; }
        public ICommand BelongingCommand { get; private set; }
        public ICommand ReportCommand { get; private set; }
        public ICommand AssetCostCommand { get; private set; }
        public ICommand InsuranceCommand { get; private set; }
        private void initializCommands()
        {
            SaveCommand = new MvvmCommand(
                (paramter) => { this.editCurrentAsset(paramter); },
                (parameter) => { return true; }
                );

            AddInfoCommand = new MvvmCommand(
                (paramter) => { this.showAddInfoWin(paramter); },
                (parameter) => { return true; }
                );

            DeleteCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.deleteMasset(parameter);
                },
                (parameter) => { return true; }
                );


            SplitCommand = new MvvmCommand(
                 (parameter) => { this.ShowSplitWindow(parameter); },
                 (parameter) => { return true; }
                );

            OrderHistoryCommand = new MvvmCommand(
                (parameter) => { this.showOrderHistoryWindow(parameter); },
                (paramter) => { return true; }
                );

            ProceedingHistoryCommand = new MvvmCommand(
                (parameter) => { this.showProceedingHistoryWindow(parameter); },
                (paramter) => { return true; }
                );

            DocumentHistoryCommand = new MvvmCommand(
                (parameter) => { this.showDocumentHistoryWindow(parameter); },
                (paramter) => { return true; }
                );

            BelongingCommand = new MvvmCommand(
                (parameter) => { this.showBelongingsWindow(parameter); },
                (parameter) => { return true; }
                );

            ReportCommand = new MvvmCommand(
                (parameter) => { detailsReport(); },
                (paremeter) => { return true; }
                );

            AssetCostCommand = new MvvmCommand(
               (parameter) => { showMAssetCostWindow(parameter); },
               (paremeter) => { return true; }
               );

            InsuranceCommand = new MvvmCommand(
                 (parameter) => { showInsuranceManageWindow(parameter); },
               (paremeter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IEmployeeService _employeeService;
        private readonly IPersonService _personService;
        private readonly IStoreService _storeService;
        private readonly IStoreBillService _storeBillService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly ISellerService _sellerService;
        private readonly IBskaStoredProcedures _beskaProcedures;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IStuffService _stuffService;
        private readonly IUnitService _unitService;
        private readonly Boolean _isEditable = false;
        private readonly Boolean _isRealasset = true;
        private Boolean _isOldSystem = false;
        private StuffHelper _stuffHelper;
        private List<RequestPermit> _permits;
        Location _NewLocation = null;
        StuffType _stufftype = StuffType.None;
        #endregion
    }
}
