using System;
using System.Collections.Generic;

namespace Northwind.Entities.BusinessModels.PublicCloud
{
    public partial class CategoriesBM
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }
    }
}
