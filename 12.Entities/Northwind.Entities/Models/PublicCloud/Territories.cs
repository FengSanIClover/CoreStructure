﻿using System;
using System.Collections.Generic;
using URF.Core.EF.Trackable;
namespace Northwind.Entities.Models.PublicCloud
{
    public partial class Territories:Entity
    {
        public Territories()
        {
            EmployeeTerritories = new HashSet<EmployeeTerritories>();
        }

        public string TerritoryId { get; set; }
        public string TerritoryDescription { get; set; }
        public int RegionId { get; set; }

        public virtual Region Region { get; set; }
        public virtual ICollection<EmployeeTerritories> EmployeeTerritories { get; set; }
    }
}
