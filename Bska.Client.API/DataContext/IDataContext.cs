
namespace Bska.Client.API.DataContext
{
    using Bska.Client.API.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    public interface IDataContext : IDisposable
    {
        int SaveChanges();
        void SyncObjectState<TEntity>(TEntity entity) where TEntity : class, IObjectState;
        void SyncObjectsStatePostCommit();
        void RefreshAll();
        void ReloadEntity<TEntity>(TEntity entity) where TEntity : class;
        void ReloadNavigationProperty<TEntity, TElement>(TEntity entity, Expression<Func<TEntity, ICollection<TElement>>> navigationProperty)
            where TEntity : class
            where TElement : class;

        void TrackConcurrency<TEntity>(TEntity entity, byte[] rowVersion)
           where TEntity : class;
    }
}
