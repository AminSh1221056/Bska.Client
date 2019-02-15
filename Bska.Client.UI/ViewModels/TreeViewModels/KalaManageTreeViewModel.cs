
namespace Bska.Client.UI.ViewModels.TreeViewModels
{
    using Bska.Client.Domain.Entity;
    using System;
    using System.Collections.ObjectModel;
    using Helper;
    using System.Linq;

    public sealed class KalaManageTreeViewModel : BaseViewModel
    {

       #region ctor
        private KalaManageTreeViewModel() { }

        public KalaManageTreeViewModel(Stuff stuff,bool isExpanded)
            : this(stuff, null,isExpanded)
        {
        }

        public KalaManageTreeViewModel(Stuff stuff,
            KalaManageTreeViewModel parent, bool isExpanded)
        {
            _stuff = stuff;
            _stuff.Description = _stuff.ToString();
            _parent = parent;
            _children = new ObservableCollection<KalaManageTreeViewModel>();
            this.StuffCurrent.Childeren.ForEach(ch =>
            {
                this._children.Add(new KalaManageTreeViewModel(ch, this, false));
            });
            IsExpanded = isExpanded;
            IsPerfect = this.StuffCurrent.OrganizationPefectStuffs.Any();
        }

        #endregion

        #region properties

        public Int32 StuffId
        {
            get { return _stuff.StuffId; }
        }

        public bool IsStuff
        {
            get { return _stuff.IsStuff; }
            set
            {
                _stuff.IsStuff = value;
                OnPropertyChanged("IsStuff");
            }
        }

        public Boolean IsPerfect
        {
            get { return GetValue(() => IsPerfect); }
            set
            {
                SetValue(() => IsPerfect, value);
            }
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
            }
        }

        public string Name
        {
            get { return _stuff?.Name; }
            set
            {
                _stuff.Name = value;
                OnPropertyChanged("Name");
            }
        }

        public ObservableCollection<KalaManageTreeViewModel> Children
        {
            get { return _children; }
        }

        public Stuff StuffCurrent
        {
            get { return _stuff; }
        }

        public KalaManageTreeViewModel Parent
        {
            get { return this._parent; }
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

        private readonly ObservableCollection<KalaManageTreeViewModel> _children;
        private readonly KalaManageTreeViewModel _parent;
        private readonly Stuff _stuff;

        #endregion
    }
}
