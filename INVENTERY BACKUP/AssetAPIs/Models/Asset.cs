using System;
using System.ComponentModel.DataAnnotations;

namespace AssetAPIs.Models
{
    public class Asset
    {
        public string PatchRequestType { get; set; }
        public string AssetId { get; set; }
        public int Quantity { get; set; }
        public int? CompanyId { get; set; }
        public int? LocationId { get; set; }
        public int? SupplierId { get; set; }
        public int? DepartmentId { get; set; }
        public int? OsId { get; set; }
        public int? PId { get; set; }
        public int? RAMSId { get; set; }
        public int? RAMTId { get; set; }
        public int? HDDId { get; set; }
        public int? SSDId { get; set; }
        public int? DisplayId { get; set; }
        public DateTime? DoP { get; set; }
        public string FinanceAssetCode { get; set; }
        public int? Warranty { get; set; }
        [Required]
        public string Type { get; set; }
        public string ManufactureSN { get; set; }
        public bool? Brandnew { get; set; }
        public decimal? Cost { get; set; }
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public string Make { get; set; }
        public string WindowsKey { get; set; }
        public string Motherboard { get; set; }
        public bool? PowerSupply { get; set; }
        public bool? RAIDSupport { get; set; }
        public int? ModelId { get; set; }
        public string Note { get; set; }
    }
}
