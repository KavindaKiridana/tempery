using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace MilkWeb
{
    public partial class About : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["centerName"] == null)
                {
                    Response.Redirect("Default.aspx");
                }
                lblSociety.Visible = false;
                ddlSociety.Visible = false;
                lblCenterOption.Text = Session["centerName"].ToString();
                fromToDateInputs();

                // Don't load GridView data on initial page load
                GridView1.Visible = false;
            }
        }

        protected void fromToDateInputs()
        {
            // Set the date attributes dynamically
            var today = DateTime.Today;
            var minDate = today.AddDays(-17).ToString("yyyy-MM-dd");
            var maxDate = today.ToString("yyyy-MM-dd");
            var currentDate = today.ToString("yyyy-MM-dd");

            // Set HTML5 attributes for validation
            txtFromDate.Attributes["min"] = minDate;
            txtFromDate.Attributes["max"] = maxDate;
            txtToDate.Attributes["min"] = minDate;
            txtToDate.Attributes["max"] = maxDate;

            // Set the actual values to display in the textboxes
            txtFromDate.Text = currentDate;
            txtToDate.Text = currentDate;
        }

        protected void rblFilterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Capture selected option into variable
            string radioOption = rblFilterType.SelectedValue;
            int CCode = Convert.ToInt32(Session["CCode"]);

            lblSociety.Visible = true;
            ddlSociety.Visible = true;
            ddlSociety.Items.Clear();
            ddlSociety.Items.Add(new ListItem("-- Select --", "null"));

            if (radioOption == "Society")
            {
                lblSociety.Text = "Society";

                string sqlSocieties = "SELECT SocietyName, SocietyCode FROM tblDSociety WHERE CCode = @CCode";
                DataTable dtSocieties = GetData(sqlSocieties, CCode);

                if (dtSocieties.Rows.Count > 0)
                {
                    ddlSociety.DataTextField = "SocietyCode";
                    ddlSociety.DataValueField = "SocietyCode";
                    ddlSociety.DataSource = dtSocieties;
                    ddlSociety.DataBind();
                }
            }
            else if (radioOption == "Collector")
            {
                lblSociety.Text = "Collector";

                string sqlCollectors = "SELECT CollectorName, CollectorCode FROM tblDCollectors WHERE CCode = @CCode";
                DataTable dtCollectors = GetData(sqlCollectors, CCode);

                if (dtCollectors.Rows.Count > 0)
                {
                    ddlSociety.DataTextField = "CollectorCode";
                    ddlSociety.DataValueField = "CollectorCode";
                    ddlSociety.DataSource = dtCollectors;
                    ddlSociety.DataBind();
                }
            }
            else
            {
                lblSociety.Visible = false;
                ddlSociety.Visible = false;
            }

            // Hide GridView when filter type changes
            GridView1.Visible = false;
        }

        // Reusable method to fetch data from database
        private DataTable GetData(string query, int cCode)
        {
            DataTable dt = new DataTable();
            string connectionString = ConfigurationManager.ConnectionStrings["MilkWeb"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CCode", cCode);
                    try
                    {
                        conn.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                    catch (Exception ex)
                    {
                        // Log exception or handle appropriately in real application
                        throw ex;
                    }
                }
            }
            return dt;
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            // Validate and assign variables
            string entitySelection = rblFilterType.SelectedValue;
            string parent = string.Empty;
            //string fromDate = txtFromDate.Text;
            string fromDate = "2025-02-20";
            string toDate = txtToDate.Text;

            // Clear any previous error messages
            lblError.Text = "";

            // Validate date inputs
            if (string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate))
            {
                lblError.Text = "Please select both From and To dates.";
                lblError.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // Validate date range (ensure From date is not later than To date)
            DateTime fromDateTime = DateTime.Parse(fromDate);
            DateTime toDateTime = DateTime.Parse(toDate);

            if (fromDateTime > toDateTime)
            {
                lblError.Text = "From date cannot be later than To date.";
                lblError.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // Validate dropdown selection for Society and Collector options
            if (entitySelection == "Society" || entitySelection == "Collector")
            {
                if (ddlSociety.SelectedValue == "null" || string.IsNullOrEmpty(ddlSociety.SelectedValue))
                {
                    lblError.Text = "Please select a " + entitySelection + ".";
                    lblError.ForeColor = System.Drawing.Color.Red;
                    return;
                }
                parent = ddlSociety.SelectedValue;
            }

            // Load filtered data based on selection
            LoadGridViewData(entitySelection, parent, fromDate, toDate);
        }

        private void LoadGridViewData(string entitySelection, string parent, string fromDate, string toDate)
        {
            try
            {
                int CCode = Convert.ToInt32(Session["CCode"]);
                string connectionString = ConfigurationManager.ConnectionStrings["MilkWeb"].ConnectionString;

                // Build SQL query based on entity selection
                string sqlQuery = BuildSqlQuery(entitySelection);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {
                        // Add common parameters
                        cmd.Parameters.AddWithValue("@CCode", CCode);
                        cmd.Parameters.AddWithValue("@fromDate", fromDate);
                        cmd.Parameters.AddWithValue("@toDate", toDate);

                        // Add parent parameter for Society and Collector
                        if (entitySelection == "Society" || entitySelection == "Collector")
                        {
                            cmd.Parameters.AddWithValue("@Parent", parent);
                        }

                        conn.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        // Bind data to GridView
                        GridView1.DataSource = dt;
                        GridView1.DataBind();
                        GridView1.Visible = true;

                        if (dt.Rows.Count == 0)
                        {
                            lblError.Text = "No records found for the selected criteria.";
                            lblError.ForeColor = System.Drawing.Color.Orange;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error loading data: " + ex.Message;
                lblError.ForeColor = System.Drawing.Color.Red;
                GridView1.Visible = false;
            }
        }

        private string BuildSqlQuery(string entitySelection)
        {
            string baseQuery = @"SELECT T1.ID, T1.CCode, T1.FarmerCode, T1.Parent, T1.DateOf, T1.TimeOf, 
                                       T1.SNF, T1.FAT, T1.Quantity, T1.CreatedDate, T1.Accept, T2.OldCode
                                FROM tblDCollection AS T1
                                JOIN tblDFarmers AS T2 ON T1.FarmerCode = T2.FarmerCode
                                WHERE T1.CCode = @CCode 
                                  AND CAST(T1.DateOf AS DATE) >= CAST(@fromDate AS DATE) 
                                  AND CAST(T1.DateOf AS DATE) <= CAST(@toDate AS DATE)";

            switch (entitySelection)
            {
                case "Center":
                    // No additional filter needed for Center
                    break;
                case "Society":
                case "Collector":
                    baseQuery += " AND T1.Parent = @Parent";
                    break;
                case "Direct Farmer":
                    baseQuery += " AND T1.Parent = 'S0000'";
                    break;
            }

            baseQuery += " ORDER BY T1.ID DESC";
            return baseQuery;
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                // Get the ID of the row to delete
                int recordId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);

                // First verify that the record still has Accept = 0
                if (VerifyRecordCanBeDeleted(recordId))
                {
                    DeleteRecord(recordId);

                    // Refresh the GridView with current filter settings
                    RefreshGridView();

                    // Automatically trigger the View button click event
                    btnView_Click(btnView, EventArgs.Empty);

                    lblError.Text = "Record deleted successfully.";
                    lblError.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblError.Text = "Cannot delete this record. Only records with Accept = 0 can be deleted.";
                    lblError.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error deleting record: " + ex.Message;
                lblError.ForeColor = System.Drawing.Color.Red;
            }
        }

        private bool VerifyRecordCanBeDeleted(int recordId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MilkWeb"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string checkQuery = "SELECT Accept FROM tblDCollection WHERE ID = @ID";
                using (SqlCommand cmd = new SqlCommand(checkQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", recordId);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        int acceptValue = Convert.ToInt32(result);
                        return acceptValue == 0;
                    }
                }
            }
            return false;
        }

        private void DeleteRecord(int recordId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MilkWeb"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string deleteQuery = "DELETE FROM tblDCollection WHERE ID = @ID AND Accept = 0";
                using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", recordId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get the Accept value for this row
                int acceptValue = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "Accept"));

                // Find the delete button in this row
                LinkButton btnDelete = e.Row.FindControl("btnDelete") as LinkButton;

                if (btnDelete != null)
                {
                    // Show delete button only if Accept = 0
                    btnDelete.Visible = (acceptValue == 0);
                }
            }
        }

        private void RefreshGridView()
        {
            // Get current filter settings and reload data
            string entitySelection = rblFilterType.SelectedValue;
            string parent = string.Empty;
            string fromDate = txtFromDate.Text;
            string toDate = txtToDate.Text;

            if (entitySelection == "Society" || entitySelection == "Collector")
            {
                parent = ddlSociety.SelectedValue;
            }

            LoadGridViewData(entitySelection, parent, fromDate, toDate);
        }
    }
}