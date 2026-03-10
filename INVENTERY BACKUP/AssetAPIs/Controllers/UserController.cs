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
    public class UserController : ApiController
    {
        private readonly Comman common = new Comman();

        // GET: api/Users
        [HttpGet]
        public IHttpActionResult GetUsers()
        {
            List<User> userList = new List<User>();
            SqlConnection con = null;
            SqlDataReader reader = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "GetUsers - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }
                con = new SqlConnection(connectionString);
                string query = "SELECT UsersId, FullName, email, Phone, Designation,IsActive FROM Users WHERE IsActive = 1 ORDER BY FullName";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    userList.Add(new User
                    {
                        Id = Convert.ToInt32(reader["UsersId"]),
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        FullName = reader["FullName"].ToString(),
                        Email = reader["email"] != DBNull.Value ? reader["email"].ToString() : null,
                        Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : null,
                        Designation = reader["Designation"] != DBNull.Value ? reader["Designation"].ToString() : null
                    });
                }
                return Ok(userList);
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during GetUsers operation", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (InvalidCastException castEx)
            {
                common.LogError(castEx, "Data conversion error during GetUsers operation", userId);
                return InternalServerError(new Exception("Data format error. Please contact support."));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during GetUsers operation", userId);
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
    }
}
