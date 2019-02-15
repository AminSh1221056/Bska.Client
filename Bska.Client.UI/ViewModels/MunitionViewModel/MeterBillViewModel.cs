
namespace Bska.Client.UI.ViewModels.MunitionViewModel
{
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.AssetEntity.MeterBills;
    using Bska.Client.Domain.Entity.AssetEntity.Meters;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ComponentModel.DataAnnotations;
    using System.Windows;
    using System.Windows.Input;
    using Bska.Client.UI.CustomAttributes;
    using Bska.Client.API.Infrastructure;
    using Bska.Client.UI.Helper;
    using System.Data.Entity.Infrastructure;
    using System.Windows.Data;
    using System.Data.Entity;
    public sealed class MeterBillListViewModel : BaseListViewModel<MeterBill>
    {
        #region ctor

        public MeterBillListViewModel(IUnityContainer container)
            : base(new List<BaseDetailsViewModel<MeterBill>>())
        {
            this._container = container;
            this._navigationService = _container.Resolve<INavigationService>();
            this._buildingService = _container.Resolve<IBuildingService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this.FilteredView = new CollectionViewSource { Source = Collection }.View;
            _items = new Dictionary<string, object>();
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
        public String SearchCriteria
        {
            get { return GetValue(() => SearchCriteria); }
            set
            {
                SetValue(() => SearchCriteria, value);
                this.searchBill(1);
            }
        }
        
        public Dictionary<string, object> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }

        public Dictionary<string, object> SelectedItems
        {
            get { return GetValue(() => SelectedItems); }
            set
            {
                SetValue(() => SelectedItems, value);
            }
        }
        #endregion

        #region methods

        private void initializObj()
        {
            Enum.GetValues(typeof(MeterType)).Cast<MeterType>().ForEach(s =>
            {
                _items.Add(s.GetDescription(), s);
            });
            SelectedItems = new Dictionary<string, object>();
            SelectedItems.Add("All", "All");
            this.initCollection();
        }

        private void initCollection()
        {
            Collection.Clear();
            _buildingService.Queryable().SelectMany(x => x.Meters).SelectMany(x => x.MeterBills).Include(mb => mb.Meter)
                .Include(mb => mb.Meter.Building)
                .ToList().ForEach(mb =>
                {
                    Collection.Add(new MeterBillDetailsViewModel(mb));
                });
        }

        public override void SelectedItemChanged()
        {
            //nothing
        }

        private void searchBill(int searchType)
        {
            if (searchType == 2)
            {
                if (SelectedItems.ContainsKey("All"))
                {
                    FilteredView.Filter = null;
                    return;
                }

                var mTypes = new HashSet<Type>();

                SelectedItems.ForEach(si =>
                {
                    MeterType mType =(MeterType) si.Value;
                    if (mType == MeterType.Gas) mTypes.Add(typeof(GasBill));
                    else if (mType == MeterType.Mobile) mTypes.Add(typeof(MobileBill));
                    else if (mType == MeterType.Power) mTypes.Add(typeof(PowerBill));
                    else if (mType == MeterType.Tell) mTypes.Add(typeof(TellBill));
                    else if (mType == MeterType.Water) mTypes.Add(typeof(WaterBill));
                });

                FilteredView.Filter = (obj) =>
                {
                    var bill = obj as MeterBillDetailsViewModel;
                    return mTypes.Contains(bill.CurrentEntity.GetType());
                };
            }
            else
            {
                FilteredView.Filter = (obj) =>
                {
                    var bill = obj as MeterBillDetailsViewModel;
                    return bill.BillRecognition.StartsWith(SearchCriteria);
                };
            }
        }

