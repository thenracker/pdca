using System;
using System.Web.Mvc;
using DataAccess.Models;
using DataAccess.Models.Dao;
using DataAccess.Models.DataUnit;
using DataAccess.Models.DataUnit.Users;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            if (!Uzivatel.UserExists(User.Identity.Name))
            {
                TempData[MessagesHelper.Info] = Resources.HomeTexts.NotAuthorized;
                return RedirectToAction("About", "Home");
            }

            var me = new Uzivatel(User.Identity.Name);  // 1. naštení 28ms

            var me2 = new Uzivatel(User.Identity.Name); // 2. 6ms - jak? jsem frajer..... Jarda

            /*var lops = new LopDao().GetAll(uziv);
            var ukolved = new UkolVedeniDao().GetAll(uziv);
            var ukoloddel = new UkolOddeleniDao().GetAll(uziv);
            var ukolvzork = new UkolVzorkovaniDao().GetAll(uziv);

            return View(new HomeCollection(lops, ukoloddel, ukolved, ukolvzork));
            */
            var lopDao = new LopDao();
            ViewBag.lopMeAsZadavatel = lopDao.GetAll();
            ViewBag.lopMeAsResitel = lopDao.GetAll();
            return View();

        }
       
        public ActionResult About()
        {
            return View();
        }

        public ActionResult Test()
        {
            var lopDao = new LopDao();
            var rnd = new Random();
            for (int i = lopDao.GetLatestLopThisYear().Action; i < 10000; i++)
            {
                lopDao.Create(new Lop()
                {
                    Action = i,
                    Nazev = "Lop "+i,
                    Zadavatel = new UzivatelDao().GetById(rnd.Next(1,8)),
                    Resitel = new UzivatelDao().GetById(rnd.Next(1, 8)),
                    Status = StatusUkolu.Open,
                    StartDate = DateTime.Now,
                    LastChangedDate = DateTime.Now,
                    PlannedCloseDate = DateTime.Now.AddDays(40),
                    Popis = "blsadklasmdaskldmalsdm"
                });
            }
            TempData[MessagesHelper.Success] ="Záznamy lopu vygenerovány";

            return RedirectToAction("Index");
        }

        public ActionResult TestEmail(string address)
        {
            var email = new Email();
            email.AddMailToAddress(address);
            email.Subject = "TEST";
            email.Body = "Testovací zpráva";
            email.Send();

            return RedirectToAction("Index");
        }
    }
}