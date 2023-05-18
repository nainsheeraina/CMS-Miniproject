<%@ Page Title="" Language="C#" MasterPageFile="~/User/UserMaster.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="Complaint_Management_System.User.ChangePassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table>
        <tr>
            <td><asp:Label ID="lblmsg" runat="server" Font-Bold="True" ForeColor="#006600"></asp:Label></td>
        </tr>
       <tr><td>

               
               
               Old Password</td>
       <td>

           <asp:TextBox ID="txtOldPassword" runat="server" TextMode="Password" ValidationGroup="s"></asp:TextBox>
           <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtOldPassword" ErrorMessage="Enter Old Password" ValidationGroup="s"></asp:RequiredFieldValidator>

       </td>
       </tr>
       <tr>
           <td>New Password</td>
           <td>

               <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" ValidationGroup="s"></asp:TextBox>
               <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtNewPassword" ErrorMessage="Enter New Password" ValidationGroup="s"></asp:RequiredFieldValidator>

           </td>
       </tr>
       <tr>
           <td>Confirm Password</td>
           <td>

               <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" ValidationGroup="s"></asp:TextBox>
               <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtConfirmPassword" ErrorMessage="Confirm Password" ValidationGroup="s"></asp:RequiredFieldValidator>

           </td>
       </tr>
       <tr>
           <td>

               <asp:Button ID="btnChangePassword" runat="server" Font-Bold="True" OnClientClick="btnChangePassword_Click" Text="Change Password" ValidationGroup="s" OnClick="btnChangePassword_Click1" />
               
           </td>
       </tr>
       <tr>
           <td colspan="2">
               <asp:ValidationSummary ID="ValidationSummary3" runat="server" ShowMessageBox="True" ValidationGroup="s" />
           </td>
       </tr>
   </table>
</asp:Content>
