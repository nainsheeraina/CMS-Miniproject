using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Complaint_Management_System
{
    public partial class _Default : Page
    {
        clsDataAccessMACET cls = new clsDataAccessMACET();
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        private void LoadNotice()
        {
            DataTable dt = cls.GetDataTable("select top(10) IsNew, ID,NoticeDate,NoticeSubject, PublishDate, NewFileName from dbo.LatestNew where Status='Y' order by NoticeDate DESC ");
            if (dt.Rows.Count > 0)
            {
                rpNews.DataSource = dt;
                rpNews.DataBind();
            }
        }
        protected void rpNews_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item)
            {
                HiddenField IsNew = (HiddenField)e.Item.FindControl("hdfIsNew");
                Image imgNew = (Image)e.Item.FindControl("Image1");
                if (IsNew.Value == "1")
                {
                    imgNew.Visible = true;
                }
                else { imgNew.Visible = false; }

            }
        }
    }
}