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
    public class DisplayController : ApiController
    {
        private readonly Comman common = new Comman();

        // PATCH: api/Display
        [HttpPatch]
        public IHttpActionResult UpdateDisplay(Display display)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                if (display == null || display.Id <= 0)
                {
                    common.LogError(new ArgumentNullException("display"), "UpdateDisplay - Invalid Display data", userId);
                    return BadRequest("Invalid display data");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "UpdateDisplay - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = @"
            UPDATE Display
            SET Display = @DisplayName,
                IsActive = @IsActive
            WHERE DisplayId = @DisplayId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@DisplayName", display.Name);
                cmd.Parameters.AddWithValue("@IsActive", display.IsActive);
                cmd.Parameters.AddWithValue("@DisplayId", display.Id);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "Display updated successfully" });
                }
                else
                {
                    common.LogError(new Exception("No rows affected during update"), "UpdateDisplay - Update failed", userId);
                    return InternalServerError(new Exception("Failed to update Display. Please try again."));
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, $"SQL Error during UpdateDisplay operation - Display ID: {display?.Id}", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"Unexpected error during UpdateDisplay operation - Display ID: {display?.Id}", userId);
                return InternalServerError(new Exception("Unable to update Display. Please try again later."));
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        // GET: api/Display
        [HttpGet]
        public IHttpActionResult GetAllDisplays()
        {
            List<Display> displayList = new List<Display>();
            SqlConnection con = null;
            SqlDataReader reader = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "GetAllDisplays - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }
                con = new SqlConnection(connectionString);
                string query = @"
SELECT 
    DisplayId,
    Display,
    IsActive,
    CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM Asset 
            WHERE Asset.DisplayId = Display.DisplayId
        ) 
        THEN 1 
        ELSE 0 
    END AS IsUsed
FROM Display 
ORDER BY Display DESC ";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    displayList.Add(new Display
                    {
                        Id = Convert.ToInt32(reader["DisplayId"]),
                        Name = reader["Display"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        IsUsed = Convert.ToBoolean(reader["IsUsed"])
                    });
                }
                return Ok(displayList);
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during GetAllDisplays operation", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (InvalidCastException castEx)
            {
                common.LogError(castEx, "Data conversion error during GetAllDisplays operation", userId);
                return InternalServerError(new Exception("Data format error. Please contact support."));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during GetAllDisplays operation", userId);
                return InternalServerError(new Exception("Unable to retrieve display list. Please try again later."));
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

        // POST: api/Display
        [HttpPost]
        public IHttpActionResult AddDisplay(Display display)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                // Input validation
                if (display == null)
                {
                    common.LogError(new ArgumentNullException("display"), "AddDisplay - Null Display object received", userId);
                    return BadRequest("Display data is required");
                }

                if (string.IsNullOrWhiteSpace(display.Name))
                {
                    common.LogError(new ArgumentException("Display Name is null or empty"), "AddDisplay - Invalid Display Name", userId);
                    return BadRequest("Display Name is required and cannot be empty");
                }

                // Trim and validate Display name length
                display.Name = display.Name.Trim();
                if (display.Name.Length > 255) // Matching the DB column length
                {
                    return BadRequest("Display Name is too long. Maximum 255 characters allowed.");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "AddDisplay - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = "INSERT INTO Display (Display, IsActive,AddedUser,AddedTime) VALUES (@DisplayName, @IsActive,@AddedUser,@AddedTime)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@DisplayName", display.Name);
                cmd.Parameters.AddWithValue("@IsActive", true);
                cmd.Parameters.AddWithValue("@AddedUser", userId);
                cmd.Parameters.AddWithValue("@AddedTime", DateTime.Now);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "Display added successfully", displayName = display.Name });
                }
                else
                {
                    common.LogError(new Exception("No rows affected during insert"), "AddDisplay - Insert failed", userId);
                    return InternalServerError(new Exception("Failed to add Display. Please try again."));
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, $"SQL Error during AddDisplay operation - Display Name: {display?.Name}", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (ArgumentException argEx)
            {
                common.LogError(argEx, "Argument validation error during AddDisplay operation", userId);
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"Unexpected error during AddDisplay operation - Display Name: {display?.Name}", userId);
                return InternalServerError(new Exception("Unable to add Display. Please try again later."));
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
