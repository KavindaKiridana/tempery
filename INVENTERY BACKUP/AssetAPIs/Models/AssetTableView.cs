using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AssetAPIs.Models
{
    public class AssetTableView
    {
        public string AssetId { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string LocationName { get; set; }
        public string ManufactureSN { get; set; }
        public string CurrentUser { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsAvailable { get; set; }
    }
}