using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class LanguageController : Controller
    {
        // GET: Language
        public ActionResult Index(string languageAbrevation="cs")
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(languageAbrevation);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(languageAbrevation);

            var cookie = new HttpCookie("Language") {Value = languageAbrevation};
            Response.Cookies.Add(cookie);

            return RedirectToAction("Index", "Home");
        }
    }
}