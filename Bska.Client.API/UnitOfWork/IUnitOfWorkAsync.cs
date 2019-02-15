
namespace Bska.Client.API.UnitOfWork
{
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.Repositories;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IUnitOfWorkAsync : IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        IRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : class, IObjectState;
    }
}
