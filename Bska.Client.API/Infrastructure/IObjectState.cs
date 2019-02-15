
namespace Bska.Client.API.Infrastructure
{
    using System.ComponentModel.DataAnnotations.Schema;

    public interface IObjectState
    {
        [NotMapped]
        ObjectState ObjectState { get; set; }
    }

    public enum ObjectState
    {
        Unchanged,
        Added,
        Modified,
        Deleted
    }
}
