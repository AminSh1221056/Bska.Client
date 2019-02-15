
namespace Bska.Client.Repository
{
    using Bska.Client.API.Repositories;
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Entity;
    using Bska.Client.Repository.Model;

    public static class PersonRepository
    {
        public static IEnumerable<RequestPermit> GetPersonPermit(this IRepository<Person> repository, int personId)
        {
            var requestPermit = repository.GetRepository<RequestPermit>().Queryable();

            var items = from c in requestPermit
                        where c.PersonId == personId
                        select c;

            return items.AsEnumerable();
        }

        public static IEnumerable<RequestPermit> GetAllPermits(this IRepository<Person> repository)
        {
          return  repository.GetRepository<RequestPermit>().Queryable().Where(x=>x.IsEnable).AsEnumerable();
        }

        public static IEnumerable<Users> GetUsers(this IRepository<Person> repository,Boolean toManager)
        {
            if (toManager)
            {
                return repository.GetRepository<Users>().Query().Include(x => x.UserAttribute)
                   .Include(x => x.Roles).Select();
            }
            else
            {
                return repository.GetRepository<Users>().Query().Include(x => x.UserAttribute)
                    .Include(x => x.Roles).Select().Where(x => x.UserName != "Manager");
            }
        }

        public static IEnumerable<EventLog> GetEvents(this IRepository<Person> repository,int userId)
        {
            var eventLog = repository.GetRepository<EventLog>().Queryable();
            return (from ev in eventLog
                   where ev.UserId == userId
                   select ev).AsEnumerable();
        }

        public static Users GetUser(this IRepository<Person> repository, int userId)
        {
            return repository.GetRepository<Users>().Queryable().Where(x => x.UserId == userId).Include(x=>x.UserAttribute).SingleOrDefault();
        }

        public static IEnumerable<Users> GetUsersByPermission(this IRepository<Person> repository, PermissionsType permission)
        {
            return repository.GetRepository<Users>().Queryable().Where(x => x.PermissionType == permission).AsEnumerable();
        }

        public static Users GetUniqueUserToPersonByPermission(this IRepository<Person> repository, PermissionsType permission)
        {
            var user = repository.GetRepository<Users>().Queryable();
            return (from u in user
                    where u.PermissionType == permission
                    select u).Include(u => u.Person).FirstOrDefault();
        }

        public static Users GetUserToRoles(this IRepository<Person> repository, int userId)
        {
            return repository.GetRepository<Users>().Query(x => x.UserId == userId)
                .Include(x => x.Roles).Select().SingleOrDefault();
        }
        
        public static Roles GetBuildingUserRoles(this IRepository<Person> repository,Int32 organizId)
        {
            var usersRole = repository.GetRepository<Roles>().Queryable();
            var items = (from c in usersRole
                         where  c.OrganizId == organizId
                         select c).SingleOrDefault();
            return items;
        }

        public static IEnumerable<PersonModel> GetAllUserToOrganizRoles(this IRepository<Person> repository)
        {
            var usersRole = repository.GetRepository<Roles>().Queryable();
            return (from c in usersRole
                         where c.OrganizId >0
                         select c.User).Distinct().Select(u=>new PersonModel
                         {
                             FullName=u.FullName,
                             NationalId=u.Person.NationalId,
                             PersonId=u.UserId
                         }).AsEnumerable();
        }
        public static IEnumerable<Roles> GetRolesByUser(this IRepository<Person> repository, Int32 UserId)
        {
            var usersRole = repository.GetRepository<Roles>().Queryable();
            return (from r in usersRole
                    where r.UserId == UserId
                    select r).AsEnumerable();
        }

        public static Users LoginToBeska(this IRepository<Person> repository, string username, string password)
        {
            return repository.GetRepository<Users>().Query(x => x.UserName == username && x.Password == password)
                .Include(x => x.UserAttribute).Include(u=>u.Person).Include(u=>u.EventLogs).Select().SingleOrDefault();
        }

        public static Roles GetSpecificRole(this IRepository<Person> repository, PermissionsType permission)
        {
            var usersRole = repository.GetRepository<Roles>().Queryable();

            return (from r in usersRole
                    where (r.OrganizId == null)
                    && r.RoleType == permission
                    select r).FirstOrDefault();
        }
    }
}
