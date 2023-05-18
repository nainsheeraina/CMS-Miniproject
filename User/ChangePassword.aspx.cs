using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Security;

namespace Complaint_Management_System.User
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        clsDataAccessMACET cls = new clsDataAccessMACET();
        protected void Page_Load(object sender, EventArgs e)
        {

        }
     

        protected void btnChangePassword_Click1(object sender, EventArgs e)
        {

            try
            {
                if (Convert.ToString(Session["password"]) != txtOldPassword.Text.Trim())
                {
                    lblmsg.Text = "Invalid Old Password";
                    txtOldPassword.Focus();
                    return;
                }


                if (txtNewPassword.Text.Trim() != txtConfirmPassword.Text.Trim())
                {
                    lblmsg.Text = "New Password Not Match with Confirm Password";
                    return;
                }

                string sql = @"  update dbo.Registration set password='" + txtNewPassword.Text.Trim() + "' where emailid='" + Convert.ToString(Session["UserId"]) + "'";

                int i = cls.ExecuteSql(sql);
                if (i > 0)
                {
                    lblmsg.Text = "Your Password has been successfully changed...";
                }
            }
            catch (Exception ex) { lblmsg.Text = "Please try again.."+ex.Message; }
        }
    }
}