        private void showMeterBillWindow()
        {
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new AddMeterBillViewModel(_container,null);
            _navigationService.ShowMeterBillWindow(viewModel);
            this.initCollection();
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void editBillWindow(object parameter)
        {
            var detailsVm = parameter as MeterBillDetailsViewModel;
            if (detailsVm==null)
            {
                return;
            }
            this.Selected = detailsVm;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", this.Window);
            var viewModel = new AddMeterBillViewModel(_container,detailsVm,true);
            _navigationService.ShowMeterBillWindow(viewModel);
            this.initCollection();
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", this.Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void report()
        {
            Mouse.SetCursor(Cursors.Wait);

            var viewModel = new ReportViewModel();
            List<string> _sItmes = new List<string>();
            if (SelectedItems.ContainsKey("All"))
            {
                _sItmes.Add("2500");
            }
            else
            {
                SelectedItems.ForEach(x =>
                {
                    MeterType temp;
                    if (Enum.TryParse(x.Value.ToString(), out temp))
                    {
                        _sItmes.Add(((int)temp).ToString());
                    }

                });
            }
            string searchquery = null;
            if (!string.IsNullOrWhiteSpace(SearchCriteria))
            {
                searchquery = SearchCriteria;
            }
            viewModel.MeterBillListReport( searchquery, _sItmes.ToArray());
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);
        }
        #endregion

        #region commands

        public ICommand NewBillCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand ReportCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }
        private void initializCommands()
        {
            NewBillCommand = new MvvmCommand(
                (parameter) => { this.showMeterBillWindow(); },
                (paramter) => { return true; }
                );

            EditCommand = new MvvmCommand(
                (parameter) => { this.editBillWindow(parameter); },
                (parameter) => { return true; }
                );

            ReportCommand = new MvvmCommand(
                (parameter) => { this.report(); },
                (parameter) => { return true; }
                );

            SearchCommand = new MvvmCommand(
                (parameter) => { this.searchBill(2); },
              (parameter) => { return true; }
              );
        }
        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IBuildingService _buildingService;
        private Dictionary<string, object> _items;

        #endregion
    }

    public sealed class AddMeterBillViewModel : BaseViewModel
    {
        #region ctor

        public AddMeterBillViewModel(IUnityContainer container,MeterBillDetailsViewModel DetailsVm,Boolean isEditing=false)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._buildingService = _container.Resolve<IBuildingService>(new ParameterOverride("repository",_unitOfWork.Repository<Building>()));
            this._dialogService = _container.Resolve<IDialogService>();
            this.IsEditing = isEditing;
            this.CurrentMeterVm = DetailsVm;
            this.initalizObj();
            this.initializCommands();
        }

        #endregion

        #region peorpeties

        public MeterBillDetailsViewModel CurrentMeterVm
        {
            get { return GetValue(() => CurrentMeterVm); }
            private set
            {
                SetValue(() => CurrentMeterVm, value);
            }
        }

        public List<Meter> Meters
        {
            get { return GetValue(() => Meters); }
            set
            {
                SetValue(() => Meters, value);
            }
        }

        public Meter CurrentMeter
        {
            get { return GetValue(() => CurrentMeter); }
            set
            {
                SetValue(() => CurrentMeter, value);
                this.initalizMeterBill();
            }
        }

        public Boolean IsEditing
        {
            get { return GetValue(() => IsEditing); }
            set
            {
                SetValue(() => IsEditing, value);
            }
        }

        #endregion

        #region methods

        private void initalizObj()
        {
            if(!IsEditing)
            Meters = _buildingService.Queryable().SelectMany(x => x.Meters).ToList();
            else
            {
                if (CurrentMeterVm != null)
                {
                    Meters =new List<Meter>{CurrentMeterVm.CurrentEntity.Meter};
                    CurrentMeter = Meters.FirstOrDefault();
                }
            }
        }

        private void initalizMeterBill()
        {
            if (CurrentMeter == null) return;
            if (IsEditing) return;
            MeterBill meterBill = null;
            this.CurrentMeterVm = null;
            if (CurrentMeter is GasMeter) meterBill = new GasBill();
            else if (CurrentMeter is PowerMeter) meterBill = new PowerBill();
            else if (CurrentMeter is TellMeter) meterBill = new TellBill();
            else if (CurrentMeter is MobileMeter) meterBill = new MobileBill();
            else if (CurrentMeter is WaterMeter) meterBill = new WaterBill();

            this.CurrentMeterVm = new MeterBillDetailsViewModel(meterBill)
            {
                NowReadDate=GlobalClass._Today,
                AgoReadDate=GlobalClass._Today,
                PayRecognition="",
                BillRecognition="",
                CostEra=0,
                TaxCost=0,
                DebtorCost=0,
                PayDate=GlobalClass._Today,
                PayDateSpace=GlobalClass._Today,
            };
        }

