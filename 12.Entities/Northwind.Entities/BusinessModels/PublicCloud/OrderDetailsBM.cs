using System;
using System.Collections.Generic;

namespace Northwind.Entities.BusinessModels.PublicCloud
{
    public partial class OrderDetailsBM
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }
    }
}
