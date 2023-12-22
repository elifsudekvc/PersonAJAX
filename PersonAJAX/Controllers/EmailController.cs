using PersonAJAX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;

namespace PersonAJAX.Controllers
{
    public class EmailController : Controller
    {
        // GET: Email
        [CustomAuthorize("Admin", "User")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [CustomAuthorize("Admin", "User")]
        public ActionResult Index(Email model, HttpPostedFileBase myFiles)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add("elifkvc88@gmail.com");
            model.SenderEmail= Session["UserEmail"] as string;
            model.SenderEmailPassword= Session["UserPassword"] as string;
            mailMessage.From = new MailAddress(Session["UserEmail"] as string);
            mailMessage.Subject = "You have a message from the person page. Title:  " + model.Title;
            mailMessage.Body = "Message from: "+model.NameSurname+" Message: "+model.Message;
            mailMessage.IsBodyHtml = true;

            if(myFiles != null)
            {
                mailMessage.Attachments.Add(new Attachment(myFiles.InputStream, myFiles.FileName));
            }

            SmtpClient smtp = new SmtpClient();
            smtp.Credentials = new NetworkCredential(model.SenderEmail, model.SenderEmailPassword);
            smtp.Port = 587;
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl= true;

            try
            {
                smtp.Send(mailMessage);
                TempData["message"] = "Your message has been delivered. will be returned as soon as possible";
            }
            catch (Exception ex)
            {
                TempData["message"] = "Email could not be sent"+ex.Message;
            }
            

            return View();
        }
    }
}