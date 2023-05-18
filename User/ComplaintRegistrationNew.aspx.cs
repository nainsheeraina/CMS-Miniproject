using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Security;
using System.IO;

namespace Complaint_Management_System.User
{
    public partial class ComplaintRegistrationNew : System.Web.UI.Page
    {
        clsDataAccessMACET cls = new clsDataAccessMACET();
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public int complainId() {
            int complainid;
            string query = "  select MAX(ComplaintId) from ComplaintRegistration";
           DataTable dt= cls.GetDataTable(query);
            if (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
            {

                complainid = (int.Parse(dt.Rows[0][0].ToString()) + 1);
            }
            else
            {
                complainid = 101;
            }
            
        return complainid;
    }
        void clear()
        {
            txtComplaintDate.Text = "";
            txtComplaintTitle.Text = "";
            txtComplaintNature.Text = "";
            txtsuggestion.Text = "";
        }

        private string InsSaveFile(string fileName, FileUpload fuFile)
        {
            string serverFileName = string.Empty;
            string uploadDirectory = string.Empty;
            try
            {
                uploadDirectory = "~//Complain//";


                if (!Directory.Exists(Server.MapPath(uploadDirectory)))
                {
                    Directory.CreateDirectory(Server.MapPath(uploadDirectory));
                }

                if (fuFile.PostedFile.FileName != "")
                {
                    UploadFile(fuFile, uploadDirectory, fileName);
                    string extension = Path.GetExtension(fuFile.PostedFile.FileName);

                    serverFileName = Path.GetFileName(fileName + extension);

                }

            }
            catch (Exception ee)
            {

            }
            return uploadDirectory + serverFileName;
        }



        private void UploadFile(FileUpload Uploader, string uploadDirectory, string fileName)
        {
            string extension = Path.GetExtension(Uploader.PostedFile.FileName);
            string serverFileName = Path.GetFileName(fileName + extension);
            string fullUploadPath = Path.Combine(Server.MapPath(uploadDirectory), serverFileName);
            string fullUploadPathToDelete3 = Path.Combine(Server.MapPath(uploadDirectory), fileName + ".pdf");
            try
            {

                if (File.Exists(fullUploadPathToDelete3))
                    File.Delete(fullUploadPathToDelete3);

                Uploader.PostedFile.SaveAs(fullUploadPath);
                string rpath = uploadDirectory + "/" + serverFileName;
            }
            catch (Exception err)
            {
                lblmsg.Text = "Oops!! error occured : " + err.Message.ToString();
            }
        }



        bool validateFile(FileUpload fuFile, string FileType)
        {
            if (fuFile.HasFile)
            {
                int contentLength = fuFile.PostedFile.ContentLength;
                string extension = Path.GetExtension(fuFile.PostedFile.FileName);
                int count = fuFile.FileName.Split('.').Length - 1;
                if (count > 1)
                {
                    return false;
                    // Display error message to prevent double extension...  
                }

                switch (FileType)
                {
                    case "zip":
                        switch (extension.ToLower())
                        {
                            case ".zip":
                                break;
                            default:
                                lblmsg.Text = "This file type is not allowed.";
                                // ClientScript.RegisterStartupScript(this.GetType(), "msgFu", "alert('This file type is not allowed.');", true);
                                return false;
                        }

                        if (contentLength > (1 * 1024 * 1024))
                        {
                            lblmsg.Text = "File size must be less than or equal to 1 MB";
                            return false;
                        }
                        break;
                    case "doc":

                        switch (extension.ToLower())
                        {
                            //case ".jpg":
                            //case ".jpeg":
                            //case ".png":
                            case ".pdf":
                                break;
                            default:
                                lblmsg.Text = "This file type is not allowed.";
                                // ClientScript.RegisterStartupScript(this.GetType(), "msgFu", "alert('This file type is not allowed.');", true);
                                return false;
                        }
                        if (contentLength > (0.4 * 1024 * 1024))
                        {
                            lblmsg.Text = "File size must be less than or equal to 400KB";
                            return false;
                        }
                        break;

                    default:
                        lblmsg.Text = "Unknown File Type !!";
                        return false;
                }
            }
            return true;
        }





        void UploadDoc()
        {
            if (!validateFile(FileUpload1, "doc"))
                return;

            string _photoname = string.Empty;
            if (FileUpload1.HasFile)
            {
                _photoname = InsSaveFile("Photo_" + DateTime.Now.ToString("yyyyMMddhmmss").Trim() + "", FileUpload1);
                ViewState["PhotoName"] = _photoname;

            }



        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            lblmsg.Text = "";
            try
            {
                if (FileUpload1.HasFile)
                    UploadDoc();
                SqlConnection con = new SqlConnection("Data Source=(local);Initial Catalog=cms;User ID=sa;pwd=123");
                SqlCommand cmd = new SqlCommand("insert into ComplaintRegistration (ComplaintId,ComplaintDate,ComplaintTitle,AttachmentFile,ComplaintNature,Complaint) values(@ComplaintId,@ComplaintDate,@ComplaintTitle,@AttachmentFile,@ComplaintNature,@Complaint)");
                cmd.Parameters.AddWithValue("@ComplaintId", complainId());
                cmd.Parameters.AddWithValue("@ComplaintDate", txtComplaintDate.Text);
                cmd.Parameters.AddWithValue("@ComplaintTitle", txtComplaintTitle.Text);
                if (FileUpload1.HasFile)
                    cmd.Parameters.AddWithValue("@AttachmentFile", ViewState["PhotoName"].ToString().Trim());
                else
               
                    cmd.Parameters.AddWithValue("@AttachmentFile", DBNull.Value);
                
                cmd.Parameters.AddWithValue("@ComplaintNature", txtComplaintNature.Text);
                cmd.Parameters.AddWithValue("@Complaint", txtsuggestion.Text);
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
               lblmsg.Text= "Registered Seccessfully";
                clear();
            }
            catch (Exception ex) { lblmsg.Text = "Please try again......"+ex.Message; }

        }


    }
}