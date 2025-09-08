using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication1;
namespace ITAssetHandling
{
    public partial class About : Page
    {
        //sqlconnection
        SqlConnection sqlconn = new SqlConnection(ConfigurationManager.ConnectionStrings["ITAssetConn"].ConnectionString);
        string currency = ""; // This tracks the currency of ddlSupply1
        decimal total = 0; // Not used directly in LoadTemplates anymore, but kept for legacy calls
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || string.IsNullOrEmpty(Session["UserId"].ToString()))
            {
                //Redirect to login page if session is null
                Response.Redirect("~/Default.aspx");
            }
            if (!IsPostBack)
            {
                txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtRequestedBy.Text = Session["FullName"].ToString();
                LoadCompanies();
                LoadDepartment();
                LoadReasons();
                LoadDivHeads();
                LoadConfirmedBy();
                LoadSuppliers();

                lbltotalINR.Visible = false;
                lbltotalLKR.Visible = false;
                lbltotalUSD.Visible = false;
            }
        }

        //function for load first supplier list
        private void LoadSuppliers()
        {
            checkconnection();
            try
            {
                sqlconn.Open();
                string query = "SELECT SupplierId, SName FROM Supplier WHERE IsActive = 1 ORDER BY SName";
                SqlCommand cmd = new SqlCommand(query, sqlconn);
                SqlDataReader reader = cmd.ExecuteReader();
                // Clear existing items
                ddlSupply.Items.Clear();
                while (reader.Read())
                {
                    ddlSupply.Items.Add(new ListItem(reader["SName"].ToString(), reader["SupplierId"].ToString()));
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                // Handle error
                Response.Write("Error loading supplier: " + ex.Message);
            }
            finally
            {
                if (sqlconn.State == ConnectionState.Open)
                {
                    sqlconn.Close();
                }
            }
        }
        //function for load companies list
        private void LoadCompanies()
        {
            checkconnection();
            try
            {
                sqlconn.Open();
                string query = "SELECT CompanyId, CName FROM Company WHERE IsActive = 1 ORDER BY CName";
                SqlCommand cmd = new SqlCommand(query, sqlconn);
                SqlDataReader reader = cmd.ExecuteReader();
                // Clear existing items
                ddlCompany.Items.Clear();
                // Add default item
                //ddlCompany.Items.Add(new ListItem("-- Select Company --", "0"));
                while (reader.Read())
                {
                    ddlCompany.Items.Add(new ListItem(reader["CName"].ToString(), reader["CompanyId"].ToString()));
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                // Handle error
                Response.Write("Error loading companies: " + ex.Message);
            }
            finally
            {
                if (sqlconn.State == ConnectionState.Open)
                {
                    sqlconn.Close();
                }
            }
        }

        //function for load departments list
        private void LoadDepartment()
        {
            checkconnection();
            try
            {
                sqlconn.Open();
                string query = "SELECT DepartmentId, DName FROM Department WHERE IsActive = 1 ORDER BY DName";
                SqlCommand cmd = new SqlCommand(query, sqlconn);
                SqlDataReader reader = cmd.ExecuteReader();
                // Clear existing items
                ddlDepartment.Items.Clear();
                ddlUsedByToWhom.Items.Clear();
                // Add default item
                //ddlDepartment.Items.Add(new ListItem("-- Select Department --", "0"));
                //ddlUsedByToWhom.Items.Add(new ListItem("-- Select Department --", "0"));
                while (reader.Read())
                {
                    ddlDepartment.Items.Add(new ListItem(reader["DName"].ToString(), reader["DepartmentId"].ToString()));
                    ddlUsedByToWhom.Items.Add(new ListItem(reader["DName"].ToString(), reader["DepartmentId"].ToString()));
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                // Handle error
                Response.Write("Error loading departments: " + ex.Message);
            }
            finally
            {
                if (sqlconn.State == ConnectionState.Open)
                {
                    sqlconn.Close();
                }
            }
        }
        //function for load reasons list
        private void LoadReasons()
        {
            checkconnection();
            try
            {
                sqlconn.Open();
                string query = "SELECT ReasonId, RName FROM Reason WHERE IsActive = 1 ORDER BY RName";
                SqlCommand cmd = new SqlCommand(query, sqlconn);
                SqlDataReader reader = cmd.ExecuteReader();
                // Clear existing items
                ddlReason.Items.Clear();
                // Add default item
                //ddlReason.Items.Add(new ListItem("-- Select Reason --", "0"));
                while (reader.Read())
                {
                    ddlReason.Items.Add(new ListItem(reader["RName"].ToString(), reader["ReasonId"].ToString()));
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                // Handle error
                Response.Write("Error loading reasons: " + ex.Message);
            }
            finally
            {
                if (sqlconn.State == ConnectionState.Open)
                {
                    sqlconn.Close();
                }
            }
        }
        //function for load divisional heads list
        private void LoadDivHeads()
        {
            checkconnection();
            try
            {
                sqlconn.Open();
                string query = "SELECT UsersId, FullName FROM Users where IsHeadOrNot='1' and IsActive = 1 ORDER BY FullName";
                SqlCommand cmd = new SqlCommand(query, sqlconn);
                SqlDataReader reader = cmd.ExecuteReader();
                // Clear existing items
                ddlHead.Items.Clear();
                // Add default item
                //ddlHead.Items.Add(new ListItem("-- Select Head --", "0"));
                while (reader.Read())
                {
                    ddlHead.Items.Add(new ListItem(reader["FullName"].ToString(), reader["UsersId"].ToString()));
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Response.Write("Error loading heads: " + ex.Message);
            }
            finally
            {
                if (sqlconn.State == ConnectionState.Open)
                {
                    sqlconn.Close();
                }
            }
        }

        //function for load confirmed by users list
        private void LoadConfirmedBy()
        {
            checkconnection();
            try
            {
                sqlconn.Open();
                string query = "SELECT UsersId, FullName FROM Users where IsActive = 1 ORDER BY FullName";
                SqlCommand cmd = new SqlCommand(query, sqlconn);
                SqlDataReader reader = cmd.ExecuteReader();
                // Clear existing items
                ddlConfirmedBy.Items.Clear();
                // Add default item
                //ddlHead.Items.Add(new ListItem("-- Select Head --", "0"));
                while (reader.Read())
                {
                    ddlConfirmedBy.Items.Add(new ListItem(reader["FullName"].ToString(), reader["UsersId"].ToString()));
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                // Handle error
                Response.Write("Error loading heads: " + ex.Message);
            }
            finally
            {
                if (sqlconn.State == ConnectionState.Open)
                {
                    sqlconn.Close();
                }
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.RawUrl);
        }

        private void LoadTemplates(decimal totalCost, string relevantCurrency)
        {
            string currencyForTemplate = relevantCurrency;
            ddlTemplate.Items.Clear();

            if (ddlCompany.SelectedValue == null || string.IsNullOrEmpty(ddlCompany.SelectedValue) || ddlCompany.SelectedValue == "0")
            {
                ddlTemplate.Items.Clear();
                return;
            }

            int selectedCompany = Convert.ToInt32(ddlCompany.SelectedValue);
            checkconnection();

            try
            {
                sqlconn.Open();
                SqlCommand cmd;

                if (currencyForTemplate != "LKR")
                {
                    string query = @"
        SELECT 
            ft.FlexibleTemplateId AS TemplateId,
            u.FullName,
            pp.Position
        FROM FlexibleTemplate ft
        INNER JOIN PersonPosition pp ON ft.FlexibleTemplateId = pp.FlexibleTemplateId
        INNER JOIN Users u ON pp.PersonId = u.UsersId
        WHERE ft.IsActive = 1 
          AND ft.CompanyId = @CompanyId
        ORDER BY ft.FlexibleTemplateId;";
                    cmd = new SqlCommand(query, sqlconn);
                    cmd.Parameters.AddWithValue("@CompanyId", selectedCompany);
                }
                else if (currencyForTemplate == "LKR")
                {
                    if (totalCost > 15000.0m)
                    {
                        // Must include MD
                        string query = @"
        SELECT 
            ft.FlexibleTemplateId AS TemplateId,
            u.FullName,
            pp.Position
        FROM FlexibleTemplate ft
        INNER JOIN PersonPosition pp ON ft.FlexibleTemplateId = pp.FlexibleTemplateId
        INNER JOIN Users u ON pp.PersonId = u.UsersId
        WHERE ft.IsActive = 1
          AND ft.CompanyId = @CompanyId
          AND EXISTS (
                SELECT 1 FROM PersonPosition pp2
                WHERE pp2.FlexibleTemplateId = ft.FlexibleTemplateId
                  AND pp2.Position = 'MD'
            )
        ORDER BY ft.FlexibleTemplateId;";
                        cmd = new SqlCommand(query, sqlconn);
                    }
                    else
                    {
                        // Must NOT include MD
                        string query = @"
        SELECT 
            ft.FlexibleTemplateId AS TemplateId,
            u.FullName,
            pp.Position
        FROM FlexibleTemplate ft
        INNER JOIN PersonPosition pp ON ft.FlexibleTemplateId = pp.FlexibleTemplateId
        INNER JOIN Users u ON pp.PersonId = u.UsersId
        WHERE ft.IsActive = 1
          AND ft.CompanyId = @CompanyId
          AND NOT EXISTS (
                SELECT 1 FROM PersonPosition pp2
                WHERE pp2.FlexibleTemplateId = ft.FlexibleTemplateId
                  AND pp2.Position = 'MD'
            )
        ORDER BY ft.FlexibleTemplateId;";
                        cmd = new SqlCommand(query, sqlconn);
                    }
                    cmd.Parameters.AddWithValue("@CompanyId", selectedCompany);
                }
                else
                {
                    ddlTemplate.Items.Clear();
                    ddlTemplate.Items.Insert(0, new ListItem("-- Unknown Currency Type --", "0"));
                    return;
                }

                SqlDataReader reader = cmd.ExecuteReader();


                // Group results by TemplateId
                Dictionary<string, List<string>> templateData = new Dictionary<string, List<string>>();

                while (reader.Read())
                {
                    string templateId = reader["TemplateId"].ToString();
                    string fullName = reader["FullName"].ToString();
                    string position = reader["Position"].ToString();

                    string displayText = fullName + ":" + position;

                    if (!templateData.ContainsKey(templateId))
                    {
                        templateData[templateId] = new List<string>();
                    }
                    templateData[templateId].Add(displayText);
                }
                reader.Close();

                // Add to dropdown
                foreach (var kvp in templateData)
                {
                    string templateId = kvp.Key;
                    string displayText = string.Join(", ", kvp.Value);
                    ddlTemplate.Items.Add(new ListItem(displayText, templateId));
                }
            }
            catch (Exception ex)
            {
                ddlTemplate.Items.Clear();
                ddlTemplate.Items.Insert(0, new ListItem("-- Error Loading Templates --", "0"));
                Response.Write("Error loading templates: " + ex.Message);
            }
            finally
            {
                if (sqlconn.State == ConnectionState.Open)
                {
                    sqlconn.Close();
                }
            }
        }


        protected void btnAddPayment_Click(object sender, EventArgs e)
        {
            // Clear any previous error messages
            lblPaymnetRecord.Text = "";
            lblPaymnetRecord.CssClass = "";

            // Validate input fields
            if (!ValidateInputs())
            {
                return;
            }

            // Get values from form controls
            string supplierId = ddlSupply.SelectedValue;
            string supplierName = ddlSupply.SelectedItem.Text;
            float qty = float.Parse(txtQty.Text.Trim());
            float unitPrice = float.Parse(txtUnitPrice.Text.Trim());
            string currency = ddlCurrency.SelectedValue;
            string detail = txtDetail.Text.Trim();
            float totalPrice = qty * unitPrice;

            // Get or create DataTable from ViewState
            DataTable dt = GetPaymentDataTable();

            // Add new row to DataTable
            DataRow newRow = dt.NewRow();
            newRow["SupplierId"] = supplierId;
            newRow["SupplierName"] = supplierName;
            newRow["Qty"] = qty;
            newRow["UnitPrice"] = unitPrice;
            newRow["Currency"] = currency;
            newRow["Detail"] = detail;
            newRow["TotalPrice"] = totalPrice;
            dt.Rows.Add(newRow);

            // Save DataTable to ViewState
            ViewState["PaymentData"] = dt;

            // Bind to GridView
            BindGridView();

            // Clear form fields
            ClearFormFields();

            // Show success message
            lblPaymnetRecord.Text = "Payment record added successfully!";
            lblPaymnetRecord.CssClass = "text-success";

            totalPriceBySameCurrency();
        }

        private void totalPriceBySameCurrency()
        {
            float totalUSD = 0;
            float totalLKR = 0;
            float totalINR = 0;

            DataTable dt = GetPaymentDataTable();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string currency = row["Currency"].ToString();
                    float totalPrice = Convert.ToSingle(row["TotalPrice"]);

                    switch (currency.ToUpper())
                    {
                        case "USD":
                            totalUSD += totalPrice;
                            break;
                        case "LKR":
                            totalLKR += totalPrice;
                            break;
                        case "INR":
                            totalINR += totalPrice;
                            break;
                    }
                }
            }

            ddlTemplate.Items.Clear();
            if (totalLKR > 0 && totalINR == 0 && totalUSD == 0)
            {
                string relevantCurrency = "LKR";
                LoadTemplates((decimal)totalLKR, relevantCurrency);
            }
            else if (totalLKR > 0 || totalINR > 0 || totalUSD > 0)
            {
                string relevantCurrency = "NoLKR";
                LoadTemplates((decimal)totalLKR, relevantCurrency);
            }
            DisplayCurrencyTotals(totalUSD, totalLKR, totalINR);
        }

        private void DisplayCurrencyTotals(float totalUSD, float totalLKR, float totalINR)
        {
            lbltotalINR.Visible = false;
            lbltotalLKR.Visible = false;
            lbltotalUSD.Visible = false;

            if (totalUSD > 0)
            {
                lbltotalUSD.Visible = true;
                lbltotalUSD.Text = "Total USD: " + totalUSD.ToString("F2");
            }
            if (totalINR > 0)
            {
                lbltotalINR.Visible = true;
                lbltotalINR.Text = "Total INR: " + totalINR.ToString("F2");
            }
            if (totalLKR > 0)
            {
                lbltotalLKR.Visible = true;
                lbltotalLKR.Text = "Total LKR: " + totalLKR.ToString("F2");
            }
        }

        private bool ValidateInputs()
        {
            // Validate Supplier selection
            if (ddlSupply.SelectedIndex == -1 || string.IsNullOrEmpty(ddlSupply.SelectedValue))
            {
                ShowErrorMessage("Please select a supplier.");
                return false;
            }

            // Validate Quantity
            if (string.IsNullOrEmpty(txtQty.Text.Trim()))
            {
                ShowErrorMessage("Please enter quantity.");
                return false;
            }

            float qty;
            if (!float.TryParse(txtQty.Text.Trim(), out qty) || qty <= 0)
            {
                ShowErrorMessage("Please enter a valid quantity (must be greater than 0).");
                return false;
            }

            // Validate Unit Price
            if (string.IsNullOrEmpty(txtUnitPrice.Text.Trim()))
            {
                ShowErrorMessage("Please enter unit price.");
                return false;
            }

            float unitPrice;
            if (!float.TryParse(txtUnitPrice.Text.Trim(), out unitPrice) || unitPrice <= 0)
            {
                ShowErrorMessage("Please enter a valid unit price (must be greater than 0).");
                return false;
            }

            // Validate Currency selection
            if (string.IsNullOrEmpty(ddlCurrency.SelectedValue))
            {
                ShowErrorMessage("Please select a currency.");
                return false;
            }

            // Validate Description
            if (string.IsNullOrEmpty(txtDetail.Text.Trim()))
            {
                ShowErrorMessage("Please enter a description.");
                return false;
            }

            return true;
        }

        private void ShowErrorMessage(string message)
        {
            lblPaymnetRecord.Text = message;
            lblPaymnetRecord.CssClass = "text-danger";
        }

        private DataTable GetPaymentDataTable()
        {
            DataTable dt = ViewState["PaymentData"] as DataTable;

            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.Add("SupplierId", typeof(string));
                dt.Columns.Add("SupplierName", typeof(string));
                dt.Columns.Add("Qty", typeof(float));
                dt.Columns.Add("UnitPrice", typeof(float));
                dt.Columns.Add("Currency", typeof(string));
                dt.Columns.Add("Detail", typeof(string));
                dt.Columns.Add("TotalPrice", typeof(float));
            }
            return dt;
        }

        private void BindGridView()
        {
            DataTable dt = GetPaymentDataTable();

            if (dt != null && dt.Rows.Count > 0)
            {
                // Create a view of the DataTable with only the columns we want to display
                DataTable displayTable = new DataTable();
                displayTable.Columns.Add("SupplierName", typeof(string));
                displayTable.Columns.Add("Qty", typeof(float));
                displayTable.Columns.Add("UnitPrice", typeof(float));
                displayTable.Columns.Add("Currency", typeof(string));
                displayTable.Columns.Add("Detail", typeof(string));
                displayTable.Columns.Add("TotalPrice", typeof(float));

                foreach (DataRow row in dt.Rows)
                {
                    DataRow newRow = displayTable.NewRow();
                    newRow["SupplierName"] = row["SupplierName"];
                    newRow["Qty"] = row["Qty"];
                    newRow["UnitPrice"] = row["UnitPrice"];
                    newRow["Currency"] = row["Currency"];
                    newRow["Detail"] = row["Detail"];
                    newRow["TotalPrice"] = row["TotalPrice"];
                    displayTable.Rows.Add(newRow);
                }

                GridView.DataSource = displayTable;
                GridView.DataBind();
            }
            else
            {
                // Bind empty data to show EmptyDataTemplate
                DataTable emptyTable = new DataTable();
                emptyTable.Columns.Add("SupplierName", typeof(string));
                emptyTable.Columns.Add("Qty", typeof(float));
                emptyTable.Columns.Add("UnitPrice", typeof(float));
                emptyTable.Columns.Add("Currency", typeof(string));
                emptyTable.Columns.Add("Detail", typeof(string));
                emptyTable.Columns.Add("TotalPrice", typeof(float));

                GridView.DataSource = emptyTable;
                GridView.DataBind();
            }
        }

        private void ClearFormFields()
        {
            ddlSupply.SelectedIndex = -1;
            txtQty.Text = "";
            txtUnitPrice.Text = "";
            ddlCurrency.SelectedValue = "USD"; // Reset to default
            txtDetail.Text = "";
        }

        // Add this method to handle the Remove button click in GridView
        protected void GridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                try
                {
                    int rowIndex = Convert.ToInt32(e.CommandArgument);

                    DataTable dt = GetPaymentDataTable();
                    if (dt != null && rowIndex >= 0 && rowIndex < dt.Rows.Count)
                    {
                        // Remove the row from DataTable
                        dt.Rows.RemoveAt(rowIndex);

                        // Update ViewState
                        ViewState["PaymentData"] = dt;

                        // Rebind GridView
                        BindGridView();

                        // Show success message
                        lblPaymnetRecord.Text = "Record deleted successfully!";
                        lblPaymnetRecord.CssClass = "text-success";

                        totalPriceBySameCurrency();
                    }
                    else
                    {
                        // Show error message if row index is invalid
                        lblPaymnetRecord.Text = "Error: Unable to delete record. Invalid row index.";
                        lblPaymnetRecord.CssClass = "text-danger";
                    }
                }
                catch (Exception ex)
                {
                    // Handle any errors during deletion
                    lblPaymnetRecord.Text = "Error deleting record: " + ex.Message;
                    lblPaymnetRecord.CssClass = "text-danger";
                }
            }
        }

        public void checkconnection()
        {
            if (sqlconn.State == ConnectionState.Open)
            {
                sqlconn.Close();
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //get form's data
            DateTime date = Convert.ToDateTime(txtDate.Text);
            int requestedBy = Convert.ToInt32(Session["UserId"]);
            int invoiceCompany = Convert.ToInt32(ddlCompany.SelectedValue); // Foreign key, int in DB
            int allocationDepartment = Convert.ToInt32(ddlDepartment.SelectedValue); // Foreign key, int in DB
            int reason = Convert.ToInt32(ddlReason.SelectedValue); // Foreign key, int in DB
            int divisionHead = Convert.ToInt32(ddlHead.SelectedValue); // Foreign key, int in DB
            int usedByToWhom = Convert.ToInt32(ddlUsedByToWhom.SelectedValue); // Foreign key, int in DB
            bool budgeted = Convert.ToBoolean(ddlBudgeted.SelectedValue); // Bool in DB
            int templateID = Convert.ToInt32(ddlTemplate.SelectedValue);// Foreign key, int in DB
            int confirmedBy = Convert.ToInt32(ddlConfirmedBy.SelectedValue);
            // Handle nullable DateTime
            DateTime? dateOfPurchase = null;
            if (!string.IsNullOrWhiteSpace(txtDateOfPurchase.Text))
            {
                dateOfPurchase = Convert.ToDateTime(txtDateOfPurchase.Text);
            }
            string warranty = txtWarranty.Text; // Can be null
            string make = txtMake.Text; // Can be null
            string serialNo = txtSerialNo.Text; // Can be null
            string model = txtModel.Text; // Can be null
            // Handle nullable BIT (bool) fields from dropdowns
            bool? quotation = null;
            if (ddlQuatation.SelectedValue != "null")
            {
                quotation = Convert.ToBoolean(ddlQuatation.SelectedValue);
            }
            bool? configuration = null;
            if (ddlConfigurationEvalation.SelectedValue != "null")
            {
                configuration = Convert.ToBoolean(ddlConfigurationEvalation.SelectedValue);
            }
            bool? costBreakdown = null;
            if (ddlCostBreakdown.SelectedValue != "null")
            {
                costBreakdown = Convert.ToBoolean(ddlCostBreakdown.SelectedValue);
            }
            string divisionComments = txtITComments.Text; // need to put validtion for prevant null values
            string recommendation = txtITDivRecommend.Text; // Can be null
            string remark = txtRemark.Text; // Can be null

            if (string.IsNullOrWhiteSpace(txtITComments.Text))
            {
                Response.Write("<script>alert('IT Division Comments cannot be empty. Please provide a comment.');</script>");
                return; 
            }

            // Save data to Document table 
            int newDocumentId = -1; // Variable to hold the ID of the newly inserted document
            try
            {
                checkconnection();
                sqlconn.Open();
                // Use parameterized query to prevent SQL injection
                string insertDocumentQuery = @"
            INSERT INTO Document (
                ReasonId, CompanyId, DepartmentId, UsedByToWhom, UsersId, DepartmentHead,
                SerialNo, ITDivisionComment, ITDivisionRecommendation, Remarks, 
                Budgeted, EIDDateOfPurchase, EIDMake, EIDSerialNo, EIDWarranty, EIDModel,
                Quotation, Configuration, CostBeakdown ,TemplateId,ConfirmedBy
            ) VALUES (
                @ReasonId, @CompanyId, @DepartmentId, @UsedByToWhom, @UsersId, @DepartmentHead,
                @SerialNo, @ITDivisionComment, @ITDivisionRecommendation, @Remarks,
                @Budgeted, @EIDDateOfPurchase, @EIDMake, @EIDSerialNo, @EIDWarranty, @EIDModel,
                @Quotation, @Configuration, @CostBeakdown, @TemplateId,@ConfirmedBy
            );
            SELECT SCOPE_IDENTITY();"; // Get the ID of the newly inserted row
                using (SqlCommand cmd = new SqlCommand(insertDocumentQuery, sqlconn))
                {
                    // Add parameters
                    cmd.Parameters.AddWithValue("@ReasonId", reason);
                    cmd.Parameters.AddWithValue("@CompanyId", invoiceCompany);
                    cmd.Parameters.AddWithValue("@DepartmentId", allocationDepartment);
                    cmd.Parameters.AddWithValue("@UsedByToWhom", usedByToWhom);
                    cmd.Parameters.AddWithValue("@UsersId", requestedBy); // Assuming 'UsersId' is the edited user
                    cmd.Parameters.AddWithValue("@DepartmentHead", divisionHead);
                    cmd.Parameters.AddWithValue("@templateID", templateID);
                    cmd.Parameters.AddWithValue("@ConfirmedBy", confirmedBy);
                    // SavedTime uses DEFAULT GETDATE(), so no need to insert explicitly
                    // Handle potentially null string fields
                    cmd.Parameters.AddWithValue("@SerialNo", (object)serialNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ITDivisionComment", divisionComments ?? (object)DBNull.Value); // Should not be null per requirement, but handle just in case
                    cmd.Parameters.AddWithValue("@ITDivisionRecommendation", (object)recommendation ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Remarks", (object)remark ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Budgeted", budgeted);
                    // Handle potentially null DateTime
                    cmd.Parameters.AddWithValue("@EIDDateOfPurchase", (object)dateOfPurchase ?? DBNull.Value);
                    // Handle potentially null string fields for EID
                    cmd.Parameters.AddWithValue("@EIDMake", (object)make ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EIDSerialNo", (object)serialNo ?? DBNull.Value); // Note: serialNo variable used
                    cmd.Parameters.AddWithValue("@EIDWarranty", (object)warranty ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EIDModel", (object)model ?? DBNull.Value);
                    // Handle potentially null BIT fields
                    cmd.Parameters.AddWithValue("@Quotation", (object)quotation ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Configuration", (object)configuration ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CostBeakdown", (object)costBreakdown ?? DBNull.Value);
                    // Execute the query and get the new DocumentId
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        newDocumentId = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the full exception details server-side for better debugging
                System.Diagnostics.Debug.WriteLine("Document Insert Error: " + ex.ToString());


                // Show a generic message to the user for better security/usability during development
                Response.Write("<script>alert('An error occurred while submitting the form. Please check the logs or contact support. \\n\\n(Dev Info: " + ex.Message.Replace("'", "\\'") + ")');</script>");

            }
            finally
            {
                if (sqlconn.State == ConnectionState.Open)
                {
                    sqlconn.Close();
                }
            }
            if (newDocumentId > 0)
            {
                // --- Save data to RequestedItemPayments table ---
                updateRequestedItemPaymentsTable(newDocumentId);
                GeneratePDF generatePDF = new GeneratePDF();
                generatePDF.GetPDF(newDocumentId);
                // FIXED: Show alert BEFORE redirect, and use proper redirect approach
                string script = @"
            <script type='text/javascript'>
                alert('Form submitted successfully! Document ID: " + newDocumentId + @"');
                window.location.href = '" + Request.RawUrl + @"';
            </script>";
                ClientScript.RegisterStartupScript(this.GetType(), "SuccessAlert", script);
            }
            else
            {
                Response.Write("<script>alert('Error: Failed to create document record.');</script>");
            }
        }


        protected void updateRequestedItemPaymentsTable(int DocumentID)
        {
            DataTable dt = GetPaymentDataTable();

            if (dt != null && dt.Rows.Count > 0)
            {
                int recordNumber = 1;
                foreach (DataRow row in dt.Rows)
                {
                    string supplierId = row["SupplierId"].ToString();
                    string description = row["Detail"].ToString();
                    string quantity = row["Qty"].ToString();
                    string unitPrice = row["UnitPrice"].ToString();
                    string total = row["TotalPrice"].ToString();
                    string recordCurrency = row["Currency"].ToString();

                    SaveRecordData(recordNumber, DocumentID, supplierId, description, quantity, unitPrice, total, recordCurrency);
                    recordNumber++;
                }
            }
            else
            {
                // Handle case where no payment records exist
                Response.Write("<script>alert('No payment records to save.');</script>");
            }
        }

        // Helper method to encapsulate saving logic for a single row
        private void SaveRecordData(int recordNumber, int DocumentID, string supplierId, string description, string quantity, string unitPrice, string total, string recordcurrency)
        {
            // Implement  database saving logic here for a single record row
            // Use the parameters passed in.
            try
            {
                checkconnection();
                sqlconn.Open();
                string query = @"INSERT INTO RequestedItemPayments
(UnitPrice, Qty, Description, DocumentID, SupplierId,Currency) VALUES
(@UnitPrice,@Quantity,@Description,@documentid,@SupplierId,@Currency)";
                SqlCommand cmd = new SqlCommand(query, sqlconn);
                cmd.Parameters.AddWithValue("@documentid", DocumentID);
                cmd.Parameters.AddWithValue("@SupplierId", supplierId);
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
                cmd.Parameters.AddWithValue("@Currency", recordcurrency);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Handle error appropriately (log it, show message)
                Response.Write($"Error saving record {recordNumber}: " + ex.Message);
            }
            finally
            {
                if (sqlconn.State == ConnectionState.Open)
                {
                    sqlconn.Close();
                }
            }
        }
    }
}