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
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        
        public ActionResult Index(Email model)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add("elifkvc88@gmail.com");
            mailMessage.From= new MailAddress(model.SenderEmail);
            mailMessage.Subject = "You have a message from the person page. Title:  " + model.Title;
            mailMessage.Body = "Message from: "+model.NameSurname+" Message: "+model.Message;
            mailMessage.IsBodyHtml = true;

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