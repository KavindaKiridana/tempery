using AssetAPIs.Filters;
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
    public class AssetController : ApiController
    {
        private readonly Comman common = new Comman();

        [HttpPost]
        public IHttpActionResult CreateCommon(Models.Asset asset)
        {
            int userId = common.GetUserId((ClaimsPrincipal)User);

            // Validation
            if (asset.CompanyId == 0 || asset.CompanyId == null)
            {
                return BadRequest("Please select a company");
            }
            if (asset.LocationId == 0 || asset.LocationId == null)
            {
                return BadRequest("Please select a location");
            }
            if (asset.SupplierId == 0 || asset.SupplierId == null)
            {
                return BadRequest("Please select a supplier");
            }
            // Validate asset.Quantity
            if (asset.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than 0");
            }
            if (asset.Quantity % 1 != 0)
            {
                return BadRequest("Quantity must be a whole number, not a decimal");
            }

            SqlConnection con = null;
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "CreateCommon - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                con.Open();

                // Parse ManufactureSN into array
                string[] manufactureSNs = ParseManufactureSNs(asset.ManufactureSN, asset.Quantity);

                // Validate quantity matches serial numbers
                if (manufactureSNs.Length != asset.Quantity)
                {
                    return BadRequest($"Number of serial numbers ({manufactureSNs.Length}) does not match quantity ({asset.Quantity})");
                }

                List<string> createdAssetIds = new List<string>();

                // Create assets based on quantity
                for (int i = 0; i < asset.Quantity; i++)
                {
                    // Generate unique AssetId for each asset
                    string newAssetId = GenerateNextAssetId(con);

                    string query = @"
                    INSERT INTO Asset (
                    AssetId, SupplierId, EditedUser, 
                    OsId, PId, RAMSId, RAMTId, HDDId, SSDId, DisplayId,
                    DoP, FinanceAssetCode, Warranty, Type, ManufactureSN, Brandnew, Cost,
                    Name, IPAddress, Make, WindowsKey, Motherboard, 
                    PowerSupply, RAIDSupport, Model, Note
                )
                VALUES (
                    @AssetId, @SupplierId, @EditedUser,
                    @OsId, @PId, @RAMSId, @RAMTId, @HDDId, @SSDId, @DisplayId,
                    @DoP, @FinanceAssetCode, @Warranty, @Type, @ManufactureSN, @Brandnew, @Cost,
                    @Name, @IPAddress, @Make, @WindowsKey, @Motherboard,
                    @PowerSupply, @RAIDSupport, @Model, @Note
                );";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@AssetId", newAssetId);
                    cmd.Parameters.AddWithValue("@SupplierId", (asset.SupplierId == 0 || asset.SupplierId == null) ? (object)DBNull.Value : asset.SupplierId);
                    cmd.Parameters.AddWithValue("@EditedUser", userId);
                    cmd.Parameters.AddWithValue("@OsId", (asset.OsId == 0 || asset.OsId == null) ? (object)DBNull.Value : asset.OsId);
                    cmd.Parameters.AddWithValue("@PId", (asset.PId == 0 || asset.PId == null) ? (object)DBNull.Value : asset.PId);
                    cmd.Parameters.AddWithValue("@RAMSId", (asset.RAMSId == 0 || asset.RAMSId == null) ? (object)DBNull.Value : asset.RAMSId);
                    cmd.Parameters.AddWithValue("@RAMTId", (asset.RAMTId == 0 || asset.RAMTId == null) ? (object)DBNull.Value : asset.RAMTId);
                    cmd.Parameters.AddWithValue("@HDDId", (asset.HDDId == 0 || asset.HDDId == null) ? (object)DBNull.Value : asset.HDDId);
                    cmd.Parameters.AddWithValue("@SSDId", (asset.SSDId == 0 || asset.SSDId == null) ? (object)DBNull.Value : asset.SSDId);
                    cmd.Parameters.AddWithValue("@DisplayId", (asset.DisplayId == 0 || asset.DisplayId == null) ? (object)DBNull.Value : asset.DisplayId);
                    cmd.Parameters.AddWithValue("@DoP", asset.DoP ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FinanceAssetCode", asset.FinanceAssetCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Warranty", asset.Warranty ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Type", asset.Type.ToString());
                    // Assign individual serial number for this asset
                    cmd.Parameters.AddWithValue("@ManufactureSN", manufactureSNs[i]);
                    cmd.Parameters.AddWithValue("@Brandnew", asset.Brandnew ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Cost", asset.Cost ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Name", asset.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IPAddress", asset.IPAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Make", asset.Make ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@WindowsKey", asset.WindowsKey ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Motherboard", asset.Motherboard ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PowerSupply", asset.PowerSupply ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RAIDSupport", asset.RAIDSupport ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Model", asset.ModelId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Note", asset.Note ?? (object)DBNull.Value);

                    cmd.ExecuteNonQuery();

                    AddStock(con, newAssetId, int.Parse(asset.CompanyId.ToString()), int.Parse(asset.LocationId.ToString()), 1);
                    common.AddTransaction(con, newAssetId, userId, asset.SupplierId, asset.LocationId, "ADD_NEW_ASSET_TO_STORE", asset.Note,null, null, null);

                    if (asset.Type == "Desktop" || asset.Type == "Laptop" || asset.Type == "Server")
                    {
                        AddSoftware(con, newAssetId);
                    }

                    createdAssetIds.Add(newAssetId);
                }

                return Ok(new { AssetIds = createdAssetIds, Count = createdAssetIds.Count });
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
        private string[] ParseManufactureSNs(string manufactureSN, int quantity)
        {
            if (string.IsNullOrWhiteSpace(manufactureSN))
            {
                // Return empty strings if no serial numbers provided
                return new string[quantity];
            }

            // Split by comma and trim whitespace
            string[] sns = manufactureSN.Split(new[] { ',' }, StringSplitOptions.None)
                                        .Select(s => s.Trim())
                                        .ToArray();

            return sns;
        }

        [HttpPatch]
        [Route("api/Asset/{assetId}")]
        public IHttpActionResult UpdateAsset(string assetId, Models.Asset asset)
        {
            int userId = common.GetUserId((ClaimsPrincipal)User);
            SqlConnection con = null;
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "UpdateDesktop - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }
                con = new SqlConnection(connectionString);
                con.Open();
                string query = @"
                UPDATE Asset SET
                OsId = @OsId,
                PId = @PId,
                RAMSId = @RAMSId,
                RAMTId = @RAMTId,
                HDDId = @HDDId,
                SSDId = @SSDId,
                DisplayId = @DisplayId,
                Make = @Make,
                WindowsKey = @WindowsKey,
                Motherboard = @Motherboard,
                Model = @Model,
                PowerSupply =@PowerSupply,
                RAIDSupport =@RAIDSupport
                WHERE AssetId = @AssetId;";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@AssetId", assetId);
                cmd.Parameters.AddWithValue("@OsId", (asset.OsId == 0 || asset.OsId == null) ? (object)DBNull.Value : asset.OsId);
                cmd.Parameters.AddWithValue("@PId", (asset.PId == 0 || asset.PId == null) ? (object)DBNull.Value : asset.PId);
                cmd.Parameters.AddWithValue("@RAMSId", (asset.RAMSId == 0 || asset.RAMSId == null) ? (object)DBNull.Value : asset.RAMSId);
                cmd.Parameters.AddWithValue("@RAMTId", (asset.RAMTId == 0 || asset.RAMTId == null) ? (object)DBNull.Value : asset.RAMTId);
                cmd.Parameters.AddWithValue("@HDDId", (asset.HDDId == 0 || asset.HDDId == null) ? (object)DBNull.Value : asset.HDDId);
                cmd.Parameters.AddWithValue("@SSDId", (asset.SSDId == 0 || asset.SSDId == null) ? (object)DBNull.Value : asset.SSDId);
                cmd.Parameters.AddWithValue("@DisplayId", (asset.DisplayId == 0 || asset.DisplayId == null) ? (object)DBNull.Value : asset.DisplayId);
                cmd.Parameters.AddWithValue("@Make", asset.Make ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@WindowsKey", asset.WindowsKey ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Motherboard", asset.Motherboard ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Model", (asset.ModelId == 0 || asset.ModelId == null) ? (object)DBNull.Value : asset.ModelId);
                cmd.Parameters.AddWithValue("@PowerSupply", asset.PowerSupply ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@RAIDSupport", asset.RAIDSupport ?? (object)DBNull.Value);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    return NotFound();
                }
                return Ok(new { AssetId = assetId });
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during UpdateDesktop operation", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during UpdateDesktop operation", userId);
                return InternalServerError(new Exception("Unable to update asset. Please try again later."));
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
        private void AddSoftware(SqlConnection con, string AssetId)
        {
            string query = "SELECT SoftwareId FROM Software WHERE IsActive = 1";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                SqlDataReader reader = cmd.ExecuteReader();
                List<int> softwareIds = new List<int>();
                while (reader.Read())
                {
                    softwareIds.Add(reader.GetInt32(0));
                }
                reader.Close();

                foreach (int softwareId in softwareIds)
                {
                    string insertQuery = @"
IF NOT EXISTS (SELECT 1 FROM InstalledSoftwares WHERE AssetId = @AssetId AND SoftwareId = @SoftwareId)
BEGIN
    INSERT INTO InstalledSoftwares (AssetId, SoftwareId, IsActive)
    VALUES (@AssetId, @SoftwareId, 0)
END";
                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, con))
                    {
                        insertCmd.Parameters.AddWithValue("@AssetId", AssetId);
                        insertCmd.Parameters.AddWithValue("@SoftwareId", softwareId);
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        [NonAction]
        private void AddStock(SqlConnection con, string AssetId, int companyId, int locationId, int Qty)
        {
            string query = @"
        INSERT INTO Stocks (AssetId,CompanyId, LocationId, Quantity)
        VALUES (@AssetId,@CompanyId, @LocationId, @Quantity);";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", AssetId);
                cmd.Parameters.AddWithValue("@CompanyId", companyId);
                cmd.Parameters.AddWithValue("@LocationId", locationId);
                cmd.Parameters.AddWithValue("@Quantity", Qty);
                cmd.ExecuteNonQuery();
            }
        }

        [NonAction]
        private string GenerateNextAssetId(SqlConnection con)
        {
            string query = "SELECT TOP 1 AssetId FROM Asset ORDER BY AssetId DESC";
            SqlCommand cmd = new SqlCommand(query, con);

            object result = cmd.ExecuteScalar();

            if (result == null || result == DBNull.Value)
            {
                // First asset
                return "RGL00001";
            }

            string lastAssetId = result.ToString();
            // Extract the numeric part (remove "RGL" prefix)
            string numericPart = lastAssetId.Substring(3);
            int lastNumber = int.Parse(numericPart);
            int nextNumber = lastNumber + 1;

            // Format with leading zeros (5 digits)
            return "RGL" + nextNumber.ToString("D5");
        }
    }
}