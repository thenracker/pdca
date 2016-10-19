using System.Collections.Generic;
using System.Web.Mvc;
using DataAccess.Models.Dao;

namespace Web.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            var dao = new ReportDao();
            IList<ReportDao.UsekOddeleniWithCount> useky = dao.GetLopCounts(ReportDao.StavUkolu.Vsechny, ReportDao.Tabulka.Lop);
            return View(useky);
        }
    }
}