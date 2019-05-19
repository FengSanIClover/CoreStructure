using Microsoft.EntityFrameworkCore;
using Northwind.Entities.Models.PublicCloud;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using TP5.ModuleResolver.NetCore;
using URF.Core.Abstractions;
using URF.Core.Abstractions.Trackable;
using URF.Core.EF;
using URF.Core.EF.Trackable;

namespace Northwind.Entities
{
    [Export(typeof(IModule))]
    public class ModuleSetup : IModule
    {
        // TODO Config: 因為有多個資料庫，所以在註冊時指定名稱，讓 Service 可以透過名稱取得正確的 IDataContextAsync 及 IUnitOfWorkAsync。
        public void SetUp(IModuleRegister register)
        {
            // 建立 重複使用的
            register.RegisterDataContext<DbContext, NorthwindContext>("NorthwindContext");
            register.RegisterUnitOfWork<IUnitOfWork, UnitOfWork>("NorthwindContext");

            register.RegisterRepository<ITrackableRepository<Accounts>, TrackableRepository<Accounts>>("NorthwindContext");
            register.RegisterRepository<ITrackableRepository<Authorizes>, TrackableRepository<Authorizes>>("NorthwindContext");
            register.RegisterRepository<ITrackableRepository<AuthTokens>, TrackableRepository<AuthTokens>>("NorthwindContext");
            register.RegisterRepository<ITrackableRepository<Categories>, TrackableRepository<Categories>>("NorthwindContext");
            register.RegisterRepository<ITrackableRepository<Customers>, TrackableRepository<Customers>>("NorthwindContext");
            register.RegisterRepository<ITrackableRepository<Employees>, TrackableRepository<Employees>>("NorthwindContext");
            register.RegisterRepository<ITrackableRepository<EmployeeTerritories>, TrackableRepository<EmployeeTerritories>>("NorthwindContext");
            register.RegisterRepository<ITrackableRepository<OrderDetails>, TrackableRepository<OrderDetails>>("NorthwindContext");
            register.RegisterRepository<ITrackableRepository<Orders>, TrackableRepository<Orders>>("NorthwindContext");
            register.RegisterRepository<ITrackableRepository<Products>, TrackableRepository<Products>>("NorthwindContext");
            register.RegisterRepository<ITrackableRepository<Region>, TrackableRepository<Region>>("NorthwindContext");
            register.RegisterRepository<ITrackableRepository<Shippers>, TrackableRepository<Shippers>>("NorthwindContext");
            register.RegisterRepository<ITrackableRepository<Suppliers>, TrackableRepository<Suppliers>>("NorthwindContext");
            register.RegisterRepository<ITrackableRepository<Territories>, TrackableRepository<Territories>>("NorthwindContext");
        }
    }
}
