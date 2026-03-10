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
    public class OSController : ApiController
    {
        private readonly Comman common = new Comman();

        // PATCH: api/OS
        [HttpPatch]
        public IHttpActionResult UpdateOS(OS os)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                // Input validation
                if (os == null || os.Id <= 0)
                {
                    common.LogError(new ArgumentNullException("os"), "UpdateOS - Invalid OS data", userId);
                    return BadRequest("Invalid OS data");
                }

                if (string.IsNullOrWhiteSpace(os.Name))
                {
                    common.LogError(new ArgumentException("OS Name is null or empty"), "UpdateOS - Invalid OS Name", userId);
                    return BadRequest("OS Name is required and cannot be empty");
                }

                // Trim and validate OS name length
                os.Name = os.Name.Trim();
                if (os.Name.Length > 255)
                {
                    return BadRequest("OS Name is too long. Maximum 255 characters allowed.");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "UpdateOS - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = @"
            UPDATE OS
            SET OS = @OSName,
                IsActive = @IsActive
            WHERE OsId = @OsId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@OSName", os.Name);
                cmd.Parameters.AddWithValue("@IsActive", os.IsActive);
                cmd.Parameters.AddWithValue("@OsId", os.Id);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "OS updated successfully" });
                }
                else
                {
                    common.LogError(new Exception("No rows affected during update"), "UpdateOS - Update failed", userId);
                    return InternalServerError(new Exception("Failed to update OS. Please try again."));
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, $"SQL Error during UpdateOS operation - OS ID: {os?.Id}", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (ArgumentException argEx)
            {
                common.LogError(argEx, "Argument validation error during UpdateOS operation", userId);
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"Unexpected error during UpdateOS operation - OS ID: {os?.Id}", userId);
                return InternalServerError(new Exception("Unable to update OS. Please try again later."));
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        // GET: api/OS
        [HttpGet]
        public IHttpActionResult GetAllOS()
        {
            List<OS> osList = new List<OS>();
            SqlConnection con = null;
            SqlDataReader reader = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "GetAllOS - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = @"SELECT OsId, Os, IsActive,
CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM Asset 
            WHERE Asset.OsId = OS.OsId
        ) 
        THEN 1 
        ELSE 0 
    END AS IsUsed FROM OS ORDER BY Os DESC ";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30; // Set timeout

                con.Open();
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    osList.Add(new OS
                    {
                        Id = Convert.ToInt32(reader["OsId"]),
                        Name = reader["Os"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        IsUsed= Convert.ToBoolean(reader["IsUsed"])
                    });
                }

                return Ok(osList);
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during GetAllOS operation", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (InvalidCastException castEx)
            {
                common.LogError(castEx, "Data conversion error during GetAllOS operation", userId);
                return InternalServerError(new Exception("Data format error. Please contact support."));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during GetAllOS operation", userId);
                return InternalServerError(new Exception("Unable to retrieve OS list. Please try again later."));
            }
            finally
            {
                // Ensure resources are properly disposed
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

        // POST: api/OS
        [HttpPost]
        public IHttpActionResult AddOS(OS os)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                // Input validation
                if (os == null)
                {
                    common.LogError(new ArgumentNullException("os"), "AddOS - Null OS object received", userId);
                    return BadRequest("OS data is required");
                }

                if (string.IsNullOrWhiteSpace(os.Name))
                {
                    common.LogError(new ArgumentException("OsName is null or empty"), "AddOS - Invalid OsName", userId);
                    return BadRequest("OS Name is required and cannot be empty");
                }

                // Trim and validate OS name length
                os.Name = os.Name.Trim();
                if (os.Name.Length > 255) // Assuming max length
                {
                    return BadRequest("OS Name is too long. Maximum 255 characters allowed.");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "AddOS - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = "INSERT INTO OS (Os, IsActive,AddedUser,AddedTime) VALUES (@OsName, @IsActive,@AddedUser,@AddedTime)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@OsName", os.Name);
                cmd.Parameters.AddWithValue("@IsActive", true);
                cmd.Parameters.AddWithValue("@AddedUser", userId);
                cmd.Parameters.AddWithValue("@AddedTime", DateTime.Now);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "OS added successfully", osName = os.Name });
                }
                else
                {
                    common.LogError(new Exception("No rows affected during insert"), "AddOS - Insert failed", userId);
                    return InternalServerError(new Exception("Failed to add OS. Please try again."));
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, $"SQL Error during AddOS operation - OS Name: {os?.Name}", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (ArgumentException argEx)
            {
                common.LogError(argEx, "Argument validation error during AddOS operation", userId);
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"Unexpected error during AddOS operation - OS Name: {os?.Name}", userId);
                return InternalServerError(new Exception("Unable to add OS. Please try again later."));
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        // DELETE: api/OS
        [HttpDelete]
        public IHttpActionResult DeleteOS(int id)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                // Input validation
                if (id <= 0)
                {
                    common.LogError(new ArgumentException($"Invalid ID: {id}"), "DeleteOS - Invalid ID", userId);
                    return BadRequest("Invalid OS ID. ID must be greater than 0.");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "DeleteOS - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = "DELETE FROM OS WHERE OsId = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    common.LogError(new Exception($"OS with ID {id} not found"), "DeleteOS - Record not found", userId);
                    return NotFound();
                }

                return Ok(new { message = "OS deleted successfully", osId = id });
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, $"SQL Error during DeleteOS operation - OS ID: {id}", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);

                // Special handling for foreign key constraint violations
                if (sqlEx.Number == 547)
                {
                    return BadRequest("Cannot delete this OS as it is being used by other records. Please remove dependencies first.");
                }

                return InternalServerError(new Exception(userMessage));
            }
            catch (ArgumentException argEx)
            {
                common.LogError(argEx, "Argument validation error during DeleteOS operation", userId);
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"Unexpected error during DeleteOS operation - OS ID: {id}", userId);
                return InternalServerError(new Exception("Unable to delete OS. Please try again later."));
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
