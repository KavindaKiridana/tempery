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
    public partial class ManageUser : System.Web.UI.Page
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

            if (string.IsNullOrEmpty(txtUserName.Text.Trim()) || string.IsNullOrEmpty(txtPassword.Text.Trim()) || string.IsNullOrEmpty(txtFullName.Text.Trim()) || string.IsNullOrEmpty(ddlAutherizer.SelectedValue) || string.IsNullOrEmpty(ddlIsHead.SelectedValue))
            {
                lblAlert.Text = "Please enter all fields.";
                lblAlert.CssClass = "text-danger";
            }
            try
            {
                checkconnection();
                sqlconn.Open();
                string query = "INSERT INTO Users (Password, IsActive, UserName, FullName, IsHeadOrNot, IsAuthorizer) " +
                                       "VALUES (@Password, @IsActive, @UserName, @FullName, @IsHeadOrNot, @IsAuthorizer)";
                using (SqlCommand command = new SqlCommand(query, sqlconn))
                {
                    // Add parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());
                    command.Parameters.AddWithValue("@IsActive", true); // New user creation, set IsActive to true
                    command.Parameters.AddWithValue("@UserName", txtUserName.Text.Trim());
                    command.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim());
                    // Convert ddlIsHead.SelectedValue to a boolean (assuming "True" or "False" strings)
                    bool isHead = Convert.ToBoolean(ddlIsHead.SelectedValue);
                    command.Parameters.AddWithValue("@IsHeadOrNot", isHead);
                    command.Parameters.AddWithValue("@IsAuthorizer", ddlAutherizer.SelectedValue.Trim());

                    command.ExecuteNonQuery();
                    lblAlert.Text = "User added successfully.";
                    lblAlert.CssClass = "text-success";
                    sqlconn.Close();

                    // Clear the form fields
                    txtUserName.Text = "";
                    txtPassword.Text = "";
                    txtFullName.Text = "";
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