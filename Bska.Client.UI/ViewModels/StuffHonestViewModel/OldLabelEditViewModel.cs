
namespace Bska.Client.UI.ViewModels.StuffHonestViewModel
{
    using Microsoft.Practices.Unity;
    using Repository.Model;
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using API;
    using System.Windows.Controls;
    using Services;
    using System.Windows;
    using Data.Service;

    public sealed class OldLabelEditViewModel : BaseViewModel
    {
        #region ctor

        public OldLabelEditViewModel(IUnityContainer container,List<OldLabelDetailsViewModel> items
            ,int kalaUid)
        {
            this._container = container;
            this._movableAssetService = _container.Resolve<IMovableAssetService>();
            this._stuffService = _container.Resolve<IStuffService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._collection = new ObservableCollection<OldLabelDetailsViewModel>(items);
            this._kalaUid = kalaUid;
            this.initalizObj();
            this.initializCommand();
        }

        #endregion

        #region properties

        public string StuffName
        {
            get { return GetValue(() => StuffName); }
            set
            {
                SetValue(() => StuffName, value);
            }
        }
        
        public Int32 Num
        {
            get { return GetValue(() => Num); }
            set
            {
                SetValue(() => Num, value);
            }
        }
        
        public ObservableCollection<OldLabelDetailsViewModel> Collection
        {
            get { return _collection; }
        }

        public OldLabelDetailsViewModel SelectedItem
        {
            get { return GetValue(() => SelectedItem); }
            set
            {
                SetValue(() => SelectedItem, value);
            }
        }

        public Boolean IsRbNewOldEnable
        {
            get { return GetValue(() => IsRbNewOldEnable); }
            set
            {
                SetValue(() => IsRbNewOldEnable, value);
            }
        }

        public Boolean IsRbAgoOldEnable
        {
            get { return GetValue(() => IsRbAgoOldEnable); }
            set
            {
                SetValue(() => IsRbAgoOldEnable, value);
            }
        }
        #endregion

        #region methods

        private void initalizObj()
        {
            if (APPSettings.Default.OldSystemFloorType == 701)
            {
                IsRbNewOldEnable = true;
                IsRbAgoOldEnable = true;
            }
            else if (APPSettings.Default.OldSystemFloorType == 704)
            {
                IsRbAgoOldEnable = true;
            }
            else
            {
                IsRbNewOldEnable = true;
            }
        }

        private void initNewOldLabel(object parameter)
        {
            RadioButton rb = parameter as RadioButton;
            var labelEdit = rb.Tag as OldLabelDetailsViewModel;
            
            if (rb.IsChecked==true)
            {
                int index = _collection.IndexOf(labelEdit);

                labelEdit.Floors = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13" };
                var stuff = _stuffService.Queryable().Single(x => x.StuffId == _kalaUid);
                if (stuff != null)
                {
                    if (stuff.FloorNew.HasValue)
                        labelEdit.Floor = stuff.FloorNew.Value.ToString();
                }
                labelEdit.OldLabel = 0;
                _collection.RemoveAt(index);
                _collection.Insert(index, labelEdit);
                SelectedItem = labelEdit;
            }
        }

        private void initOldLabel(object parameter)
        {
            RadioButton rb = parameter as RadioButton;
            var labelEdit = rb.Tag as OldLabelDetailsViewModel;
            if (rb.IsChecked == true)
            {
                int index = _collection.IndexOf(labelEdit);
                labelEdit.Floors = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13","14" };
                var stuff = _stuffService.Queryable().Single(x => x.StuffId == _kalaUid);
                if (stuff != null)
                {
                    if(stuff.FloorOld.HasValue)
                    labelEdit.Floor = stuff.FloorOld.Value.ToString();
                }

                labelEdit.OldLabel = 0;
                _collection.RemoveAt(index);
                _collection.Insert(index, labelEdit);
                SelectedItem = labelEdit;
            }
        }
        
        private void confirmOldLabels(object parameter)
        {
            var win = parameter as Window;
            if (win != null)
            {
                Boolean confirm = _dialogService.AskConfirmation("پرسش", ErrorMessages.Default.AskConfrimation);
                if (confirm)
                {
                    win.DialogResult = true;
                }
            }
        }

        #endregion

        #region commands

        public ICommand NewLabelCommand { get; private set; }
        public ICommand OldLabelCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }
        private void initializCommand()
        {
            NewLabelCommand = new MvvmCommand(
                (parameter) => { this.initNewOldLabel(parameter); },
                (parameter) => { return true; }
                );

            OldLabelCommand= new MvvmCommand(
                (parameter) => { this.initOldLabel(parameter); },
                (parameter) => { return true; }
                );

            ConfirmCommand = new MvvmCommand(
                (parameter) => { this.confirmOldLabels(parameter); },
                (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IStuffService _stuffService;
        private readonly ObservableCollection<OldLabelDetailsViewModel> _collection;
        private readonly int _kalaUid;

        #endregion
    }

    public sealed class OldLabelDetailsViewModel: BaseDetailsViewModel<OldLabelModel>
    {

        #region ctor
        public OldLabelDetailsViewModel(OldLabelModel currentEntity)
            : base(currentEntity)
        {

        }

        #endregion

        #region properties

        public int Label
        {
            get { return CurrentEntity.Label; }
        }

        public int FloorType
        {
            get { return CurrentEntity.FloorType; }
            set
            {
                CurrentEntity.FloorType = value;
                OnErrorsChanged("FloorType");
                checkOldLabels();
            }
        }

        public string Floor
        {
            get { return CurrentEntity.Floor; }
            set
            {
                CurrentEntity.Floor = value;
                OnPropertyChanged("Floor");
                checkOldLabels();
            }
        }

        public int OldLabel
        {
            get { return CurrentEntity.OldLabel; }
            set
            {
                CurrentEntity.OldLabel = value;
                OnPropertyChanged("OldLabel");
                checkOldLabels();
            }
        }

        public List<string> Floors
        {
            get { return CurrentEntity.Floors; }
            set
            {
                CurrentEntity.Floors = value;
                OnPropertyChanged("Floors");
            }
        }
        #endregion

        #region methods

        private void checkOldLabels()
        {
            string key =Floor + "*" +FloorType;
            if (OldLabel > 0 && !string.IsNullOrEmpty(Floor))
            {
                if (_oldLabels.ContainsKey(key))
                {
                    if (_oldLabels[key].Contains(OldLabel))
                    {
                        System.Windows.MessageBox.Show("این شماره برچسب قبلا ثبت شده است", "توجه",System.Windows.MessageBoxButton.OK,System.Windows.MessageBoxImage.Error);
                        OldLabel = 0;
                    }
                    else
                    {
                        _oldLabels[key].Add(OldLabel);
                    }
                }
                else
                {
                    _oldLabels.Add(key, new List<int> {OldLabel });
                }
            }
        }

        #endregion

        #region commands
        #endregion

        #region fields
        internal Dictionary<string, List<int>> _oldLabels;
        #endregion
    }
}
