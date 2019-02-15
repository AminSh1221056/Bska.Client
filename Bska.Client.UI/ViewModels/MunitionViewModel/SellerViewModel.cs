
namespace Bska.Client.UI.ViewModels
{
    using API;
    using Client.API.Infrastructure;
    using Client.API.UnitOfWork;
    using Data.Service;
    using Domain.Entity;
    using Helper;
    using Microsoft.Practices.Unity;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity.Infrastructure;
    using System.Windows.Input;
    using System.Linq;
    using Common;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;

    public sealed class SellerViewModel : BaseListViewModel<Seller>
    {
        #region ctor

        public SellerViewModel(IUnityContainer container)
            : base(new List<BaseDetailsViewModel<Seller>>())
        {
            this._container = container;
            _unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            _sellerService = _container.Resolve<ISellerService>(new ParameterOverride("repository", _unitOfWork.Repository<Seller>()));
            _dialogService = _container.Resolve<IDialogService>();
            this.FilteredView= new CollectionViewSource { Source = Collection }.View;
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

        public Boolean IsRealSeller
        {
            get { return GetValue(() => IsRealSeller); }
            set
            {
                SetValue(() => IsRealSeller, value);
                this.initDetails();
            }
        }

        public SellerDetailsViewModel SellerDetailsVM
        {
            get { return GetValue(() => SellerDetailsVM); }
            private set
            {
                SetValue(() => SellerDetailsVM, value);
            }
        }
        public String SearchCriteria
        {
            get { return GetValue(() => SearchCriteria); }
            set
            {
                SetValue(() => SearchCriteria, value);
                this.searchSellers();
            }
        }
        #endregion

        #region methods

        private void initializObj()
        {
            Collection.Clear();
            _sellerService.Queryable().ToList()
                .ForEach(sr =>
                {
                    Collection.Add(new SellerDetailsViewModel(sr));
                });
            IsRealSeller = true;
        }

        private void initDetails()
        {
            this.Selected = null;
            if (IsRealSeller)
            {
                SellerDetailsVM = new SellerDetailsViewModel(new Seller { ObjectState=ObjectState.Added})
                {
                    Name="",
                    Lastname="",
                    Mobile="",
                    Coding="",
                    SelectedZone = "",
                    SelectedProvince = "",
                    SelectedTwonShip = "",
                    SelectedCity = "",
                };
            }
            else
            {
                SellerDetailsVM = new SellerDetailsViewModel(new Seller { ObjectState = ObjectState.Added })
                {
                    Name = "",
                    Mobile = "",
                    Tell = "",
                    Lastname = "",
                    Coding = "",
                    SelectedZone = "",
                    SelectedProvince = "",
                    SelectedTwonShip = "",
                    SelectedCity = "",
                };
            }
        }

        private void searchSellers()
        {
            this.FilteredView.Filter = (obj =>
            {
                var seller = obj as SellerDetailsViewModel;
                return seller.Name.StartsWith(SearchCriteria);
            });
        }

        public override void SelectedItemChanged()
        {
            if (Selected != null)
            {
                var item = (SellerDetailsViewModel)Selected;
                this.SellerDetailsVM = item;

                this.SellerDetailsVM.CurrentEntity.Province= item.SelectedProvince;
                this.SellerDetailsVM.CurrentEntity.TownShip = item.SelectedTwonShip;
                this.SellerDetailsVM.CurrentEntity.Zone = item.SelectedZone;
                this.SellerDetailsVM.CurrentEntity.City = item.SelectedCity;
            }
        }

        private void saveSeller()
        {
            if (this.SellerDetailsVM.HasErrors)
            {
                _dialogService.ShowError("خطا", ErrorMessages.Default.InputInvalid);
                return;
            }

            bool confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                if (Selected != null)
                {
                    SellerDetailsVM.CurrentEntity.ObjectState = ObjectState.Modified;
                    _sellerService.Update(SellerDetailsVM.CurrentEntity);
                }
                else
                {
                    if (IsRealSeller)
                    {
                        SellerDetailsVM.CurrentEntity.Type = SellerType.RealSeller;
                    }
                    else
                    {
                        SellerDetailsVM.CurrentEntity.Type = SellerType.LegalSeller;
                    }

                    _sellerService.Insert(SellerDetailsVM.CurrentEntity);
                }

                try
                {
                    Mouse.SetCursor(Cursors.Wait);
                    _unitOfWork.SaveChanges();
                    Mouse.SetCursor(Cursors.Arrow);
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    if (Selected == null)
                    {
                        Collection.Add(new SellerDetailsViewModel(SellerDetailsVM.CurrentEntity));
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
            }
        }

        private void deleteSeller(object parameter)
        {
            var item = parameter as SellerDetailsViewModel;
            if (item != null)
            {
                if (item.SellerId == 1)
                {
                    _dialogService.ShowAlert("توجه", "فروشنده دیفالت سیستم قابلیت حذف ندارد");
                    return;
                }

                bool confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
                if (confirm)
                {
                    item.CurrentEntity.ObjectState = ObjectState.Deleted;
                    try
                    {
                        _sellerService.Delete(item.CurrentEntity);
                        Mouse.SetCursor(Cursors.Wait);
                        _unitOfWork.SaveChanges();
                        Mouse.SetCursor(Cursors.Arrow);
                        _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                        Collection.Remove(item);
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
        }

        #endregion

        #region commands

        public ICommand SaveCommand { get; private set; }
        public ICommand NewCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        private void initializCommands()
        {
            SaveCommand = new MvvmCommand(
                (parameter) => { this.saveSeller(); },
                (parameter) => { return true; }
                );

            NewCommand = new MvvmCommand(
                (parameter) => { this.initDetails() ; },
                (parameter) => { return true; }
                );

            DeleteCommand= new MvvmCommand(
                (parameter) => { this.deleteSeller(parameter); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly ISellerService _sellerService;
        private readonly IDialogService _dialogService;

        #endregion
    }

    public sealed class SellerDetailsViewModel : BaseDetailsViewModel<Seller>
    {
        #region ctor

        public SellerDetailsViewModel(Seller currentEntity)
            : base(currentEntity)
        {
            this.Provinces = new List<Province>();
            this.Zones = new List<Zone>();
            this.Cities = new List<City>();
            this.TwonShips = new List<TwonShip>();
            _helper = new SeedDataHelper();
            this.initalizObj();
        }

        #endregion

        #region properties

        public Int32 SellerId
        {
            get { return CurrentEntity.SellerId; }
        }

        [Required(ErrorMessage ="نام الزامی است")]
        public string Name
        {
            get { return CurrentEntity.Name; }
            set
            {
                CurrentEntity.Name = value;
                ValidateProperty(value);
                OnPropertyChanged("Name");
            }
        }
        
        [Required(ErrorMessage = "این فیلد الزامی است")]
        public string Lastname
        {
            get { return CurrentEntity.Lastname; }
            set
            {
                CurrentEntity.Lastname = value;
                ValidateProperty(value);
                OnPropertyChanged("Lastname");
            }
        }
        
        public string Tell
        {
            get { return CurrentEntity.Tell; }
            set
            {
                CurrentEntity.Tell = value;
                OnPropertyChanged("Tell");
            }
        }

        [Required(ErrorMessage = "موبایل الزامی است الزامی است")]
        public string Mobile
        {
            get { return CurrentEntity.Mobile; }
            set
            {
                CurrentEntity.Mobile = value;
                ValidateProperty(value);
                OnPropertyChanged("Mobile");
            }
        }

        [Required(ErrorMessage = "کد اقتصادی الزامی است ")]
        public string Coding
        {
            get { return CurrentEntity.Coding; }
            set
            {
                CurrentEntity.Coding = value;
                ValidateProperty(value);
                OnPropertyChanged("Coding");
            }
        }

        public string AddressLine
        {
            get { return CurrentEntity.AddressLine; }
            set
            {
                CurrentEntity.AddressLine = value;
                OnPropertyChanged("AddressLine");
            }
        }

        [Required(ErrorMessage = "نام استان الزامی است")]
        public String SelectedProvince
        {
            get { return CurrentEntity.Province; }
            set
            {
                CurrentEntity.Province = value;
                ValidateProperty(value);
                OnPropertyChanged("SelectedProvince");
                this.TwonShips = _helper.GetTwonShips(value);
            }
        }

        [Required(ErrorMessage = "نام شهرستان الزامی است")]
        public String SelectedTwonShip
        {
            get { return CurrentEntity.TownShip; }
            set
            {
                CurrentEntity.TownShip = value;
                ValidateProperty(value);
                OnPropertyChanged("SelectedTwonShip");
                this.Zones = _helper.GetZones(value);
            }
        }

        [Required(ErrorMessage = "نام بخش الزامی است")]
        public String SelectedZone
        {
            get { return CurrentEntity.Zone; }
            set
            {
                CurrentEntity.Zone = value;
                ValidateProperty(value);
                OnPropertyChanged("SelectedZone");
                this.Cities = _helper.GetCities(value);
            }
        }

        [Required(ErrorMessage = "نام شهر الزامی است")]
        public String SelectedCity
        {
            get { return CurrentEntity.City; }
            set
            {
                CurrentEntity.City = value;
                ValidateProperty(value);
                OnPropertyChanged("SelectedCity");
            }
        }

        public List<Province> Provinces
        {
            get { return _provinces; }
            set
            {
                _provinces = value;
                OnPropertyChanged("Provinces");
            }
        }

        public List<City> Cities
        {
            get { return _cities; }
            set
            {
                _cities = value;
                OnPropertyChanged("Cities");
            }
        }

        public List<TwonShip> TwonShips
        {
            get { return _townShips; }
            set
            {
                _townShips = value;
                OnPropertyChanged("TwonShips");
            }
        }

        public List<Zone> Zones
        {
            get { return _zones; }
            set
            {
                _zones = value;
                OnPropertyChanged("Zones");
            }
        }
        
        public string FullName
        {
            get { return GetValue(() => FullName); }
            set
            {
                SetValue(() => FullName, value);
            }
        }

        public String Type
        {
            get { return GetValue(() => Type); }
            set
            {
                SetValue(() => Type, value);
            }
        }

        #endregion

        #region methods

        private void initalizObj()
        {
            this.Provinces = _helper.GetProvinces();
            if (CurrentEntity.Type == SellerType.RealSeller)
            {
                FullName = $"{Name} {Lastname}";
            }
            else
            {
                FullName=$"{Name}";
            }
            Type = CurrentEntity.Type.GetDescription();
        }
        
        #endregion

        #region commands
        #endregion

        #region fields

        private List<Province> _provinces;
        private List<City> _cities;
        private List<TwonShip> _townShips;
        private List<Zone> _zones;
        private SeedDataHelper _helper;

        #endregion

    }
}
