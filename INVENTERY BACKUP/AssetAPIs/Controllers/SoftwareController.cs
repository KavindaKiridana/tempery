using AssetAPIs.Filters;
using AssetAPIs.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Web.Http;

namespace AssetAPIs.Controllers
{
    [JwtAuthentication]
    public class SoftwareController : ApiController
    {
        private readonly Comman common = new Comman();

        // PATCH: api/Software
        [HttpPatch]
        [Route("api/Software")]
        public IHttpActionResult UpdateSoftware(Softwares software)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                // Input validation
                if (software == null || software.Id <= 0)
                {
                    common.LogError(new ArgumentNullException("software"), "UpdateSoftware - Invalid Software data", userId);
                    return BadRequest("Invalid Software data");
                }

                if (string.IsNullOrWhiteSpace(software.Name))
                {
                    common.LogError(new ArgumentException("Software Name is null or empty"), "UpdateSoftware - Invalid Software Name", userId);
                    return BadRequest("Software Name is required and cannot be empty");
                }

                // Trim and validate Software name length
                software.Name = software.Name.Trim();
                if (software.Name.Length > 255)
                {
                    return BadRequest("Software Name is too long. Maximum 255 characters allowed.");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "UpdateSoftware - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = @"
            UPDATE Software
            SET SoftwareName = @SoftwareName,
                IsActive = @IsActive
            WHERE SoftwareId = @SoftwareId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@SoftwareName", software.Name);
                cmd.Parameters.AddWithValue("@IsActive", software.IsActive);
                cmd.Parameters.AddWithValue("@SoftwareId", software.Id);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "Software updated successfully" });
                }
                else
                {
                    common.LogError(new Exception("No rows affected during update"), "UpdateSoftware - Update failed", userId);
                    return InternalServerError(new Exception("Failed to update Software. Please try again."));
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, $"SQL Error during UpdateSoftware operation - Software ID: {software?.Id}", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (ArgumentException argEx)
            {
                common.LogError(argEx, "Argument validation error during UpdateSoftware operation", userId);
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"Unexpected error during UpdateSoftware operation - Software ID: {software?.Id}", userId);
                return InternalServerError(new Exception("Unable to update Software. Please try again later."));
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        //id means AssetId
        //if the incoming API came without id, then return all softwares list weather they installed on any asset or not
        //if the incoming API came with id,then return all the active softwares list from InstalledSoftwares table for that asset with it's installedStatus
        [HttpGet]
        [Route("api/Software/{id?}")]
        public IHttpActionResult GetSoftwaresList(string id = null)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    return InternalServerError(new Exception("Database configuration error: Connection string is missing."));
                }

                con = new SqlConnection(connectionString);
                con.Open();

                if (string.IsNullOrEmpty(id))
                {
                    // If no id is provided, return all active softwares
                    List<Softwares> softwareList = GetAllSoftwares(con);
                    return Ok(softwareList);
                }
                else
                {
                    // If id is provided, return installed softwares for that asset
                    List<InstallesSoftwares> installedSoftwares = GetInstalledSoftwares(con, id);
                    return Ok(installedSoftwares);
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "Database error while fatching software", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (InvalidCastException castEx)
            {
                common.LogError(castEx, "Data conversion error during fatch softwares", userId);
                return InternalServerError(new Exception("Data format error. Please contact support."));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during fatching softwares", userId);
                return InternalServerError(new Exception("Unable to retrieve supplier list. Please try again later."));
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
        public List<InstallesSoftwares> GetInstalledSoftwares(SqlConnection con, string assetId)
        {
            List<InstallesSoftwares> installedSoftwares = new List<InstallesSoftwares>();

            string query = @"
  SELECT 
      ins.InstalledSoftware,
      s.SoftwareName,
      ins.IsActive as IsInstalled
  FROM InstalledSoftwares ins
  INNER JOIN Software s ON ins.SoftwareId = s.SoftwareId
  WHERE ins.AssetId = @AssetId
  AND s.IsActive = 1
  ORDER BY s.SoftwareName;";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@AssetId", assetId);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                installedSoftwares.Add(new InstallesSoftwares
                {
                    InstalledSoftwareId = Convert.ToInt32(reader["InstalledSoftware"]),
                    SoftwareName = reader.GetString(1),
                    InstalledStatus = reader.GetBoolean(2)
                });
            }

            reader.Close();
            return installedSoftwares;
        }

        [NonAction]
        public List<Softwares> GetAllSoftwares(SqlConnection con)
        {
            List<Softwares> softwareList = new List<Softwares>();
            string query = @"SELECT SoftwareId, SoftwareName, IsActive,
CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM InstalledSoftwares 
            WHERE InstalledSoftwares.SoftwareId = Software.SoftwareId and InstalledSoftwares.IsActive = 1
        ) 
        THEN 1 
        ELSE 0 
    END AS IsUsed FROM Software WHERE IsActive = 1 ORDER BY SoftwareName";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                softwareList.Add(new Softwares
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    IsActive = reader.GetBoolean(2),
                    IsUsed = Convert.ToBoolean(reader.GetInt32(3))
                });
            }

            reader.Close();
            return softwareList;
        }

        // API to add new software
        [HttpPost]
        [Route("api/Software")]
        public IHttpActionResult AddSoftware(Softwares softwares)
        {
            if (softwares.Name == null || softwares.Name.Trim() == "")
            {
                return BadRequest("Software name cannot be empty.");
            }
            if (softwares.Name.Length > 255)
            {
                return BadRequest("Software name cannot exceed 255 characters.");
            }
            int userId = common.GetUserId((ClaimsPrincipal)User);
            SqlConnection con = null;
            string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                common.LogError(new Exception("Connection string is null or empty"), "AddDepartment - Configuration Error", userId);
                return InternalServerError(new Exception("Database configuration error"));
            }
            try
            {
                con = new SqlConnection(connectionString);
                string query = "INSERT INTO Software (SoftwareName,AddedUser,AddedTime) VALUES (@softwarrname,@AddedUser,@AddedTime); SELECT SCOPE_IDENTITY();";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@softwarrname", softwares.Name);
                cmd.Parameters.AddWithValue("@AddedUser", userId);
                cmd.Parameters.AddWithValue("@AddedTime", DateTime.Now);
                con.Open();
                int softwareId = Convert.ToInt32(cmd.ExecuteScalar());
                AddInstalledSoftware(con, softwareId);
                return Ok(new { message = "Software added successfully" });
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "Database error while adding software", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error while adding software", userId);
                return InternalServerError(new Exception("Unable to add software. Please try again later."));
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
        public void AddInstalledSoftware(SqlConnection con, int softwareId)
        {
            string query = @"
SELECT AssetId
FROM Asset
WHERE Type IN ('Laptop', 'Desktop','Server')";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                SqlDataReader reader = cmd.ExecuteReader();
                List<string> assetIds = new List<string>();
                while (reader.Read())
                {
                    assetIds.Add(reader.GetString(0));
                }
                reader.Close();

                foreach (string assetId in assetIds)
                {
                    string insertQuery = @"
IF NOT EXISTS (SELECT 1 FROM InstalledSoftwares WHERE AssetId = @AssetId AND SoftwareId = @SoftwareId)
BEGIN
    INSERT INTO InstalledSoftwares (AssetId, SoftwareId, IsActive)
    VALUES (@AssetId, @SoftwareId, 0)
END";
                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, con))
                    {
                        insertCmd.Parameters.AddWithValue("@AssetId", assetId);
                        insertCmd.Parameters.AddWithValue("@SoftwareId", softwareId);
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
