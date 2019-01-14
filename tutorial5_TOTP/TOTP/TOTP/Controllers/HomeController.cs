using Google.Authenticator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TOTP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult SetupAuthentication()
        {
            TwoFactorAuthenticator twoFaAuth = new TwoFactorAuthenticator();
            var info = twoFaAuth.GenerateSetupCode("MY_MVC_APP", "MY_EMAIL_ADDRESS", "MY_SECRET_KEY", 300, 300);

            string imageUrl = info.QrCodeSetupImageUrl;
            string entrySetupCode = info.ManualEntryKey;

            ViewBag.Message = "<h2>QR-Code</h2> <img src='" + imageUrl + "' /><h2>Token for manual entry</h2>" + entrySetupCode;

            return View();

        }

        [HttpPost]
        public ActionResult Login()
        {
            var username = Request["username"];
            var password = Request["password"];
            var token = Request["token"];

            if (username == "yanick" && password == "yanick")
            {
                TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                bool isCorrectPIN = tfa.ValidateTwoFactorPIN("MY_SECRET_KEY", token);
                if (isCorrectPIN)
                {
                    ViewBag.Message = "Login and Token Correct";
                }
                else
                {
                    ViewBag.Message = "Wrong credentials and token";
                }
            }
            else
            {
                ViewBag.Message = "Wrong credentials";
            }

            return View();
        }
    }
}