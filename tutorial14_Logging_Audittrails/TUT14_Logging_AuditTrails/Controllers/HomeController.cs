using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TUT14_Logging_AuditTrails.Models;

namespace TUT14_Logging_AuditTrails.Controllers
{
    public class HomeController : Controller
    {



        public ActionResult Index()
        {
           
            return View();
        }

        [HttpPost]
        public ActionResult DoLogin()
        {
            var username = Request["username"];
            var password = Request["password"];

            var ip = Request.ServerVariables["REMOTE_ADDR"];
            var platform = Request.Browser.Platform;
            var browser = Request.UserAgent;

            SqlConnection con = new SqlConnection();
            con.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\cflei\Documents\logging_intrusion_detection.mdf;Integrated Security = True; Connect Timeout = 30";

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.CommandText = "SELECT [Id] FROM [dbo].[User] WHERE [Username] = '" + username + "' and [Password] = '" + password + "'";
            cmd.Connection = con;
            con.Open();
            reader = cmd.ExecuteReader();

            
            if (reader.HasRows)
            {
                var status = reader.GetString(5);
                if (status == "blocked")
                {
                    RedirectToAction("Blocked");
                }
                else
                {
                    while (reader.Read())
                    {
                        var userid = 0;
                        while (reader.Read())
                        {
                            userid = reader.GetInt32(0);
                            break;
                        }
                        con.Close();
                        con.Open();

                        SqlCommand cmd_browser = new SqlCommand();
                        cmd_browser.CommandText = "SELECT id FROM Userlog WHERE userid = '" + userid + "'and ip like '" + ip.Substring(0, 2) + "%' and browser like '" + platform + "%'";
                        cmd_browser.Connection = con;

                        SqlDataReader reader_browser = cmd_browser.ExecuteReader();
                        if (!reader_browser.HasRows)
                        {
                            con.Close();
                            con.Open();
                            SqlCommand logCommand = new SqlCommand();
                            logCommand.CommandText = "INSERT INTO userlog (userid, ip, action, result, createdon, browser) VALUES('" + userid + "', '" + ip + "', 'login', 'success', GETDATE(), '" + platform + "')";
                            logCommand.Connection = con;
                            logCommand.ExecuteReader();


                        }
                    }
                }
               
            }
            else
            {
                con.Close();
                con.Open();

                SqlCommand cmdUserid = new SqlCommand();
                cmdUserid.CommandText = "SELECT id FROM [dbo].[User] WHERE username = '" + username + "'";
                cmdUserid.Connection = con;

                SqlDataReader readerUserid = cmdUserid.ExecuteReader();
                if (readerUserid.HasRows)
                {
                    var userid = 0;
                    while (readerUserid.Read())
                    {
                        userid = readerUserid.GetInt32(0);
                        break;
                    }
                    con.Close();
                    con.Open();

                    SqlCommand failedLogCommand = new SqlCommand();
                    failedLogCommand.CommandText = "SELECT count(id) FROM [dbo].[UserLog] WHERE userid = '" + userid + "' " + "and result = 'failed' and cast(createdon as date) = '" +
                        System.DateTime.Now.ToShortDateString().Substring(0, 10) + "'";
                    failedLogCommand.Connection = con;
                    SqlDataReader failedCount = failedLogCommand.ExecuteReader();

                    var attemps = 0;
                    if (failedCount.HasRows)
                    {
                        while (failedCount.Read())
                        {
                            attemps = failedCount.GetInt32(0);
                            break;
                        }
                    }
                    if(attemps >= 5 || password.Length < 4 || password.Length > 20)
                    {
                        SqlCommand blockUser = new SqlCommand();
                        blockUser.CommandText = "upadate [dbo].[User] set role = 'blocked' WHERE userid = '" + userid + "'";
                        RedirectToAction("Index");
                        con.Close();
                    }
                    con.Close();
                    con.Open();

                    SqlCommand logCmd = new SqlCommand();
                    logCmd.CommandText = "INSERT INTO [dbo].[UserLog] (userid, ip, action, result, createdon, browser) VALUES('" + userid + "', '" + ip + "', 'login', 'failed', GETDATE(), '" + platform + "')";
                    logCmd.Connection = con;
                    logCmd.ExecuteReader();

                    ViewBag.Message = "No User found";
                }

                else
                {
                    con.Close();
                    con.Open();

                    SqlCommand logCmd = new SqlCommand();
                    logCmd.CommandText = "INSERT INTO userlog (userid, ip, action, result, createdon, additionalinformation, browser) VALUES('0, '"+ ip + "', 'login', 'failed', GETDATE(), 'no User found' + '" + platform + "')";
                    logCmd.Connection = con;
                    logCmd.ExecuteReader();

                    ViewBag.Message = "No User found";
                }
            }

            con.Close();
            return RedirectToAction("Logs", "Home");
        }

        public ActionResult Blocked()
        {
            return View();
        }

        public ActionResult Logs()
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\cflei\Documents\logging_intrusion_detection.mdf;Integrated Security = True; Connect Timeout = 30";

            SqlCommand cmdCredentials = new SqlCommand();
            cmdCredentials.CommandText = "SELECT * FROM Userlog ul join user u on ul.userid = u.id oderby ul.createdon Desc";
            cmdCredentials.Connection = con;

            con.Open();

            SqlDataReader reader = cmdCredentials.ExecuteReader();

            if (reader.HasRows)
            {
                List<HomeHontrollerViewModel> model = new List<HomeHontrollerViewModel>();
                while (reader.Read())
                {
                    var logEntry = new HomeHontrollerViewModel();
                    logEntry.UserId = reader.GetValue(10).ToString();
                    logEntry.LogId = reader.GetValue(0).ToString();
                    logEntry.LogCreatedOn = reader.GetValue(7).ToString();

                    model.Add(logEntry);
                }
                return View(model);
            }
            else
            {
                ViewBag.message = "No result found";
                return View();
            }


        }



    }
}