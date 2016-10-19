using System.Web.Mvc;
using DataAccess.Models.Dao;
using DataAccess.Models.DataUnit.Users;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    public class UzivatelController : Controller
    {
        // GET: Uzivatel
        public ActionResult Index()
        {
            var dao = new UzivatelDao();
            var list = dao.GetAll();
            return View(list);
        }

        // GET: Uzivatel/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View(new Uzivatel());
        }

        // POST: Uzivatel/Create
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Create(Uzivatel collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var dao = new UzivatelDao();
                    collection.WindowsId = collection.WindowsId.ToUpper();
                    collection.Id = (int)dao.Create(collection);
                    if (collection.Role.Id > 0)
                    {
                        var permDao = new PermissionDao();
                        permDao.Create(new Permission()
                        {
                            Role = collection.Role,
                            WindowsId = collection.WindowsId
                        });
                    }

                    TempData[MessagesHelper.Success] = Resources.UzivatelTexts.UserAdded;
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData[MessagesHelper.Warning] = "Zkontrolujte zadané údaje";
                }
            }
            catch
            {
                TempData[MessagesHelper.Warning] = "Zkontrolujte zadané údaje";
            }
            return View(collection);
        }

        // GET: Uzivatel/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int id)
        {
            var dao = new UzivatelDao();
            var item = dao.GetById(id);
            if (item == null)
                return RedirectToAction("Index");
            item.LoadPermission();
            return View(item);
        }

        // POST: Uzivatel/Edit/5
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int id, Uzivatel collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var dao = new UzivatelDao();
                    collection.WindowsId = collection.WindowsId.ToUpper();
                    dao.Update(collection);

                    var permDao = new PermissionDao();
                    var permItem = permDao.GetByUser(collection);
                    if (collection.Role.Id == 0)
                    {
                        if (permItem != null)
                            permDao.Delete(permItem);
                    }
                    else
                    {
                        if (permItem == null)
                        {
                            permDao.Create(new Permission()
                            {
                                Role = collection.Role,
                                WindowsId = collection.WindowsId
                            });
                        }
                        else if (permItem.Role.Id != collection.Role.Id)
                        {
                            permItem.Role.Id = collection.Role.Id;
                            permDao.Update(permItem);
                        }
                    }

                    TempData[MessagesHelper.Success] = Resources.UzivatelTexts.UserUpdated;
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData[MessagesHelper.Warning] = "Zkontrolujte zadané údaje";
                }
            }
            catch
            {
                TempData[MessagesHelper.Warning] = "Zkontrolujte zadané údaje";
            }
            return View(collection);
        }

        // GET: Uzivatel/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int id)
        {
            var dao = new UzivatelDao();
            var item = dao.GetById(id);
            if (item == null)
                return RedirectToAction("Index");
            var permDao = new PermissionDao();
            var permItem = permDao.GetByUser(item);
            if (permItem != null)
                permDao.Delete(permItem);

            dao.Delete(item);
            TempData[MessagesHelper.Success] = Resources.UzivatelTexts.UserDeleted;
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public ActionResult CustomTableSetting(string tableName, FormCollection collection, string controller,
            string view = "Index")
        {
            string[] keys;
            switch (tableName)
            {
                case "lop/index-lopList":
                case "home/index-meAsResitel":
                case "home/index-meAsZadavatel":
                    keys = new[]
                    {
                        "Id", "Action", "Status", "Dilna", "Nazev", "StartDate", "PlannedCloseDate", "MaxPlannedCloseDate",
                        "ProductPN", "ProductPopis", "ProductSapNumber", "Resitel", "Investice", "LessonLearned",
                        "PrehledPodukolu", "LastChangedDate"
                    };
                    break;
                default:
                    return RedirectToAction(view, controller);
            }
            var uzivatel = new UzivatelDao().GetByWindowsId(User.Identity.Name);
            var customSettingDao = new CustomTableSettingDao();
            foreach (var key in keys)
            {
                var item = customSettingDao.Get(uzivatel, tableName, key);
                if (item == null)
                {
                    customSettingDao.Create(new CustomTableSetting()
                    {
                        Uzivatel = uzivatel,
                        Tabulka = tableName,
                        Sloupec = key,
                        Zobrazit = (collection[key] == "true")
                    });
                }
                else
                {
                    item.Zobrazit = (collection[key] == "true");
                    customSettingDao.Update(item);
                }
            }
            UzivatelDao.ClearFromCache(uzivatel);
            return RedirectToAction(view, controller);
        }
        #region Modals
        [AllowAnonymous]
        public ActionResult ModalUserInfo(string winId)
        {
            var uzivatel = new Uzivatel(winId);
            return View(uzivatel);
        }
        #endregion
    }
}
