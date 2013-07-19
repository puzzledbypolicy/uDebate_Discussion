<%@ Control Language="C#" Inherits="DotNetNuke.Modules.uDebate_Discussion.View" AutoEventWireup="True"
    CodeBehind="View.ascx.cs" %>
<%@ Register Assembly="ATC.WebControls" Namespace="ATC.WebControls" TagPrefix="atc" %>
<%@ Register TagPrefix="uDebate_Discussion" TagName="Attachment" Src="~/DesktopModules/uDebate_Discussion/controls/AttachmentControl.ascx" %>
<%@ Register TagPrefix="uDebate_Discussion" TagName="Breadcrump" Src="~/DesktopModules/uDebate_Discussion/controls/ForumBreadcrumb.ascx" %>
<%@ Register Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls"
    TagPrefix="telerik" %>

<script type="text/javascript">

    function openTreeViewPrinter(threadID) {
        var sURL = '<%= ConfigurationManager.AppSettings["DomainName"]%>/DesktopModules/uDebate/ThreadsPostsPrintTreeView.aspx?threadID=' + threadID;
        //alert(sURL);
        myRef = window.open(sURL, 'mywinfiles', 'left=20,top=20,width=600,height=800,toolbar=1,resizable=1,scrollbars=1');
    }

    function openSelectedPostPrinter(postId) {
        var sURL = '<%= ConfigurationManager.AppSettings["DomainName"]%>/DesktopModules/uDebate/ThreadsPostsPrintPost.aspx?postID=' + postId;
        //alert(sURL);
        myRef = window.open(sURL, 'mywinfiles', 'left=20,top=20,width=600,height=800,toolbar=1,resizable=1,scrollbars=1');
    }

    jQuery(document).ready(function() {

        /* Click handler for the Clear menu item */
        jQuery('.admin_link').dnnConfirm({
            text: 'Are you sure?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Action Confirmation'
        });


        jQuery.scrollTo(".rtlRSel",0, {offset:-30});

        jQuery('.dnnTooltip').dnnTooltip();

        jQuery(function() {

            // The height of the content block when it's not expanded, 96/12=8 lines
            var adjustheight = 100;
            // The "more" link text
            var moreText = '<%= Localization.GetString("ExpandThis.Text", LocalResourceFile) %>';
            // The "less" link text
            var lessText = "Show Less";

            if ($(".more-less .more-block").height() >= 97) {
                // Sets the .more-block div to the specified height and hides any content that overflows
                $(".more-less .more-block").css('height', adjustheight).css('overflow', 'hidden');

                // The section added to the bottom of the "more-less" div           
                $(".more-less").append('<a href="#" class="adjust" rel="nofollow"></a>');

                $("a.adjust").text(moreText);

                $(".adjust").toggle(function() {
                    $(this).parents("div:first").find(".more-block").css('height', 'auto').css('overflow', 'visible');
                    $(this).text(lessText);
                }, function() {
                    $(this).parents("div:first").find(".more-block").css('height', adjustheight).css('overflow', 'hidden');
                    $(this).text(moreText);
                });
            }
        });

        jQuery("#mini-help-tabs ul li,#mini-help-tabs div.tab-container a").click(function() {
            jQuery("#mini-help-tabs div.tab-container").hide();
            jQuery("#mini-help-tabs ul li").removeClass("active");
            var tab_class = jQuery(this).attr("class");
            jQuery("#mini-help-tabs div." + tab_class).show();
            jQuery("#mini-help-tabs ul li." + tab_class).addClass("active");
        })

        //select all the a tag with name equal to modal
        jQuery('a[name=modal]').click(function(e) {

            e.preventDefault();

            jQuery("#mini-help-tabs ul li:first").addClass("active");
            jQuery("#mini-help-tabs div.tab-container:first").show();

            var winW = jQuery(window).width();
            var fromLeft = winW / 2 - 480;
            jQuery("#mini-help-tabs").css('left', fromLeft).show();

            jQuery("#mini-help-tabs").dialog({
                position: [fromLeft, 200],
                modal: true
            });

            jQuery('.ui-dialog-titlebar').css('left', fromLeft + 890);
        });

        jQuery('#mini-help-close').click(function(e) {
            jQuery("#mini-help-tabs").dialog('close');
        });
    });     /* end of document ready */


    /* Re-subscribe to all the events of document ready, so they are available after the partial update. */
    var prm = Sys.WebForms.PageRequestManager.getInstance();

    


    prm.add_endRequest(function() {

        /* Click handler for the Clear menu item */
        jQuery('.admin_link').dnnConfirm({
            text: 'Are you sure?',
            yesText: 'Yes',
            noText: 'No',
            title: 'Action Confirmation'
        });

        jQuery('.dnnTooltip').dnnTooltip();

        /* Changing widths to fix editor looks in IE  */
        jQuery('.reToolbar').css('width', '400px');
        jQuery('.RadEditor').css('height', '210px');

        jQuery(function() {

            // The height of the content block when it's not expanded, 96/12=8 lines
            var adjustheight = 100;
            // The "more" link text
            var moreText = '<%= Localization.GetString("ExpandThis.Text", LocalResourceFile) %>';
            // The "less" link text
            var lessText = "Show Less";

            if ($(".more-less .more-block").height() >= 97) {
                // Sets the .more-block div to the specified height and hides any content that overflows
                $(".more-less .more-block").css('height', adjustheight).css('overflow', 'hidden');

                // The section added to the bottom of the "more-less" div           
                $(".more-less").append('<a href="#" class="adjust" rel="nofollow"></a>');

                $("a.adjust").text(moreText);

                $(".adjust").toggle(function() {
                    $(this).parents("div:first").find(".more-block").css('height', 'auto').css('overflow', 'visible');
                    $(this).text(lessText);
                }, function() {
                    $(this).parents("div:first").find(".more-block").css('height', adjustheight).css('overflow', 'hidden');
                    $(this).text(moreText);
                });
            }
        });

        jQuery("#mini-help-tabs ul li,#mini-help-tabs div.tab-container a").click(function() {
            jQuery("#mini-help-tabs div.tab-container").hide();
            jQuery("#mini-help-tabs ul li").removeClass("active");
            var tab_class = jQuery(this).attr("class");
            jQuery("#mini-help-tabs div." + tab_class).show();
            jQuery("#mini-help-tabs ul li." + tab_class).addClass("active");
        })

        //select all the a tag with name equal to modal
        jQuery('a[name=modal]').click(function(e) {

            jQuery("#mini-help-tabs ul li:first").addClass("active");
            jQuery("#mini-help-tabs div.tab-container:first").show();

            e.preventDefault();
            var winW = jQuery(window).width();
            var fromLeft = winW / 2 - 480;         
            
            jQuery("#mini-help-tabs").dialog({
            position: [fromLeft, 200],
            modal: true
            });
            jQuery('.ui-dialog-titlebar').css('left', fromLeft + 890);
        });
    });

