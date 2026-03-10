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
    public class TypeController : ApiController
    {
        private readonly Comman common = new Comman();

        // GET: api/Type
        [HttpGet]
        public IHttpActionResult GetAllTypes()
        {
            List<AssetAPIs.Models.Type> typeList = new List<AssetAPIs.Models.Type>();
            SqlConnection con = null;
            SqlDataReader reader = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "GetAllTypes - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = @"
                    SELECT
                        TypeId,
                        Type,
                        Category,
                        IsActive
                        -- , CASE
                        --    WHEN EXISTS (
                        --        SELECT 1
                        --        FROM Asset
                        --        WHERE Asset.TypeId = Type.TypeId
                        --    )
                        --    THEN 1
                        --    ELSE 0
                        --END AS IsUsed
                    FROM Type
                    ORDER BY Type DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                con.Open();
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    typeList.Add(new AssetAPIs.Models.Type
                    {
                        Id = Convert.ToInt32(reader["TypeId"]),
                        AssetType = reader["Type"].ToString(),
                        Category = reader["Category"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"])
                       // IsUsed = Convert.ToBoolean(reader["IsUsed"])
                    });
                }
                return Ok(typeList);
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during GetAllTypes operation", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (InvalidCastException castEx)
            {
                common.LogError(castEx, "Data conversion error during GetAllTypes operation", userId);
                return InternalServerError(new Exception("Data format error. Please contact support."));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during GetAllTypes operation", userId);
                return InternalServerError(new Exception("Unable to retrieve type list. Please try again later."));
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

        // POST: api/Type
        [HttpPost]
        public IHttpActionResult AddType(AssetAPIs.Models.Type type)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                if (type == null)
                {
                    common.LogError(new ArgumentNullException("type"), "AddType - Null type object received", userId);
                    return BadRequest("Type data is required");
                }

                if (string.IsNullOrWhiteSpace(type.AssetType))
                {
                    common.LogError(new ArgumentException("AssetType is null or empty"), "AddType - Invalid AssetType", userId);
                    return BadRequest("Asset Type is required and cannot be empty");
                }

                if (string.IsNullOrWhiteSpace(type.Category))
                {
                    common.LogError(new ArgumentException("Category is null or empty"), "AddType - Invalid Category", userId);
                    return BadRequest("Category is required and cannot be empty");
                }

                type.AssetType = type.AssetType.Trim();
                type.Category = type.Category.Trim();

                if (type.AssetType.Length > 255 || type.Category.Length > 255)
                {
                    return BadRequest("Asset Type or Category is too long. Maximum 255 characters allowed.");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "AddType - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = @"
                    INSERT INTO Type
                    (Type, Category, IsActive, AddedUser, AddedTime)
                    VALUES
                    (@AssetType, @Category, @IsActive, @AddedUser, @AddedTime)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@AssetType", type.AssetType);
                cmd.Parameters.AddWithValue("@Category", type.Category);
                cmd.Parameters.AddWithValue("@IsActive", true);
                cmd.Parameters.AddWithValue("@AddedUser", userId);
                cmd.Parameters.AddWithValue("@AddedTime", DateTime.Now);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "Type added successfully", typeName = type.AssetType });
                }
                else
                {
                    common.LogError(new Exception("No rows affected during insert"), "AddType - Insert failed", userId);
                    return InternalServerError(new Exception("Failed to add type. Please try again."));
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, $"SQL Error during AddType operation - Type: {type?.AssetType}", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"Unexpected error during AddType operation - Type: {type?.AssetType}", userId);
                return InternalServerError(new Exception("Unable to add type. Please try again later."));
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        // PATCH: api/Type
        [HttpPatch]
        public IHttpActionResult UpdateType(AssetAPIs.Models.Type type)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                if (type == null || type.Id <= 0)
                {
                    common.LogError(new ArgumentNullException("type"), "UpdateType - Invalid type data", userId);
                    return BadRequest("Invalid type data");
                }

                if (string.IsNullOrWhiteSpace(type.AssetType))
                {
                    common.LogError(new ArgumentException("AssetType is null or empty"), "UpdateType - Invalid AssetType", userId);
                    return BadRequest("Asset Type is required and cannot be empty");
                }

                if (string.IsNullOrWhiteSpace(type.Category))
                {
                    common.LogError(new ArgumentException("Category is null or empty"), "UpdateType - Invalid Category", userId);
                    return BadRequest("Category is required and cannot be empty");
                }

                type.AssetType = type.AssetType.Trim();
                type.Category = type.Category.Trim();

                if (type.AssetType.Length > 255 || type.Category.Length > 255)
                {
                    return BadRequest("Asset Type or Category is too long. Maximum 255 characters allowed.");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "UpdateType - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = @"
                    UPDATE Type
                    SET
                        Type = @AssetType,
                        Category = @Category,
                        IsActive = @IsActive
                    WHERE TypeId = @TypeId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@AssetType", type.AssetType);
                cmd.Parameters.AddWithValue("@Category", type.Category);
                cmd.Parameters.AddWithValue("@IsActive", type.IsActive);
                cmd.Parameters.AddWithValue("@TypeId", type.Id);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "Type updated successfully" });
                }
                else
                {
                    common.LogError(new Exception("No rows affected during update"), "UpdateType - Update failed", userId);
                    return InternalServerError(new Exception("Failed to update type. Please try again."));
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, $"SQL Error during UpdateType operation - Type ID: {type?.Id}", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"Unexpected error during UpdateType operation - Type ID: {type?.Id}", userId);
                return InternalServerError(new Exception("Unable to update type. Please try again later."));
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
