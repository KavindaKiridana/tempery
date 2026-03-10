using System.ComponentModel.DataAnnotations;

namespace AssetAPIs.Models
{
    public class PostTransaction
    {
        [Required]
        public string Type { get; set; }
        [Required]
        public string AssetId { get; set; }
        [Required]
        public string Time { get; set; }
        public int? EditedUser { get; set; }
        public string EditedUserFullName { get; set; }
        public int? FromId { get; set; }
        public string FromName { get; set; }
        public int? ToId { get; set; }
        public string ToName { get; set; }
        public string RelatedAssetId { get; set; }
        public string RelatedAssetName { get; set; }
        public string Note { get; set; }
        public decimal? RepairCost { get; set; }
        public bool? IsTempAssigned { get; set; }
        public string RepairStatus { get; set; }
        public int UserId { get; set; }
        public int? ComplainId { get; set; }
        public int? ObservationId { get; set; }
    }

    public enum TransactionType
    {
        ADD_COMPLAIN,
        ADD_NEW_ASSET_TO_STORE,
        ASSET_LOCATION_CHANGED,
        ASSET_ASSIGNED_TO_USER,
        ASSET_ASSIGNED_TO_ASSET_PART,
        ASSET_ASSIGNED_TO_ASSET_MAIN,
        ASSET_REMOVE_FROM_USER,
        ASSET_RETURNED_FROM_ASSET_PART,
        ASSET_RETURNED_FROM_ASSET_MAIN,
        GIVEN_TO_REAPAIR,
        STILL_IN_REPAIR,
        RETURNED_FROM_REPAIR,
        ASSET_DESTROYED_FROM_USER,
        ASSET_DESTROYED_STOCK,
        ASSET_LOST_STOLEN_USER,
        ASSET_LOST_STOLEN_STOCK,
        SPARE_PART_DEACTIVATED,
        MAIN_ASSET_DEACTIVATED,
        USER_LOCATION_CHANGED,
        USER_RESIGNED,
        ITOBSERVATION,
        SPAREPART_DESTROYED,
        SPAREPART_LOST_STOLEN,
    }
}


