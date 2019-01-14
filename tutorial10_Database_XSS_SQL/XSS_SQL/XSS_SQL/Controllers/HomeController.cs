using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XSS_SQL.Controllers
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
        public ActionResult DoLogin()
        {
            var username = Request["username"];
            var password = Request["password"];

            SqlConnection con = new SqlConnection();
            con.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\kustery\Documents\sql_xss_injection.mdf;Integrated Security=True;Connect Timeout=30";

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.CommandText = "SELECT [Id], [Username], [Password] FROM [dbo].[User] WHERE [Username] = '" + username + "' and [Password] = '" + password + "'";
            cmd.Connection = con;

            con.Open();

            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                ViewBag.Message = "success";
                while (reader.Read())
                {
                    ViewBag.Message += reader.GetInt32(0) + " " + reader.GetString(1) + " " + reader.GetString(2); ;
                }
            }
            else
            {
                ViewBag.Message = "nothing to read of";
            }
            return RedirectToAction("Index");

        }


        [HttpGet]
        public ActionResult Feedback()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DoFeedback()
        {

            var feedback = Request["feedback"];

            SqlConnection con = new SqlConnection();
            con.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\kustery\Documents\sql_xss_injection.mdf;Integrated Security=True;Connect Timeout=30";

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.CommandText = "INSERT INTO [dbo].[Feedback] SET [feedback] = '" + feedback + "'";
            cmd.Connection = con;

            con.Open();

            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                ViewBag.Message = "success";
                while (reader.Read())
                {
                    ViewBag.Message += reader.GetInt32(0) + " " + reader.GetString(1) + " " + reader.GetString(2); ;
                }
            }
            else
            {
                ViewBag.Message = "nothing to read of";
            }
            return RedirectToAction("Index");
        }
    }
}