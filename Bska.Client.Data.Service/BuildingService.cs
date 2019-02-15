
namespace Bska.Client.Data.Service
{
    using Bska.Client.Domain.Entity;
    using Bska.Client.Service.Pattern;
    using Bska.Client.Repository;
    using Bska.Client.API.Repositories;
    using System.Collections.Generic;
    using System;
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity.AssetEntity.Meters;
    public interface IBuildingService : IService<Building>
    {
        IEnumerable<Meter> GetMeters(Int32 buildingId = 0);
    }

    public class BuildingService : Service<Building>,IBuildingService
    {
        private readonly IRepositoryAsync<Building> _repository;

        public BuildingService(IRepositoryAsync<Building> repository)
            :base(repository)
        {
            this._repository = repository;
        }
        
        public IEnumerable<Meter> GetMeters(int buildingId = 0)
        {
            return _repository.GetMeters(buildingId);
        }
    }
}
