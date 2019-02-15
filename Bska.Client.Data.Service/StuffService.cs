
namespace Bska.Client.Data.Service
{
    using Bska.Client.API.Repositories;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Service.Pattern;
    public interface IStuffService : IService<Stuff>
    {

    }

    public class StuffService : Service<Stuff>,IStuffService
    {
        private readonly IRepositoryAsync<Stuff> _repository;

        public StuffService(IRepositoryAsync<Stuff> repository)
            : base(repository)
        {
            this._repository = repository;
        }
    }
}
