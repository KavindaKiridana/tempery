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
    public class RAMSizeController : ApiController
    {
        private readonly Comman common = new Comman();

        // PATCH: api/RAMSize
        [HttpPatch]
        public IHttpActionResult UpdateRAMSize(RAMSize ramSize)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                // Input validation
                if (ramSize == null || ramSize.Id <= 0)
                {
                    common.LogError(new ArgumentNullException("ramSize"), "UpdateRAMSize - Invalid RAM Size data", userId);
                    return BadRequest("Invalid RAM Size data");
                }

                if (string.IsNullOrWhiteSpace(ramSize.Name))
                {
                    common.LogError(new ArgumentException("RAM Size is null or empty"), "UpdateRAMSize - Invalid RAM Size", userId);
                    return BadRequest("RAM Size is required and cannot be empty");
                }

                // Trim and validate RAM Size length
                ramSize.Name = ramSize.Name.Trim();
                if (ramSize.Name.Length > 255)
                {
                    return BadRequest("RAM Size is too long. Maximum 255 characters allowed.");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "UpdateRAMSize - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = @"
            UPDATE RAMSize
            SET Size = @Size,
                IsActive = @IsActive
            WHERE RAMSId = @RAMSId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@Size", ramSize.Name);
                cmd.Parameters.AddWithValue("@IsActive", ramSize.IsActive);
                cmd.Parameters.AddWithValue("@RAMSId", ramSize.Id);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "RAM Size updated successfully" });
                }
                else
                {
                    common.LogError(new Exception("No rows affected during update"), "UpdateRAMSize - Update failed", userId);
                    return InternalServerError(new Exception("Failed to update RAM Size. Please try again."));
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, $"SQL Error during UpdateRAMSize operation - RAM Size ID: {ramSize?.Id}", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (ArgumentException argEx)
            {
                common.LogError(argEx, "Argument validation error during UpdateRAMSize operation", userId);
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"Unexpected error during UpdateRAMSize operation - RAM Size ID: {ramSize?.Id}", userId);
                return InternalServerError(new Exception("Unable to update RAM Size. Please try again later."));
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        // GET: api/RAMSize
        [HttpGet]
        public IHttpActionResult GetAllRAMSizes()
        {
            List<RAMSize> ramSizeList = new List<RAMSize>();
            SqlConnection con = null;
            SqlDataReader reader = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "GetAllRAMSizes - Configuration Error",userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }
                con = new SqlConnection(connectionString);
                string query = @"SELECT RAMSId, Size, IsActive,
CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM Asset 
            WHERE Asset.RAMSId = RAMSize.RAMSId
        ) 
        THEN 1 
        ELSE 0 
    END AS IsUsed FROM RAMSize ORDER BY Size DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ramSizeList.Add(new RAMSize
                    {
                        Id = Convert.ToInt32(reader["RAMSId"]),
                        Name = reader["Size"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        IsUsed = Convert.ToBoolean(reader["IsUsed"])
                    });
                }
                return Ok(ramSizeList);
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during GetAllRAMSizes operation", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (InvalidCastException castEx)
            {
                common.LogError(castEx, "Data conversion error during GetAllRAMSizes operation", userId);
                return InternalServerError(new Exception("Data format error. Please contact support."));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during GetAllRAMSizes operation", userId  );
                return InternalServerError(new Exception("Unable to retrieve RAM size list. Please try again later."));
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

        // POST: api/RAMSize
        [HttpPost]
        public IHttpActionResult AddRAMSize(RAMSize ramSize)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                // Input validation
                if (ramSize == null)
                {
                    common.LogError(new ArgumentNullException("ramSize"), "AddRAMSize - Null RAMSize object received", userId);
                    return BadRequest("RAM Size data is required");
                }

                if (string.IsNullOrWhiteSpace(ramSize.Name))
                {
                    common.LogError(new ArgumentException("Size is null or empty"), "AddRAMSize - Invalid Size", userId);
                    return BadRequest("RAM Size is required and cannot be empty");
                }

                // Trim and validate RAM Size length
                ramSize.Name = ramSize.Name.Trim();
                if (ramSize.Name.Length > 255) // Assuming max length as per your table definition
                {
                    return BadRequest("RAM Size is too long. Maximum 255 characters allowed.");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "AddRAMSize - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = "INSERT INTO RAMSize (Size, IsActive,AddedUser,AddedTime) VALUES (@Size, @IsActive,@AddedUser,@AddedTime)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@Size", ramSize.Name);
                cmd.Parameters.AddWithValue("@IsActive", true);
                cmd.Parameters.AddWithValue("@AddedUser", userId);
                cmd.Parameters.AddWithValue("@AddedTime", DateTime.Now);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "RAM Size added successfully", size = ramSize.Name });
                }
                else
                {
                    common.LogError(new Exception("No rows affected during insert"), "AddRAMSize - Insert failed", userId);
                    return InternalServerError(new Exception("Failed to add RAM Size. Please try again."));
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, $"SQL Error during AddRAMSize operation - RAM Size: {ramSize?.Name}", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (ArgumentException argEx)
            {
                common.LogError(argEx, "Argument validation error during AddRAMSize operation", userId);
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"Unexpected error during AddRAMSize operation - RAM Size: {ramSize?.Name}", userId);
                return InternalServerError(new Exception("Unable to add RAM Size. Please try again later."));
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
    }
}
