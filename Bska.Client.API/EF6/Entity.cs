
namespace Bska.Client.API.EF6
{
    using Bska.Client.API.Infrastructure;
    using System.ComponentModel.DataAnnotations.Schema;
    using System;

    public abstract class Entity : IObjectState
    {
        [NotMapped]
        public ObjectState ObjectState { get; set; }
    }
}
