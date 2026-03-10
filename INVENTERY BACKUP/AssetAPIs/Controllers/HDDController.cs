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
    public class HDDController : ApiController
    {
        private readonly Comman common = new Comman();

        [HttpPatch]
        public IHttpActionResult UpdateHDD(HDD hdd)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                // Input validation
                if (hdd == null || hdd.Id <= 0)
                {
                    common.LogError(new ArgumentNullException("hdd"), "UpdateHDD - Invalid HDD data", userId);
                    return BadRequest("Invalid HDD data");
                }

                if (string.IsNullOrWhiteSpace(hdd.Name))
                {
                    common.LogError(new ArgumentException("HDDName is null or empty"), "UpdateHDD - Invalid HDDName", userId);
                    return BadRequest("HDD Name is required and cannot be empty");
                }

                // Trim and validate HDD name length
                hdd.Name = hdd.Name.Trim();
                if (hdd.Name.Length > 255)
                {
                    return BadRequest("HDD Name is too long. Maximum 255 characters allowed.");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "UpdateHDD - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = @"
            UPDATE HDD
            SET HDD = @HDDName,
                IsActive = @IsActive
            WHERE HDDId = @HDDId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@HDDName", hdd.Name);
                cmd.Parameters.AddWithValue("@IsActive", hdd.IsActive);
                cmd.Parameters.AddWithValue("@HDDId", hdd.Id);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "HDD updated successfully" });
                }
                else
                {
                    common.LogError(new Exception("No rows affected during update"), "UpdateHDD - Update failed", userId);
                    return InternalServerError(new Exception("Failed to update HDD. Please try again."));
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, $"SQL Error during UpdateHDD operation - HDD ID: {hdd?.Id}", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"Unexpected error during UpdateHDD operation - HDD ID: {hdd?.Id}", userId);
                return InternalServerError(new Exception("Unable to update HDD. Please try again later."));
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        // GET: api/HDD
        [HttpGet]
        public IHttpActionResult GetAllHDDs()
        {
            List<HDD> hddList = new List<HDD>();
            SqlConnection con = null;
            SqlDataReader reader = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "GetAllHDDs - Configuration Error",userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }
                con = new SqlConnection(connectionString);
                string query = @"SELECT HDDId, HDD, IsActive,
    CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM Asset 
            WHERE Asset.HDDId = HDD.HDDId
        ) 
        THEN 1 
        ELSE 0 
    END AS IsUsed FROM HDD ORDER BY HDD DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    hddList.Add(new HDD
                    {
                        Id = Convert.ToInt32(reader["HDDId"]),
                        Name = reader["HDD"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        IsUsed = Convert.ToBoolean(reader["IsUsed"])
                    });
                }
                return Ok(hddList);
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during GetAllHDDs operation", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (InvalidCastException castEx)
            {
                common.LogError(castEx, "Data conversion error during GetAllHDDs operation", userId);
                return InternalServerError(new Exception("Data format error. Please contact support."));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during GetAllHDDs operation", userId);
                return InternalServerError(new Exception("Unable to retrieve HDD list. Please try again later."));
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

        // POST: api/HDD
        [HttpPost]
        public IHttpActionResult AddHDD(HDD hdd)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                // Input validation
                if (hdd == null)
                {
                    common.LogError(new ArgumentNullException("hdd"), "AddHDD - Null HDD object received", userId);
                    return BadRequest("HDD data is required");
                }

                if (string.IsNullOrWhiteSpace(hdd.Name))
                {
                    common.LogError(new ArgumentException("HDDName is null or empty"), "AddHDD - Invalid HDDName", userId);
                    return BadRequest("HDD Name is required and cannot be empty");
                }

                // Trim and validate HDD name length
                hdd.Name = hdd.Name.Trim();
                if (hdd.Name.Length > 255) // Assuming max length as per your table definition
                {
                    return BadRequest("HDD Name is too long. Maximum 255 characters allowed.");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "AddHDD - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = "INSERT INTO HDD (HDD, IsActive,AddedUser,AddedTime) VALUES (@HDDName, @IsActive,@AddedUser,@AddedTime)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@HDDName", hdd.Name);
                cmd.Parameters.AddWithValue("@IsActive", true);
                cmd.Parameters.AddWithValue("@AddedUser", userId);
                cmd.Parameters.AddWithValue("@AddedTime", DateTime.Now);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "HDD added successfully", hddName = hdd.Name });
                }
                else
                {
                    common.LogError(new Exception("No rows affected during insert"), "AddHDD - Insert failed", userId);
                    return InternalServerError(new Exception("Failed to add HDD. Please try again."));
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, $"SQL Error during AddHDD operation - HDD Name: {hdd?.Name}", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (ArgumentException argEx)
            {
                common.LogError(argEx, "Argument validation error during AddHDD operation", userId);
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"Unexpected error during AddHDD operation - HDD Name: {hdd?.Name}", userId);
                return InternalServerError(new Exception("Unable to add HDD. Please try again later."));
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
