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
    public class LogHistoryController : ApiController
    {
        private readonly Comman common = new Comman();

        [HttpGet]
        [Route("api/LogHistory/{assetId}")]
        public IHttpActionResult ViewHistory(string assetId)
        {
            // Validate assetId
            if (string.IsNullOrEmpty(assetId) || assetId == "0")
            {
                return BadRequest("AssetId is required and cannot be null, empty, or zero.");
            }

            int userId = common.GetUserId((ClaimsPrincipal)User);
            if (userId == 0)
            {
                return Unauthorized();
            }

            List<PostTransaction> transactions = new List<PostTransaction>();
            SqlConnection con = null;
            SqlDataReader reader = null;

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "ViewHistory - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                con = new SqlConnection(connectionString);
                con.Open();

                // Query to get all transactions for the given AssetId
                string query = @"
                    SELECT 
                        t.TransactionId,
                        t.AssetId,
                        t.Type,
                        t.Time,
                        t.EditedUser,
                        u.FullName AS EditedUserFullName,
                        t.FromId, 
                        t.ToId,
                        t.RelatedAssetId, 
                        t.Note
                    FROM Transactions t
                    INNER JOIN Users u ON t.EditedUser = u.UsersId
                    WHERE t.AssetId = @AssetId
                    ORDER BY t.Time ASC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@AssetId", assetId);
                cmd.CommandTimeout = 30;

                reader = cmd.ExecuteReader();

                // First, read all transaction data into memory
                List<PostTransaction> tempTransactions = new List<PostTransaction>();

                while (reader.Read())
                {
                    PostTransaction transaction = new PostTransaction
                    {
                        Type = reader["Type"].ToString(),
                        Time = Convert.ToDateTime(reader["Time"]).ToString("yyyy-MM-dd HH:mm:ss"),
                        EditedUser = Convert.ToInt32(reader["EditedUser"]),
                        EditedUserFullName = reader["EditedUserFullName"].ToString(),
                        FromId = reader["FromId"] != DBNull.Value ? Convert.ToInt32(reader["FromId"]) : (int?)null,
                        ToId = reader["ToId"] != DBNull.Value ? Convert.ToInt32(reader["ToId"]) : (int?)null,
                        RelatedAssetId = reader["RelatedAssetId"] != DBNull.Value ? reader["RelatedAssetId"].ToString() : null,
                        Note = reader["Note"] != DBNull.Value ? reader["Note"].ToString() : null
                    };

                    tempTransactions.Add(transaction);
                }

                // Close the reader before executing additional queries
                reader.Close();

                // Now populate FromName and ToName for each transaction
                foreach (var transaction in tempTransactions)
                {
                    transaction.AssetId = assetId;
                    GetTransactionNames(con, transaction);
                    transactions.Add(transaction);
                }

                return Ok(transactions);
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during ViewHistory operation", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during ViewHistory operation", userId);
                return InternalServerError(new Exception("Unable to retrieve transaction history. Please try again later."));
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

        [NonAction]
        private void GetTransactionNames(SqlConnection con, PostTransaction transaction)
        {
            // Logic to populate FromName and ToName based on transaction type
            if (transaction.RelatedAssetId != null)
            {
                transaction.RelatedAssetName = GetAssetName(con, transaction.RelatedAssetId);
            }
            // ADD_NEW_ASSET_TO_STORE: FromId = SupplierId, ToId = LocationId
            if (transaction.Type == "ADD_NEW_ASSET_TO_STORE")
            {
                if (transaction.FromId.HasValue)
                {
                    transaction.FromName = GetSupplierName(con, transaction.FromId.Value);
                }
                if (transaction.ToId.HasValue)
                {
                    transaction.ToName = GetLocationName(con, transaction.ToId.Value);
                }
            }
            // ASSET_LOCATION_CHANGED: FromId = LocationId, ToId = LocationId
            else if (transaction.Type == "ASSET_LOCATION_CHANGED")
            {
                if (transaction.FromId.HasValue)
                {
                    transaction.FromName = GetLocationName(con, transaction.FromId.Value);
                }
                if (transaction.ToId.HasValue)
                {
                    transaction.ToName = GetLocationName(con, transaction.ToId.Value);
                }
            }
            // ASSET_ASSIGNED_TO_USER: FromId = LocationId, ToId = UserId
            else if (transaction.Type == "ASSET_ASSIGNED_TO_USER")
            {
                if (transaction.FromId.HasValue)
                {
                    transaction.FromName = GetLocationName(con, transaction.FromId.Value);
                }
                if (transaction.ToId.HasValue)
                {
                    transaction.ToName = GetUserName(con, transaction.ToId.Value);
                }
            }
            // ASSET_REMOVE_FROM_USER: FromId = UserId, ToId = LocationId
            else if (transaction.Type == "ASSET_REMOVE_FROM_USER")
            {
                if (transaction.FromId.HasValue)
                {
                    transaction.FromName = GetUserName(con, transaction.FromId.Value);
                }
                if (transaction.ToId.HasValue)
                {
                    transaction.ToName = GetLocationName(con, transaction.ToId.Value);
                }
            }
            // ASSET_DESTROYED or ASSET_LOST_OR_STOLEN with a last user: FromId = UserId
            else if (transaction.Type == "ASSET_LOST_STOLEN_FROM_USER" ||
                     transaction.Type == "ASSET_DESTROYED_FROM_USER")
            {
                if (transaction.FromId.HasValue)
                {
                    transaction.FromName = GetLocationName(con, transaction.FromId.Value);
                }
                transaction.ToName = null; // ToId is null for these types
            }
            // ASSET_DESTROYED or ASSET_LOST_OR_STOLEN with a last stock: FromId = locationId
            else if (transaction.Type == "ASSET_LOST_STOLEN_FROM_STOCK" ||
                     transaction.Type == "ASSET_DESTROYED_FROM_STOCK")
            {
                if (transaction.FromId.HasValue)
                {
                    transaction.FromName = GetLocationName(con, transaction.FromId.Value);
                }
                transaction.ToName = null; // ToId is null for these types
            }
            // ADD_COMPLAIN: No FromId or ToId expected
            else if (transaction.Type == "ADD_COMPLAIN")
            {
                transaction.FromName = null;
                transaction.ToName = null;
            }
            //GIVEN_TO_REAPAIR, STILL_IN_REPAIR: ToName=SupplierId, FromName=null
            else if (transaction.Type == "GIVEN_TO_REAPAIR" || transaction.Type == "STILL_IN_REPAIR")
            {
                transaction.ToName = GetSupplierName(con, transaction.ToId.Value);
                transaction.FromName = null;
            }
            else if (transaction.Type == "RETURNED_FROM_REPAIR")
            {
                transaction.ToName = null;
                transaction.FromName = GetSupplierName(con, transaction.FromId.Value);
            }
            // USER_LOCATION_CHANGED both ToId and FromId are LocationIds
            else if (transaction.Type == "USER_LOCATION_CHANGED")
            {
                transaction.ToName = GetLocationName(con, transaction.ToId.Value);
                transaction.FromName = GetLocationName(con, transaction.FromId.Value);
            }
            else if (transaction.Type == "SPAREPART_DESTROYED" ||
                     transaction.Type == "SPAREPART_LOST_STOLEN")
            {
                transaction.ToName = null; // ToId is null for these types
                transaction.RelatedAssetName = GetAssetName(con, transaction.RelatedAssetId);

            }
            else if (transaction.Type == "USER_RESIGNED")
            {
                transaction.ToName = GetLocationName(con, transaction.ToId.Value);
                transaction.FromName = GetUserName(con, transaction.FromId.Value);
            }
        }

        [NonAction]
        private string GetAssetName(SqlConnection con, string assetId)
        {
            string query = @"
    SELECT Name
    FROM Asset
    WHERE AssetId = @AssetId";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", assetId);
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    return $"{assetId} : {result}";
                }
                else
                {
                    return null;
                }
            }
        }

        [NonAction]
        private string GetSupplierName(SqlConnection con, int supplierId)
        {
            string query = "SELECT SName FROM Supplier WHERE SupplierId = @SupplierId";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@SupplierId", supplierId);
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : null;
            }
        }

        [NonAction]
        private string GetLocationName(SqlConnection con, int locationId)
        {
            string query = "SELECT LName FROM Location WHERE LocationId = @LocationId";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@LocationId", locationId);
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : null;
            }
        }

        [NonAction]
        private string GetUserName(SqlConnection con, int userId)
        {
            string query = "SELECT FullName FROM Users WHERE UsersId = @UserId";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : null;
            }
        }
    }
}