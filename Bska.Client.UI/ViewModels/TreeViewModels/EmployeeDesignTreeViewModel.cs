
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Domain.Entity;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    public sealed class EmployeeDesignTreeViewModel : BaseViewModel
    {
        #region ctor

        public EmployeeDesignTreeViewModel(EmployeeDesign buildDesign, EmployeeDesignTreeViewModel parent)
            : this(buildDesign, parent, null)
        {
        }

        public EmployeeDesignTreeViewModel(EmployeeDesign buildDesign, EmployeeDesignTreeViewModel parent, IEnumerable<int> permit)
        {
            _buidldingDesign = buildDesign;
            _children =
              new ObservableCollection<EmployeeDesignTreeViewModel>(
                (_buidldingDesign.ChildNode.Select(child => new EmployeeDesignTreeViewModel(child, this, permit))).ToList());
            IsExpanded = true;
            _parent = parent;
            IsEnabled = permit != null ? permit.Contains(this.BuildingDesignCurrent.BuidldingDesignId) : true;
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
            }
        }

        public bool IsEditing
        {
            get { return GetValue(() => IsEditing); }
            set
            {
                SetValue(() => IsEditing, value);
            }
        }
        
        public string Name
        {
            get { return _buidldingDesign.Name; }
            set
            {
                if (_buidldingDesign.Name != value)
                {
                    _buidldingDesign.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public string Code
        {
            get { return _buidldingDesign.Code; }
            set
            {
                if (_buidldingDesign.Code != value)
                {
                    _buidldingDesign.Code = value;
                    OnPropertyChanged("Code");
                }
            }
        }

        public ObservableCollection<EmployeeDesignTreeViewModel> Children
        {
            get { return _children; }
        }

        public EmployeeDesign BuildingDesignCurrent
        {
            get { return _buidldingDesign; }
        }

        public EmployeeDesignTreeViewModel Parent
        {
            get { return this._parent ?? null; }
        }

        public bool HaveRole
        {
            get { return _buidldingDesign.HaveRole; }
            set
            {
                _buidldingDesign.HaveRole = value;
                OnPropertyChanged("HaveRole");
            }
        }

        #endregion

        #region methods

        #endregion

        #region commands
        #endregion

        #region fields

        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _children;
        private readonly EmployeeDesignTreeViewModel _parent;
        private readonly EmployeeDesign _buidldingDesign;

        #endregion
    }
}
