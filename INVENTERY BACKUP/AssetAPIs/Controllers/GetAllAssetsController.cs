using AssetAPIs.Filters;
using AssetAPIs.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Web.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using ClosedXML.Excel;
using System.IO;

namespace AssetAPIs.Controllers
{
    [JwtAuthentication]
    public class GetAllAssetsController : ApiController
    {
        private readonly Comman common = new Comman();

        [HttpGet]
        public IHttpActionResult ViewAssets(bool isExport, string search = "")
        {
            List<AssetTableView> assetList = new List<AssetTableView>();
            SqlConnection con = null;
            SqlDataReader reader = null;

            int userId = common.GetUserId((ClaimsPrincipal)User);

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    common.LogError(new Exception("Connection string is null or empty"), "ViewAssets - Configuration Error", userId);
                    return InternalServerError(new Exception("Database configuration error"));
                }

                string query = @"
SELECT
    a.AssetId,
    a.Type,
    a.Name,
    c.CName as Crop,
    l.LName as Geo,
    a.ManufactureSN,
    CASE
        WHEN st.Quantity <= 0 THEN 0
        ELSE 1
    END AS IsActive,
    -- Get the current active user's full name (if any)
    (
        SELECT TOP 1 u.FullName
        FROM AssetUsedBy aub
        INNER JOIN Users u ON aub.UsedBy = u.UsersId
        WHERE aub.AssetId = a.AssetId
        AND aub.IsActive = 1
    ) AS CurrentUser,
	-- IsAvailable logic: 0 if asset is used OR is a spare part, otherwise 1
    CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM AssetUsedBy aub 
            WHERE aub.AssetId = a.AssetId 
            AND aub.IsActive = 1
        ) THEN 0
        WHEN EXISTS (
            SELECT 1 
            FROM AssetSpareParts asp 
            WHERE asp.SparePartId = a.AssetId 
            AND asp.IsActive = 1
        ) THEN 0
        ELSE 1
    END AS IsAvailable
FROM
    Asset a
LEFT JOIN
    Stocks st ON a.AssetId = st.AssetId
LEFT JOIN
    Company c ON c.CompanyId = st.CompanyId
LEFT JOIN
    Location l ON l.LocationId = st.LocationId
WHERE
    (@search IS NULL OR @search = '')
    OR
    (
        a.AssetId LIKE '%' + @search + '%' OR
        a.Name LIKE '%' + @search + '%' OR
        c.CName LIKE '%' + @search + '%' OR
        l.LName LIKE '%' + @search + '%' OR
        a.Type LIKE '%' + @search + '%'  OR
        a.ManufactureSN LIKE '%' + @search + '%' OR
        (
            -- Search for CurrentUser by repeating the subquery logic
            EXISTS (
                SELECT 1
                FROM AssetUsedBy aub
                INNER JOIN Users u ON aub.UsedBy = u.UsersId
                WHERE aub.AssetId = a.AssetId
                AND aub.IsActive = 1
                AND u.FullName LIKE '%' + @search + '%'
            )
        )
        OR
        (
        -- Handle IsAvailable search
        (@search IN ('true', 'True', 'TRUE', '1', 'yes', 'Yes', 'YES', 'available', 'Available') 
            AND NOT EXISTS (
                SELECT 1 FROM AssetUsedBy aub 
                WHERE aub.AssetId = a.AssetId AND aub.IsActive = 1
            )
            AND NOT EXISTS (
                SELECT 1 FROM AssetSpareParts asp 
                WHERE asp.SparePartId = a.AssetId AND asp.IsActive = 1
            ))
        OR 
        (@search IN ('false', 'False', 'FALSE', '0', 'no', 'No', 'NO', 'unavailable', 'Unavailable')
            AND (
                EXISTS (
                    SELECT 1 FROM AssetUsedBy aub 
                    WHERE aub.AssetId = a.AssetId AND aub.IsActive = 1
                )
                OR EXISTS (
                    SELECT 1 FROM AssetSpareParts asp 
                    WHERE asp.SparePartId = a.AssetId AND asp.IsActive = 1
                )
            ))
   )
)
ORDER BY a.AssetId";

