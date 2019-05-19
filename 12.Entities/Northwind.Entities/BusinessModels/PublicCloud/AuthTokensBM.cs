using System;
using System.Collections.Generic;

namespace Northwind.Entities.BusinessModels.PublicCloud
{
    public partial class AuthTokensBM
    {
        public int TokenId { get; set; }
        public int? AccountId { get; set; }
        public string Token { get; set; }
        public DateTime IssuedOn { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}
