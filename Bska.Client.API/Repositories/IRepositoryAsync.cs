﻿
namespace Bska.Client.API.Repositories
{
    using Bska.Client.API.Infrastructure;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRepositoryAsync<TEntity> : IRepository<TEntity> where TEntity : class ,IObjectState
    {
        Task<TEntity> FindAsync(params object[] keyValues);
        Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues);
        Task<bool> DeleteAsync(params object[] keyValues);
        Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues);
    }
}
