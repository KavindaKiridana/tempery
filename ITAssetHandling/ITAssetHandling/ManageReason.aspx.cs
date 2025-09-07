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
    public partial class ManageSupplier : System.Web.UI.Page
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

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtRName.Text.Trim()))
            {
                lblAlert.Text = "Please enter the reason.";
                lblAlert.CssClass = "text-danger";
            }
            try
            {
                checkconnection();
                sqlconn.Open();
                string query = " insert into Reason (RName) values (@RName)";
                using (SqlCommand command = new SqlCommand(query, sqlconn))
                {
                    command.Parameters.AddWithValue("@RName", txtRName.Text.Trim());
                    command.ExecuteNonQuery();
                    lblAlert.Text = "Reason added successfully.";
                    lblAlert.CssClass = "text-success";
                    sqlconn.Close();

                    // Clear the form fields
                    txtRName.Text = "";
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