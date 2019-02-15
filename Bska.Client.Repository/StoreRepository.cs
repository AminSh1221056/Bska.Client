
namespace Bska.Client.Repository
{
    using Bska.Client.API.Repositories;
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public static class StoreRepository
    {
        public static IEnumerable<StoreDesign> GetParentNode(this IRepository<Store> repository, int storeId)
        {
            var stores = repository.GetRepository<StoreDesign>().Queryable();
            var items = from c in stores
                        where c.StoreId == storeId
                        select c;
            return items.AsEnumerable();
        }

        public static Boolean HavingStoreRole(this IRepository<Store> repository, Int32 storeId, PermissionsType type)
        {
            var usersRole = repository.GetRepository<Roles>().Queryable();
            return usersRole.Any(x => x.RoleType == type && x.StoreId == storeId && x.OrganizId == null);
        }

        public static Users GetUserForStore(this IRepository<Store> repository, Int32 storeId)
        {
            var usersRole = repository.GetRepository<Roles>().Queryable();
            return (from r in usersRole
                        where r.RoleType == PermissionsType.StoreLeader
                        && r.StoreId == storeId
                        select r.User).SingleOrDefault();
        }
    }
}
