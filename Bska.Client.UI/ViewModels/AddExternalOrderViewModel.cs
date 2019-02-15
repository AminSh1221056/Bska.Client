
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.UI.Helper;
    using Data.Service;
    using Microsoft.Practices.Unity;
    using System;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Collections.ObjectModel;
    using Domain.Entity;
    using System.Net.Http;
    using System.Windows.Input;
    using API;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using CustomAttributes;
    using Services;
    using Common;
    using Bska.Client.Repository.Model;

    public sealed class AddExternalOrderViewModel : BaseViewModel
    {
        #region ctor

        public AddExternalOrderViewModel(IUnityContainer container)
        {
            this._container = container;
            this._stuffService = _container.Resolve<IStuffService>();
            this._stuffList = new ObservableCollection<Stuff>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._firstGeneration = new ObservableCollection<StuffTreeViewModel>();
            this._exOrderDetails = new ObservableCollection<ExternalOrderDetailsModel>();
            this._client = UserLog.UniqueInstance.Client;
            this._subUnits = new ObservableCollection<UnitTreeViewModel>();
            this._exOrderDetails = new ObservableCollection<ExternalOrderDetailsModel>();
            this.initializObj();
            this.initializCommands();
        }

        #endregion

        #region properties

        public ObservableCollection<StuffTreeViewModel> FirstGeneration
        {
            get { return _firstGeneration; }
        }

        public ObservableCollection<UnitTreeViewModel> SubUnits
        {
            get { return _subUnits; }
        }

        public StuffTreeViewModel SelectedNode
        {
            get { return GetValue(() => SelectedNode); }
            set
            {
                SetValue(() => SelectedNode, value);
                if (value != null)
                {
                    initStuffAsync();
                }
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
        public ObservableCollection<Stuff> StuffList
        {
            get { return _stuffList; }
        }

        public Stuff SelectedStuff
        {
            get { return GetValue(() => SelectedStuff); }
            set
            {
                SetValue(() => SelectedStuff, value);
                this.initUnits();
            }
        }

        [Required(ErrorMessage = "تعداد الزامی است")]
        [PositiveIntNumber(ErrorMessage = "تعداد وارد شده معتبر نیست")]
        public Int32 Num
        {
            get { return _num; }
            set
            {
                _num = value;
                ValidateProperty(value);
                OnPropertyChanged("Num");
            }
        }

        [Required(ErrorMessage = "واحد الزامی است")]
        [PositiveIntNumber(ErrorMessage = "واحد انتخابی معتبر نیست")]
        public Int32 UnitId
        {
            get { return _unitId; }
            set
            {
                _unitId = value;
                ValidateProperty(value);
                OnPropertyChanged("UnitId");
            }
        }

        [Required(ErrorMessage = "نوع درخواست الزامی است")]
        [PositiveIntNumber(ErrorMessage = "نوع درخواست انتخابی معتبر نیست")]
        public Int32 ExOrderTypeId
        {
            get { return _exOrderType; }
            set
            {
                _exOrderType = value;
                ValidateProperty(value);
                OnPropertyChanged("ExOrderTypeId");
                this.initOrgans();
            }
        }
        public Boolean IsEditableOrgan
        {
            get { return GetValue(() => IsEditableOrgan); }
            set
            {
                SetValue(() => IsEditableOrgan, value);
            }
        }

        public List<OrganizationModel> OrganNames
        {
            get { return GetValue(() => OrganNames); }
            set
            {
                SetValue(() => OrganNames, value);
            }
        }

        public OrganizationModel SelectedOrgan
        {
            get { return GetValue(() => SelectedOrgan); }
            set
            {
                SetValue(() => SelectedOrgan, value);
                if (value != null) clearOnSlecectChange();
            }
        }

        public String IdentificationCode
        {
            get { return GetValue(() => IdentificationCode); }
            set
            {
                SetValue(() => IdentificationCode, value);
            }
        }

        public ObservableCollection<ExternalOrderDetailsModel> ExOrderCollection
        {
            get { return _exOrderDetails; }
        }

        #endregion

        #region methods

        private async void initializObj()
        {
            this.Num = this.UnitId = _exOrderType = 0;
            IsEditableOrgan = false;
            await stuffGenerationAsync();
        }

        private void initUnits()
        {
            if (SelectedStuff == null) return;
            _subUnits.Clear();
            Units.Where(x => x.Parent == null && (x.StuffId == StuffType.None || x.StuffId == SelectedStuff.StuffType)).ForEach(x =>
            {
                _subUnits.Add(new UnitTreeViewModel(x, _container));
            });
        }

        private void initOrgans()
        {
            if (_exOrderType == 2002)
            {
                IsEditableOrgan = true;
            }
            else
            {
                SelectedOrgan = OrganNames.Where(x => x.Name == UserLog.UniqueInstance.LogedEmployee.ParentName).FirstOrDefault();
                IsEditableOrgan = false;
            }
        }

        private Boolean parentRecovery(Stuff parent)
        {
            bool isChild = false;
            if (parent.Equals(SelectedNode.StuffCurrent))
            {
                isChild = true;
            }
            else if (parent.Parent != null)
            {
                isChild = this.parentRecovery(parent.Parent);
            }
            return isChild;
        }

        private void clearOnSlecectChange()
        {
            bool confirm = true;
            if (_exOrderDetails.Count > 0)
            {
                confirm = _dialogService.AskConfirmation("پرسش", "تمام درخواست های داخل لیست پاک می شوند.آیا می خواهید ادامه دهید");
            }
            if (confirm)
                _exOrderDetails.Clear();
        }

        private Task stuffGenerationAsync()
        {
            _stuffList.Clear();
            _firstGeneration.Clear();
            var ts = new Task(() =>
            {
                _stuffService.Query(x => (x.Parent == null || x.IsStuff == true)).Include(x => x.Parent)
                           .Select().AsEnumerable().ForEach(s =>
                           {
                               DispatchService.Invoke
                               (() =>
                               {
                                   if (s.IsStuff) _stuffList.Add(s);
                                   else
                                   {
                                       if (s.Parent == null) _firstGeneration.Add(new StuffTreeViewModel(s, _container));
                                   }
                               });
                           });
            });
            ts.Start();
            return ts;
        }

        private void initStuffAsync()
        {
            _stuffList.Clear();
            Task.Run(() =>
            {
                _stuffService.Query().Include(x => x.Parent)
                    .Select(x => x).Where(x => x.IsStuff == true).ToList().ForEach(x =>
                    {
                        if (SelectedNode != null)
                        {
                            bool isTrue = parentRecovery(x.Parent);
                            if (isTrue)
                            {
                                DispatchService.Invoke(() =>
                                {
                                    _stuffList.Add(x);
                                });
                            }
                        }
                        else
                        {
                            DispatchService.Invoke(() =>
                            {
                                _stuffList.Add(x);
                            });
                        }
                    });
            });
        }

        private void addToLstCommand()
        {
            if (this.HasErrors || this.SelectedStuff == null)
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.InputInvalid);
                return;
            }

            if (this.SelectedOrgan == null)
            {
                _dialogService.ShowAlert("توجه", "انتخاب سازمان الزامی است");
                return;
            }

            var newExOrderDetails = new ExternalOrderDetailsModel
            {
                Num = this.Num,
                UnitId = this.UnitId,
                OrderType = this.ExOrderTypeId,
                StuffName = SelectedStuff.Name,
                StuffType = SelectedStuff.StuffType,
                State = 0,
                TargetEmployee = SelectedOrgan.BudgetNo
            };

            _exOrderDetails.Add(newExOrderDetails);
        }

        private async void saveOrder()
        {
            if (GlobalClass.CheckForInternetConnection(Settings.Default.ServerUrl))
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoInternetAccess);
                return;
            }

            if (_exOrderDetails.Count <= 0)
            {
                _dialogService.ShowAlert("توجه", "هیچ درخواستی در لیست موجود نیست");
                return;
            }

            bool confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
            if (confirm)
            {
                try
                {
                    await CreateOrderAsync($"Order/{IdentificationCode}", _exOrderDetails);
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    _exOrderDetails.Clear();
                }
                catch (HttpRequestException ex)
                {
                    _dialogService.ShowError("خطا در بروز رسانی اطلاعات", ex.Message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private async Task<Uri> CreateOrderAsync(string uri, IEnumerable<ExternalOrderDetailsModel> model)
        {
            HttpResponseMessage response = null;
            response = await _client.PostAsJsonAsync(uri, model);
            response.EnsureSuccessStatusCode();
            return response?.Headers.Location;
        }

        private void removeFormLst(object parameter)
        {
            var exOrderDetails = parameter as ExternalOrderDetailsModel;
            if (exOrderDetails != null)
            {
                if (_exOrderDetails.Contains(exOrderDetails))
                {
                    _exOrderDetails.Remove(exOrderDetails);
                }
            }
        }
        #endregion

        #region commands

        public ICommand AddListCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        private void initializCommands()
        {
            AddListCommand = new MvvmCommand(
                (parameter) => { this.addToLstCommand(); },
                (parameter) => { return true; }
                );

            SaveCommand = new MvvmCommand(
                (parameter) => { this.saveOrder(); },
                  (parameter) => { return true; }
                );

            DeleteCommand = new MvvmCommand(
                (parameter) => { this.removeFormLst(parameter); },
                 (parameter) => { return true; }
                );
        }
        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IStuffService _stuffService;
        private readonly ObservableCollection<StuffTreeViewModel> _firstGeneration;
        private readonly ObservableCollection<Stuff> _stuffList;
        private readonly HttpClient _client;
        private readonly IDialogService _dialogService;
        private readonly ObservableCollection<ExternalOrderDetailsModel> _exOrderDetails;
        private readonly ObservableCollection<UnitTreeViewModel> _subUnits;
        private Int32 _num;
        private Int32 _unitId;
        private Int32 _exOrderType;

        #endregion

    }
}
