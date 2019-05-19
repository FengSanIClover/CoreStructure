using System;
using System.Collections.Generic;
using URF.Core.EF.Trackable;
namespace Northwind.Entities.Models.PublicCloud
{
    public partial class Shippers:Entity
    {
        public Shippers()
        {
            Orders = new HashSet<Orders>();
        }

        public int ShipperId { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }

        public virtual ICollection<Orders> Orders { get; set; }
    }
}
