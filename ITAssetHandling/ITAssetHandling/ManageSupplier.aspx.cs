using Microsoft.Ajax.Utilities;
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
    public partial class ManageSupplier1 : System.Web.UI.Page
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
            if (string.IsNullOrEmpty(txtSName.Text.Trim()) || string.IsNullOrEmpty(ddlCurrency.SelectedValue)) 
            {
                lblAlert.Text = "Please enter the supplier and currency.";
                lblAlert.CssClass = "text-danger";
            }
            try
            {
                checkconnection();
                sqlconn.Open();
                string query = " insert into  Supplier (SName,Currency) values (@SName,@Currency)";
                using (SqlCommand command = new SqlCommand(query, sqlconn))
                {
                    command.Parameters.AddWithValue("@SName", txtSName.Text.Trim());
                    command.Parameters.AddWithValue("@Currency", ddlCurrency.SelectedValue);
                    command.ExecuteNonQuery();
                    lblAlert.Text = "Supplier added successfully.";
                    lblAlert.CssClass = "text-success"; // Requires Bootstrap or custom CSS
                    sqlconn.Close();

                    // Clear the form fields
                    txtSName.Text = "";
                    // Refresh the GridView to show the new data
                    GridView1.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblAlert.Text = ex.Message;
                lblAlert.CssClass = "text-warning"; // Requires Bootstrap or custom CSS
            }
        }

        public void checkconnection()
        {
            if (sqlconn.State == ConnectionState.Open)
            {
                sqlconn.Close();
            }
        }

        // Event handler for GridView row deleting
        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                // Get the SupplierId from DataKeys
                int supplierId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);

                // You can add additional validation here if needed
                // For example, check if supplier is referenced in other tables

                //lblAlert.Text = "Deleting supplier...";
                //lblAlert.CssClass = "alert alert-info";
                //lblAlert.Visible = true;
            }
            catch (Exception ex)
            {
                //lblAlert.Text = "Error during delete: " + ex.Message;
                //lblAlert.CssClass = "alert alert-danger";
                //lblAlert.Visible = true;
                //e.Cancel = true; // Cancel the delete operation
            }
        }

        // Event handler for SqlDataSource deleting
        protected void SqlDataSource1_Deleting(object sender, SqlDataSourceCommandEventArgs e)
        {
            // This event fires before the delete command is executed
            // You can add additional validation or logging here
        }

        //Event handler for SqlDataSource deleted
        protected void SqlDataSource1_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            // This event fires after the delete command is executed
        }
    }
}