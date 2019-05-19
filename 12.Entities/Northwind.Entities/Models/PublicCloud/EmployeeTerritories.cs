using System;
using System.Collections.Generic;
using URF.Core.EF.Trackable;
namespace Northwind.Entities.Models.PublicCloud
{
    public partial class EmployeeTerritories:Entity
    {
        public int EmployeeId { get; set; }
        public string TerritoryId { get; set; }

        public virtual Employees Employee { get; set; }
        public virtual Territories Territory { get; set; }
    }
}
