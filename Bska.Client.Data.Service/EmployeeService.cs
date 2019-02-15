
namespace Bska.Client.Data.Service
{
    using Common;
    using Bska.Client.Repository;
    using Bska.Client.API.Repositories;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Service.Pattern;
    using System.Collections.Generic;
    using System;
    using Repository.Model;
    using Domain.Entity.AssetEntity;
    using Bska.Client.Domain.Entity.OrderEntity;

    public interface IEmployeeService : IService<Employee>
    {
        IEnumerable<EmployeeDesign> GetParentNode(int groupType);
        IEnumerable<EmployeeDesign> GetChilderen(int groupType, int parentid);
        EmployeeDesign GetDesignById(int designId, int groupType);
        IEnumerable<StrategyDesign> GetParnetNodeToBuilding(int groupType);
        IEnumerable<EmployeeDesign> GetOrganizByRole();
        IEnumerable<AccountDocumentCoding> GetAccountCodings();

        IEnumerable<AccountDocumentCoding> GetAccountCodingsByParent(int parentId);
        IEnumerable<Seller> GetSellers();
        IEnumerable<AccountDocumentDetails> GetAccountDetails(string code,DateTime fromDate,DateTime toDate);
        IEnumerable<AccountDocumentDetails> GetAccountDetailsByMaster(Int64 masterId);
        IEnumerable<AccountDocumentMaster> GetAccountMaster();
        IEnumerable<ReturnIndentRequest> GetReturnRequestByStatus(GlobalRequestStatus status,bool toSpIndents);
        int CountReturnRequestByStatus(GlobalRequestStatus status);
    }

    public class EmployeeService : Service<Employee>,IEmployeeService
    {
        private readonly IRepositoryAsync<Employee> _repository;

        public EmployeeService(IRepositoryAsync<Employee> repository)
            : base(repository)
        {
            this._repository = repository;
        }

        public int CountReturnRequestByStatus(GlobalRequestStatus status)
        {
            return _repository.CountReturnRequestByStatus(status);
        }

        public IEnumerable<AccountDocumentCoding> GetAccountCodings()
        {
            return _repository.GetAccountCodings();
        }

        public IEnumerable<AccountDocumentCoding> GetAccountCodingsByParent(int parentId)
        {
            return _repository.GetAccountCodingsByParent(parentId);
        }

        public IEnumerable<AccountDocumentDetails> GetAccountDetails(string code, DateTime fromDate, DateTime toDate)
        {
            return _repository.GetAccountDetails(code,fromDate,toDate);
        }

        public IEnumerable<AccountDocumentDetails> GetAccountDetailsByMaster(long masterId)
        {
            return _repository.GetAccountDetailsByMaster(masterId);
        }

        public IEnumerable<AccountDocumentMaster> GetAccountMaster()
        {
            return _repository.GetAccountMaster();
        }

        public IEnumerable<EmployeeDesign> GetChilderen(int groupType, int parentid)
        {
            return _repository.GetChilderen(groupType, parentid);
        }

        public EmployeeDesign GetDesignById(int designId, int groupType)
        {
            return _repository.GetDesignById(designId, groupType);
        }

        public IEnumerable<EmployeeDesign> GetOrganizByRole()
        {
            return _repository.GetOrganizByRole();
        }

        public IEnumerable<EmployeeDesign> GetParentNode(int groupType)
        {
            return _repository.GetParentNode(groupType);
        }

        public IEnumerable<StrategyDesign> GetParnetNodeToBuilding(int groupType)
        {
            return _repository.GetParnetNodeToBuilding(groupType);
        }

        public IEnumerable<ReturnIndentRequest> GetReturnRequestByStatus(GlobalRequestStatus status, bool toSpIndents)
        {
            return _repository.GetReturnRequestByStatus(status,toSpIndents);
        }

        public IEnumerable<Seller> GetSellers()
        {
            return _repository.GetSellers();
        }
    }
}
