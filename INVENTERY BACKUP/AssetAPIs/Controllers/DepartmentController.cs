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
    public class DepartmentController : ApiController
    {
        private readonly Comman common = new Comman();

        // GET: api/Department
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetAllDepartments()
        {
            List<Department> List = new List<Department>();
            SqlConnection con = null;
            SqlDataReader reader = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "GetAllOS - Configuration Error",userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = "SELECT DepartmentId, DName, IsActive FROM Department ORDER BY DName DESC ";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30; // Set timeout

                con.Open();
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    List.Add(new Department
                    {
                        Id = Convert.ToInt32(reader["DepartmentId"]),
                        Name = reader["DName"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"])
                    });
                }
                return Ok(List);
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during GetAllDepartment operation", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (InvalidCastException castEx)
            {
                common.LogError(castEx, "Data conversion error during GetAllDepartment operation", userId);
                return InternalServerError(new Exception("Data format error. Please contact support."));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during GetAllDepartment operation", userId);
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

        // POST: api/Departmet
        [System.Web.Http.HttpPost]
        public IHttpActionResult AddDepartment(Department department)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                // Input validation
                if (department == null)
                {
                    common.LogError(new ArgumentNullException("department"), "AddDepartment - Null Department object received", userId);
                    return BadRequest("Department data is required");
                }

                if (string.IsNullOrWhiteSpace(department.Name))
                {
                    common.LogError(new ArgumentException("Department Name is null or empty"), "AddDepartment - Invalid Department Name", userId);
                    return BadRequest("Department Name is required and cannot be empty");
                }

                // Trim and validate  name length
                department.Name = department.Name.Trim();
                if (department.Name.Length > 255) // Assuming max length
                {
                    return BadRequest("Department Name is too long. Maximum 255 characters allowed.");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "AddDepartment - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = "INSERT INTO Department (DName, IsActive) VALUES (@OsName, @IsActive)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                cmd.Parameters.AddWithValue("@OsName", department.Name);
                cmd.Parameters.AddWithValue("@IsActive", true);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "Department added successfully" });
                }
                else
                {
                    common.LogError(new Exception("No rows affected during insert"), "AddDepartment - Insert failed", userId);
                    return InternalServerError(new Exception("Failed to add Department. Please try again."));
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, $"SQL Error during AddDepartment operation - Department Name: {department?.Name}", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (ArgumentException argEx)
            {
                common.LogError(argEx, "Argument validation error during AddDeprtment operation", userId);
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                common.LogError(ex, $"Unexpected error during AddDepartment operation - Department Name: {department?.Name}",userId);
                return InternalServerError(new Exception("Unable to add Department. Please try again later."));
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
        [System.Web.Http.HttpDelete]
        public IHttpActionResult DeleteDepartment(int id)
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