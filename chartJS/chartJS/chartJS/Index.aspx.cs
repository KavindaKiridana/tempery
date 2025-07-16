using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace chartJS
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            using (OleDbConnection conn = new OleDbConnection(ConfigurationManager.ConnectionStrings["ExcelConnection"].ConnectionString))
            {
                conn.Open();
                // Check connection state
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    TextBox1.Text = "connection was successful";

                    string query = "SELECT * FROM [vanilla_farm_data.csv]"; // MUST include .txt//csv file name without extension
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())//fix the bug here
                        {
                            while (reader.Read())
                            {
                                // FIX: Skip the first row if it contains headers
                                if (reader.HasRows) // Ensure there's at least one row to read
                                {
                                    reader.Read(); // This advances the reader past the first row (the header row)
                                }

                                while (reader.Read())
                                {
                                    // Now, the reader will start from the second row, which contains your actual data.
                                    // You can access your data here, for example:
                                    // string column1Value = reader["ColumnName"].ToString();
                                    // int column2Value = Convert.ToInt32(reader["AnotherColumn"].ToString());

                                    // For demonstration, let's just append some data to the TextBox
                                    TextBox1.Text += Environment.NewLine + "Data Row: " + reader[0].ToString(); // Reads the first column of the current row
                                }
                            }
                        }
                    }
                }              
                conn.Close();
            }
        }
    }
}