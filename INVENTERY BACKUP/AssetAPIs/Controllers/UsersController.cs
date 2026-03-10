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
    public class UsersController : ApiController
    {
        private readonly Comman common = new Comman();

        // POST: api/Users
        [HttpPost]
        public IHttpActionResult AddUser(User user)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);

            try
            {
                // Validate input
                if (user == null)
                {
                    common.LogError(new ArgumentNullException("user"), "AddUser - Null User object received", userId);
                    return BadRequest("User data is required");
                }

                // Validate required fields
                if (string.IsNullOrWhiteSpace(user.FullName))
                {
                    return BadRequest("FullName is required");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "AddUser - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);

                // Insert new user
                string insertQuery = @"
        INSERT INTO Users (
            FullName,
            Email,
            Phone,
            Designation,
            IsActive,
            AddedUser,
            AddedTime,
            isCapexUser,
            DepartmentId,
            LocationId
        )
        VALUES (
            @FullName,
            @Email,
            @Phone,
            @Designation,
            @IsActive,
            @AddedUser,
            @AddedTime,
            @isCapexUser,
            @DepartmentId,
            @LocationId
        );
        SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(insertQuery, con);
                cmd.Parameters.AddWithValue("@FullName", user.FullName);
                cmd.Parameters.AddWithValue("@Email", (object)user.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", (object)user.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Designation", (object)user.Designation ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", true);
                cmd.Parameters.AddWithValue("@AddedUser", userId);
                cmd.Parameters.AddWithValue("@AddedTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@isCapexUser", user.isCapexUser);
                cmd.Parameters.AddWithValue("@DepartmentId", (object)user.DepartmentId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@LocationId", (object)user.LocationId ?? DBNull.Value);

                con.Open();
                int newUserId = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();

                // Return the newly created user ID
                return Ok(new { message = "User added successfully", userId = newUserId });
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during AddUser operation", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during AddUser operation", userId);
                return InternalServerError(new Exception("Unable to add user. Please try again later."));
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        // GET: api/Users
        [HttpGet]
        public IHttpActionResult GetAllUsers(bool needEveryUsers = true, int? requestedUserId = null, string search = "")
        {
            SqlConnection con = null;
            SqlDataReader reader = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "GetAllUsers - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }
                con = new SqlConnection(connectionString);
                if (needEveryUsers)
                {
                    List<User> userList = new List<User>();
                    string query = @"
SELECT
    u.UsersId,
    u.FullName,
    u.Email,
    u.Phone,
    u.Designation,
    u.IsActive,
    u.isCapexUser,
    u.DepartmentId,
    u.LocationId,
    d.DName,
    l.LName,
    u.AddedUser,
    CONVERT(date, u.AddedTime) AS AddedDate,
    addedUser.FullName AS AddedUserName
FROM Users AS u
LEFT JOIN Department AS d ON u.DepartmentId = d.DepartmentId
LEFT JOIN Location AS l ON u.LocationId = l.LocationId
LEFT JOIN Users AS addedUser ON u.AddedUser = addedUser.UsersId
WHERE
    (@search IS NULL OR @search = '')
    OR
    (
        u.FullName LIKE '%' + @search + '%' OR
        u.Email LIKE '%' + @search + '%' OR
        u.Phone LIKE '%' + @search + '%' OR
        u.Designation LIKE '%' + @search + '%' OR
        d.DName LIKE '%' + @search + '%' OR
        l.LName LIKE '%' + @search + '%'
    )
ORDER BY u.FullName ASC";
                    SqlCommand cmd = new SqlCommand(query, con);
                    string searchTerm = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
                    cmd.Parameters.AddWithValue("@search", (object)searchTerm ?? DBNull.Value);
                    cmd.CommandTimeout = 30;
                    con.Open();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        userList.Add(new User
                        {
                            Id = Convert.ToInt32(reader["UsersId"]),
                            FullName = reader["FullName"].ToString(),
                            Email = reader["Email"].ToString(),
                            Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : null,
                            Designation = reader["Designation"] != DBNull.Value ? reader["Designation"].ToString() : null,
                            isCapexUser = Convert.ToBoolean(reader["isCapexUser"]),
                            IsActive = Convert.ToBoolean(reader["IsActive"]),
                            DepartmentName = reader["DName"] != DBNull.Value ? reader["DName"].ToString() : null,
                            LocationName = reader["LName"] != DBNull.Value ? reader["LName"].ToString() : null,
                            AddedUserName = reader["AddedUserName"] != DBNull.Value ? reader["AddedUserName"].ToString() : null,
                            AddedTime = reader["AddedDate"] != DBNull.Value ? Convert.ToDateTime(reader["AddedDate"]) : default(DateTime)
                        });
                    }
                    return Ok(userList);
                }
                else
                {
                    if (requestedUserId == null)
                    {
                        return BadRequest("UserId is required when needEveryUsers is false");
                    }

                    User user = null;
                    bool isUsed = false;

                    string query = @"
        SELECT u.UsersId, u.FullName, u.Email, u.Phone, u.Designation, u.IsActive, 
               u.isCapexUser, u.DepartmentId, d.DName, u.LocationId, l.LName
        FROM Users AS u
        LEFT JOIN Department AS d ON u.DepartmentId = d.DepartmentId
        LEFT JOIN Location AS l ON u.LocationId = l.LocationId
        WHERE u.UsersId = @requestedUserId";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@requestedUserId", requestedUserId.Value);
                    cmd.CommandTimeout = 30;

                    con.Open();
                    reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        user = new User
                        {
                            Id = Convert.ToInt32(reader["UsersId"]),
                            FullName = reader["FullName"].ToString(),
                            Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : null,
                            Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : null,
                            Designation = reader["Designation"] != DBNull.Value ? reader["Designation"].ToString() : null,
                            isCapexUser = Convert.ToBoolean(reader["isCapexUser"]),
                            IsActive = Convert.ToBoolean(reader["IsActive"]),
                            DepartmentName = reader["DName"] != DBNull.Value ? reader["DName"].ToString() : null,
                            DepartmentId = reader["DepartmentId"] != DBNull.Value ? Convert.ToInt32(reader["DepartmentId"]) : (int?)null,
                            LocationId = reader["LocationId"] != DBNull.Value ? Convert.ToInt32(reader["LocationId"]) : (int?)null,
                            LocationName = reader["LName"] != DBNull.Value ? reader["LName"].ToString() : null
                        };
                    }

                    reader.Close();

                    // Check if the user is used in AssetUsedBy table
                    //        if (user != null)
                    //        {
                    //            string checkUsageQuery = @"
                    //SELECT COUNT(1) 
                    //FROM AssetUsedBy 
                    //WHERE UsedBy = @requestedUserId";

                    //            SqlCommand checkCmd = new SqlCommand(checkUsageQuery, con);
                    //            checkCmd.Parameters.AddWithValue("@requestedUserId", requestedUserId.Value);
                    //            checkCmd.CommandTimeout = 30;

                    //            int usageCount = (int)checkCmd.ExecuteScalar();
                    //            isUsed = usageCount > 0;

                    //            user.IsUsed = isUsed;
                    //        }
                    user.IsUsed = false; // Temporarily disabled this feature

                    return Ok(user);
                }

            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during GetAllUsers operation", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (InvalidCastException castEx)
            {
                common.LogError(castEx, "Data conversion error during GetAllUsers operation", userId);
                return InternalServerError(new Exception("Data format error. Please contact support."));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during GetAllUsers operation", userId);
                return InternalServerError(new Exception("Unable to retrieve user list. Please try again later."));
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

        [HttpPut]
        public IHttpActionResult UpdateUser(User user)
        {
            SqlConnection con = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                if (user == null)
                {
                    common.LogError(new ArgumentNullException("user"), "UpdateUser - Null User object received", userId);
                    return BadRequest("User data is required");
                }

                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "UpdateUser - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);

                // Check if the user is a CAPEX user
                string checkCapexQuery = "SELECT isCapexUser FROM Users WHERE UsersId = @UsersId";
                SqlCommand checkCapexCmd = new SqlCommand(checkCapexQuery, con);
                checkCapexCmd.Parameters.AddWithValue("@UsersId", user.Id);
                con.Open();
                bool isCapexUser = (bool)checkCapexCmd.ExecuteScalar();
                con.Close();

                // Build the update query based on isCapexUser
                string updateQuery;
                if (isCapexUser)
                {
                    // Only allow editing Phone, Designation, DepartmentId for CAPEX users
                    updateQuery = @"
                UPDATE Users
                SET
                    Phone = @Phone,
                    Designation = @Designation,
                    DepartmentId = @DepartmentId
                WHERE UsersId = @UsersId";
                }
                else
                {
                    // Allow editing all fields for non-CAPEX users
                    updateQuery = @"
                UPDATE Users
                SET
                    FullName = @FullName,
                    Email = @Email,
                    Phone = @Phone,
                    Designation = @Designation,
                    DepartmentId = @DepartmentId,
                    IsActive = @IsActive
                WHERE UsersId = @UsersId";
                }

                SqlCommand cmd = new SqlCommand(updateQuery, con);
                cmd.Parameters.AddWithValue("@UsersId", user.Id);
                if (!isCapexUser)
                {
                    cmd.Parameters.AddWithValue("@FullName", user.FullName);
                    cmd.Parameters.AddWithValue("@Email", (object)user.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                }
                cmd.Parameters.AddWithValue("@Phone", (object)user.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Designation", (object)user.Designation ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DepartmentId", (object)user.DepartmentId ?? DBNull.Value);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "User updated successfully" });
                }
                else
                {
                    common.LogError(new Exception("No rows affected during update"), "UpdateUser - Update failed", userId);
                    return InternalServerError(new Exception("Failed to update user. Please try again."));
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during UpdateUser operation", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during UpdateUser operation", userId);
                return InternalServerError(new Exception("Unable to update user. Please try again later."));
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