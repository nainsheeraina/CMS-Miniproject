<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="Complaint_Management_System.Contact" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
     
<link href="https://fonts.googleapis.com/css?family=Open+Sans:300,400,600,700|Roboto:400,500&display=swap"
    rel="stylesheet" />
    <!-- contact section -->
  <section class="contact_section layout_padding-top">
    <div class="container-fluid">
      <div class="row">
        <div class=" col-md-6">
          <div id="map" class="h-100 w-100 " >
              <iframe src="https://www.google.com/maps/embed?pb=!1m14!1m12!1m3!1d14390.542002950493!2d85.11681399999999!3d25.61702565!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!5e0!3m2!1sen!2sin!4v1663148769891!5m2!1sen!2sin" width="600" height="640" style="border:4;" allowfullscreen="" loading="lazy" referrerpolicy="no-referrer-when-downgrade"></iframe>
          </div>
        </div>
        <div class=" col-md-6 contact_form-container">
          <div class="contact_box">
         
              <asp:Label ID="lblmsg" runat="server" Text=""></asp:Label>
         
              <asp:TextBox ID="txtname" runat="server" placeholder="Full Name" CssClass="form-control" required="#"></asp:TextBox>
              <br />
               <asp:TextBox ID="txtemail" runat="server" placeholder="Email Id" CssClass="form-control" required="#" ></asp:TextBox>
              <br />
              <asp:TextBox ID="txtno" runat="server" placeholder="Mobile Number" CssClass="form-control" MaxLength="10" TextMode="Number" required="#"></asp:TextBox>
              <br />
              <asp:TextBox ID="txtmsg" runat="server" placeholder="Enter Message" CssClass="form-control" TextMode="MultiLine" Height="150" required="#"></asp:TextBox>
              <br />
              <asp:Button ID="Btnsubmit" CssClass="btn btn-blue" runat="server" Text="Submit"  OnClick="BtnSubmit_Click" />
              <br />
             
         
          </div>
        </div>
      </div>
    </div>
  </section>

 <script>
     function myMap() {
         var mapProp = {
             center: new google.maps.LatLng(51.508742, -0.120850),
             zoom: 5,
         };
         var map = new google.maps.Map(document.getElementById("googleMap"), mapProp);
     }
 </script>

<script src="https://maps.googleapis.com/maps/api/js?key=YOUR_KEY&callback=myMap"></script>


</asp:Content>
