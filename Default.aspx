<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Complaint_Management_System._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="css/Style.css" rel="stylesheet" />
    <div class="row carousel-holder">

                    <div class="col-md-12">
                        <div id="carousel-example-generic" class="carousel slide" data-ride="carousel">
                            <ol class="carousel-indicators">
                                <li data-target="#carousel-example-generic" data-slide-to="0" class="active"></li>
                                <li data-target="#carousel-example-generic" data-slide-to="1"></li>
                                <li data-target="#carousel-example-generic" data-slide-to="2"></li>
                                
                            </ol>
                            <div class="carousel-inner">
                                <div class="item active">
                                    <img class="slide-image" src="img/slide6.jpg" alt=""/>

                                </div>
                                <div class="item">
                                    <img class="slide-image" src="img/slide2.jpg" alt=""/>
                                </div>
                                 <div class="item">
                                    <img class="slide-image" src="img/slide3.jpg" alt=""/>
                                </div>
                                
                            </div>
                            <a class="left carousel-control" href="#carousel-example-generic" data-slide="prev">
                                <span class="glyphicon glyphicon-chevron-left"></span>
                            </a>
                            <a class="right carousel-control" href="#carousel-example-generic" data-slide="next">
                                <span class="glyphicon glyphicon-chevron-right"></span>
                            </a>
                        </div>
                    </div>

                </div>

                <br />
    <br />
                <div class="row">
      <div class="col-lg-4 notice">         
               <h1>Notice</h1> 
               <hr />
               <div>
        <marquee width="100%" style="min-height: 350px;" onmouseover="this.stop()" onmouseout="this.start()" scrollamount="2" scrolldelay="2" direction="up" behavior="scroll">
                <asp:Repeater ID="rpNews" runat="server" OnItemDataBound="rpNews_ItemDataBound">
                                        <ItemTemplate>

                            <div class="panel-ribbon">
                            <div class="ribbon-heading">
                                <div class="check-title">
                                  <asp:HiddenField ID="hdfIsNew" runat="server" Value='<%# Bind("IsNew") %>' />
                                    <a href="ViewNewEvent.aspx?Notice=<%#Eval("ID") %>" target="_blank"><i class="fa fa-chevron-right" aria-hidden="true"></i>
                                 <strong style="font-size: 16px;    color: red;    font-style: italic;">      <asp:Literal ID="LtImpLinkCentral" runat="server" Text='<%#Bind("NoticeSubject") %>' >  </asp:Literal> </strong> 
                                        <span>   <asp:Image ID="Image1" runat="server" ImageUrl="img/new.gif" Visible="false"  Width="30px" /></span>
                                        <p style="font-weight: 600;    font-size: 12px; color:#111;"> PublishDate : <asp:Literal ID="Literal1" runat="server" Text='<%#Bind("PublishDate") %>' /> </p>
                                        
                                    </a>
                                </div>
                            </div>
                        </div>

                                </ItemTemplate>
                                    </asp:Repeater>
 </marquee>
               </div>
               </div>
 <section class="welcome_section layout_padding">
<div class="col-lg-6">

<div class="welcome_detail"> 
    <h3> Welcome
            </h3>
    <h2  style="text-align:center;"> COMPLAINT MANAGEMENT SYSTEM</h2>
                It provides an online way of solving the problems faced by the
              users. The objective of this system is to make complaints easier
              to coordinate, monitor, track and resolve. This system is used
              to record resolve and respond to customer complaints, requests
              as well as facilitate any other feedback.
              By using this system users can know the currently processing
              status of their Complaints. They have facility to edit profile,
              change password and check status of complaint etc

                </div>
 </div>
     </section>
 <div  class="row"> 
 
 <div class="col-lg-6">    </div>

  <div class="col-lg-6">    </div>

 </div>
   
     <!-- service section -->
  <section class="service_section ">
    <div class="container">
      <div class="service_detail">
        <h3>
          We
        </h3>
        <h2>
          we take complaints against
        </h2>
      </div>
      <div class="service_img-container">
        <div class="service_img-box i-box-1">
          <a href="Registration.aspx"> Management</a>
        </div>
        <div class="service_img-box i-box-2">
          <a href="Registration.aspx">Infrastructure </a>
        </div>
        <div class="service_img-box i-box-3">
          <a href="Registration.aspx">Transaction</a>
        </div>
        <div class="service_img-box i-box-4">
          <a href="Registration.aspx">Others</a>
        </div>
      </div>
     <div class="service_btn">
        <a href="AboutUs.aspx">
          <span>
            Read More
          </span>
          <img src="images/arrow-black.png" alt="" class="ml-2" />
        </a>
      </div>
    </div>
  </section>
  <!-- end service section -->

  
</div>
</asp:Content>


