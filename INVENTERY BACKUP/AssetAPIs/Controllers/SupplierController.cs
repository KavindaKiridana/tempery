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
    public class SupplierController : ApiController
    {
        private readonly Comman common = new Comman();

        // GET: api/Suppliers
        [HttpGet]
        public IHttpActionResult GetAllSuppliers()
        {
            List<Supplier> supplierList = new List<Supplier>();
            SqlConnection con = null;
            SqlDataReader reader = null;
            int userId = common.GetUserId((ClaimsPrincipal)User);
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "GetAllSuppliers - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }
                con = new SqlConnection(connectionString);
                string query = "SELECT SupplierId, SName, Currency, IsActive FROM Supplier ORDER BY SName DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 30;
                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    supplierList.Add(new Supplier
                    {
                        Id = Convert.ToInt32(reader["SupplierId"]),
                        Name = reader["SName"].ToString(),
                        Currency = reader["Currency"] != DBNull.Value ? reader["Currency"].ToString() : null,
                        IsActive = Convert.ToBoolean(reader["IsActive"])
                    });
                }
                return Ok(supplierList);
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during GetAllSuppliers operation", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (InvalidCastException castEx)
            {
                common.LogError(castEx, "Data conversion error during GetAllSuppliers operation", userId);
                return InternalServerError(new Exception("Data format error. Please contact support."));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during GetAllSuppliers operation", userId);
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