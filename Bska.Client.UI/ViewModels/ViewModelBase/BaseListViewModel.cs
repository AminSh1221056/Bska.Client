
namespace Bska.Client.UI.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Data;

    public abstract class BaseListViewModel<TEntity> : BaseViewModel
    {

        /// <summary> 
        /// Gets or sets the collection. 
        /// </summary> 
        /// <value>The collection.</value> 
        public ObservableCollection<BaseDetailsViewModel<TEntity>> Collection { get; private set; }

        public ICollectionView FilteredView { get; set; }
        /// <summary> 
        /// Gets or sets the selected. 
        /// </summary> 
        /// <value>The selected.</value>
        public BaseDetailsViewModel<TEntity> Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                SelectedItemChanged();
                OnPropertyChanged("Selected");
            }
        }

        /// <summary> 
        /// Initializes a new instance of the <see cref="BaseListViewModel&lt;TEntity&gt;"/>class. 
        /// </summary> 
        /// <param name="collection">The collection.</param>
        protected BaseListViewModel(List<BaseDetailsViewModel<TEntity>> collection)
        {
            this.Collection = new ObservableCollection<BaseDetailsViewModel<TEntity>>(collection);
        }

        public abstract void SelectedItemChanged();

        private BaseDetailsViewModel<TEntity> _selected;
    }
}
