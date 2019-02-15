
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using System.Linq;
    public sealed class StuffTreeViewModel : BaseViewModel
    {
        #region ctor

        private StuffTreeViewModel() { }

        public StuffTreeViewModel(Stuff stuff, IUnityContainer container)
            : this(stuff, container, null)
        {
        }

        private StuffTreeViewModel(Stuff stuff, IUnityContainer container, StuffTreeViewModel parent)
        {
            _stuff = stuff;
            _container = container;
            _stuff.Description = _stuff.ToString();
            _parent = parent;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._stuffService = _container.Resolve<IStuffService>();
            _children = new ObservableCollection<StuffTreeViewModel>();

            _children.Add(DummyChild);
        }

        #endregion

        #region properties

        public Int32 StuffId
        {
            get { return _stuff.StuffId; }
        }

        public bool IsSelected
        {
            get { return GetValue(() => IsSelected); }
            set
            {
                SetValue(() => IsSelected, value);
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
            get { return _stuff.Name; }
            set
            {
                _stuff.Name = value;
                OnPropertyChanged("Name");
            }
        }
        
        public ObservableCollection<StuffTreeViewModel> Children
        {
            get { return _children; }
        }

        public Stuff StuffCurrent
        {
            get { return _stuff; }
        }

        public StuffTreeViewModel Parent
        {
            get { return this._parent; }
        }

        #endregion

        #region methods

        private void GetChilderen()
        {
            Mouse.SetCursor(Cursors.Wait);
            this.Children.Clear();
            if (StuffId == 3)
            {
                _stuffService.Query(x => x.Parent.StuffId == StuffId && x.IsStuff == false
                || (x.Parent.StuffId==2)).Select()
               .ToList().ForEach(x =>
               {
                   this.Children.Add(new StuffTreeViewModel(x, _container,this));
               });
            }
            else
            {
                _stuffService.Query(x => x.Parent.StuffId == StuffId && x.IsStuff == false).Select()
               .ToList().ForEach(x =>
               {
                   this.Children.Add(new StuffTreeViewModel(x, _container,this));
               });
            }

            this._dataLoaded = true;
            Mouse.SetCursor(Cursors.Arrow);
        }
        
        #endregion

        #region commands
        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IStuffService _stuffService;
        private readonly ObservableCollection<StuffTreeViewModel> _children;
        private readonly StuffTreeViewModel _parent;
        private readonly Stuff _stuff;
        private Boolean _dataLoaded = false;
        static readonly StuffTreeViewModel DummyChild = new StuffTreeViewModel();

        #endregion
    }
}
