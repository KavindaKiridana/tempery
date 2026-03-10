using AssetAPIs.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Web.Http;

namespace AssetAPIs.Controllers
{
    [JwtAuthentication]
    public class InstalledSoftwaresController : ApiController
    {
        private readonly Comman common = new Comman();
        public class InstalledSoftwaresRequest
        {
            public string AssetId { get; set; }
            public List<int> SoftwareIds { get; set; }
        }

        public class InstalledSoftwaresResponse
        {
            public int InsertedCount { get; set; }
            public int SkippedCount { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PostInstalledSoftwares(InstalledSoftwaresRequest request)
        {
            int userId = common.GetUserId((ClaimsPrincipal)User);

            if (string.IsNullOrEmpty(request.AssetId) || request.SoftwareIds == null || request.SoftwareIds.Count == 0)
            {
                return BadRequest("AssetId and SoftwareIds are required and SoftwareIds must not be empty.");
            }

            int insertedCount = 0;
            int skippedCount = 0;
            SqlConnection con = null;
            SqlTransaction transaction = null;

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    return InternalServerError(new Exception("Database configuration error: Connection string is missing."));
                }

                con = new SqlConnection(connectionString);
                con.Open();
                transaction = con.BeginTransaction();

                string query = @"
                    IF NOT EXISTS (
                        SELECT 1 FROM InstalledSoftwares
                        WHERE AssetId = @AssetId AND SoftwareId = @SoftwareId
                    )
                    BEGIN
                        INSERT INTO InstalledSoftwares (AssetId, SoftwareId, IsActive)
                        VALUES (@AssetId, @SoftwareId, 1);
                        SELECT 1;
                    END
                    ELSE
                    BEGIN
                        SELECT 0;
                    END
                ";

                SqlCommand cmd = new SqlCommand(query, con, transaction);
                cmd.Parameters.Add(new SqlParameter("@AssetId", request.AssetId));

                var softwareIdParam = cmd.Parameters.Add("@SoftwareId", System.Data.SqlDbType.Int);

                foreach (int softwareId in request.SoftwareIds)
                {
                    softwareIdParam.Value = softwareId;
                    int result = (int)cmd.ExecuteScalar();
                    if (result == 1)
                    {
                        insertedCount++;
                    }
                    else
                    {
                        skippedCount++;
                    }
                }

                transaction.Commit();
                return Ok(new InstalledSoftwaresResponse
                {
                    InsertedCount = insertedCount,
                    SkippedCount = skippedCount
                });
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "Database error occurred while saving installed software.",userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (InvalidCastException castEx)
            {
                common.LogError(castEx, "Data conversion error while saving installed software.", userId);
                return InternalServerError(new Exception("Data format error. Please contact support."));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error while saving installed software.", userId);
                return InternalServerError(new Exception("Unable to retrieve supplier list. Please try again later."));
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
