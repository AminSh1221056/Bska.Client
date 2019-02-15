
namespace Bska.Client.Data.Service
{
    using Bska.Client.API.Repositories;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Service.Pattern;
    using Bska.Client.Repository;
    using System.Collections.Generic;
    using System;
    using Bska.Client.Common;
    using Bska.Client.Repository.Model;

    public interface IPersonService : IService<Person>
    {
        IEnumerable<RequestPermit> GetPersonPermit(int personId);
        IEnumerable<RequestPermit> GetAllPermits();
        IEnumerable<Users> GetUsers(Boolean toManager=false);
        IEnumerable<EventLog> GetEvents(int userId);
        IEnumerable<Users> GetUsersByPermission(PermissionsType permission);
        Users GetUser(int userId);
        Users GetUserToRole(int userId);
        Roles GetBuildingUserRoles(Int32 organizId);
        IEnumerable<PersonModel> GetAllUserToOrganizRoles();
        IEnumerable<Roles> GetRolesByUser(Int32 UserId);
        Users LoginToBeska(string username, string password);
        Roles GetSpecificRole(PermissionsType permission);
        Users GetUniqueUserToPersonByPermission(PermissionsType permission);
    }

    public class PersonService : Service<Person>, IPersonService
    {
        private readonly IRepositoryAsync<Person> _repository;

        public PersonService(IRepositoryAsync<Person> repository)
            : base(repository)
        {
            this._repository = repository;
        }
        public IEnumerable<RequestPermit> GetPersonPermit(int personId)
        {
            return _repository.GetPersonPermit(personId);
        }
        public IEnumerable<Users> GetUsers(Boolean toManager = false)
        {
            return _repository.GetUsers(toManager);
        }
        public Roles GetBuildingUserRoles(int organizId)
        {
            return _repository.GetBuildingUserRoles(organizId);
        }
        public IEnumerable<Roles> GetRolesByUser(int UserId)
        {
            return _repository.GetRolesByUser(UserId);
        }

        public Users LoginToBeska(string username, string password)
        {
            return _repository.LoginToBeska(username, password);
        }

        public Roles GetSpecificRole(PermissionsType permission)
        {
            return _repository.GetSpecificRole(permission);
        }

        public Users GetUser(int userId)
        {
            return _repository.GetUser(userId);
        }

        public Users GetUserToRole(int userId)
        {
            return _repository.GetUserToRoles(userId);
        }
        
        public IEnumerable<RequestPermit> GetAllPermits()
        {
            return _repository.GetAllPermits();
        }

        public IEnumerable<EventLog> GetEvents(int userId)
        {
            return _repository.GetEvents(userId);
        }

        public IEnumerable<Users> GetUsersByPermission(PermissionsType permission)
        {
            return _repository.GetUsersByPermission(permission);
        }

        public Users GetUniqueUserToPersonByPermission(PermissionsType permission)
        {
            return _repository.GetUniqueUserToPersonByPermission(permission);
        }

        public IEnumerable<PersonModel> GetAllUserToOrganizRoles()
        {
            return _repository.GetAllUserToOrganizRoles();
        }
    }
}
