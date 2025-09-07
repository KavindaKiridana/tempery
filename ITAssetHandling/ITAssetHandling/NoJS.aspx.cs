using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ITAssetHandling
{
    public partial class NoJS : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {
            int value;
            if (int.TryParse(TextBox1.Text, out value))
            {
                Label1.Text = (value * 2).ToString(); // Just an example calculation
            }
            else
            {
                Label1.Text = "Invalid input!";
            }
        }
    }
}