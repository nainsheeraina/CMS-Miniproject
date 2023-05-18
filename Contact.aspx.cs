using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI.WebControls;

namespace Complaint_Management_System
{
    public partial class Contact : System.Web.UI.Page
    {
        clsDataAccessMACET cls = new clsDataAccessMACET();

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        void clear()
        {
            txtname.Text = "";
            txtmsg.Text = "";
            txtno.Text = "";
            txtemail.Text = "";
        }
        protected void BtnSubmit_Click(object sender, EventArgs e)
        {
            lblmsg.Text = "";
            try
            {
                SqlConnection con = new SqlConnection("Data Source=(local);Initial Catalog=cms;User ID=sa;pwd=123");

                SqlCommand cmd = new SqlCommand("insert into ContactUs (Name,EmailId,MobileNo,Message,Cdate) values(@Name,@EmailId,@MobileNo,@Message,@Cdate)");
                cmd.Parameters.AddWithValue("@Name", txtname.Text);
                cmd.Parameters.AddWithValue("@EmailId", txtemail.Text);
                cmd.Parameters.AddWithValue("@MobileNo", txtno.Text);
                cmd.Parameters.AddWithValue("@Message", txtmsg.Text);
               
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                lblmsg.Text = " Registered Successfully...";
                clear();
            }
            catch (Exception ex) { lblmsg.Text = "Please try again......" + ex.Message; }
        }
       
    }
}