</script>

<% if (Request.IsAuthenticated)
   {%>

<script type="text/javascript">
    jQuery(document).ready(function() {
        jQuery(".proposeSlider").show();
        jQuery(".proposeSlider").click(function() {
            jQuery(".ProposeTopic").toggle("slow");
            jQuery(this).toggleClass("active");
            return false;
        });
    });

    /* * * CONFIGURATION VARIABLES: EDIT BEFORE PASTING INTO YOUR WEBPAGE * * */

    var locale = "<%=System.Threading.Thread.CurrentThread.CurrentCulture.Name %>";
    if (locale == "es-ES") {
        var disqus_shortname = 'puzzledbypolicyes';
        var disqus_identifier = "spanish";
    }
    else if (locale == "el-GR") {
        var disqus_shortname = 'puzzledbypolicyel';
        var disqus_identifier = "greek";
    }
    else if (locale == "it-IT") {
        var disqus_shortname = 'joinpuzzledbypolicyitalian';
        var disqus_identifier = "italian";
    }
    else if (locale == "hu-HU") {
        var disqus_shortname = 'puzzledbypolicyhu';
        var disqus_identifier = "hungarian";
    }
    else {
        var disqus_shortname = 'puzzledbypolicy';
        var disqus_identifier = "english";
    }

    var disqus_developer = 1;

    /* * * DON'T EDIT BELOW THIS LINE * * */
    (function() {
        var dsq = document.createElement('script'); dsq.type = 'text/javascript'; dsq.async = true;
        dsq.src = 'http://' + disqus_shortname + '.disqus.com/embed.js';
        (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);
    })();   
</script>

<noscript>
    Please enable JavaScript to view the <a href="http://disqus.com/?ref_noscript">comments
        powered by Disqus.</a></noscript>
<% }%>
<div id="fb-root">
</div>

