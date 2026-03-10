using System;
using System.Collections.Generic;

namespace AssetAPIs.Models
{
    public class EditAsset
    {
        public string AssetId { get; set; }
        public string Type { get; set; }
        public DateTime? DoP { get; set; } //here migth be an issue with datetime format
        public string FinanceAssetCode { get; set; }
        public int? Warranty { get; set; }
        public int CompanyId { get; set; }
        public string CName { get; set; }
        public List<Company> CompanyList { get; set; }
        public int LocationId { get; set; }
        public string LName { get; set; }
        public List<Location> LocationList { get; set; }
        public string ManufactureSN { get; set; }
        public bool? Brandnew { get; set; }
        public decimal? Cost { get; set; }
        public string Name { get; set; }
        public int SupplierId { get; set; }
        public string SName { get; set; }
        public List<Supplier> SupplierList { get; set; }
        public int DepartmentId { get; set; }
        public string DName { get; set; }
        public List<Department> DepartmentList { get; set; }
        public string IPAddress { get; set; }
        public string Note { get; set; }
//all the attributes below here are only related to Server, Laptop and Desktop asset types (not for SparePart assets)
        public int OsId { get; set; }
        public string OS { get; set; }
        public List<OS> OSList { get; set; }
        public int PId { get; set; }
        public string Processor { get; set; }
        public List<Processor> ProcessorList { get; set; }
        public int RAMSId { get; set; }
        public string RAMSize { get; set; }
        public List<RAMSize> RAMSizeList { get; set; }
        public int RAMTId { get; set; }
        public string RAMType { get; set; }
        public List<RAMType> RAMTypeList { get; set; }
        public int HDDId { get; set; }
        public string HDD { get; set; }
        public List<HDD> HDDList { get; set; }
        public int SSDId { get; set; }
        public string SSD { get; set; }
        public List<SSD> SSDList { get; set; }
        public int DisplayId { get; set; }
        public string Display { get; set; }
        public List<Display> DisplayList { get; set; }
        public int ModelId { get; set; }
        public string Model { get; set; }
        public List<Model> ModelList { get; set; }
        public string Make { get; set; }
        public string WindowsKey { get; set; }
        public string Motherboard { get; set; }
// only if the asset type is 'Laptop' or 'Desktop' this list will be populated otherwise this softwares list would be null
        public List<Softwares> SoftwareList { get; set; }
// attributes below here are only related to Server asset type 
        public bool? PowerSupply { get; set; }
        public bool? RAIDSupport { get; set; }
    }
}