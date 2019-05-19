using System;
using System.Collections.Generic;

namespace Northwind.Entities.BusinessModels.PublicCloud
{
    public partial class AuthorizesBM
    {
        public int AuthorizationId { get; set; }
        public string AuthorizationType { get; set; }
        public string Description { get; set; }
    }
}
