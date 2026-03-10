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
    public class ViewAssetController : ApiController
    {
        private readonly Comman common = new Comman();

        [HttpGet]
        [Route("api/ViewAsset/{assetId}")]
        public IHttpActionResult ViewAsset(string assetId)
        {
            int userId = common.GetUserId((ClaimsPrincipal)User);

            if (string.IsNullOrWhiteSpace(assetId))
                return BadRequest("Asset ID is required.");

            SqlConnection con = null;

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                    return InternalServerError(new Exception("Database configuration error"));

                con = new SqlConnection(connectionString);
                con.Open();

                EditAsset asset = GetAssetDetails(con, assetId);
                if (asset == null)
                    return NotFound();

                //asset.CompanyList = GetActiveCompanies(con);
                //asset.LocationList = GetActiveLocations(con);
                //asset.SupplierList = GetActiveSuppliers(con);
                //asset.OSList = GetActiveOS(con);
                //asset.ProcessorList = GetActiveProcessors(con);
                //asset.RAMSizeList = GetActiveRAMSizes(con);
                //asset.RAMTypeList = GetActiveRAMTypes(con);
                //asset.HDDList = GetActiveHDDs(con);
                //asset.SSDList = GetActiveSSDs(con);
                //asset.DisplayList = GetActiveDisplays(con);

                if (asset.Type == "Desktop" || asset.Type == "Laptop" || asset.Type == "Server")
                    asset.SoftwareList = GetInstalledSoftwares(con, assetId);

                return Ok(asset);
            }
            catch (Exception ex)
            {
                common.LogError(ex, "ViewAsset error", userId);
                return InternalServerError(new Exception("Unable to retrieve asset."));
            }
            finally
            {
                if (con != null && con.State == System.Data.ConnectionState.Open)
                    con.Close();
            }
        }

        // ========================= PRIVATE METHODS =========================

        [NonAction]
        private EditAsset GetAssetDetails(SqlConnection con, string assetId)
        {
            string query = @"
SELECT
    a.AssetId, a.Type, a.DoP, a.FinanceAssetCode, a.Warranty,
    st.CompanyId, c.CName,
    st.LocationId, l.LName,
    a.ManufactureSN, a.Brandnew, a.Cost, a.Name,
    a.SupplierId, s.SName,
    a.IPAddress, a.Note,
    a.OsId, os.OS,
    a.PId, p.Processor,
    a.RAMSId, rs.Size AS RAMSize,
    a.RAMTId, rt.Type AS RAMType,
    a.HDDId, h.HDD,
    a.SSDId, ssd.SSD,
    a.DisplayId, disp.Display,
    a.Make, a.WindowsKey, a.Motherboard, a.Model, m.Model as ModelName,
    a.PowerSupply, a.RAIDSupport
FROM Asset a
LEFT JOIN Model m ON m.ModelId = a.Model
LEFT JOIN Stocks st ON a.AssetId = st.AssetId
LEFT JOIN Company c ON st.CompanyId = c.CompanyId
LEFT JOIN Location l ON st.LocationId = l.LocationId
LEFT JOIN Supplier s ON a.SupplierId = s.SupplierId
LEFT JOIN OS os ON a.OsId = os.OsId
LEFT JOIN Processor p ON a.PId = p.PId
LEFT JOIN RAMSize rs ON a.RAMSId = rs.RAMSId
LEFT JOIN RAMType rt ON a.RAMTId = rt.RAMTId
LEFT JOIN HDD h ON a.HDDId = h.HDDId
LEFT JOIN SSD ssd ON a.SSDId = ssd.SSDId
LEFT JOIN Display disp ON a.DisplayId = disp.DisplayId
WHERE a.AssetId = @AssetId";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", assetId);

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;

                    return new EditAsset
                    {
                        AssetId = r["AssetId"].ToString(),
                        Type = r["Type"].ToString(),
                        DoP = r["DoP"] as DateTime?,
                        FinanceAssetCode = r["FinanceAssetCode"] as string,
                        Warranty = r["Warranty"] as int?,

                        CompanyId = r["CompanyId"] == DBNull.Value ? 0 : Convert.ToInt32(r["CompanyId"]),
                        CName = r["CName"] as string,

                        LocationId = r["LocationId"] == DBNull.Value ? 0 : Convert.ToInt32(r["LocationId"]),
                        LName = r["LName"] as string,

                        SupplierId = r["SupplierId"] == DBNull.Value ? 0 : Convert.ToInt32(r["SupplierId"]),
                        SName = r["SName"] as string,

                        ManufactureSN = r["ManufactureSN"] as string,
                        Brandnew = r["Brandnew"] as bool?,
                        Cost = r["Cost"] as decimal?,
                        Name = r["Name"] as string,
                        IPAddress = r["IPAddress"] as string,
                        Note = r["Note"] as string,

                        OsId = r["OsId"] == DBNull.Value ? 0 : Convert.ToInt32(r["OsId"]),
                        OS = r["OS"] as string,

                        PId = r["PId"] == DBNull.Value ? 0 : Convert.ToInt32(r["PId"]),
                        Processor = r["Processor"] as string,

                        RAMSId = r["RAMSId"] == DBNull.Value ? 0 : Convert.ToInt32(r["RAMSId"]),
                        RAMSize = r["RAMSize"] as string,

                        RAMTId = r["RAMTId"] == DBNull.Value ? 0 : Convert.ToInt32(r["RAMTId"]),
                        RAMType = r["RAMType"] as string,

                        HDDId = r["HDDId"] == DBNull.Value ? 0 : Convert.ToInt32(r["HDDId"]),
                        HDD = r["HDD"] as string,

                        SSDId = r["SSDId"] == DBNull.Value ? 0 : Convert.ToInt32(r["SSDId"]),
                        SSD = r["SSD"] as string,

                        DisplayId = r["DisplayId"] == DBNull.Value ? 0 : Convert.ToInt32(r["DisplayId"]),
                        Display = r["Display"] as string,

                        ModelId = r["Model"] == DBNull.Value ? 0 : Convert.ToInt32(r["Model"]),
                        Model = r["ModelName"] as string,

                        Make = r["Make"] as string,
                        WindowsKey = r["WindowsKey"] as string,
                        Motherboard = r["Motherboard"] as string,
                        PowerSupply = r["PowerSupply"] as bool?,
                        RAIDSupport = r["RAIDSupport"] as bool?
                    };
                }
            }
        }

        // ========================= LIST LOADERS =========================

        [NonAction] private List<Company> GetActiveCompanies(SqlConnection con) => LoadList(con, "SELECT CompanyId Id, CName Name FROM Company WHERE IsActive = 1", r => new Company { Id = (int)r["Id"], Name = r["Name"].ToString() });
        [NonAction] private List<Location> GetActiveLocations(SqlConnection con) => LoadList(con, "SELECT LocationId Id, LName Name FROM Location WHERE IsActive = 1", r => new Location { Id = (int)r["Id"], Name = r["Name"].ToString() });
        [NonAction] private List<Supplier> GetActiveSuppliers(SqlConnection con) => LoadList(con, "SELECT SupplierId Id, SName Name, Currency FROM Supplier WHERE IsActive = 1", r => new Supplier { Id = (int)r["Id"], Name = r["Name"].ToString(), Currency = r["Currency"] as string });
        [NonAction] private List<OS> GetActiveOS(SqlConnection con) => LoadList(con, "SELECT OsId Id, OS Name FROM OS WHERE IsActive = 1", r => new OS { Id = (int)r["Id"], Name = r["Name"].ToString() });
        [NonAction] private List<Processor> GetActiveProcessors(SqlConnection con) => LoadList(con, "SELECT PId Id, Processor Name FROM Processor WHERE IsActive = 1", r => new Processor { Id = (int)r["Id"], Name = r["Name"].ToString() });
        [NonAction] private List<RAMSize> GetActiveRAMSizes(SqlConnection con) => LoadList(con, "SELECT RAMSId Id, Size FROM RAMSize WHERE IsActive = 1", r => new RAMSize { Id = (int)r["Id"], Name = r["Size"].ToString() });
        [NonAction] private List<RAMType> GetActiveRAMTypes(SqlConnection con) => LoadList(con, "SELECT RAMTId Id, Type FROM RAMType WHERE IsActive = 1", r => new RAMType { Id = (int)r["Id"], Name = r["Type"].ToString() });
        [NonAction] private List<HDD> GetActiveHDDs(SqlConnection con) => LoadList(con, "SELECT HDDId Id, HDD FROM HDD WHERE IsActive = 1", r => new HDD { Id = (int)r["Id"], Name = r["HDD"].ToString() });
        [NonAction] private List<SSD> GetActiveSSDs(SqlConnection con) => LoadList(con, "SELECT SSDId Id, SSD FROM SSD WHERE IsActive = 1", r => new SSD { Id = (int)r["Id"], Name = r["SSD"].ToString() });
        [NonAction] private List<Display> GetActiveDisplays(SqlConnection con) => LoadList(con, "SELECT DisplayId Id, Display FROM Display WHERE IsActive = 1", r => new Display { Id = (int)r["Id"], Name = r["Display"].ToString() });

        [NonAction]
        private List<T> LoadList<T>(SqlConnection con, string query, Func<SqlDataReader, T> map)
        {
            List<T> list = new List<T>();
            using (SqlCommand cmd = new SqlCommand(query, con))
            using (SqlDataReader r = cmd.ExecuteReader())
                while (r.Read()) list.Add(map(r));
            return list;
        }

        [NonAction]
        private List<Softwares> GetInstalledSoftwares(SqlConnection con, string assetId)
        {
            List<Softwares> list = new List<Softwares>();
            string query = @"SELECT s.SoftwareId, s.SoftwareName, isw.IsActive
                             FROM InstalledSoftwares isw
                             JOIN Software s ON isw.SoftwareId = s.SoftwareId
                             WHERE isw.AssetId = @AssetId AND s.IsActive = 1";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", assetId);
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new Softwares
                        {
                            Id = (int)r["SoftwareId"],
                            Name = r["SoftwareName"].ToString(),
                            IsActive = (bool)r["IsActive"]
                        });
                    }
                }
            }
            return list;
        }
    }
}
