using System;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult NotFound()
        {
            TempData[MessagesHelper.Warning] = Resources.HomeTexts.PageNotFound;
            return RedirectToAction("About","Home");
        }

        public ActionResult Default(object sender, EventArgs e)
        {
            TempData[MessagesHelper.Info] = Resources.HomeTexts.ErrorSaved;
            return View();
        }
    }
}