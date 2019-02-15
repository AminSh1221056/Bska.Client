
namespace Bska.Client.Data.Service
{
    using Bska.Client.Domain.Entity;
    using Bska.Client.Service.Pattern;
    using Bska.Client.Repository;
    using Bska.Client.API.Repositories;
    using System.Collections.Generic;
    using System;
    using Bska.Client.Common;
    public interface IStoreService : IService<Store>
    {
        IEnumerable<StoreDesign> GetParentNode(int storeId);
        Boolean HavingStoreRole(Int32 storeId, PermissionsType type);
        Users GetUserForStore(Int32 storeId);
    }

    public class StoreService : Service<Store>,IStoreService
    {
         private readonly IRepositoryAsync<Store> _repository;

         public StoreService(IRepositoryAsync<Store> repository)
            :base(repository)
        {
            this._repository = repository;
        }

         public IEnumerable<StoreDesign> GetParentNode(int storeId)
         {
             return _repository.GetParentNode(storeId);
         }

        public Users GetUserForStore(int storeId)
        {
            return _repository.GetUserForStore(storeId);
        }

        public bool HavingStoreRole(int storeId, PermissionsType type)
         {
             return _repository.HavingStoreRole(storeId, type);
         }
    }
}
