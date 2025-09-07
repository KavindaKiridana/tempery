using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static System.Net.Mime.MediaTypeNames;

namespace ITAssetHandling
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        

        protected void record1_TextChanged(object sender, EventArgs e)
        {
            calculateTotal("txtQty1", "txtPrice1", "lblTotal1");
        }
        protected void record2_TextChanged(object sender, EventArgs e)
        {
            calculateTotal("txtQty2", "txtPrice2", "lblTotal2");
        }
        protected void record3_TextChanged(object sender, EventArgs e)
        {
            calculateTotal("txtQty3", "txtPrice3", "lblTotal3");
        }
        protected void record4_TextChanged(object sender, EventArgs e)
        {
            calculateTotal("txtQty4", "txtPrice4", "lblTotal4");
        }

        public void calculateTotal(string txtQtyId, string txtPriceId, string lblTotal)
        {
            // Get the ContentPlaceHolder
            ContentPlaceHolder mainContent = (ContentPlaceHolder)Master.FindControl("MainContent");

            if (mainContent == null) return;

            // Find the controls inside the placeholder
            TextBox qtyTextBox = (TextBox)mainContent.FindControl(txtQtyId);
            TextBox priceTextBox = (TextBox)mainContent.FindControl(txtPriceId);
            Label totalLabel = (Label)mainContent.FindControl(lblTotal);

            if (qtyTextBox == null || priceTextBox == null || totalLabel == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(qtyTextBox.Text) || string.IsNullOrEmpty(priceTextBox.Text))
            {
                totalLabel.Text = "";
                return;
            }

            //  int qty;
            decimal qty;
            decimal price;

            if (!decimal.TryParse(qtyTextBox.Text, out qty))
            {
                totalLabel.Text = "Quantity must be a number";
                return;
            }

            if (!decimal.TryParse(priceTextBox.Text, out price))
            {
                totalLabel.Text = "Price must be a number";
                return;
            }

            totalLabel.Text = (qty * price).ToString("N2"); // formats to 2 decimal places
        }
    }
}