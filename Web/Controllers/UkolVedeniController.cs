using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DataAccess.Models.Dao;
using DataAccess.Models.DataUnit;
using DataAccess.Models.DataUnit.Users;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    public class UkolVedeniController : Controller
    {
        // GET: UkolVedeni
        public ActionResult Index(bool? smazane)
        {
            if (!Uzivatel.UserExists(User.Identity.Name))
            {
                TempData[MessagesHelper.Info] = Resources.HomeTexts.NotAuthorized;
                return RedirectToAction("About", "Home");
            }
            var dao = new UkolVedeniDao();
            IList<UkolVedeni> list;

            if (smazane == true)
            {
                ViewBag.ZeSmazanych = true; //TODO otestovat!
                list = dao.GetOnlyDeleted(new UzivatelDao().GetByWindowsId(User.Identity.Name));
            }
            else
            {
                list = dao.GetAll(new UzivatelDao().GetByWindowsId(User.Identity.Name));
            }
            
            return View(list);
        }

        //DETAIL
        [Authorize(Roles = "authorized")]
        public ActionResult Detail(int id, bool? smazane)
        {
            var uvd = new UkolVedeniDao();
            var uv = uvd.GetById(id);


            if (smazane == true)
            {
                ViewBag.ZeSmazanych = true;
            }

            if (uv != null && !uv.Deleted) 
            {
                return View(uv);
            }

            TempData[MessagesHelper.Danger] = "úkol vedení neexistuje, nebo byl smazán";
            return RedirectToAction("Index");
        }

        //DELETE
        [Authorize(Roles = "authorized")]
        public ActionResult Delete(int id)
        {
            try
            {
                var uvd = new UkolVedeniDao();
                var uv = uvd.GetById(id);
                /*
                var uvhd = new UkolVedeniHistoryDao();
                var uvh = new UkolVedeniHistory(uv);
                uvhd.Create(uvh);
                */
                uv.Deleted = true;
                uvd.Update(uv);

                TempData[MessagesHelper.Success] = "úkol vedení byl úspěšně smazán";
            }
            catch
            {
                TempData[MessagesHelper.Danger] = "Nepodařilo se smazat úkol vedení";
            }

            return RedirectToAction("Index");
        }

        //CREATE
        [Authorize(Roles = "authorized")]
        public ActionResult Create()
        {
            ViewBag.FirstCreate = true;

            var v = new UkolVedeni()
            {
                Action = 1,
                Zadavatel = new UzivatelDao().GetByWindowsId(User.Identity.Name)
            };

            return View(v);
        }

        //CREATE(collection)
        [HttpPost]
        [Authorize(Roles = "authorized")]
        public ActionResult Create(UkolVedeni collection, bool poslatMail, string produktyInput)
        {
            ViewBag.FirstCreate = false;

            try
            {
                if (ModelState.IsValid)
                {
                    var uvDao = new UkolVedeniDao();
                    collection.DateStart = DateTime.Now;
                    collection.DateLastChanged = DateTime.Now;
                    collection.DateFinish = null; //teď fakt úkol vedení není vytvořen
                    collection.DateDeadline = null; //nezadáváme
                    collection.Status = StatusUkolu.Open; //když vytváříme - úkol vedení je standartně otevřený
                    collection.LessonLearned = false; //zatim jsme se z toho asi fakt neponaučili
                    var latest = uvDao.GetLatestLopThisYear();
                    collection.Action = latest?.Action + 1 ?? 1;
                    collection.Deleted = false;
                    collection.Id = (int) uvDao.Create(collection);
                    TempData[MessagesHelper.Success] = "úkol vedení přidaný";

                    if (produktyInput != "")
                    {
                        try
                        {
                            var ukolMaterialDao = new UkolVedeniMaterialDao();
                            foreach (
                                var ukolProdukt in
                                    produktyInput.Split(';')
                                        .Where(item => item != "")
                                        .Select(item => new UkolVedeniMaterial()
                                        {
                                            Ukol = collection,
                                            Produkt = new MaterialDao().GetById(Convert.ToInt32(item)),
                                            DateAdded = DateTime.Now
                                        }))
                            {
                                ukolMaterialDao.Create(ukolProdukt);
                            }
                        }
                        catch
                        {
                            TempData[MessagesHelper.Danger] = "Nepodařilo se připojení produktů k úkolu vedení";
                        }
                    }

                    new UkolVedeniMaterialDao().ClearWrongHistoryAfterCreate(); //po vytvoření je třeba vymazat řádky v historii matroše

                    //posíláme vždy
                    //if (!poslatMail) return RedirectToAction("Detail", new {id = collection.Id});
                    var s = "Byl Vám přidělen nový úkol vedení " +
                            "<a href='/UkolVedeni/Detail/" + collection.Id + "'>" + collection.Nazev + "</a>";
                    Notifikace.Create(collection.Resitel.Id, s);
                    return RedirectToAction("Detail", new {id = collection.Id});
                }
                else
                {

                    TempData[MessagesHelper.Warning] = "Zkontrolujte zadané údaje";

                    return View(collection);
                }
            }
            catch
            {
                TempData[MessagesHelper.Danger] = "Došlo k neočekávané chybě";
            }

            return View(collection);
            
        }
        [Authorize(Roles = "authorized")]
        public ActionResult Edit(int id)
        {
            var uvd = new UkolVedeniDao();
            var item = uvd.GetById(id);
            if (item == null)
                return RedirectToAction("Index");
            var matDao = new MaterialDao();
            var materialList = matDao.GetAll();
            ViewBag.materialList = materialList;
            TempData["UkolVedeniHistory"] = item;
            return View(item);
        }

        [HttpPost]
        [Authorize(Roles = "authorized")]
        public ActionResult Edit (int id, UkolVedeni collection, bool poslatMailResiteli, bool poslatMailZadavateli, string produktyInput)
        {
            var history = TempData["UkolVedeniHistory"] as UkolVedeni;
            if (history == null)
                return RedirectToAction("Index");

            try
            {
                //POKUD VŠE SEDÍ
                if (ModelState.IsValid)
                {
                    if (produktyInput != history.MaterialyInput)
                    {
                        var ukolMaterialDao = new UkolVedeniMaterialDao();
                        foreach (var item in history.MaterialyInput.Split(';').Where(item => !produktyInput.Contains(";" + item + ";")).Where(item => item != ""))
                        { // Odebrání přebytečných
                            ukolMaterialDao.Delete(history.Materialy.First(x => x.Produkt.Id == Convert.ToInt32(item)));
                        }
                        try
                        {//Přidání nových
                            foreach (var ukolProdukt in produktyInput.Split(';').Where(item => item != "").Select(item => new UkolVedeniMaterial()
                            {
                                Ukol = new UkolVedeni() { Id = collection.Id },
                                Produkt = new MaterialDao().GetById(Convert.ToInt32(item)),
                                DateAdded = DateTime.Now
                            }).Where(lopProdukt => !history.MaterialyInput.Contains(lopProdukt.Produkt.Id.ToString())))
                            {
                                ukolMaterialDao.Create(ukolProdukt);
                            }
                        }
                        catch
                        {
                            TempData[MessagesHelper.Danger] = "Nepodařilo se připojení produktů k úkolu vedení";
                        }
                    }
                    //TEST SHODNOTI
                    /*
                    if (collection.IsSame(history)) //vrací FALSE, pokud nastala změna
                    {
                        if (produktyInput != history.MaterialyInput)
                        {
                            TempData[MessagesHelper.Success] = "Provázané materiály byly aktualizovány";
                            return RedirectToAction("Detail/" + collection.Id);
                        }
                        TempData[MessagesHelper.Info] = "Nebyly provedeny žádné změny";
                        return RedirectToAction("Detail/" + collection.Id);
                    }

                    //SAVE HISTORY
                    var uvhd = new UkolVedeniHistoryDao();
                    var copy = new UkolVedeniHistory(history);
                    uvhd.Create(copy);
                    */

                    if (collection.DateFinish != null)
                    {
                        collection.Status = StatusUkolu.Closed; //byl zadán datum ukončení? nebo tohle přepíná zadavatel??
                    }

                    //SAVE OBJECT
                    collection.DateLastChanged = DateTime.Now;
                    var uvd = new UkolVedeniDao();
                    uvd.Update(collection);

                    //NOTIFIKACE
                    string s = "Byl upraven úkol <a href='/UkolVedeni/Detail/" + collection.Id + "?showHistory=true#historyBtn'>" + collection.Nazev + "</a>";
                    if (poslatMailResiteli) Notifikace.Create(collection.Resitel.Id, s);
                    if (poslatMailZadavateli) Notifikace.Create(collection.Zadavatel.Id, s);


                    //DONE
                    TempData[MessagesHelper.Success] = "Změny byly úspěšně uloženy";
                    return RedirectToAction("Detail/"+collection.Id);
                }

            }
            catch
            {
                TempData[MessagesHelper.Danger] = "Došlo k neočekávané chybě";
            }

            //NON VALID - RE EDIT IT
            TempData["UkolVedeniHistory"] = history;
            TempData[MessagesHelper.Warning] = "Zkontrolujte zadané údaje";

            var matDao = new MaterialDao();
            var materialList = matDao.GetAll();
            ViewBag.materialList = materialList;
            collection.Materialy = history.Materialy;
            return View(collection);
        }
       
        [Authorize(Roles = "authorized")]
        public ActionResult FinishUkol(int id, bool finished, string komentar)
        {
            try
            {
                var ukolDao = new UkolVedeniDao();
                var ukol = ukolDao.GetById(id);

                ukol.DateLastChanged = DateTime.Now; //něco se děje

                if (finished)
                {
                    /*
                    //uložení historie
                    var histDao = new UkolVedeniHistoryDao();
                    var copy = new UkolVedeniHistory(ukol);
                    histDao.Create(copy);
                    */
                    Uzivatel u = new UzivatelDao().GetByWindowsId(User.Identity.Name.ToUpper());
                    bool isZadavatel = u.Id == ukol.Zadavatel.Id;

                    if (isZadavatel && ukol.Status == StatusUkolu.Closed)
                    {
                        ukol.DateCheck = DateTime.Now; //proběhla kontrola (ať už jakákoliv)
                        ukol.DateFinish = DateTime.Now; //v pořádku    
                    }
                    else
                    {
                        ukol.DateDeadline = DateTime.Now;
                    }

                    ukol.Status = StatusUkolu.Closed; //řešitel pouze uzavírá na close
                    if (!string.IsNullOrEmpty(komentar))
                    {
                        var koment = komentar + "<br />";
                        koment += $"<span class=\"glyphicon glyphicon-time\"></span><i>{DateTime.Now} - by {new Uzivatel(User.Identity.Name)}</i>";
                        ukol.Komentar = (!string.IsNullOrEmpty(ukol.Komentar) ? ukol.Komentar + "<hr />" + koment : koment);
                    }
                    

                    //update objektu
                    ukolDao.Update(ukol);

                    if (isZadavatel)
                    {
                        string s = "Bylo přijaté řešení úkolu vedení " +
                                   "<a href='/UkolVedeni/Detail/" + ukol.Id + "?showHistory=true#historyBtn'>" + ukol.Nazev +
                                   "</a>";
                        Notifikace.Create(ukol.Resitel.Id, s);
                    }
                    else
                    {
                        string s = "Úkol vedení byl označen jako vyřešený " +
                               "<a href='/UkolVedeni/Detail/" + ukol.Id + "'>" + ukol.Nazev + "</a>";
                        Notifikace.Create(ukol.Zadavatel.Id, s);
                    }


                    TempData[MessagesHelper.Success] = "Úkol byl vyřešen";

                    //return RedirectToAction("Detail/" + id);
                }
                else
                {
                    // uložení historie
                    var histDao = new UkolVedeniHistoryDao();
                    var copy = new UkolVedeniHistory(ukol);
                    histDao.Create(copy);

                    //pokud chceme označit jako nevyřešený
                    ukol.DateCheck = DateTime.Now; //proběhla kontrola (ať už jakákoliv)
                    ukol.DateDeadline = null;
                    ukol.DateFinish = null; //nepřijato
                    ukol.Status = StatusUkolu.Open; //úkol musí být dále zpracováván
                    if (!string.IsNullOrEmpty(komentar))
                    {
                        var koment = komentar + "<br />";
                        koment += $"<span class=\"glyphicon glyphicon-time\"></span><i>{DateTime.Now} - by {new Uzivatel(User.Identity.Name)}</i>";
                        ukol.Komentar = (!string.IsNullOrEmpty(ukol.Komentar) ? ukol.Komentar + "<hr />" + koment : koment);
                    }
                    

                    ukolDao.Update(ukol);

                    //update objektu
                    string s = "Bylo odmítnuto řešení úkolu vedení " +
                               "<a href='/UkolVedeni/Detail/" + ukol.Id + "?showHistory=true#historyBtn'>" + ukol.Nazev + "</a>";
                    Notifikace.Create(ukol.Resitel.Id, s);


                    TempData[MessagesHelper.Warning] = "Úkol je znovu v řešení";

                    //return RedirectToAction("Detail/" + id);
                }
            }
            catch
            {
                TempData[MessagesHelper.Warning] = "Nepodařilo se upravit záznam";
            }

            return RedirectToAction("Detail/" + id);

        }

        public ActionResult DenyUkolVedeni(int id, string deniedMessage)
        {
            try
            {
                if (String.IsNullOrEmpty(deniedMessage.Trim()))
                {
                    deniedMessage = "Odmítnuto (bez udání důvodu)"; //natvrdo popisek
                }

                var ukolDao = new UkolVedeniDao();
                var ukol = ukolDao.GetById(id);
                /*
                //uložení historie
                var histDao = new LopHistoryDao();
                var copy = new LopHistory(lop);
                histDao.Create(copy);
                */

                ukol.DateLastChanged = DateTime.Now; //něco se děje
                ukol.DeniedMessage = deniedMessage;

                //update objektu
                ukolDao.Update(ukol);

                var s = ukol.Resitel + " " + Resources.LopTexts.OdmitlResitUkol + " " +
                               "<a href='/Lop/Detail/" + ukol.Id + "'>" + ukol.Nazev +
                               "</a><br />" + ukol.DeniedMessage;
                Notifikace.Create(ukol.Zadavatel.Id, s);

                TempData[MessagesHelper.Success] = Resources.LopTexts.UkolBylOdmitnut;

                return RedirectToAction("Index");
            }
            catch
            {
                TempData[MessagesHelper.Warning] = Resources.LopTexts.NepodariloSeOdmitnoutUkol;
            }

            return RedirectToAction("Detail/" + id);
        }

        [Authorize(Roles = "authorized")]
        public ActionResult DetailHistory(int id)
        {
            var ukolHist = new UkolVedeniHistoryDao().GetById(id); //natáhneme historii a pošleme i s jejím lopem
            return View(new UkolVedeniHistoryVsUkolVedeni(ukolHist, new UkolVedeniDao().GetById(ukolHist.UkolVedeni.Id)));
        }
    }
}