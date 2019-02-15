
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Domain.Entity;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    public class StoreTreeViewModel : BaseViewModel
    {
        #region ctor

        public StoreTreeViewModel(StoreDesign storeDesign, StoreTreeViewModel parent, bool isExpanded)
        {
            _storeDesign = storeDesign;
            _parent = parent;

            _children =
              new ObservableCollection<StoreTreeViewModel>(
                (_storeDesign.ChildNode.Select(child => new StoreTreeViewModel(child, this, isExpanded))).ToList());
            IsExpanded = isExpanded;
            _parent = parent;
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

        public Boolean IsEditing
        {
            get { return GetValue(() => IsEditing); }
            set
            {
                SetValue(() => IsEditing, value);
            }
        }

        public string Name
        {
            get { return _storeDesign.Name; }
            set
            {
                if (_storeDesign.Name != value)
                {
                    _storeDesign.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public ObservableCollection<StoreTreeViewModel> Children
        {
            get { return _children; }
        }

        public StoreDesign StoreDesignCurrent
        {
            get { return _storeDesign; }
        }

        public StoreTreeViewModel Parent
        {
            get { return _parent; }
        }

        #endregion

        #region methods

        public bool NameContainsText(string text)
        {
            if (String.IsNullOrEmpty(text) || String.IsNullOrEmpty(this.Name))
                return false;

            return this.Name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        #endregion

        #region commands
        #endregion

        #region fields

        private readonly ObservableCollection<StoreTreeViewModel> _children;
        private readonly StoreTreeViewModel _parent;
        private readonly StoreDesign _storeDesign;

        #endregion
    }
}
