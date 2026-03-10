
namespace AssetAPIs.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsStockLocation { get; set; }
        public bool? IsActive { get; set; }
    }
}