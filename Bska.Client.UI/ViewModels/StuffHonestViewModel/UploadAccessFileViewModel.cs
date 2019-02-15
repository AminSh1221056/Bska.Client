
namespace Bska.Client.UI.ViewModels.StuffHonestViewModel
{
    using System;
    using Microsoft.Practices.Unity;
    using System.Windows;
    using System.Collections.Generic;
    using Bska.Client.Repository.Model;
    using Bska.Client.Domain.Entity;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Data.Service;
    using Bska.Client.UI.Services;
    using Bska.Client.API.UnitOfWork;
    using System.Windows.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Bska.Client.UI.Helper;
    using System.Windows.Input;
    using System.IO;
    using Bska.Client.UI.API;
    using System.Data.OleDb;
    using System.Data;
    using Common;
    using System.Windows.Controls;
    using Bska.Client.API.Infrastructure;
    using System.Data.Entity.Infrastructure;

    public sealed class UploadAccessFileViewModel : BaseViewModel
    {
        #region ctor

        public UploadAccessFileViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._personService = _container.Resolve<IPersonService>();
            this._employeeService = _container.Resolve<IEmployeeService>();
            this._storeBillService = _container.Resolve<IStoreBillService>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>(new ParameterOverride("repository", _unitOfWork.Repository<MovableAsset>()));
          
            this._stuffService = _container.Resolve<IStuffService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._sellerService = _container.Resolve<ISellerService>();

            this._organizCollection = new ObservableCollection<EmployeeDesignTreeViewModel>();
            this._strategyCollection = new ObservableCollection<EmployeeDesignTreeViewModel>();
            this._movableAssetCollection = new ObservableCollection<AccessUploadAssetModel>();
            _personPermit = new List<RequestPermit>();
            this._storeBills = new HashSet<StoreBill>();
            this._documents = new Dictionary<Location, Document>();
            this._relatedAccounts = new Dictionary<StoreBill, AccountDocumentMaster>();
            this._relatedAccountsDoc = new Dictionary<Document, AccountDocumentMaster>();
            this._relatedAccountDocId = new Dictionary<Document, int>();
            this._relatedAccountId = new Dictionary<StoreBill, int>();
            this.MovableAssetFilteredView = new CollectionViewSource { Source = MovableAssetCollection }.View;
            this._labels = new Dictionary<int, Tuple<int?, string, int?>>();
            this._seedDataHelper = new SeedDataHelper();
            this._selectedAssets = new List<Tuple<AccessUploadAssetModel, string, int, int>>();
            this.initalizObj();
            this.initializCommands();
        }

        #endregion

        #region propeties

        public Window Window
        {
            get;
            set;
        }

        public List<Unit> Units
        {
            get { return GetValue(() => Units); }
            set
            {
                SetValue(() => Units, value);
            }
        }

        public List<PersonModel> Persons
        {
            get { return GetValue(() => Persons); }
            set
            {
                SetValue(() => Persons, value);
            }
        }

        public PersonModel SelectedPerson
        {
            get { return GetValue(() => SelectedPerson); }
            set
            {
                SetValue(() => SelectedPerson, value);
                this.GetPersonPermit();
                this.GetParentNode();
            }
        }

        public ICollectionView MovableAssetFilteredView { get; set; }
        public ObservableCollection<AccessUploadAssetModel> MovableAssetCollection
        {
            get { return _movableAssetCollection; }
        }
        
        public ObservableCollection<EmployeeDesignTreeViewModel> OrganizCollection
        {
            get { return _organizCollection; }
        }

        public ObservableCollection<EmployeeDesignTreeViewModel> StrategyCollection
        {
            get { return _strategyCollection; }
        }

        public EmployeeDesignTreeViewModel StrategySelected
        {
            get { return GetValue(() => StrategySelected); }
            set
            {
                SetValue(() => StrategySelected, value);
                this.initselecteRequest();
            }
        }

        public EmployeeDesignTreeViewModel OrganizSelected
        {
            get { return GetValue(() => OrganizSelected); }
            set
            {
                SetValue(() => OrganizSelected, value);
                if (value != null)
                {
                    GetStrategyRelated();
                }
            }
        }

