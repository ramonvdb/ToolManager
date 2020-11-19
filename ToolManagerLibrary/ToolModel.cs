using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolManagerLibrary
{
    public class ToolModel
    {   
        /// <summary>
        /// All the data for a tool in the database
        /// </summary>
        public int ToolID { get; set; }
        public string Description { get; set; }
        public string Function { get; set; }
        public string Type { get; set; }
        public double Diameter { get; set; }
        public double Length { get; set; }
        public string Holder { get; set; }
        public string Machine { get; set; }
        public string Storage { get; set; }
        public string Location { get; set; }
        public string HolderVendor { get; set; }
        public string HolderArt { get; set; }
        public string ExtensionVendor { get; set; }
        public string ExtensionArt { get; set; }
        public string ToolVendor { get; set; }
        public string ToolArt { get; set; }
    }
}
