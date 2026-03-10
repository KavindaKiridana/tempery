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
    public class ModelController : ApiController
    {
        private readonly Comman common = new Comman();

        // PATCH: api/Model
        [HttpPatch]
        public IHttpActionResult UpdateModel(Model model)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                // Input validation
                if (model == null || model.Id <= 0)
                {
                    common.LogError(new ArgumentNullException("model"), "UpdateModel - Invalid Model data", userId);
                    return BadRequest("Invalid Model data");
                }

                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    common.LogError(new ArgumentException("Model Name is null or empty"), "UpdateModel - Invalid Model Name", userId);
                    return BadRequest("Model Name is required and cannot be empty");
                }

                // Trim and validate Model name length
                model.Name = model.Name.Trim();
                if (model.Name.Length > 255)
                {
                    return BadRequest("Model Name is too long. Maximum 255 characters allowed.");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "UpdateModel - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = @"
            UPDATE Model
            SET Model = @ModelName,
                IsActive = @IsActive
            WHERE ModelId = @ModelId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@ModelName", model.Name);
                cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
                cmd.Parameters.AddWithValue("@ModelId", model.Id);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "Model updated successfully" });
                }
                else
                {
                    common.LogError(new Exception("No rows affected during update"), "UpdateModel - Update failed", userId);
                    return InternalServerError(new Exception("Failed to update Model. Please try again."));
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, $"SQL Error during UpdateModel operation - Model ID: {model?.Id}", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (ArgumentException argEx)
            {
                common.LogError(argEx, "Argument validation error during UpdateModel operation", userId);
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"Unexpected error during UpdateModel operation - Model ID: {model?.Id}", userId);
                return InternalServerError(new Exception("Unable to update Model. Please try again later."));
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        // GET: api/Model
        [HttpGet]
        public IHttpActionResult GetAllModels()
        {
            List<Model> modelList = new List<Model>();
            SqlConnection con = null;
            SqlDataReader reader = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "GetAllModels - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }
                con = new SqlConnection(connectionString);
                string query = @"SELECT ModelId, Model, IsActive,
CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM Asset 
            WHERE Asset.Model = Model.ModelId
        ) 
        THEN 1 
        ELSE 0 
    END AS IsUsed  FROM Model ORDER BY Model DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    modelList.Add(new Model
                    {
                        Id = Convert.ToInt32(reader["ModelId"]),
                        Name = reader["Model"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        IsUsed= Convert.ToBoolean(reader["IsUsed"])
                    });
                }
                return Ok(modelList);
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during GetAllModels operation", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (InvalidCastException castEx)
            {
                common.LogError(castEx, "Data conversion error during GetAllModels operation", userId);
                return InternalServerError(new Exception("Data format error. Please contact support."));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during GetAllModels operation", userId);
                return InternalServerError(new Exception("Unable to retrieve model list. Please try again later."));
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

        // POST: api/Model
        [HttpPost]
        public IHttpActionResult AddModel(Model model)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                // Input validation
                if (model == null)
                {
                    common.LogError(new ArgumentNullException("model"), "AddModel - Null Model object received", userId);
                    return BadRequest("Model data is required");
                }

                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    common.LogError(new ArgumentException("Model Name is null or empty"), "AddModel - Invalid Model Name", userId);
                    return BadRequest("Model Name is required and cannot be empty");
                }

                // Trim and validate Model name length
                model.Name = model.Name.Trim();
                if (model.Name.Length > 255) // Matching the DB column length
                {
                    return BadRequest("Model Name is too long. Maximum 255 characters allowed.");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "AddModel - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = "INSERT INTO Model (Model, IsActive,AddedUser,AddedTime) VALUES (@ModelName, @IsActive,@AddedUser,@AddedTime)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@ModelName", model.Name);
                cmd.Parameters.AddWithValue("@IsActive", true);
                cmd.Parameters.AddWithValue("@AddedUser", userId);
                cmd.Parameters.AddWithValue("@AddedTime", DateTime.Now);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "Model added successfully", modelName = model.Name });
                }
                else
                {
                    common.LogError(new Exception("No rows affected during insert"), "AddModel - Insert failed", userId);
                    return InternalServerError(new Exception("Failed to add Model. Please try again."));
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, $"SQL Error during AddModel operation - Model Name: {model?.Name}", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (ArgumentException argEx)
            {
                common.LogError(argEx, "Argument validation error during AddModel operation", userId);
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"Unexpected error during AddModel operation - Model Name: {model?.Name}", userId);
                return InternalServerError(new Exception("Unable to add Model. Please try again later."));
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
