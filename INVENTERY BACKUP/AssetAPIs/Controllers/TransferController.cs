using AssetAPIs.Filters;
using AssetAPIs.Models;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Ajax.Utilities;
using Swashbuckle.SwaggerUi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Web.Http;
using static ClosedXML.Excel.XLPredefinedFormat;
namespace AssetAPIs.Controllers
{
    [JwtAuthentication]
    public class TransferController : ApiController
    {
        private readonly Comman common = new Comman();

        //GET /api/Transfer?Type=asset_list
        //GET /api/Transfer?Type=asset_list&UserId=123
        [HttpGet]
        public IHttpActionResult GetTransferData([FromUri] GetTransaction getTransaction)
        {
            // Validate input
            if (getTransaction == null || !getTransaction.Type.HasValue)
            {
                return BadRequest("Request Type parameter required.");
            }

            // Validate enum value is defined
            if (!Enum.IsDefined(typeof(GetListType), getTransaction.Type.Value))
            {
                return BadRequest("Invalid Type value.");
            }

            List<UnAssignedAssets> assetsList = new List<UnAssignedAssets>();
            UnAssignedAssets asset = null;
            SqlConnection con = null;
            SqlDataReader reader = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            if (userId == 0)
            {
                return Unauthorized();
            }

            string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                common.LogError(new Exception("Connection string is null or empty"), "GetTransferData - Configuration Error");
                return InternalServerError(new Exception("Database configuration error"));
            }
            con = new SqlConnection(connectionString);

            try
            {
                if (getTransaction.Type == GetListType.asset_list)
                {
                    assetsList = GetUnassignedAssetsList(con, reader);
                    return Ok(assetsList);
                }
                else if (getTransaction.Type == GetListType.get_asset_name_by_id)
                {
                    if (string.IsNullOrEmpty(getTransaction.AssetId) || getTransaction.AssetId == "0")
                    {
                        return BadRequest("Request AssetId parameter required.");
                    }
                    string assetName = GetAssetNameOnly(con, getTransaction.AssetId);
                    return Ok(new { AssetName = assetName });
                }
                else if (getTransaction.Type == GetListType.asset_destroyed_lost_stolen)
                {
                    if (string.IsNullOrEmpty(getTransaction.AssetId) || getTransaction.AssetId == "0")
                    {
                        return BadRequest("Request AssetId parameter required.");
                    }
                    TransactionPage transactionPage = new TransactionPage();
                    transactionPage = GetAssetAssignmentStatus(con, reader, getTransaction.AssetId, true);
                    transactionPage.AssetName = GetAssetNameOnly(con, getTransaction.AssetId);
                    if (transactionPage.HasExistingUser == false)
                    {
                        // Check if this asset is currently an active spare part of any other main asset
                        transactionPage.IsActiveSparePart = IsAssetActiveSparePart(con, getTransaction.AssetId);
                        if (transactionPage.IsActiveSparePart ?? false)
                        {
                            // Get the MainAssetId this asset is actively attached to
                            string getMainAssetQuery = @"
            SELECT MainAssetId 
            FROM AssetSpareParts 
            WHERE SparePartId = @AssetId AND IsActive = 1";

                            using (SqlCommand cmd = new SqlCommand(getMainAssetQuery, con))
                            {
                                cmd.Parameters.AddWithValue("@AssetId", getTransaction.AssetId);
                                cmd.CommandTimeout = 30;

                                if (con.State != System.Data.ConnectionState.Open)
                                {
                                    con.Open();
                                }
                                var result = cmd.ExecuteScalar();
                                if (result != null)
                                {
                                    transactionPage.AssociateAssetId = result.ToString();
                                }
                            }
                        }
                    }
                    return Ok(transactionPage);
                }
                else if (getTransaction.Type == GetListType.assign_to_asset)
                {
                    if (string.IsNullOrEmpty(getTransaction.AssetId) || getTransaction.AssetId == "0")
                    {
                        return BadRequest("Request AssetId parameter required.");
                    }
                    AssignToAsset assignToAsset = new AssignToAsset();
                    assignToAsset = getAssetInfoWithAvailableAssets(con, reader, getTransaction.AssetId);
                    assignToAsset.AssetName = GetAssetNameOnly(con, getTransaction.AssetId);
                    return Ok(assignToAsset);
                }
                else if (getTransaction.Type == GetListType.transaction_page)
                {
                    if (string.IsNullOrEmpty(getTransaction.AssetId) || getTransaction.AssetId == "0")
                    {
                        return BadRequest("Request AssetId parameter required.");
                    }
                    TransactionPage transactionPage = GetAssetAssignmentStatus(con, reader, getTransaction.AssetId, false);
                    transactionPage.IsActiveAsset = CheckIfAssetIsActive(con, reader, getTransaction.AssetId).IsActive;
                    transactionPage.IsActiveSparePart = IsAssetActiveSparePart(con, getTransaction.AssetId);
                    transactionPage.HaveActiveSpareParts = FindHaveActiveSpareParts(con, getTransaction.AssetId);
                    transactionPage.HasOngoingRepair = CheckIfAssetHasOngoingRepair(con, getTransaction.AssetId);
                    return Ok(transactionPage);
                }
                else if (getTransaction.Type == GetListType.return_from_user)
                {
                    if (string.IsNullOrEmpty(getTransaction.AssetId) || getTransaction.AssetId == "0")
                    {
                        return BadRequest("Request AssetId parameter required.");
                    }
                    asset = new UnAssignedAssets(); // <-- Initialize here
                    if (!GetAssetAssignmentStatus(con, reader, getTransaction.AssetId, false).HasExistingUser)
                    {
                        asset.HasExistingUser = false;
                    }
                    else
                    {
                        asset = GetAssetWithCurrentUserDetails(con, reader, getTransaction.AssetId);
                    }
                    asset.AssetName = GetAssetNameOnly(con, getTransaction.AssetId);
                    return Ok(asset);
                }
                else if (getTransaction.Type == GetListType.assign_to_user)
                {
                    if (string.IsNullOrEmpty(getTransaction.AssetId) || getTransaction.AssetId == "0")
                    {
                        return BadRequest("Request AssetId parameter required.");
                    }
                    asset = GetAssetNameandLocationName(con, reader, getTransaction.AssetId);
                    asset.AssetName = GetAssetNameOnly(con, getTransaction.AssetId);
                    return Ok(asset);
                }
                else if (getTransaction.Type == GetListType.asset_list_belong_to_user)
                {
                    if (getTransaction.UserId <= 0)
                    {
                        return BadRequest("Request UserId parameter required.");
                    }
                    assetsList = GetAssetsAssignedToUser(con, reader, getTransaction.UserId);
                    return Ok(assetsList);
                }
                else if (getTransaction.Type == GetListType.move_asset_to_location)
                {
                    if (string.IsNullOrEmpty(getTransaction.AssetId) || getTransaction.AssetId == "0")
                    {
                        return BadRequest("Request AssetId parameter required.");
                    }
                    MoveAssetToLocation moveAssetToLocation = new MoveAssetToLocation();
                    if (GetAssetAssignmentStatus(con, reader, getTransaction.AssetId, false).HasExistingUser)
                    {
                        moveAssetToLocation.HasExistingUser = true;
                    }
                    else
                    {
                        moveAssetToLocation = GetAssetMoveOptions(con, reader, getTransaction.AssetId);
                        moveAssetToLocation.AssetName = GetAssetNameOnly(con, getTransaction.AssetId);
                    }
                    return Ok(moveAssetToLocation);
                }
                else if (getTransaction.Type == GetListType.is_asset_active)
                {
                    if (string.IsNullOrEmpty(getTransaction.AssetId) || getTransaction.AssetId == "0")
                    {
                        return BadRequest("Request AssetId parameter required.");
                    }
                    return Ok(CheckIfAssetIsActive(con, reader, getTransaction.AssetId));
                }
                else if (getTransaction.Type == GetListType.remove_spare_parts)
                {
                    if (string.IsNullOrEmpty(getTransaction.AssetId) || getTransaction.AssetId == "0")
                    {
                        return BadRequest("Request AssetId parameter required.");
                    }
                    RemoveSpareParts removeSpareParts = new RemoveSpareParts();
                    removeSpareParts = GetAssetInfoWithSparePartsList(con, reader, getTransaction.AssetId);
                    removeSpareParts.AssetName = GetAssetNameOnly(con, getTransaction.AssetId);
                    return Ok(removeSpareParts);
                }
                else if (getTransaction.Type == GetListType.get_list_of_parts)
                {
                    if (string.IsNullOrEmpty(getTransaction.AssetId) || getTransaction.AssetId == "0")
                    {
                        return BadRequest("Request AssetId parameter required.");
                    }
                    return Ok(GetActiveSparePartsList(con, reader, getTransaction.AssetId));
                }
                else if (getTransaction.Type == GetListType.returned_from_repair)
                {
                    if (string.IsNullOrEmpty(getTransaction.AssetId) || getTransaction.AssetId == "0")
                    {
                        return BadRequest("Request AssetId parameter required.");
                    }
                    ReturnedFromRepair returnedFromRepair = new ReturnedFromRepair();
                    returnedFromRepair = GetReturnFromRapairFormInfor(con, getTransaction.AssetId);
                    returnedFromRepair.AssetName = GetAssetNameOnly(con, getTransaction.AssetId);
                    return Ok(returnedFromRepair);
                }
                else if (getTransaction.Type == GetListType.are_their_any_active_complaints)
                {
                    if (string.IsNullOrEmpty(getTransaction.AssetId) || getTransaction.AssetId == "0")
                    {
                        return BadRequest("Request AssetId parameter required.");
                    }
                    return Ok(HasActiveComplain(con, getTransaction.AssetId));
                }
                else if (getTransaction.Type == GetListType.is_active_spare_part)
                {
                    if (string.IsNullOrEmpty(getTransaction.AssetId) || getTransaction.AssetId == "0")
                    {
                        return BadRequest("Request AssetId parameter required.");
                    }
                    return Ok(IsAssetActiveSparePart(con, getTransaction.AssetId));
                }
                else if (getTransaction.Type == GetListType.is_eligible_to_location_transfer)
                {
                    if (string.IsNullOrEmpty(getTransaction.AssetId) || getTransaction.AssetId == "0")
                    {
                        return BadRequest("Request AssetId parameter required.");
                    }
                    TransactionPage transactionPage = new TransactionPage();
                    // Check if asset has an existing user
                    transactionPage.HasExistingUser = GetAssetAssignmentStatus(con, reader, getTransaction.AssetId, false).HasExistingUser;
                    // Check if asset is an active spare part
                    transactionPage.IsActiveSparePart = IsAssetActiveSparePart(con, getTransaction.AssetId);
                    return Ok(transactionPage);
                }
                else if (getTransaction.Type == GetListType.user_transfer_page_data)
                {
                    if (getTransaction.UserId <= 0)
                    {
                        return BadRequest("Request UserId parameter required.");
                    }
                    return Ok(GetUserInfo(con, getTransaction.UserId));
                }
                else if (getTransaction.Type == GetListType.get_related_users)
                {
                    if (string.IsNullOrEmpty(getTransaction.AssetId) || getTransaction.AssetId == "0")
                    {
                        return BadRequest("Request AssetId parameter required.");
                    }
                    var users = GetUsersAtAssetLocation(con, getTransaction.AssetId);
                    return Ok(users);
                }
                else if (getTransaction.Type == GetListType.get_list_of_active_complaints)
                {
                    return Ok(GetActiveComplainsForAsset(con, getTransaction.AssetId));
                }
                else if (getTransaction.Type == GetListType.get_list_of_assets_under_complain)
                {
                    // Check if ComplainId is not null and not 0
                    if (getTransaction.ComplainId <= 0)
                    {
                        return BadRequest("Request ComplainId parameter is required");
                    }
                    return Ok(GetAssetsUnderComplain(con, getTransaction.ComplainId));
                }
                else if (getTransaction.Type == GetListType.are_their_any_active_itobservations)
                {
                    return Ok(GetActiveItObservations(con, getTransaction.AssetId));
                }
                else if (getTransaction.Type == GetListType.get_list_of_softwares)
                {
                    if (string.IsNullOrEmpty(getTransaction.AssetId) || getTransaction.AssetId == "0")
                    {
                        return BadRequest("Request AssetId parameter required.");
                    }
                    return Ok(GetActiveSoftwaresList(con, getTransaction.AssetId));
                }
                else
                {
                    return BadRequest("Invalid request type parameter.");
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during GetTransferData operation", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (InvalidCastException castEx)
            {
                common.LogError(castEx, "Data conversion error during GetTransferData operation", userId);
                return InternalServerError(new Exception("Data format error. Please contact support."));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during GetTransferData operation", userId);
                return InternalServerError(new Exception("Unable to retrieve unassigned assets. Please try again later."));
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        [NonAction]
        public List<string> GetActiveSoftwaresList(SqlConnection con, string AssetId)
        {
            List<string> softwareNames = new List<string>();

            string query = @"
        SELECT s.SoftwareName
        FROM InstalledSoftwares isf
        INNER JOIN Software s ON isf.SoftwareId = s.SoftwareId
        WHERE isf.AssetId = @AssetId AND isf.IsActive = 1 AND s.IsActive = 1
        ORDER BY s.SoftwareName";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", AssetId);
                cmd.CommandTimeout = 30;

                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        softwareNames.Add(reader["SoftwareName"] != DBNull.Value ? reader["SoftwareName"].ToString() : "");
                    }
                }
            }

