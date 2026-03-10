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
    public class CompanyController : ApiController
    {
        private readonly Comman common = new Comman();
        // GET: api/Suppliers
        [HttpGet]
        public IHttpActionResult GetAllCompanies()
        {
            List<Company> companyList = new List<Company>();
            SqlConnection con = null;
            SqlDataReader reader = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "GetAllCompanies - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }
                con = new SqlConnection(connectionString);
                string query = "SELECT CompanyId,Flag,CName, IsActive FROM Company ORDER BY CName DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    companyList.Add(new Company
                    {
                        Id = Convert.ToInt32(reader["CompanyId"]),
                        Name = reader["CName"].ToString(),
                        Flag = reader["Flag"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"])
                    });
                }
                return Ok(companyList);
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during GetAllCompanies operation",userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (InvalidCastException castEx)
            {
                common.LogError(castEx, "Data conversion error during GetAllCompanies operation", userId);
                return InternalServerError(new Exception("Data format error. Please contact support."));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during GetAllCompanies operation", userId);
                return InternalServerError(new Exception("Unable to retrieve supplier list. Please try again later."));
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