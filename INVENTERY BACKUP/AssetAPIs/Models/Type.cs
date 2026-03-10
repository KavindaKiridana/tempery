
namespace AssetAPIs.Models
{
    public class Type
    {
        public int Id { get; set; }

        public string AssetType { get; set; }
        public string Category { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsUsed { get; set; }
    }
}