
namespace Bska.Client.UI.ViewModels.TreeViewModels
{
    using Domain.Entity;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    public sealed class AccountCodingTreeViewModel : BaseViewModel
    {
        #region ctor

        public AccountCodingTreeViewModel(AccountDocumentCoding codingDesign, AccountCodingTreeViewModel parent)
        {
            _codingDesign = codingDesign;
            _children =
              new ObservableCollection<AccountCodingTreeViewModel>(
                (_codingDesign.Childeren.Select(child => new AccountCodingTreeViewModel(child, this))).ToList());
            IsExpanded = true;
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
            get { return _codingDesign.Name; }
            set
            {
                if (_codingDesign.Name != value)
                {
                    _codingDesign.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public string Code
        {
            get { return _codingDesign.AccountCode; }
            set
            {
                if (_codingDesign.AccountCode != value)
                {
                    _codingDesign.AccountCode = value;
                    OnPropertyChanged("Code");
                }
            }
        }

        public ObservableCollection<AccountCodingTreeViewModel> Children
        {
            get { return _children; }
        }

        public AccountDocumentCoding AccountDocumentCodingCurrent
        {
            get { return _codingDesign; }
        }

        public AccountCodingTreeViewModel Parent
        {
            get { return this._parent ?? null; }
        }

        #endregion

        #region methods
        #endregion

        #region commands
        #endregion

        #region fields


        private readonly ObservableCollection<AccountCodingTreeViewModel> _children;
        private readonly AccountCodingTreeViewModel _parent;
        private readonly AccountDocumentCoding _codingDesign;

        #endregion
    }
}