                con = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand(query, con);
                string searchTerm = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
                cmd.Parameters.AddWithValue("@search", (object)searchTerm ?? DBNull.Value);
                con.Open();
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    assetList.Add(new AssetTableView
                    {
                        AssetId = reader["AssetId"].ToString(),
                        Type = reader["Type"].ToString(),
                        Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : null,
                        CompanyName = reader["Crop"] != DBNull.Value ? reader["Crop"].ToString() : null,
                        LocationName = reader["Geo"] != DBNull.Value ? reader["Geo"].ToString() : null,
                        ManufactureSN = reader["ManufactureSN"] != DBNull.Value ? reader["ManufactureSN"].ToString() : null,
                        CurrentUser= reader["CurrentUser"] != DBNull.Value ? reader["CurrentUser"].ToString() : null,
                        IsActive = Convert.ToBoolean(reader["IsActive"]),
                        IsAvailable = Convert.ToBoolean(reader["IsAvailable"])
                    });
                }

                // If export is requested, generate Excel file
                if (isExport)
                {
                    return GenerateExcelFile(assetList, search);
                }

                return Ok(assetList);
            }
            catch (SqlException sqlEx)
            {
                common.LogError(sqlEx, "SQL Error during ViewAssets operation", userId);
                string userMessage = common.GetSqlErrorMessage(sqlEx.Number);
                return InternalServerError(new Exception(userMessage));
            }
            catch (InvalidCastException castEx)
            {
                common.LogError(castEx, "Data conversion error during ViewAssets operation", userId);
                return InternalServerError(new Exception("Data format error. Please contact support."));
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Unexpected error during ViewAssets operation", userId);
                return InternalServerError(new Exception("Unable to retrieve asset list. Please try again later."));
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

        private IHttpActionResult GenerateExcelFile(List<AssetTableView> assetList, string search)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Assets");

                    // Add headers
                    worksheet.Cell(1, 1).Value = "Asset ID";
                    worksheet.Cell(1, 2).Value = "Type";
                    worksheet.Cell(1, 3).Value = "Name";
                    worksheet.Cell(1, 4).Value = "Company";
                    worksheet.Cell(1, 5).Value = "Location"; 
                    worksheet.Cell(1, 6).Value = "IsAvailable";
                    worksheet.Cell(1, 7).Value = "Is Active";

                    // Style headers
                    var headerRange = worksheet.Range(1, 1, 1, 7);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                    // Add data
                    for (int i = 0; i < assetList.Count; i++)
                    {
                        var asset = assetList[i];
                        int row = i + 2;

                        worksheet.Cell(row, 1).Value = asset.AssetId;
                        worksheet.Cell(row, 2).Value = asset.Type;
                        worksheet.Cell(row, 3).Value = asset.Name ?? "";
                        worksheet.Cell(row, 4).Value = asset.CompanyName ?? "";
                        worksheet.Cell(row, 5).Value = asset.LocationName ?? "";
                        worksheet.Cell(row, 6).Value = asset.IsAvailable ? "Yes" : "No";
                        worksheet.Cell(row, 7).Value = asset.IsActive ? "Yes" : "No";
                    }

                    // Auto-fit columns
                    worksheet.Columns().AdjustToContents();

                    // Generate file
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Position = 0;

                        var result = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ByteArrayContent(stream.ToArray())
                        };

                        string fileName = string.IsNullOrWhiteSpace(search)
                            ? $"Assets_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                            : $"Assets_Filtered_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = fileName
                        };

                        return ResponseMessage(result);
                    }
                }
            }
            catch (Exception ex)
            {
                common.LogError(ex, "Error generating Excel file", common.GetUserId((ClaimsPrincipal)User));
                return InternalServerError(new Exception("Failed to generate Excel file"));
            }
        }
    }
}