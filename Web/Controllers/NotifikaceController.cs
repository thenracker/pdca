using System.Web.Mvc;
using DataAccess.Models.Dao;
using DataAccess.Models.DataUnit.Users;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    public class NotifikaceController : Controller
    {
        // GET: Notifikace
        public ActionResult Index()
        {
            if (!Uzivatel.UserExists(User.Identity.Name))
            {
                TempData[MessagesHelper.Info] = Resources.HomeTexts.NotAuthorized;
                return RedirectToAction("About", "Home");
            }
            var dao = new NotifikaceDao();
            var nove = dao.GetByUser(new Uzivatel(User.Identity.Name), false);
            var seen = dao.GetByUser(new Uzivatel(User.Identity.Name));

            ViewBag.nove = nove;
            ViewBag.seen = seen;

            return View();
        }
    }
}