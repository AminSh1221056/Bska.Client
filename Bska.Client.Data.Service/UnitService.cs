
namespace Bska.Client.Data.Service
{
    using Bska.Client.API.Repositories;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Service.Pattern;
    public interface IUnitService : IService<Unit>
    {

    }

    public class UnitService : Service<Unit>,IUnitService
    { 
        private readonly IRepositoryAsync<Unit> _repository;

        public UnitService(IRepositoryAsync<Unit> repository)
            :base(repository)
        {
            this._repository = repository;
        }
    }
}
