
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using System.Linq;
    public sealed class UnitTreeViewModel : BaseViewModel
    {
        #region ctor

        private UnitTreeViewModel() { }

        public UnitTreeViewModel(Unit unit, IUnityContainer container)
            : this(unit, container, null)
        {
        }

        public UnitTreeViewModel(Unit unit, IUnityContainer container, UnitTreeViewModel parent)
        {
            _unit = unit;
            _unit.Desctiption = _unit.ToString("urm", null);
            _parent = parent;
            _children = new ObservableCollection<UnitTreeViewModel>();

            _children.Add(DummyChild);

            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._unitService = _container.Resolve<IUnitService>();
        }

        #endregion

        #region properties

        public bool IsSelected
        {
            get { return GetValue(() => IsSelected); }
            set
            {
                SetValue(() => IsSelected, value);
            }
        }

        public bool IsEnabled
        {
            get { return GetValue(() => IsEnabled); }
            set
            {
                SetValue(() => IsEnabled, value);
            }
        }

        public bool IsExpanded
        {
            get { return GetValue(() => IsExpanded); }
            set
            {
                SetValue(() => IsExpanded, value);
                // Expand all the way up to the root.
                if (value && _parent != null)
                    _parent.IsExpanded = true;
                if (!this._dataLoaded && value)
                    this.GetChilderen();
            }
        }

        public string Name
        {
            get { return _unit.Name; }
            set
            {
                if (_unit.Name != value)
                {
                    _unit.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public Int32 UnitId
        {
            get { return _unit.UnitId; }
            set
            {
                _unit.UnitId = value;
                OnPropertyChanged("UnitId");
            }
        }
        public ObservableCollection<UnitTreeViewModel> Children
        {
            get { return _children; }
        }

        public Unit UnitCurrent
        {
            get { return _unit; }
        }

        public UnitTreeViewModel Parent
        {
            get { return this._parent ?? null; }
        }

        #endregion

        #region methods

        public void EditNode()
        {
            var unit = _unitService.Find(_unit.UnitId);
            unit.ObjectState = ObjectState.Modified;
            unit.Name = Name;
            _unitOfWork.Repository<Unit>().Update(unit);
            try
            {
                _unitOfWork.SaveChanges();
            }
            catch { }
        }

        private void GetChilderen()
        {
            Mouse.SetCursor(Cursors.Wait);

            this.Children.Clear();

            _unitService.Queryable().Where(x => x.Parent != null)
                .Where(x => x.Parent.UnitId == _unit.UnitId).ToList().ForEach(x =>
                {
                    this.Children.Add(new UnitTreeViewModel(x, _container,this));
                });

            this._dataLoaded = true;

            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IUnitService _unitService;
        private readonly ObservableCollection<UnitTreeViewModel> _children;
        private readonly UnitTreeViewModel _parent;
        private readonly Unit _unit;
        private Boolean _dataLoaded = false;
        static readonly UnitTreeViewModel DummyChild = new UnitTreeViewModel();

        #endregion
    }
}
