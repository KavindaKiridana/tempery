using System;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Web;

namespace AssetAPIs
{
    public class Comman
    {
        public void AddTransaction(SqlConnection con, string AssetId, int editedUserId, int? FromId, int? ToId, string Type, string note,  string RelatedAssetId, decimal? cost , bool? IsTempAssigned = null)
        {
            string query = @"
        INSERT INTO Transactions (AssetId, EditedUser, Type, FromId, ToId, Time, Note,RelatedAssetId,RepairCost,IsTempAssigned)
        VALUES (@AssetId, @EditedUser, @Type, @FromId, @ToId, @Time, @Note,@RelatedAssetId, @RepairCost, @IsTempAssigned)";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AssetId", AssetId);
                cmd.Parameters.AddWithValue("@EditedUser", editedUserId);
                cmd.Parameters.AddWithValue("@Type", Type);
                cmd.Parameters.AddWithValue("@FromId", FromId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ToId", ToId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Time", DateTime.Now);
                cmd.Parameters.AddWithValue("@Note", string.IsNullOrWhiteSpace(note) ? (object)DBNull.Value : note);
                cmd.Parameters.AddWithValue("@RelatedAssetId", RelatedAssetId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@RepairCost", cost ?? (object)DBNull.Value); 
                cmd.Parameters.AddWithValue("@IsTempAssigned", IsTempAssigned ?? (object)DBNull.Value); 
                cmd.ExecuteNonQuery();
            }
        }

        // Method to get user-friendly SQL error messages
        public string GetSqlErrorMessage(int errorNumber)
        {
            if (errorNumber == -1 || errorNumber == -2)
            {
                return "Database connection timeout. Please try again.";
            }
            else if (errorNumber == 53)
            {
                return "Database service unavailable.";
            }
            else if (errorNumber == 18456)
            {
                return "Authentication failed.";
            }
            else if (errorNumber == 26)
            {
                return "Database server not found. Please contact support.";
            }
            else if (errorNumber == 547)
            {
                return "This operation cannot be completed because it would violate data relationships. Please check your data and try again.";
            }
            else if (errorNumber == 2627)
            {
                return "This record already exists. Please use a different value.";
            }
            else if (errorNumber == 2601)
            {
                return "A duplicate entry was detected. Please enter unique information.";
            }
            else if (errorNumber == 515)
            {
                return "Required information is missing. Please fill in all mandatory fields.";
            }
            else if (errorNumber == 8152 || errorNumber == 8115)
            {
                return "The data entered is too long or in an incorrect format. Please check your input.";
            }
            else if (errorNumber == 1205)
            {
                return "The system is busy. Please wait a moment and try again.";
            }
            else
            {
                return "Unable to complete your request. Please try again later.";
            }
        }

        //find userId from api key
        //In Comman class
        public int GetUserId(ClaimsPrincipal user)
        {
            if (user == null)
            {
                return 0;
            }

            var userIdClaim =
                user.FindFirst("UserId") ??
                user.FindFirst(ClaimTypes.NameIdentifier) ??
                user.FindFirst("sub");

            if (userIdClaim == null)
            {
                return 0;
            }

            return int.Parse(userIdClaim.Value);
        }

        // Method to log errors to a file
        public void LogError(Exception ex, string context, int? EditedUserId = null)
        {
            try
            {
                // Log to App_Data folder (safe and standard location)
                string logDirectory;
                if (HttpContext.Current != null)
                {
                    logDirectory = HttpContext.Current.Server.MapPath("~/App_Data");
                }
                else
                {
                    // Fallback for when HttpContext is not available
                    logDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
                }

                string logPath = System.IO.Path.Combine(logDirectory, "errors.log");

                // Create directory if it doesn't exist
                if (!System.IO.Directory.Exists(logDirectory))
                {
                    System.IO.Directory.CreateDirectory(logDirectory);
                }

                string userInfo = EditedUserId.HasValue ? EditedUserId.ToString() : "Edited User Not Found";

                // Format the error message
                string logMessage = string.Format(
                    "\n{0}\nTime: {1}\nContext: {2}\nUser: {3}\nError Type: {4}\nMessage: {5}\nStack Trace: {6}\n{7}\n",
                    new string('=', 80),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    context,
                    userInfo,
                    ex.GetType().Name,
                    ex.Message,
                    ex.StackTrace,
                    new string('=', 80)
                );

                // Write to file
                System.IO.File.AppendAllText(logPath, logMessage);
            }
            catch (Exception logEx)
            {
                // If file logging fails, write to debug output as last resort
                System.Diagnostics.Debug.WriteLine("CRITICAL: Logging failed - " + logEx.Message);
                System.Diagnostics.Debug.WriteLine("Original error: " + ex.Message);
            }
        }
    }

}