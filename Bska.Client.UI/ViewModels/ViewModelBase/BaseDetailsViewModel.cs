
namespace Bska.Client.UI.ViewModels
{
    public abstract class BaseDetailsViewModel<TEntity> : BaseViewModel
    {
        /// <summary> 
        /// Initializes a new instance of the class. 
        /// </summary> 
        /// <param name="currentEntity">The current entity.</param> 
        protected BaseDetailsViewModel(TEntity currentEntity)
        {
            this.CurrentEntity = currentEntity;
        }

        /// <summary> 
        /// Gets the current entity. 
        /// </summary> 
        /// <value>The current entity.</value> 
        public TEntity CurrentEntity { get; private set; }

    }
}
