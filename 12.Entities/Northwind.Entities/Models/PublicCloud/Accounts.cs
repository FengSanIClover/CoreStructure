using System;
using System.Collections.Generic;
using URF.Core.EF.Trackable;

namespace Northwind.Entities.Models.PublicCloud
{
    public partial class Accounts:Entity
    {
        public Accounts()
        {
            AuthTokens = new HashSet<AuthTokens>();
        }

        public int EmployeeId { get; set; }
        public string UserAccount { get; set; }
        public string UserPassword { get; set; }

        public virtual Employees Employees { get; set; }
        public virtual ICollection<AuthTokens> AuthTokens { get; set; }
    }
}
