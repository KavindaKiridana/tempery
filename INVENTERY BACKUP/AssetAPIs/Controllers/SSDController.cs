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
    public class SSDController : ApiController
    {
        private readonly Comman common = new Comman();

        // PATCH: api/SSD
        [HttpPatch]
        public IHttpActionResult UpdateSSD(SSD ssd)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                // Input validation
                if (ssd == null || ssd.Id <= 0)
                {
                    common.LogError(new ArgumentNullException("ssd"), "UpdateSSD - Invalid SSD data", userId);
                    return BadRequest("Invalid SSD data");
                }

                if (string.IsNullOrWhiteSpace(ssd.Name))
                {
                    common.LogError(new ArgumentException("SSD Name is null or empty"), "UpdateSSD - Invalid SSD Name", userId);
                    return BadRequest("SSD Name is required and cannot be empty");
                }

                // Trim and validate SSD name length
                ssd.Name = ssd.Name.Trim();
                if (ssd.Name.Length > 255)
                {
                    return BadRequest("SSD Name is too long. Maximum 255 characters allowed.");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "UpdateSSD - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = @"
            UPDATE SSD
            SET SSD = @SSDName,
                IsActive = @IsActive
            WHERE SSDId = @SSDId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@SSDName", ssd.Name);
                cmd.Parameters.AddWithValue("@IsActive", ssd.IsActive);
                cmd.Parameters.AddWithValue("@SSDId", ssd.Id);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "SSD updated successfully" });
                }
                else
                {
                    common.LogError(new Exception("No rows affected during update"), "UpdateSSD - Update failed", userId);
                    return InternalServerError(new Exception("Failed to update SSD. Please try again."));
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, $"SQL Error during UpdateSSD operation - SSD ID: {ssd?.Id}", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (ArgumentException argEx)
            {
                common.LogError(argEx, "Argument validation error during UpdateSSD operation", userId);
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"Unexpected error during UpdateSSD operation - SSD ID: {ssd?.Id}", userId);
                return InternalServerError(new Exception("Unable to update SSD. Please try again later."));
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        // GET: api/SSD
        [HttpGet]
        public IHttpActionResult GetAllSSDs()
        {
            List<SSD> ssdList = new List<SSD>();
            SqlConnection con = null;
            SqlDataReader reader = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "GetAllSSDs - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }
                con = new SqlConnection(connectionString);
                string query = @"SELECT SSDId, SSD, IsActive,
CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM Asset 
            WHERE Asset.SSDId = SSD.SSDId
        ) 
        THEN 1 
        ELSE 0 
    END AS IsUsed  FROM SSD ORDER BY SSD DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ssdList.Add(new SSD
                    {
                        Id = Convert.ToInt32(reader["SSDId"]),
                        Name = reader["SSD"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        IsUsed = Convert.ToBoolean(reader["IsUsed"])
                    });
                }
                return Ok(ssdList);
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during GetAllSSDs operation", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (InvalidCastException castEx)
            {
                common.LogError(castEx, "Data conversion error during GetAllSSDs operation", userId);
                return InternalServerError(new Exception("Data format error. Please contact support."));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during GetAllSSDs operation", userId);
                return InternalServerError(new Exception("Unable to retrieve SSD list. Please try again later."));
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

        // POST: api/SSD
        [HttpPost]
        public IHttpActionResult AddSSD(SSD ssd)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                // Input validation
                if (ssd == null)
                {
                    common.LogError(new ArgumentNullException("ssd"), "AddSSD - Null SSD object received", userId);
                    return BadRequest("SSD data is required");
                }

                if (string.IsNullOrWhiteSpace(ssd.Name))
                {
                    common.LogError(new ArgumentException("SSD Name is null or empty"), "AddSSD - Invalid SSD Name", userId);
                    return BadRequest("SSD Name is required and cannot be empty");
                }

                // Trim and validate SSD name length
                ssd.Name = ssd.Name.Trim();
                if (ssd.Name.Length > 255) // Matching the DB column length
                {
                    return BadRequest("SSD Name is too long. Maximum 255 characters allowed.");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "AddSSD - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = "INSERT INTO SSD (SSD, IsActive,AddedUser,AddedTime) VALUES (@SSDName, @IsActive,@AddedUser,@AddedTime)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@SSDName", ssd.Name);
                cmd.Parameters.AddWithValue("@IsActive", true);
                cmd.Parameters.AddWithValue("@AddedUser", userId);
                cmd.Parameters.AddWithValue("@AddedTime", DateTime.Now);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "SSD added successfully", ssdName = ssd.Name });
                }
                else
                {
                    common.LogError(new Exception("No rows affected during insert"), "AddSSD - Insert failed", userId);
                    return InternalServerError(new Exception("Failed to add SSD. Please try again."));
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, $"SQL Error during AddSSD operation - SSD Name: {ssd?.Name}", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (ArgumentException argEx)
            {
                common.LogError(argEx, "Argument validation error during AddSSD operation", userId);
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"Unexpected error during AddSSD operation - SSD Name: {ssd?.Name}", userId);
                return InternalServerError(new Exception("Unable to add SSD. Please try again later."));
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
