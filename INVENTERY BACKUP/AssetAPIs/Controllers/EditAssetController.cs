using AssetAPIs.Filters;
using AssetAPIs.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;

namespace AssetAPIs.Controllers
{
    [JwtAuthentication]
    public class EditAssetController : ApiController
    {
        private readonly Comman common = new Comman();

        [HttpPatch]
        public IHttpActionResult EditAsset([FromBody] Asset asset)
        {
            int userId = common.GetUserId((ClaimsPrincipal)User);
            SqlConnection con = null;
            if (asset.AssetId == null || string.IsNullOrEmpty(asset.AssetId))
            {
                return BadRequest("AssetId required");
            }

            string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                common.LogError(new Exception("Connection string is null or empty"), "GetTransferData - Configuration Error");
                return InternalServerError(new Exception("Database configuration error"));
            }
            con = new SqlConnection(connectionString);

            if (asset.PatchRequestType == null || string.IsNullOrEmpty(asset.PatchRequestType))
            {
                return BadRequest("Patch request type is required");
            }
            con.Open();
            try
            {
                if (asset.PatchRequestType == "common")
                {
                    UpdateCommonData(con, asset);
                    return Ok();
                }
                else if (asset.PatchRequestType == "computer")
                {
                    UpdateComputerData(con, asset);
                    return Ok();
                }
                else
                {
                    return BadRequest("Invalid patch request type");
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
        public void UpdateCommonData(SqlConnection con, Asset asset)
        {
            int userId = common.GetUserId((ClaimsPrincipal)User);

            // Update Asset table
            string updateAssetQuery = @"
        UPDATE Asset 
        SET Type = @Type,
            DoP = @DoP,
            FinanceAssetCode = @FinanceAssetCode,
            Warranty = @Warranty,
            ManufactureSN = @ManufactureSN,
            Brandnew = @Brandnew,
            Cost = @Cost,
            Name = @Name,
            SupplierId = @SupplierId,
            IPAddress = @IPAddress,
            Note = @Note,
            EditedUser = @EditedUser
        WHERE AssetId = @AssetId";

            using (SqlCommand cmd = new SqlCommand(updateAssetQuery, con))
            {
                cmd.Parameters.AddWithValue("@Type", asset.Type ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DoP", asset.DoP ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FinanceAssetCode", asset.FinanceAssetCode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Warranty", asset.Warranty ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ManufactureSN", asset.ManufactureSN ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Brandnew", asset.Brandnew ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Cost", asset.Cost ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Name", asset.Name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SupplierId", asset.SupplierId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@IPAddress", asset.IPAddress ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Note", asset.Note ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EditedUser", userId);
                cmd.Parameters.AddWithValue("@AssetId", asset.AssetId);

                cmd.ExecuteNonQuery();
            }

            // Update Stocks table
            string updateStocksQuery = @"
        UPDATE Stocks 
        SET CompanyId = @CompanyId,
            LocationId = @LocationId
        WHERE AssetId = @AssetId";

            using (SqlCommand cmd = new SqlCommand(updateStocksQuery, con))
            {
                cmd.Parameters.AddWithValue("@CompanyId", asset.CompanyId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LocationId", asset.LocationId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@AssetId", asset.AssetId);

                cmd.ExecuteNonQuery();
            }
        }

        [NonAction]
        public void UpdateComputerData(SqlConnection con, Asset asset)
        {
            int userId = common.GetUserId((ClaimsPrincipal)User);

            string updateQuery = "";

            // Build query based on asset type
            if (asset.Type == "Laptop")
            {
                // Laptops: Update all except DisplayId, PowerSupply, RAIDSupport
                updateQuery = @"
            UPDATE Asset 
            SET OsId = @OsId,
                PId = @PId,
                RAMSId = @RAMSId,
                RAMTId = @RAMTId,
                HDDId = @HDDId,
                SSDId = @SSDId,
                Make = @Make,
                WindowsKey = @WindowsKey,
                Motherboard = @Motherboard,
                Model = @Model,
                EditedUser = @EditedUser
            WHERE AssetId = @AssetId";
            }
            else if (asset.Type == "Desktop")
            {
                // Desktops: Update all except PowerSupply, RAIDSupport
                updateQuery = @"
            UPDATE Asset 
            SET OsId = @OsId,
                PId = @PId,
                RAMSId = @RAMSId,
                RAMTId = @RAMTId,
                HDDId = @HDDId,
                SSDId = @SSDId,
                Make = @Make,
                WindowsKey = @WindowsKey,
                Motherboard = @Motherboard,
                Model = @Model,
                DisplayId = @DisplayId,
                EditedUser = @EditedUser
            WHERE AssetId = @AssetId";
            }
            else if (asset.Type == "Server")
            {
                // Servers: Update all except DisplayId
                updateQuery = @"
            UPDATE Asset 
            SET OsId = @OsId,
                PId = @PId,
                RAMSId = @RAMSId,
                RAMTId = @RAMTId,
                HDDId = @HDDId,
                SSDId = @SSDId,
                Make = @Make,
                WindowsKey = @WindowsKey,
                Motherboard = @Motherboard,
                Model = @Model,
                PowerSupply = @PowerSupply,
                RAIDSupport = @RAIDSupport,
                EditedUser = @EditedUser
            WHERE AssetId = @AssetId";
            }
            else
            {
                // For other types (if any), update all fields
                updateQuery = @"
            UPDATE Asset 
            SET OsId = @OsId,
                PId = @PId,
                RAMSId = @RAMSId,
                RAMTId = @RAMTId,
                HDDId = @HDDId,
                SSDId = @SSDId,
                Make = @Make,
                WindowsKey = @WindowsKey,
                Motherboard = @Motherboard,
                Model = @Model,
                DisplayId = @DisplayId,
                PowerSupply = @PowerSupply,
                RAIDSupport = @RAIDSupport,
                EditedUser = @EditedUser
            WHERE AssetId = @AssetId";
            }

            using (SqlCommand cmd = new SqlCommand(updateQuery, con))
            {
                // Add common parameters
                cmd.Parameters.AddWithValue("@OsId", asset.OsId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PId", asset.PId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@RAMSId", asset.RAMSId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@RAMTId", asset.RAMTId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@HDDId", asset.HDDId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SSDId", asset.SSDId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Make", asset.Make ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@WindowsKey", asset.WindowsKey ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Motherboard", asset.Motherboard ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Model", asset.ModelId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EditedUser", userId);
                cmd.Parameters.AddWithValue("@AssetId", asset.AssetId);

                // Add type-specific parameters
                if (asset.Type == "Desktop" || (asset.Type != "Laptop" && asset.Type != "Server"))
                {
                    cmd.Parameters.AddWithValue("@DisplayId", asset.DisplayId ?? (object)DBNull.Value);
                }

                if (asset.Type == "Server" || (asset.Type != "Laptop" && asset.Type != "Desktop"))
                {
                    cmd.Parameters.AddWithValue("@PowerSupply", asset.PowerSupply ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RAIDSupport", asset.RAIDSupport ?? (object)DBNull.Value);
                }

                cmd.ExecuteNonQuery();
            }
        }
    }
}