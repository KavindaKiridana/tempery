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
    public class RAMTypeController : ApiController
    {
        private readonly Comman common = new Comman();

        // GET: api/RAMType
        [HttpGet]
        public IHttpActionResult GetAllRAMTypes()
        {
            List<RAMType> ramTypeList = new List<RAMType>();
            SqlConnection con = null;
            SqlDataReader reader = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "GetAllRAMTypes - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }
                con = new SqlConnection(connectionString);
                string query = @"SELECT RAMTId, Type, IsActive,CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM Asset 
            WHERE Asset.RAMTId = RAMType.RAMTId
        ) 
        THEN 1 
        ELSE 0 
    END AS IsUsed  FROM RAMType ORDER BY Type DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ramTypeList.Add(new RAMType
                    {
                        Id = Convert.ToInt32(reader["RAMTId"]),
                        Name = reader["Type"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        IsUsed = Convert.ToBoolean(reader["IsUsed"])
                    });
                }
                return Ok(ramTypeList);
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during GetAllRAMTypes operation", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (InvalidCastException castEx)
            {
                common.LogError(castEx, "Data conversion error during GetAllRAMTypes operation", userId);
                return InternalServerError(new Exception("Data format error. Please contact support."));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during GetAllRAMTypes operation", userId  );
                return InternalServerError(new Exception("Unable to retrieve RAM type list. Please try again later."));
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

        // POST: api/RAMType
        [HttpPost]
        public IHttpActionResult AddRAMType(RAMType ramType)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                // Input validation
                if (ramType == null)
                {
                    common.LogError(new ArgumentNullException("ramType"), "AddRAMType - Null RAMType object received", userId);
                    return BadRequest("RAM Type data is required");
                }

                if (string.IsNullOrWhiteSpace(ramType.Name))
                {
                    common.LogError(new ArgumentException("Type is null or empty"), "AddRAMType - Invalid Type", userId);
                    return BadRequest("RAM Type is required and cannot be empty");
                }

                // Trim and validate RAM Type length
                ramType.Name = ramType.Name.Trim();
                if (ramType.Name.Length > 255) // Matching the DB column length
                {
                    return BadRequest("RAM Type is too long. Maximum 255 characters allowed.");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "AddRAMType - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = "INSERT INTO RAMType (Type, IsActive,AddedUser,AddedTime) VALUES (@Type, @IsActive,@AddedUser,@AddedTime)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@Type", ramType.Name);
                cmd.Parameters.AddWithValue("@IsActive", true);
                cmd.Parameters.AddWithValue("@AddedUser", userId);
                cmd.Parameters.AddWithValue("@AddedTime", DateTime.Now);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "RAM Type added successfully", ramType = ramType.Name });
                }
                else
                {
                    common.LogError(new Exception("No rows affected during insert"), "AddRAMType - Insert failed", userId);
                    return InternalServerError(new Exception("Failed to add RAM Type. Please try again."));
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, $"SQL Error during AddRAMType operation - RAM Type: {ramType?.Name}", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (ArgumentException argEx)
            {
                common.LogError(argEx, "Argument validation error during AddRAMType operation", userId);
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"Unexpected error during AddRAMType operation - RAM Type: {ramType?.Name}", userId);
                return InternalServerError(new Exception("Unable to add RAM Type. Please try again later."));
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
