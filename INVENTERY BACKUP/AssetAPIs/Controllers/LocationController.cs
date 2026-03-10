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
    public class LocationController : ApiController
    {
        private readonly Comman common = new Comman();

        // GET: api/Location
        [HttpGet]
        public IHttpActionResult GetAllLocations()
        {
            List<Location> locationList = new List<Location>();
            SqlConnection con = null;
            SqlDataReader reader = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "GetAllLocations - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }
                con = new SqlConnection(connectionString);
                string query = "SELECT LocationId, LName, IsActive,IsStockLocation FROM Location ORDER BY LName DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    locationList.Add(new Location
                    {
                        Id = Convert.ToInt32(reader["LocationId"]),
                        Name = reader["LName"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        IsStockLocation= Convert.ToBoolean(reader["IsStockLocation"])
                    });
                }
                return Ok(locationList);
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during GetAllLocations operation", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (InvalidCastException castEx)
            {
                common.LogError(castEx, "Data conversion error during GetAllLocations operation", userId);
                return InternalServerError(new Exception("Data format error. Please contact support."));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during GetAllLocations operation", userId);
                return InternalServerError(new Exception("Unable to retrieve location list. Please try again later."));
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

        // POST: api/Location
        [HttpPost]
        public IHttpActionResult AddLocation(Location location)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                if (location == null)
                {
                    common.LogError(new ArgumentNullException("location"), "AddLocation - Null Location object received", userId);
                    return BadRequest("Location data is required");
                }
                if (string.IsNullOrWhiteSpace(location.Name))
                {
                    common.LogError(new ArgumentException("Location name is null or empty"), "AddLocation - Invalid Location Name", userId);
                    return BadRequest("Location Name is required and cannot be empty");
                }
                location.Name = location.Name.Trim();
                if (location.Name.Length > 255)
                {
                    return BadRequest("Location Name is too long. Maximum 255 characters allowed.");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "AddLocation - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }
                con = new SqlConnection(connectionString);
                string query = "INSERT INTO Location (LName, IsActive) VALUES (@LName, @IsActive)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@LName", location.Name);
                cmd.Parameters.AddWithValue("@IsActive", true);
                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return Ok(new { message = "Location added successfully", locationName = location.Name });
                }
                else
                {
                    common.LogError(new Exception("No rows affected during insert"), "AddLocation - Insert failed", userId);
                    return InternalServerError(new Exception("Failed to add Location. Please try again."));
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, $"SQL Error during AddLocation operation - Location Name: {location?.Name}", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (ArgumentException argEx)
            {
                common.LogError(argEx, "Argument validation error during AddLocation operation", userId);
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"Unexpected error during AddLocation operation - Location Name: {location?.Name}",userId);
                return InternalServerError(new Exception("Unable to add Location. Please try again later."));
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        // DELETE: api/Location
        [HttpDelete]
        public IHttpActionResult DeleteLocation(int id)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                if (id <= 0)
                {
                    common.LogError(new ArgumentException($"Invalid ID: {id}"), "DeleteLocation - Invalid ID", userId);
                    return BadRequest("Invalid Location ID. ID must be greater than 0.");
                }
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "DeleteLocation - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }
                con = new SqlConnection(connectionString);
                string query = "DELETE FROM Location WHERE LocationId = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    common.LogError(new Exception($"Location with ID {id} not found"), "DeleteLocation - Record not found", userId);
                    return NotFound();
                }
                return Ok(new { message = "Location deleted successfully", locationId = id });
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, $"SQL Error during DeleteLocation operation - Location ID: {id}", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                if (sqlEx.Number == 547)
                {
                    return BadRequest("Cannot delete this Location as it is being used by other records. Please remove dependencies first.");
                }
                return InternalServerError(new Exception(userMessage));
            }
            catch (ArgumentException argEx)
            {
                common.LogError(argEx, "Argument validation error during DeleteLocation operation", userId);
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"Unexpected error during DeleteLocation operation - Location ID: {id}", userId);
                return InternalServerError(new Exception("Unable to delete Location. Please try again later."));
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