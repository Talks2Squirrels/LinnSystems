using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinnSystemConsignmentApp
{
    class Item
    {
        public int? PackageId { get; set; }
        public string ItemCode { get; set; }
        public int Quantity { get; set; }
        public decimal UnitWeight { get; set; }

        public Item()
        {

        }
    }
}
