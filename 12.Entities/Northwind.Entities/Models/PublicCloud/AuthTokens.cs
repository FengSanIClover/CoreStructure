using System;
using System.Collections.Generic;
using URF.Core.EF.Trackable;
namespace Northwind.Entities.Models.PublicCloud
{
    public partial class AuthTokens:Entity
    {
        public int TokenId { get; set; }
        public int? AccountId { get; set; }
        public string Token { get; set; }
        public DateTime IssuedOn { get; set; }
        public DateTime ExpiresOn { get; set; }

        public virtual Accounts Account { get; set; }
        public virtual Employees AccountNavigation { get; set; }
    }
}
