namespace AssetAPIs.Models
{
    public class Softwares
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool? IsUsed { get; set; }
    }

    public class InstallesSoftwares
    {
        public int InstalledSoftwareId { get; set; }
        public string SoftwareName { get; set; }
        public bool InstalledStatus { get; set; }
    }
}
