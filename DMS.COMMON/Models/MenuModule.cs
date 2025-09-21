using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.COMMON.Models
{
    public class MenuModule
    {
        public int MenuModuleID { get; set; }
        public string Name { get; set; }
        public int? ParentID { get; set; }
        public bool IsModule { get; set; }
        public bool ActiveFlag { get; set; }
    }

  

}
