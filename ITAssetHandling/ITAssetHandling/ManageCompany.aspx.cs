using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ITAssetHandling
{
    public partial class ManageCompany : System.Web.UI.Page
    {
        //sqlconnection
        SqlConnection sqlconn = new SqlConnection(ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            string[] allowedRoles = { "Editor", "IT Manager" };

            if (Session["UserId"] == null ||
                string.IsNullOrEmpty(Session["UserId"].ToString()) ||
                Session["IsAuthorizer"] == null ||
                !allowedRoles.Contains(Session["IsAuthorizer"].ToString()))
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        // Triggered after GridView update
        protected void GridView1_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            if (e.Exception == null)
            {
                // Get the CompanyId of the updated record
                int companyId = Convert.ToInt32(GridView1.DataKeys[e.AffectedRows > 0 ? 0 : 0].Value);
                validateTemplete(companyId);
            }
        }

        protected void validateTemplete(int companyId)
        {
            //after user update any record's IsActive attribute, here you can write the code for automatically update the relevant template
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCName.Text.Trim()) || string.IsNullOrEmpty(txtFlag.Text.Trim()))
            {
                lblAlert.Text = "Please enter both name and flag.";
                lblAlert.CssClass = "text-danger";
            }
            try
            {
                checkconnection();
                sqlconn.Open();
                string query = " insert into Company (CName,Flag) values (@companyName,@flag)";
                using (SqlCommand command = new SqlCommand(query, sqlconn))
                {
                    command.Parameters.AddWithValue("@companyName", txtCName.Text.Trim());
                    command.Parameters.AddWithValue("@flag", txtFlag.Text.Trim());
                    command.ExecuteNonQuery();
                    lblAlert.Text = "Company added successfully.";
                    lblAlert.CssClass = "text-success";
                    sqlconn.Close();

                    // Clear the form fields
                    txtCName.Text = "";
                    txtFlag.Text = "";
                    // Refresh the GridView to show the new data
                    GridView1.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblAlert.Text = ex.Message;
                lblAlert.CssClass = "text-warning";
            }
        }

        public void checkconnection()
        {
            if (sqlconn.State == ConnectionState.Open)
            {
                sqlconn.Close();
            }
        }
    }
}