<script>    (function(d, s, id) {
        var js, fjs = d.getElementsByTagName(s)[0];
        if (d.getElementById(id)) return;
        js = d.createElement(s); js.id = id;
        js.src = "//connect.facebook.net/en_US/all.js#xfbml=1&appId=274597442564126";
        fjs.parentNode.insertBefore(js, fjs);
    } (document, 'script', 'facebook-jssdk'));</script>


<!-- content start -->
<uDebate_Discussion:Breadcrump ID="ctlBreadcrump" runat="server" />
<div class="dnnClear">
</div>
<div id="forumContainer">
    <asp:DetailsView ID="ThreadDetails" BorderStyle="None" runat="server" Width="100%"
        BorderWidth="0" CssClass="threadContainer" AutoGenerateRows="false" DataSourceID="SqluDebateThread"
        OnDataBound="ThreadDetails_DataBound">
        <Fields>
            <asp:TemplateField HeaderStyle-CssClass="hidden" ItemStyle-BorderStyle="None">
                <ItemTemplate>
                    <asp:HiddenField ID="TopicId" runat="server" Value='<%# Bind("TopicId") %>' Visible="false" />
                    <asp:HiddenField ID="Status" runat="server" Value='<%# Bind("Status") %>' Visible="false" />
                    <asp:HiddenField ID="userID" runat="server" Value='<%# Bind("UserID") %>' Visible="false" />
                    <div class="facebook-like">
                        <fb:like send="false" width="80" show_faces="false" layout="button_count"></fb:like>
                    </div>
                    <div class="threadOpened">
                        <%= Localization.GetString("OpenedDate", LocalResourceFile)%>:
                        <asp:Label ID="openLbl" runat="server" Text='<%# Bind("Opened_Date","{0:dd-MM-yyyy}") %>'></asp:Label>
                        <%= Localization.GetString("PostBy", LocalResourceFile)%>
                        <asp:HyperLink ID="creatorLbl" CssClass="threadCreator" runat="server" Text='<%# Bind("Thread_Creator") %>'></asp:HyperLink>,
                        <%= Localization.GetString("Posts", LocalResourceFile)%>:
                        <asp:Label ID="postsLbl" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("Posts").ToString()) ? "0" : Eval("Posts")) %>'></asp:Label>,
                        <%= Localization.GetString("Views", LocalResourceFile)%>:
                        <asp:Label ID="viewsLbl" runat="server" Text='<%# Eval("View_Count") %>'></asp:Label>
                    </div>
                    <div class="dnnClear">
                    </div><span style="height:1px"></span>
                    <asp:Label ID="threadLbl" CssClass="threadTitle" runat="server" Text='<%# Bind("Description") %>'></asp:Label>
                    <br />
                    <asp:Label ID="SubtitleLbl" CssClass="threadSubTitle" runat="server" Text='<%# Bind("SubTitle") %>'></asp:Label>
                    <div style="padding-top: 5px">
                        <b>
                            <%=Localization.GetString("Summary", LocalResourceFile)%>:</b>
                    </div>
                    <div class="more-less">
                        <div class="more-block">
                            <asp:Label ID="summaryLbl" CssClass="threadSummary " runat="server" Text='<%#Bind("Text")%>'></asp:Label>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
        </Fields>
        <EmptyDataTemplate>
            <p>
                &nbsp;</p>
        </EmptyDataTemplate>
    </asp:DetailsView>
    <div class="debateHeader">
        <asp:Label runat="server" ID="TotalPostsLbl" Font-Bold="true" resourcekey="TotalPosts"></asp:Label>
        <asp:Label ID="statusLbl" runat="server" Text='Thread is Closed' Visible="false"
            CssClass="forum_error_new"></asp:Label>
        <asp:LinkButton ID="newDiscussionBtn" runat="server" Visible="false" Text="&nbsp;"
            OnClick="newDiscussionBtn_Click" />
        <asp:LinkButton ID="printLink" runat="server" CssClass="printLink" resourcekey="PrintAll" />
        <asp:LinkButton ID="expandLink" runat="server" CssClass="expandLink" OnClick="expandLink_Click"
            resourcekey="ExpandAll" />
        <div class='tooltipWrapper'>
            <asp:CheckBox ID="notifyCheck" runat="server" AutoPostBack="true" OnCheckedChanged="notifyCheck_CheckedChanged" />
            <div class='dnnTooltip'>
                <label id='label' runat='server'>
                    <asp:LinkButton ID='cmdHelp' TabIndex='-1' runat='server' CausesValidation='False'
                        EnableViewState='False' CssClass='dnnFormHelp'>
                        <asp:Label ID='notifyLabel' CssClass="notification" runat='server' resourcekey="Notification"
                            EnableViewState='False' />
                    </asp:LinkButton>
                    <asp:Label ID='lblNoHelpLabel' runat='server' EnableViewState='False' Visible='false' />
                </label>
                <asp:Panel ID='pnlHelp' runat='server' CssClass='dnnFormHelpContent dnnClear' EnableViewState='False'
                    Style='display: none;'>
                    <asp:Label ID='lblHelp' runat='server' EnableViewState='False' resourcekey="NotificationHelp"
                        class='dnnHelpText' />
                    <a href='#' class='pinHelp'></a>
                </asp:Panel>
            </div>
        </div>
        <asp:HyperLink ID="threadSummaryBtn" CssClass="threadSummary" runat="server" Visible="false"
            Text="&nbsp;" onclick="threadSummaryBtn_Click" />
        <asp:HyperLink ID="EditLink" runat="server" Visible="false" />
    </div>
    <div class="legend">
        <span class="legendLabel">
            <%=Localization.GetString("Legend", LocalResourceFile)%>:</span>
        <asp:Image ID="legendIssueImg" runat="server" /><asp:Label ID="legendIssueLbl" runat="server" />
        <asp:Image ID="legendAltImg" runat="server" /><asp:Label ID="legendAltLbl" runat="server" />
        <asp:Image ID="legendProImg" runat="server" /><asp:Label ID="legendProLbl" runat="server" />
        <asp:Image ID="legendConImg" runat="server" /><asp:Label ID="legendConLbl" runat="server" />
        <asp:Image ID="legendCommentImg" runat="server" /><asp:Label ID="legendCommentLbl"
            runat="server" />
     
         <a href="#mini-help" class="help" name="modal">
            <asp:Label ID="helpLbl" runat="server" resourcekey="Help"></asp:Label></a>
    </div>
    <div class="dnnClear">
    </div>
    <asp:Label ID="needLoginLblTop" runat="server" Text="" CssClass="forum_login_fail_bigger"
        Visible="false"></asp:Label>
    <telerik:DnnTreeList ID="DebateList" runat="server" ShowOuterBorders="false" HeaderStyle-CssClass="hidden"
        BackColor="#f5f5f5" AllowSorting="true" GridLines="None" AlternatingItemStyle-CssClass="alternatingBorder"
        OnItemDataBound="DebateList_ItemDataBound" ShowTreeLines="false" OnInsertCommand="DebateList_InsertCommand"
        EditMode="EditForms" OnItemCommand="DebateList_ItemCommand"  >
        <DetailTemplate>
            <div>
                <asp:HiddenField ID="hiddenPostID" runat="server" Value='<%#Eval("ID") %>' />
                <asp:Label ID="LblMessage" runat="server" CssClass="postBody" Text='<%#Server.HtmlDecode(Eval("Message").ToString()) %>'></asp:Label>
                <div class="userActions">
                    <asp:LinkButton ID="newPostBtn" runat="server" Text="Reply" CommandName="replyMessage"
                        resourcekey="Post" CssClass="replyBtn" />
                    <%--<asp:LinkButton ID="printPostLink" runat="server" CssClass="printPost" Text="Print this issue" />--%>
                    <%--<a href="javascript:;" onclick='javascript:PostToFacebook("sdsdfdfsdfsdfd");'>
                        <img src="/dnn6/DesktopModules/EuProfiler/icons/share_fb.png" /></a>--%>
                    <asp:HyperLink ID="publishPost" runat="server" CssClass="admin_link" Visible="false" />
                    <asp:HyperLink ID="deletePost" runat="server" CssClass="admin_link" Text="Delete"
                        Visible="false" />
                </div>
            </div>
        </DetailTemplate>
        <EditFormSettings EditFormType="Template" FormStyle-BackColor="#E6F1F1">
            <FormTemplate>
                <div class="replyLbl">
                    <asp:Label ID="lbSubjectD" runat="server" Text="Subject" resourcekey="Subject"></asp:Label>
                </div>
                <div class="replyField">
                    <asp:TextBox ID="txtSubjectPost" runat="server" Width="300"></asp:TextBox>
                   <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtSubjectPost"
                        resourcekey="SubjectHelp" />--%>
                </div>
                <%--<div class="replyLbl">
                    <asp:Label ID="lbFiles" runat="server" Text="Files" resourcekey="AttachedFiles"></asp:Label>
                </div>
                <div class="replyField">
                    <uDebate_Discussion:Attachment ID="ctlAddAttachment" runat="server" Width="375px" />
                </div>--%>
                <div class="replyLbl">
                    <asp:Label ID="lbPostType" runat="server" Text="Type" resourcekey="ResponseType"></asp:Label>
                </div>
                <div class="replyField" style="height: 40px;">
                    <div class="postType">
                        <asp:RadioButton ID="IssueRadio" runat="server" GroupName="postTypeOptions" CssClass="Issue"
                            Text="Issue" resourcekey="Issue" /></div>
                    <div class="postType">
                        <asp:RadioButton ID="AlterRadio" runat="server" GroupName="postTypeOptions" CssClass="Alter"
                            Text="Alternative" resourcekey="Alternative" /></div>
                    <div class="postType">
                        <asp:RadioButton ID="ProRadio" runat="server" GroupName="postTypeOptions" CssClass="Pro"
                            Text="ProArgument" resourcekey="ProArgument" /></div>
                    <div class="postType">
                        <asp:RadioButton ID="ConRadio" runat="server" GroupName="postTypeOptions" CssClass="Con"
                            Text="ConArgument" resourcekey="ConArgument" /></div>
                    <div class="postType">
                        <asp:RadioButton ID="CommentRadio" runat="server" GroupName="postTypeOptions" CssClass="Comment"
                            Text="Comment" resourcekey="Comment" Checked="true" /></div>
                </div>
                <div class="replyLbl">
                    <asp:Label ID="lbReply" runat="server" resourcekey="Description"></asp:Label>
                </div>
                <div class="replyField" style="padding-left: 0px">
                    <telerik:DnnEditor ID="txtReply" runat="server" LocalizationPath="~/DesktopModules/Admin/RadEditorProvider/App_LocalResources" autodetectlanguage="true" DialogHandlerUrl="~/DesktopModules/Admin/RadEditorProvider/DialogHandler.aspx"
                        ChooseMode="false" EditModes="Design,Preview" Height="200px" ContentAreaMode="Iframe" StripFormattingOptions="MSWordRemoveAll"                         
                        DocumentManager-ViewPaths="~/Portals/0/UserAttachments" DocumentManager-UploadPaths="~/Portals/0/UserAttachments"
                        ToolbarMode="Default" Width="100%" ToolsFile="~/DesktopModules/uDebate_Discussion/Components/ToolsFile.Registered.PortalId.0.xml">
                    </telerik:DnnEditor>
                   <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtReply"
                        resourcekey="DescHelp" />--%>
                </div>
                <div class="replyLbl">
                </div>
                <div class="replyField">
                    <div style="padding: 0px 7px 10px 7px;">
                        <asp:LinkButton ID="BtnCancel" runat="server" Text="Cancel" CommandName="Cancel"
                            CausesValidation="false" CssClass="cancelBtn" resourcekey="Cancel" />
                        <asp:LinkButton ID="BtnSubmit" runat="server" Text="Submit" CommandName="PerformInsert"
                            CssClass="submitBtn" resourcekey="SendMessage" />
                    </div>
                </div>
                <div class="dnnClear">
                </div>
            </FormTemplate>
        </EditFormSettings>
        <NoRecordsTemplate>
            There are no posts in this discussion.
        </NoRecordsTemplate>
    </telerik:DnnTreeList>
    <asp:Label ID="needLoginLblBottom" runat="server" Text="" CssClass="forum_login_fail_bigger"
        Visible="false"></asp:Label>
