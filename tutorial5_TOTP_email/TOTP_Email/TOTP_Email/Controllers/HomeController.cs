using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace TOTP_Email.Controllers
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



        [HttpPost]
        public ActionResult Login()
        {
            var username = Request["username"];
            var password = Request["password"];

            if (username == "yanick" && password == "yanick")
            {
                var request = (HttpWebRequest)WebRequest.Create("https://api.mailgun.net/v3/sandbox72783a5e953e465491fbd51ddc838f02.mailgun.org");
                String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("api:05dc820e1686db62eb5cbc60e8d1ac4e-3939b93a-32380dcb"));
                request.Headers.Add("Authorization", "Basic " + encoded);

                var secret = "TEST_SECRET";

                var postData = "from=Test User <mailgun@sandbox72783a5e953e465491fbd51ddc838f02.mailgun.org>";
                postData += "&api_secret=";
                postData += "&to=yanick.kuster@me.com";
                postData += "&subject=Secret-Token";
                postData += "&text=\"" + secret + "\"";

                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                ViewBag.Message = responseString;
                return View();

            }
            else
            {
                ViewBag.message = "Wrong Credentials";
                return View("Index");
            }
        }


        [HttpPost]
        public void TokenLogin()
        {
            var token = Request["token"];
            if (token == "TEST_SECRET")
            {
                //Token korrekt
            }
            else
            {
                //Token inkorrekt
            }
        }
    }
}