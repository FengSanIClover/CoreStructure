using System;
using System.Collections.Generic;
using System.Text;

namespace Northwind.Entities.BusinessModels.PublicCloud
{
    public class UserBM
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