</div>
<!-- content end -->
<!-- Mini Help -->
<div id="mini-help-tabs">
   <div id="mini-help-close" title="<%= Localization.GetString("Close", LocalResourceFile) %>"> </div>
        <div id="tab_headers">
        <div class="helpHowto">
        <asp:Label ID="howtoLbl" runat="server" resourcekey="minihHow" /></div>
    <ul>
        <li class="tab1">
            <asp:Label ID="help1Lbl" runat="server" resourcekey="minihLogin" /></li>
        <li class="tab2">
            <asp:Label ID="help2Lbl" runat="server" resourcekey="minihComment" /></li>
        <li class="tab3">
            <asp:Label ID="help3Lbl" runat="server" resourcekey="minihDiscussion" /></li>
    </ul></div>
    <div class="tab-container tab1">
        <p style="text-align: center">
            <asp:Image ID="help1Image" runat="server" AlternateText="Help 1 image" ImageUrl="~/DesktopModules/uDebate_Discussion/images/Help-Tab-Graphic-01.png" />
        </p>
        <h2>
            <asp:Label ID="help1" runat="server" resourcekey="minihLogin" /></h2>
        <p class="help-text">
            <asp:Label ID="Label1" runat="server" resourcekey="minihLoginBody" />
        </p>
        <div class="nextlink">
            <a class="tab2" href="#">
                <asp:Label ID="firstLink" runat="server" Text="Next" resourcekey="Next" /></a></div>
    </div>
    <div class="tab-container tab2">
        <p style="text-align: center">
            <asp:Image ID="Image2" runat="server" AlternateText="Help 2 image" ImageUrl="~/DesktopModules/uDebate_Discussion/images/Help-Tab-Graphic-02.png" />
        </p>
        <h2>
            <asp:Label ID="Label2" runat="server" resourcekey="minihComment" /></h2>
        <p class="help-text">
            <asp:Label ID="Label3" runat="server" resourcekey="minihCommentBody" />
        </p>
        <div class="nextlink">
            <a class="tab3" href="#">
                <asp:Label ID="secondLink" runat="server" Text="Next" resourcekey="Next" /></a></div>
    </div>
    <div class="tab-container tab3">
        <p style="text-align: center">
            <asp:Image ID="Image3" runat="server" AlternateText="Help 3 image" ImageUrl="~/DesktopModules/uDebate_Discussion/images/Help-Tab-Graphic-03.png" />
        </p>
        <h2>
            <asp:Label ID="Label4" runat="server" resourcekey="minihDiscussion" /></h2>
        <p class="help-text">
            <asp:Label ID="Label5" runat="server" resourcekey="minihDiscussionBody" />
        </p>
    </div>
