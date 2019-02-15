
namespace Bska.Client.UI.ViewModels.StoreViewModel
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

    public sealed class StoreProceedingViewModel : BaseViewModel
    {
        #region ctor

        public StoreProceedingViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>(new ParameterOverride("repository", _unitOfWork.Repository<MovableAsset>()));
            this._proceedingService = _container.Resolve<IProceedingService>(new ParameterOverride("repository", _unitOfWork.Repository<Proceeding>()));
            this._orderService = _container.Resolve<IOrderService>(new ParameterOverride("repository", _unitOfWork.Repository<Order>()));
            this._dialogService = _container.Resolve<IDialogService>();
            this._employeeService = _container.Resolve<IEmployeeService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._storeService = _container.Resolve<IStoreService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._personService = _container.Resolve<IPersonService>();
            this._selectedDictionary = new Dictionary<MovableAsset, AssetProceeding>();
            this._assetCollection = new ObservableCollection<ProceedingAssetModel>();
            this._seedDataHelper = new SeedDataHelper();
            this.initializObj();
            this.initializCommands();
        }

        #endregion

        #region properties

        public Window Window
        {
            get;
            set;
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

        public ProceedingState ProceedingState
        {
            get { return GetValue(() => ProceedingState); }
            set
            {
                SetValue(() => ProceedingState, value);
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

        public List<Proceeding> ProceddingsItems
        {
            get { return GetValue(() => ProceddingsItems); }
            set
            {
                SetValue(() => ProceddingsItems, value);
            }
        }

        public Proceeding SelectedProceeding
        {
            get { return GetValue(() => SelectedProceeding); }
            set
            {
                SetValue(() => SelectedProceeding, value);
            }
        }

        public int Counter
        {
            get { return GetValue(() => Counter); }
            set
            {
                SetValue(() => Counter, value);
            }
        }

        public ObservableCollection<ProceedingAssetModel> AssetCollection
        {
            get { return _assetCollection; }
        }

        public ProceedingAssetModel SelectedAsset
        {
            get { return GetValue(() => SelectedAsset); }
            set
            {
                SetValue(() => SelectedAsset, value);
            }
        }

        #endregion

        #region methods

        private void initializObj()
        {
            if (UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.Manager ||
                 UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.StuffHonest)
            {
                Stores = _storeService.Queryable().Where(s=>s.StoreType!=StoreType.Retiring).ToList();
            }
            else if (UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.StoreLeader)
            {
                var storeRoles = _personService.GetRolesByUser(UserLog.UniqueInstance.LogedUser.UserId)
                    .Where(x => x.RoleType == PermissionsType.StoreLeader).Select(x => x.StoreId);

                Stores = _storeService.Queryable().Where(s => storeRoles.Contains(s.StoreId)
                    && s.StoreType != StoreType.Retiring).ToList();
            }
            Units = _unitService.Queryable().ToList();
            ProceedingState = ProceedingState.StoreConfirm;
        }

        private void searchProceedings()
        {
            if (SelectedStore == null)
            {
                _dialogService.ShowAlert("توجه", "هیچ انباری انتخاب نشده است");
                return;
            }
            Mouse.SetCursor(Cursors.Wait);
            if (ProceedingState == ProceedingState.None)
            {
                ProceddingsItems = _proceedingService.Query(x => x.StoreId == SelectedStore.StoreId).Select().ToList();
            }
            else
            {
                ProceddingsItems = _proceedingService.Query(x => x.State == ProceedingState && x.StoreId == SelectedStore.StoreId).Select().ToList();
            }
            Mouse.SetCursor(Cursors.Arrow);
        }

        private async void getProcAssets(object parameter)
        {
            var proc = parameter as Proceeding;
            if (proc == null) return;
            Mouse.SetCursor(Cursors.Wait);
            SelectedProceeding = proc;
            _selectedDictionary.Clear();
            _assetCollection.Clear();
            Counter = 0;

            bool isSelectable = false;
            _movableAssetService.GetAssetProceedingsToMAssets(SelectedProceeding.ProceedingId).Where(ap=>ap.State!=AssetProceedingState.IsRejected).ToList().ForEach(ap =>
            {
                var loc = _movableAssetService.GetLastLocation(ap.AssetId);
                if (SelectedProceeding.Type == ProceedingsType.Delete)
                {
                    if (loc.Status == LocationStatus.Retiring
                    || loc.Status == LocationStatus.Accident)
                    {
                        isSelectable = true;
                    }
                    else
                    {
                        isSelectable = false;
                    }
                }
                else if (SelectedProceeding.Type == ProceedingsType.ReturnFromTrust)
                {
                    if (loc.Status == LocationStatus.Trust)
                    {
                        isSelectable = true;
                    }
                    else
                    {
                        isSelectable = false;
                    }
                }
                else if(SelectedProceeding.Type == ProceedingsType.Sale)
                {
                    if (loc.Status == LocationStatus.Retiring
                    || loc.Status == LocationStatus.StoreActive)
                    {
                        isSelectable = true;
                    }
                    else
                    {
                        isSelectable = false;
                    }
                }
                else
                {
                    if (loc.Status == LocationStatus.MovedRequest
                    || loc.Status == LocationStatus.StoreActive)
                    {
                        isSelectable = true;
                    }
                    else
                    {
                        isSelectable = false;
                    }
                }

                _assetCollection.Add(new ProceedingAssetModel
                {
                    AssetId = ap.AssetId,
                    IsEditableLicense =false,
                    IsEditablePrice = false,
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
                    IsSelected = isSelectable,
                    IsSelectable = isSelectable,
                });
            });

            Mouse.SetCursor(Cursors.Arrow);


            _maxDraft = await initDraftNo(DocumentType.StoreInternalDraft, false);
            if (SelectedProceeding.Type == ProceedingsType.AssetRetiring)
            {
                _maxRetiringDraft = await initDraftNo(DocumentType.StoreBillRetiring, true);
            }
            else if (SelectedProceeding.Type == ProceedingsType.Delete || SelectedProceeding.Type == ProceedingsType.BudgetLicencing ||
                    SelectedProceeding.Type == ProceedingsType.SpecialLicencing)
            {
                _maxRetiringDraft = await initDraftNo(DocumentType.StoreInternalDraft, true);
            }
        }

        private void showProceedingDetails(object parameter)
        {
            var proc = parameter as Proceeding;
            if (proc == null) return;
            SelectedProceeding = proc;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new ProceedingInformationViewModel(_container, proc.ProceedingId);
            var window = _navigationService.ShowProceedingDetailsWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showAssetDetailsWindow(object parameter)
        {
            var mAsset = parameter as ProceedingAssetModel;
            if (mAsset == null) return;

            Mouse.SetCursor(Cursors.Wait);
            this.SelectedAsset = mAsset;
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new MovableAssetDetailsViewModel(_container, mAsset.AssetId);
            _navigationService.ShowMAssetDetailsWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);

            Mouse.SetCursor(Cursors.Arrow);
        }

        private void selectMAsset(object parameter)
        {
            CheckBox ch = parameter as CheckBox;
            if (ch.Tag == null)
            {
                return;
            }

            var apm = ch.Tag as ProceedingAssetModel;
            if (apm == null) return;
            var ap =SelectedProceeding.AssetProceedings.Single(x => x.AssetId == apm.AssetId);
            this.SelectedAsset = apm;
            if (ch.IsChecked == true)
            {
                if (!_selectedDictionary.ContainsKey(ap.MAsset))
                {
                    _selectedDictionary.Add(ap.MAsset,ap);
                    apm.IsSelected = false;
                }
            }
            else
            {
                if (_selectedDictionary.ContainsKey(ap.MAsset))
                {
                    _selectedDictionary.Remove(ap.MAsset);
                    apm.IsSelected = true;
                }
            }
            Counter = _selectedDictionary.Count;
        }

        private void confirmProcAssets()
        {
            if (SelectedProceeding == null)
            {
                _dialogService.ShowAlert("توجه", "هیچ صورت جلسه ای انتخاب نشده است");
                return;
            }

            if (_selectedDictionary.Count <= 0)
            {
                _dialogService.ShowAlert("توجه", "هیچ مالی انتخاب نشده است");
                return;
            }

            Boolean confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                SelectedProceeding.ObjectState = ObjectState.Modified;
                BskaUIHelper myHelper = new BskaUIHelper();
                MAssetCurState curState = myHelper.getAssetDefenitiveStateByProc(SelectedProceeding.Type);
                int storeState = 201;
               
                LocationStatus locStatus = myHelper.getLocationStatusByProc(SelectedProceeding.Type);

                var newDoc = new Document
                {
                    ObjectState = ObjectState.Added,
                    Desc1 = _maxDraft,
                    DocumentDate = GlobalClass._Today,
                    Transferee = SelectedProceeding.Desc1
                };

                var accountMaster = new AccountDocumentMaster
                {
                    AccountCover = "1",
                    AccountDate = GlobalClass._Today,
                    EmployeeId = UserLog.UniqueInstance.LogedEmployee.EmployeeId,
                    ObjectState = ObjectState.Added,
                };
                newDoc.AccountDocument = accountMaster;
                string errorList = "";
                Document retiringBill = null;
                _selectedDictionary.ForEach(ma =>
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

                    newDoc.DocumentType = DocumentType.None;
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

                    if (SelectedProceeding.Type == ProceedingsType.Accident || SelectedProceeding.Type == ProceedingsType.Earthquake || SelectedProceeding.Type == ProceedingsType.Fire
                        || SelectedProceeding.Type == ProceedingsType.Flood || SelectedProceeding.Type == ProceedingsType.Theft)
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
                    else if (SelectedProceeding.Type == ProceedingsType.ReturnFromRetiring)
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
                    else if (SelectedProceeding.Type == ProceedingsType.TrustTransfer)
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
                    else if (SelectedProceeding.Type == ProceedingsType.StateTransfer
                        || SelectedProceeding.Type == ProceedingsType.DefinitiveTransfer)
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
                    else if (SelectedProceeding.Type == ProceedingsType.Sale)
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
                            newLoc.StoreId = loc.StoreId;
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
                    else if (SelectedProceeding.Type == ProceedingsType.AssetRetiring)
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
                        if (int.TryParse(SelectedProceeding.Desc1, out storeid))
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
                                    Transferee = SelectedProceeding.Desc1
                                };
                                retiringBill.AccountDocument = accountMaster;
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
                    else if (SelectedProceeding.Type == ProceedingsType.Delete
                        || SelectedProceeding.Type == ProceedingsType.BudgetLicencing || SelectedProceeding.Type == ProceedingsType.SpecialLicencing)
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
                    else if (SelectedProceeding.Type == ProceedingsType.ReturnFromTrust)
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
                    else if (SelectedProceeding.Type == ProceedingsType.RefundTrust)
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
                        if (!(ma.Key is Belonging))
                        {
                            this.setAccountDocumentDetails(ma.Key as UnConsumption, newLoc, loc, accountMaster);
                        }
                        ma.Key.Locations.Add(newLoc);
                    }

                    if (newLoc1 != null)
                    {
                        newLoc1.AssetId = ma.Key.AssetId;
                        if (!(ma.Key is Belonging))
                        {
                            this.setAccountDocumentDetails(ma.Key as UnConsumption, newLoc1, newLoc, accountMaster);
                        }
                        ma.Key.Locations.Add(newLoc1);
                    }

                    ma.Value.ObjectState = ObjectState.Modified;
                    ma.Value.State = AssetProceedingState.IsConfirmed;
                    ma.Value.AssetId = ma.Key.AssetId;
                    SelectedProceeding.AssetProceedings.Add(ma.Value);
                });

                if (_assetCollection.All(x => !x.IsSelected))
                {
                    SelectedProceeding.ExecutionTime = GlobalClass._Today;
                    SelectedProceeding.State = ProceedingState.CompletedConfirm;
                    SelectedProceeding.Description += "*" + "تایید و تکمیل صورت جلسه توسط انباردار سازمان مذکور به نام" + UserLog.UniqueInstance.LogedUser.FullName
                        + " " + "در تاریخ " + GlobalClass._Today.PersianDateString();
                }
                else
                {
                    SelectedProceeding.Description += "*" + "ارسال " + _selectedDictionary.Count + " مال از صورت جلسه با شماره سند " + _maxDraft + " توسط انبار دار به نام " + UserLog.UniqueInstance.LogedUser.FullName
                     + " " + "در تاریخ " + GlobalClass._Today.PersianDateString();
                }

                if (errorList.Length > 0)
                {
                    _dialogService.ShowError("خطا", errorList);
                    return;
                }

                _proceedingService.InsertOrUpdateGraph(SelectedProceeding);
                
                try
                {
                    _unitOfWork.SaveChanges();
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    this.getProcAssets(SelectedProceeding);
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

        private void setAccountDocumentDetails(UnConsumption asset, Location loc, Location oldLoc, AccountDocumentMaster accountMaster)
        {
            var ma = asset;
            if (ma != null && loc != null)
            {
                List<Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>> currentAccountCodings = new List<Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>>();
                var accountCodings = _employeeService.GetAccountCodings();
                var parentcode = accountCodings.FirstOrDefault(x => x.Parent == null);
                var OrganNames = _seedDataHelper.GetOrganizations().ToList();

                string desc = "نامشخص";
                string code = "0";
                if (loc.AccountDocumentType == AccountDocumentType.TrustToUnits)
                {
                    var organ = OrganNames.FirstOrDefault(x => x.BudgetNo.ToString() == SelectedProceeding.Desc1);
                    if (organ != null)
                    {
                        code = organ.BudgetNo.ToString();
                        desc = organ.Name;
                    }
                    currentAccountCodings.Add(
                       new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.TrustOrganizationReciver),
                       "Creditor", desc, code, ma.Cost, ma));

                    var organization = _employeeService.GetParentNode(1).FirstOrDefault(x => x.BuidldingDesignId == loc.OrganizId);
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
                else if (loc.AccountDocumentType == AccountDocumentType.TrustToStock)
                {
                    var organ = OrganNames.FirstOrDefault(x => x.BudgetNo.ToString() == SelectedProceeding.Desc1);
                    if (organ != null)
                    {
                        code = organ.BudgetNo.ToString();
                        desc = organ.Name;
                    }
                    currentAccountCodings.Add(
                       new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.TrustOrganizationReciver),
                       "Creditor", desc, code, ma.Cost, ma));

                    code = ma.KalaUid.ToString();
                    desc = ma.Name + "**" + ma.Cost.ToString("N0");
                    currentAccountCodings.Add(
                       new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.StockAssetBuyAndType),
                       "Debtor", desc, code, ma.Cost, ma));
                }
                else if (loc.AccountDocumentType == AccountDocumentType.UnitsToDisaster)
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

                    code = ma.KalaUid.ToString();
                    desc = ma.Name + "**" + code;
                    currentAccountCodings.Add(
                       new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.DisasterAssetType),
                       "Debtor", desc, code, ma.Cost, ma));
                }
                else if (loc.AccountDocumentType == AccountDocumentType.StockToDisaster)
                {
                    code = ma.KalaUid.ToString();
                    desc = ma.Name + "**" + ma.Cost.ToString("N0");
                    currentAccountCodings.Add(
                       new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.StockAssetBuyAndType),
                       "Creditor", desc, code, ma.Cost, ma));

                    code = ma.KalaUid.ToString();
                    desc = ma.Name + "**" + code;
                    currentAccountCodings.Add(
                       new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.DisasterAssetType),
                       "Debtor", desc, code, ma.Cost, ma));
                }
                else if (loc.AccountDocumentType == AccountDocumentType.UnitsToTrust)
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

                    var organ = OrganNames.FirstOrDefault(x => x.BudgetNo.ToString() == SelectedProceeding.Desc1);
                    if (organ != null)
                    {
                        code = organ.BudgetNo.ToString();
                        desc = organ.Name;
                    }
                    currentAccountCodings.Add(
                       new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.TrustOrganizationReciver),
                       "Debtor", desc, code, ma.Cost, ma));
                }
                else if (loc.AccountDocumentType == AccountDocumentType.StockToTrust)
                {
                    code = ma.KalaUid.ToString();
                    desc = ma.Name + "**" + ma.Cost.ToString("N0");
                    currentAccountCodings.Add(
                       new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.StockAssetBuyAndType),
                       "Creditor", desc, code, ma.Cost, ma));

                    var organ = OrganNames.FirstOrDefault(x => x.BudgetNo.ToString() == SelectedProceeding.Desc1);
                    if (organ != null)
                    {
                        code = organ.BudgetNo.ToString();
                        desc = organ.Name;
                    }
                    currentAccountCodings.Add(
                       new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.TrustOrganizationReciver),
                       "Debtor", desc, code, ma.Cost, ma));
                }
                else if (loc.AccountDocumentType == AccountDocumentType.StockToSent)
                {
                    code = ma.KalaUid.ToString();
                    desc = ma.Name + "**" + ma.Cost.ToString("N0");
                    currentAccountCodings.Add(
                       new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.StockAssetBuyAndType),
                       "Creditor", desc, code, ma.Cost, ma));

                    if (loc.Status == LocationStatus.Delete)
                    {
                        code = "0";
                        desc = "حذف شده";

                        currentAccountCodings.Add(
                          new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.SendDelete),
                          "Debtor", desc, code, ma.Cost, ma));
                    }
                    else if (loc.Status == LocationStatus.Sale)
                    {
                        code = "0";
                        desc = "فروخته شده";

                        currentAccountCodings.Add(
                          new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.SendSold),
                          "Debtor", desc, code, ma.Cost, ma));
                    }
                    else if (loc.Status == LocationStatus.Transfer
                        || loc.Status == LocationStatus.TransferState)
                    {
                        var organ = OrganNames.FirstOrDefault(x => x.BudgetNo.ToString() == SelectedProceeding.Desc1);
                        if (organ != null)
                        {
                            code = organ.BudgetNo.ToString();
                            desc = "انتقال داده شده";
                        }
                        currentAccountCodings.Add(
                          new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.SendTransfer),
                          "Debtor", desc, code, ma.Cost, ma));
                    }
                }
                else if (loc.AccountDocumentType == AccountDocumentType.RetiringToSent)
                {
                    code = oldLoc.StoreId.ToString();
                    desc = ma.KalaUid.ToString();
                    currentAccountCodings.Add(
                       new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.RetiringAssetType),
                       "Creditor", desc, code, ma.Cost, ma));

                    if (loc.Status == LocationStatus.Delete)
                    {
                        code = "0";
                        desc = "حذف شده";

                        currentAccountCodings.Add(
                          new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.SendDelete),
                          "Debtor", desc, code, ma.Cost, ma));
                    }
                    else if (loc.Status == LocationStatus.Sale)
                    {
                        code = "0";
                        desc = "فروخته شده";

                        currentAccountCodings.Add(
                          new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.SendSold),
                          "Debtor", desc, code, ma.Cost, ma));
                    }
                    else if (loc.Status == LocationStatus.Transfer
                        || loc.Status == LocationStatus.TransferState)
                    {
                        var organ = OrganNames.FirstOrDefault(x => x.BudgetNo.ToString() == SelectedProceeding.Desc1);
                        if (organ != null)
                        {
                            code = organ.BudgetNo.ToString();
                            desc = "انتقال داده شده";
                        }
                        currentAccountCodings.Add(
                          new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.SendTransfer),
                          "Debtor", desc, code, ma.Cost, ma));
                    }
                }
                else if (loc.AccountDocumentType == AccountDocumentType.UnitsToRetiring)
                {
                    var organization=_employeeService.GetParentNode(1).FirstOrDefault(x => x.BuidldingDesignId == oldLoc.OrganizId);
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
                    desc = ma.Name + "**" + code;
                    currentAccountCodings.Add(
                       new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.RetiringAssetType),
                       "Debtor", desc, code, ma.Cost, ma));
                }
                else if (loc.AccountDocumentType == AccountDocumentType.StockToRetiring)
                {
                    code = ma.KalaUid.ToString();
                    desc = ma.Name + "**" + ma.Cost.ToString("N0");
                    currentAccountCodings.Add(
                       new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.StockAssetBuyAndType),
                       "Creditor", desc, code, ma.Cost, ma));

                    code = ma.KalaUid.ToString();
                    desc = ma.Name + "**" + code;
                    currentAccountCodings.Add(
                       new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.RetiringAssetType),
                       "Debtor", desc, code, ma.Cost, ma));
                }
                else if (loc.AccountDocumentType == AccountDocumentType.DisasterToSent)
                {
                    code = "0";
                    desc = ma.Name + "**" + ma.KalaUid.ToString();
                    currentAccountCodings.Add(
                       new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.DisasterAssetType),
                       "Creditor", desc, code, ma.Cost, ma));

                    if (loc.Status == LocationStatus.Delete)
                    {
                        code = "0";
                        desc = "حذف شده";

                        currentAccountCodings.Add(
                          new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.SendDelete),
                          "Debtor", desc, code, ma.Cost, ma));
                    }
                    else if (loc.Status == LocationStatus.Sale)
                    {
                        code = "0";
                        desc = "فروخته شده";

                        currentAccountCodings.Add(
                          new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.SendSold),
                          "Debtor", desc, code, ma.Cost, ma));
                    }
                    else if (loc.Status == LocationStatus.Transfer
                        || loc.Status == LocationStatus.TransferState)
                    {
                        var organ = OrganNames.FirstOrDefault(x => x.BudgetNo.ToString() == SelectedProceeding.Desc1);
                        if (organ != null)
                        {
                            code = organ.BudgetNo.ToString();
                            desc = "انتقال داده شده";
                        }
                        currentAccountCodings.Add(
                          new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.SendTransfer),
                          "Debtor", desc, code, ma.Cost, ma));
                    }
                }
                else if (loc.AccountDocumentType == AccountDocumentType.TrustToEscrow)
                {
                    var organ = OrganNames.FirstOrDefault(x => x.BudgetNo.ToString() == SelectedProceeding.Desc1);
                    if (organ != null)
                    {
                        code = organ.BudgetNo.ToString();
                        desc = organ.Name;
                    }
                    currentAccountCodings.Add(
                       new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedTrustOrganizationSender),
                       "Creditor", desc, code, ma.Cost, ma));

                    currentAccountCodings.Add(
                         new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.Escrow),
                         "Debtor", "طرف حساب امانی", "0", ma.Cost, ma));
                }
                else if (loc.AccountDocumentType == AccountDocumentType.SentToExecutive)
                {
                    if (oldLoc.Status == LocationStatus.Delete)
                    {
                        code = "0";
                        desc = "حذف شده";

                        currentAccountCodings.Add(
                          new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.SendDelete),
                          "Creditor", desc, code, ma.Cost, ma));
                    }
                    else if (oldLoc.Status == LocationStatus.Sale)
                    {
                        code = "0";
                        desc = "فروخته شده";

                        currentAccountCodings.Add(
                          new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.SendSold),
                          "Creditor", desc, code, ma.Cost, ma));
                    }
                    else if (oldLoc.Status == LocationStatus.Transfer
                        || oldLoc.Status == LocationStatus.TransferState)
                    {
                        var organ = OrganNames.FirstOrDefault(x => x.BudgetNo.ToString() == SelectedProceeding.Desc1);
                        if (organ != null)
                        {
                            code = organ.BudgetNo.ToString();
                            desc = "انتقال داده شده";
                        }
                        currentAccountCodings.Add(
                          new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.SendTransfer),
                          "Creditor", desc, code, ma.Cost, ma));
                    }

                    if (ma.Label.HasValue)
                    {
                        code = ma.Label.Value.ToString();
                        desc = "برجسب**" + code;
                    }
                    currentAccountCodings.Add(
                        new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ExecutiveSequenceLabel),
                        "Debtor", desc, code, ma.Cost, ma));
                }
                else if (loc.AccountDocumentType == AccountDocumentType.RetiringToStock)
                {
                    code = oldLoc.StoreId.ToString();
                    desc = ma.Name + "**" + ma.KalaUid.ToString();
                    currentAccountCodings.Add(
                       new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.RetiringAssetType),
                       "Creditor", desc, code, ma.Cost, ma));

                    code = ma.KalaUid.ToString();
                    desc = ma.Name + "**" + ma.Cost.ToString("N0");
                    currentAccountCodings.Add(
                       new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.StockAssetBuyAndType),
                       "Debtor", desc, code, ma.Cost, ma));
                }
                else if (loc.AccountDocumentType == AccountDocumentType.RetiringToUnits)
                {
                    code = oldLoc.StoreId.ToString();
                    desc = ma.Name + "**" + ma.KalaUid.ToString();
                    currentAccountCodings.Add(
                       new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.RetiringAssetType),
                       "Creditor", desc, code, ma.Cost, ma));

                    var organization = _employeeService.GetDesignById(loc.OrganizId, 1);
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
                        Description =vdesc,
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

        #endregion

        #region commands

        public ICommand SearchCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }
        public ICommand SelectCommand { get; private set; }
        public ICommand ProceedingMAssetCommand { get; private set; }
        public ICommand ProceedingDetailsCommand { get; private set; }
        public ICommand AssetDetailsCommand { get; private set; }
        private void initializCommands()
        {
            SearchCommand = new MvvmCommand(
                (parameter) => { this.searchProceedings(); },
                (parameter) => { return true; }
                );

            ProceedingDetailsCommand = new MvvmCommand(
                (parameter) => { this.showProceedingDetails(parameter); },
                (parameter) => { return true; }
                );

            ProceedingMAssetCommand = new MvvmCommand(
                (parameter) => { this.getProcAssets(parameter); },
                (parameter) => { return true; }
                );

            AssetDetailsCommand = new MvvmCommand(
                 (parameter) => { this.showAssetDetailsWindow(parameter); },
                (parameter) => { return true; }
                );

            SelectCommand = new MvvmCommand(
                (parameter) => { this.selectMAsset(parameter); },
                (parameter) => { return true; }
                );

            ConfirmCommand = new MvvmCommand(
                  (parameter) => { this.confirmProcAssets(); },
                (parameter) => { return true; }
                );
        }


        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IProceedingService _proceedingService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IOrderService _orderService;
        private readonly IUnitService _unitService;
        private readonly IStoreService _storeService;
        private readonly IEmployeeService _employeeService;
        private readonly IPersonService _personService;
        private readonly ObservableCollection<ProceedingAssetModel> _assetCollection;
        private readonly Dictionary<MovableAsset, AssetProceeding> _selectedDictionary;
        private readonly SeedDataHelper _seedDataHelper;
        string _maxDraft;
        string _maxRetiringDraft;

        #endregion
    }
}
