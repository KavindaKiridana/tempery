using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MilkWeb
{
    public partial class Contact : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["centerName"] == null)
                {
                    Response.Redirect("Default.aspx");
                }
                lblCenterOption.Text = Session["centerName"].ToString();
                lblCollector.Visible = false;
                ddlCollector.Visible = false;
                limitDateInput();
            }
        }

        protected void limitDateInput()
        {
            int backDays = Convert.ToInt32(Session["backDays"]);

            // Calculate the minimum and maximum allowed dates
            DateTime today = DateTime.Today;
            DateTime minDate = today.AddDays(-backDays);
            DateTime maxDate = today;

            // Set the date attributes for validation
            txtDate.Attributes["min"] = minDate.ToString("yyyy-MM-dd");
            txtDate.Attributes["max"] = maxDate.ToString("yyyy-MM-dd");

            // Set today's date as the default value
            txtDate.Text = today.ToString("yyyy-MM-dd");
        }

        protected void rblEntity_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected value from the RadioButtonList
            string selectedValue = rblEntity.SelectedValue;
            int CCode = Convert.ToInt32(Session["CCode"]);
            lblCollector.Visible = true;
            ddlCollector.Visible = true;
            ddlCollector.Items.Clear();
            ddlCollector.Items.Add(new ListItem("-- Select --", "null"));

            if (selectedValue == "Societies")
            {
                lblCollector.Text = "Society";
                string sqlSocieties = "SELECT (SocietyName + ' - ' + SocietyCode) AS DisplayText, SocietyCode FROM tblDSociety WHERE CCode = @CCode";
                DataTable dtSocieties = GetData(sqlSocieties, CCode);

                if (dtSocieties.Rows.Count > 0)
                {
                    ddlCollector.DataTextField = "DisplayText"; // Shows "Name - Code"
                    ddlCollector.DataValueField = "SocietyCode";
                    ddlCollector.DataSource = dtSocieties;
                    ddlCollector.DataBind();

                    string parentSelection = dtSocieties.Rows[0]["SocietyCode"].ToString();
                    displayFarmersList(parentSelection);
                }
            }
            else if (selectedValue == "Collectors")
            {
                lblCollector.Text = "Collector";
                string sqlCollectors = "SELECT (CollectorName + ' - ' + CollectorCode) AS DisplayText, CollectorCode FROM tblDCollectors WHERE CCode = @CCode";
                DataTable dtCollectors = GetData(sqlCollectors, CCode);

                if (dtCollectors.Rows.Count > 0)
                {
                    ddlCollector.DataTextField = "DisplayText";
                    ddlCollector.DataValueField = "CollectorCode";
                    ddlCollector.DataSource = dtCollectors;
                    ddlCollector.DataBind();

                    string parentSelection = dtCollectors.Rows[0]["CollectorCode"].ToString();
                    displayFarmersList(parentSelection);
                }
            }
            else if (selectedValue == "Direct Farmers")
            {
                lblCollector.Visible = false;
                ddlCollector.Visible = false;
                string parentSelection = "S0000";
                displayFarmersList(parentSelection);
            }
        }

        protected void ddlCollector_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected value from the DropDownList
            string parentSelection = ddlCollector.SelectedValue;
            displayFarmersList(parentSelection);
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

        protected void displayFarmersList(string parentSelection)
        {
            //write code here as when this function called, 
        }

        //protected void FilterOptions(object sender, EventArgs e)
        //{
        //    string centerCode = Session["centerCode"] != null ? Session["centerCode"].ToString() : "";
        //    int centerId = Session["CCode"] != null ? Convert.ToInt32(Session["CCode"]) : 0;
        //    int backDays = Session["backDays"] != null ? Convert.ToInt32(Session["backDays"]) : 0;
        //}
    }
}