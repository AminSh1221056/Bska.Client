
namespace Bska.Client.API.EF6
{
    using Bska.Client.API.DataContext;
    using Bska.Client.API.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Threading;
    using System.Threading.Tasks;
    public interface IFakeDbContext : IDataContextAsync
    {
        DbSet<T> Set<T>() where T : class;

        void AddFakeDbSet<TEntity, TFakeDbSet>()
            where TEntity : Entity, new()
            where TFakeDbSet : FakeDbSet<TEntity>, IDbSet<TEntity>, new();
    }

    public abstract class FakeDbContext : IFakeDbContext
    {
        #region Private Fields
        private readonly Dictionary<Type, object> _fakeDbSets;
        #endregion Private Fields

        protected FakeDbContext()
        {
            _fakeDbSets = new Dictionary<Type, object>();
        }

        public int SaveChanges() { return default(int); }

        public void SyncObjectState<TEntity>(TEntity entity) where TEntity : class, IObjectState
        {
            // no implentation needed, unit tests which uses FakeDbContext since there is no actual database for unit tests, 
            // there is no actual DbContext to sync with, please look at the Integration Tests for test that will run against an actual database.
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken) { return new Task<int>(() => default(int)); }

        public Task<int> SaveChangesAsync() { return new Task<int>(() => default(int)); }

        public void Dispose() { }

        public DbSet<T> Set<T>() where T : class { return (DbSet<T>)_fakeDbSets[typeof(T)]; }

        public void AddFakeDbSet<TEntity, TFakeDbSet>()
            where TEntity : Entity, new()
            where TFakeDbSet : FakeDbSet<TEntity>, IDbSet<TEntity>, new()
        {
            var fakeDbSet = Activator.CreateInstance<TFakeDbSet>();
            _fakeDbSets.Add(typeof(TEntity), fakeDbSet);
        }

        public void SyncObjectsStatePostCommit() { }


        public void ReloadEntity<TEntity>(TEntity entity)
             where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void ReloadNavigationProperty<TEntity, TElement>(TEntity entity, System.Linq.Expressions.Expression<Func<TEntity, ICollection<TElement>>> navigationProperty)
            where TEntity : class
            where TElement : class
        {
            throw new NotImplementedException();
        }


        public void RefreshAll()
        {
            throw new NotImplementedException();
        }


        public void TrackConcurrency<TEntity>(TEntity entity, byte[] rowVersion) where TEntity : class
        {
            throw new NotImplementedException();
        }
    }
}
