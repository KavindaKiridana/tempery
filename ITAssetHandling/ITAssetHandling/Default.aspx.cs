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
    public partial class _Default : Page
    {
        //sqlconnection
        SqlConnection sqlconn = new SqlConnection(ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)//Only clear the session on first load (not postback)
            {
                Session.Clear();
                Session.Abandon();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserName.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                lblAlert.Text = "Please enter both email and password.";
                lblAlert.CssClass = "text-danger";
            }
            try
            {
                checkconnection();
                sqlconn.Open();
                string query = " SELECT UsersId, FullName, IsHeadOrNot, IsAuthorizer FROM [Users] WHERE UserName =@UserName AND Password =@Password AND IsActive = 1";
                using (SqlCommand command = new SqlCommand(query, sqlconn))
                {
                    // Use parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@UserName", txtUserName.Text.Trim());
                    command.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // User found - create session
                            // Fixed: Convert int UserId to string properly
                            Session["UserId"] = Convert.ToInt32(reader["UsersId"]).ToString();
                            Session["FullName"] = reader["FullName"].ToString();
                            // Fixed: Handle bit values properly
                            Session["IsHeadOrNot"] = Convert.ToBoolean(reader["IsHeadOrNot"]);
                            Session["IsAuthorizer"] = reader["IsAuthorizer"].ToString();
                            Session["LoginTime"] = DateTime.Now;

                            if (!(Session["UserId"] == null || string.IsNullOrEmpty(Session["UserId"].ToString())))//i put this condition to ensure that user should redirect to next page only when session is working correctly,but it seems like their is a error near session when come to next page
                            {
                                //Redirect to dashboard
                                Response.Redirect("About.aspx");
                            }
                        }
                        else
                        {
                            lblAlert.Text = "Invalid email or password.";
                            lblAlert.CssClass = "text-danger"; 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblAlert.Text = ex.Message;
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