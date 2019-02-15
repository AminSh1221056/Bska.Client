
namespace Bska.Client.Repository
{
    using Bska.Client.API.Repositories;
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.Domain.Entity.OrderEntity;
    using Bska.Client.Repository.Model;
    using Domain.Entity;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    public static class OrderRepository
    {
        public static IEnumerable<OrderModel> GetOrders(this IRepository<Order> repository, int personId, DateTime minDate)
        {
            var order = repository.GetRepository<Order>().Queryable();
            if (personId <= 0)
            {
                return (from o in order
                        where o.OrderDate > minDate
                        select new OrderModel
                        {
                            OrderDate = o.OrderDate,
                            OrderId = o.OrderId,
                            Status = o.Status,
                            OrderType = o.OrderType,
                            IsEditable =false,
                        }).OrderByDescending(x => x.OrderDate).AsEnumerable();
            }
            else
            {
                return (from o in order
                        where o.PersonId == personId && o.OrderDate > minDate
                        select new OrderModel
                        {
                            OrderDate = o.OrderDate,
                            OrderId = o.OrderId,
                            Status = o.Status,
                            OrderType = o.OrderType,
                            IsEditable =false,
                        }).OrderByDescending(x => x.OrderDate).AsEnumerable();
            }
        }

        public static IEnumerable<OrderModel> GetRecivedOrders(this IRepository<Order> repository, Boolean IsManager, 
            int userId,Boolean forStore, OrderStatus oStatus)
        {
            var orders = repository.GetRepository<Order>().Queryable();
            if (forStore)
            {
                var items = from o in orders
                            where (o.Status == oStatus && o.OrderType == OrderType.Store)
                            select new OrderModel
                            {
                                OrderDate = o.OrderDate,
                                OrderId = o.OrderId,
                                OrderType = o.OrderType,
                                PersonName = o.Person.FirstName + " " + o.Person.LastName,
                                PersonId = o.PersonId.Value,
                                NationalId = o.Person.NationalId,
                                Description = o.Description,
                                IsEditable = true,
                                DueDate = o.DueDate,
                                Status = o.Status
                            };
                return items.AsEnumerable();
            }
            else
            {
                if (IsManager)
                {
                    var items = from o in orders
                                where ((o.Status == OrderStatus.OrganizManagerConfirm || 
                                o.Status == OrderStatus.ManagerConfirm) && o.OrderType != OrderType.Store)
                                select new OrderModel
                                {
                                    OrderDate = o.OrderDate,
                                    OrderId = o.OrderId,
                                    OrderType = o.OrderType,
                                    PersonName = o.Person.FirstName + " " + o.Person.LastName,
                                    PersonId = o.PersonId.Value,
                                    NationalId = o.Person.NationalId,
                                    Description = o.Description,
                                    IsEditable = true,
                                    DueDate = o.DueDate,
                                    Status = o.Status
                                };
                    return items.AsEnumerable();
                }
                var pitems = (from o in orders
                              from od in o.OrderDetails
                              from ou in od.OrderUserHistories
                              where (!ou.UserDecision && ou.UserId == userId) && 
                              (o.Status == oStatus) && o.OrderType != OrderType.Store
                              select new OrderModel
                              {
                                  OrderDate = o.OrderDate,
                                  OrderId = o.OrderId,
                                  OrderType = o.OrderType,
                                  PersonName = o.Person.FirstName + " " + o.Person.LastName,
                                  PersonId = o.PersonId.Value,
                                  NationalId = o.Person.NationalId,
                                  Description = o.Description,
                                  IsEditable = ou.IsCurrent,
                                  DueDate = o.DueDate,
                                  Status = o.Status
                              }).Distinct();
                return pitems.AsEnumerable();
            }
        }

        public static IEnumerable<OrderModel> GetRecivedOrganizManageOrders(this IRepository<Order> repository,int userId)
        {
            var orders = repository.GetRepository<Order>().Queryable();
            List<OrderModel> ordersM = new List<OrderModel>();

            (from o in orders
                          from od in o.OrderDetails
                          let ou= od.OrderUserHistories.Any(u =>u.IsCurrent && u.UserId == userId)
                          where od.OrderUserHistories.Any(u=> !u.UserDecision && u.UserId == userId) &&
                          o.OrderType != OrderType.Store
                          select new OrderModel
                          {
                              OrderDate = o.OrderDate,
                              OrderId = o.OrderId,
                              OrderType = o.OrderType,
                              PersonName = o.Person.FirstName + " " + o.Person.LastName,
                              PersonId = o.PersonId.Value,
                              NationalId = o.Person.NationalId,
                              Description = o.Description,
                              IsEditable = ou,
                              DueDate = o.DueDate,
                              Status = o.Status
                          }).AsEnumerable().ToList().ForEach(om=>
                          {
                              var item= ordersM.SingleOrDefault(oc => oc.OrderId == om.OrderId);
                              if (item!=null)
                              {
                                  if (!om.IsEditable)
                                  {
                                      ordersM.Remove(item);
                                      ordersM.Add(om);
                                  }
                              }
                              else
                              {
                                  ordersM.Add(om);
                              }
                          });
            return ordersM;
        }

        public static IEnumerable<OrderModel> GetHonestRecivedOrders(this IRepository<Order> repository, int userId,bool isManager)
        {
            var orders = repository.GetRepository<Order>().Queryable();
            List<OrderModel> ordersM = new List<OrderModel>();

            if (isManager)
            {
                var items = from o in orders
                            where ((o.Status == OrderStatus.StuffHonest) && o.OrderType != OrderType.Procceding)
                            select new OrderModel
                            {
                                OrderDate = o.OrderDate,
                                OrderId = o.OrderId,
                                OrderType = o.OrderType,
                                PersonName = o.Person.FirstName + " " + o.Person.LastName,
                                PersonId = o.PersonId.Value,
                                NationalId = o.Person.NationalId,
                                Description = o.Description,
                                IsEditable = true,
                                DueDate = o.DueDate,
                                Status = o.Status
                            };
                return items.AsEnumerable();
            }

            (from o in orders
             from od in o.OrderDetails
             let ou = od.OrderUserHistories.Any(u => u.IsCurrent && u.UserId == userId)
             where od.OrderUserHistories.Any(u => !u.UserDecision && u.UserId == userId)
             && o.OrderType != OrderType.Procceding
             select new OrderModel
             {
                 OrderDate = o.OrderDate,
                 OrderId = o.OrderId,
                 OrderType = o.OrderType,
                 PersonName = o.Person.FirstName + " " + o.Person.LastName,
                 PersonId = o.PersonId.Value,
                 NationalId = o.Person.NationalId,
                 Description = o.Description,
                 IsEditable = ou,
                 DueDate = o.DueDate,
                 Status = o.Status
             }).AsEnumerable().ToList().ForEach(om =>
             {
                 var item = ordersM.SingleOrDefault(oc => oc.OrderId == om.OrderId);
                 if (item != null)
                 {
                     if (!om.IsEditable)
                     {
                         ordersM.Remove(item);
                         ordersM.Add(om);
                     }
                 }
                 else
                 {
                     ordersM.Add(om);
                 }
             });
            return ordersM;
        }

        public static IEnumerable<OrderUserHistory> GetUserHistories(this IRepository<Order> repository, Int64 orderDetailsId)
        {
            var UserHistories = repository.GetRepository<OrderUserHistory>().Queryable();
            return (from ou in UserHistories
                   where ou.OrderDetailsId==orderDetailsId
                        select ou).AsEnumerable();
        }

        public static IEnumerable<OrderDetails> GetRecivedOrderDetails(this IRepository<Order> repository, Int64 orderId,Int32 userId,Boolean isManager)
        {
            var orderDetails = repository.GetRepository<OrderDetails>().Queryable();
            if (isManager)
            {
                var items = (from od in orderDetails
                              from ou in od.OrderUserHistories
                              where od.OrderId == orderId && (od.State==OrderDetailsState.OrganizManagerConfirm || 
                              od.State==OrderDetailsState.ManagerConfirm) && (!ou.UserDecision)
                              select od).Include(od=>od.OrderUserHistories).DistinctBy(od=>od.OrderDetialsId);
                return items.AsEnumerable();
            }
            var items1 = (from od in orderDetails
                         from ou in od.OrderUserHistories
                         where od.OrderId == orderId && (!ou.UserDecision && ou.UserId == userId)
                         && (od.State == OrderDetailsState.OrganizManagerConfirm ||
                              od.State == OrderDetailsState.ManagerConfirm)
                          select od).Include(od => od.OrderUserHistories).DistinctBy(od => od.OrderDetialsId);
            return items1.AsEnumerable();
        }

        public static IEnumerable<OrderDetails> GetHonestRecivedOrderDetails(this IRepository<Order> repository, Int64 orderId)
        {
            var orderDetails = repository.GetRepository<OrderDetails>().Queryable();
            var items1 = (from od in orderDetails
                          where od.OrderId == orderId
                          select od).Include(od => od.OrderUserHistories).Include(od=>od.Order).AsNoTracking();
            return items1.AsEnumerable();
        }

        public static IEnumerable<SubOrder> GetSubOrders(this IRepository<Order> repository,Int64 orderDetailsId)
        {
            var subOrders = repository.GetRepository<SubOrder>().Queryable();
            return (from so in subOrders
                   where so.OrderDetailsId == orderDetailsId
                   select so).AsEnumerable();
        }

        public static IEnumerable<SubOrderModel> GetBuySubOrders(this IRepository<Order> repository)
        {
            var subOrders = repository.GetRepository<SubOrder>().Queryable();
            return (from so in subOrders
                    where (so.Type==SubOrderType.Supplier)
                    select new SubOrderModel
                    {
                        Num = so.Num,
                        Remain = so.Remain,
                        UnitId = so.UnitId,
                        OrderDate = so.OrderDetails.Order.OrderDate,
                        OrderDetailsId = so.OrderDetailsId.Value,
                        OrderId = so.OrderDetails.OrderId.Value,
                        StuffName = so.OrderDetails.StuffName,
                        KalaUid = so.OrderDetails.KalaUid,
                        KalaNo = so.OrderDetails.kalaNo,
                        StuffType = so.OrderDetails.StuffType,
                        SubOrderId = so.SubOrderId,
                        Identity = so.Identity,
                        OrderType = so.OrderDetails.Order.OrderType,
                        State = so.State
                    }).AsEnumerable();
        }

        public static IEnumerable<SubOrderModel> GetSupplierIndentWithouReturnRequest(this IRepository<Order> repository,int supplierId)
        {
            var spIndent = repository.GetRepository<SupplierIndent>()
                .Queryable().Where(so=>so.Remain > 0).Include(sp=>sp.SubOrder).Include(sp=>sp.SubOrder.OrderDetails)
                .Include(sp => sp.SubOrder.OrderDetails.Order).Include(x=>x.ReturnIndentRequsts);
            List<SubOrderModel> items = new List<SubOrderModel>() ;

            if (supplierId > 0)
            {
                spIndent = spIndent.Where(so => so.SupplierId == supplierId);
            }

            spIndent.ToList().ForEach(so =>
            {
                if (!so.ReturnIndentRequsts.Any(rrq=> rrq.Status == GlobalRequestStatus.Confirmed
                    || rrq.Status == GlobalRequestStatus.Pending))
                {
                    items.Add(new SubOrderModel
                    {
                        Num = so.Num,
                        Remain = so.Remain,
                        UnitId = so.UnitId,
                        OrderDate = so.SubOrder.OrderDetails.Order.OrderDate,
                        OrderDetailsId = so.SubOrder.OrderDetailsId.Value,
                        OrderId = so.SubOrder.OrderDetails.OrderId.Value,
                        StuffName = so.SubOrder.OrderDetails.StuffName,
                        StuffType = so.SubOrder.OrderDetails.StuffType,
                        KalaUid = so.SubOrder.OrderDetails.KalaUid,
                        KalaNo = so.SubOrder.OrderDetails.kalaNo,
                        SubOrderId = so.SubOrderId ?? 0,
                        SupplierIndentId = so.ID,
                        OrderType = so.SubOrder.OrderDetails.Order.OrderType,
                        SpState = so.State,
                        SellerId = so.SellerId,
                        SupplierId = so.SupplierId
                    });
                }
                else
                {
                    if (so.ReturnIndentRequsts.Count <= 0)
                    {
                        items.Add(new SubOrderModel
                        {
                            Num = so.Num,
                            Remain = so.Remain,
                            UnitId = so.UnitId,
                            OrderDate = so.SubOrder.OrderDetails.Order.OrderDate,
                            OrderDetailsId = so.SubOrder.OrderDetailsId.Value,
                            OrderId = so.SubOrder.OrderDetails.OrderId.Value,
                            StuffName = so.SubOrder.OrderDetails.StuffName,
                            StuffType = so.SubOrder.OrderDetails.StuffType,
                            KalaUid = so.SubOrder.OrderDetails.KalaUid,
                            KalaNo = so.SubOrder.OrderDetails.kalaNo,
                            SubOrderId = so.SubOrderId ?? 0,
                            SupplierIndentId = so.ID,
                            OrderType = so.SubOrder.OrderDetails.Order.OrderType,
                            SpState = so.State,
                            SellerId = so.SellerId,
                            SupplierId = so.SupplierId
                        });
                    }
                }
            });
            return items;
        }

        public static IEnumerable<SubOrderModel> GetSupplierIndentByRequest(this IRepository<Order> repository, int requestId)
        {
            var spIndent = repository.GetRepository<ReturnIndentRequest>();
            return spIndent.Queryable().Where(sp => sp.Id == requestId).SelectMany(x => x.SupplierIndents).Select(so => new SubOrderModel
            {
                Num = so.Num,
                Remain = so.Remain,
                UnitId = so.UnitId,
                OrderDate = so.SubOrder.OrderDetails.Order.OrderDate,
                OrderDetailsId = so.SubOrder.OrderDetailsId.Value,
                OrderId = so.SubOrder.OrderDetails.OrderId.Value,
                StuffName = so.SubOrder.OrderDetails.StuffName,
                StuffType = so.SubOrder.OrderDetails.StuffType,
                KalaUid = so.SubOrder.OrderDetails.KalaUid,
                KalaNo = so.SubOrder.OrderDetails.kalaNo,
                SubOrderId = so.SubOrderId ?? 0,
                SupplierIndentId = so.ID,
                OrderType = so.SubOrder.OrderDetails.Order.OrderType,
                SpState = so.State,
                SellerId = so.SellerId,
                SupplierId = so.SupplierId
            }).AsEnumerable();
        }

        public static IEnumerable<SubOrderModel> GetSubOrdersByIdentity(this IRepository<Order> repository, String identity, SubOrderState subState,
            SubOrderType subType,bool allIndent, DateTime fromDate, DateTime toDate)
        {
            var subOrders = repository.GetRepository<SubOrder>().Queryable();
            if (allIndent)
            {
                return (from so in subOrders
                        where (so.Identity == identity && so.Type == subType)
                        select new SubOrderModel
                        {
                            Num = so.Num,
                            Remain = so.Remain,
                            UnitId = so.UnitId,
                            OrderDate = so.OrderDetails.Order.OrderDate,
                            OrderDetailsId = so.OrderDetailsId.Value,
                            OrderId = so.OrderDetails.OrderId.Value,
                            StuffName = so.OrderDetails.StuffName,
                            KalaUid =  so.OrderDetails.KalaUid,
                            KalaNo = so.OrderDetails.kalaNo,
                            StuffType = so.OrderDetails.StuffType,
                            SubOrderId = so.SubOrderId,
                            Identity = so.Identity,
                            OrderType = so.OrderDetails.Order.OrderType,
                            State=so.State,
                            SpState = SupplierIndentState.Ongoing
                        }).Where(so=>so.OrderDate>fromDate && so.OrderDate<=toDate)
                        .OrderByDescending(x=>x.OrderDate).AsEnumerable();
            }
            return (from so in subOrders
                    where (so.Identity == identity && so.State == subState && so.Type == subType)
                    select new SubOrderModel
                    {
                        Num = so.Num,
                        Remain = so.Remain,
                        UnitId = so.UnitId,
                        OrderDate = so.OrderDetails.Order.OrderDate,
                        OrderDetailsId = so.OrderDetailsId.Value,
                        OrderId = so.OrderDetails.OrderId.Value,
                        StuffName = so.OrderDetails.StuffName,
                        KalaUid = so.OrderDetails.KalaUid,
                        KalaNo = so.OrderDetails.kalaNo,
                        StuffType = so.OrderDetails.StuffType,
                        SubOrderId = so.SubOrderId,
                        Identity = so.Identity,
                        OrderType = so.OrderDetails.Order.OrderType,
                        State = so.State,
                        SpState=SupplierIndentState.Ongoing
                    }).Where(so => so.OrderDate > fromDate && so.OrderDate <= toDate)
                    .OrderByDescending(x => x.OrderDate).AsEnumerable();
        }

        public static IEnumerable<SubOrderModel> GetTrenderSubOrders(this IRepository<Order> repository,bool isManager,string identity, 
            HashSet<SubOrderState> subState, DateTime fromDate, DateTime toDate)
        {
            var subOrders = repository.GetRepository<SubOrder>().Queryable();
            if (isManager)
            {
                return (from so in subOrders
                        where (so.Type == SubOrderType.TenderOffer && subState.Contains(so.State))
                        select new SubOrderModel
                        {
                            Num = so.Num,
                            Remain = so.Remain,
                            UnitId = so.UnitId,
                            OrderDate = so.OrderDetails.Order.OrderDate,
                            OrderDetailsId = so.OrderDetailsId.Value,
                            OrderId = so.OrderDetails.OrderId.Value,
                            StuffName = so.OrderDetails.StuffName,
                            KalaUid = so.OrderDetails.KalaUid,
                            KalaNo = so.OrderDetails.kalaNo,
                            StuffType = so.OrderDetails.StuffType,
                            SubOrderId = so.SubOrderId,
                            Identity = so.Identity,
                            OrderType = so.OrderDetails.Order.OrderType,
                            State = so.State,
                            SpState = SupplierIndentState.Ongoing,
                            IsSelected=so.SupplierTrenderOffers.Any()
                        }).Where(so => so.OrderDate > fromDate && so.OrderDate <= toDate)
                        .OrderByDescending(x => x.OrderDate).AsEnumerable();
            }

            return (from so in subOrders
                    where (so.Identity==identity && so.Type == SubOrderType.TenderOffer && subState.Contains(so.State))
                    select new SubOrderModel
                    {
                        Num = so.Num,
                        Remain = so.Remain,
                        UnitId = so.UnitId,
                        OrderDate = so.OrderDetails.Order.OrderDate,
                        OrderDetailsId = so.OrderDetailsId.Value,
                        OrderId = so.OrderDetails.OrderId.Value,
                        StuffName = so.OrderDetails.StuffName,
                        KalaUid = so.OrderDetails.KalaUid,
                        KalaNo = so.OrderDetails.kalaNo,
                        StuffType = so.OrderDetails.StuffType,
                        SubOrderId = so.SubOrderId,
                        Identity = so.Identity,
                        OrderType = so.OrderDetails.Order.OrderType,
                        State = so.State,
                        SpState = SupplierIndentState.Ongoing
                    }).Where(so => so.OrderDate > fromDate && so.OrderDate <= toDate)
                       .OrderByDescending(x => x.OrderDate).AsEnumerable();
        }

        public static IEnumerable<SubOrderModel> GetTrenderSubOrdersForSupplier(this IRepository<Order> repository, bool isManager, int supplierId, DateTime fromDate, DateTime toDate)
        {
            var trrenderOffers = repository.GetRepository<SupplierTrenderOffer>().Queryable();
            if (isManager)
            {
                return trrenderOffers.Where(so => so.InsertDate > fromDate && so.InsertDate <= toDate).Select(so => new SubOrderModel
                {
                    Num = so.SubOrder.Num,
                    Remain = so.SubOrder.Remain,
                    UnitId = so.SubOrder.UnitId,
                    OrderDate = so.SubOrder.OrderDetails.Order.OrderDate,
                    OrderDetailsId = so.SubOrder.OrderDetailsId.Value,
                    OrderId = so.SubOrder.OrderDetails.OrderId.Value,
                    StuffName = so.SubOrder.OrderDetails.StuffName,
                    KalaUid = so.SubOrder.OrderDetails.KalaUid,
                    KalaNo = so.SubOrder.OrderDetails.kalaNo,
                    StuffType = so.SubOrder.OrderDetails.StuffType,
                    SubOrderId = so.SubOrder.SubOrderId,
                    Identity = so.SubOrder.Identity,
                    OrderType = so.SubOrder.OrderDetails.Order.OrderType,
                    State = so.SubOrder.State,
                    SpState = SupplierIndentState.Ongoing,
                    IsSelected=so.ISEnableTrender,
                    SellerId=so.Id,
                }).AsEnumerable();
            }

            return trrenderOffers.Where(so =>so.SupplierId==supplierId && so.InsertDate > fromDate &&
            so.InsertDate <= toDate).Select(so => new SubOrderModel
            {
                Num = so.SubOrder.Num,
                Remain = so.SubOrder.Remain,
                UnitId = so.SubOrder.UnitId,
                OrderDate = so.SubOrder.OrderDetails.Order.OrderDate,
                OrderDetailsId = so.SubOrder.OrderDetailsId.Value,
                OrderId = so.SubOrder.OrderDetails.OrderId.Value,
                StuffName = so.SubOrder.OrderDetails.StuffName,
                KalaUid = so.SubOrder.OrderDetails.KalaUid,
                KalaNo = so.SubOrder.OrderDetails.kalaNo,
                StuffType = so.SubOrder.OrderDetails.StuffType,
                SubOrderId = so.SubOrder.SubOrderId,
                Identity = so.SubOrder.Identity,
                OrderType = so.SubOrder.OrderDetails.Order.OrderType,
                State = so.SubOrder.State,
                SpState = SupplierIndentState.Ongoing,
                IsSelected = so.ISEnableTrender,
                SellerId = so.Id,
            }).AsEnumerable();
        }

        public static IEnumerable<SubOrderModel> GetRecivedIndentToSupplier(this IRepository<Order> repository, string identity)
        {
            var subOrders = repository.GetRepository<SubOrder>().Queryable();
            return (from so in subOrders
                    where (so.Identity == identity && so.State!=SubOrderState.Deliviry && so.Type==SubOrderType.Supplier)
                    select new SubOrderModel
                    {
                        Num = so.Num,
                        Remain = so.Remain,
                        UnitId = so.UnitId,
                        OrderDate = so.OrderDetails.Order.OrderDate,
                        OrderDetailsId = so.OrderDetailsId.Value,
                        OrderId = so.OrderDetails.OrderId.Value,
                        StuffName = so.OrderDetails.StuffName,
                        KalaUid = so.OrderDetails.KalaUid,
                        KalaNo=so.OrderDetails.kalaNo,
                        StuffType = so.OrderDetails.StuffType,
                        SubOrderId = so.SubOrderId,
                        Identity = so.Identity,
                        OrderType = so.OrderDetails.Order.OrderType,
                        State = so.State
                    }).AsEnumerable();
        }

        public static IEnumerable<OrderModel> GetPersonOrdersInfo(this IRepository<Order> repository, OrderType orderType,
           DateTime fromDate, DateTime ToDate, Int32 personId, OrderStatus status)
        {
            var order = repository.GetRepository<Order>().Queryable();
            if (status == OrderStatus.None)
            {
                var orders = (from o in order
                              where (o.OrderType == orderType && (o.OrderDate > fromDate && o.OrderDate <= ToDate)
                              && o.PersonId == personId)
                              select new OrderModel
                              {
                                  OrderDate = o.OrderDate,
                                  DueDate = o.DueDate,
                                  OrderId = o.OrderId,
                                  OrderType = o.OrderType,
                                  PersonName = o.Person.FirstName + " " + o.Person.LastName,
                                  PersonId = o.PersonId.Value,
                              });
                return orders.AsEnumerable();
            }
            var orders1 = (from o in order
                           where (o.OrderType == orderType && (o.OrderDate > fromDate && o.OrderDate <= ToDate)
                           && o.PersonId == personId && o.Status == status)
                           select new OrderModel
                           {
                               OrderDate = o.OrderDate,
                               DueDate = o.DueDate,
                               OrderId = o.OrderId,
                               OrderType = o.OrderType,
                               PersonName = o.Person.FirstName + " " + o.Person.LastName,
                               PersonId = o.PersonId.Value,
                           });
            return orders1.AsEnumerable();
        }

        public static Int32 GetRecivedOrdersCount(this IRepository<Order> repository, int UserId, params OrderDetailsState[] odState)
        {
            var orderDetailsHistory = repository.GetRepository<OrderUserHistory>().Queryable();
            return orderDetailsHistory.Where(x => x.UserId == UserId && !x.UserDecision && (x.OrderDetails.IsReject==false && odState.Contains(x.OrderDetails.State))).Include(x=>x.OrderDetails).AsEnumerable()
                .DistinctBy(od=>od.OrderDetails.OrderId).Count();
        }

        public static Int32 GetRecivedOrdersForStuffHonest(this IRepository<Order> repository, int userId)
        {
            var orders = repository.GetRepository<Order>().Queryable();
            return (from o in orders
                    from od in o.OrderDetails
                    let ou = od.OrderUserHistories.Any(u => u.IsCurrent && u.UserId == userId)
                    where od.OrderUserHistories.Any(u => !u.UserDecision && u.UserId == userId)
                    && o.OrderType != OrderType.Procceding
                    select o).Count();
        }
        
        public static IEnumerable<OrderDetailsModel> GetMyOrdersForManage(this IRepository<Order> repository, bool fromManager, OrderType[] ordertypes, int personId)
        {
            var order = repository.GetRepository<Order>().Queryable();
            if (fromManager)
            {
                return (from o in order
                        from od in o.OrderDetails
                        where ordertypes.Contains(o.OrderType)
                        select new OrderDetailsModel
                        {
                            DueDate = o.DueDate,
                            NationalId = o.Person.NationalId,
                            OrderDate = o.OrderDate,
                            UnitId = od.UnitId,
                            StuffType = od.StuffType,
                            StuffName = od.StuffName,
                            kalaUid=od.KalaUid,
                            KalaNo = od.kalaNo,
                            StrategyId = od.StrategyId,
                            OrganizId = od.OrganizId,
                            OrderId = o.OrderId,
                            OrderStatus = o.Status,
                            OrderType = o.OrderType,
                            PersonName = o.Person.FirstName + " " + o.Person.LastName,
                            PersonId = o.PersonId.Value,
                            Num = od.Num,
                            OrderDetailsId = od.OrderDetialsId
                        }).AsEnumerable();
            }
            return (from o in order
                    from od in o.OrderDetails
                    where ordertypes.Contains(o.OrderType) &&  o.PersonId == personId
                    select new OrderDetailsModel
                    {
                        DueDate = o.DueDate,
                        NationalId = o.Person.NationalId,
                        OrderDate = o.OrderDate,
                        UnitId = od.UnitId,
                        StuffType = od.StuffType,
                        StuffName = od.StuffName,
                        kalaUid = od.KalaUid,
                        KalaNo = od.kalaNo,
                        StrategyId = od.StrategyId,
                        OrganizId = od.OrganizId,
                        OrderId = o.OrderId,
                        OrderStatus = o.Status,
                        OrderType = o.OrderType,
                        PersonName = o.Person.FirstName + " " + o.Person.LastName,
                        PersonId = o.PersonId.Value,
                        Num = od.Num,
                        OrderDetailsId = od.OrderDetialsId
                    }).AsEnumerable();
        }

        public static IEnumerable<OrderDetailsModel> GetManagedOrdersByMy(this IRepository<Order> repository, OrderType[] orderTypes, Int32 userId)
        {
            var order = repository.GetRepository<Order>().Queryable();
            return (from o in order
                      from od in o.OrderDetails
                      from ouh in od.OrderUserHistories
                      where ouh.UserId == userId && ouh.UserDecision == true && !ouh.Description.Contains("ثبت درخواست") && orderTypes.Contains(o.OrderType)
                     
                      select new OrderDetailsModel
                      {
                          DueDate = o.DueDate,
                          NationalId = o.Person.NationalId,
                          OrderDate = o.OrderDate,
                          UnitId = od.UnitId,
                          StuffType = od.StuffType,
                          StuffName = od.StuffName,
                          kalaUid = od.KalaUid,
                          KalaNo = od.kalaNo,
                          StrategyId = od.StrategyId,
                          OrganizId = od.OrganizId,
                          OrderId = o.OrderId,
                          OrderStatus = o.Status,
                          OrderType = o.OrderType,
                          PersonName = o.Person.FirstName + " " + o.Person.LastName,
                          PersonId = o.PersonId.Value,
                          Num = od.Num,
                          OrderDetailsId = od.OrderDetialsId
                      }).Distinct().AsEnumerable();
        }

        public static OrderDetails GetOrderDetails(this IRepository<Order> repository, long orderDetailsId)
        {
            var order = repository.GetRepository<Order>().Queryable();
            return (from o in order
                    from od in o.OrderDetails
                    where od.OrderDetialsId == orderDetailsId
                    select od).Include(od=>od.Order).FirstOrDefault();
        }

        public static IEnumerable<MovableAsset> GetOrderdAssetsForProceeding(this IRepository<Order> repository, OrderType OType, bool checkCompietion,ProceedingsType procType)
        {
            var order = repository.GetRepository<Order>().Queryable();
            if (checkCompietion)
            {
                return (order.Where(o => o.OrderType == OType && o.Status==OrderStatus.StuffHonest && o.OrderProcType==procType).SelectMany(o => o.MovableAssets).Where(ma => ma.ISCompietion==CompietionState.Reported &&
                    ma.Locations.Any(x=>x.Status==LocationStatus.MovedRequest)).Include(ma => ma.AssetProceedings)).AsNoTracking().AsEnumerable();
            }
            return (order.Where(o => o.OrderType == OType && o.Status == OrderStatus.StuffHonest && o.OrderProcType == procType).SelectMany(o => o.MovableAssets)
                .Where(ma => ma.Locations.Any(x => x.Status == LocationStatus.MovedRequest)).Include(ma => ma.AssetProceedings)).AsNoTracking().AsEnumerable();
        }

        public static int GetRecivedStoreIndentsCount(this IRepository<Order> repository, string[] storeId)
        {
            var subOrders = repository.GetRepository<SubOrder>().Queryable();
            if (storeId!=null)
            {
                return (from so in subOrders
                        where (storeId.Contains(so.Identity) && so.State==SubOrderState.None && so.Type==SubOrderType.Store)
                        select so).Count();
            }
            return (from so in subOrders
                    where (so.State == SubOrderState.None && so.Type == SubOrderType.Store)
                    select so).Count();
        }

        public static int GetRecivedMunituinIndentsCount(this IRepository<Order> repository, string identity)
        {
            var subOrders = repository.GetRepository<SubOrder>().Queryable();
            return (from so in subOrders
                    where (so.Identity == identity && so.Type == SubOrderType.Supplier && so.State == SubOrderState.None)
                    select so).Count();
        }

        public static int GetStoreOrdersForManager(this IRepository<Order> repository)
        {
            var order = repository.GetRepository<Order>().Queryable();
            return order.Count(x => x.OrderType == OrderType.Store && x.Status == OrderStatus.ManagerConfirm);
        }

        public static IEnumerable<SupplierIndent> GetSupplierIndents(this IRepository<Order> repository,long subOrderId)
        {
            var spIndent = repository.GetRepository<SupplierIndent>().Queryable();
            return (from sp in spIndent
                    where sp.SubOrderId == subOrderId
                    select sp).AsEnumerable();
        }

        public static IEnumerable<SubOrderModel> GetSupplierIndentsForSupplier(this IRepository<Order> repository, int supplierId, SupplierIndentState state, bool allIndent)
        {
            var spIndent = repository.GetRepository<SupplierIndent>().Queryable();
            if (allIndent)
            {
                return (from so in spIndent
                        where so.SupplierId == supplierId
                        select new SubOrderModel
                        {
                            Num = so.Num,
                            Remain = so.Remain,
                            UnitId = so.UnitId,
                            OrderDate = so.SubOrder.OrderDetails.Order.OrderDate,
                            OrderDetailsId = so.SubOrder.OrderDetailsId.Value,
                            OrderId = so.SubOrder.OrderDetails.OrderId.Value,
                            StuffName = so.SubOrder.OrderDetails.StuffName,
                            StuffType = so.SubOrder.OrderDetails.StuffType,
                            KalaUid=so.SubOrder.OrderDetails.KalaUid,
                            KalaNo = so.SubOrder.OrderDetails.kalaNo,
                            SubOrderId = so.SubOrderId ?? 0,
                            SupplierIndentId = so.ID,
                            OrderType = so.SubOrder.OrderDetails.Order.OrderType,
                            SpState = so.State,
                            SellerId = so.SellerId,
                            SupplierId = so.SupplierId
                        }).AsEnumerable();
            }
            return (from so in spIndent
                    where so.SupplierId == supplierId && so.State==state
                    select new SubOrderModel
                    {
                        Num = so.Num,
                        Remain = so.Remain,
                        UnitId = so.UnitId,
                        OrderDate = so.SubOrder.OrderDetails.Order.OrderDate,
                        OrderDetailsId = so.SubOrder.OrderDetailsId.Value,
                        OrderId = so.SubOrder.OrderDetails.OrderId.Value,
                        StuffName = so.SubOrder.OrderDetails.StuffName,
                        StuffType = so.SubOrder.OrderDetails.StuffType,
                        KalaUid = so.SubOrder.OrderDetails.KalaUid,
                        KalaNo = so.SubOrder.OrderDetails.kalaNo,
                        SubOrderId = so.SubOrderId??0,
                        SupplierIndentId=so.ID,
                        OrderType = so.SubOrder.OrderDetails.Order.OrderType,
                        SpState = so.State,
                        SellerId = so.SellerId,
                        SupplierId = so.SupplierId
                    }).AsEnumerable();
        }

        public static IEnumerable<SubOrderModel> GetSupplierCurrentIndent(this IRepository<Order> repository, int supplierId)
        {
            var spIndent = repository.GetRepository<SupplierIndent>().Queryable();
            return (from so in spIndent
                    where so.SupplierId == supplierId && so.Remain>0
                    select new SubOrderModel
                    {
                        Num = so.Num,
                        Remain = so.Remain,
                        UnitId = so.UnitId,
                        OrderDate = so.SubOrder.OrderDetails.Order.OrderDate,
                        OrderDetailsId = so.SubOrder.OrderDetailsId.Value,
                        OrderId = so.SubOrder.OrderDetails.OrderId.Value,
                        StuffName = so.SubOrder.OrderDetails.StuffName,
                        StuffType = so.SubOrder.OrderDetails.StuffType,
                        KalaUid = so.SubOrder.OrderDetails.KalaUid,
                        KalaNo = so.SubOrder.OrderDetails.kalaNo,
                        SubOrderId = so.SubOrderId ?? 0,
                        SupplierIndentId = so.ID,
                        OrderType = so.SubOrder.OrderDetails.Order.OrderType,
                        SpState = so.State,
                        SellerId = so.SellerId,
                        SupplierId = so.SupplierId
                    }).AsEnumerable();
        }

        public static Person GetRelatedOrderPerson(this IRepository<Order> repository,long orderId)
        {
            var orders = repository.GetRepository<Order>().Queryable();
            return (from o in orders
                    where o.OrderId == orderId
                    select o.Person).SingleOrDefault();
        }

        public static IEnumerable<SubOrderModel> GetRelatedSubOrders(this IRepository<Order> repository, string identity, long orderDetailsId)
        {
            var rpOd = repository.GetRepository<OrderDetails>().Queryable();
            var orderDetails = (from od in rpOd
                            where od.OrderDetialsId == orderDetailsId
                            select od).Include(od=>od.Order).SingleOrDefault();
            if (orderDetails == null) return null;
            var subOrders = repository.GetRepository<SubOrder>().Queryable();
            var items = (from so in subOrders
                         where so.State == SubOrderState.None && so.Type == SubOrderType.Store
                         && so.Identity == identity && (so.OrderDetails.OrganizId == orderDetails.OrganizId && so.OrderDetails.StrategyId == orderDetails.StrategyId
                         && so.OrderDetails.Order.PersonId == orderDetails.Order.PersonId)
                         select new SubOrderModel
                         {
                             Num = so.Num,
                             Remain = so.Remain,
                             UnitId = so.UnitId,
                             OrderDate = so.OrderDetails.Order.OrderDate,
                             OrderDetailsId = so.OrderDetailsId.Value,
                             OrderId = so.OrderDetails.OrderId.Value,
                             StuffName = so.OrderDetails.StuffName,
                             StuffType = so.OrderDetails.StuffType,
                             KalaUid = so.OrderDetails.KalaUid,
                             KalaNo=so.OrderDetails.kalaNo,
                             SubOrderId = so.SubOrderId,
                             Identity = so.Identity,
                             OrderType = so.OrderDetails.Order.OrderType,
                             State = so.State,
                             SpState = SupplierIndentState.Ongoing
                         }).AsEnumerable();
            return items;
        }

        public static IEnumerable<OrderSumModel> GetOrderDetailsByKalaUid(this IRepository<Order> repository, StuffType stype, int kalaUid, DateTime fromDate, DateTime ToDate, bool filterByStuff)
        {
            var rpOd = repository.GetRepository<OrderDetails>().Queryable();
            if (filterByStuff)
            {
                return (from od in rpOd
                        where (od.Order.OrderType == OrderType.InternalRequest || od.Order.OrderType == OrderType.Store) && (od.Order.OrderDate > fromDate && od.Order.OrderDate <= ToDate)
                        && od.KalaUid == kalaUid && od.StuffType == stype
                        group od by new { od.UnitId, od.Order.OrderType, od.Order.Status, od.Order.OrderDate } into g
                        where g.Count() >= 1
                        select new OrderSumModel
                        {
                            KalaUid = kalaUid,
                            Num = g.Sum(v => v.Num),
                            OrderType = g.Key.OrderType,
                            Status = g.Key.Status,
                            UnitId = g.Key.UnitId,
                            OrderDate = g.Key.OrderDate
                        }).AsEnumerable();
            }
            else
            {
                return (from od in rpOd
                        where (od.Order.OrderType == OrderType.InternalRequest || od.Order.OrderType == OrderType.Store) && (od.Order.OrderDate > fromDate && od.Order.OrderDate <= ToDate)
                        && od.KalaUid == kalaUid
                        group od by new { od.UnitId, od.Order.OrderType, od.Order.Status, od.Order.OrderDate } into g
                        where g.Count() >= 1
                        select new OrderSumModel
                        {
                            KalaUid = kalaUid,
                            Num = g.Sum(v => v.Num),
                            OrderType = g.Key.OrderType,
                            Status = g.Key.Status,
                            UnitId = g.Key.UnitId,
                            OrderDate = g.Key.OrderDate
                        }).AsEnumerable();
            }
           
        }
        
        public static SubOrder GetSubOrderBySupplierIndent(this IRepository<Order> repository, long spIndentId)
        {
            var rpOd = repository.GetRepository<SubOrder>().Queryable();
            return rpOd.Where(so => so.SupplierIndents.Any(ma => ma.ID == spIndentId))
                .Include(so => so.SupplierIndents).FirstOrDefault();
        }

        public static IEnumerable<SupplierIndentModel> GetSupplierIndentModel(this IRepository<Order> repository,int supplierid,HashSet<SupplierIndentState> spStates)
        {
            var rpOd = repository.GetRepository<SupplierIndent>().Queryable();
            return (from c in rpOd
             where c.SupplierId == supplierid
             && spStates.Contains(c.State)
              select new SupplierIndentModel
             {
                 IndentId=c.ID,
                 NationalId=c.SubOrder.OrderDetails.Order.Person.NationalId,
                 PersonName= c.SubOrder.OrderDetails.Order.Person.FirstName+" "+ c.SubOrder.OrderDetails.Order.Person.LastName,
                 Num=c.Num,
                 UnitId=c.UnitId,
                 StuffType=c.SubOrder.OrderDetails.StuffType,
                 StuffName=c.SubOrder.OrderDetails.StuffName,
                 KalaUid=c.SubOrder.OrderDetails.KalaUid,
                 kalaNo=c.SubOrder.OrderDetails.kalaNo,
                 SellerId=c.SellerId,
                 Remain=c.Remain,
                 OrganizId=c.SubOrder.OrderDetails.OrganizId,
                 StrategyId=c.SubOrder.OrderDetails.StrategyId,
                 StoreId=c.SubOrder.OrderDetails.StoreId,
                 StoreAddressId=c.SubOrder.OrderDetails.StoreDesignId,
                 State=c.State,
                 OrderDate=c.SubOrder.OrderDetails.Order.OrderDate
             }).AsEnumerable();
        }

        public static Tuple<IEnumerable<OrderDetails>,OrderDetails> GetRelatedOrderDetailsByIndent(this IRepository<Order> repository,Int64 spIndentId)
        {
            var rpOd = repository.GetRepository<SupplierIndent>().Queryable();
            var od = rpOd.Where(s => s.ID == spIndentId).Select(s => s.SubOrder.OrderDetails).Single();
            var ods = rpOd.Where(s => s.ID == spIndentId).SelectMany(s => s.SubOrder.OrderDetails.Order.OrderDetails).AsEnumerable();
            return new Tuple<IEnumerable<OrderDetails>, OrderDetails>(ods, od);
        }

        public static UnConsumption GetParentBelongingAsstBySupllierIndent(this IRepository<Order> repository, long spIndent)
        {
            var rpOd = repository.GetRepository<SupplierIndent>().Queryable();
            var unconsumption= repository.GetRepository<MovableAsset>().Queryable().OfType<UnConsumption>();
            long? unconsumId = rpOd.Where(soid => soid.ID == spIndent).Select(soid => soid.SubOrder.OrderDetails.BelongingParentLable).FirstOrDefault();
            if (unconsumId.HasValue)
            {
                return unconsumption.Single(s => s.AssetId == unconsumId.Value);
            }
            return null;
        }

        public static SupplierIndent GetSupplierIndent(this IRepository<Order> repository, long spId)
        {
            var rpOd = repository.GetRepository<SupplierIndent>().Queryable();
            return rpOd.SingleOrDefault(sp => sp.ID == spId);
        }

        public static SubOrder GetSubOrder(this IRepository<Order> repository, long subOrderId)
        {
            var rpOd = repository.GetRepository<SubOrder>().Queryable();
            return rpOd.SingleOrDefault(sb => sb.SubOrderId == subOrderId);
        }

        public static bool IsTrenderOffersConfirmed(this IRepository<Order> repository, long subOrderId)
        {
            var rpOd = repository.GetRepository<SubOrder>().Queryable();
            return rpOd.Where(sb=>sb.SubOrderId==subOrderId).Any(sb => sb.SupplierTrenderOffers.Any(st => st.ISEnableTrender));
        }

        public static SupplierTrenderOffer GetSubOrderTrenderOffers(this IRepository<Order> repository, int trenderOfferId)
        {
            var rpOd = repository.GetRepository<SupplierTrenderOffer>().Queryable();
            return rpOd.SingleOrDefault(st => st.Id == trenderOfferId);
        }

        public static IEnumerable<SupplierTrenderOffer> GetTrenderOffersBySubOrder(this IRepository<Order> repository, long subOrderId)
        {
            var rpOd = repository.GetRepository<SupplierTrenderOffer>().Queryable();
            return rpOd.Where(rt => rt.SubOrderId == subOrderId).AsEnumerable();
        }
    }
}
