
namespace Bska.Client.Data.Service
{
    using API.Repositories;
    using Bska.Client.Service.Pattern;
    using Domain.Entity;

    public interface ISellerService: IService<Seller>
    {

    }

    public class SellerService: Service<Seller>, ISellerService
    {
        private readonly IRepositoryAsync<Seller> _repository;

        public SellerService(IRepositoryAsync<Seller> repository)
            :base(repository)
        {
            this._repository = repository;
        }
    }
}
