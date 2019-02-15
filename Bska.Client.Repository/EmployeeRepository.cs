
namespace Bska.Client.Repository
{
    using API.Repositories;
    using Domain.Entity;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Entity;
    using Common;
    using Model;
    using Domain.Entity.AssetEntity;
    using System;
    using Bska.Client.Domain.Entity.OrderEntity;

    public static class EmployeeRepository
    {
        public static IEnumerable<EmployeeDesign> GetParentNode(this IRepository<Employee> repository, int groupType)
        {
            var design = repository.GetRepository<EmployeeDesign>().Queryable();
            if (groupType == 1)
            {
                var items = from c in design.OfType<OrganizationDesign>()
                            select c;
                return items.AsEnumerable();
            }
            else
            {
                var items = from c in design.OfType<StrategyDesign>()
                            select c;
                return items.AsEnumerable();
            }
        }

        public static IEnumerable<EmployeeDesign> GetChilderen(this IRepository<Employee> repository,int groupType, int parentid)
        {
            var design = repository.GetRepository<EmployeeDesign>().Queryable();
            if (groupType == 1)
            {
                var items = from c in design.OfType<OrganizationDesign>()
                            where c.ParentNode.BuidldingDesignId==parentid
                            select c;
                return items.AsEnumerable();
            }
            else
            {
                var items = from c in design.OfType<StrategyDesign>()
                            where c.ParentNode.BuidldingDesignId == parentid
                            select c;
                return items.AsEnumerable();
            }
        }

        public static EmployeeDesign GetDesignById(this IRepository<Employee> repository, int designId, int groupType)
        {
            var design = repository.GetRepository<EmployeeDesign>().Queryable();
            if (groupType == 1)
            {
                return (from c in design.OfType<OrganizationDesign>()
                        where c.BuidldingDesignId == designId
                        select c).Include(x => x.ParentNode).SingleOrDefault();
            }
            else
            {
                return (from c in design.OfType<StrategyDesign>()
                        where c.BuidldingDesignId == designId
                        select c).Include(x => x.ParentNode).SingleOrDefault();
            }
        }

        public static IEnumerable<AccountDocumentCoding> GetAccountCodings(this IRepository<Employee> repository)
        {
            var coding = repository.GetRepository<AccountDocumentCoding>().Queryable();
            return (from act in coding
                   select act).Include(x=>x.Parent).AsEnumerable();
        }

        public static IEnumerable<AccountDocumentCoding> GetAccountCodingsByParent(this IRepository<Employee> repository,int parentId)
        {
            var coding = repository.GetRepository<AccountDocumentCoding>().Queryable();
            return (from act in coding
                    where act.Parent.ID==parentId
                    select act).Include(x => x.Parent).AsEnumerable();
        }

        public static IEnumerable<EmployeeDesign> GetOrganizByRole(this IRepository<Employee> repository)
        {
            var buildings = repository.GetRepository<EmployeeDesign>().Queryable();
            var userRole = repository.GetRepository<Roles>().Queryable();
            var items = from c in buildings.OfType<OrganizationDesign>()
                        select c;
            items.ToList().ForEach(x =>
            {
                if (userRole.Any(c => c.OrganizId == x.BuidldingDesignId)) x.HaveRole = true;
            });

            return items.AsEnumerable();
        }

        public static IEnumerable<StrategyDesign> GetParnetNodeToBuilding(this IRepository<Employee> repository,int groupType)
        {
            if (groupType == 2)
                return repository.GetRepository<EmployeeDesign>().Queryable().OfType<StrategyDesign>().Include(em => em.Building).AsEnumerable();
            else return null;
        }

        public static IEnumerable<Seller> GetSellers(this IRepository<Employee> repository)
        {
            var sellers = repository.GetRepository<Seller>().Queryable();
            return (from c in sellers
                    select c).AsEnumerable();
        }

        public static IEnumerable<AccountDocumentDetails> GetAccountDetails(this IRepository<Employee> repository, string code, DateTime fromDate, DateTime toDate)
        {
            var accountDetails = repository.GetRepository<AccountDocumentDetails>().Queryable();
            return accountDetails.Where(x => x.AccountNo.StartsWith(code) && 
            (x.AccountDocumentMaster.AccountDate> fromDate && x.AccountDocumentMaster.AccountDate <=toDate))
            .Include(x => x.MAsset).Include(x => x.AccountDocumentMaster).AsEnumerable();
        }

        public static IEnumerable<AccountDocumentMaster> GetAccountMaster(this IRepository<Employee> repository)
        {
            var accountmaster = repository.GetRepository<AccountDocumentMaster>().Queryable();
            return (from ac in accountmaster
                    select ac).AsEnumerable();
        }

        public static IEnumerable<AccountDocumentDetails> GetAccountDetailsByMaster(this IRepository<Employee> repository,long masterId)
        {
            var accountDetails = repository.GetRepository<AccountDocumentDetails>().Queryable();
            return accountDetails.Where(acd => acd.MasterId == masterId).AsEnumerable();
        }

        public static IEnumerable<ReturnIndentRequest> GetReturnRequestByStatus(this IRepository<Employee> repository, GlobalRequestStatus status, bool toSpIndents)
        {
            var returnRequsts = repository.GetRepository<ReturnIndentRequest>().Queryable();
            if (toSpIndents) return returnRequsts.Where(rq => rq.Status == status).Include(rq => rq.SupplierIndents).AsEnumerable();
            return returnRequsts.Where(rg => rg.Status == status).AsEnumerable();
        }

        public static int CountReturnRequestByStatus(this IRepository<Employee> repository, GlobalRequestStatus status)
        {
            var returnRequsts = repository.GetRepository<ReturnIndentRequest>().Queryable();
            return returnRequsts.Where(rg => rg.Status == status).Count();
        }

    }
}
