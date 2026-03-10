using AssetAPIs.Helpers;
using AssetAPIs.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Http;

namespace AssetAPIs.Controllers
{
    public class LoginController : ApiController
    {
        private readonly Comman common = new Comman();

        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult Authenticate([FromBody] LoginModel login)
        {
            if (string.IsNullOrEmpty(login.UserName) || string.IsNullOrEmpty(login.Password))
            {
                return BadRequest("Please enter both email and password.");
            }

            SqlConnection con = null;
            SqlDataReader reader = null;

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "Login - Configuration Error");
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                string query = "SELECT UsersId, FullName, IsHeadOrNot FROM [Users] WHERE UserName = @UserName AND Password = @Password AND IsActive = 1";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserName", login.UserName.Trim());
                cmd.Parameters.AddWithValue("@Password", login.Password.Trim());

                con.Open();
                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    var userId = Convert.ToInt32(reader["UsersId"]).ToString();
                    var fullName = reader["FullName"].ToString();

                    // Generate JWT token
                    var token = JwtHelper.GenerateToken(userId, fullName);

                    // Return user data (you can customize the response as needed)
                    return Ok(new
                    {
                        Token = token,
                        UserId = userId,
                        FullName = fullName,
                        LoginTime = DateTime.Now
                    });
                }
                else
                {
                    return Unauthorized(); // Invalid credentials
                }
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during Login operation");
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (InvalidCastException castEx)
            {
                common.LogError(castEx, "Data conversion error during Login operation");
                return InternalServerError(new Exception("Data format error. Please contact support."));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during Login operation");
                return InternalServerError(new Exception("Unable to process login. Please try again later."));
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

    // Model for login request
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
