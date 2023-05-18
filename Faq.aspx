<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Faq.aspx.cs" Inherits="Complaint_Management_System.Faq" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="css/faq.css" rel="stylesheet" />
    <main>
        <h1 class="faq-heading">Frequently Asked Questions</h1>
        <section class="faq-container">
            <div class="faq-one");>

                
                <h1 class="faq-page">What happened to make the customer upset?</h1>

               
                <div class="faq-body">
                    <p>It’s critical to stop and actively listen. Gather details, take notes if necessary, and show real concern for customers by empathizing with their frustration. 
                        When business owners put themselves in another person’s shoes, they’re less likely to feel dismissive or annoyed with the customer—and more ready to leap into action.</p>
                </div>
            </div>
            <hr class="hr-line">

            <div class="faq-two">

                
                <h1 class="faq-page">What is the major challenge for digital India?</h1>

               

                <div class="faq-body">
                    <p>An effective complaint handling system provides three key benefits to agencies: It resolves issues raised by a dissatisfied person in a timely and cost-effective way. It provides information which can lead to improvements in service delivery.</p>
                    
                </div>
            </div>
            <hr class="hr-line">


            <div class="faq-three">

               
<h1 class="faq-page">Why is it necessary to have effective complaint management systemin an organization?</h1>

                
                <div class="faq-body">
                    <p>No one is perfect. Mistakes will happen. Minimizing complaints and expertly solving problems are the real goals.</p>
                    <p>To get there, business owners should focus on steady improvements and knowledge building. Training staff, fine-tuning processes, and empowering employees to find great solutions will go a long way to reducing the volume and intensity of customer complaints.</p>
                    <p>Remember, many customers are neutral on a business until it’s been tested by a real complaint. Either the business steps up and wows them, or it disappoints. Handling problems well is a chance to shine.</p>
                </div>
            </div>

        </section>
    </main>
        
    <script>
        var faq = document.getElementsByClassName("faq-page");
        var i;
        
        for (i = 0; i < faq.length; i++) {
            faq[i].addEventListener("click", function () {
                this.classList.toggle("active");
                var body = this.nextElementSibling;
                if (body.style.display === "block") {
                    body.style.display = "none";
                } else {
                    body.style.display = "block";
                }
            });
        }
    </script>
</asp:Content>
