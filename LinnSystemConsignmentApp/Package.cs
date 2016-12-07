using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinnSystemConsignmentApp
{
    class Package
    {
        public int? PackageId { get; set; }
        public decimal PackageWidth { get; set; }
        public decimal PackageHeight { get; set; }
        public decimal PackageDepth { get; set; }
        public string PackageType { get; set; }

        public List<Item> PackageItems { get; set; }

        public Package()
        {

        }
    }
}