        private void addMeterBill()
        {
            if (CurrentMeter == null)
            {
                _dialogService.ShowAlert("توجه", "هیچ کنتوری انتخاب نشده است");
                return;
            }

            if (CurrentMeterVm.HasErrors)
            {
                _dialogService.ShowError("خطا",ErrorMessages.Default.InputInvalid);
                return;
            }

            Boolean confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                Building building = null;
                var entity = CurrentMeterVm.CurrentEntity;
                if (IsEditing)
                {
                    building = CurrentMeterVm.CurrentEntity.Meter.Building;
                    entity.ObjectState = ObjectState.Modified;
                }
                else
                {
                    building = _buildingService.Find(CurrentMeter.BuildingId);
                    entity.ObjectState = ObjectState.Added;
                    entity.InsertDate = GlobalClass._Today;
                    entity.ModifiedDate = GlobalClass._Today;
                }

                CurrentMeter.MeterBills.Add(entity);
                building.Meters.Add(CurrentMeter);
                _buildingService.InsertOrUpdateGraph(building);
                try
                {
                    Mouse.SetCursor(Cursors.Wait);
                    _unitOfWork.SaveChanges();
                    Mouse.SetCursor(Cursors.Arrow);
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    this.initalizMeterBill();
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

        #endregion

        #region commands

        public ICommand SubmitCommand { get; private set; }

        private void initializCommands()
        {
            SubmitCommand = new MvvmCommand(
                (parameter) => { this.addMeterBill(); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDialogService _dialogService;
        private readonly IBuildingService _buildingService;

        #endregion
    }

    public sealed class MeterBillDetailsViewModel : BaseDetailsViewModel<MeterBill>
    {
        #region ctor

        public MeterBillDetailsViewModel(MeterBill currentEntity)
            : base(currentEntity)
        {
            Space = string.Format("{0}/{1}", this.Year??"خالی", this.Mounth??"خالی");
            this.initNums1();
            this.initNums2();
            this.initNums3();
            this.initTotalCost();
        }

        #endregion

        #region properties

        public Int32 MeterBillId
        {
            get { return CurrentEntity.MeterBillId; }
        }

        [Required(ErrorMessage = "تاریخ قرائت کنونی الزامی است")]
        public DateTime NowReadDate
        {
            get { return CurrentEntity.NowReadDate; }
            set
            {
                CurrentEntity.NowReadDate = value;
                ValidateProperty(value);
                OnPropertyChanged("NowReadDate");
            }
        }

        [Required(ErrorMessage = "تاریخ قرائت پیشین الزامی است")]
        public DateTime AgoReadDate
        {
            get { return CurrentEntity.AgoReadDate; }
            set
            {
                CurrentEntity.AgoReadDate = value;
                ValidateProperty(value);
                OnPropertyChanged("AgoReadDate");
            }
        }

        public String Year
        {
            get { return CurrentEntity.Year; }
            set
            {
                CurrentEntity.Year = value;
                OnPropertyChanged("Year");
            }
        }

        public string Mounth
        {
            get { return CurrentEntity.Mounth; }
            set
            {
                CurrentEntity.Mounth = value;
                OnPropertyChanged("Mounth");
            }
        }

       [Required(ErrorMessage = "مقدار وارد شده صحیح نیست")]
       [PositiveNumber(ErrorMessage = "مقدار وارد شده صحیح نیست")]
        public Decimal CostEra
        {
            get { return CurrentEntity.CostEra; }
            set
            {
                CurrentEntity.CostEra = value;
                ValidateProperty(value);
                OnPropertyChanged("CostEra");
                initTotalCost();
            }
        }

        [Required(ErrorMessage = "مقدار وارد شده صحیح نیست")]
        [PositiveNumber(ErrorMessage = "مقدار وارد شده صحیح نیست")]
       public Decimal TaxCost
       {
           get { return CurrentEntity.TaxCost; }
           set
           {
               CurrentEntity.TaxCost = value;
               ValidateProperty(value);
               OnPropertyChanged("TaxCost");
               initTotalCost();
           }
       }

        [Required(ErrorMessage = "مقدار وارد شده صحیح نیست")]
        [PositiveNumber(ErrorMessage = "مقدار وارد شده صحیح نیست")]
        public Decimal DebtorCost
        {
            get { return CurrentEntity.DebtorCost; }
            set
            {
                CurrentEntity.DebtorCost = value;
                ValidateProperty(value);
                OnPropertyChanged("DebtorCost");
                initTotalCost();
            }
        }

         [Required(ErrorMessage = "مقدار وارد شده صحیح نیست")]
        public Decimal OtehrCost
        {
            get { return CurrentEntity.OtehrCost; }
            set
            {
                CurrentEntity.OtehrCost = value;
                ValidateProperty(value);
                OnPropertyChanged("OtehrCost");
                initTotalCost();
            }
        }

         [Required(ErrorMessage = "شناسه قبض الزامی است")]
         [PositiveNumber(ErrorMessage = "مقدار وارد شده صحیح نیست")]
        public String BillRecognition
        {
            get { return CurrentEntity.BillRecognition; }
            set
            {
                CurrentEntity.BillRecognition = value;
                ValidateProperty(value);
                OnPropertyChanged("BillRecognition");
            }
        }

         [Required(ErrorMessage = "شناسه پرداخت الزامی است")]
        public String PayRecognition
        {
            get { return CurrentEntity.PayRecognition; }
            set
            {
                CurrentEntity.PayRecognition = value;
                ValidateProperty(value);
                OnPropertyChanged("PayRecognition");
            }
        }

        [Required(ErrorMessage = "مهلت پرداخت الزامی است")]
         public DateTime PayDateSpace
         {
             get { return CurrentEntity.PayDateSpace; }
             set
             {
                 CurrentEntity.PayDateSpace = value;
                 ValidateProperty(value);
                 OnPropertyChanged("PayDateSpace");
             }
         }

         [Required(ErrorMessage = "تاریخ پرداخت الزامی است")]
        public DateTime PayDate
        {
            get { return CurrentEntity.PayDate; }
            set
            {
                CurrentEntity.PayDate = value;
                ValidateProperty(value);
                OnPropertyChanged("PayDate");
            }
        }

         public String BankName
         {
             get { return CurrentEntity.BankName; }
             set
             {
                 CurrentEntity.BankName = value;
                 OnPropertyChanged("BankName");
             }
         }

         public String PersonAccountnumber
         {
             get { return CurrentEntity.PersonAccountnumber; }
             set
             {
                 CurrentEntity.PersonAccountnumber = value;
                 OnPropertyChanged("PersonAccountnumber");
             }
         }

         public String PursuitNum
         {
             get { return CurrentEntity.PursuitNum; }
             set
             {
                 CurrentEntity.PursuitNum = value;
                 OnPropertyChanged("PursuitNum");
             }
         }

         public Decimal TotalCost
         {
             get { return GetValue(() => TotalCost); }
             set
             {
                 SetValue(() => TotalCost, value);
             }
         }

         public Int32? Num1
         {
             get { return CurrentEntity.Num1; }
             set
             {
                 CurrentEntity.Num1 = value;
                 OnPropertyChanged("Num1");
                 this.initNums1();
             }
         }
         public Int32? Num2
         {
             get { return CurrentEntity.Num2; }
             set
             {
                 CurrentEntity.Num2 = value;
                 OnPropertyChanged("Num2");
                 this.initNums2();
             }
         }
         public Int32? Num3
         {
             get { return CurrentEntity.Num3; }
             set
             {
                 CurrentEntity.Num3 = value;
                 OnPropertyChanged("Num3");
                 this.initNums3();
             }
         }
         public Int32? Num4
         {
             get { return CurrentEntity.Num4; }
             set
             {
                 CurrentEntity.Num4 = value;
                 OnPropertyChanged("Num4");
                 this.initNums1();
             }
         }
         public Int32? Num5
         {
             get { return CurrentEntity.Num5; }
             set
             {
                 CurrentEntity.Num5 = value;
                 OnPropertyChanged("Num5");
                 this.initNums2();
             }
         }
         public Int32? Num6
         {
             get { return CurrentEntity.Num6; }
             set
             {
                 CurrentEntity.Num6 = value;
                 OnPropertyChanged("Num6");
                 this.initNums3();
             }
         }

         public Decimal? DNum1
         {
             get { return CurrentEntity.DNum1; }
             set
             {
                 CurrentEntity.DNum1 = value;
                 OnPropertyChanged("DNum1");
                 initTotalCost();
             }
         }

         public Decimal? DNum2
         {
             get { return CurrentEntity.DNum2; }
             set
             {
                 CurrentEntity.DNum2 = value;
                 OnPropertyChanged("DNum2");
                 initTotalCost();
             }
         }

         public Decimal? DNum3
         {
             get { return CurrentEntity.DNum3; }
             set
             {
                 CurrentEntity.DNum3 = value;
                 OnPropertyChanged("DNum3");
                 initTotalCost();
             }
         }

         public Decimal? DNum4
         {
             get { return CurrentEntity.DNum4; }
             set
             {
                 CurrentEntity.DNum4 = value;
                 OnPropertyChanged("DNum4");
                 initTotalCost();
             }
         }

         public Decimal? DNum5
         {
             get { return CurrentEntity.DNum5; }
             set
             {
                 CurrentEntity.DNum5 = value;
                 OnPropertyChanged("DNum5");
                 initTotalCost();
             }
         }

         public string Space
         {
             get;
             set;
         }

         public int? Nums1
         {
             get { return GetValue(() => Nums1); }
             set
             {
                 SetValue(() => Nums1, value);
             }
         }
         public int? Nums2
         {
             get { return GetValue(() => Nums2); }
             set
             {
                 SetValue(() => Nums2, value);
             }
         }
         public int? Nums3
         {
             get { return GetValue(() => Nums3); }
             set
             {
                 SetValue(() => Nums3, value);
             }
         }

        #endregion 

        #region methods

         private void initNums1()
         {
             if (CurrentEntity is PowerBill)
             {
                 if(Num4>=Num1)
                 Nums1 = Num4 - Num1;
             }
         }

         private void initNums2()
         {
             if (CurrentEntity is PowerBill)
             {
                 if (Num5 >= Num2)
                     Nums2 = Num5 - Num2;
             }
         }

         private void initNums3()
         {
             if (CurrentEntity is PowerBill)
             {
                 if (Num6 >= Num3)
                     Nums3 = Num6 - Num3;
             }
         }
         private void initTotalCost()
         {
             decimal tCost = CostEra + OtehrCost + DebtorCost + TaxCost;
             decimal itemCost =0;
             if (CurrentEntity is PowerBill)
             {
                 //nothing
             }
            if (CurrentEntity is WaterBill)
            {
                //nothing
            }
            else if (CurrentEntity is GasBill)
             {
                 itemCost= (DNum1??0) + (DNum2??0) +(DNum3??0);
             }
             else if (CurrentEntity is TellBill)
             {
                 itemCost = (DNum1??0) + (DNum2??0) + (DNum3??0) + (DNum4??0) +(DNum5??0);
             }
             else if (CurrentEntity is MobileBill)
             {
                 itemCost = (DNum1??0) + (DNum2??0) + (DNum3??0) - (DNum4??0);
             }
             TotalCost=tCost+itemCost;
         }

        #endregion

        #region commands
        #endregion

        #region fields

        #endregion
    }
}
