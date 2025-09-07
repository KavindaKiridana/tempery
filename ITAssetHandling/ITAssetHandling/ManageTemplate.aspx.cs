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
    public partial class ManageTemplate : System.Web.UI.Page
    {
        SqlConnection sqlconn = new SqlConnection(ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCompanies();
                LoadUsers();
                InitializeDataTable();
                BindGridView();
            }
        }

        private void BindGridView()
        {
            try
            {
                // Get the maximum number of person positions for any template
                int maxPositions = GetMaxPersonPositions();

                // Create dynamic columns
                CreateDynamicColumns(maxPositions);

                // Bind data
                DataTable dt = GetTemplateData(maxPositions);
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error loading templates: " + ex.Message;
                lblMessage.CssClass = "text-danger";
            }
        }

        private int GetMaxPersonPositions()
        {
            int maxCount = 0;
            try
            {
                sqlconn.Open();
                string query = @"SELECT MAX(PositionCount) FROM (
                                    SELECT COUNT(*) as PositionCount 
                                    FROM PersonPosition 
                                    GROUP BY FlexibleTemplateId
                                ) as Counts";
                SqlCommand cmd = new SqlCommand(query, sqlconn);
                object result = cmd.ExecuteScalar();
                if (result != DBNull.Value)
                {
                    maxCount = Convert.ToInt32(result);
                }
            }
            catch (Exception)
            {
                maxCount = 0;
            }
            finally
            {
                if (sqlconn.State == ConnectionState.Open)
                    sqlconn.Close();
            }
            return maxCount > 0 ? maxCount : 1; // At least 1 column
        }

        private void CreateDynamicColumns(int maxPositions)
        {
            // Remove existing dynamic columns (keep first 3 columns: Edit, Company, IsActive)
            for (int i = GridView1.Columns.Count - 1; i >= 3; i--)
            {
                GridView1.Columns.RemoveAt(i);
            }

            // Add dynamic columns for person positions
            for (int i = 1; i <= maxPositions; i++)
            {
                BoundField positionField = new BoundField();
                positionField.DataField = "Position" + i;
                positionField.HeaderText = "Person" + i + " Position";
                positionField.ReadOnly = true;
                GridView1.Columns.Add(positionField);

                BoundField nameField = new BoundField();
                nameField.DataField = "PersonName" + i;
                nameField.HeaderText = "Person" + i + " Name";
                nameField.ReadOnly = true;
                GridView1.Columns.Add(nameField);
            }
        }

        private DataTable GetTemplateData(int maxPositions)
        {
            DataTable dt = new DataTable();

            try
            {
                sqlconn.Open();

                // Build dynamic query
                string selectClause = "SELECT ft.FlexibleTemplateId, ft.IsActive, c.CName as CompanyName";
                string fromClause = @" FROM FlexibleTemplate ft 
                                      INNER JOIN Company c ON ft.CompanyId = c.CompanyId";
                string leftJoinClause = "";

                for (int i = 1; i <= maxPositions; i++)
                {
                    selectClause += $", pp{i}.Position as Position{i}, u{i}.FullName as PersonName{i}";
                    leftJoinClause += $@" LEFT JOIN (
                                            SELECT *, ROW_NUMBER() OVER (PARTITION BY FlexibleTemplateId ORDER BY PersonPositionId) as rn 
                                            FROM PersonPosition
                                        ) pp{i} ON ft.FlexibleTemplateId = pp{i}.FlexibleTemplateId AND pp{i}.rn = {i}
                                        LEFT JOIN Users u{i} ON pp{i}.PersonId = u{i}.UsersId";
                }

                string query = selectClause + fromClause + leftJoinClause + " ORDER BY ft.FlexibleTemplateId DESC";

                SqlDataAdapter adapter = new SqlDataAdapter(query, sqlconn);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlconn.State == ConnectionState.Open)
                    sqlconn.Close();
            }

            return dt;
        }

        private void LoadCompanies()
        {
            try
            {
                sqlconn.Open();
                string query = "SELECT CompanyId, CName FROM Company WHERE IsActive = 1 ORDER BY CName";
                SqlCommand cmd = new SqlCommand(query, sqlconn);
                SqlDataReader reader = cmd.ExecuteReader();

                ddlCompany.Items.Clear();
                ddlCompany.DataSource = reader;
                ddlCompany.DataValueField = "CompanyId";
                ddlCompany.DataTextField = "CName";
                ddlCompany.DataBind();
              //  ddlCompany.Items.Insert(0, new ListItem("--Select Company--", ""));
                reader.Close();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error loading companies: " + ex.Message;
                lblMessage.CssClass = "text-danger";
            }
            finally
            {
                if (sqlconn.State == ConnectionState.Open)
                    sqlconn.Close();
            }
        }

        private void LoadUsers()
        {
            try
            {
                sqlconn.Open();
                string query = "SELECT UsersId, FullName FROM Users WHERE IsActive = 1 ORDER BY FullName";
                SqlCommand cmd = new SqlCommand(query, sqlconn);
                SqlDataReader reader = cmd.ExecuteReader();

                ddlUsers.DataSource = reader;
                ddlUsers.DataValueField = "UsersId";
                ddlUsers.DataTextField = "FullName";
                ddlUsers.DataBind();
              //  ddlUsers.Items.Insert(0, new ListItem("--Select User--", ""));
                reader.Close();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error loading users: " + ex.Message;
                lblMessage.CssClass = "text-danger";
            }
            finally
            {
                if (sqlconn.State == ConnectionState.Open)
                    sqlconn.Close();
            }
        }

        private void InitializeDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Position", typeof(string));
            dt.Columns.Add("UserId", typeof(int));
            dt.Columns.Add("UserName", typeof(string));
            ViewState["PersonPositions"] = dt;
        }

        private DataTable GetPersonPositions()
        {
            return (DataTable)ViewState["PersonPositions"];
        }

        private void SetPersonPositions(DataTable dt)
        {
            ViewState["PersonPositions"] = dt;
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPosition.Text))
            {
                lblMessage.Text = "Please enter a position.";
                lblMessage.CssClass = "text-danger";
                return;
            }

            if (string.IsNullOrEmpty(ddlUsers.SelectedValue))
            {
                lblMessage.Text = "Please select a user.";
                lblMessage.CssClass = "text-danger";
                return;
            }

            DataTable dt = GetPersonPositions();
            DataRow row = dt.NewRow();
            row["Position"] = txtPosition.Text.Trim();
            row["UserId"] = Convert.ToInt32(ddlUsers.SelectedValue);
            row["UserName"] = ddlUsers.SelectedItem.Text;
            dt.Rows.Add(row);
            SetPersonPositions(dt);

            GridView3.DataSource = dt;
            GridView3.DataBind();

            // Clear inputs
            txtPosition.Text = "";
            ddlUsers.SelectedIndex = 0;
            lblMessage.Text = "";
        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int index = Convert.ToInt32(btn.CommandArgument);

            DataTable dt = GetPersonPositions();
            if (index >= 0 && index < dt.Rows.Count)
            {
                dt.Rows.RemoveAt(index);
                SetPersonPositions(dt);
                GridView3.DataSource = dt;
                GridView3.DataBind();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlCompany.SelectedValue))
            {
                lblMessage.Text = "Please select a company.";
                lblMessage.CssClass = "text-danger";
                return;
            }

            DataTable dt = GetPersonPositions();
            if (dt.Rows.Count == 0)
            {
                lblMessage.Text = "Please add at least one person position.";
                lblMessage.CssClass = "text-danger";
                return;
            }

            SqlTransaction transaction = null;
            try
            {
                sqlconn.Open();
                transaction = sqlconn.BeginTransaction();

                // Insert into FlexibleTemplate
                string insertTemplateQuery = "INSERT INTO FlexibleTemplate (CompanyId) VALUES (@CompanyId); SELECT SCOPE_IDENTITY();";
                SqlCommand cmd = new SqlCommand(insertTemplateQuery, sqlconn, transaction);
                cmd.Parameters.AddWithValue("@CompanyId", Convert.ToInt32(ddlCompany.SelectedValue));
                int flexibleTemplateId = Convert.ToInt32(cmd.ExecuteScalar());

                // Insert into PersonPosition
                string insertPersonQuery = "INSERT INTO PersonPosition (FlexibleTemplateId, PersonId, Position) VALUES (@FlexibleTemplateId, @PersonId, @Position)";
                foreach (DataRow row in dt.Rows)
                {
                    SqlCommand cmdPerson = new SqlCommand(insertPersonQuery, sqlconn, transaction);
                    cmdPerson.Parameters.AddWithValue("@FlexibleTemplateId", flexibleTemplateId);
                    cmdPerson.Parameters.AddWithValue("@PersonId", Convert.ToInt32(row["UserId"]));
                    cmdPerson.Parameters.AddWithValue("@Position", row["Position"].ToString());
                    cmdPerson.ExecuteNonQuery();
                }

                transaction.Commit();
                lblMessage.Text = "Template saved successfully!";
                lblMessage.CssClass = "text-success";
                InitializeDataTable(); // Reset
                GridView3.DataSource = null;
                GridView3.DataBind();
                ddlCompany.SelectedIndex = 0;

                // Refresh the main gridview
                // BindGridView();//this referesing part is not working correctly.so i commented this 
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                lblMessage.Text = "Error saving template: " + ex.Message;
                lblMessage.CssClass = "text-danger";
            }
            finally
            {
                if (sqlconn.State == ConnectionState.Open)
                    sqlconn.Close();
                Response.Redirect("ManageTemplate.aspx", false);
            }
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            BindGridView();
        }


        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                int templateId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);

                // Get the checkbox value from the correct cell
                CheckBox chkIsActive = (CheckBox)GridView1.Rows[e.RowIndex].Cells[2].Controls[0];
                bool isActive = chkIsActive.Checked;

                sqlconn.Open();
                string query = "UPDATE FlexibleTemplate SET IsActive = @IsActive WHERE FlexibleTemplateId = @FlexibleTemplateId";
                SqlCommand cmd = new SqlCommand(query, sqlconn);
                cmd.Parameters.AddWithValue("@IsActive", isActive);
                cmd.Parameters.AddWithValue("@FlexibleTemplateId", templateId);
                cmd.ExecuteNonQuery();

                GridView1.EditIndex = -1;
                BindGridView();
                lblMessage.Text = "Template updated successfully!";
                lblMessage.CssClass = "text-success";
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error updating template: " + ex.Message;
                lblMessage.CssClass = "text-danger";
            }
            finally
            {
                if (sqlconn.State == ConnectionState.Open)
                    sqlconn.Close();
                Response.Redirect("ManageTemplate.aspx", false);
            }
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            BindGridView();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Handle checkbox in edit mode
                if ((e.Row.RowState & DataControlRowState.Edit) == DataControlRowState.Edit)
                {
                    CheckBox chkIsActive = (CheckBox)e.Row.FindControl("IsActive");
                    if (chkIsActive != null)
                    {
                        DataRowView rowView = (DataRowView)e.Row.DataItem;
                        chkIsActive.Checked = Convert.ToBoolean(rowView["IsActive"]);
                    }
                }
            }
        }
    }
}