
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Common;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.UI.Helper;
    using Bska.Client.Data.Service;
    using Microsoft.Practices.Unity;
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Entity;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Data;
    using System.Windows.Controls;
    using Domain.Entity;
    using System.Threading.Tasks;
    using System.Data.Entity.Infrastructure;
    using Client.API.UnitOfWork;
    using System.IO;
    using Repository.Model;

    public sealed class AccessExportViewModel : BaseViewModel
    {
        #region ctor

        public AccessExportViewModel(IUnityContainer container)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>(new ParameterOverride("repository", _unitOfWork.Repository<MovableAsset>()));
            this._employeeService = _container.Resolve<IEmployeeService>(new ParameterOverride("repository",_unitOfWork.Repository<Employee>()));
            this._proceddingService = _container.Resolve<IProceedingService>(new ParameterOverride("repository", _unitOfWork.Repository<Proceeding>()));
            this._storeBillService = _container.Resolve<IStoreBillService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._procCollection = new ObservableCollection<Proceeding>();
            this._exportDetails = new ObservableCollection<ExportDetails>();
            this.ProceddingsItems = new CollectionViewSource { Source = ProcCollection }.View;
            this.ExportDetailsItems = new CollectionViewSource { Source = ExportDetailsCollection }.View;
            this._selectedProcs = new HashSet<Proceeding>();
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

        public String AccessFilePath
        {
            get { return GetValue(() => AccessFilePath); }
            set
            {
                SetValue(() => AccessFilePath, value);
            }
        }

        [Required( ErrorMessage="شماره نامه الزامی است")]
        public String LetterNo
        {
            get { return _leterNo; }
            set
            {
                _leterNo = value;
                ValidateProperty(value);
                OnPropertyChanged("LetterNo");
            }
        }

        public Boolean IsArrival
        {
            get { return GetValue(() => IsArrival); }
            set
            {
                SetValue(() => IsArrival, value);
                if (value)
                {
                    _tbNo = 1001;
                    this.getExportDetailsAsync("Arrival");
                }
            }
        }

        public Boolean IsArrivalHokm
        {
            get { return GetValue(() => IsArrivalHokm); }
            set
            {
                SetValue(() => IsArrivalHokm, value);
                if (value)
                {
                    _tbNo = 1002;
                    this.getExportDetailsAsync("Arrival_hokmmasrafi");
                }
            }
        }

        public Boolean IsPerm
        {
            get { return GetValue(() => IsPerm); }
            set
            {
                SetValue(() => IsPerm, value);
                if (value)
                {
                    _tbNo = 1005;
                    this.getProceedings();
                }
            }
        }

        public Boolean IsTransmitIn
        {
            get { return GetValue(() => IsTransmitIn); }
            set
            {
                SetValue(() => IsTransmitIn, value);
                if (value)
                {
                    _tbNo = 1003;
                    this.getExportDetailsAsync("TransmitIn");
                }
            }
        }

        public Boolean IsTransmitOut
        {
            get { return GetValue(() => IsTransmitOut); }
            set
            {
                SetValue(() => IsTransmitOut, value);
                if (value)
                {
                    _tbNo = 1004;
                    this.getProceedings();
                }
            }
        }

        public Boolean IsPermEdit
        {
            get { return GetValue(() => IsPermEdit); }
            set
            {
                SetValue(() => IsPermEdit, value);
                if (value)
                {
                    _tbNo = 1006;
                    this.getProceedings();
                }
            }
        }

        public ObservableCollection<Proceeding> ProcCollection
        {
            get { return _procCollection; }
        }

        public ObservableCollection<ExportDetails> ExportDetailsCollection
        {
            get { return _exportDetails; }
        }

        public Proceeding SelectedProc
        {
            get { return GetValue(() => SelectedProc); }
            set
            {
                SetValue(() => SelectedProc, value);
            }
        }

        public StoreBill SelectedBill
        {
            get { return GetValue(() => SelectedBill); }
            set
            {
                SetValue(() => SelectedBill, value);
            }
        }

        public ICollectionView ProceddingsItems { get; set; }
        public ICollectionView ExportDetailsItems { get; set; }
        public String SearchCriteria
        {
            get { return GetValue(() => SearchCriteria); }
            set
            {
                SetValue(() => SearchCriteria, value);
                this.searchItems();
            }
        }
        
        public ExportDetails SelectedExport
        {
            get { return GetValue(() => SelectedExport); }
            set
            {
                SetValue(() => SelectedExport, value);
            }
        }

        public PersianDate FromDate
        {
            get { return GetValue(() => FromDate); }
            set
            {
                SetValue(() => FromDate, value);
            }
        }

        public PersianDate ToDate
        {
            get { return GetValue(() => ToDate); }
            set
            {
                SetValue(() => ToDate, value);
            }
        }

        public string FolderPath
        {
            get { return GetValue(() => FolderPath); }
            set
            {
                SetValue(() => FolderPath, value);
            }
        }

        public Boolean IsNewFolder
        {
            get { return GetValue(() => IsNewFolder); }
            set
            {
                SetValue(() => IsNewFolder, value);
            }
        }
        #endregion

        #region methods

        private void initializObj()
        {
            LetterNo = null;
            IsArrival = true;
            IsNewFolder = true;
        }

        private void setAccessExportPath()
        {
            if (IsNewFolder)
            {
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (!string.IsNullOrEmpty(dialog.SelectedPath))
                    {
                        string temp = ToDate.Year + "-" + ToDate.Month + "-" + ToDate.Day;
                        string directory = "";
                        if (dialog.SelectedPath.Last() == '\\')
                            directory = dialog.SelectedPath + temp;
                        else
                            directory = dialog.SelectedPath + "\\" + temp;

                        this.AccessFilePath = directory + "\\Transdata.mdb";
                        FolderPath = directory;
                    }
                }
            }
            else
            {
               using(var dialog=new System.Windows.Forms.OpenFileDialog())
               {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (!string.IsNullOrEmpty(dialog.FileName))
                    {
                        AccessFilePath = dialog.FileName;
                        FolderPath= Path.GetDirectoryName(dialog.FileName);
                    }
                }
            }
        }

        private void getProceedings()
        {
            Mouse.SetCursor(Cursors.Wait);
            _procCollection.Clear();
            _selectedProcs.Clear();
            if (_tbNo == 1005)
            {
                _proceddingService.Query(p => p.State == ProceedingState.Confirmed && p.Type != ProceedingsType.EditRequest && !p.IsSended)
                    .Include(p => p.AssetProceedings).Select().ToList().ForEach(p =>
                {
                    p.AssetProceedings = p.AssetProceedings.Where(x => x.State == AssetProceedingState.InProgress).ToList();
                    _procCollection.Add(p);
                });
                this.getExportDetailsAsync("Perm");
            }
            else if (_tbNo == 1004)
            {
                _proceddingService.Query(p => p.State == ProceedingState.CompletedConfirm)
                    .Include(p => p.AssetProceedings).Select().ToList().ForEach(p =>
                {
                    p.AssetProceedings = p.AssetProceedings.Where(x => x.State == AssetProceedingState.InProgress).ToList();
                    _procCollection.Add(p);
                });
                this.getExportDetailsAsync("TransmitOut");
            }
            else if (_tbNo == 1006)
            {
                _proceddingService.Query(p => p.State == ProceedingState.Confirmed && p.Type==ProceedingsType.EditRequest && !p.IsSended)
                   .Include(p => p.AssetProceedings).Select().ToList().ForEach(p =>
                   {
                       p.AssetProceedings = p.AssetProceedings.Where(x => x.State == AssetProceedingState.InProgress).ToList();
                       _procCollection.Add(p);
                   });
                this.getExportDetailsAsync("PermEdit");
            }
            Mouse.SetCursor(Cursors.Arrow);
        }
        
        private void searchItems()
        {
            ExportDetailsItems.Filter = (obj =>
            {
                ExportDetails exd = obj as ExportDetails;
                return exd.FileNo.StartsWith(SearchCriteria);
            });
        }

        private async void getExportDetailsAsync(string tbName)
        {
            _exportDetails.Clear();
            Task ts = new Task(() =>
              {
                  var items= _employeeService.Queryable().SelectMany(e => e.ExportDetails).Include(ed=>ed.ExportDetailsMAsset).Include(ed=>ed.ExportDetailsProceeding)
                  .Include(ed=>ed.ExportDetailsMAsset.Select(edm=>edm.MAsset)).Include(ed => ed.ExportDetailsProceeding.Select(edm => edm.Proceeding))
                  .Include(ed => ed.ExportDetailsProceeding.Select(edm => edm.Proceeding.AssetProceedings))
                  .OrderByDescending(ed=>ed.InsertDate).Where(ed => ed.TbName == tbName).ToList();
                  MovableAsset fMAsset = null;
                    
                 if(string.Equals("Arrival_hokmmasrafi", tbName)) fMAsset = _movableAssetService.Queryable().OfType<InCommidity>().FirstOrDefault(ma => ma.ISCompietion == CompietionState.NotReported);
                  else fMAsset = _movableAssetService.Queryable().OfType<UnConsumption>().FirstOrDefault(ma => ma.ISCompietion == CompietionState.NotReported);

                  DispatchService.Invoke(() =>
                  {
                      items.ForEach(ed =>
                      {
                          ed.Description = ed.ToString("DESC",null);
                          _exportDetails.Add(ed);
                      });
                      if (fMAsset != null)
                      {
                          FromDate = fMAsset.InsertDate.PersianDateTime();
                      }
                      else
                      {
                          FromDate = PersianDate.Today;
                      }
                      ToDate = PersianDate.Today;
                  });
              });
            ts.Start();
            await ts;
        }

        private void createTable()
        {
            if (this.HasErrors)
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.InputInvalid);
                return;
            }

            string tbName = this.getTbName(_tbNo);
            bool hasOldLetterNo = ExportDetailsCollection.Any(x => x.TbName == tbName && x.FileNo == LetterNo);
            if (hasOldLetterNo)
            {
                _dialogService.ShowAlert("توجه", "شما قبلا خروجی با این نام جدول و این شماره نامه تهیه کرده اید لطفا شماره نامه را تغییر دهید");
                return;
            }

            bool hasPendingRow = ExportDetailsCollection.Where(x => x.State == ExportState.Pending).Any(x => x.TbName == tbName);
            if (hasPendingRow)
            {
                _dialogService.ShowAlert("توجه", "یک سطر بدون ثبت مجوز دارایی با حالت تایید نشده وجود دارد.لطفا آنرا تایید کرده یا به حالت عودت تغییر وضعیت بدهید");
                return;
            }

            if (string.IsNullOrEmpty(this.AccessFilePath))
            {
                _dialogService.ShowAlert("توجه", "مسیر ذخیره سازی مشخص نیست");
                return;
            }

            if (_tbNo == 1005 || _tbNo==1004 || _tbNo==1006)
            {
                if (_selectedProcs.Count <= 0)
                {
                    _dialogService.ShowAlert("توجه", "هیچ صورت جلسه ای انتخاب نشده است");
                    return;
                }
            }
            
            bool confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                if (Directory.Exists(FolderPath))
                {
                  bool directoryConfirm= _dialogService.AskConfirmation("توجه", "مسیر ذخیره سازی با توجه به تاریخ روز وجود دارد.اگر دیتابیس موجود در این مسیر جدول انتخاب شده را داشته باشد ،پاک شده و جدول جدید جایگزین خواهد شد در غیر این صورت یک جدول جدید به دیتابیس اضافه می شود آیا می خواهید ادامه دهید");
                    if (!directoryConfirm)
                    {
                        return;
                    }
                }
                else
                {
                    if (IsNewFolder)
                    {
                        Directory.CreateDirectory(FolderPath);
                    }
                }

                Mouse.SetCursor(Cursors.Wait);
                var employee = _employeeService.Queryable().Single();
                AccessDatabaseHelper helper = new AccessDatabaseHelper();
                bool tbSuccess = false;
                var exportDetails = new ExportDetails
                {
                    InsertDate = GlobalClass._Today,
                    ObjectState = Client.API.Infrastructure.ObjectState.Added,
                    SendType = ExportType.FileTransfer,
                    FileNo = this.LetterNo,
                    BillDate=ToDate.ToDateTime(),
                    Directory=FolderPath
                };

                DataTable dt = null;
                int havaleno = 0;
                int year;
                UInt32 cost=0;
                int budgetNo = 0;
                int billNo = 1;
                int myBudgetNo = UserLog.UniqueInstance.LogedEmployee.BudgetNo;
                string pToday = PersianDate.Today.ToString();
                if (_tbNo == 1001)
                {
                    dt = helper.creatDataTable(tbName, _tbNo);
                    List<UnConsumption> items = null;
                    items = _movableAssetService.Queryable().OfType<UnConsumption>().Include(x => x.StoreBill).Include(x => x.Locations)
                       .Where(x => (x.ISConfirmed && x.ISCompietion != CompietionState.Reported) && x.StoreBill.AcqType != StateOwnership.Trust && x.Label.HasValue).ToList();

                    if (items.Count <= 0)
                    {
                        _dialogService.ShowAlert("توجه", "هیچ مالی با وضعیت ارسال نشده یافت نشد");
                        return;
                    }

                    items.ForEach(ma =>
                    {
                        havaleno = ma.Locations.Count();
                        year = ma.StoreBill.ArrivalDate.PersianDateTime().Year;
                        UInt32.TryParse(ma.Cost.ToString("F0"), out cost);
                        if (!int.TryParse(ma.StoreBill.StoreBillNo, out billNo))
                        {
                            billNo = 1;
                        }

                        if (ma.StoreBill.AcqType == StateOwnership.GovCompanyRecived || ma.StoreBill.AcqType == StateOwnership.Trust)
                        {
                            budgetNo = int.Parse(ma.StoreBill.Desc1);
                        }
                        else
                        {
                            budgetNo = 0;
                        }

                        dt.Rows.Add(ma.KalaUid, (int)ma.CurState, (int)ma.StoreBill.AcqType, billNo, havaleno
                            , myBudgetNo, ma.Label.Value, 0, this.LetterNo, year, ma.Description ?? "", cost, 0, ma.Uid1 ?? "", ma.Uid2 ?? "", ma.Uid3 ?? "", ma.Uid4 ?? "",
                            ma.Desc1 ?? "", ma.Desc2 ?? "", ma.Desc3 ?? "", ma.Desc4 ?? "", budgetNo);
                        ma.ObjectState = Client.API.Infrastructure.ObjectState.Modified;
                        ma.ISCompietion = CompietionState.Reporting;
                        ma.ISConfirmed = true;
                        exportDetails.ExportDetailsMAsset.Add(new ExportDetailsMAsset
                        {
                            MAsset = ma,
                            ObjectState=Client.API.Infrastructure.ObjectState.Added,
                        });
                    });
                    exportDetails.TbName = tbName;
                    exportDetails.State = ExportState.Pending;
                }
                else if (_tbNo == 1002)
                {
                    dt = helper.creatDataTable(tbName, _tbNo);
                    List<InCommidity> items = null;
                    items = _movableAssetService.Queryable().OfType<InCommidity>().Include(x => x.StoreBill).Include(x => x.Locations)
                       .Where(x => (x.ISConfirmed && x.ISCompietion != CompietionState.Reported) && x.StoreBill.AcqType != StateOwnership.Trust).ToList();
                    if (items.Count <= 0)
                    {
                        _dialogService.ShowAlert("توجه", "هیچ مالی با وضعیت ارسال نشده یافت نشد");
                        return;
                    }

                    items.ForEach(ma =>
                    {
                        havaleno = ma.Locations.Count();
                        year = ma.StoreBill.ArrivalDate.PersianDateTime().Year;
                        UInt32.TryParse(ma.Cost.ToString("F0"), out cost);

                        if (!int.TryParse(ma.StoreBill.StoreBillNo, out billNo))
                        {
                            billNo = 1;
                        }

                        if (ma.StoreBill.AcqType == StateOwnership.GovCompanyRecived || ma.StoreBill.AcqType == StateOwnership.Trust)
                        {
                            budgetNo = int.Parse(ma.StoreBill.Desc1);
                        }
                        else
                        {
                            budgetNo = 0;
                        }

                        dt.Rows.Add(ma.KalaUid, (int)ma.CurState, (int)ma.StoreBill.AcqType, billNo, havaleno
                            , myBudgetNo, 0, 0, this.LetterNo, year, ma.Description ?? "", cost, 0, ma.Uid1 ?? "", ma.Uid2 ?? "", ma.Uid3 ?? "", ma.Uid4 ?? "",
                            ma.Desc1 ?? "", ma.Desc2 ?? "", ma.Desc3 ?? "", ma.Desc4 ?? "", budgetNo);
                        ma.ObjectState = Client.API.Infrastructure.ObjectState.Modified;
                        ma.ISCompietion = CompietionState.Reporting;
                        ma.ISConfirmed = true;
                        exportDetails.ExportDetailsMAsset.Add(new ExportDetailsMAsset
                        {
                            MAsset = ma,
                            ObjectState = Client.API.Infrastructure.ObjectState.Added,
                        });
                    });
                    exportDetails.TbName = tbName;
                    exportDetails.State = ExportState.Pending;
                }
                else if (_tbNo == 1005)
                {
                    int optTyp = 0;
                    int organFault = 0;

                    dt = helper.creatDataTable(tbName, _tbNo);
                    _selectedProcs.ForEach(p =>
                    {
                        p.AssetProceedings.Where(mp => mp.State != AssetProceedingState.IsRejected).ForEach(mp =>
                        {
                            optTyp = (int)mp.Proceeding.Type;
                            var ma = _movableAssetService.Queryable().Where(x => x.AssetId == mp.AssetId).Include(x => x.StoreBill).Include(x => x.Locations).Single();
                            if (mp.Proceeding.Type == ProceedingsType.Accident || mp.Proceeding.Type == ProceedingsType.Earthquake || mp.Proceeding.Type == ProceedingsType.Fire
                                || mp.Proceeding.Type == ProceedingsType.Flood || mp.Proceeding.Type == ProceedingsType.Theft)
                            {
                                optTyp = 17004;
                            }
                            else if (mp.Proceeding.Type == ProceedingsType.Delete)
                            {
                                if (ma.Locations.Last().Status == LocationStatus.Accident)
                                {
                                    if (mp.IsOrganFault)
                                    {
                                        organFault = 6901;
                                    }
                                    else
                                    {
                                        organFault = 6902;
                                    }
                                }
                                else
                                {
                                    organFault = 0;
                                }
                            }
                            else if (mp.Proceeding.Type == ProceedingsType.DefinitiveTransfer)
                            {
                                if (!int.TryParse(mp.Proceeding.Desc1, out budgetNo))
                                {
                                    budgetNo = 1;
                                }
                            }
                            else if (mp.Proceeding.Type == ProceedingsType.TrustTransfer)
                            {
                                if (!int.TryParse(mp.Proceeding.Desc1, out budgetNo))
                                {
                                    budgetNo = 1;
                                }
                            }
                            else if (mp.Proceeding.Type == ProceedingsType.Sale)
                            {

                            }

                            year = ma.StoreBill.ArrivalDate.PersianDateTime().Year;
                            UInt32.TryParse(ma.Cost.ToString("F0"), out cost);

                            if (!int.TryParse(ma.StoreBill.StoreBillNo, out billNo))
                            {
                                billNo = 1;
                            }

                            dt.Rows.Add(ma.KalaUid, 15001, (int)ma.StoreBill.AcqType, billNo, 0
                               , myBudgetNo, ma.Label ?? 0, 0, this.LetterNo, year, ma.Description ?? "", cost, 0, ma.Uid1 ?? "", ma.Uid2 ?? "", ma.Uid3 ?? "", ma.Uid4 ?? "",
                               ma.Desc1 ?? "", ma.Desc2 ?? "", ma.Desc3 ?? "", ma.Desc4 ?? "", optTyp, 1, pToday, LetterNo, pToday, 1, pToday, budgetNo, mp.AccidentDivanNo ?? "", pToday, organFault, 1, 1);
                        });
                        exportDetails.ExportDetailsProceeding.Add(new ExportDetailsProceeding
                        {
                            ObjectState=Client.API.Infrastructure.ObjectState.Added,
                            Proceeding=p
                        });
                        p.IsSended = true;
                        p.ObjectState = Client.API.Infrastructure.ObjectState.Modified;
                    });
                    exportDetails.TbName = tbName;
                    exportDetails.State = ExportState.Confirmed;
                }
                else if (_tbNo == 1003)
                {
                    dt = helper.creatDataTable(tbName, _tbNo);
                    int actTyp = 16001;

                    _movableAssetService.Queryable().Include(ma=>ma.StoreBill).Where(x=>(x is UnConsumption || x is InCommidity) && x.ISConfirmed).ToList().ForEach(ma =>
                    {
                        var sb = ma.StoreBill;
                        if (!int.TryParse(sb.StoreBillNo, out billNo))
                        {
                            billNo = 1;
                        }

                        if (sb.AcqType == StateOwnership.GovCompanyRecived || sb.AcqType == StateOwnership.Trust)
                        {
                            budgetNo = int.Parse(sb.Desc1);
                        }
                        else
                        {
                            budgetNo = 0;
                        }
                        year = sb.ArrivalDate.PersianDateTime().Year;
                        UInt32.TryParse(ma.Cost.ToString("F0"), out cost);
                        dt.Rows.Add(ma.KalaUid, (int)ma.CurState, (int)sb.AcqType, billNo, 0
                           , myBudgetNo, ma.Label ?? 0, 0, this.LetterNo, year, ma.Description ?? "", cost, ma.Uid1 ?? "", ma.Uid2 ?? "", ma.Uid3 ?? "", ma.Uid4 ?? "",
                           ma.Desc1 ?? "", ma.Desc2 ?? "", ma.Desc3 ?? "", ma.Desc4 ?? "", actTyp, budgetNo, ma.OrganLabel ?? 0, 1, pToday);
                        exportDetails.ExportDetailsMAsset.Add(new ExportDetailsMAsset
                        {
                            MAsset = ma,
                            ObjectState = Client.API.Infrastructure.ObjectState.Added,
                        });
                    });

                    exportDetails.TbName = tbName;
                    exportDetails.State = ExportState.Confirmed;
                }
                else if (_tbNo == 1004)
                {
                    int actTyp = 16002;
                    int permuid = 1;
                    dt = helper.creatDataTable(tbName, _tbNo);
                    _selectedProcs.ForEach(p =>
                    {
                        p.AssetProceedings.Where(mp => mp.State == AssetProceedingState.IsConfirmed).ForEach(mp =>
                        {
                            var ma = _movableAssetService.Queryable().Where(x => x.AssetId == mp.AssetId).Single();
                            if (mp.Proceeding.Type == ProceedingsType.Accident || mp.Proceeding.Type == ProceedingsType.Earthquake || mp.Proceeding.Type == ProceedingsType.Fire
                                || mp.Proceeding.Type == ProceedingsType.Flood || mp.Proceeding.Type == ProceedingsType.Theft)
                            {

                            }
                            else if (mp.Proceeding.Type == ProceedingsType.DefinitiveTransfer)
                            {
                                if (!int.TryParse(mp.Proceeding.Desc1, out budgetNo))
                                {
                                    budgetNo = 1;
                                }
                            }
                            else if (mp.Proceeding.Type == ProceedingsType.TrustTransfer)
                            {
                                if (!int.TryParse(mp.Proceeding.Desc1, out budgetNo))
                                {
                                    budgetNo = 1;
                                }
                            }
                            else if (mp.Proceeding.Type == ProceedingsType.Sale)
                            {

                            }

                            if (!int.TryParse(mp.LicenseNumber, out permuid))
                            {
                                permuid = 0;
                            }

                            dt.Rows.Add(myBudgetNo, ma.Label ?? 0, 0, this.LetterNo, actTyp, permuid, budgetNo, mp.RecipetNo ?? "", pToday, 1, mp.Price, mp.Price, 1);
                        });
                        exportDetails.ExportDetailsProceeding.Add(new ExportDetailsProceeding
                        {
                            ObjectState = Client.API.Infrastructure.ObjectState.Added,
                            Proceeding = p
                        });
                        p.IsSended = true;
                        p.ObjectState = Client.API.Infrastructure.ObjectState.Modified;
                    });
                    exportDetails.TbName = tbName;
                    exportDetails.State = ExportState.Confirmed;
                }
                else if (_tbNo == 1006)
                {
                    dt = helper.creatDataTable(tbName, _tbNo);
                    _selectedProcs.ForEach(p =>
                    {
                        p.AssetProceedings.Where(mp => mp.State != AssetProceedingState.IsRejected).ForEach(mp =>
                        {
                            var ma = _movableAssetService.Queryable().Where(x => x.AssetId == mp.AssetId).Include(x => x.StoreBill).Include(x => x.Locations).Single();

                            year = ma.StoreBill.ArrivalDate.PersianDateTime().Year;
                            UInt32.TryParse(ma.Cost.ToString("F0"), out cost);

                            if (!int.TryParse(ma.StoreBill.StoreBillNo, out billNo))
                            {
                                billNo = 1;
                            }
                            string pDate = mp.Proceeding.ProceedingDate.PersianDateString();

                            dt.Rows.Add(ma.KalaUid, (int)ma.CurState, myBudgetNo, ma.Label ?? 0, mp.ProceedingId.ToString(), pDate, this.LetterNo, year, ma.Description ?? "", cost, mp.TempUid1 ?? "", mp.TempUid2 ?? "", mp.TempUid3 ?? "", mp.TempUid4 ?? "",
                               mp.TempDesc1 ?? "", mp.TempDesc2 ?? "", mp.TempDesc3 ?? "", mp.TempDesc4 ?? "");
                        });
                        exportDetails.ExportDetailsProceeding.Add(new ExportDetailsProceeding
                        {
                            ObjectState = Client.API.Infrastructure.ObjectState.Added,
                            Proceeding = p
                        });
                        p.IsSended = true;
                        p.ObjectState = Client.API.Infrastructure.ObjectState.Modified;
                    });
                    exportDetails.TbName = tbName;
                    exportDetails.State = ExportState.Confirmed;
                }

                 employee.ExportDetails.Add(exportDetails);
                try
                {
                    _employeeService.InsertOrUpdateGraph(employee);
                    _unitOfWork.SaveChanges();
                    tbSuccess = helper.CreateTable(AccessFilePath, _tbNo);
                    if (tbSuccess)
                    {
                        helper.insertToTable(_tbNo, dt);
                        _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                        this.getExportDetailsAsync(exportDetails.TbName);
                    }
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
        
        private void verifiedExportItem(object parameter)
        {
            var exDetails = parameter as ExportDetails;
            if (exDetails != null)
            {
                this.SelectedExport = exDetails;
                if (string.IsNullOrEmpty(exDetails.VertifiedNo))
                {
                    _dialogService.ShowAlert("توجه", "شماره مجوز وارد نشده است");
                    return;
                }

                Boolean confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
                if (confirm)
                {
                    Mouse.SetCursor(Cursors.Wait);
                    var employee = _employeeService.Queryable().Single();
                    exDetails.State = ExportState.Confirmed;
                    exDetails.ObjectState = Client.API.Infrastructure.ObjectState.Modified;
                    var assets = _movableAssetService.GetMAssetsByExportDetails(exDetails.ID,false);
                    assets.ForEach(ma =>
                    {
                        ma.ISCompietion = CompietionState.Reported;
                        ma.ObjectState = Client.API.Infrastructure.ObjectState.Modified;
                    });
                    employee.ExportDetails.Add(exDetails);
                    _employeeService.InsertOrUpdateGraph(employee);
                    try
                    {
                        _unitOfWork.SaveChanges();
                        _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                        this.getExportDetailsAsync(exDetails.TbName);
                    }
                    catch (DbUpdateException ex)
                    {
                        _dialogService.ShowError("خطا", ex.InnerException.InnerException.Message);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    Mouse.SetCursor(Cursors.Arrow);
                }
            }
        }

        private void RejectExportRow(object parameter)
        {
            var exDetails = parameter as ExportDetails;
            if (exDetails != null)
            {
                this.SelectedExport = exDetails;
                Boolean confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
                if (confirm)
                {
                    Mouse.SetCursor(Cursors.Wait);
                    var employee = _employeeService.Queryable().Single();
                    exDetails.State = ExportState.Rejected;
                    exDetails.ObjectState = Client.API.Infrastructure.ObjectState.Modified;
                    var assets = _movableAssetService.GetMAssetsByExportDetails(exDetails.ID, false);
                    assets.ForEach(ma =>
                    {
                        ma.ISCompietion = CompietionState.NotReported;
                        ma.ObjectState = Client.API.Infrastructure.ObjectState.Modified;
                    });
                    employee.ExportDetails.Add(exDetails);
                    _employeeService.InsertOrUpdateGraph(employee);
                    try
                    {
                        _unitOfWork.SaveChanges();
                        _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                        this.getExportDetailsAsync(exDetails.TbName);
                    }
                    catch (DbUpdateException ex)
                    {
                        _dialogService.ShowError("خطا", ex.InnerException.InnerException.Message);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    Mouse.SetCursor(Cursors.Arrow);
                }
            }
        }

        private void showProceedingDetails(object parameter)
        {
            var proc = parameter as Proceeding;
            if (proc == null) return;
            SelectedProc = proc;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardHideWindow", this.Window);
            var viewModel = new ProceedingInformationViewModel(_container, proc.ProceedingId) { ConfirmEnabled=false};
            var window = _navigationService.ShowProceedingDetailsWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardShowWindow", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void showProcAssets(object parameter)
        {
            var proc = parameter as Proceeding;
            if (proc == null) return;
            SelectedProc = proc;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardHideWindow", this.Window);
            var viewModel = new MAssetListViewModel(_container,1001,proc.ProceedingId);
            Int64[] assetIds = proc.AssetProceedings.Where(x=>x.State==AssetProceedingState.InProgress).Select(x => x.AssetId).ToArray();
            viewModel.AssetList = _movableAssetService.Queryable().Where(x => assetIds.Contains(x.AssetId)).AsEnumerable().Select(x=>new MovableAssetModel
             {
                AssetId=x.AssetId,
                CurState=x.CurState,
                InsertDate=x.InsertDate,
                Name=x.Name,
                Num=x.Num,
                MAssetType=x.ToString("T",null),
                UnitId=x.UnitId,
                Label=x.Label,
                kalaUid=x.KalaUid,
             }).ToList();
            var window = _navigationService.ShowMAssetListWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardShowWindow", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }
        
        private void showExportMAssetList(object parameter)
        {
            Mouse.SetCursor(Cursors.Wait);
            var exDetails = parameter as ExportDetails;
            this.SelectedExport = exDetails;
            StoryboardManager.PlayStoryboard("StoryboardHideWindow", this.Window);
            int descTyp = 1005;
            var viewModel = new MAssetListViewModel(_container, descTyp, exDetails.ID);
            viewModel.Description = "لیست اموال فایل اکسس خروجی با شماره نامه " + exDetails.FileNo;
            if(string.Equals(exDetails.TbName, "PermEdit")|| string.Equals(exDetails.TbName, "Perm")
                || string.Equals(exDetails.TbName, "TransmitOut"))
            {
                viewModel.AssetList = _proceddingService.GetAssetsByExportDetails(exDetails.ID).Select(x => new MovableAssetModel
                {
                    AssetId = x.AssetId,
                    CurState = x.CurState,
                    InsertDate = x.InsertDate,
                    Name = x.Name,
                    Num = x.Num,
                    MAssetType = x.ToString("T", null),
                    UnitId = x.UnitId,
                    Label = x.Label,
                    kalaUid = x.KalaUid,
                }).ToList();
            }
            else
            {
                viewModel.AssetList = _movableAssetService.GetMAssetsByExportDetails(exDetails.ID, false).Select(x => new MovableAssetModel
                {
                    AssetId = x.AssetId,
                    CurState = x.CurState,
                    InsertDate = x.InsertDate,
                    Name = x.Name,
                    Num = x.Num,
                    MAssetType = x.ToString("T", null),
                    UnitId = x.UnitId,
                    Label = x.Label,
                    kalaUid = x.KalaUid,
                }).ToList();
            }
           
            _navigationService.ShowMAssetListWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardShowWindow", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void selectProc(object parameter)
        {
            var ch = parameter as CheckBox;
            var proc = ch.Tag as Proceeding;
            if (proc != null)
            {
                if (ch.IsChecked==true)
                {
                    if (!_selectedProcs.Contains(proc))
                    {
                        _selectedProcs.Add(proc);
                    }
                    proc.IsSended = true;
                }
                else
                {
                    if (_selectedProcs.Contains(proc))
                    {
                        _selectedProcs.Remove(proc);
                    }
                    proc.IsSended = false;
                }
            }
        }
        
        private void showOpenAccessWindow(object parameter)
        {
            var exDetails = parameter as ExportDetails;
            if (exDetails != null)
            {
                this.SelectedExport = exDetails;
                if (string.IsNullOrWhiteSpace(exDetails.Directory))
                {
                    _dialogService.ShowAlert("توجه", "جدول خروجی مشخص نیست");
                    return;
                }
                if (!Directory.Exists(exDetails.Directory))
                {
                    _dialogService.ShowAlert("توجه", "فایل مورد نظر موجود نیست یا پاک شده است");
                    return;
                }
                string temp = exDetails.Directory + "\\TransData.mdb";
                var viewModel = new ShowAccessFileViewModel(_container, exDetails.TbName, temp);
                StoryboardManager.PlayStoryboard("StoryboardHideWindow", this.Window);
                _navigationService.ShowAccessFileWindow(viewModel);
                StoryboardManager.PlayStoryboard("StoryboardShowWindow", this.Window);
            } 
        }

        private void createExportRowTable(object parameter)
        {
            var exDetails = parameter as ExportDetails;

            if (exDetails != null)
            {
                if (string.IsNullOrWhiteSpace(exDetails.Directory))
                {
                    _dialogService.ShowAlert("توجه", "مسیر ذخیره سازی مشخص نیست");
                    return;
                }

                this.SelectedExport = exDetails;
                bool confirm = _dialogService.AskConfirmation("پرسش", "اگر اکنون فایلی در مسیر "+exDetails.Directory+" وجود داشته باشد پاک شده و فایل جدید جایگزین خواهد شد آیا میخواهید ادامه دهید");
                if (confirm)
                {
                    Mouse.SetCursor(Cursors.Wait);
                    bool isModifid = false;
                    if (!Directory.Exists(exDetails.Directory))
                    {
                        try
                        {
                            Directory.CreateDirectory(exDetails.Directory);
                        }
                        catch (IOException ex)
                        {
                            if(ex.Message.Contains("The device is not ready"))
                            {
                                bool ask= _dialogService.AskConfirmation("پرسش", "مسیر ذخیره سازی روی این سیستم وجود ندارد آیا می خواهید فولدر جدید انتخاب کنید");
                                if (ask)
                                {
                                    using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                                    {
                                        System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                                        if (!string.IsNullOrEmpty(dialog.SelectedPath))
                                        {
                                            string temp = ToDate.Year + "-" + ToDate.Month + "-" + ToDate.Day;
                                            string directory = "";
                                            if (dialog.SelectedPath.Last() == '\\')
                                                directory = dialog.SelectedPath + temp;
                                            else
                                                directory = dialog.SelectedPath + "\\" + temp;

                                            exDetails.Directory = directory;
                                            exDetails.ObjectState = Client.API.Infrastructure.ObjectState.Modified;
                                            isModifid = true;
                                            Directory.CreateDirectory(exDetails.Directory);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                _dialogService.ShowAlert("Error", ex.Message);
                                return;
                            }
                        }
                        catch(Exception ex)
                        {
                            throw ex;
                        }
                    }

                    DataTable dt = null;
                    AccessDatabaseHelper helper = new AccessDatabaseHelper();
                    int havaleno = 0;
                    int year;
                    int budgetNo = 0;
                    int billNo = 1;
                    int myBudgetNo = UserLog.UniqueInstance.LogedEmployee.BudgetNo;
                    string pToday = PersianDate.Today.ToString();
                    if (string.Equals("Arrival", exDetails.TbName))
                    {
                        dt = helper.creatDataTable(exDetails.TbName, 1001);
                        List<UnConsumption> items = _movableAssetService.GetMAssetsByExportDetails(exDetails.ID, true).OfType<UnConsumption>().ToList();
                        items.ForEach(ma =>
                        {
                            havaleno = ma.Locations.Count();
                            year = ma.StoreBill.ArrivalDate.PersianDateTime().Year;
                            if (!int.TryParse(ma.StoreBill.StoreBillNo, out billNo))
                            {
                                billNo = 1;
                            }

                            if (ma.StoreBill.AcqType == StateOwnership.GovCompanyRecived || ma.StoreBill.AcqType == StateOwnership.Trust)
                            {
                                budgetNo = int.Parse(ma.StoreBill.Desc1);
                            }
                            else
                            {
                                budgetNo = 0;
                            }

                            dt.Rows.Add(ma.KalaUid, (int)ma.CurState, (int)ma.StoreBill.AcqType, billNo, havaleno
                                , myBudgetNo, ma.Label.Value, 0, exDetails.FileNo, year, ma.Description ?? "", ma.Cost, 0, ma.Uid1 ?? "", ma.Uid2 ?? "", ma.Uid3 ?? "", ma.Uid4 ?? "",
                                ma.Desc1 ?? "", ma.Desc2 ?? "", ma.Desc3 ?? "", ma.Desc4 ?? "", budgetNo);
                        });
                    }
                    else if (string.Equals("Arrival_hokmmasrafi", exDetails.TbName))
                    {
                        dt = helper.creatDataTable(exDetails.TbName, 1002);
                        List<InCommidity> items = _movableAssetService.GetMAssetsByExportDetails(exDetails.ID, true).OfType<InCommidity>().ToList();
                        items.ForEach(ma =>
                        {
                            havaleno = ma.Locations.Count();
                            year = ma.StoreBill.ArrivalDate.PersianDateTime().Year;

                            if (!int.TryParse(ma.StoreBill.StoreBillNo, out billNo))
                            {
                                billNo = 1;
                            }

                            if (ma.StoreBill.AcqType == StateOwnership.GovCompanyRecived || ma.StoreBill.AcqType == StateOwnership.Trust)
                            {
                                budgetNo = int.Parse(ma.StoreBill.Desc1);
                            }
                            else
                            {
                                budgetNo = 0;
                            }

                            dt.Rows.Add(ma.KalaUid, (int)ma.CurState, (int)ma.StoreBill.AcqType, billNo, havaleno
                                , myBudgetNo, 0, 0, exDetails.FileNo, year, ma.Description ?? "", ma.Cost, 0, ma.Uid1 ?? "", ma.Uid2 ?? "", ma.Uid3 ?? "", ma.Uid4 ?? "",
                                ma.Desc1 ?? "", ma.Desc2 ?? "", ma.Desc3 ?? "", ma.Desc4 ?? "", budgetNo);
                        });
                    }
                    else if (string.Equals("TransmitIn", exDetails.TbName))
                    {
                        dt = helper.creatDataTable(exDetails.TbName, 1003);
                        var items = _movableAssetService.GetMAssetsByExportDetails(exDetails.ID, true).ToList();
                        int actTyp = 16001;
                        items.ForEach(ma =>
                        {
                            var sb = ma.StoreBill;
                            if (!int.TryParse(sb.StoreBillNo, out billNo))
                            {
                                billNo = 1;
                            }

                            if (sb.AcqType == StateOwnership.GovCompanyRecived || sb.AcqType == StateOwnership.Trust)
                            {
                                budgetNo = int.Parse(sb.Desc1);
                            }
                            else
                            {
                                budgetNo = 0;
                            }
                            year = sb.ArrivalDate.PersianDateTime().Year;

                            dt.Rows.Add(ma.KalaUid, (int)ma.CurState, (int)sb.AcqType, billNo, 0
                               , myBudgetNo, ma.Label ?? 0, 0, exDetails.FileNo, year, ma.Description ?? "", ma.Cost, ma.Uid1 ?? "", ma.Uid2 ?? "", ma.Uid3 ?? "", ma.Uid4 ?? "",
                               ma.Desc1 ?? "", ma.Desc2 ?? "", ma.Desc3 ?? "", ma.Desc4 ?? "", actTyp, budgetNo, ma.OrganLabel ?? 0, 1, pToday);
                        });
                    }
                    else if(string.Equals("Perm", exDetails.TbName))
                    {
                        int optTyp = 0;
                        int organFault = 0;

                        dt = helper.creatDataTable(exDetails.TbName, 1005);
                        _proceddingService.GetProceedingByExportDetails(exDetails.ID).ForEach(p =>
                        {
                            p.AssetProceedings.Where(mp => mp.State != AssetProceedingState.IsRejected).ForEach(mp =>
                            {
                                optTyp = (int)mp.Proceeding.Type;
                                var ma = _movableAssetService.Queryable().Where(x => x.AssetId == mp.AssetId).Include(x => x.StoreBill).Include(x => x.Locations).Single();
                                if (mp.Proceeding.Type == ProceedingsType.Accident || mp.Proceeding.Type == ProceedingsType.Earthquake || mp.Proceeding.Type == ProceedingsType.Fire
                                    || mp.Proceeding.Type == ProceedingsType.Flood || mp.Proceeding.Type == ProceedingsType.Theft)
                                {
                                    optTyp = 17004;
                                }
                                else if (mp.Proceeding.Type == ProceedingsType.Delete)
                                {
                                    if (ma.Locations.Last().Status == LocationStatus.Accident)
                                    {
                                        if (mp.IsOrganFault)
                                        {
                                            organFault = 6901;
                                        }
                                        else
                                        {
                                            organFault = 6902;
                                        }
                                    }
                                    else
                                    {
                                        organFault = 0;
                                    }
                                }
                                else if (mp.Proceeding.Type == ProceedingsType.DefinitiveTransfer)
                                {
                                    if (!int.TryParse(mp.Proceeding.Desc1, out budgetNo))
                                    {
                                        budgetNo = 1;
                                    }
                                }
                                else if (mp.Proceeding.Type == ProceedingsType.TrustTransfer)
                                {
                                    if (!int.TryParse(mp.Proceeding.Desc1, out budgetNo))
                                    {
                                        budgetNo = 1;
                                    }
                                }
                                else if (mp.Proceeding.Type == ProceedingsType.Sale)
                                {

                                }

                                year = ma.StoreBill.ArrivalDate.PersianDateTime().Year;

                                if (!int.TryParse(ma.StoreBill.StoreBillNo, out billNo))
                                {
                                    billNo = 1;
                                }

                                dt.Rows.Add(ma.KalaUid, 15001, (int)ma.StoreBill.AcqType, billNo, 0
                                   , myBudgetNo, ma.Label ?? 0, 0, this.LetterNo, year, ma.Description ?? "", ma.Cost, 0, ma.Uid1 ?? "", ma.Uid2 ?? "", ma.Uid3 ?? "", ma.Uid4 ?? "",
                                   ma.Desc1 ?? "", ma.Desc2 ?? "", ma.Desc3 ?? "", ma.Desc4 ?? "", optTyp, 1, pToday, LetterNo, pToday, 1, pToday, budgetNo, mp.AccidentDivanNo ?? "", pToday, organFault, 1, 1);
                            });
                        });
                    }
                    else if (string.Equals("TransmitOut", exDetails.TbName))
                    {
                        int actTyp = 16002;
                        int permuid = 1;
                        dt = helper.creatDataTable(exDetails.TbName, 1004);
                        _proceddingService.GetProceedingByExportDetails(exDetails.ID).ForEach(p =>
                        {
                            p.AssetProceedings.Where(mp => mp.State == AssetProceedingState.IsConfirmed).ForEach(mp =>
                            {
                                var ma = _movableAssetService.Queryable().Where(x => x.AssetId == mp.AssetId).Single();
                                if (mp.Proceeding.Type == ProceedingsType.Accident || mp.Proceeding.Type == ProceedingsType.Earthquake || mp.Proceeding.Type == ProceedingsType.Fire
                                    || mp.Proceeding.Type == ProceedingsType.Flood || mp.Proceeding.Type == ProceedingsType.Theft)
                                {

                                }
                                else if (mp.Proceeding.Type == ProceedingsType.DefinitiveTransfer)
                                {
                                    if (!int.TryParse(mp.Proceeding.Desc1, out budgetNo))
                                    {
                                        budgetNo = 1;
                                    }
                                }
                                else if (mp.Proceeding.Type == ProceedingsType.TrustTransfer)
                                {
                                    if (!int.TryParse(mp.Proceeding.Desc1, out budgetNo))
                                    {
                                        budgetNo = 1;
                                    }
                                }
                                else if (mp.Proceeding.Type == ProceedingsType.Sale)
                                {

                                }

                                if (!int.TryParse(mp.LicenseNumber, out permuid))
                                {
                                    permuid = 0;
                                }

                                dt.Rows.Add(myBudgetNo, ma.Label ?? 0, 0, this.LetterNo, actTyp, permuid, budgetNo, mp.RecipetNo ?? "", pToday, 1, mp.Price, mp.Price, 1);
                            });
                        });
                    }
                    else if (string.Equals("PermEdit", exDetails.TbName))
                    {
                        dt = helper.creatDataTable(exDetails.TbName, 1006);
                        _proceddingService.GetProceedingByExportDetails(exDetails.ID).ForEach(p =>
                        {
                            p.AssetProceedings.Where(mp => mp.State != AssetProceedingState.IsRejected).ForEach(mp =>
                            {
                                var ma = _movableAssetService.Queryable().Where(x => x.AssetId == mp.AssetId).Include(x => x.StoreBill).Include(x => x.Locations).Single();

                                year = ma.StoreBill.ArrivalDate.PersianDateTime().Year;

                                if (!int.TryParse(ma.StoreBill.StoreBillNo, out billNo))
                                {
                                    billNo = 1;
                                }
                                string pDate = mp.Proceeding.ProceedingDate.PersianDateString();

                                dt.Rows.Add(ma.KalaUid, (int)ma.CurState, myBudgetNo, ma.Label ?? 0, mp.ProceedingId.ToString(), pDate, this.LetterNo, year, ma.Description ?? "", ma.Cost, mp.TempUid1 ?? "", mp.TempUid2 ?? "", mp.TempUid3 ?? "", mp.TempUid4 ?? "",
                                   mp.TempDesc1 ?? "", mp.TempDesc2 ?? "", mp.TempDesc3 ?? "", mp.TempDesc4 ?? "");
                            });
                        });
                    }
                    else
                    {
                        _dialogService.ShowAlert("توجه", "این جدول قابلیت بازیابی ندارد");
                        return;
                    }

                    bool tbSuccess = helper.CreateTable(exDetails.Directory+ "\\Transdata.mdb", _tbNo);
                    if (tbSuccess)
                    {
                        helper.insertToTable(_tbNo, dt);
                        if (isModifid)
                        {
                            var employee = _employeeService.Queryable().Single();
                            employee.ExportDetails.Add(exDetails);
                            _employeeService.InsertOrUpdateGraph(employee);
                            try
                            {
                                _unitOfWork.SaveChanges();
                                this.getExportDetailsAsync(exDetails.TbName);
                            }
                            catch (DbUpdateException ex)
                            {
                                _dialogService.ShowError("خطا", ex.InnerException.InnerException.Message);
                                return;
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                        _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    }
                }
            }
        }

        private string getTbName(int tbNo)
        {
            string tbName = "";
            if (tbNo == 1001)
            {
                tbName = "Arrival";
            }
            else if (tbNo == 1002)
            {
                tbName = "Arrival_hokmmasrafi";
            }
            else if (tbNo == 1003)
            {
                tbName = "TransmitIn";
            }
            else if (tbNo == 1004)
            {
                tbName = "TransmitOut";
            }
            else if (tbNo == 1005)
            {
                tbName = "Perm";
            }
            else if (tbNo == 1006)
            {
                tbName = "PermEdit";
            }
            else
            {
                return null;
            }

            return tbName;
        }

        #endregion

        #region commands

        public ICommand AccessFilePathCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }
        public ICommand ProceedingMAssetCommand { get; private set; }
        public ICommand ProceedingDetailsCommand { get; private set; }
        public ICommand SelectCommand { get; private set; }
        public ICommand AccessOpenCommand { get; private set; }
        public ICommand VerifiedCommand { get; private set; }
        public ICommand ExportDetailsCommand { get; private set; }
        public ICommand RowExportCommand { get; private set; }
        public ICommand RejectCommand { get; private set; }
        private void initializCommands()
        {
            AccessFilePathCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.setAccessExportPath();
                },
                (parameter) => { return true; }
                );

            ConfirmCommand = new MvvmCommand(
                (parameter) => { this.createTable(); },
                (parameter) => { return true; }
                );

            ProceedingDetailsCommand = new MvvmCommand(
                (parameter) => { this.showProceedingDetails(parameter); },
                (parameter) => { return true; }
                );

            ProceedingMAssetCommand = new MvvmCommand(
                (parameter) => { this.showProcAssets(parameter); },
                (parameter) => { return true; }
                );

            SelectCommand = new MvvmCommand(
                (parameter) => { this.selectProc(parameter); },
                (parameter) => { return true; }
                );
            
            AccessOpenCommand = new MvvmCommand(
                 (parameter) => { this.showOpenAccessWindow(parameter); },
                 (parameter) => { return true; }
                );

            VerifiedCommand = new MvvmCommand(
                 (parameter) => { this.verifiedExportItem(parameter); },
                 (parameter) => { return true; }
                );

            ExportDetailsCommand = new MvvmCommand(
                 (parameter) => { this.showExportMAssetList(parameter); },
                 (parameter) => { return true; });

            RowExportCommand = new MvvmCommand(
                 (parameter) => { this.createExportRowTable(parameter); },
                 (parameter) => { return true; }
                );

            RejectCommand = new MvvmCommand(
                  (parameter) => { this.RejectExportRow(parameter); },
                 (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IEmployeeService _employeeService;
        private readonly IProceedingService _proceddingService;
        private readonly INavigationService _navigationService;
        private readonly IStoreBillService _storeBillService;
        private readonly IUnitService _unitService;
        private readonly ObservableCollection<Proceeding> _procCollection;
        private readonly ObservableCollection<ExportDetails> _exportDetails;
        private readonly HashSet<Proceeding> _selectedProcs;
        private int _tbNo;
        private string _leterNo;

        #endregion

    }
}
