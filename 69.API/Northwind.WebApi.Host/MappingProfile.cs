using AutoMapper;
using Northwind.Entities.BusinessModels.PublicCloud;
using Northwind.Entities.Models.PublicCloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind.WebApi.Host
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects

            CreateMap<Accounts, AccountsBM>();
            CreateMap<AccountsBM, Accounts>();

            CreateMap<Authorizes, AuthorizesBM>();
            CreateMap<AuthorizesBM, Authorizes>();

            CreateMap<AuthTokens, AuthTokensBM>();
            CreateMap<AuthTokensBM, AuthTokens>();

            CreateMap<Categories, CategoriesBM>();
            CreateMap<CategoriesBM, Categories>();

            CreateMap<Customers, CustomersBM>();
            CreateMap<CustomersBM, Customers>();

            CreateMap<Employees, EmployeesBM>();
            CreateMap<EmployeesBM, Employees>();

            CreateMap<EmployeeTerritories, EmployeeTerritoriesBM>();
            CreateMap<EmployeeTerritoriesBM, EmployeeTerritories>();

            CreateMap<OrderDetails, OrderDetailsBM>();
            CreateMap<OrderDetailsBM, OrderDetails>();

            CreateMap<Orders, OrdersBM>();
            CreateMap<OrdersBM, Orders>();

            CreateMap<Products, ProductsBM>();
            CreateMap<ProductsBM, Products>();

            CreateMap<Region, RegionBM>();
            CreateMap<RegionBM, Region>();

            CreateMap<Shippers, ShippersBM>();
            CreateMap<ShippersBM, Shippers>();

            CreateMap<Suppliers, SuppliersBM>();
            CreateMap<SuppliersBM, Suppliers>();

            CreateMap<Territories, TerritoriesBM>();
            CreateMap<TerritoriesBM, Territories>();

        }
    }
}
