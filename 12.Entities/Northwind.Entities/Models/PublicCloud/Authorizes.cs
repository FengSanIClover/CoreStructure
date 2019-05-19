using System;
using System.Collections.Generic;
using URF.Core.EF.Trackable;

namespace Northwind.Entities.Models.PublicCloud
{
    public partial class Authorizes:Entity
    {
        public Authorizes()
        {
            Employees = new HashSet<Employees>();
        }

        public int AuthorizationId { get; set; }
        public string AuthorizationType { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Employees> Employees { get; set; }
    }
}