</div>
<asp:SqlDataSource ID="SqluDebatePosts"  runat="server" ConnectionString="<%$ ConnectionStrings:SiteSqlServer %>"
    SelectCommand="select ID, ParentID, Subject,Message,PostDate,PostType, P.UserId, U.Username as Post_Author,IsPublished 
                   FROM uDebate_Forum_Posts as P
                   LEFT OUTER JOIN Users AS U ON P.UserID = U.UserID 
                   WHERE P.ThreadID = @ThreadID AND ((P.IsPublished = 1 AND P.Active=1) OR 1=@Authorized)
                   ORDER BY PostDate DESC">
    <SelectParameters>
        <asp:Parameter Name="ThreadID" Type="String" />
        <asp:Parameter Name="Authorized" Type="String" DefaultValue="0" />
    </SelectParameters>
</asp:SqlDataSource>
<asp:SqlDataSource ID="SqluDebateThread" runat="server" ConnectionString="<%$ ConnectionStrings:SiteSqlServer %>"
    SelectCommand="SELECT  ThreadPosts.Posts, ID, TopicId,Description,Summary as SubTitle,Text,Status, U.Username AS Thread_Creator,
                           Th.UserID,Opened_Date,Closed_Date, View_Count
                   FROM (SELECT COUNT(ID) AS 'Posts',uDebate_Forum_Posts.ThreadID
                         FROM uDebate_Forum_Posts
                         GROUP BY uDebate_Forum_Posts.ThreadID) as ThreadPosts
                   RIGHT JOIN uDebate_Forum_Threads AS Th on ThreadPosts.ThreadID = Th.ID
                   INNER JOIN Users AS U ON Th.UserID = U.UserID 
                   WHERE Th.ID = @ThreadID">
    <SelectParameters>
        <asp:Parameter Name="ThreadID" Type="String" />
    </SelectParameters>
</asp:SqlDataSource>
