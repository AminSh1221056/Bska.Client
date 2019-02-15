
namespace Bska.Client.UI.ViewModels.PersonDetailsInfoViewModels
{
    using Bska.Client.Domain.Entity;
    public sealed class PersonDetailsInfoViewModel : BaseViewModel
    {
        #region ctor

        public PersonDetailsInfoViewModel(int personId)
        {
            this.PersonId = personId;
        }
        #endregion

        #region properties

        public int PersonId
        {
            get;
            private set;
        }

        #endregion

        #region fields

        #endregion
    }
}
