
namespace Bska.Client.Repository
{
    using Bska.Client.API.Repositories;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Domain.Entity.AssetEntity.Meters;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public static class BuildingRepository
    {
        public static IEnumerable<Meter> GetMeters(this IRepository<Building> repository,int buildingId)
        {
            var buildings = repository.GetRepository<Building>().Queryable();
            if (buildingId == 0)
            {
                return buildings.SelectMany(x => x.Meters).AsEnumerable();
            }
            else
            {
                return buildings.Where(x => x.BuildingId == buildingId).SelectMany(x => x.Meters).AsEnumerable();
            }
        }
    }
}