            return softwareNames;
        }

        [NonAction]
        public List<UnAssignedAssets> GetAssetsUnderComplain(SqlConnection con, int ComplainId)
        {
            List<UnAssignedAssets> assets = new List<UnAssignedAssets>();

            // Query to get all assets linked to the given ComplainId
            string query = @"
        SELECT a.AssetId, a.Name
        FROM Asset a
        INNER JOIN AssetComplains ac ON a.AssetId = ac.AssetId
        INNER JOIN Stocks s ON a.AssetId = s.AssetId
        WHERE ac.ComplainId = @ComplainId AND s.Quantity > 0
        ORDER BY a.Name";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@ComplainId", ComplainId);
                cmd.CommandTimeout = 30;

                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        assets.Add(new UnAssignedAssets
                        {
                            AssetId = reader["AssetId"] != DBNull.Value ? reader["AssetId"].ToString() : "",
                            AssetName = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : ""
                            // Other properties are ignored as per requirements
                        });
                    }
                }
            }

            return assets;
        }

        [NonAction]
        public ItObservationTracker GetActiveItObservations(SqlConnection con, string AssetId)
        {
            ItObservationTracker tracker = new ItObservationTracker
            {
                HasActiveItObservations = false,
                ActiveItObservations = new List<ItObservation>()
            };
            string query = @"
        SELECT
            o.ObservationId,
            o.ObservedBy,
            u.FullName AS ObservedByName,
            o.ObservationNote,
            o.ObservationTime,
            o.ActionTaken
        FROM ITObservation o
        INNER JOIN Users u ON o.ObservedBy = u.UsersId
        WHERE o.AssetId = @AssetId AND o.ActionTaken = 0 AND o.IsActive = 1
        ORDER BY o.ObservationTime DESC";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", AssetId);
                cmd.CommandTimeout = 30;

                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tracker.ActiveItObservations.Add(new ItObservation
                        {
                            ObservationId = Convert.ToInt32(reader["ObservationId"]),
                            // ObservedBy = reader["ObservedBy"] != DBNull.Value ? (int?)Convert.ToInt32(reader["ObservedBy"]) : null,
                            ObservedByName = reader["ObservedByName"] != DBNull.Value ? reader["ObservedByName"].ToString() : null,
                            ObservationNote = reader["ObservationNote"] != DBNull.Value ? reader["ObservationNote"].ToString() : null,
                            ObservationTime = reader["ObservationTime"] != DBNull.Value ? Convert.ToDateTime(reader["ObservationTime"]) : System.DateTime.MinValue,
                            ActionTaken = Convert.ToBoolean(reader["ActionTaken"])
                        }); ;
                    }
                }
            }
            tracker.HasActiveItObservations = tracker.ActiveItObservations.Any();
            return tracker;
        }

        [NonAction]
        public List<ComplainInfo> GetActiveComplainsForAsset(SqlConnection con, string AssetId)
        {
            List<ComplainInfo> complains = new List<ComplainInfo>();

            string query = @"
        SELECT 
            c.ComplainId,
            u.FullName AS UserName,
            c.Note,
            c.Time AS CreatedAt
        FROM Complains c
        INNER JOIN AssetComplains ac ON c.ComplainId = ac.ComplainId
        INNER JOIN Users u ON c.UserId = u.UsersId
        WHERE ac.AssetId = @AssetId
          AND c.IsActive = 1
        ORDER BY c.Time DESC";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", AssetId);
                cmd.CommandTimeout = 30;

                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        complains.Add(new ComplainInfo
                        {
                            ComplainId = Convert.ToInt32(reader["ComplainId"]),
                            UserName = reader["UserName"] != DBNull.Value ? reader["UserName"].ToString() : "",
                            Note = reader["Note"] != DBNull.Value ? reader["Note"].ToString() : "",
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                        });
                    }
                }
            }

            return complains;
        }

        [NonAction]
        public List<User> GetUsersAtAssetLocation(SqlConnection con, string AssetId)
        {
            List<User> users = new List<User>();

            // 1. Get the current LocationId of the asset from the Stocks table
            string locationQuery = @"
        SELECT LocationId
        FROM Stocks
        WHERE AssetId = @AssetId";

            int? locationId = null;

            using (SqlCommand cmd = new SqlCommand(locationQuery, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", AssetId);
                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }

                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    locationId = Convert.ToInt32(result);
                }
            }

            // 2. If locationId is found, get all active users at that location
            if (locationId.HasValue)
            {
                string usersQuery = @"
            SELECT UsersId AS Id, FullName, IsActive
            FROM Users
            WHERE LocationId = @LocationId AND IsActive = 1
            ORDER BY FullName";

                using (SqlCommand cmd = new SqlCommand(usersQuery, con))
                {
                    cmd.Parameters.AddWithValue("@LocationId", locationId.Value);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                FullName = reader["FullName"] != DBNull.Value ? reader["FullName"].ToString() : "",
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            });
                        }
                    }
                }
            }

            return users;
        }

        [NonAction]
        public UserInfor GetUserInfo(SqlConnection con, int UserId)
        {
            UserInfor userdata = new UserInfor();
            // Query to get the user's name, current location ID, and location name
            string query = @"
        SELECT
            u.FullName AS UserName,
            l.LocationId,
            l.LName AS LocationName
        FROM Users u
        INNER JOIN Location l ON u.LocationId = l.LocationId
        WHERE u.UsersId = @UserId";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.CommandTimeout = 30;

                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        userdata.UserName = reader["UserName"] != DBNull.Value ? reader["UserName"].ToString() : null;
                        userdata.LocationId = reader["LocationId"] != DBNull.Value ? Convert.ToInt32(reader["LocationId"]) : 0;
                        userdata.LocationName = reader["LocationName"] != DBNull.Value ? reader["LocationName"].ToString() : null;
                    }
                }
            }
            return userdata;
        }

        [NonAction]
        public bool HasActiveComplain(SqlConnection con, string AssetId)
        {
            string query = @"
        SELECT 1
        FROM AssetComplains ac
        INNER JOIN Complains c ON ac.ComplainId = c.ComplainId
        WHERE ac.AssetId = @AssetId AND c.IsActive = 1";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", AssetId);
                cmd.CommandTimeout = 30;

                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }

                var result = cmd.ExecuteScalar();

                // If any record exists, return true; otherwise, return false
                return result != null;
            }
        }

        [NonAction]
        public List<UnAssignedAssets> GetActiveSparePartsList(SqlConnection con, SqlDataReader reader, string AssetId)
        {
            List<UnAssignedAssets> sparePartsList = new List<UnAssignedAssets>();

            if (con != null && con.State == System.Data.ConnectionState.Open)
            {
                con.Close();
            }

            // 1. Get all active spare parts for the given main asset
            string sparePartsQuery = @"
        SELECT SparePartId 
        FROM AssetSpareParts 
        WHERE MainAssetId = @AssetId AND IsActive = 1";

            List<string> sparePartIds = new List<string>();

            using (SqlCommand cmd = new SqlCommand(sparePartsQuery, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", AssetId);

                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }

                using (SqlDataReader spareReader = cmd.ExecuteReader())
                {
                    while (spareReader.Read())
                    {
                        sparePartIds.Add(spareReader["SparePartId"].ToString());
                    }
                } // Reader is closed here
            }

            // 2. Get the name of each spare part from the Asset table
            foreach (string sparePartId in sparePartIds)
            {
                string assetNameQuery = @"
            SELECT Name 
            FROM Asset 
            WHERE AssetId = @SparePartId";

                using (SqlCommand nameCmd = new SqlCommand(assetNameQuery, con))
                {
                    nameCmd.Parameters.AddWithValue("@SparePartId", sparePartId);

                    using (SqlDataReader nameReader = nameCmd.ExecuteReader())
                    {
                        if (nameReader.Read())
                        {
                            sparePartsList.Add(new UnAssignedAssets
                            {
                                AssetId = sparePartId,
                                AssetName = nameReader["Name"] != DBNull.Value ? nameReader["Name"].ToString() : ""
                            });
                        }
                    }
                }
            }

            if (con != null && con.State == System.Data.ConnectionState.Open)
            {
                con.Close();
            }

            return sparePartsList;
        }

        [NonAction]
        public string GetAssetNameOnly(SqlConnection con, string AssetId)
        {
            var assetName = string.Empty;
            string query = @"
        SELECT Name
        FROM Asset
        WHERE AssetId = @AssetId";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
                cmd.Parameters.AddWithValue("@AssetId", AssetId);
                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                var result = cmd.ExecuteScalar();
                assetName = result != null ? result.ToString() : string.Empty;
            }
            if (con.State == System.Data.ConnectionState.Open)
            {
                con.Close();
            }
            return assetName;
        }

        [NonAction]
        public ReturnedFromRepair GetReturnFromRapairFormInfor(SqlConnection con, string AssetId)
        {
            ReturnedFromRepair returnedFromRepair = new ReturnedFromRepair();

            // Query to get the last 'GIVEN_TO_REAPAIR' or 'STILL_IN_REPAIR' transaction
            // and join with Supplier and Asset tables to get required information
            string query = @"
SELECT TOP 1 
t.ToId AS SupplierId,
s.SName AS SupplierName,
t.RepairCost,
t.IsTempAssigned
FROM Transactions as t
left join Supplier as s on s.SupplierId=t.ToId
WHERE AssetId = @AssetId
  AND Type IN ('GIVEN_TO_REAPAIR', 'STILL_IN_REPAIR')
ORDER BY Time DESC";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@AssetId", AssetId);
            cmd.CommandTimeout = 30;

            if (con.State != System.Data.ConnectionState.Open)
            {
                con.Open();
            }

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    returnedFromRepair.SupplierId = reader["SupplierId"] != DBNull.Value
                        ? (int?)Convert.ToInt32(reader["SupplierId"])
                        : null;

                    returnedFromRepair.SupplierName = reader["SupplierName"] != DBNull.Value
                        ? reader["SupplierName"].ToString()
                        : "";

                    returnedFromRepair.Cost = reader["RepairCost"] != DBNull.Value
                        ? (decimal?)Convert.ToDecimal(reader["RepairCost"])
                        : null;

                    returnedFromRepair.IsTempAssigned = reader["IsTempAssigned"] != DBNull.Value
                        ? (bool?)Convert.ToBoolean(reader["IsTempAssigned"])
                        : null;
                }
            }

            return returnedFromRepair;
        }

        // Checks if an asset has any ongoing repairs.
        // Returns true if the asset has an ongoing repair (RepairStatus = 1), otherwise false.
        [NonAction]
        public bool CheckIfAssetHasOngoingRepair(SqlConnection con, string AssetId)
        {
            string query = @"
    SELECT
        CASE
            WHEN EXISTS (
                SELECT 1
                FROM Repairs
                WHERE AssetId = @AssetId AND RepairStatus = 1
            )
            THEN 1
            ELSE 0
        END AS HasOngoingRepair";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", AssetId);
                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                var result = cmd.ExecuteScalar();
                return Convert.ToBoolean(Convert.ToInt32(result));
            }
        }

        // Retrieves the details of a main asset and its attached spare parts.
        // A `RemoveSpareParts` object containing:
        // - The name and location of the main asset (from `Asset` and `Location` tables).
        // - A list of all active spare parts attached to the main asset (from `AssetSpareParts` table).
        //   For each spare part, the following details are included:
        //   - Spare part's name, location ID, and location name (from `Asset`, `Stocks`, and `Location` tables).
        [NonAction]
        public RemoveSpareParts GetAssetInfoWithSparePartsList(SqlConnection con, SqlDataReader reader, string AssetId)
        {
            RemoveSpareParts removeSpareParts = new RemoveSpareParts
            {
                currentSparePartsList = new List<UnAssignedAssets>()
            };

            // 1. Get the main asset's name and location
            string mainAssetQuery = @"
        SELECT
            l.LName AS LocationName
        FROM Asset a
        INNER JOIN Stocks s ON a.AssetId = s.AssetId
        INNER JOIN Location l ON s.LocationId = l.LocationId
        WHERE a.AssetId = @AssetId";

            SqlCommand cmd = new SqlCommand(mainAssetQuery, con);
            cmd.Parameters.AddWithValue("@AssetId", AssetId);
            cmd.CommandTimeout = 30;

            if (con.State != System.Data.ConnectionState.Open)
            {
                con.Open();
            }

            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                removeSpareParts.LocationName = reader["LocationName"] != DBNull.Value ? reader["LocationName"].ToString() : "";
            }
            reader.Close();

            // 2. Get all spare parts for the main asset
            string sparePartsQuery = @"
        SELECT asp.SparePartId
        FROM AssetSpareParts asp
        WHERE asp.MainAssetId = @AssetId AND asp.IsActive = 1";

            SqlCommand sparePartsCmd = new SqlCommand(sparePartsQuery, con);
            sparePartsCmd.Parameters.AddWithValue("@AssetId", AssetId);
            sparePartsCmd.CommandTimeout = 30;

            reader = sparePartsCmd.ExecuteReader();

            List<string> sparePartIds = new List<string>();
            while (reader.Read())
            {
                sparePartIds.Add(reader["SparePartId"].ToString());
            }
            reader.Close();

            // 3. For each spare part, get its details
            foreach (string sparePartId in sparePartIds)
            {
                string sparePartDetailsQuery = @"
            SELECT
                a.AssetId,
                a.Name AS AssetName,
                s.LocationId,
                l.LName AS LocationName
            FROM Asset a
            INNER JOIN Stocks s ON a.AssetId = s.AssetId
            INNER JOIN Location l ON s.LocationId = l.LocationId
            WHERE a.AssetId = @SparePartId";

                SqlCommand sparePartDetailsCmd = new SqlCommand(sparePartDetailsQuery, con);
                sparePartDetailsCmd.Parameters.AddWithValue("@SparePartId", sparePartId);
                sparePartDetailsCmd.CommandTimeout = 30;

                reader = sparePartDetailsCmd.ExecuteReader();

                if (reader.Read())
                {
                    removeSpareParts.currentSparePartsList.Add(new UnAssignedAssets
                    {
                        AssetId = reader["AssetId"] != DBNull.Value ? reader["AssetId"].ToString() : "",
                        AssetName = reader["AssetName"] != DBNull.Value ? reader["AssetName"].ToString() : "",
                        LocationId = Convert.ToInt32(reader["LocationId"]),
                        LocationName = reader["LocationName"] != DBNull.Value ? reader["LocationName"].ToString() : ""
                    });
                }
                reader.Close();
            }

            return removeSpareParts;
        }

        [NonAction]
        public bool FindHaveActiveSpareParts(SqlConnection con, string AssetId)
        {
            string query = @"
SELECT
    CASE
        WHEN EXISTS (
            SELECT 1
            FROM AssetSpareParts
            WHERE MainAssetId = @AssetId AND IsActive = 1
        )
        THEN 1
        ELSE 0
    END AS IsActiveSparePart";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", AssetId);
                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                var result = cmd.ExecuteScalar();
                return Convert.ToBoolean(Convert.ToInt32(result));
            }
        }

        [NonAction]
        public bool IsAssetActiveSparePart(SqlConnection con, string AssetId)
        {
            string query = @"
SELECT
    CASE
        WHEN EXISTS (
            SELECT 1
            FROM AssetSpareParts
            WHERE SparePartId = @AssetId AND IsActive = 1
        )
        THEN 1
        ELSE 0
    END AS IsActiveSparePart";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", AssetId);
                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                var result = cmd.ExecuteScalar();
                return Convert.ToBoolean(Convert.ToInt32(result));
            }
        }

        // Checks if an asset is active (i.e., has a stock quantity greater than zero).
        [NonAction]
        public AssetStatus CheckIfAssetIsActive(SqlConnection con, SqlDataReader reader, string AssetId)
        {
            AssetStatus isActive = new AssetStatus { IsActive = false };

            string query = @"
        SELECT Quantity
        FROM Stocks
        WHERE AssetId = @AssetId";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@AssetId", AssetId);
            cmd.CommandTimeout = 30;

            if (con.State != System.Data.ConnectionState.Open)
            {
                con.Open();
            }

            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    int quantity = Convert.ToInt32(dr["Quantity"]);
                    isActive.IsActive = quantity > 0;
                }
            }
            return isActive;
        }

        // return the curent asset infor (locations & asset-name) and all the available asset-list*
        // asset-list -> this asset cannit have any active user & not be active spare-part of any other asset
        [NonAction]
        public AssignToAsset getAssetInfoWithAvailableAssets(SqlConnection con, SqlDataReader reader, string AssetId)
        {
            AssignToAsset assignToAsset = null;

            // Query to get the main Asset's details (Location and Company)
            string mainAssetQuery = @"
        SELECT 
            a.AssetId, 
            s.LocationId, 
            l.LName AS LocationName, 
            s.CompanyId, 
            c.CName AS CompanyName
        FROM Asset a
        INNER JOIN Stocks s ON a.AssetId = s.AssetId
        INNER JOIN Location l ON s.LocationId = l.LocationId
        INNER JOIN Company c ON s.CompanyId = c.CompanyId
        WHERE a.AssetId = @AssetId";

            SqlCommand cmd = new SqlCommand(mainAssetQuery, con);
            cmd.Parameters.AddWithValue("@AssetId", AssetId);
            cmd.CommandTimeout = 30;

            if (con.State != System.Data.ConnectionState.Open) con.Open();
            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                assignToAsset = new AssignToAsset
                {
                    AssetId = reader["AssetId"].ToString(),
                    LocationId = Convert.ToInt32(reader["LocationId"]),
                    LocationName = reader["LocationName"] != DBNull.Value ? reader["LocationName"].ToString() : "",
                    CompanyId = Convert.ToInt32(reader["CompanyId"]),
                    CompanyName = reader["CompanyName"] != DBNull.Value ? reader["CompanyName"].ToString() : "",
                    AvailableAssets = new List<UnAssignedAssets>()
                };

                // Close reader to execute the next query on the same connection
                reader.Close();

                //  Query to get Available Assets that can be assigned
                // look for assets with Quantity > 0 and not currently in the AssetUsedBy table as not active

                string availableAssetsQuery = @"
    SELECT DISTINCT 
        a.AssetId, 
        a.Name, 
        s.LocationId, 
        l.LName AS LocationName
    FROM Asset a
    INNER JOIN Stocks s ON a.AssetId = s.AssetId
    INNER JOIN Location l ON s.LocationId = l.LocationId
    WHERE s.Quantity > 0 
        AND s.LocationId=@LocationId -- Same location as the main asset
        AND a.AssetId != @AssetId -- Exclude the main asset itself
        AND NOT EXISTS (
            -- Exclude assets currently assigned to users
            SELECT 1 
            FROM AssetUsedBy au 
            WHERE au.AssetId = a.AssetId 
                AND au.IsActive = 1
        )
        AND NOT EXISTS (
            -- Exclude assets that are active spare parts of other assets
            SELECT 1 
            FROM AssetSpareParts asp 
            WHERE asp.SparePartId = a.AssetId 
                AND asp.IsActive = 1
        )
    ORDER BY a.Name";

                SqlCommand nextCmd = new SqlCommand(availableAssetsQuery, con);
                nextCmd.Parameters.AddWithValue("@AssetId", AssetId);
                nextCmd.Parameters.AddWithValue("@LocationId", assignToAsset.LocationId);

                reader = nextCmd.ExecuteReader();

                while (reader.Read())
                {
                    assignToAsset.AvailableAssets.Add(new UnAssignedAssets
                    {
                        AssetId = reader["AssetId"] != DBNull.Value ? reader["AssetId"].ToString() : "",
                        AssetName = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : "",
                        LocationId = Convert.ToInt32(reader["LocationId"]),
                        LocationName = reader["LocationName"] != DBNull.Value ? reader["LocationName"].ToString() : ""
                    });
                }
            }

            return assignToAsset;
        }

        // Determines if an asset is currently assigned to a user and, optionally, retrieves the last place (user or location) it was assigned to.
        // If DoesLastPlaceNeed true, retrieves the last place (user or location) the asset was assigned to.
        [NonAction]
        public TransactionPage GetAssetAssignmentStatus(SqlConnection con, SqlDataReader reader, string AssetId, bool DoesLastPlaceNeed)
        {
            TransactionPage transactionPage = null;

            // Build the query - if DoesLastPlaceNeed is true, we need to retrieve FromId and FromName
            string query = @"
        SELECT 
            /* 1. Check if asset is currently assigned to a user */
            CASE 
                WHEN EXISTS(
                    SELECT 1 
                    FROM AssetUsedBy as AUB 
                    WHERE AUB.AssetId = @AssetId 
                    AND AUB.IsActive = 1
                ) THEN 1 
                ELSE 0 
            END AS HasExistingUser";

            // If DoesLastPlaceNeed is true, we need to determine the last place (either user or location)
            if (DoesLastPlaceNeed)
            {
                query += @",
            /* 2. Get FromId - either UserId if assigned to user, or LocationId if in stock */
            CASE 
                WHEN EXISTS(
                    SELECT 1 
                    FROM AssetUsedBy 
                    WHERE AssetId = @AssetId 
                    AND IsActive = 1
                ) 
                THEN (
                    -- Asset is assigned to a user, get the UserId
                    SELECT TOP 1 UsedBy 
                    FROM AssetUsedBy 
                    WHERE AssetId = @AssetId 
                    AND IsActive = 1
                )
                ELSE (
                    -- Asset is in stock, get the LocationId
                    SELECT TOP 1 LocationId 
                    FROM Stocks 
                    WHERE AssetId = @AssetId
                )
            END AS FromId,
            
            /* 3. Get FromName - either User's FullName or Location's LName */
            CASE 
                WHEN EXISTS(
                    SELECT 1 
                    FROM AssetUsedBy 
                    WHERE AssetId = @AssetId 
                    AND IsActive = 1
                ) 
                THEN (
                    -- Asset is assigned to a user, get the User's FullName
                    SELECT TOP 1 u.FullName 
                    FROM AssetUsedBy au
                    INNER JOIN Users u ON au.UsedBy = u.UsersId
                    WHERE au.AssetId = @AssetId 
                    AND au.IsActive = 1
                )
                ELSE (
                    -- Asset is in stock, get the Location's name
                    SELECT TOP 1 l.LName 
                    FROM Stocks s
                    INNER JOIN Location l ON s.LocationId = l.LocationId
                    WHERE s.AssetId = @AssetId
                )
            END AS FromName";
            }

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@AssetId", AssetId);
            cmd.CommandTimeout = 30;

            // Open connection if not already open
            if (con.State != System.Data.ConnectionState.Open)
            {
                con.Open();
            }

            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                // Initialize the TransactionPage object with HasExistingUser
                transactionPage = new TransactionPage
                {
                    HasExistingUser = Convert.ToBoolean(reader["HasExistingUser"])
                };

                // If DoesLastPlaceNeed is true, populate FromId and FromName
                if (DoesLastPlaceNeed)
                {
                    // FromId can be either UserId or LocationId (both are integers)
                    transactionPage.FromId = reader["FromId"] != DBNull.Value
                        ? (int?)Convert.ToInt32(reader["FromId"])
                        : null;

                    // FromName can be either User's FullName or Location's LName
                    transactionPage.FromName = reader["FromName"] != DBNull.Value
                        ? reader["FromName"].ToString()
                        : null;
                }
            }
            if (reader != null && !reader.IsClosed)
            {
                reader.Close();
            }
            return transactionPage;
        }

        // Retrieves the details of an asset and the user it is currently assigned to.
        [NonAction]
        public UnAssignedAssets GetAssetWithCurrentUserDetails(SqlConnection con, SqlDataReader reader, string AssetId)
        {
            UnAssignedAssets asset = null;
            string query = @"
    SELECT
        a.AssetId,
        s.LocationId,
        l.LName AS LocationName,
        au.UsedBy AS UserId,
        u.FullName AS UserName
    FROM Asset a
    INNER JOIN Stocks s ON a.AssetId = s.AssetId
    INNER JOIN Location l ON s.LocationId = l.LocationId
    INNER JOIN AssetUsedBy au ON a.AssetId = au.AssetId
    INNER JOIN Users u ON au.UsedBy = u.UsersId
    WHERE au.IsActive=1 and a.AssetId = @AssetId";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@AssetId", AssetId);
            cmd.CommandTimeout = 30;
            if (con.State != System.Data.ConnectionState.Open)
            {
                con.Open();
            }
            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                asset = new UnAssignedAssets
                {
                    HasExistingUser = true,
                    LocationId = Convert.ToInt32(reader["LocationId"]),
                    LocationName = reader["LocationName"] != DBNull.Value ? reader["LocationName"].ToString() : "",
                    UserId = Convert.ToInt32(reader["UserId"]),
                    UserName = reader["UserName"] != DBNull.Value ? reader["UserName"].ToString() : ""
                };
            }

            return asset;
        }

        // Retrieves the name and location details of a specific asset.
        [NonAction]
        public UnAssignedAssets GetAssetNameandLocationName(SqlConnection con, SqlDataReader reader, string AssetId)
        {
            UnAssignedAssets asset = null;

            string query = @"
        SELECT
            a.AssetId,
            s.LocationId,
            l.LName AS LocationName
        FROM Asset a
        INNER JOIN Stocks s ON a.AssetId = s.AssetId
        INNER JOIN Location l ON s.LocationId = l.LocationId
        WHERE a.AssetId = @AssetId";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@AssetId", AssetId);
            cmd.CommandTimeout = 30;
            con.Open();
            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                asset = new UnAssignedAssets
                {
                    LocationId = Convert.ToInt32(reader["LocationId"]),
                    LocationName = reader["LocationName"] != DBNull.Value ? reader["LocationName"].ToString() : ""
                };
            }
            return asset;
        }

        // Retrieves the current location of an asset and a list of available locations it can be moved to.
        [NonAction]
        public MoveAssetToLocation GetAssetMoveOptions(SqlConnection con, SqlDataReader reader, string AssetId)
        {
            if (con.State == System.Data.ConnectionState.Open)
            {
                con.Close();
            }
            MoveAssetToLocation moveAssetToLocation = null;

            // Query to get current asset location/company and all available locations
            string query = @"
        SELECT 
            s.LocationId AS ExistingLocationId,
            s.CompanyId AS ExistingCompanyId,
            l.LName AS ExistingLocationName,
            c.CName AS ExistingCompanyName
        FROM Stocks s
        INNER JOIN Location l ON s.LocationId = l.LocationId
        INNER JOIN Company c ON s.CompanyId = c.CompanyId
        WHERE s.AssetId = @AssetId AND s.Quantity > 0";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@AssetId", AssetId);
            cmd.CommandTimeout = 30;

            con.Open();
            reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                int existingLocationId = Convert.ToInt32(reader["ExistingLocationId"]);

                MoveAssetToLocation assetLocation = new MoveAssetToLocation
                {
                    HasExistingUser = false,
                    ExistingLocationId = existingLocationId,
                    ExitingCompanyId = Convert.ToInt32(reader["ExistingCompanyId"]),
                    ExistingLocationName = reader["ExistingLocationName"].ToString(),
                    ExistingCompanyName = reader["ExistingCompanyName"].ToString(),
                    NextLocations = new List<NextLocations>()
                };

                reader.Close();

                // Get all available locations except the current one
                string nextLocationsQuery = @"
            SELECT LocationId, LName
            FROM Location
            WHERE IsActive = 1 AND LocationId != @ExistingLocationId
            ORDER BY LName";

                SqlCommand nextCmd = new SqlCommand(nextLocationsQuery, con);
                nextCmd.Parameters.AddWithValue("@ExistingLocationId", existingLocationId);
                nextCmd.CommandTimeout = 30;

                reader = nextCmd.ExecuteReader();

                while (reader.Read())
                {
                    assetLocation.NextLocations.Add(new NextLocations
                    {
                        LocationId = Convert.ToInt32(reader["LocationId"]),
                        LocationName = reader["LName"].ToString()
                    });
                }

                moveAssetToLocation = assetLocation;
            }

            return moveAssetToLocation;
        }

        // Retrieves a list of assets (laptops, desktops, or servers) currently assigned to a specific user.
        [NonAction]
        public List<UnAssignedAssets> GetAssetsAssignedToUser(SqlConnection con, SqlDataReader reader, int UserId)
        {
            List<UnAssignedAssets> unassignedAssetsList = new List<UnAssignedAssets>();
            string query = @"
                SELECT a.AssetId, a.Name, s.LocationId
                FROM Asset a
                INNER JOIN Stocks s ON a.AssetId = s.AssetId
                INNER JOIN AssetUsedBy au ON a.AssetId = au.AssetId
                WHERE s.Quantity > 0 
                AND a.Type IN ('laptop', 'desktop', 'server')
                AND au.UsedBy = @UserId
                AND au.IsActive = 1
                ORDER BY a.AssetId";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@UserId", UserId);
            cmd.CommandTimeout = 30;
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                unassignedAssetsList.Add(new UnAssignedAssets
                {
                    LocationId = Convert.ToInt32(reader["LocationId"]),
                    AssetName = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : ""
                });
            }
            return (unassignedAssetsList);
        }

        // Retrieves a list of unassigned assets (laptops, desktops, or servers) available for assignment.
        [NonAction]
        public List<UnAssignedAssets> GetUnassignedAssetsList(SqlConnection con, SqlDataReader reader)
        {
            List<UnAssignedAssets> unassignedAssetsList = new List<UnAssignedAssets>();
            string query = @"
                SELECT DISTINCT a.AssetId, a.Name, s.LocationId
                FROM Asset a
                INNER JOIN Stocks s ON a.AssetId = s.AssetId
                WHERE s.Quantity > 0 
                AND a.Type IN ('laptop', 'desktop', 'server')
                AND NOT EXISTS (
                    SELECT 1 
                    FROM AssetUsedBy au 
                    WHERE au.AssetId = a.AssetId 
                    AND au.IsActive = 1
                )
                ORDER BY a.AssetId";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandTimeout = 30;
            con.Open();
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                unassignedAssetsList.Add(new UnAssignedAssets
                {
                    LocationId = Convert.ToInt32(reader["LocationId"]),
                    AssetName = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : ""
                });
            }

            return (unassignedAssetsList);
        }

        [HttpPost]
        public IHttpActionResult PostTransfer(PostTransaction postTransaction)
        {
            SqlConnection con = null;
            string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                common.LogError(new Exception("Connection string is null or empty"), "CreateCommon - Configuration Error");
                return InternalServerError(new Exception("Database configuration error"));
            }
            con = new SqlConnection(connectionString);
            con.Open();

            // Validate the incoming object first
            if (postTransaction == null)
            {
                return BadRequest("Request body is required. Please provide transaction details.");
            }

            //getting edited userId from token
            int userId = common.GetUserId((ClaimsPrincipal)User);
            if (userId == 0)
            {
                return Unauthorized();
            }
            postTransaction.Type = postTransaction.Type?.Trim();
            postTransaction.Note = postTransaction.Note?.Trim();
            if (string.IsNullOrEmpty(postTransaction.Time))
            {
                postTransaction.Time = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//if GaveAt null or empty, assign current date time
            }
            if (!Enum.IsDefined(typeof(TransactionType), postTransaction.Type))
            {
                return BadRequest("Invalid transfer type.");
            }
            if (postTransaction.Type != "USER_LOCATION_CHANGED" && postTransaction.Type != "USER_RESIGNED")
            {
                if (string.IsNullOrEmpty(postTransaction.AssetId) || postTransaction.AssetId == "0")
                {
                    return BadRequest("AssetId is required and cannot be null, empty, or zero.");
                }
                // Check if the asset is valid and active before allowing any transaction
                if (!IsThisAssetIdValidAndActive(con, postTransaction.AssetId))
                {
                    return BadRequest("Cannot perform transactions on an invalid or inactive asset.");
                }
            }

            try
            {
                if (postTransaction.Type == "ASSET_ASSIGNED_TO_USER")
                {
                    if (IsActiveWithoutUser(con, postTransaction.AssetId))
                    {
                        if (postTransaction.ToId <= 0)
                        {
                            return BadRequest("Please select a user to assign the asset");
                        }
                        AssetAssignedToUser(con, postTransaction.AssetId, postTransaction.ToId);
                        common.AddTransaction(con, postTransaction.AssetId, userId, postTransaction.FromId, postTransaction.ToId, postTransaction.Type, postTransaction.Note, null, null, null);
                        return Ok(new { Message = "Asset assigned to user successfully" });
                    }
                    else
                    {
                        return BadRequest("This asset is not available for assignment.");
                    }
                }
                else if (postTransaction.Type == "ASSET_REMOVE_FROM_USER")
                {
                    if (AssetReturnedFromUser(con, postTransaction.AssetId, postTransaction.FromId))
                    {
                        common.AddTransaction(con, postTransaction.AssetId, userId, postTransaction.FromId, postTransaction.ToId, postTransaction.Type, postTransaction.Note, null, null, null);
                        return Ok(new { Message = "Asset returned from user successfully" });
                    }
                    else
                    {
                        return BadRequest("This asset is not assigned to the specified user or is already returned.");
                    }
                }
                else if (postTransaction.Type == "ASSET_DESTROYED_FROM_USER" || postTransaction.Type == "ASSET_DESTROYED_FROM_STOCK" || postTransaction.Type == "ASSET_LOST_STOLEN_FROM_USER" || postTransaction.Type == "ASSET_LOST_STOLEN_FROM_STOCK")
                {
                    if (AssetDestroyed(con, postTransaction.AssetId))
                    {
                        common.AddTransaction(con, postTransaction.AssetId, userId, postTransaction.FromId, postTransaction.ToId, postTransaction.Type, postTransaction.Note, null, null, null);
                        if (IsAssetActiveSparePart(con, postTransaction.AssetId))
                        {
                            // Get the main asset ID that this spare part is actively assigned to
                            string getMainAssetQuery = @"
        SELECT MainAssetId 
        FROM AssetSpareParts 
        WHERE SparePartId = @AssetId AND IsActive = 1";
                            string mainAssetId = null;
                            using (SqlCommand cmd = new SqlCommand(getMainAssetQuery, con))
                            {
                                cmd.Parameters.AddWithValue("@AssetId", postTransaction.AssetId);
                                cmd.CommandTimeout = 30;
                                var result = cmd.ExecuteScalar();
                                if (result != null)
                                {
                                    mainAssetId = result.ToString();
                                }
                            }
                            if (!string.IsNullOrEmpty(mainAssetId))
                            {
                                // Update the AssetSpareParts table to deactivate this spare part
                                string deactivateSparePartQuery = @"
            UPDATE AssetSpareParts 
            SET IsActive = 0 
            WHERE SparePartId = @AssetId AND IsActive = 1";

                                using (SqlCommand cmd = new SqlCommand(deactivateSparePartQuery, con))
                                {
                                    cmd.Parameters.AddWithValue("@AssetId", postTransaction.AssetId);
                                    cmd.CommandTimeout = 30;
                                    cmd.ExecuteNonQuery();
                                }

                                // Add transaction for the main asset
                                common.AddTransaction(con, mainAssetId, userId, null, null, "SPARE_PART_DEACTIVATED", postTransaction.Note, postTransaction.AssetId, null, null);
                            }
                        }
                        if (FindHaveActiveSpareParts(con, postTransaction.AssetId))
                        {
                            // Get the list of active spare parts for this asset
                            List<string> activeSparePartIds = GetActiveSparePartIds(con, postTransaction.AssetId);

                            // Destroy each spare part
                            foreach (string sparePartId in activeSparePartIds)
                            {
                                // Update the Stocks table to set Quantity = 0 for this spare part
                                string destroySparePartQuery = @"
            UPDATE Stocks 
            SET Quantity = 0 
            WHERE AssetId = @SparePartId";

                                using (SqlCommand cmd = new SqlCommand(destroySparePartQuery, con))
                                {
                                    cmd.Parameters.AddWithValue("@SparePartId", sparePartId);
                                    cmd.CommandTimeout = 30;
                                    cmd.ExecuteNonQuery();
                                }

                                // Deactivate the spare part relationship in AssetSpareParts table
                                string deactivateRelationshipQuery = @"
            UPDATE AssetSpareParts 
            SET IsActive = 0 
            WHERE MainAssetId = @MainAssetId AND SparePartId = @SparePartId";

                                using (SqlCommand cmd = new SqlCommand(deactivateRelationshipQuery, con))
                                {
                                    cmd.Parameters.AddWithValue("@MainAssetId", postTransaction.AssetId);
                                    cmd.Parameters.AddWithValue("@SparePartId", sparePartId);
                                    cmd.CommandTimeout = 30;
                                    cmd.ExecuteNonQuery();
                                }

                                // Add transaction for each destroyed spare part
                                common.AddTransaction(con, sparePartId, userId, null, null, "MAIN_ASSET_DEACTIVATED", postTransaction.Note, postTransaction.AssetId, null, null);
                            }
                        }
                        return Ok(new { Message = "Success" });
                    }
                    else
                    {
                        return BadRequest("Asset cannot be marked as lost or stolen because its quantity is already zero.");
                    }
                }
                else if (postTransaction.Type == "SPAREPART_DESTROYED" || postTransaction.Type == "SPAREPART_LOST_STOLEN")
                {
                    if (AssetDestroyed(con, postTransaction.AssetId))
                    {
                        common.AddTransaction(con, postTransaction.AssetId, userId, null, null, postTransaction.Type, postTransaction.Note, postTransaction.RelatedAssetId, null, null);
                        if (IsAssetActiveSparePart(con, postTransaction.AssetId))
                        {
                            // Get the main asset ID that this spare part is actively assigned to
                            string getMainAssetQuery = @"
        SELECT MainAssetId 
        FROM AssetSpareParts 
        WHERE SparePartId = @AssetId AND IsActive = 1";
                            string mainAssetId = null;
                            using (SqlCommand cmd = new SqlCommand(getMainAssetQuery, con))
                            {
                                cmd.Parameters.AddWithValue("@AssetId", postTransaction.AssetId);
                                cmd.CommandTimeout = 30;
                                var result = cmd.ExecuteScalar();
                                if (result != null)
                                {
                                    mainAssetId = result.ToString();
                                }
                            }
                            if (!string.IsNullOrEmpty(mainAssetId))
                            {
                                // Update the AssetSpareParts table to deactivate this spare part
                                string deactivateSparePartQuery = @"
            UPDATE AssetSpareParts 
            SET IsActive = 0 
            WHERE SparePartId = @AssetId AND IsActive = 1";

                                using (SqlCommand cmd = new SqlCommand(deactivateSparePartQuery, con))
                                {
                                    cmd.Parameters.AddWithValue("@AssetId", postTransaction.AssetId);
                                    cmd.CommandTimeout = 30;
                                    cmd.ExecuteNonQuery();
                                }
                                // Add transaction for the main asset
                                common.AddTransaction(con, mainAssetId, userId, null, null, "SPARE_PART_DEACTIVATED", postTransaction.Note, postTransaction.AssetId, null, null);
                            }
                        }
                        if (FindHaveActiveSpareParts(con, postTransaction.AssetId))
                        {
                            // Get the list of active spare parts for this asset
                            List<string> activeSparePartIds = GetActiveSparePartIds(con, postTransaction.AssetId);

                            // Destroy each spare part
                            foreach (string sparePartId in activeSparePartIds)
                            {
                                // Update the Stocks table to set Quantity = 0 for this spare part
                                string destroySparePartQuery = @"
            UPDATE Stocks 
            SET Quantity = 0 
            WHERE AssetId = @SparePartId";

                                using (SqlCommand cmd = new SqlCommand(destroySparePartQuery, con))
                                {
                                    cmd.Parameters.AddWithValue("@SparePartId", sparePartId);
                                    cmd.CommandTimeout = 30;
                                    cmd.ExecuteNonQuery();
                                }

                                // Deactivate the spare part relationship in AssetSpareParts table
                                string deactivateRelationshipQuery = @"
            UPDATE AssetSpareParts 
            SET IsActive = 0 
            WHERE MainAssetId = @MainAssetId AND SparePartId = @SparePartId";

                                using (SqlCommand cmd = new SqlCommand(deactivateRelationshipQuery, con))
                                {
                                    cmd.Parameters.AddWithValue("@MainAssetId", postTransaction.AssetId);
                                    cmd.Parameters.AddWithValue("@SparePartId", sparePartId);
                                    cmd.CommandTimeout = 30;
                                    cmd.ExecuteNonQuery();
                                }

                                // Add transaction for each destroyed spare part
                                common.AddTransaction(con, sparePartId, userId, null, null, "MAIN_ASSET_DEACTIVATED", postTransaction.Note, postTransaction.AssetId, null, null);
                            }
                        }
                        return Ok(new { Message = "Success" });
                    }
                    else
                    {
                        return BadRequest("Asset cannot be marked as lost or stolen because its quantity is already zero.");
                    }
                }
                else if (postTransaction.Type == "ASSET_LOCATION_CHANGED")
                {
                    if (postTransaction.ToId <= 0)
                    {
                        return BadRequest("Please select a location");
                    }
                    ChangeAssetLocation(con, postTransaction.AssetId, postTransaction.ToId);
                    common.AddTransaction(con, postTransaction.AssetId, userId, postTransaction.FromId, postTransaction.ToId, postTransaction.Type, postTransaction.Note, null, null, null);
                    return Ok(new { Message = "Asset moved successfully" });
                }
                else if (postTransaction.Type == "GIVEN_TO_REAPAIR")
                {
                    ActiveRepairStatus(con, postTransaction.AssetId);
                    common.AddTransaction(con, postTransaction.AssetId, userId, null, postTransaction.ToId, postTransaction.Type, postTransaction.Note, null, postTransaction.RepairCost, postTransaction.IsTempAssigned);
                    DeActivateObservation(con, postTransaction.ObservationId);
                    return Ok(new { Message = "Asset sent to repair successfully" });
                }
                else if (postTransaction.Type == "RETURNED_FROM_REPAIR")
                {
                    DeActiveRepairStatus(con, postTransaction.AssetId);
                    common.AddTransaction(con, postTransaction.AssetId, userId, postTransaction.FromId, null, postTransaction.Type, postTransaction.Note, null, postTransaction.RepairCost, postTransaction.IsTempAssigned);
                    return Ok(new { Message = "Asset returned from repair successfully" });
                }
                else if (postTransaction.Type == "STILL_IN_REPAIR")
                {
                    common.AddTransaction(con, postTransaction.AssetId, userId, null, postTransaction.ToId, postTransaction.Type, postTransaction.Note, null, postTransaction.RepairCost, postTransaction.IsTempAssigned);
                    return Ok(new { Message = "Updated successfully" });
                }
                else if (postTransaction.Type == "ASSET_ASSIGNED_TO_ASSET_PART")
                {
                    if (IsActiveWithoutUserAndNotActiveSparePart(con, postTransaction.AssetId))
                    {
                        if (String.IsNullOrWhiteSpace(postTransaction.RelatedAssetId))
                        {
                            return BadRequest("Please select a user to assign the asset");
                        }
                        else
                        {
                            AttachSparePartToAsset(con, postTransaction.AssetId, postTransaction.RelatedAssetId);
                            //adding new record at transaction table for Spart-Part asset 
                            common.AddTransaction(con, postTransaction.AssetId, userId, postTransaction.FromId, null, postTransaction.Type, postTransaction.Note, postTransaction.RelatedAssetId, null);
                            //adding new record at transaction table for Main asset
                            common.AddTransaction(con, postTransaction.RelatedAssetId, userId, null, null, "ASSET_ASSIGNED_TO_ASSET_MAIN", postTransaction.Note, postTransaction.AssetId, null, null);
                            return Ok(new { Message = "Asset assigned to asset successfully" });
                        }
                    }
                    else
                    {
                        return BadRequest("This asset is not available for assignment.");
                    }
                }
                else if (postTransaction.Type == "ASSET_RETURNED_FROM_ASSET_PART")
                {
                    if (PartReturnedFromAsset(con, postTransaction))
                    {
                        //adding new record at transaction table for Spart-Part asset 
                        common.AddTransaction(con, postTransaction.AssetId, userId, postTransaction.FromId, null, postTransaction.Type, postTransaction.Note, postTransaction.RelatedAssetId, null, null);
                        //adding new record at transaction table for Main asset
                        common.AddTransaction(con, postTransaction.RelatedAssetId, userId, null, null, "ASSET_RETURNED_FROM_ASSET_MAIN", postTransaction.Note, postTransaction.AssetId, null, null);
                        return Ok(new { Message = "Asset returned from asset successfully" });
                    }
                    else
                    {
                        return BadRequest("This asset is not assigned as a spare part to the specified asset or is already returned.");
                    }
                }
                else if (postTransaction.Type == "ADD_COMPLAIN")
                {
                    AddComplain(con, postTransaction, userId);
                    return Ok(new { Mesage = "Complain added sucessfully" });
                }
                else if (postTransaction.Type == "USER_LOCATION_CHANGED")
                {
                    if (postTransaction.UserId > 0)
                    {
                        // Get the list of assets the user is currently using
                        List<string> userAssets = GetAssetsAssignedToUserIds(con, postTransaction.UserId);
                        foreach (string assetId in userAssets)
                        {
                            ChangeAssetLocation(con, assetId, postTransaction.ToId);
                            common.AddTransaction(con, assetId, userId, postTransaction.FromId, postTransaction.ToId, postTransaction.Type, postTransaction.Note, null, null, null);
                        }
                        UpdateUserLocation(con, postTransaction.UserId, postTransaction.ToId);
                        return Ok(new { Message = "User and associated assets transferred successfully" });
                    }
                    return BadRequest("user ID missing");
                }
                else if (postTransaction.Type == "USER_RESIGNED")
                {
                    if (postTransaction.UserId <= 0)
                    {
                        return BadRequest("UserId is required and cannot be null, empty, or zero.");
                    }
                    // Get the list of assets the user is currently using
                    List<string> userAssets = GetAssetsAssignedToUserIds(con, postTransaction.UserId);
                    // Deactivate all assets assigned to the user
                    foreach (string assetId in userAssets)
                    {
                        DeactivateAssetForUser(con, assetId, postTransaction.UserId);
                        common.AddTransaction(con, assetId, userId, postTransaction.UserId, postTransaction.ToId, postTransaction.Type, postTransaction.Note, null, null, null);
                    }
                    // Deactivate the user
                    DeactivateUser(con, postTransaction.UserId);
                    return Ok(new { Message = "User resigned successfully and all assets deactivated" });
                }
                else if (postTransaction.Type == "ITOBSERVATION")
                {
                    AddITObservation(con, postTransaction, userId);
                    common.AddTransaction(con, postTransaction.AssetId, userId, null, null, postTransaction.Type, postTransaction.Note, null, null, null);
                    return Ok(new { Message = "IT Observation added successfully" });
                }
                else
                {
                    return BadRequest("Invalid transfer type.");
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during CreateCommon operation", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during CreateCommon operation", userId);
                return InternalServerError(new Exception("Unable to create asset. Please try again later."));
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        [NonAction]
        public void DeActivateObservation(SqlConnection sqlConnection, int? ObservationId)
        {
            if (!ObservationId.HasValue || ObservationId.Value <= 0)
            {
                return; // No action if ObservationId is null or invalid
            }
            string query = @"
        UPDATE ITObservation
        SET ActionTaken = 1
        WHERE ObservationId = @ObservationId";

            using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
            {
                cmd.Parameters.AddWithValue("@ObservationId", ObservationId.Value);
                cmd.CommandTimeout = 30;

                if (sqlConnection.State != System.Data.ConnectionState.Open)
                {
                    sqlConnection.Open();
                }
                cmd.ExecuteNonQuery();
            }
        }

        [NonAction]
        public void AddITObservation(SqlConnection con, PostTransaction postTransaction, int userId)
        {
            // Insert a new record into the ITObservation table
            string insertObservationQuery = @"
        INSERT INTO ITObservation (AssetId, ObservedBy, ObservationNote, IsActive, ActionTaken, ObservationTime)
        VALUES (@AssetId, @ObservedBy, @ObservationNote, 1, 0, GETDATE())";

            using (SqlCommand cmd = new SqlCommand(insertObservationQuery, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", postTransaction.AssetId);
                cmd.Parameters.AddWithValue("@ObservedBy", userId);
                cmd.Parameters.AddWithValue("@ObservationNote", string.IsNullOrWhiteSpace(postTransaction.Note) ? (object)DBNull.Value : postTransaction.Note);
                cmd.ExecuteNonQuery();
            }

            // Update the Complains table to set IsActive = 0 for the relevant complain
            if (postTransaction.ComplainId.HasValue && postTransaction.ComplainId.Value > 0)
            {
                string updateComplainQuery = @"
            UPDATE Complains
            SET IsActive = 0
            WHERE ComplainId = @ComplainId";

                using (SqlCommand cmd = new SqlCommand(updateComplainQuery, con))
                {
                    cmd.Parameters.AddWithValue("@ComplainId", postTransaction.ComplainId.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        [NonAction]
        public void AddComplain(SqlConnection con, PostTransaction transaction, int editedUserId)
        {
            // Step 1: Insert new record into Complains table and get the new ComplainId
            int complainId;
            string insertComplainQuery = @"
        INSERT INTO Complains (UserId, Note, IsActive)
        VALUES (@UserId, @Note, 1);
        SELECT SCOPE_IDENTITY();";

            using (SqlCommand cmd = new SqlCommand(insertComplainQuery, con))
            {
                cmd.Parameters.AddWithValue("@UserId", transaction.UserId);
                cmd.Parameters.AddWithValue("@Note", string.IsNullOrWhiteSpace(transaction.Note) ? (object)DBNull.Value : transaction.Note);
                complainId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            // Step 2: Get all active spare parts for the main asset
            List<string> sparePartIds = GetActiveSparePartIds(con, transaction.AssetId);

            // Step 3: Build full asset list — main asset + spare parts
            List<string> allAssetIds = new List<string> { transaction.AssetId };
            allAssetIds.AddRange(sparePartIds);

            // Step 4: Insert records into AssetComplains and add transactions for each asset
            string insertAssetComplainQuery = @"
        INSERT INTO AssetComplains (ComplainId, AssetId)
        VALUES (@ComplainId, @AssetId)";

            foreach (string assetId in allAssetIds)
            {
                // Insert into AssetComplains
                using (SqlCommand cmd = new SqlCommand(insertAssetComplainQuery, con))
                {
                    cmd.Parameters.AddWithValue("@ComplainId", complainId);
                    cmd.Parameters.AddWithValue("@AssetId", assetId);
                    cmd.ExecuteNonQuery();
                }

                // Add transaction record
                common.AddTransaction(con, assetId, editedUserId, null, null, transaction.Type, transaction.Note, null, null, null);
            }
        }

        [NonAction]
        public void DeactivateAssetForUser(SqlConnection con, string AssetId, int UserId)
        {
            string query = @"
    UPDATE AssetUsedBy
    SET IsActive = 0
    WHERE AssetId = @AssetId AND UsedBy = @UserId";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", AssetId);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.ExecuteNonQuery();
            }
        }

        [NonAction]
        public void DeactivateUser(SqlConnection con, int UserId)
        {
            string query = @"
    UPDATE Users
    SET IsActive = 0
    WHERE UsersId = @UserId";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.ExecuteNonQuery();
            }
        }

        [NonAction]
        public List<string> GetAssetsAssignedToUserIds(SqlConnection con, int UserId)
        {
            List<string> assetIds = new List<string>();

            string query = @"
        SELECT AssetId
        FROM AssetUsedBy
        WHERE UsedBy = @UserId AND IsActive = 1";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.CommandTimeout = 30;

                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        assetIds.Add(reader["AssetId"].ToString());
                    }
                }
            }

            return assetIds;
        }

        [NonAction]
        public void UpdateUserLocation(SqlConnection con, int UserId, int? ToLocationId)
        {
            string query = @"
        UPDATE Users
        SET LocationId = @ToLocationId
        WHERE UsersId = @UserId";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@ToLocationId", ToLocationId);
                cmd.ExecuteNonQuery();
            }
        }

        [NonAction]
        public bool IsUserUsingAnyAsset(SqlConnection con, int UserId)
        {
            string query = @"
        SELECT CASE
            WHEN EXISTS (
                SELECT 1
                FROM AssetUsedBy
                WHERE UsedBy = @UserId AND IsActive = 1
            )
            THEN 1
            ELSE 0
        END AS IsUsingAsset";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.CommandTimeout = 30;
                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                var result = cmd.ExecuteScalar();
                return result != null && Convert.ToInt32(result) == 1;
            }
        }

        [NonAction]
        public List<string> GetActiveSparePartIds(SqlConnection con, string AssetId)
        {
            List<string> sparePartIds = new List<string>();

            string query = @"
        SELECT SparePartId 
        FROM AssetSpareParts 
        WHERE MainAssetId = @AssetId 
        AND IsActive = 1";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", AssetId);
                cmd.CommandTimeout = 30;

                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sparePartIds.Add(reader["SparePartId"].ToString());
                    }
                }
            }
            return sparePartIds;
        }

        //    [NonAction]
        //    public void DeactiveComplainRecord(SqlConnection con, string AssetId)
        //    {
        //        string query = @"
        //IF EXISTS (SELECT 1 FROM Complains WHERE AssetId = @AssetId)
        //BEGIN
        //    UPDATE Complains
        //    SET IsActive = 0
        //    WHERE AssetId = @AssetId
        //END";

        //        using (SqlCommand cmd = new SqlCommand(query, con))
        //        {
        //            cmd.Parameters.AddWithValue("@AssetId", AssetId);
        //            cmd.ExecuteNonQuery();
        //        }
        //    }

        [NonAction]
        public void DeActiveRepairStatus(SqlConnection con, string AssetId)
        {
            string query = @"
    IF EXISTS (SELECT 1 FROM Repairs WHERE AssetId = @AssetId)
    BEGIN
        UPDATE Repairs
        SET RepairStatus = 0
        WHERE AssetId = @AssetId
    END";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", AssetId);
                cmd.ExecuteNonQuery();
            }
        }

        [NonAction]
        public void ActiveRepairStatus(SqlConnection con, string AssetId)
        {
            string query = @"
        IF EXISTS (SELECT 1 FROM Repairs WHERE AssetId = @AssetId)
        BEGIN
            UPDATE Repairs
            SET RepairStatus = 1
            WHERE AssetId = @AssetId
        END
        ELSE
        BEGIN
            INSERT INTO Repairs (AssetId, RepairStatus)
            VALUES (@AssetId, 1)
        END";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", AssetId);
                cmd.ExecuteNonQuery();
            }
        }

        // Deactivates the assignment of a spare part from a main asset.
        // Checks if the spare part is currently assigned and active for the given main asset.
        // If so, sets IsActive to 0 for that assignment.    
        // return True if deactivation was successful, false otherwise returns
        [NonAction]
        public bool PartReturnedFromAsset(SqlConnection con, PostTransaction postTransaction)
        {
            // Check if the spare part is currently assigned and active for the given main asset
            string checkQuery = @"
        SELECT COUNT(*)
        FROM AssetSpareParts
        WHERE MainAssetId = @MainAssetId
          AND SparePartId = @SparePartId
          AND IsActive = 1";

            using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
            {
                checkCmd.Parameters.AddWithValue("@MainAssetId", postTransaction.RelatedAssetId);
                checkCmd.Parameters.AddWithValue("@SparePartId", postTransaction.AssetId);

                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    // If assigned and active, update IsActive to 0
                    string updateQuery = @"
                UPDATE AssetSpareParts
                SET IsActive = 0
                WHERE MainAssetId = @MainAssetId
                  AND SparePartId = @SparePartId";

                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                    {
                        updateCmd.Parameters.AddWithValue("@MainAssetId", postTransaction.RelatedAssetId);
                        updateCmd.Parameters.AddWithValue("@SparePartId", postTransaction.AssetId);
                        updateCmd.ExecuteNonQuery();
                    }
                    return true;
                }
                else
                {
                    // Not assigned or not active
                    return false;
                }
            }
        }

        [NonAction]
        public bool IsThisAssetIdValidAndActive(SqlConnection con, string AssetId)
        {
            string query = @"
    SELECT CASE
        WHEN EXISTS (
            SELECT 1
            FROM Stocks
            WHERE AssetId = @AssetId AND Quantity > 0
        )
        THEN 1
        ELSE 0
    END AS IsValidAndActive";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", AssetId);
                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                var result = cmd.ExecuteScalar();
                return result != null && Convert.ToInt32(result) == 1;
            }
        }

        [NonAction]
        public void AttachSparePartToAsset(SqlConnection con, string SparePartId, string MainItemId)
        {
            string query = @"
    -- Check if the spare part is already assigned to the main item but inactive
    IF EXISTS (
        SELECT 1
        FROM AssetSpareParts
        WHERE MainAssetId = @MainItemId AND SparePartId = @SparePartId AND IsActive = 0
    )
    BEGIN
        -- Reactivate the existing record
        UPDATE AssetSpareParts
        SET IsActive = 1
        WHERE MainAssetId = @MainItemId AND SparePartId = @SparePartId
    END
    ELSE
    BEGIN
        -- Insert a new record
        INSERT INTO AssetSpareParts (MainAssetId, SparePartId, IsActive)
        VALUES (@MainItemId, @SparePartId, 1)
    END";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@MainItemId", MainItemId);
                cmd.Parameters.AddWithValue("@SparePartId", SparePartId);
                cmd.ExecuteNonQuery();
            }
        }

        [NonAction]
        public bool IsActiveWithoutUser(SqlConnection con, string AssetId)
        {
            string query = @"
        SELECT CASE
            WHEN s.Quantity > 0 AND NOT EXISTS (
                SELECT 1
                FROM AssetUsedBy au
                WHERE au.AssetId = @AssetId AND au.IsActive = 1
            )
            THEN 1
            ELSE 0
        END AS IsAvailable
        FROM Stocks s
        WHERE s.AssetId = @AssetId";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", AssetId);
                var result = cmd.ExecuteScalar();
                return result != null && Convert.ToInt32(result) == 1;
            }
        }

        [NonAction]
        public bool IsActiveWithoutUserAndNotActiveSparePart(SqlConnection con, string AssetId)
        {
            string query = @"
        SELECT CASE
            WHEN s.Quantity > 0 
                AND NOT EXISTS (
                    SELECT 1
                    FROM AssetUsedBy au
                    WHERE au.AssetId = @AssetId AND au.IsActive = 1
                )
                AND NOT EXISTS (
                    SELECT 1
                    FROM AssetSpareParts asp
                    WHERE asp.SparePartId = @AssetId AND asp.IsActive = 1
                )
            THEN 1
            ELSE 0
        END AS IsAvailable
        FROM Stocks s
        WHERE s.AssetId = @AssetId";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", AssetId);
                var result = cmd.ExecuteScalar();
                return result != null && Convert.ToInt32(result) == 1;
            }
        }

        [NonAction]
        public void ChangeAssetLocation(SqlConnection con, string AssetId, int? ToLocationId)
        {
            string query = @"
    UPDATE Stocks
    SET LocationId = @ToLocationId
    WHERE AssetId = @AssetId";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", AssetId);
                cmd.Parameters.AddWithValue("@ToLocationId", ToLocationId);
                cmd.ExecuteNonQuery();
            }
        }

        [NonAction]
        public bool AssetDestroyed(SqlConnection con, string AssetId)
        {
            // Check if the asset's Quantity is greater than 0
            string checkQuery = @"
        SELECT Quantity
        FROM Stocks
        WHERE AssetId = @AssetId";

            using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
            {
                checkCmd.Parameters.AddWithValue("@AssetId", AssetId);
                var quantity = checkCmd.ExecuteScalar();

                if (quantity != null && Convert.ToInt32(quantity) > 0)
                {
                    // If Quantity > 0, update it to 0
                    string updateQuery = @"
                UPDATE Stocks
                SET Quantity = 0
                WHERE AssetId = @AssetId";

                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                    {
                        updateCmd.Parameters.AddWithValue("@AssetId", AssetId);
                        updateCmd.ExecuteNonQuery();
                    }
                    return true;
                }
                else
                {
                    // Quantity is not greater than 0
                    return false;
                }
            }
        }

        [NonAction]
        public bool AssetReturnedFromUser(SqlConnection con, string AssetId, int? UsedBy)
        {
            // First, check if the asset is assigned to the user and is active
            string checkQuery = @"
        SELECT COUNT(*)
        FROM AssetUsedBy
        WHERE AssetId = @AssetId AND UsedBy = @UsedBy AND IsActive = 1";

            using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
            {
                checkCmd.Parameters.AddWithValue("@AssetId", AssetId);
                checkCmd.Parameters.AddWithValue("@UsedBy", UsedBy);

                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    // If assigned and active, update IsActive to 0
                    string updateQuery = @"
                UPDATE AssetUsedBy
                SET IsActive = 0
                WHERE AssetId = @AssetId AND UsedBy = @UsedBy";

                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                    {
                        updateCmd.Parameters.AddWithValue("@AssetId", AssetId);
                        updateCmd.Parameters.AddWithValue("@UsedBy", UsedBy);
                        updateCmd.ExecuteNonQuery();
                    }
                    return true;
                }
                else
                {
                    // Not assigned or not active
                    return false;
                }
            }
        }

        [NonAction]
        public void AssetAssignedToUser(SqlConnection con, string AssetId, int? UsedBy)
        {
            string query = @"
IF EXISTS (SELECT 1 FROM AssetUsedBy WHERE AssetId = @AssetId AND UsedBy = @UsedBy)
BEGIN
    UPDATE AssetUsedBy 
    SET IsActive = 1 
    WHERE AssetId = @AssetId AND UsedBy = @UsedBy
END
ELSE
BEGIN
    INSERT INTO AssetUsedBy (AssetId, UsedBy, IsActive)
    VALUES (@AssetId, @UsedBy, 1)
END";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", AssetId);
                cmd.Parameters.AddWithValue("@UsedBy", UsedBy);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public class ItObservationTracker
    {
        public bool HasActiveItObservations { get; set; }
        public List<ItObservation> ActiveItObservations { get; set; }
    }

    public class ItObservation
    {
        public int ObservationId { get; set; }
        public int? ObservedBy { get; set; }
        public string ObservedByName { get; set; }
        public string ObservationNote { get; set; }
        public bool ActionTaken { get; set; }
        public System.DateTime ObservationTime { get; set; }
    }

    public class MoveAssetToLocation
    {
        public string AssetName { get; set; }
        public bool? HasExistingUser { get; set; }
        public int? ExistingLocationId { get; set; }
        public int? ExitingCompanyId { get; set; }
        public string ExistingLocationName { get; set; }
        public string ExistingCompanyName { get; set; }
        public List<NextLocations> NextLocations { get; set; }
    }

    public class NextLocations
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; }
    }

    public class UnAssignedAssets
    {
        public string AssetId { get; set; }
        public bool? HasExistingUser { get; set; }
        public string AssetName { get; set; }// These will be ignored if null
        public int? LocationId { get; set; }// These will be ignored if null
        public string LocationName { get; set; }// These will be ignored if null
        public int? UserId { get; set; }// These will be ignored if null
        public string UserName { get; set; }// These will be ignored if null
    }

    public class RemoveSpareParts
    {
        public string AssetName { get; set; }
        public string LocationName { get; set; }
        public List<UnAssignedAssets> currentSparePartsList { get; set; }
    }

    public class GetTransaction
    {
        public GetListType? Type { get; set; } //not null
        public string AssetId { get; set; } //nullable
        public int UserId { get; set; } //nullable
        public int ComplainId { get; set; } //nullable
    }

    public class UserInfor
    {
        public string UserName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
    }

    public class TransactionPage
    {
        public string AssetName { get; set; }
        public bool? HasOngoingRepair { get; set; }
        public bool HasExistingUser { get; set; }
        // if any given asset have currently existing user assigned actively then this true else false 
        public bool? IsActiveSparePart { get; set; }
        // Indicates whether the asset is currently spare part of any other main asset, actively.
        public bool? IsActiveAsset { get; set; }
        public bool? HaveActiveSpareParts { get; set; }
        // does this aseet have any active spare parts assigned to it
        public int? FromId { get; set; }
        // The ID of the last place (either UserId or LocationId) this asset was assigned to.
        // If the asset is currently assigned to a user, this is the UserId; otherwise, it is the LocationId.
        public string FromName { get; set; }
        // The ID of the last place (either UserId or LocationId) this asset was assigned to.
        // If the asset is currently assigned to a user, this is the UserId; otherwise, it is the LocationId.
        public string AssociateAssetId { get; set; } //if asset is an active spare part of any other asset then this main assetId would sent using this parameter when requried
    }

    public class AssignToAsset
    {
        public string AssetId { get; set; }
        public string AssetName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public List<UnAssignedAssets> AvailableAssets { get; set; }
    }

    public class AssetStatus
    {
        public bool IsActive { get; set; }
    }

    public class ReturnedFromRepair
    {
        public string AssetName { get; set; }
        public int? SupplierId { get; set; }
        public string SupplierName { get; set; }
        public decimal? Cost { get; set; }
        public bool? IsTempAssigned { get; set; }
    }

    public class ComplainInfo
    {
        public int ComplainId { get; set; }
        public string UserName { get; set; }
        public string Note { get; set; }
        public System.DateTime CreatedAt { get; set; }
    }

    public enum GetListType
    {
        get_list_of_softwares,
        get_asset_name_by_id,
        get_list_of_assets_under_complain,
        are_their_any_active_itobservations,
        get_list_of_active_complaints,
        get_related_users,
        user_transfer_page_data,
        is_eligible_to_location_transfer,
        is_active_spare_part,
        are_their_any_active_complaints,

        //asset-id requeird
        //return all the attached spare-parts of the given asset
        //this used to display all the attached spare-parts list at ViewInfo page
        get_list_of_parts,

        // Returns a list of all available (unused) laptops, desktops, or servers in the inventory.
        // These assets are not currently assigned to any user and have a stock quantity greater than zero.
        // The result includes the asset's ID, name, and location ID.
        asset_list,

        // Returns a list of assets (laptops, desktops, or servers) that are currently assigned to a specific user.
        // The result includes the asset's ID, name, and location ID.
        // Requires a valid UserId to be provided in the request.
        asset_list_belong_to_user,

        // Returns the current location and company of a specific asset, along with a list of all other available locations (excluding the current one) where the asset can be moved.
        // Requires a valid AssetId to be provided in the request.
        // sent asset-name too
        move_asset_to_location,

        // Returns the name and location details (ID and name) of a specific asset.
        // This is typically used to confirm or display asset details before assigning it to a user.
        // Requires a valid AssetId to be provided in the request. 
        assign_to_user,

        // Retuns the assetId, asset's location name with locationId, current user's name with user's id
        // this type used to display asset's existing user and stock location before returning it from user
        //requires a valid AssetId 
        return_from_user,

        // this tell does this asset currently have existing user assigned or not and check this asset is active or not
        //this type used to populater asset-assign-to-user-form,asset-return-from-user-form and asset-give-to-repair-forms at transaction-page
        //requires a valid AssetId
        transaction_page,

        //this return the given asset's AssetId, AssetName, locationId, locationName, CompanyId, CompanyName & other asset-list (with stock.qty>0) except current asset 
        //this used to populate asset's details & available asset list at asset-assign-to-asset-form page
        //requires a valid AssetId
        assign_to_asset,

        //this tell weather this asset have a user assigned or not,if so return userId and userName else locationId and locationName with the asset's name
        //this used at both asset-destrory form and asset-lost-stolen forms
        //requires a valid AssetId
        asset_destroyed_lost_stolen,

        //this tell weather this asset is active or not
        //used to control AddSoftware page's 'AddSoftwares' button displaying as asset's status 
        //requires a valid AssetId
        is_asset_active,

        //this get request type used to get all requried information to remove spare-part form including current asset's details and attached assests list (spare-parts)
        //requires a valid AssetId
        remove_spare_parts,

        //asset-id is requeired
        //this is used to get necessary information to process 'returned-from-repair' form at transaction-page
        returned_from_repair
    }
}