using AssetAPIs.Filters;
using AssetAPIs.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Security.Claims;
using System.Web.Http;

namespace AssetAPIs.Controllers
{
    [JwtAuthentication]
    public class InstallSoftwareController : ApiController
    {
        private readonly Comman common = new Comman();

        [HttpPatch]
        public IHttpActionResult InstallSoftwares([FromBody] List<InstallesSoftwares> requests)
        {
            if (requests == null || requests.Count == 0)
            {
                return BadRequest("No data provided.");
            }

            // Get userId from token
            int userId = common.GetUserId((ClaimsPrincipal)User);
            if (userId == 0)
            {
                return Unauthorized();
            }

            SqlConnection con = null;
            string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                common.LogError(new Exception("Connection string is null or empty"), "InstallSoftwares - Configuration Error", userId);
                return InternalServerError(new Exception("Database configuration error"));
            }

            con = new SqlConnection(connectionString);

            try
            {
                con.Open();
                foreach (var request in requests)
                {
                    string checkQuery = @"
                update InstalledSoftwares set IsActive=@IsActive where InstalledSoftware=@InstalledSoftware";

                    using (SqlCommand cmd = new SqlCommand(checkQuery, con))
                    {
                       cmd.Parameters.AddWithValue("@InstalledSoftware", request.InstalledSoftwareId);
                        cmd.Parameters.AddWithValue("@IsActive", request.InstalledStatus);
                        cmd.ExecuteNonQuery();
                    }
                }
                return Ok(new { Message = "Software statuses updated successfully." });
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during InstallSoftwares operation", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (InvalidCastException castEx)
            {
                common.LogError(castEx, "Data conversion error during InstallSoftwares operation", userId);
                return InternalServerError(new Exception("Data format error. Please contact support."));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during InstallSoftwares operation", userId);
                return InternalServerError(new Exception("Unable to update software statuses. Please try again later."));
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