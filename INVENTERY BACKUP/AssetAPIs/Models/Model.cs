using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetAPIs.Models
{
    public class Model
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsUsed { get; set; }
    }
}