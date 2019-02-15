
namespace Bska.Client.UI.ViewModels.StoreViewModel
{
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Data.Service;
    using Bska.Client.UI.Services;
    using Bska.Client.UI.Helper;
    using Bska.Client.UI.API;
    using Bska.Client.API.Infrastructure;
    using Bska.Client.Domain.Entity;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity.Infrastructure;
    using Bska.Client.Common;
    using Bska.Client.UI.ViewModels.TreeViewModels;

    public sealed class KalaManageViewModel : BaseViewModel
    {
        #region ctor

        public KalaManageViewModel(IUnityContainer container)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._stuffService = _container.Resolve<IStuffService>(new ParameterOverride("repository", _unitOfWork.Repository<Stuff>()));
            this._commodityService = _container.Resolve<IMAssetCommodityService>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._firstGeneration = new ObservableCollection<StuffTreeViewModel>();
            this._stuffList = new ObservableCollection<Stuff>();
            this.initializObj();
            this.initializCommands();
        }

        #endregion

        #region properties

        public ObservableCollection<StuffTreeViewModel> FirstGeneration
        {
            get { return _firstGeneration; }
        }

        public StuffTreeViewModel SelectedNode
        {
            get { return GetValue(() => SelectedNode); }
            set
            {
                SetValue(() => SelectedNode, value);
                this.initOnSelectedNode();
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
                this.generateLeftCode();
            }
        }
        
        [Required(ErrorMessage ="نام کالا الزامی است")]
        public string StuffName
        {
            get { return _stuffName; }
            set
            {
                _stuffName = value;
                ValidateProperty(value);
                OnPropertyChanged("StuffName");
            }
        }

        [Required(ErrorMessage = "کد کالا الزامی است")]
        public string StuffCode
        {
            get { return _stuffCode; }
            set
            {
                _stuffCode = value;
                ValidateProperty(value);
                OnPropertyChanged("StuffCode");
            }
        }

        public string GS1
        {
            get { return _gs1; }
            set
            {
                _gs1 = value;
                OnPropertyChanged("GS1");
            }
        }
        
        public string CurrentRightCode
        {
            get { return GetValue(() => CurrentRightCode); }
            set
            {
                SetValue(() => CurrentRightCode, value);
            }
        }
        #endregion

        #region methods

        private async void initializObj()
        {
            StuffCode = StuffName = "";
            _firstGeneration.Clear();
            await Task.Run(() =>
            {
                _stuffService.Queryable().ToList()
                .Where(x =>x.Parent == null).ForEach(x =>
                {
                    DispatchService.Invoke(() =>
                    {
                        _firstGeneration.Add(new StuffTreeViewModel(x, _container));
                    });
                });
            });
            this.initOnSelectedNode();
        }

        private async void initOnSelectedNode()
        {
            _stuffList.Clear();
            await Task.Run(() =>
            {
                _stuffService.Queryable()
                .Where(x => x.IsStuff).ToList().ForEach(x =>
                {
                    DispatchService.Invoke(() =>
                    {
                        if (this.SelectedNode !=null)
                        {
                            bool isTrue = this.parentRecovery(x);
                            if (isTrue)
                            {
                                _stuffList.Add(x);
                            }
                        }
                        else
                        {
                            _stuffList.Add(x);
                        }
                    });
                });
            });
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

        private void generateLeftCode()
        {
            if (SelectedStuff == null) return;
            var spItem = SelectedStuff.KalaNo.Split('-');
            if (spItem.Count() > 0)
            {
                CurrentRightCode = SelectedStuff.KalaNo.Replace('-', '0');
            }
            else
            {
                CurrentRightCode = SelectedStuff.KalaNo;
            }
            string code = "1";
            if (SelectedStuff.Childeren.Count > 0)
            {
                int count = SelectedStuff.Childeren.Count + 1;
                code = count.ToString();
            }
            StuffCode = code;
        }
        
        private void saveKala()
        {
            if (this.HasErrors)
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.InputInvalid);
                return;
            }

