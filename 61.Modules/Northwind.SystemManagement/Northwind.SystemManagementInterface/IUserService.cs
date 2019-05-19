using Northwind.Entities.BusinessModels.PublicCloud;
using System;
using System.Collections.Generic;
using System.Text;

namespace Northwind.SystemManagementInterface
{
    public interface IUserService
    {
        UserBM Authenticate(string username, string password);
        IEnumerable<UserBM> GetAll();
    }
}
