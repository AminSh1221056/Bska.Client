
namespace Bska.Client.API.UnitOfWork
{
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.Repositories;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq.Expressions;
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
        void Dispose(bool disposing);
        IRepository<TEntity> Repository<TEntity>() where TEntity : class, IObjectState;
        void ReloadEntity<TEntity>(TEntity entity) where TEntity : class;
        void ReloadNavigationProperty<TEntity, TElement>(TEntity entity, Expression<Func<TEntity, ICollection<TElement>>> navigationProperty)
            where TEntity : class
            where TElement : class;
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        bool Commit();
        void Rollback();
        void RefreshAll();
        void TrackConcurrency<TEntity>(TEntity entity, byte[] rowVersion)
           where TEntity : class;
    }
}
