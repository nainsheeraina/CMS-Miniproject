using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Security;

namespace Complaint_Management_System.User
{
    public partial class ComplaintRegistration : System.Web.UI.Page
    {
        clsDataAccessMACET cls = new clsDataAccessMACET();
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnSubmit_Click(Object sender,EventArgs e)
        {
         
            SqlCommand cmd = new SqlCommand("insert into ComplaintRegistration (ComplaintId,ComplaintDate,ComplaintTitle,AttachmentFile,ComplaintNature,Complaint) values(@ComplaintId,@ComplaintDate,@ComplaintTitle,@AttachmentFile,@ComplaintNature,@Complaint)");
            
        }
    }
}