        public List<RequestPermit> RequestPermits
        {
            get { return GetValue(() => RequestPermits); }
            set
            {
                SetValue(() => RequestPermits, value);
            }
        }

        public RequestPermit SelectedRequest
        {
            get { return GetValue(() => SelectedRequest); }
            set
            {
                SetValue(() => SelectedRequest, value);
                if (value != null)
                {
                    this.SelectedPerson = Persons.Find(x => x.PersonId == value.PersonId);
                    this.PerformSearch();
                }
            }
        }
        public string PermitId
        {
            get { return GetValue(() => PermitId); }
            set
            {
                SetValue(() => PermitId, value);
            }
        }
        public String SearchCriteria
        {
            get { return GetValue(() => SearchCriteria); }
            set
            {
                SetValue(() => SearchCriteria, value);
                this.movableAssetFilters();
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

        public List<OrganizationModel> Organizations
        {
            get { return GetValue(() => Organizations); }
            set
            {
                SetValue(() => Organizations, value);
            }
        }
        #endregion

        #region methods
        private void initalizObj()
        {
            _organizCollection.Clear();
            _strategyCollection.Clear();

            Persons = _personService.Queryable()
                .Where(x => x.PersonId != 1).Select(p => new PersonModel
            {
                PersonId = p.PersonId,
                FullName = p.FirstName + " " + p.LastName,
                NationalId = p.NationalId
            }).ToList();

            RequestPermits = _personService.GetAllPermits().ToList();
            Organizations = _seedDataHelper.GetOrganizations();
            _allOrganiz = _employeeService.GetParentNode(1).ToList();
            _allStrategy = _employeeService.GetParentNode(2).ToList();
            _stuffs = _stuffService.Queryable().Where(x=>x.IsStuff).ToList();
        }

        private void movableAssetFilters()
        {
            this.MovableAssetFilteredView.Filter = ((obj) =>
            {
                AccessUploadAssetModel items = obj as AccessUploadAssetModel;
                if (items != null)
                    return items.Name.StartsWith(SearchCriteria) || items.lable.ToString() == SearchCriteria;
                return false;
            });
        }

        private void initselecteRequest()
        {
            var rp = RequestPermits.FirstOrDefault(r => r.PersonId == SelectedPerson.PersonId && r.OrganizId == OrganizSelected.BuildingDesignCurrent.BuidldingDesignId
                   && r.StrategyId == StrategySelected.BuildingDesignCurrent.BuidldingDesignId);
            this.PermitId = rp.RequestPermitId.ToString();
        }

        private void GetPersonPermit()
        {
            if (SelectedPerson != null)
            {
                _personPermit.Clear();
                if (RequestPermits == null)
                {
                    foreach (var k in _personService.GetPersonPermit(SelectedPerson.PersonId))
                    {
                        _personPermit.Add(k);
                    }
                }
                else
                {
                    RequestPermits.Where(rp => rp.PersonId == SelectedPerson.PersonId).ForEach(rp =>
                    {
                        _personPermit.Add(rp);
                    });
                }
            }
        }

        private void GetParentNode()
        {
            _organizCollection.Clear();
            _strategyCollection.Clear();
            var organizPermit = _personPermit.Select(x => x.OrganizId);
            var orgItems = _allOrganiz.Where(x => x.ParentNode == null);

            foreach (var org in orgItems)
            {
                _organizCollection.Add(new EmployeeDesignTreeViewModel(org, null, organizPermit));
            }
        }

        private void GetStrategyRelated()
        {
            _strategyCollection.Clear();
            var strategyPermit = _personPermit.Where(x => x.OrganizId == OrganizSelected.BuildingDesignCurrent.BuidldingDesignId)
                .Select(x => x.StrategyId);

            var stgyItems = _allStrategy.Where(x => x.ParentNode == null);
            foreach (var stgy in stgyItems)
            {
                _strategyCollection.Add(new EmployeeDesignTreeViewModel(stgy, null, strategyPermit));
            }
        }

        async void PerformSearch()
        {
            await this.VerifyMatchingOrganizEnumeratorAsync();
            await this.VerifyMatchingStrategyEnumeratorAsync();
        }

        Task VerifyMatchingOrganizEnumeratorAsync()
        {
            var ts = new Task(() =>
            {
                foreach (var k in _organizCollection)
                {
                    var matches = this.FindMatches(SelectedRequest.OrganizId, k);
                    if (matches.Count() > 0)
                    {
                        DispatchService.Invoke(() =>
                        {
                            OrganizSelected = matches.First();
                            OrganizSelected.IsSelected = true;
                        });
                        break;
                    }
                }
            });

            try
            {
                ts.Start();
                return ts;

            }
            catch (NullReferenceException) { return null; }
            catch (Exception) { throw; }
        }

        Task VerifyMatchingStrategyEnumeratorAsync()
        {
            var ts = new Task(() =>
            {
                foreach (var k in _strategyCollection)
                {
                    var matches = this.FindMatches(SelectedRequest.StrategyId, k);
                    if (matches.Count() > 0)
                    {
                        DispatchService.Invoke(() =>
                        {
                            StrategySelected = matches.First();
                            StrategySelected.IsSelected = true;
                        });
                        break;
                    }
                }
            });

            try
            {
                ts.Start();
                return ts;
            }
            catch (NullReferenceException) { return null; }
            catch (Exception) { throw; }
        }

        IEnumerable<EmployeeDesignTreeViewModel> FindMatches(int id, EmployeeDesignTreeViewModel buildingdesign)
        {
            if (buildingdesign.BuildingDesignCurrent.BuidldingDesignId == id)
                yield return buildingdesign;

            foreach (EmployeeDesignTreeViewModel child in buildingdesign.Children)
                foreach (EmployeeDesignTreeViewModel match in this.FindMatches(id, child))
                    yield return match;
        }

        private void saveAccessFile()
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Filter = "Access 2000-2003 (*.mdb)|*.mdb|Access 2007 (*.accdb)|*accdb";
                dialog.Title = "Please select a database";
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (!string.IsNullOrEmpty(dialog.FileName))
                {
                    _movableAssetCollection.Clear();
                    Mouse.SetCursor(Cursors.Wait);
                    string fileName = dialog.FileName;
                    string folderPath = Path.GetDirectoryName(dialog.FileName);
                    OleDbConnection con = new OleDbConnection();
                    con.ConnectionString = @"Provider =Microsoft.ACE.OLEDB.12.0; Data Source = " + fileName + "; Persist Security Info = False;";
                    try
                    {
                        con.Open();
                        OleDbCommand command = new OleDbCommand();
                        command.CommandText = "select * from [Arrival]";
                        command.Connection = con;
                        OleDbDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection);
                        DataTable dtSchema = dr.GetSchemaTable();
                        DataTable dt = new DataTable();
                        // You can also use an ArrayList instead of List<> 
                        List<DataColumn> listCols = new List<DataColumn>();
                        if (dtSchema != null)
                        {
                            foreach (DataRow drow in dtSchema.Rows)
                            {
                                string columnName = System.Convert.ToString(drow["ColumnName"]);
                                DataColumn column = new DataColumn(columnName, (Type)(drow["DataType"]));
                                column.Unique = (bool)drow["IsUnique"];
                                column.AllowDBNull = (bool)drow["AllowDBNull"];
                                column.AutoIncrement = (bool)drow["IsAutoIncrement"];
                                listCols.Add(column);
                                dt.Columns.Add(column);
                            }

                        }

                        while (dr.Read())
                        {
                            DataRow dataRow = dt.NewRow();
                            for (int i = 0; i < listCols.Count; i++)
                            {
                                dataRow[((DataColumn)listCols[i])] = dr[i];
                            }
                            dt.Rows.Add(dataRow);
                        }

                        dt.DataTableToList<AccessUploadAssetModel>().ForEach(mi =>
                        {
                            mi.IsSelected = false;
                            mi.Name = _stuffs.FirstOrDefault(st => st.StuffId == mi.kalauid)?.Name;
                            mi.CurState = (MAssetCurState)mi.Curstate;
                            _movableAssetCollection.Add(mi);
                            _selectedAssets.Add(new Tuple<AccessUploadAssetModel, string, int, int>(mi, "0", 0, 0));
                        });
                    }
                    catch (OleDbException ex)
                    {
                        _dialogService.ShowAlert("Error",ex.Message);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }

        private void relationToLocation(object parameter)
        {
            var checkbox = parameter as CheckBox;
            var selected = checkbox.Tag as AccessUploadAssetModel;
            if (selected == null) return;

            if (selected.IsSelected)
            {
                if (MovableAssetCollection.Any(x => x.IsSelected))
                {
                    if (SelectedPerson == null || OrganizSelected == null || StrategySelected == null)
                    {
                        _dialogService.ShowAlert("توجه", "انتخاب پرسنل،منطقه سازمانی و منطقه استراتژیکی الزامی است");
                        checkbox.IsChecked = false;
                        return;
                    }
                }

                var item = _selectedAssets.Where(x => x.Item1.lable == selected.lable).First();
                int index = _selectedAssets.IndexOf(item);

                _selectedAssets.RemoveAt(index);
                _selectedAssets.Add(new Tuple<AccessUploadAssetModel, string, int, int>(item.Item1, SelectedPerson.NationalId, OrganizSelected.BuildingDesignCurrent.BuidldingDesignId, StrategySelected.BuildingDesignCurrent.BuidldingDesignId));
            }
            else
            {
                var item = _selectedAssets.Where(x => x.Item1.lable == selected.lable && x.Item2 == SelectedPerson.NationalId
                  && x.Item3 == OrganizSelected.BuildingDesignCurrent.BuidldingDesignId && x.Item4 == StrategySelected.BuildingDesignCurrent.BuidldingDesignId).FirstOrDefault();
                
                if (item != null)
                {
                    _selectedAssets.Remove(item);
                }
            }

            Counter = _selectedAssets.Count(x => x.Item1.IsSelected);
        }

        private void saveMAsset()
        {
            if (_movableAssetCollection.Count <= 0)
            {
                _dialogService.ShowAlert("خطا", ErrorMessages.Default.NoRowSelected);
                return;
            }

            var employee = _employeeService.Queryable().SingleOrDefault();
            if (employee == null)
            {
                _dialogService.ShowError("خطا", "هیچ سازمانی یافت نشد");
                return;
            }

            bool confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (!confirm) return;

            Mouse.SetCursor(Cursors.Wait);

            var docType = DocumentType.StoreInternalDraft;
            AccountDocumentMaster accountMaster = null;
            AccountDocumentMaster accountMasterDoc = null;
            int accountMasterId = 0;
            int accountMasterDocId = 0;
            string transfree = "";
            Location docLocation = null;
            Document doc = null;
            StoreBill storeBill = null;
            var unit = _unitService.Queryable().FirstOrDefault(x => x.Name == "عدد");
            var selectedCollection = new HashSet<UnConsumption>();
            _selectedAssets.ForEach(selectedAsset =>
            {
                PersianDate date = new PersianDate(selectedAsset.Item1.Year, 1, 1);
                string stNo = selectedAsset.Item1.anbrsdno;
                string docNo = selectedAsset.Item1.anbhvlno;
                var stuff = _stuffs.FirstOrDefault(st => st.StuffId == selectedAsset.Item1.kalauid);
                if (!selectedAsset.Item1.IsSelected)
                {
                    docLocation = new Location
                    {
                        StoreId = 1,
                        InsertDate = GlobalClass._Today,
                        ObjectState = ObjectState.Added,
                        StoreAddressId =0,
                        Status = LocationStatus.StoreActive,
                    };
                    transfree ="انبار اصلی";
                }
                else
                {
                    docLocation = new Location
                    {
                        StoreId = 1,
                        InsertDate = GlobalClass._Today,
                        ObjectState = ObjectState.Added,
                        OrganizId = selectedAsset.Item3,
                        PersonId = selectedAsset.Item2,
                        StrategyId = selectedAsset.Item4,
                        Status = LocationStatus.Active,
                    };
                    transfree = SelectedPerson.FullName;
                }
               
                storeBill = _storeBills
                            .Where(x => x.StoreBillNo == stNo)
                            .FirstOrDefault();
                doc = _documents.Where(x => x.Value.Desc1 == docNo).Select(x => x.Value).FirstOrDefault();

                if (storeBill != null)
                {
                    accountMasterId = _storeBillService.getRelatedAccountMasterId(storeBill.StoreBillId);
                    if (_relatedAccounts.ContainsKey(storeBill))
                    {
                        accountMaster = _relatedAccounts[storeBill];
                    }
                }
               
                if (doc != null && selectedAsset.Item1.IsSelected)
                {
                    accountMasterDocId = _movableAssetService.GetRelatedAccountDocumentByDoc(doc.DocumentId);
                }

                if (doc == null)
                {
                    doc = new Document
                    {
                        Desc1 = docNo,
                        Desc2 = "",
                        Desc3 = "",
                        Desc4 = "",
                        DocumentDate = date.ToDateTime(),
                        DocumentType = docType,
                        ObjectState = ObjectState.Added,
                        StoreId = 1,
                        Transferee = transfree,
                    };
                }

                if (selectedAsset.Item1.IsSelected && accountMasterDoc == null && accountMasterDocId == 0)
                {
                    accountMasterDoc = new AccountDocumentMaster
                    {
                        AccountDate = GlobalClass._Today,
                        AccountCover = "1",
                        ObjectState = ObjectState.Added,
                        EmployeeId = employee.EmployeeId,
                    };

                    if (doc.AccountDocument == null)
                    {
                        doc.AccountDocument = accountMasterDoc;
                    }
                }

                if (storeBill == null)
                {
                    storeBill = new StoreBill
                    {
                        AcqType = (StateOwnership)selectedAsset.Item1.acqtyp,
                        ArrivalDate = date.ToDateTime(),
                        ObjectState = ObjectState.Added,
                        ModifiedDate = GlobalClass._Today,
                        StoreBillNo = stNo,
                        StoreId = 1,
                        StuffType = StuffType.UnConsumption,
                        SellerId = 1,
                        Desc1=selectedAsset.Item1.couid
                    };

                    accountMaster = new AccountDocumentMaster
                    {
                        AccountDate = GlobalClass._Today,
                        AccountCover = "1",
                        ObjectState = ObjectState.Added,
                        EmployeeId = employee.EmployeeId,
                    };
                    accountMaster.StoreBill = storeBill;
                }

                if (selectedAsset.Item1.IsSelected)
                {
                    if (!_relatedAccountsDoc.ContainsKey(doc))
                    {
                        _relatedAccountsDoc.Add(doc, accountMasterDoc);
                    }

                    if (!_relatedAccountDocId.ContainsKey(doc))
                    {
                        _relatedAccountDocId.Add(doc, accountMasterDocId);
                    }
                }
               

                if (!_relatedAccounts.ContainsKey(storeBill))
                {
                    _relatedAccounts.Add(storeBill, accountMaster);
                }

                if (!_relatedAccountId.ContainsKey(storeBill))
                {
                    _relatedAccountId.Add(storeBill, accountMasterId);
                }

                if (storeBill.AcqType == StateOwnership.Trust)
                {
                    docType = DocumentType.InternalStoreTrustDraft;
                }

                var item = new UnConsumption
                {
                    AssetId=selectedAsset.Item1.lable,
                    Cost = selectedAsset.Item1.Cost,
                    CurState = selectedAsset.Item1.CurState,
                    Desc1 = selectedAsset.Item1.Desc1,
                    Desc2 = selectedAsset.Item1.Desc2,
                    Desc3 = selectedAsset.Item1.Desc3,
                    Desc4 = selectedAsset.Item1.Desc4,
                    Description = selectedAsset.Item1.Desc,
                    InsertDate = GlobalClass._Today,
                    ISCompietion = CompietionState.NotReported,
                    ISConfirmed = false,
                    Label = selectedAsset.Item1.lable,
                    Uid1 = selectedAsset.Item1.Uid1,
                    Uid2 = selectedAsset.Item1.Uid2,
                    Uid3 = selectedAsset.Item1.Uid3,
                    Uid4 = selectedAsset.Item1.Uid4,
                    Quality = "A",
                    Num = 1,
                    Name = selectedAsset.Item1.Name,
                    ObjectState = ObjectState.Added,
                    ModeifiedDate = GlobalClass._Today,
                    KalaNo = stuff.KalaNo,
                    KalaUid = stuff.StuffId,
                    UnitId = unit.UnitId,
                };

                if (storeBill.AcqType == StateOwnership.Trust)
                {
                    item.Label = null;
                    item.Locations.Add(new Location
                    {
                        InsertDate = GlobalClass._Today,
                        ObjectState = ObjectState.Added,
                        Status = LocationStatus.Executive,
                        AccountDocumentType = AccountDocumentType.EscrowToTrust,
                    });
                    docLocation.AccountDocumentType = AccountDocumentType.None;
                    item.Locations.Add(docLocation.Clone());
                }
                else
                {
                    item.Locations.Add(new Location
                    {
                        InsertDate = GlobalClass._Today,
                        ObjectState = ObjectState.Added,
                        Status = LocationStatus.Executive,
                        AccountDocumentType = AccountDocumentType.ExecutiveToReached
                    });
                    if (!selectedAsset.Item1.IsSelected)
                    {
                        docLocation.AccountDocumentType = AccountDocumentType.ReachedToStock;
                        docLocation.StoreAddressId =0;
                    }
                    else
                    {
                        item.Locations.Add(new Location
                        {
                            StoreId = 1,
                            InsertDate = GlobalClass._Today,
                            ObjectState = ObjectState.Added,
                            Status = LocationStatus.StoreDeActive,
                            ReturnDate = GlobalClass._Today,
                            MovedRequestDate = GlobalClass._Today,
                            AccountDocumentType = AccountDocumentType.ReachedToStock
                        });
                        docLocation.AccountDocumentType = AccountDocumentType.StockToUnits;
                    }
                    item.Documetns.Add(doc);
                }

                item.Locations.Add(docLocation.Clone());
                item.StoreBill = storeBill;
                selectedCollection.Add(item);
                if (!_documents.ContainsKey(docLocation))
                {
                    _documents.Add(docLocation, doc);
                }
                if (!_storeBills.Contains(storeBill))
                {
                    _storeBills.Add(storeBill);
                }

                this.setAccountDocDetails(item, employee, _relatedAccounts[item.StoreBill], _relatedAccountId[item.StoreBill]);
                if (selectedAsset.Item1.IsSelected)
                {
                    var doc1 = item.Documetns.First();
                    this.setAccountDocDetailsForDoc(item, employee, _relatedAccountsDoc[doc1], _relatedAccountDocId[doc1]);
                }
            });

            try
            {
                _movableAssetService.InsertGraphRange(selectedCollection);
                _unitOfWork.SaveChanges();
                _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                _movableAssetCollection.Clear();
                _relatedAccountId.Clear();
                _relatedAccountDocId.Clear();
                _relatedAccounts.Clear();
                _relatedAccountsDoc.Clear();

                UserLog.UniqueInstance.AddLog(new EventLog
                {
                    EntryDate = GlobalClass._Today,
                    Key = UserLog.UniqueInstance.LogedUser.FullName,
                    Message = "ثبت مال با آپلود فایل اکسس",
                    ObjectState = ObjectState.Added,
                    Type = EventType.Update,
                    UserId = UserLog.UniqueInstance.LogedUser.UserId
                });
            }
            catch (DbUpdateException ex)
            {
                _dialogService.ShowError("Error", ex.InnerException.InnerException.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void setAccountDocDetails(UnConsumption assets, Employee emp, AccountDocumentMaster accountMaster, int accountMasterId)
        {
            if (assets != null || emp != null || accountMaster != null)
            {
                List<Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>> currentAccountCodings = new List<Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>>();
                var accountCodings = _employeeService.GetAccountCodings();
                var ma = assets;
                var parentcode = accountCodings.FirstOrDefault(x => x.Parent == null);
                ma.Locations.ForEach(l =>
                {
                    string desc = "نامشخص";
                    string code = "0";
                    if (l.AccountDocumentType == AccountDocumentType.EscrowToTrust)
                    {
                        var organ = Organizations.FirstOrDefault(o => o.BudgetNo.ToString() == ma.StoreBill.Desc1);
                        if (organ != null)
                        {
                            code = organ.BudgetNo.ToString();
                            desc = organ.Name;
                        }
                        currentAccountCodings.Add(
                           new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.Escrow),
                           "Creditor", desc, code, ma.Cost, ma));

                        currentAccountCodings.Add(new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(
                            accountCodings.First(x => x.CertainAccountType == CertainAccountsType.TrustOrganizationReciver),
                            "Debtor", emp.Name, emp.BudgetNo.ToString(), ma.Cost, ma));

                    }
                    else if (l.AccountDocumentType == AccountDocumentType.ExecutiveToReached)
                    {

                        if (ma.Label.HasValue)
                        {
                            code = ma.Label.ToString();
                            desc = "برچسب" + code;
                        }
                        currentAccountCodings.Add(
                            new Tuple<AccountDocumentCoding, string, string, string, decimal,
                            UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ExecutiveSequenceLabel),
                            "Creditor", desc, code, ma.Cost, ma));

                        if (ma.StoreBill.AcqType == StateOwnership.Purchase)
                        {
                            desc = "خریداری";
                            if (ma.StoreBill.SellerId.HasValue)
                            {
                                var seller = _sellerService.Find(ma.StoreBill.SellerId.Value);
                                desc += "**" + seller.Name;
                                code = seller.Coding;
                            }
                            currentAccountCodings.Add(
                                new Tuple<AccountDocumentCoding, string, string, string, decimal,
                                UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetBuy),
                                "Debtor", desc, code, ma.Cost, ma));
                        }
                        else if (ma.StoreBill.AcqType == StateOwnership.Owned)
                        {
                            desc = "تملیکی**" + ma.StoreBill.Desc1;
                            currentAccountCodings.Add(
                                new Tuple<AccountDocumentCoding, string, string, string, decimal,
                                UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetOther),
                                "Debtor", desc, code, ma.Cost, ma));
                        }
                        else if (ma.StoreBill.AcqType == StateOwnership.GovCompanyRecived)
                        {
                            var organ = Organizations.FirstOrDefault(o => o.BudgetNo.ToString() == ma.StoreBill.Desc1);
                            if (organ != null)
                            {
                                desc = "انتفالی**" + organ.Name;
                                code = organ.BudgetNo.ToString();
                            }

                            currentAccountCodings.Add(
                               new Tuple<AccountDocumentCoding, string, string, string, decimal,
                               UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetTransfer),
                               "Debtor", desc, code, ma.Cost, ma));
                        }
                        else if (ma.StoreBill.AcqType == StateOwnership.Donation)
                        {
                            desc = "اهدایی**" + ma.StoreBill.Desc1;
                            currentAccountCodings.Add(
                                new Tuple<AccountDocumentCoding, string, string, string, decimal,
                                UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetDenotion),
                                "Debtor", desc, code, ma.Cost, ma));
                        }
                    }
                    else if (l.AccountDocumentType == AccountDocumentType.ReachedToStock)
                    {
                        if (ma.StoreBill.AcqType == StateOwnership.Purchase)
                        {
                            desc = "خریداری";
                            if (ma.StoreBill.SellerId.HasValue)
                            {
                                var seller = _sellerService.Find(ma.StoreBill.SellerId.Value);
                                desc += "**" + seller.Name;
                                code = seller.Coding;
                            }
                            currentAccountCodings.Add(
                               new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetBuy),
                               "Creditor", desc, code, ma.Cost, ma));
                        }
                        else if (ma.StoreBill.AcqType == StateOwnership.Owned)
                        {
                            desc = "اهدایی**" + ma.StoreBill.Desc1;
                            currentAccountCodings.Add(
                               new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetOther),
                               "Creditor", desc, code, ma.Cost, ma));
                        }
                        else if (ma.StoreBill.AcqType == StateOwnership.GovCompanyRecived)
                        {
                            var organ = Organizations.FirstOrDefault(o => o.BudgetNo.ToString() == ma.StoreBill.Desc1);
                            if (organ != null)
                            {
                                desc = "انتقالی**" + organ.Name;
                                code = organ.BudgetNo.ToString();
                            }

                            currentAccountCodings.Add(
                              new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetTransfer),
                              "Creditor", desc, code, ma.Cost, ma));
                        }
                        else if (ma.StoreBill.AcqType == StateOwnership.Donation)
                        {
                            desc = "تملیکی**" + ma.StoreBill.Desc1;
                            currentAccountCodings.Add(
                              new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.ReachedAssetDenotion),
                              "Creditor", desc, code, ma.Cost, ma));
                        }
                        code = ma.KalaUid.ToString();
                        desc = ma.Name + "**" + ma.Cost.ToString("N0");
                        currentAccountCodings.Add(
                           new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.StockAssetBuyAndType),
                           "Debtor", desc, code, ma.Cost, ma));
                    }
                });

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
                    };

                    if (accountMaster != null)
                    {
                        newDetails.AccountDocumentMaster = accountMaster;
                    }
                    else
                    {
                        newDetails.MasterId = accountMasterId;
                    }

                    adc.Item6.AccountDocumentDetails.Add(newDetails);
                });
            }
        }

        private void setAccountDocDetailsForDoc(UnConsumption assets, Employee emp, AccountDocumentMaster accountMaster, int accountMasterId)
        {
            if (assets != null || emp != null || accountMaster != null)
            {
                List<Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>> currentAccountCodings = new List<Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>>();
                var accountCodings = _employeeService.GetAccountCodings();
                var ma = assets;
                var parentcode = accountCodings.FirstOrDefault(x => x.Parent == null);
                ma.Locations.ForEach(l =>
                {
                    string desc = "نامشخص";
                    string code = "0";

                    if (l.AccountDocumentType == AccountDocumentType.StockToUnits)
                    {
                        code = ma.KalaUid.ToString();
                        desc = ma.Name + "**" + ma.Cost.ToString("N0");
                        currentAccountCodings.Add(
                           new Tuple<AccountDocumentCoding, string, string, string, decimal, UnConsumption>(accountCodings.First(x => x.CertainAccountType == CertainAccountsType.StockAssetBuyAndType),
                           "Creditor", desc, code, ma.Cost, ma));

                        var organization = _employeeService.GetDesignById(l.OrganizId, 1);
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
                });

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
                    };

                    if (accountMaster != null)
                    {
                        newDetails.AccountDocumentMaster = accountMaster;
                    }
                    else
                    {
                        newDetails.MasterId = accountMasterId;
                    }

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
        #endregion

        #region commands

        public ICommand AccessCommand { get; private set; }
        public ICommand SelectCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        private void initializCommands()
        {
            AccessCommand = new MvvmCommand(
                (parameter) => { this.saveAccessFile(); },
                (parameter) => { return true; }
                );

            SelectCommand = new MvvmCommand(
                (parameter) => { this.relationToLocation(parameter); },
                (parameter) => { return true; }
                );

            SaveCommand = new MvvmCommand(
                 (parameter) => { this.saveMAsset(); },
                (parameter) => { return true; }
                );
        }
        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly INavigationService _navigationService;
        private readonly IPersonService _personService;
        private readonly IStuffService _stuffService;
        private readonly IEmployeeService _employeeService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IStoreBillService _storeBillService;
        private readonly IUnitService _unitService;
        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _organizCollection;
        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _strategyCollection;
        private readonly ObservableCollection<AccessUploadAssetModel> _movableAssetCollection;
        private readonly HashSet<StoreBill> _storeBills;
        private readonly Dictionary<Location, Document> _documents;
        private readonly Dictionary<StoreBill, AccountDocumentMaster> _relatedAccounts;
        private readonly Dictionary<StoreBill, int> _relatedAccountId;
        private readonly Dictionary<Document, AccountDocumentMaster> _relatedAccountsDoc;
        private readonly Dictionary<Document, int> _relatedAccountDocId;
        private readonly ISellerService _sellerService;

        private readonly Dictionary<int, Tuple<int?, string, int?>> _labels;
        private List<RequestPermit> _personPermit;
        private List<EmployeeDesign> _allOrganiz;
        private List<EmployeeDesign> _allStrategy;
        private List<Stuff> _stuffs;
        private readonly SeedDataHelper _seedDataHelper;
        private readonly List<Tuple<AccessUploadAssetModel, string, int, int>> _selectedAssets;

        #endregion
    }
}
