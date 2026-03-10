using System;

namespace AssetAPIs.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Designation { get; set; }
        public string AddedUserName { get; set; }
        public DateTime AddedTime { get; set; }
        public int? DepartmentId { get; set; } 
        public string DepartmentName { get; set; }
        public int? LocationId { get; set; }
        public string LocationName { get; set; }
        public bool isCapexUser { get; set; }
        public bool IsActive { get; set; }
        public bool? IsUsed { get; set; }
    }
}
// isCapexUser=true mean this particuler user is in CAPEX-Requesting-System too
// if any user's isCapexUser=true program wouldnt allow to edit FullName, Email, IsActive attributes.but will allow to edit Phone, Designation, DepartmentId attributes
// if any user's isCapexUser=false program will allow to edit FullName, Email, Phone, Designation, DepartmentId, IsActive attributes
