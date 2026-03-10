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
    public class ProcessorController : ApiController
    {
        private readonly Comman common = new Comman();

        // PATCH: api/Processor
        [HttpPatch]
        public IHttpActionResult UpdateProcessor(Processor processor)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                // Input validation
                if (processor == null || processor.Id <= 0)
                {
                    common.LogError(new ArgumentNullException("processor"), "UpdateProcessor - Invalid Processor data", userId);
                    return BadRequest("Invalid Processor data");
                }

                if (string.IsNullOrWhiteSpace(processor.Name))
                {
                    common.LogError(new ArgumentException("Processor Name is null or empty"), "UpdateProcessor - Invalid Processor Name", userId);
                    return BadRequest("Processor Name is required and cannot be empty");
                }

                // Trim and validate Processor name length
                processor.Name = processor.Name.Trim();
                if (processor.Name.Length > 255)
                {
                    return BadRequest("Processor Name is too long. Maximum 255 characters allowed.");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "UpdateProcessor - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = @"
            UPDATE Processor
            SET Processor = @ProcessorName,
                IsActive = @IsActive
            WHERE PId = @ProcessorId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@ProcessorName", processor.Name);
                cmd.Parameters.AddWithValue("@IsActive", processor.IsActive);
                cmd.Parameters.AddWithValue("@ProcessorId", processor.Id);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "Processor updated successfully" });
                }
                else
                {
                    common.LogError(new Exception("No rows affected during update"), "UpdateProcessor - Update failed", userId);
                    return InternalServerError(new Exception("Failed to update Processor. Please try again."));
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, $"SQL Error during UpdateProcessor operation - Processor ID: {processor?.Id}", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (ArgumentException argEx)
            {
                common.LogError(argEx, "Argument validation error during UpdateProcessor operation", userId);
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"Unexpected error during UpdateProcessor operation - Processor ID: {processor?.Id}", userId);
                return InternalServerError(new Exception("Unable to update Processor. Please try again later."));
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        // GET: api/Processor
        [HttpGet]
        public IHttpActionResult GetAllProcessors()
        {
            List<Processor> processorList = new List<Processor>();
            SqlConnection con = null;
            SqlDataReader reader = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "GetAllProcessors - Configuration Error",userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }
                con = new SqlConnection(connectionString);
                string query = @"SELECT PId, Processor, IsActive,
CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM Asset 
            WHERE Asset.PId = Processor.PId
        ) 
        THEN 1 
        ELSE 0 
    END AS IsUsed
FROM Processor ORDER BY Processor DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    processorList.Add(new Processor
                    {
                        Id = Convert.ToInt32(reader["PId"]),
                        Name = reader["Processor"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        IsUsed= Convert.ToBoolean(reader["IsUsed"])
                    });
                }
                return Ok(processorList);
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during GetAllProcessors operation", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (InvalidCastException castEx)
            {
                common.LogError(castEx, "Data conversion error during GetAllProcessors operation", userId);
                return InternalServerError(new Exception("Data format error. Please contact support."));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during GetAllProcessors operation", userId);
                return InternalServerError(new Exception("Unable to retrieve processor list. Please try again later."));
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

        // POST: api/Processor
        [HttpPost]
        public IHttpActionResult AddProcessor(Processor processor)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                // Input validation
                if (processor == null)
                {
                    common.LogError(new ArgumentNullException("processor"), "AddProcessor - Null Processor object received", userId);
                    return BadRequest("Processor data is required");
                }

                if (string.IsNullOrWhiteSpace(processor.Name))
                {
                    common.LogError(new ArgumentException("ProcessorName is null or empty"), "AddProcessor - Invalid ProcessorName", userId);
                    return BadRequest("Processor Name is required and cannot be empty");
                }

                // Trim and validate Processor name length
                processor.Name = processor.Name.Trim();
                if (processor.Name.Length > 255) // Assuming max length as per your table definition
                {
                    return BadRequest("Processor Name is too long. Maximum 255 characters allowed.");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "AddProcessor - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = "INSERT INTO Processor (Processor, IsActive,AddedUser,AddedTime) VALUES (@ProcessorName, @IsActive,@AddedUser,@AddedTime)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@ProcessorName", processor.Name);
                cmd.Parameters.AddWithValue("@IsActive", true);
                cmd.Parameters.AddWithValue("@AddedUser", userId);
                cmd.Parameters.AddWithValue("@AddedTime", DateTime.Now);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "Processor added successfully", processorName = processor.Name });
                }
                else
                {
                    common.LogError(new Exception("No rows affected during insert"), "AddProcessor - Insert failed", userId);
                    return InternalServerError(new Exception("Failed to add Processor. Please try again."));
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, $"SQL Error during AddProcessor operation - Processor Name: {processor?.Name}", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (ArgumentException argEx)
            {
                common.LogError(argEx, "Argument validation error during AddProcessor operation", userId);
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"Unexpected error during AddProcessor operation - Processor Name: {processor?.Name}", userId);
                return InternalServerError(new Exception("Unable to add Processor. Please try again later."));
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
