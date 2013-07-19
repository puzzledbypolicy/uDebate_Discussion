/*
' Copyright (c) 2010  DotNetNuke Corporation
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Modules.uDebate_Discussion.Components;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Installer.Log;
using DotNetNuke.Services.Mail;
using System.Collections;
using System.Configuration;
using System.Data.SqlClient;
using Telerik.Web.UI;
using System.Threading;


namespace DotNetNuke.Modules.uDebate_Discussion
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The ViewuDebate_Discussion class displays the content
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : uDebate_DiscussionModuleBase
    {
        private Logger log = new Logger();
        private string Thread_ID;

        /* Change the title of the page to the title of the current thread.
         * Also used for facebook like button, which uses this as the title 
         * to the newsitem it publishes to the timeline
         */
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            string title = getDescription(ATC.Tools.URLParam("Thread"));
            if (!title.Equals(""))
                ((DotNetNuke.Framework.CDefault)this.Page).Title = title;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Thread_ID = ATC.Tools.URLParam("Thread");
            string culture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

            /* IF no topic is passed we redirect the user to the udebta start page*/
            if (Thread_ID.Equals(""))
            {
                Response.Redirect(ConfigurationManager.AppSettings["DomainName"] + /*"/" + culture + */"/udebate.aspx");
            }

            Page.ClientScript.RegisterClientScriptInclude("hoverintent", ResolveUrl("~/Resources/Shared/Scripts/jquery/jquery.hoverIntent.min.js"));
            Page.ClientScript.RegisterClientScriptInclude("scroll", ResolveUrl("~/DesktopModules/uDebate_Discussion/Components/jquery.scrollTo-1.4.3.1-min.js"));

            log.AddInfo("loading discussion tree " + Thread_ID);
            ctlBreadcrump.CurrentModuleId = ModuleId.ToString();
            Page.MaintainScrollPositionOnPostBack = true;

            /* uncomment for multilingual sites */ 
            //LocalResourceFile = Localization.GetResourceFile(this, "View.ascx." + culture + ".resx");

            legendIssueImg.ImageUrl = "images/issue_icon.gif";
            legendIssueLbl.Text = Localization.GetString("Issue", LocalResourceFile);
            legendAltImg.ImageUrl = "images/alter_icon.gif";
            legendAltLbl.Text = Localization.GetString("Alternative", LocalResourceFile);
            legendProImg.ImageUrl = "images/pro_icon.gif";
            legendProLbl.Text = Localization.GetString("ProArgument", LocalResourceFile);
            legendConImg.ImageUrl = "images/con_icon.gif";
            legendConLbl.Text = Localization.GetString("ConArgument", LocalResourceFile);
            legendCommentImg.ImageUrl = "images/comments_icon.gif";
            legendCommentLbl.Text = Localization.GetString("Comment", LocalResourceFile);

            log.AddInfo("finished loading discussion tree " + Thread_ID);

            printLink.Attributes.Add("onclick", "openTreeViewPrinter(" + Thread_ID + ");return false;");

            string postAction = ATC.Tools.URLParam("PostAction").ToString();
            if (postAction != string.Empty && DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
                ChangePostStatus(postAction);

            if (!Request.IsAuthenticated)
            {
                notifyCheck.Visible = false;
                notifyLabel.Visible = false;
            }
            if (!IsPostBack)
            {
                notifyCheck.Checked = checkUsedNotified(UserId.ToString(), Thread_ID);
                IncViewCounter();

                DebateList.DataKeyNames = new string[] { "ID" };
                DebateList.ParentDataKeyNames = new string[] { "ParentID" };
                DebateList.DataSourceID = "SqluDebatePosts";
                DebateList.AutoGenerateColumns = false;
                SqluDebatePosts.SelectParameters["ThreadID"].DefaultValue = Thread_ID;
                SqluDebateThread.SelectParameters["ThreadID"].DefaultValue = Thread_ID;


                bool ThreadViewPermission = false;
                if (Request.IsAuthenticated && Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString() == getUserIdByThread()
                    ||
                    (Request.IsAuthenticated && Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString() == getUserIdByTopic())
                    ||
                    (DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
                   )
                {
                    ThreadViewPermission = true;
                }

                /* Check if the user is an admin and therefore all posts should be displayed */
                if (Request.IsAuthenticated && ThreadViewPermission == true)
                {
                    SqluDebatePosts.SelectParameters["Authorized"].DefaultValue = "1";
                }

                TreeListBoundColumn postTypeColumn = new TreeListBoundColumn();
                DebateList.Columns.Add(postTypeColumn);
                postTypeColumn.DataField = "PostType";
                postTypeColumn.UniqueName = "PostType";
                postTypeColumn.HeaderText = "PostType";

                TreeListBoundColumn subjectColumn = new TreeListBoundColumn();
                DebateList.Columns.Add(subjectColumn);
                subjectColumn.DataField = "Subject";
                subjectColumn.UniqueName = "Subject";
                subjectColumn.HeaderText = "Subject";

                TreeListBoundColumn dateColumn = new TreeListBoundColumn();
                DebateList.Columns.Add(dateColumn);
                dateColumn.DataField = "PostDate";
                dateColumn.UniqueName = "PostDate";
                dateColumn.HeaderText = "PostDate Date";
                dateColumn.DataType = typeof(DateTime);
                dateColumn.ItemStyle.CssClass = "postDate";
                dateColumn.DataFormatString = "{0:dd-MM-yyyy, HH:mm} " + Localization.GetString("PostBy", LocalResourceFile) + ": ";

                TreeListBoundColumn authorColumn = new TreeListBoundColumn();
                DebateList.Columns.Add(authorColumn);
                authorColumn.DataField = "Post_Author";
                authorColumn.UniqueName = "Post_Author";
                authorColumn.HeaderText = "Post Author";

                TreeListBoundColumn publishedColumn = new TreeListBoundColumn();
                DebateList.Columns.Add(publishedColumn);
                publishedColumn.DataField = "IsPublished";
                publishedColumn.UniqueName = "IsPublished";
                publishedColumn.Visible = false;

                TreeListBoundColumn userIDColumn = new TreeListBoundColumn();
                DebateList.Columns.Add(userIDColumn);
                userIDColumn.DataField = "UserId";
                userIDColumn.UniqueName = "UserId";
                userIDColumn.Visible = false;

                DebateList.ExpandedIndexes.Add(new TreeListHierarchyIndex { NestedLevel = 0, LevelIndex = 0 });
                DataRow lastPost = getLatestPostOfThread(Thread_ID);

                if (lastPost != null)
                    FindAndSelectItem(Convert.ToInt32(lastPost["ID"]));               
            }

            /* Styling */

            //subjectColumn
            DebateList.Columns[0].HeaderStyle.Width = Unit.Pixel(16);
            DebateList.Columns[0].ItemStyle.Width = Unit.Pixel(16);

            //subjectColumn
            //DebateList.Columns[1].HeaderStyle.Width = Unit.Percentage(71);
            //DebateList.Columns[1].ItemStyle.Width = Unit.Percentage(71);
            DebateList.Columns[1].ItemStyle.CssClass = "postTitle";

            //dateColumn
            DebateList.Columns[2].HeaderStyle.Width = Unit.Pixel(160);
            DebateList.Columns[2].ItemStyle.Width = Unit.Pixel(160);
            DebateList.Columns[2].ItemStyle.CssClass = "postDate";
            DebateList.Columns[2].ItemStyle.HorizontalAlign = HorizontalAlign.Right;

            //authorColumn
            DebateList.Columns[3].HeaderStyle.Width = Unit.Pixel(100);
            DebateList.Columns[3].ItemStyle.Width = Unit.Pixel(100);
            DebateList.Columns[3].ItemStyle.CssClass = "postAuthor";
        }

        private void IncViewCounter()
        {
            string ThreadID = ATC.Tools.IntURLParam("Thread", "-1").ToString();
            if (ThreadID == "-1" || DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders")
                || DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
                return;
            try
            {
                ATC.Database.sqlExecuteCommand("UPDATE uDebate_Forum_Threads SET View_Count = View_Count + 1 WHERE ID=" +
                    ThreadID + " AND View_Count is not null");
            }
            catch
            {
            }
        }


        //        // Take all attachments from the attachment control and change their associated post-id to be the id of 
        //        // the new post that was just created
        //        public void saveUserFilesInPost(IDbConnection Connection, IDbTransaction Transaction, string PostId, bool isNewPost)
        //        {
        //            string lstAttachmentIDs = string.Empty;

        //            // Check if it is a New Discussion or a new Post and take the respective Attachmetn control
        //            if (!isNewPost)
        //                lstAttachmentIDs = ctlAddAttachment.lstAttachmentIDs; // comma separated list of ids
        //            else
        //                lstAttachmentIDs = ctlNewAttachment.lstAttachmentIDs;

        //            if (lstAttachmentIDs != string.Empty)
        //            {
        //                string[] stringSeparators = new string[] { ";" };
        //                string[] attachIDs = lstAttachmentIDs.Split(stringSeparators, System.StringSplitOptions.None);

        //                foreach (string attachID in attachIDs)
        //                {
        //                    if (attachID != string.Empty)
        //                    {
        //                        String cmd = string.Empty;
        //                        cmd = "UPDATE uDebate_Attachments SET PostId = " + PostId + "WHERE AttachmentID = " + attachID;
        //                        ATC.Database.sqlExecuteCommand(cmd, Connection, Transaction);
        //                    }
        //                }
        //            }
        //        }


        public void ChangePostStatus(string postAction)
        {
            string postID = ATC.Tools.IntURLParam("Post", "-1").ToString();
            if (postID != "-1")
            {
                string published = "";
                string active = "";

                if (postAction == "Accept")
                {
                    active = "1";
                    published = "1";
                    string SQL = "UPDATE uDebate_Forum_Posts SET Published_Date = GetDate(), IsPublished = " +
                                  published + ", Active = " + active + " WHERE ID = " + postID;
                    ATC.Database.sqlExecuteCommand(SQL);
                }
                else if (postAction == "Reject")
                {
                    active = "0";
                    published = "0";
                    string SQL = "UPDATE uDebate_Forum_Posts SET IsPublished = " + published + ", Active = " + active + " WHERE ID = " + postID;
                    ATC.Database.sqlExecuteCommand(SQL);

                    string notifySQL = @"SELECT Users.Email FROM Users 
                                         INNER JOIN uDebate_Forum_Posts on Users.UserID=uDebate_Forum_Posts.UserID
                                         WHERE uDebate_Forum_Posts.ID =" + postID;

                    string notifyEmail = ATC.Database.sqlGetFirst(notifySQL);

                    string notifyBody = String.Format(Localization.GetString("UnpublishedBody", LocalResourceFile),
                                        ConfigurationManager.AppSettings["DomainName"] + /*"/" +
                                        System.Threading.Thread.CurrentThread.CurrentCulture.Name +*/
                                        "/udebatediscussion.aspx?Thread=" + ATC.Tools.IntURLParam("Thread"));

                    Mail.SendEmail("discussion@ogpireland.ie", notifyEmail, Localization.GetString("UnpublishedSubject", LocalResourceFile),
                                    notifyBody);

                    return;
                }
                else if (postAction == "Delete")
                {
                    ATC.Database.sqlExecuteCommand("UPDATE uDebate_Forum_Threads SET Delete_Count = Delete_Count + 1 WHERE ID = (SELECT ThreadID FROM uDebate_Forum_Posts WHERE ID = " + postID + ")");
                    ATC.Database.sqlExecuteCommand("DELETE FROM uDebate_Forum_Posts WHERE ID = " + postID);
                    return;
                }
                else
                {
                    return;
                }
            }
        }

        public string getPostLanguageByThread(string threadID)
        {
            string sOut = string.Empty;
            string sSQL = @"SELECT [Language]
                  FROM [uDebate_Forum_Threads]
                where [ID]=" + threadID;
            try
            {
                sOut = ATC.Database.sqlGetFirst(sSQL);
            }
            catch (Exception x)
            {
            }
            return sOut;
        }

        public string getUserId(string postID)
        {
            string sOut = string.Empty;
            string sUser = string.Empty;

            string sSQL = @"SELECT Users.UserID,Users.Username,Users.FirstName, uDebate_Forum_Posts.PostDate
                            FROM  uDebate_Forum_Posts INNER JOIN
                            Users ON uDebate_Forum_Posts.UserID = Users.UserID
                            WHERE (uDebate_Forum_Posts.ID = " + postID + ")";
            try
            {
                DataTable dt = ATC.Database.sqlExecuteDataTable(sSQL);

                if (dt.Rows.Count > 0)
                {
                    DataRow dtr = dt.Rows[0];
                    DateTime postDate = (DateTime)dtr["PostDate"];
                    if (dtr["FirstName"] != null && dtr["FirstName"].ToString() != "")
                        sUser = sUser + dtr["FirstName"].ToString();
                    else
                        sUser = sUser + dtr["Username"].ToString();
                    sOut = "<div class='postDate'> " + postDate.ToString("dd-MMM-yyyy")
                          + ", <span class='time'>" + postDate.ToString("HH:mm")
                          + " " + Localization.GetString("PostBy", LocalResourceFile)
                          + ":</span><a href='" + DotNetNuke.Common.Globals.UserProfileURL(Convert.ToInt32(dtr["UserID"]))
                          + "'>" + sUser + "</a></div>";
                }
            }
            catch (Exception x)
            {
            }
            return sOut;
        }

        public string getUserIdByThread()
        {
            string sUserId = ATC.Database.sqlGetFirst("SELECT [UserID] FROM [uDebate_Forum_Threads] where [ID]=" +
                            ATC.Tools.URLParam("Thread"));
            return sUserId;
        }

        public string getUserIdByTopic()
        {
            string sUserId = ATC.Database.sqlGetFirst("SELECT [UserID] FROM [uDebate_Forum_Topics] where [ID]=" +
                            ATC.Tools.URLParam("Thread"));
            return sUserId;
        }

        public string getAttachedFilesByPostId(string PostId)
        {
            string SQL = @"SELECT [ID],[UserTitle],[FileTitle],[FileSize],[FileType],[PostId]
                        FROM [uDebate_Forum_Post_Files] where PostId=" + PostId;

            string sHTML = string.Empty;

            AttachmentController cntAttachment = new AttachmentController();
            List<AttachmentInfo> lstFiles = null;

            if (Convert.ToInt32(PostId) > 0)
            {
                lstFiles = cntAttachment.GetAllByPostID(Convert.ToInt32(PostId));
            }

            if (lstFiles.Count > 0)
            {
                sHTML += "<table class='attachments' cellspacing='0' cellpadding='0' width='160px'>";
                sHTML += "<td style='border-top: 1px solid #FDFDFD; padding:5px 3px;width:24px;'> <img src='" + ATC.Tools.GetParam("RootURL") +
                                     @"DesktopModules/uDebate/images/s_attachment.png' align=middle'> </td> ";
                foreach (AttachmentInfo objFile in lstFiles)
                {
                    string strlink = Common.Globals.LinkClick("FileID=" + objFile.FileID, PortalSettings.ActiveTab.TabID, ModuleId, false, true);
                    string strFileName = objFile.LocalFileName;
                    sHTML += @" <td style='border-top: 1px solid #FDFDFD; padding: 5px 0;'><a href='" + strlink + "'>"
                            + strFileName.Substring(0, Math.Min(strFileName.Length, 15)).ToLower() + @"</a>&nbsp;</td>";
                }
                sHTML += "</table>";
            }
            return sHTML;
        }

        protected void notifyCheck_CheckedChanged(object sender, EventArgs e)
        {

            if (Request.IsAuthenticated)//make sure user is logged in
            {
                string userID = UserId.ToString();
                string userEmail = UserInfo.Email;               

                if (notifyCheck.Checked)
                {
                    String SQL = "INSERT INTO uDebate_Forum_Notifications (userID,threadID,userEmail,insertedOn) VALUES(" +
                                userID + "," + Thread_ID + ",'" + userEmail + "',getdate())";
                    ATC.Database.sqlExecuteCommand(SQL);
                    notifyCheck.Checked = true;
                }
                else
                {
                    String SQL = "DELETE FROM uDebate_Forum_Notifications WHERE userID=" + userID +
                                " AND threadID=" + Thread_ID;
                    ATC.Database.sqlExecuteCommand(SQL);
                    notifyCheck.Checked = false;
                }
            }
        }

        public bool checkUsedNotified(string userID, string threadID)
        {
            string sOut = string.Empty;
            string sSQL = @"SELECT [userID]
                  FROM [uDebate_Forum_Notifications]
                where [userID]=" + userID + " AND [threadID]=" + threadID;
            try
            {
                sOut = ATC.Database.sqlGetFirst(sSQL);
            }
            catch (Exception x)
            {
            }
            if (sOut != "")
                return true;
            return false;
        }

        /*****************       Version 2 functions                   **************************/


        protected void DebateList_ItemDataBound(object sender, TreeListItemDataBoundEventArgs e)
        {
            if (e.Item is TreeListDataItem)
            {
                TreeListDataItem itm = e.Item as TreeListDataItem;
                itm["PostType"].Text = "<img src='" + ModulePath + getImageIconUrl(itm["PostType"].Text) + "'>";

                HyperLink userProfile = new HyperLink();
                userProfile.NavigateUrl = DotNetNuke.Common.Globals.UserProfileURL(Convert.ToInt32(itm["UserId"].Text));


                if (!itm["Post_Author"].Text.Equals("&nbsp;"))
                {
                    userProfile.Text = itm["Post_Author"].Text + " " + itm.GetDataKeyValue("ID");
                   itm["Post_Author"].Controls.Add(userProfile);
                }
                else
                {
                    itm["Post_Author"].Text = "guest";
                }
                

                if (itm.IsChildInserted)
                {
                    itm.CssClass = "whiteBack";
                }
            }
            else if (e.Item is TreeListDetailTemplateItem)
            {
                TreeListDetailTemplateItem itm = e.Item as TreeListDetailTemplateItem;

                HiddenField hiddenpostId = (HiddenField)itm.FindControl("hiddenPostID");
                /*LinkButton printPostBtn = (LinkButton)itm.FindControl("printPostLink");
                printPostBtn.Attributes.Add("onclick", "openSelectedPostPrinter(" + hiddenpostId.Value + ");return false;");*/


                if (DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
                {
                    HyperLink publishPost = (HyperLink)itm.FindControl("publishPost");
                    HyperLink deletePost = (HyperLink)itm.FindControl("deletePost");
                    publishPost.Visible = true;
                    deletePost.Visible = true;
                    string href = Request.RawUrl.Substring(0, Request.RawUrl.IndexOf("Thread") + 7) + ATC.Tools.URLParam("Thread");

                    /* if post is published admins can unpublish or delete */
                    deletePost.NavigateUrl = href + "&PostAction=Delete&Post=" + hiddenpostId.Value;
                    if (itm.ParentItem["IsPublished"].Text == "1")
                    {
                        publishPost.NavigateUrl = href + "&PostAction=Reject&Post=" + hiddenpostId.Value;
                        publishPost.Text = "Unpublish";
                    }
                    else // Post unpublished
                    {
                        publishPost.NavigateUrl = href + "&PostAction=Accept&Post=" + hiddenpostId.Value;
                        publishPost.Text = "Re-publish";
                    }
                }

                if (statusLbl.Visible) // Thread is closed
                {
                    LinkButton newPostBtn = (LinkButton)itm.FindControl("newPostBtn");
                    newPostBtn.Enabled = false;
                    notifyCheck.Visible = false;
                    notifyLabel.Visible = false;
                }
                else
                {
                    if (itm.ParentItem.IsChildInserted)
                    {
                        //itm.CssClass = "whiteBack";
                        LinkButton newPostBtn = (LinkButton)itm.FindControl("newPostBtn");
                        newPostBtn.CssClass = "newPostActive";
                    }
                }
            }
            else if (e.Item is TreeListEditFormInsertItem)
            {
                /* If a new discussion is to be created, the post type can only be issue*/
                if (DebateList.IsItemInserted)
                {
                    TreeListEditFormInsertItem itm = e.Item as TreeListEditFormInsertItem;
                    RadioButton Issue = (itm.FindControl("IssueRadio") as RadioButton);
                    RadioButton Alter = (itm.FindControl("AlterRadio") as RadioButton);
                    RadioButton Pro = (itm.FindControl("ProRadio") as RadioButton);
                    RadioButton Con = (itm.FindControl("ConRadio") as RadioButton);
                    RadioButton Comment = (itm.FindControl("CommentRadio") as RadioButton);
                    Comment.Checked = false;
                    Issue.Checked = true;
                    Alter.Enabled = false;
                    Pro.Enabled = false;
                    Con.Enabled = false;
                    Comment.Enabled = false;
                }
            }
        }

        protected void expandLink_Click(Object sender, EventArgs e)
        {
            DebateList.ExpandAllItems();
        }

        protected void newDiscussionBtn_Click(Object sender, EventArgs e)
        {
            DebateList.IsItemInserted = true;
            DebateList.Rebind();
        }

        protected void DebateList_ItemCommand(object source, TreeListCommandEventArgs e)
        {
            if (e.CommandName == "replyMessage")
            {
                if (e.Item is TreeListDetailTemplateItem)
                {
                    TreeListDetailTemplateItem itm = e.Item as TreeListDetailTemplateItem;
                    if (!Request.IsAuthenticated)
                    {
                        needLoginLblTop.Visible = true;
                        needLoginLblBottom.Visible = true;
                        needLoginLblTop.Text = "<div class='dnnFormMessage dnnFormWarning'>" +
                            Localization.GetString("LoggedIn", LocalResourceFile) +
                         "  <a href='" + ConfigurationManager.AppSettings["DomainName"] + /*"/" +
                         System.Threading.Thread.CurrentThread.CurrentCulture.Name +*/
                         "/login.aspx?returnurl=" + HttpUtility.UrlEncode(Request.Url.PathAndQuery) + "'>" +
                         Localization.GetString("Login", LocalResourceFile) + "</a></div>";
                        needLoginLblBottom.Text = needLoginLblTop.Text;
                    }
                    else
                    {
                        itm.CssClass = "whiteBack";
                        LinkButton newPostBtn = (LinkButton)itm.FindControl("newPostBtn");
                        newPostBtn.CssClass = "newPostActive";
                        TreeListDataItem parentPost = itm.ParentItem;
                        parentPost.IsChildInserted = true;
                        DebateList.Rebind();
                    }

                }
            }
        }

        protected void DebateList_InsertCommand(object sender, TreeListCommandEventArgs e)
        {
            string ConnString = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString;
            string commandText = @"INSERT INTO uDebate_Forum_Posts (ThreadID, ParentID, UserID, PostLevel, SortOrder, Subject,
                                   Message, PostDate, IsPublished, PostType, Active, Published_Date, Complaint_Count, ModuleID)
                                   VALUES (@ThreadID, @ParentID, " + UserInfo.UserID + @", 1, 1, @Subject, @Message, getDate(), 1,
                                           @PostType,1,getDate(),0," + ModuleId + ")";

            SqlConnection conn = new SqlConnection(ConnString);
            SqlCommand command = new SqlCommand(commandText, conn);
            Hashtable table = new Hashtable();
            TreeListEditFormInsertItem item = e.Item as TreeListEditFormInsertItem;
            table["Subject"] = (item.FindControl("txtSubjectPost") as TextBox).Text;
            table["Message"] = (item.FindControl("txtReply") as DotNetNuke.Web.UI.WebControls.DnnEditor).GetHtml(EditorStripHtmlOptions.None).Replace("'", "\"");

            RadioButton Issue = (item.FindControl("IssueRadio") as RadioButton);
            RadioButton Alter = (item.FindControl("AlterRadio") as RadioButton);
            RadioButton Pro = (item.FindControl("ProRadio") as RadioButton);
            RadioButton Con = (item.FindControl("ConRadio") as RadioButton);
            RadioButton Comment = (item.FindControl("CommentRadio") as RadioButton);

            String selection = String.Empty;

            if (Issue.Checked)
            {
                selection = "1";
            }
            else if (Alter.Checked)
            {
                selection = "2";
            }
            else if (Pro.Checked)
            {
                selection = "3";
            }
            else if (Con.Checked)
            {
                selection = "4";
            }
            else if (Comment.Checked)
            {
                selection = "8";
            }

            table["PostType"] = selection;
            command.Parameters.AddWithValue("ThreadID", Thread_ID);
            command.Parameters.AddWithValue("Subject", table["Subject"]);
            command.Parameters.AddWithValue("Message", table["Message"]);
            command.Parameters.AddWithValue("PostType", table["PostType"]);

            object parentValue;
            if (item.ParentItem != null)
            {
                parentValue = item.ParentItem.GetDataKeyValue("ID");
            }
            else
            {
                parentValue = "0";
            }

            command.Parameters.AddWithValue("ParentID", parentValue);

            conn.Open();
            try
            {
                command.ExecuteNonQuery();
                /*if the new post is a reply we have to disable edit mode of parent and expand it */
                if (item.ParentItem != null)
                {
                    item.ParentItem.Expanded = true;
                    item.ParentItem.IsChildInserted = false;
                }
                DebateList.IsItemInserted = false;             

                DebateList.Rebind();
                DataRow lastPost = getLatestPostOfThread(Thread_ID);
                if (lastPost != null)
                    FindAndSelectItem(Convert.ToInt32(lastPost["ID"]));  
               
            }
            finally
            {
                conn.Close();
            }

            /* If a user posts to a thread we add him to the notification list*/
            AddUserToNotified(Thread_ID);
            notifyCheck.Checked = true;

            string fromAddress = "discussion@ogpireland.ie";
            string subject = "OGP Ireland - New Post";
            string body = "Hi, <br /><br/>A new post has been submitted to the OGP Ireland thread \"<b>" +
                         getDescription(Thread_ID) + "\"</b>.<br /> To see this post, visit " +
                        ConfigurationManager.AppSettings["DomainName"] +/* "/" +
                        System.Threading.Thread.CurrentThread.CurrentCulture.Name +*/
                        "/udebatediscussion.aspx?Thread=" + Thread_ID + "<br /><br/>Kind Regards,<br /><br/>"+
                        PortalSettings.PortalName + "<br /><a href='" + PortalSettings.DefaultPortalAlias +
                        "'>" + PortalSettings.DefaultPortalAlias + "</a>" + "<br /><br />" +
                        "<img src='http://" + PortalSettings.DefaultPortalAlias + "/Portals/0/pbp_logo270.jpg'/>";

            SendTokenizedBulkEmail mailer = new SendTokenizedBulkEmail();           

            /* Notify moderators of the new post*/
            switch (getPostLanguageByThread(Thread_ID).ToLower())
            {
                case "el-gr": 
                     Entities.Users.UserInfo user = new Entities.Users.UserInfo();
                     user.Email = "i.giannakoudaki@daem.gr";
                     mailer.AddAddressedUser(user);  

                    break;
               /* case "es-es":
                    Entities.Users.UserInfo user3 = new Entities.Users.UserInfo();
                    user3.Email = "enielsen@ull.es";
                     mailer.AddAddressedUser(user3);
                     Entities.Users.UserInfo user4 = new Entities.Users.UserInfo();
                    user4.Email = "cmartinv@ull.es";
                     mailer.AddAddressedUser(user4);
                     Entities.Users.UserInfo user5 = new Entities.Users.UserInfo();
                     user5.Email = "vzapata@ull.es";
                     mailer.AddAddressedUser(user5); 
                    break;
                case "it-it":
                    Entities.Users.UserInfo user7 = new Entities.Users.UserInfo();
                    user7.Email = "stefano.moro@csi.it";
                     mailer.AddAddressedUser(user7); 
                    break;
                case "hu-hu": 
                    Entities.Users.UserInfo user9 = new Entities.Users.UserInfo();
                    user9.Email = "Takacs.GyulaPeter@nisz.hu";
                     mailer.AddAddressedUser(user9); 
                    break;
                case "en-gb":
                    Entities.Users.UserInfo user11 = new Entities.Users.UserInfo();
                    user11.Email = "matej.delakorda@inepa.si";
                     mailer.AddAddressedUser(user11);
                     Entities.Users.UserInfo user12 = new Entities.Users.UserInfo();
                     user12.Email = "mateja.delakorda@inepa.si";
                     mailer.AddAddressedUser(user12);
                    break;*/
                default:   break;
            }

            Entities.Users.UserInfo user14 = new Entities.Users.UserInfo();
            user14.Email = "l.kallipolitis@atc.gr";
            mailer.AddAddressedUser(user14);
           
            mailer.Priority = DotNetNuke.Services.Mail.MailPriority.Normal;

            mailer.AddressMethod = DotNetNuke.Services.Mail.SendTokenizedBulkEmail.AddressMethods.Send_TO;

            Entities.Users.UserInfo senderUser = new Entities.Users.UserInfo();
            senderUser.Email = "discussion@ogpireland.ie";

            mailer.SendingUser = senderUser;

            mailer.ReportRecipients = false;

            mailer.Subject = subject;

            mailer.Body = body;

            mailer.BodyFormat = DotNetNuke.Services.Mail.MailFormat.Html;

            Thread objThread = new Thread(mailer.Send);

            objThread.Start();

            /* Send an email to all the subscribed users of this thread*/
            string subjectNotify = "OGP Ireland - There is a new post in the thread you are following";
            string bodyNotify = "Hi, <br /><br/>A new post has been submitted to the OGP Ireland thread \"<b>" +
                         getDescription(Thread_ID) + "\"</b>.<br /> To see this post, visit " +
                        ConfigurationManager.AppSettings["DomainName"] + /*"/" +
                        System.Threading.Thread.CurrentThread.CurrentCulture.Name +*/
                        "/udebatediscussion.aspx?Thread=" + Thread_ID + "<br /><br/>Kind Regards,<br /><br/>"+
                        PortalSettings.PortalName + "<br /><a href='" + PortalSettings.DefaultPortalAlias +
                        "'>" + PortalSettings.DefaultPortalAlias + "</a>" + "<br /><br />" +
                        "<img src='http://" + PortalSettings.DefaultPortalAlias + "/Portals/0/pbp_logo270.jpg'/>";


            string SQL_notified = "SELECT userID,userEmail FROM uDebate_Forum_Notifications where threadID=" + Thread_ID;
            try
            {
                DataSet ds = ATC.Database.sqlExecuteDataSet(SQL_notified);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    SendTokenizedBulkEmail notificationMailer = new SendTokenizedBulkEmail();

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        // Only send email to users different than the current one (the post writer)
                        if (UserId != Int32.Parse(row["userID"].ToString()))
                        {
                            int numRecs = 100;
                            ArrayList findEmailinRegistered = Entities.Users.UserController.GetUsersByEmail(PortalId, row["userEmail"].ToString(), 0, 10, ref numRecs,false,false);

                            //Check that the user is still registered
                            if (findEmailinRegistered.Count > 0)
                            {
                                Entities.Users.UserInfo newUser = new Entities.Users.UserInfo();
                                newUser.Email = row["userEmail"].ToString();
                                notificationMailer.AddAddressedUser(newUser);
                            }
                            //if no, remove him from the list of notified users
                            else
                            {
                                RemoveUserFromNotified(row["userEmail"].ToString());
                             
                            }
                        }                          
                    }

                    notificationMailer.Priority = DotNetNuke.Services.Mail.MailPriority.Normal;

                    notificationMailer.AddressMethod = DotNetNuke.Services.Mail.SendTokenizedBulkEmail.AddressMethods.Send_TO;

                    Entities.Users.UserInfo sendingUser = new Entities.Users.UserInfo();
                    sendingUser.Email = "discussion@ogpireland.ie";

                    notificationMailer.SendingUser = sendingUser;

                    notificationMailer.ReportRecipients = true;

                    notificationMailer.Subject = subjectNotify;

                    notificationMailer.Body = bodyNotify;

                    notificationMailer.BodyFormat = DotNetNuke.Services.Mail.MailFormat.Html;

                    Thread objThread1 = new Thread(notificationMailer.Send);

                    objThread1.Start();
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }


        protected void ThreadDetails_DataBound(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                string culture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                newDiscussionBtn.Visible = true;
                newDiscussionBtn.CssClass = "newDiscussion disc_" + culture;

                if (DotNetNuke.Security.PortalSecurity.IsInRoles("Pilot Leaders"))
                {
                    String TopicId = "1";
                    HiddenField hiddenTopicId = (HiddenField)((DetailsView)sender).FindControl("TopicId");
                    if (hiddenTopicId != null)
                    {
                        TopicId = hiddenTopicId.Value;
                    }

                    EditLink.NavigateUrl = ConfigurationManager.AppSettings["DomainName"] + "/tabid/" +
                            PortalSettings.ActiveTab.TabID + "/ctl/Edit/mid/" + ModuleId +
                           "/TopicID/" + TopicId + "/ThreadID/" + Thread_ID +
                           /*"/language/" + culture +*/ "/default.aspx";
                    EditLink.ImageUrl = ATC.Tools.GetParam("RootURL") + "Images/manage-icn.png";
                    EditLink.Attributes.Add("onclick", "return " + UrlUtils.PopUpUrl(EditLink.NavigateUrl, this, PortalSettings, true, false));
                    EditLink.Visible = true;

                    /* check if summary exists to know if we have posts */
                    Label summaryLabel = ThreadDetails.Rows[0].Cells[0].FindControl("summaryLbl") as Label;
                    if (summaryLabel != null)
                    {
                        /* Summary Button */
                        threadSummaryBtn.Visible = true;
                        threadSummaryBtn.NavigateUrl = ConfigurationManager.AppSettings["DomainName"] + "/tabid/" +
                                PortalSettings.ActiveTab.TabID + "/ctl/TreeViewSummary/mid/" + ModuleId +
                                "/ThreadID/" + Thread_ID +
                                /*"/language/" + culture + */"/uDebate.aspx";
                    }
                }
            }
            /* If status is not null then the thread has posts >0 so we update the fields accordingly */
            String Status = "";
            HiddenField hiddenstatus = (HiddenField)((DetailsView)sender).FindControl("Status");
            Status = hiddenstatus.Value;
            if (Status != "1")
                statusLbl.Visible = true;
            Label summaryLbl = ThreadDetails.Rows[0].Cells[0].FindControl("summaryLbl") as Label;
            summaryLbl.Text = Server.HtmlDecode(summaryLbl.Text);
            HyperLink creatorLbl = ThreadDetails.Rows[0].Cells[0].FindControl("creatorLbl") as HyperLink;
            HiddenField userID = (HiddenField)((DetailsView)sender).FindControl("userID");
            creatorLbl.NavigateUrl = DotNetNuke.Common.Globals.UserProfileURL(Convert.ToInt32(userID.Value));
        }


        public DataRow getLatestPostOfThread(string threadID)
        {
            DataRow result = null;

            string sSQL = @"SELECT TOP (1) posts.ID
                            FROM uDebate_Forum_Posts AS posts 
                            WHERE     (posts.ThreadID =" + threadID +
                            ") ORDER BY posts.PostDate DESC ";
            try
            {
                result = ATC.Database.sqlExecuteDataRow(sSQL);
            }
            catch (Exception x)
            {
            }
            return result;
        }


        private void FindAndSelectItem(int id)
        {
            //first check if the item is not already in the Items collection
            foreach (var item in DebateList.Items)
            {
                if ((int)item.GetDataKeyValue("ID") == id)
                {
                    item.Selected = true;
                    return;
                }
            }

            //save the previously expanded indexes
            var expandedIndexes = new TreeListHierarchyIndex[DebateList.ExpandedIndexes.Count];
            DebateList.ExpandedIndexes.CopyTo(expandedIndexes);

            //add newly expanded indexes here
            var newIndexes = new List<TreeListHierarchyIndex>();

            //cause all items to expand to reveal all data
            DebateList.ExpandAllItems();

            //loop through all the items and search for the target by its key value
            foreach (var item in DebateList.Items)
            {
                if ((int)item.GetDataKeyValue("ID") == id)
                {
                    //select the item
                    item.Selected = true;

                    //expand all parents of the item
                    var parent = item.ParentItem;
                    while (parent != null)
                    {
                        newIndexes.Add(parent.HierarchyIndex);
                        parent = parent.ParentItem;
                    }
                }
            }

            //shrink back the treelist
            DebateList.CollapseAllItems();

            //restore the previously expanded indexes
            DebateList.ExpandedIndexes.AddRange(expandedIndexes);

            //add the newly expanded indexes that will reveal the selected item
            foreach (var index in newIndexes)
            {
                if (!DebateList.ExpandedIndexes.Contains(index))
                {
                    DebateList.ExpandedIndexes.Add(index);
                }
            }

            //rebind to reflect the changes
            DebateList.Rebind();
        }


        public string getImageIconUrl(string sID)
        {
            string sOut = string.Empty;
            switch (sID)
            {
                case "1":
                    sOut = "issue_icon.gif";
                    break;
                case "2":
                    sOut = "alter_icon.gif";
                    break;
                case "3":
                    sOut = "pro_icon.gif";
                    break;
                case "4":
                    sOut = "con_icon.gif";
                    break;
                case "5":
                    sOut = "question_icon.gif";
                    break;
                case "6":
                    sOut = "answer_icon.gif";
                    break;
                case "7":
                    sOut = "comments_icon.gif";
                    break;
                case "8":
                    sOut = "comments_icon.gif";
                    break;
                default:
                    sOut = "issue_icon.gif";
                    break;
            }
            return "images/" + sOut;
        }

        public string getDescription(string threadID)
        {
            string sOut = string.Empty;
            string sSQL = @"SELECT [Description]
                  FROM [uDebate_Forum_Threads]
                where [ID]=" + threadID;
            try
            {
                sOut = ATC.Database.sqlGetFirst(sSQL);
            }
            catch (Exception x)
            {
            }

            return sOut;
        }

        /*Adds a user to the notification list for the given thread*/
        private void AddUserToNotified(string threadID)
        {
            if (Request.IsAuthenticated)//make sure user is logged in
            {
                if (!checkUsedNotified(UserInfo.UserID.ToString(), threadID))
                {                    
                    String SQL = "INSERT INTO uDebate_Forum_Notifications (userID,threadID,userEmail,insertedOn) VALUES(" +
                                        UserInfo.UserID + "," + Thread_ID + ",'" + UserInfo.Email + "',getdate())";
                    ATC.Database.sqlExecuteCommand(SQL);
                }
            }
        }

        /*Removes the given email from the notification list*/
        private void RemoveUserFromNotified(string userEmail)
        {
            String SQL = "DELETE FROM uDebate_Forum_Notifications WHERE userEmail='" + userEmail + "'";
            ATC.Database.sqlExecuteCommand(SQL);
        }

    }
}