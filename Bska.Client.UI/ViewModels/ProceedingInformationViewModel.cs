
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.Services;
    using Domain.Entity.OrderEntity;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Data.Entity;
    public sealed class ProceedingInformationViewModel : BaseViewModel
    {
        #region ctor

        public ProceedingInformationViewModel(IUnityContainer container,Int32 procId)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._proceedingService = _container.Resolve<IProceedingService>(new ParameterOverride("repository", _unitOfWork.Repository<Proceeding>()));
            this._movableAssetService = _container.Resolve<IMovableAssetService>(new ParameterOverride("repository", _unitOfWork.Repository<MovableAsset>()));
            this._orderService = _container.Resolve<IOrderService>(new ParameterOverride("repository", _unitOfWork.Repository<Order>()));
            this._employeeService = _container.Resolve<IEmployeeService>();
            this._storeService = _container.Resolve<IStoreService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._collection = new ObservableCollection<ProceedingAssetModel>();
            this._selectedDictionary = new Dictionary<MovableAsset, AssetProceeding>();
            this._seedDataHelper = new SeedDataHelper();
            this.initializObj(procId);
            this.initializCommands();
        }

        #endregion

        #region properties

        public Proceeding CurrentProc
        {
            get { return GetValue(() => CurrentProc); }
            set
            {
                SetValue(() => CurrentProc, value);
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

        public String Description
        {
            get { return GetValue(() => Description); }
            set
            {
                SetValue(() => Description, value);
            }
        }
        public Boolean StoreCompeletion
        {
            get { return GetValue(() => StoreCompeletion); }
            set
            {
                SetValue(() => StoreCompeletion, value);
                if (value) getStores();
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

        public Store SelectedStore
        {
            get { return GetValue(() => SelectedStore); }
            set
            {
                SetValue(() => SelectedStore, value);
            }
        }

        public Boolean ConfirmEnabled
        {
            get { return GetValue(() => ConfirmEnabled); }
            set
            {
                SetValue(() => ConfirmEnabled, value);
            }
        }

        public ObservableCollection<ProceedingAssetModel> AssetProceedingView
        {
            get { return _collection; }
        }

        public ProceedingAssetModel Selected
        {
            get { return GetValue(() => Selected); }
            set
            {
                SetValue(() => Selected, value);
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

        public Int32 Counter
        {
            get { return GetValue(() => Counter); }
            set
            {
                SetValue(() => Counter, value);
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

        private async void initializObj(int procId)
        {
            CurrentProc = _proceedingService.Query(p=>p.ProceedingId==procId).Select().Single();
            Units = _unitService.Queryable().ToList();
            ProceedingDetailsVM = new ProceedingDetailsViewModel(CurrentProc);
            var descs = CurrentProc.Description.Split('*');
            foreach (string desc in descs)
            {
                Description += desc + Environment.NewLine;
            }

            bool checkIsSended = true;
            bool checkDivanEnable = false;
            if (CurrentProc.Type == ProceedingsType.AssetRetiring)
            {
                int storeId;
                if (int.TryParse(ProceedingDetailsVM.Desc1, out storeId))
                {
                    ProceedingDetailsVM.Stores = _storeService.Queryable().Where(x => x.StoreId == storeId).ToList();
                    ProceedingDetailsVM.SelectedStore = ProceedingDetailsVM.Stores.FirstOrDefault();
                }
                checkIsSended = false;
            }
             else if(CurrentProc.Type==ProceedingsType.Fire || CurrentProc.Type==ProceedingsType.Flood
                || CurrentProc.Type == ProceedingsType.Earthquake || CurrentProc.Type == ProceedingsType.Accident
                || CurrentProc.Type == ProceedingsType.Theft || CurrentProc.Type==ProceedingsType.ReturnFromRetiring)
            {
                checkIsSended = false;
            }
                else if( CurrentProc.Type == ProceedingsType.ReturnFromTrust
                || CurrentProc.Type == ProceedingsType.StateTransfer)
            {
                checkIsSended = false;
            }
            else if (CurrentProc.Type == ProceedingsType.Delete
                || CurrentProc.Type == ProceedingsType.SpecialLicencing
                || CurrentProc.Type == ProceedingsType.BudgetLicencing)
            {
                if (APPSettings.Default.AccidentProccedingConfirm != 2002)
                    checkDivanEnable = true;
            }

            bool enableLicensing = false;
            bool enablePricing = false;
            bool isSelected = false;
            Boolean isSelectable = false;
            Boolean divanEnable = false;
            _movableAssetService.GetAssetProceedingsToMAssets(CurrentProc.ProceedingId).ToList().ForEach(ap =>
            {
                if (ap.State == AssetProceedingState.IsRejected)
                {
                    isSelected = false;
                    enablePricing = false;
                    enableLicensing = false;
                    isSelectable = false;
                    ap.LicenseNumber = null;
                }
                else
                {
                    _counter++;
                    isSelected = true;
                    this.GlobalPrice += ap.Price;
                    if (CurrentProc.State == ProceedingState.Confirmed)
                    {
                        ConfirmEnabled = true;
                        if (checkIsSended)
                        {
                            if (CurrentProc.IsSended)
                            {
                                enableLicensing = true;
                                enablePricing = true;
                                isSelectable = true;
                                ap.LicenseNumber = null;
                                isSelected = false;

                                if (checkDivanEnable)
                                {
                                    var loc = _movableAssetService.GetLastLocation(ap.AssetId);
                                    if (loc.Status != LocationStatus.Retiring)
                                    {
                                        divanEnable = true;
                                    }
                                    else
                                    {
                                        divanEnable = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            ap.LicenseNumber = "تایید شده";
                            ap.State = AssetProceedingState.IsConfirmed;
                            _selectedDictionary.Add(ap.MAsset, ap);
                        }
                    }
                }

                _collection.Add(new ProceedingAssetModel
                {
                    AssetId = ap.AssetId,
                    IsEditableLicense = enableLicensing,
                    IsEditablePrice = enablePricing,
                    AccidentDivanNo = ap.AccidentDivanNo,
                    IsOrganFault = ap.IsOrganFault,
                    Label = ap.MAsset.Label,
                    LicenseNumber = ap.LicenseNumber,
                    Name = ap.MAsset.Name,
                    Num = ap.MAsset.Num,
                    Price = ap.Price,
                    RecipetNo = ap.RecipetNo,
                    State = ap.State,
                    UnitId = ap.MAsset.UnitId,
                    IsSelected = isSelected,
                    IsSelectable=isSelectable,
                    IsEditableDivan=divanEnable,
                    TempDesc1=ap.TempDesc1,
                    TempDesc2 = ap.TempDesc2,
                    TempDesc3 = ap.TempDesc3,
                    TempDesc4 = ap.TempDesc4,
                    TempUid1=ap.TempUid1,
                    TempUid2 = ap.TempUid2,
                    TempUid3= ap.TempUid3,
                    TempUid4 = ap.TempUid4,
                });
            });

            Counter = _selectedDictionary.Count;
            if (CurrentProc.State == ProceedingState.Confirmed)
            {
                _maxDraft = await initDraftNo(DocumentType.StoreInternalDraft, false);
                if (CurrentProc.Type == ProceedingsType.AssetRetiring
                    || CurrentProc.Type == ProceedingsType.Delete || CurrentProc.Type == ProceedingsType.BudgetLicencing ||
                        CurrentProc.Type == ProceedingsType.SpecialLicencing || CurrentProc.Type==ProceedingsType.ReturnFromRetiring)
                {
                    _maxRetiringDraft = await initDraftNo(DocumentType.StoreBillRetiring, true);
                }
            }
        }

        private void getStores()
        {
            Stores = _storeService.Queryable().Where(s=>s.StoreType!=StoreType.Retiring).ToList();
        }

        private void addToSelectList(object parameter)
        {
            CheckBox ch = parameter as CheckBox;
            var apm = ch.Tag as ProceedingAssetModel;
            if (apm == null) return;
            var ap = CurrentProc.AssetProceedings.Single(x => x.AssetId == apm.AssetId);
            if (ch.IsChecked==true)
            {
                if (apm.Price <= 0)
                {
                    _dialogService.ShowAlert("توجه", "قیمت مال صحیح نیست");
                    ch.IsChecked = false;
                    return;
                }

                if (apm.IsEditableDivan)
                {
                    if (string.IsNullOrWhiteSpace(apm.AccidentDivanNo))
                    {
                        _dialogService.ShowAlert("توجه", "شماره رای دیوان عدالت اداری الزامی است");
                        ch.IsChecked = false;
                        return;
                    }
                }

                if (apm.IsOrganFault)
                {
                    if (string.IsNullOrWhiteSpace(apm.RecipetNo))
                    {
                        _dialogService.ShowAlert("توجه", "شماره فیش واریزی با توجه به مقصر بودن الزامی است");
                        ch.IsChecked = false;
                        return;
                    }
                }

                if (string.IsNullOrWhiteSpace(apm.LicenseNumber))
                {
                    Boolean confirm = _dialogService.AskConfirmation("پرسش", "شماره مجوز دارایی وارد نشده است.آیا این مال مورد تایید دارایی واقع نشده است");
                    if (!confirm)
                    {
                        ch.IsChecked = false;
                        return;
                    }
                    apm.State = AssetProceedingState.IsRejected;
                }
                else
                {
                    apm.State = AssetProceedingState.IsConfirmed;
                }

                ap.State = apm.State;
                if (apm.Price != ap.Price) ap.Price = apm.Price;
                ap.AccidentDivanNo = apm.AccidentDivanNo;
                ap.IsOrganFault = apm.IsOrganFault;
                ap.LicenseNumber = apm.LicenseNumber;
                ap.RecipetNo = apm.RecipetNo;
                
               if(!_selectedDictionary.ContainsKey(ap.MAsset))
                _selectedDictionary.Add(ap.MAsset, ap);
            }
            else
            {
                if (apm.State == AssetProceedingState.IsRejected)
                {
                    apm.State = AssetProceedingState.InProgress;
                }
                else apm.State = AssetProceedingState.InProgress;
                ap.State = apm.State;
                if (_selectedDictionary.ContainsKey(ap.MAsset))
                    _selectedDictionary.Remove(ap.MAsset);
            }

            int index = _collection.IndexOf(apm);
            _collection.RemoveAt(index);
            _collection.Insert(index, apm);

            var items = _selectedDictionary.Where(x => x.Value.State == AssetProceedingState.IsConfirmed);
            Counter = _selectedDictionary.Count();
            this.Selected = apm;
        }

        private void confirmProceeding(object parameter)
        {
            var window = parameter as Window;
            if (window == null) return;
            if (CurrentProc == null) return;

            if (_selectedDictionary.Count != _counter)
            {
                _dialogService.ShowAlert("خطا", "تعداد مال انتخابی با تعداد مال داخل صورت جلسه برابر نمی باشد");
                return;
            }

            Boolean confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                Mouse.SetCursor(Cursors.Wait);
                CurrentProc.ObjectState = ObjectState.Modified;

                if (CurrentProc.Type == ProceedingsType.EditRequest)
                {
                    _selectedDictionary.ForEach(ma =>
                    {
                        var loc = ma.Key.Locations.OrderByDescending(x => x.LocationId).FirstOrDefault();
                        if (loc.Status == LocationStatus.MovedRequest)
                        {
                            loc.ObjectState = ObjectState.Modified;
                            loc.Status = LocationStatus.Active;
                            ma.Key.Locations.Add(loc);
                        }
                        ma.Key.Desc1 = ma.Value.TempDesc1;
                        ma.Key.Desc2 = ma.Value.TempDesc2;
                        ma.Key.Desc3 = ma.Value.TempDesc3;
                        ma.Key.Desc4 = ma.Value.TempDesc4;
                        ma.Key.Uid1 = ma.Value.TempUid1;
                        ma.Key.Uid2 = ma.Value.TempUid2;
                        ma.Key.Uid3 = ma.Value.TempUid3;
                        ma.Key.Uid4 = ma.Value.TempUid4;
                        ma.Value.ObjectState = ObjectState.Modified;
                        ma.Value.AssetId = ma.Key.AssetId;
                        CurrentProc.AssetProceedings.Add(ma.Value);
                        _movableAssetService.Update(ma.Key);
                    });

                    CurrentProc.ExecutionTime = GlobalClass._Today;
                    CurrentProc.State = ProceedingState.CompletedConfirm;
                    CurrentProc.Description += "*" + "تایید و تکمیل صورت جلسه توسط امین اموال سازمان مذکور به نام" + UserLog.UniqueInstance.LogedUser.FullName
                        + " " + "در تاریخ " + GlobalClass._Today.PersianDateString();

                    _proceedingService.InsertOrUpdateGraph(CurrentProc);
                }
                else
                {
                    string errorList = "";
                    if (!StoreCompeletion)
                    {
                        BskaUIHelper myHelper = new BskaUIHelper();
                        MAssetCurState curState = myHelper.getAssetDefenitiveStateByProc(CurrentProc.Type);
                        int storeState = 201;
                        LocationStatus locStatus = myHelper.getLocationStatusByProc(CurrentProc.Type);

                        var newDoc = new Document
                        {
                            ObjectState = ObjectState.Added,
                            Desc1 = _maxDraft,
                            DocumentDate = GlobalClass._Today,
                            DocumentType = DocumentType.None,
                            Transferee =CurrentProc.Desc1
                        };

                        var accountMaster = new AccountDocumentMaster
                        {
                            AccountCover = "1",
                            AccountDate = GlobalClass._Today,
                            EmployeeId = UserLog.UniqueInstance.LogedEmployee.EmployeeId,
                            ObjectState = ObjectState.Added,
                        };

                        newDoc.AccountDocument = accountMaster;

                        Document retiringBill = null;
                        _selectedDictionary.ForEach(ma =>
                        {
                            if (ma.Value.State == AssetProceedingState.IsConfirmed)
                            {
                                var loc = ma.Key.Locations.OrderByDescending(x => x.LocationId).FirstOrDefault();

                                var newLoc = new Location
                                {
                                    InsertDate = GlobalClass._Today,
                                    MovedRequestDate = GlobalClass._Today,
                                    ReturnDate = GlobalClass._Today,
                                    ObjectState = ObjectState.Added,
                                    Status = locStatus,
                                };

                                loc.ObjectState = ObjectState.Modified;
                                ma.Key.CurState = curState;
                                ma.Key.ObjectState = ObjectState.Modified;
                                if (loc.Status == LocationStatus.StoreActive)
                                {
                                    loc.Status = LocationStatus.StoreDeActive;
                                    loc.MovedRequestDate = GlobalClass._Today;
                                    loc.ReturnDate = GlobalClass._Today;
                                    newDoc.StoreId = loc.StoreId;
                                    storeState = 202;
                                }
                                else if (loc.Status == LocationStatus.Retiring)
                                {
                                    storeState = 203;
                                    loc.Status = LocationStatus.RetiringDeActive;
                                    loc.ReturnDate = GlobalClass._Today;
                                    newDoc.StoreId = loc.StoreId;
                                }
                                else if (loc.Status == LocationStatus.Accident)
                                {
                                    storeState = 204;
                                    loc.Status = LocationStatus.AccidentDeActive;
                                    loc.ReturnDate = GlobalClass._Today;
                                }
                                else if (loc.Status == LocationStatus.MovedRequest)
                                {
                                    loc.Status = LocationStatus.DeActive;
                                    loc.ReturnDate = GlobalClass._Today;
                                    storeState = 201;
                                }
                                else if (loc.Status == LocationStatus.Trust)
                                {
                                    loc.Status = LocationStatus.TrustDeActive;
                                    loc.ReturnDate = GlobalClass._Today;
                                    var lastOneLoc = ma.Key.Locations.OrderByDescending(x => x.LocationId).Take(2).LastOrDefault();
                                    if (lastOneLoc.Status == LocationStatus.DeActive)
                                    {
                                        newLoc.Status = LocationStatus.Active;
                                        newLoc.AccountDocumentType = AccountDocumentType.TrustToUnits;
                                        storeState = 201;
                                    }
                                    else if (lastOneLoc.Status == LocationStatus.StoreDeActive)
                                    {
                                        newLoc.Status = LocationStatus.StoreActive;
                                        newLoc.AccountDocumentType = AccountDocumentType.TrustToStock;
                                        storeState = 202;
                                        newDoc.StoreId = lastOneLoc.StoreId;
                                    }
                                    else
                                    {
                                        errorList += "آخرین مکان مال قبل از ثبت صورت جلسه امانی نامعتبر است" + Environment.NewLine;
                                    }

                                    newLoc.ReturnDate = null;
                                    newLoc.MovedRequestDate = null;
                                    newLoc.StrategyId = lastOneLoc.StrategyId;
                                    newLoc.StoreId = lastOneLoc.StoreId;
                                    newLoc.StoreAddressId = lastOneLoc.StoreAddressId;
                                    newLoc.PersonId = lastOneLoc.PersonId;
                                    newLoc.OrganizId = lastOneLoc.OrganizId;
                                }
                                else
                                {
                                    errorList += "آخرین گردش مال برای ثبت در صورت جلسه معتبر نیست" + Environment.NewLine;
                                }

                                Location newLoc1 = null;

                                if (CurrentProc.Type == ProceedingsType.Accident || CurrentProc.Type == ProceedingsType.Earthquake || CurrentProc.Type == ProceedingsType.Fire
                                    || CurrentProc.Type == ProceedingsType.Flood || CurrentProc.Type == ProceedingsType.Theft)
                                {
                                    if (storeState == 201)
                                    {
                                        newLoc.AccountDocumentType = AccountDocumentType.UnitsToDisaster;
                                    }
                                    else if (storeState == 202)
                                    {
                                        newLoc.AccountDocumentType = AccountDocumentType.StockToDisaster;
                                    }

                                    newLoc.OrganizId = loc.OrganizId;
                                    newLoc.PersonId = loc.PersonId;
                                    newLoc.StrategyId = loc.StrategyId;
                                    newLoc.StoreId = loc.StoreId;
                                    newLoc.StoreAddressId = loc.StoreAddressId;
                                }
                                else if (CurrentProc.Type == ProceedingsType.ReturnFromRetiring)
                                {
                                    var agoloc = ma.Key.Locations.OrderByDescending(x => x.LocationId).Skip(1).FirstOrDefault();
                                    if (agoloc != null)
                                    {
                                        newDoc.DocumentType = DocumentType.ExitStoreDraft;
                                        newDoc.Desc1 = _maxRetiringDraft;
                                        if (agoloc.Status == LocationStatus.StoreDeActive)
                                        {
                                            newLoc.Status = LocationStatus.StoreActive;
                                            newLoc.AccountDocumentType = AccountDocumentType.RetiringToStock;
                                            if (retiringBill == null)
                                            {
                                                retiringBill = new Document
                                                {
                                                    ObjectState = ObjectState.Added,
                                                    Desc1 = _maxDraft,
                                                    DocumentDate = GlobalClass._Today,
                                                    DocumentType = DocumentType.ReturnToStoreDraft,
                                                    StoreId = agoloc.StoreId,
                                                    Transferee = "انبار"
                                                };
                                                accountMaster.Document = retiringBill;
                                            }
                                            ma.Key.Documetns.Add(retiringBill);
                                        }
                                        else
                                        {
                                            newLoc.Status = LocationStatus.Active;
                                            newLoc.AccountDocumentType = AccountDocumentType.RetiringToUnits;
                                        }

                                        newLoc.OrganizId = agoloc.OrganizId;
                                        newLoc.PersonId = agoloc.PersonId;
                                        newLoc.StrategyId = agoloc.StrategyId;
                                        newLoc.StoreId = agoloc.StoreId;
                                        newLoc.StoreAddressId = agoloc.StoreAddressId;
                                        ma.Key.Documetns.Add(newDoc);
                                    }
                                    else
                                    {
                                        errorList += "مکان نامعتبر برای بازگشت از اسقاط" + Environment.NewLine;
                                    }
                                }
                                else if (CurrentProc.Type == ProceedingsType.TrustTransfer)
                                {
                                    if (storeState == 201)
                                    {
                                        newLoc.AccountDocumentType = AccountDocumentType.UnitsToTrust;
                                    }
                                    else if (storeState == 202)
                                    {
                                        newLoc.AccountDocumentType = AccountDocumentType.StockToTrust;
                                        newDoc.DocumentType = DocumentType.ExitStoreTrustDraft;
                                        ma.Key.Documetns.Add(newDoc);
                                    }
                                    newLoc.OrganizId = loc.OrganizId;
                                    newLoc.PersonId = loc.PersonId;
                                    newLoc.StrategyId = loc.StrategyId;
                                    newLoc.StoreId = loc.StoreId;
                                    newLoc.StoreAddressId = loc.StoreAddressId;
                                }
                                else if (CurrentProc.Type == ProceedingsType.StateTransfer
                                    || CurrentProc.Type == ProceedingsType.DefinitiveTransfer)
                                {
                                    if (storeState == 202)
                                    {
                                        newLoc.AccountDocumentType = AccountDocumentType.StockToSent;
                                        newLoc.StoreId = loc.StoreId;
                                        newLoc.StoreAddressId = loc.StoreAddressId;

                                        newLoc1 = new Location
                                        {
                                            ObjectState = ObjectState.Added,
                                            AccountDocumentType = AccountDocumentType.SentToExecutive,
                                            InsertDate = GlobalClass._Today,
                                            MovedRequestDate = GlobalClass._Today,
                                            ReturnDate = GlobalClass._Today,
                                            Status = LocationStatus.Send,
                                        };
                                        newDoc.DocumentType = DocumentType.ExitStoreDraft;
                                        ma.Key.Documetns.Add(newDoc);
                                    }
                                    else
                                    {
                                        errorList += "اموال نامعتبر در صورت جلسه انتقال" + Environment.NewLine;
                                    }
                                }
                                else if (CurrentProc.Type == ProceedingsType.Sale)
                                {
                                    if (storeState == 202)
                                    {
                                        newLoc.AccountDocumentType = AccountDocumentType.StockToSent;
                                        newLoc.StoreId = loc.StoreId;
                                        newLoc.StoreAddressId = loc.StoreAddressId;
                                        newDoc.DocumentType = DocumentType.ExitStoreDraft;
                                        ma.Key.Documetns.Add(newDoc);
                                    }
                                    else if (storeState == 203)
                                    {
                                        newLoc.AccountDocumentType = AccountDocumentType.RetiringToSent;
                                        newDoc.StoreId = loc.StoreId;
                                        newDoc.Desc1 = _maxRetiringDraft;
                                        newDoc.DocumentType = DocumentType.ExitStoreDraft;
                                        ma.Key.Documetns.Add(newDoc);
                                    }

                                    newLoc1 = new Location
                                    {
                                        ObjectState = ObjectState.Added,
                                        AccountDocumentType = AccountDocumentType.SentToExecutive,
                                        InsertDate = GlobalClass._Today,
                                        MovedRequestDate = GlobalClass._Today,
                                        ReturnDate = GlobalClass._Today,
                                        Status = LocationStatus.Send,
                                    };
                                }
                                else if (CurrentProc.Type == ProceedingsType.AssetRetiring)
                                {
                                    if (storeState == 201)
                                    {
                                        newLoc.AccountDocumentType = AccountDocumentType.UnitsToRetiring;
                                    }
                                    else if (storeState == 202)
                                    {
                                        newLoc.AccountDocumentType = AccountDocumentType.StockToRetiring;
                                        newDoc.DocumentType = DocumentType.ExitStoreDraft;
                                        ma.Key.Documetns.Add(newDoc);
                                    }

                                    int storeid;
                                    if (int.TryParse(CurrentProc.Desc1, out storeid))
                                    {
                                        newLoc.StoreId = storeid;
                                        if (retiringBill == null)
                                        {
                                            retiringBill = new Document
                                            {
                                                ObjectState = ObjectState.Added,
                                                Desc1 = _maxRetiringDraft,
                                                DocumentDate = GlobalClass._Today,
                                                DocumentType = DocumentType.StoreBillRetiring,
                                                StoreId = storeid,
                                                Transferee = "اسقاط"
                                            };
                                            accountMaster.Document = retiringBill;
                                        }
                                        ma.Key.Documetns.Add(retiringBill);
                                    }
                                    else
                                    {
                                        errorList += "انبار اسقاط نامعتبر است" + Environment.NewLine;
                                    }

                                    newLoc.OrganizId = loc.OrganizId;
                                    newLoc.PersonId = loc.PersonId;
                                    newLoc.StrategyId = loc.StrategyId;
                                }
                                else if (CurrentProc.Type == ProceedingsType.Delete || CurrentProc.Type == ProceedingsType.BudgetLicencing ||
                                    CurrentProc.Type == ProceedingsType.SpecialLicencing)
                                {
                                    if (storeState == 204)
                                    {
                                        newLoc.AccountDocumentType = AccountDocumentType.DisasterToSent;
                                    }
                                    else if (storeState == 203)
                                    {
                                        newLoc.AccountDocumentType = AccountDocumentType.RetiringToSent;
                                        newDoc.Desc1 = _maxRetiringDraft;
                                        newDoc.DocumentType = DocumentType.ExitStoreDraft;
                                        ma.Key.Documetns.Add(newDoc);
                                    }

                                    newLoc.StoreId = loc.StoreId;
                                    newLoc.StoreAddressId = loc.StoreAddressId;

                                    newLoc1 = new Location
                                    {
                                        ObjectState = ObjectState.Added,
                                        AccountDocumentType = AccountDocumentType.SentToExecutive,
                                        InsertDate = GlobalClass._Today,
                                        MovedRequestDate = GlobalClass._Today,
                                        ReturnDate = GlobalClass._Today,
                                        Status = LocationStatus.Send,
                                    };
                                }
                                else if (CurrentProc.Type == ProceedingsType.ReturnFromTrust)
                                {
                                    if (storeState == 201)
                                    {
                                    }
                                    else if (storeState == 202)
                                    {
                                        newDoc.DocumentType = DocumentType.StoreBillReturnFromTrust;
                                        ma.Key.Documetns.Add(newDoc);
                                    }
                                }
                                else if (CurrentProc.Type == ProceedingsType.RefundTrust)
                                {
                                    if (storeState == 201)
                                    {
                                    }
                                    else if (storeState == 202)
                                    {
                                        newDoc.DocumentType = DocumentType.ExitStoreDraft;
                                        ma.Key.Documetns.Add(newDoc);
                                    }

                                    newLoc.AccountDocumentType = AccountDocumentType.TrustToEscrow;
                                    newLoc.OrganizId = loc.OrganizId;
                                    newLoc.PersonId = loc.PersonId;
                                    newLoc.StrategyId = loc.StrategyId;
                                    newLoc.StoreId = loc.StoreId;
                                    newLoc.StoreAddressId = loc.StoreAddressId;
                                }
                                else
                                {
                                    errorList += "نوع صورت جلسه نامعتبر است" + Environment.NewLine;
                                }

                                bool hasBelonging = _movableAssetService.ContainBelongings(ma.Key.AssetId);
                                if (hasBelonging)
                                {
                                    var belongs = _movableAssetService.GetBelongingsToLocation(ma.Key.AssetId);
                                    belongs.ForEach(b =>
                                    {
                                        b.ObjectState = ObjectState.Modified;
                                        b.CurState = ma.Key.CurState;
                                        var bloc = b.Locations.OrderByDescending(x => x.LocationId).FirstOrDefault();
                                        bloc.ObjectState = ObjectState.Modified;
                                        bloc.Status = loc.Status;
                                        bloc.MovedRequestDate = loc.MovedRequestDate;
                                        bloc.ReturnDate = loc.ReturnDate;

                                        var bnewLoc = new Location
                                        {
                                            InsertDate = GlobalClass._Today,
                                            MovedRequestDate = GlobalClass._Today,
                                            ReturnDate = GlobalClass._Today,
                                            ObjectState = ObjectState.Added,
                                            Status = newLoc.Status,
                                            AccountDocumentType = newLoc.AccountDocumentType,
                                            OrganizId = newLoc.OrganizId,
                                            PersonId = newLoc.PersonId,
                                            StoreAddressId = newLoc.StoreAddressId,
                                            StoreId = newLoc.StoreId,
                                            StrategyId = newLoc.StrategyId,
                                        };
                                        b.Locations.Add(bloc);
                                        b.Locations.Add(bnewLoc);
                                        if (newLoc1 != null)
                                        {
                                            var bnewloc1 = new Location
                                            {
                                                InsertDate = GlobalClass._Today,
                                                MovedRequestDate = GlobalClass._Today,
                                                ReturnDate = GlobalClass._Today,
                                                ObjectState = ObjectState.Added,
                                                Status = newLoc1.Status,
                                                AccountDocumentType = newLoc1.AccountDocumentType,
                                                OrganizId = newLoc1.OrganizId,
                                                PersonId = newLoc1.PersonId,
                                                StoreAddressId = newLoc1.StoreAddressId,
                                                StoreId = newLoc1.StoreId,
                                                StrategyId = newLoc1.StrategyId,
                                            };
                                            b.Locations.Add(bnewloc1);
                                        }

                                        if (newDoc.DocumentType != DocumentType.None)
                                            b.Documetns.Add(newDoc);

                                        if (retiringBill != null)
                                        {
                                            b.Documetns.Add(retiringBill);
                                        }
                                    });
                                }
                                else if (ma.Key is Belonging)
                                {
                                    var b = ma.Key as Belonging;
                                    if (b.ParentMAsset == null)
                                    {
                                        b.ParentMAsset = _movableAssetService.GetBelongingParnet(b.AssetId);
                                    }
                                    b.ParentMAsset = null;
                                }

                                ma.Key.Locations.Add(loc);
                                if (newLoc != null)
                                {
                                    newLoc.AssetId = ma.Key.AssetId;
                                    ma.Key.Locations.Add(newLoc);
                                }

                                if (newLoc1 != null)
                                {
                                    newLoc1.AssetId = ma.Key.AssetId;
                                    ma.Key.Locations.Add(newLoc1);
                                }
                            }

                            ma.Value.ObjectState = ObjectState.Modified;
                            ma.Value.AssetId = ma.Key.AssetId;
                            CurrentProc.AssetProceedings.Add(ma.Value);
                        });
                        CurrentProc.ExecutionTime = GlobalClass._Today;
                        CurrentProc.State = ProceedingState.CompletedConfirm;
                        CurrentProc.Description += "*" + "تایید و تکمیل صورت جلسه توسط امین اموال سازمان مذکور به نام" + UserLog.UniqueInstance.LogedUser.FullName
                            + " " + "در تاریخ " + GlobalClass._Today.PersianDateString();

                        _proceedingService.InsertOrUpdateGraph(CurrentProc);
                    }
                    else
                    {
                        if (SelectedStore == null)
                        {
                            errorList += "هیچ انباری انتخاب نشده است";
                        }

                        _selectedDictionary.ForEach(ap =>
                        {
                            if (ap.Value.State == AssetProceedingState.IsConfirmed)
                            {
                                ap.Value.State = AssetProceedingState.InProgress;
                            }

                            ap.Value.ObjectState = ObjectState.Modified;
                            CurrentProc.AssetProceedings.Add(ap.Value);
                        });

                        CurrentProc.State = ProceedingState.StoreConfirm;
                        CurrentProc.StoreId = SelectedStore.StoreId;
                        CurrentProc.Description += "*" + "تایید و ارسال به انبار صورت جلسه توسط امین اموال سازمان مذکور به نام " + UserLog.UniqueInstance.LogedUser.FullName
                            + " " + "در تاریخ " + GlobalClass._Today.PersianDateString();
                        _proceedingService.InsertOrUpdateGraph(CurrentProc);
                    }

                    if (errorList.Length > 0)
                    {
                        _dialogService.ShowError("خطا", errorList);
                        return;
                    }
                }
                var assetIds = CurrentProc.AssetProceedings.Select(s => s.AssetId);
                var orders = _orderService.Queryable().Where(o => o.Status == OrderStatus.StuffHonest && o.OrderType == OrderType.Procceding).Include(o => o.MovableAssets).ToList();
                orders.ForEach(o =>
                {
                    bool isRelatedOrder = false;
                    o.MovableAssets.ForEach(ma =>
                    {
                        if (assetIds.Contains(ma.AssetId))
                        {
                            isRelatedOrder = true;
                        }
                    });
                    if (isRelatedOrder)
                    {
                        o.ObjectState = ObjectState.Modified;
                        o.Status = OrderStatus.Deliviry;
                        _orderService.Update(o);
                    }
                });

                try
                {
                    _unitOfWork.SaveChanges();
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
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
                _navigationService.ShowMAssetDetailsWindow(viewModel);
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
                var viewModel = new PermEditDetailsViewModel(_container, model.AssetId,false);
                viewModel.EditYear = model.TempYear ?? 0;
                viewModel.CurrentMovableAsset.Desc1 = model.TempDesc1;
                viewModel.CurrentMovableAsset.Desc2 = model.TempDesc2;
                viewModel.CurrentMovableAsset.Desc3 = model.TempDesc3;
                viewModel.CurrentMovableAsset.Desc4 = model.TempDesc4;
                viewModel.CurrentMovableAsset.Uid1 = model.TempUid1;
                viewModel.CurrentMovableAsset.Uid2 = model.TempUid2;
                viewModel.CurrentMovableAsset.Uid3 = model.TempUid3;
                viewModel.CurrentMovableAsset.Uid4 = model.TempUid4;
                _navigationService.ShowPermEditWindow(viewModel);
                StoryboardManager.PlayStoryboard("StoryboardShowWindow", window);
                Mouse.SetCursor(Cursors.Arrow);
            }
        }

        private Task<String> initDraftNo(DocumentType docType, bool isRetiringStore)
        {
            var ts = new Task<string>(() =>
            {
                string draftNo = "1";
                int temp;
                var docs = new List<Document>();
                
                if (docType == DocumentType.StoreBillRetiring)
                {
                    docs = _movableAssetService.GetDocumentsByType(docType, isRetiringStore).ToList();
                }
                else
                {
                    docs = _movableAssetService.GetDocuments(false, isRetiringStore).ToList();
                }

                if (docs.Count() > 0)
                {
                    int maxVal = docs
                        .Select(d => int.TryParse(d.Desc1, out temp) ? temp : 0).Max();
                    draftNo = (maxVal + 1).ToString();
                }
                return draftNo;
            });
            ts.Start();
            return ts;
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

        public ICommand SelectCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }
        public ICommand MAssetDetailsCommand { get; private set; }
        public ICommand MAssetEditCommand { get; private set; }
        private void initializCommands()
        {
            ConfirmCommand = new MvvmCommand(
                (parameter) => { this.confirmProceeding(parameter); }
                , (parameter) => { return true; }
                );

            SelectCommand = new MvvmCommand(
                (parameter) => { addToSelectList(parameter); },
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
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IEmployeeService _employeeService;
        private readonly IProceedingService _proceedingService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IStoreService _storeService;
        private readonly IUnitService _unitService;
        private readonly IOrderService _orderService;
        private readonly ObservableCollection<ProceedingAssetModel> _collection;
        private readonly Dictionary<MovableAsset, AssetProceeding> _selectedDictionary;
        private readonly SeedDataHelper _seedDataHelper;
        string _maxDraft;
        string _maxRetiringDraft;
        Int32 _counter = 0;

        #endregion
    }
}