            if (SelectedStuff == null)
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoRowSelected);
                return;
            }

            if (SelectedStuff.Childeren.Any(x=>x.KalaNo==x.StuffId.ToString()))
            {
                _dialogService.ShowAlert("توجه", "شما میتوانید تنها برای آخرین شاخه کالا تعریف کنید");
                return;
            }
            var cstuff = SelectedStuff;
            cstuff.ObjectState = ObjectState.Modified;
            cstuff.IsStuff = false;
            var newStuff =new Stuff{StuffType=cstuff.StuffType,StuffId=cstuff.StuffId,
            Name=StuffName,KalaNo= CurrentRightCode + "-"+StuffCode,FloorNew=cstuff.FloorNew,FloorOld=cstuff.FloorOld,GS1=GS1,IsStuff=true,
            ObjectState=ObjectState.Added};

            cstuff.Childeren.Add(newStuff);
            try
            {
                Mouse.SetCursor(Cursors.Wait);
                _stuffService.InsertOrUpdateGraph(cstuff);
                _unitOfWork.SaveChanges();
                _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                SelectedStuff.IsStuff = false;
                SelectedStuff.Childeren.Add(newStuff);
                _stuffList.Add(newStuff);
                if (SelectedNode != null)
                {
                    if (SelectedStuff.Parent == SelectedNode.StuffCurrent)
                    {
                        if (!SelectedNode.Children.Any(ch => ch.StuffId != SelectedStuff.StuffId))
                        {
                            SelectedNode.Children.Add(new StuffTreeViewModel(SelectedStuff, _container));
                            SelectedNode.IsExpanded = true;
                        }
                    }
                }
                this.generateLeftCode();
                Mouse.SetCursor(Cursors.Arrow);
            }
            catch (DbUpdateException ex)
            {
                if (ex.Message.Contains("The EntityEntries property will return null"))
                {
                    _dialogService.ShowError("خطا", "کد کالا تکراری است");
                }
                else
                {
                    _dialogService.ShowError("خطا", ex.Message);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void deletekala()
        {
            if (SelectedStuff == null)
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoRowSelected);
                return;
            }

            if (SelectedStuff.Childeren.Count > 0 
                || string.Equals(SelectedStuff.KalaNo, SelectedStuff.StuffId.ToString()))
            {
                _dialogService.ShowAlert("توجه", "این شاخه  قابلیت حذف ندارد");
                return;
            }

            if (SelectedStuff.StuffType ==StuffType.Consumable)
            {
                if (_commodityService.Queryable().Any(co => co.KalaNo == SelectedStuff.KalaNo))
                {
                    _dialogService.ShowError("خطا", "از این نوع کالا در سیستم موجود می باشد و قابلیت حذف ندارد");
                    return;
                }
            }
            else
            {
                if (_movableAssetService.Queryable().Any(co => co.KalaNo == SelectedStuff.KalaNo))
                {
                    _dialogService.ShowError("خطا", "از این نوع کالا در سیستم موجود می باشد و قابلیت حذف ندارد");
                    return;
                }
            }

            var cstuff = SelectedStuff;
            var parent = SelectedStuff.Parent;
            parent.Childeren.Remove(SelectedStuff);
            cstuff.ObjectState = ObjectState.Deleted;
            _stuffService.Delete(cstuff);

            if (parent.Childeren.Count <= 0)
            {
                var pStuff = parent;
                pStuff.IsStuff = true;
                pStuff.ObjectState = ObjectState.Modified;
                _stuffService.Update(pStuff);
                parent.IsStuff = true;
            }

            try
            {
                Mouse.SetCursor(Cursors.Wait);
                _unitOfWork.SaveChanges();
                _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                this.generateLeftCode();
                Mouse.SetCursor(Cursors.Arrow);
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

        #endregion

        #region commands

        public ICommand StuffTreeSearchCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand ReportCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }

        private void initializCommands()
        {
            SaveCommand= new MvvmCommand(
                (parameter) => { this.saveKala(); },
                (parameter) => { return true; }
                );

            DeleteCommand= new MvvmCommand(
                (parameter) => { this.deletekala(); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IStuffService _stuffService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IMAssetCommodityService _commodityService;
        private readonly IDialogService _dialogService;
        private readonly ObservableCollection<Stuff> _stuffList;
        private readonly ObservableCollection<StuffTreeViewModel> _firstGeneration;
        private string _stuffName;
        private string _stuffCode;
        private string _gs1;

        #endregion
    }
}
