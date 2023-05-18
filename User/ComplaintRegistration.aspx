<%@ Page Title="" Language="C#" MasterPageFile="~/User/UserMaster.Master" AutoEventWireup="true" CodeBehind="ComplaintRegistration.aspx.cs" Inherits="Complaint_Management_System.User.ComplaintRegistration" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
          <div class="row">
            <div class="col-md-12 ">
                <div class="contact-h-cont">
                  <h3 class="text-center">Complaint Registration</h3><br>
                    <div>
                        <label for="ComplaintId">Complaint Id</label>
                        <asp:TextBox ID="txtComplaintId" runat="server" ValidationGroup="s"></asp:TextBox>
                       <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtComplaintId" ErrorMessage="Enter Complaint Id" ForeColor="#003300" ValidationGroup="s"></asp:RequiredFieldValidator>
                    </div>
     
                     <div>
                         <label for="ComplaintDate">Complaint Date</label>
                         <asp:TextBox ID="txtComplaintDate" runat="server" ValidationGroup="s"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtComplaintDate" ErrorMessage="Select a valid date" ForeColor="#003300" ValidationGroup="s"></asp:RequiredFieldValidator>
                     </div>
                        <div>
                            <label for="EnterComplaintTitle">Enter Complaint Title</label>
                             <asp:TextBox ID="txtComplaintTitle" runat="server" ValidationGroup="s"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtComplaintTitle" ErrorMessage="Enter Complaint Title" ForeColor="#003300" ValidationGroup="s"></asp:RequiredFieldValidator>
                        </div>
                          <div>
                              <label for="AttachmentFile">Attachment File</label>
                              <asp:TextBox ID="txtComplaintNature" runat="server" ValidationGroup="s"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtComplaintTitle" ErrorMessage="Please Enter Nature of Your Complaint" ForeColor="#003300" ValidationGroup="s"></asp:RequiredFieldValidator>
                          </div>
                       <div>
                           <label for="PleaseEnterNatureofyourcomplaint">Please Enter Nature of your complaint</label>
                           <asp:FileUpload ID="FileUpload2" runat="server" ForeColor="#000300" />
                       </div>
                        <div>
                            <label for="Suggestion">What do you think we should do to put things right?</label>
                            <asp:TextBox ID="txtsuggestion" runat="server" ValidationGroup="s"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtsuggestion"  ValidationGroup="s"></asp:RequiredFieldValidator>
                        </div>
                       
                    <div>
                        <asp:Button ID="btnSubmit" runat="server" ForeColor="#003300" Text="Submit" ValidationGroup="s" CssClass="btn btn-blue" OnClientClick="btn Submit_Click" />
                        <asp:Label ID="lblmsg" runat="server" Font-Bold="True" ForeColor="#003300" ></asp:Label>
                        <asp:Button ID="btnPostNewComplaint" runat="server" Text="Post New Complaint" ValidationGroup="s" />
                        <asp:Button ID="btnBack" runat="server" ForeColor="#003300" Text="Back" ValidationGroup="s" />
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" ValidationGroup="s" />
                    </div>
                </div>
              
            </div>
            
          </div>  
        </div>
</asp:Content>
