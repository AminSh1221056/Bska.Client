
namespace Bska.Client.API.EF6
{
    using Bska.Client.API.DataContext;
    using Bska.Client.API.Infrastructure;
    using System;
    using System.Data.Entity;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;

    public class DataContext : DbContext, IDataContextAsync
    {
        #region ctor
        public DataContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            _instanceId = Guid.NewGuid();
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        #endregion ctor

        #region properties
        public Guid InstanceId { get { return _instanceId; } }

        #endregion

        #region methods
        /// <summary>
        ///     Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateException">
        ///     An error occurred sending updates to the database.</exception>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateConcurrencyException">
        ///     A database command did not affect the expected number of rows. This usually
        ///     indicates an optimistic concurrency violation; that is, a row has been changed
        ///     in the database since it was queried.</exception>
        /// <exception cref="System.Data.Entity.Validation.DbEntityValidationException">
        ///     The save was aborted because validation of entity property values failed.</exception>
        /// <exception cref="System.NotSupportedException">
        ///     An attempt was made to use unsupported behavior such as executing multiple
        ///     asynchronous commands concurrently on the same context instance.</exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     The context or connection have been disposed.</exception>
        /// <exception cref="System.InvalidOperationException">
        ///     Some error occurred attempting to process entities in the context either
        ///     before or after sending commands to the database.</exception>
        /// <seealso cref="DbContext.SaveChanges"/>
        /// <returns>The number of objects written to the underlying database.</returns>
        public override int SaveChanges()
        {
            SyncObjectsStatePreCommit();
            var changes = base.SaveChanges();
            SyncObjectsStatePostCommit();
            return changes;
        }

        /// <summary>
        ///     Asynchronously saves all changes made in this context to the underlying database.
        /// </summary>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateException">
        ///     An error occurred sending updates to the database.</exception>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateConcurrencyException">
        ///     A database command did not affect the expected number of rows. This usually
        ///     indicates an optimistic concurrency violation; that is, a row has been changed
        ///     in the database since it was queried.</exception>
        /// <exception cref="System.Data.Entity.Validation.DbEntityValidationException">
        ///     The save was aborted because validation of entity property values failed.</exception>
        /// <exception cref="System.NotSupportedException">
        ///     An attempt was made to use unsupported behavior such as executing multiple
        ///     asynchronous commands concurrently on the same context instance.</exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     The context or connection have been disposed.</exception>
        /// <exception cref="System.InvalidOperationException">
        ///     Some error occurred attempting to process entities in the context either
        ///     before or after sending commands to the database.</exception>
        /// <seealso cref="DbContext.SaveChangesAsync"/>
        /// <returns>A task that represents the asynchronous save operation.  The 
        ///     <see cref="Task.Result">Task.Result</see> contains the number of 
        ///     objects written to the underlying database.</returns>
        public override async Task<int> SaveChangesAsync()
        {
            return await this.SaveChangesAsync(CancellationToken.None);
        }

        /// <summary>
        ///     Asynchronously saves all changes made in this context to the underlying database.
        /// </summary>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateException">
        ///     An error occurred sending updates to the database.</exception>
        /// <exception cref="System.Data.Entity.Infrastructure.DbUpdateConcurrencyException">
        ///     A database command did not affect the expected number of rows. This usually
        ///     indicates an optimistic concurrency violation; that is, a row has been changed
        ///     in the database since it was queried.</exception>
        /// <exception cref="System.Data.Entity.Validation.DbEntityValidationException">
        ///     The save was aborted because validation of entity property values failed.</exception>
        /// <exception cref="System.NotSupportedException">
        ///     An attempt was made to use unsupported behavior such as executing multiple
        ///     asynchronous commands concurrently on the same context instance.</exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     The context or connection have been disposed.</exception>
        /// <exception cref="System.InvalidOperationException">
        ///     Some error occurred attempting to process entities in the context either
        ///     before or after sending commands to the database.</exception>
        /// <seealso cref="DbContext.SaveChangesAsync"/>
        /// <returns>A task that represents the asynchronous save operation.  The 
        ///     <see cref="Task.Result">Task.Result</see> contains the number of 
        ///     objects written to the underlying database.</returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            var changesAsync = await base.SaveChangesAsync(cancellationToken);
            return changesAsync;
        }

        public void SyncObjectState<TEntity>(TEntity entity) where TEntity : class, IObjectState
        {
            Entry(entity).State = StateHelper.ConvertState(entity.ObjectState);
        }

        private void SyncObjectsStatePreCommit()
        {
            foreach (var dbEntityEntry in ChangeTracker.Entries())
            {
                dbEntityEntry.State = StateHelper.ConvertState(((IObjectState)dbEntityEntry.Entity).ObjectState);
            }
        }

        public void SyncObjectsStatePostCommit()
        {
            foreach (var dbEntityEntry in ChangeTracker.Entries())
            {
                ((IObjectState)dbEntityEntry.Entity).ObjectState = StateHelper.ConvertState(dbEntityEntry.State);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // free other managed objects that implement
                    // IDisposable only
                }

                // release any unmanaged objects
                // set object references to null

                _disposed = true;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Fields

        private readonly Guid _instanceId;
        bool _disposed;

        #endregion Fields

        public void TrackConcurrency<TEntity>(TEntity entity, byte[] rowVersion)
            where TEntity : class
        {
            this.Entry(entity).OriginalValues["RowVersion"] = rowVersion;
        }

        public void ReloadEntity<TEntity>(TEntity entity) where TEntity : class
        {
            Entry(entity).Reload();
        }

        public void ReloadNavigationProperty<TEntity, TElement>(TEntity entity, Expression<Func<TEntity, ICollection<TElement>>> navigationProperty)
            where TEntity : class
            where TElement : class
        {
            Entry(entity).Collection<TElement>(navigationProperty).Query();
        }

        public void RefreshAll()
        {
            var context = ((IObjectContextAdapter)this).ObjectContext;
            var refreshableObjects = (from entry in context.ObjectStateManager.GetObjectStateEntries(EntityState.Deleted
                                           | EntityState.Modified
                                           | EntityState.Unchanged)
                                      where entry.EntityKey != null
                                      select entry.Entity).ToList();

            context.Refresh(RefreshMode.StoreWins, refreshableObjects);
        }
    }
}
