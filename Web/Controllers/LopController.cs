using System;
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
    public class LopController : Controller
    {
        #region Výpis úkolů
        // GET: Lop
        public ActionResult Index(bool? smazane)
        {
            if (!Uzivatel.UserExists(User.Identity.Name))
            {
                TempData[MessagesHelper.Info] = Resources.HomeTexts.NotAuthorized;
                return RedirectToAction("About", "Home");
            }
            ViewBag.showAll = false;
            var lopDao = new LopDao();
            IList<Lop> listLop;
            if (smazane == true)
            {
                ViewBag.ZeSmazanych = true;
                listLop = lopDao.GetOnlyDeleted(new UzivatelDao().GetByWindowsId(User.Identity.Name));
            }
            else
            {
                listLop = lopDao.GetAll(new UzivatelDao().GetByWindowsId(User.Identity.Name));
            }
            return View(listLop);
        }

        public ActionResult All()
        {
            if (!Uzivatel.UserExists(User.Identity.Name))
            {
                TempData[MessagesHelper.Info] = Resources.HomeTexts.NotAuthorized;
                return RedirectToAction("About", "Home");
            }
            ViewBag.showAll = true;
            return View("Index", new LopDao().GetAll());
        }
        #endregion
        #region Lop
        // GET: Lop/Detail/5
        [Authorize(Roles = "authorized")]
        public ActionResult Detail(int id, bool? showHistory, bool? smazane)
        {
            if (showHistory != null && showHistory == true)
                ViewBag.ShowHistory = true;
            else
                ViewBag.ShowHistory = false;

            if (smazane == true)
            {
                ViewBag.ZeSmazanych = true;
            }

            var lopDao = new LopDao();
            var lop = lopDao.GetById(id);

            if (lop == null)
            {
                TempData[MessagesHelper.Danger] = "Nebyl nalezen lop";
                return RedirectToAction("Index");
            }
            ViewBag.timeline = new LopWorkFlowEventDao().GetByLop(lop);
            return View(lop);
        }
        #region Create, Edit, Delete
        // GET: Lop/Create
        [Authorize(Roles = "authorized")]
        public ActionResult Create()
        {
            ViewBag.FirstCreate = true;
            var item = new Lop()
            {
                Action = 1,
                Zadavatel = new Uzivatel(User.Identity.Name)
                /*,
                PlannedCloseDate = DateTime.Now.AddMonths(1),
                CloseDate = DateTime.Now.AddMonths(1),
                CheckDate = DateTime.Now.AddMonths(1)*/ //je plněno stejně ve formulářích
            };
            return View(item);
        }

        // POST: Lop/Create
        [HttpPost]
        [Authorize(Roles = "authorized")]
        public ActionResult Create(Lop collection, bool poslatMail, string produktyInput)
        {
            ViewBag.FirstCreate = false;
            try
            {
                if (ModelState.IsValid)
                {
                    LopDao lopDao = new LopDao();

                    collection.StartDate = DateTime.Now;
                    collection.LastChangedDate = DateTime.Now;
                    collection.Status = StatusUkolu.Open; //když vytváříme - úkol je standartně otevřený
                    var latest = lopDao.GetLatestLopThisYear();
                    collection.Action = latest?.Action + 1 ?? 1;
                    collection.Id = (int)lopDao.Create(collection);
                    TempData[MessagesHelper.Success] = Resources.LopTexts.UkolPridany;

                    if (produktyInput != "")
                    {
                        try
                        {
                            var ukolMaterialDao = new LopMaterialDao();
                            foreach (var ukolProdukt in produktyInput.Split(';').Where(item => item != "").Select(item => new LopMaterial()
                            {
                                Lop = collection,
                                Produkt = new MaterialDao().GetById(Convert.ToInt32(item)),
                                DateAdded = DateTime.Now
                            }))
                            {
                                ukolMaterialDao.Create(ukolProdukt);
                            }

                        }
                        catch
                        {
                            TempData[MessagesHelper.Danger] = Resources.LopTexts.NepodariloSePripojeniProduktuKUkolu;
                        }
                    }
                    new LopMaterialDao().ClearWrongHistoryAfterCreate(); //po vytvoření je nadbytečná historie lopu

                    //posíláme vždy
                    //if (!poslatMail) return RedirectToAction("Detail", new {id = collection.Id});
                    var s = Resources.LopTexts.BylVamPridelenNovyUkol +
                               " <a href='/Lop/Detail/" + collection.Id + "'>" + collection.Nazev + "</a>";
                    Notifikace.Create(collection.Resitel.Id, s);

                    return RedirectToAction("Detail", new { id = collection.Id });
                }
                TempData[MessagesHelper.Warning] = Resources.LopTexts.ZkontrolujteZadaneUdaje;
            }
            catch
            {
                TempData[MessagesHelper.Danger] = Resources.LopTexts.DosloKNeocekavaneChybe;
            }
            return View(collection);
        }

        // GET: Lop/Edit/5
        [Authorize(Roles = "authorized")]
        public ActionResult Edit(int id)
        {
            var dao = new LopDao();
            var item = dao.GetById(id);
            if (item == null)
                return RedirectToAction("Index");

            var matDao = new MaterialDao();
            var materialList = matDao.GetAll();
            ViewBag.materialList = materialList;

            TempData["LopHistory"] = item;
            return View(item);
        }

        // POST: Lop/Edit/5
        [HttpPost]
        [Authorize(Roles = "authorized")]
        public ActionResult Edit(int id, Lop collection, bool poslatMailResiteli, bool poslatMailZadavateli, string produktyInput)
        {
            var history = TempData["LopHistory"] as Lop;
            if (history == null)
                return RedirectToAction("Index");
            try
            {
                collection.Zadavatel = new UzivatelDao().GetByWindowsId(User.Identity.Name);
                if (ModelState.IsValid)
                {
                    if (produktyInput != history.MaterialyInput)
                    {
                        var lopMaterialDao = new LopMaterialDao();
                        foreach (var item in history.MaterialyInput.Split(';').Where(item => !produktyInput.Contains(";" + item + ";")).Where(item => item != ""))
                        { // Odebrání přebytečných
                            lopMaterialDao.Delete(history.Materialy.First(x => x.Produkt.Id == Convert.ToInt32(item)));
                        }
                        try
                        {//Přidání nových
                            foreach (var lopProdukt in produktyInput.Split(';').Where(item => item != "").Select(item => new LopMaterial()
                            {
                                Lop = new Lop() { Id = collection.Id },
                                Produkt = new MaterialDao().GetById(Convert.ToInt32(item)),
                                DateAdded = DateTime.Now
                            }).Where(lopProdukt => !history.MaterialyInput.Contains(lopProdukt.Produkt.Id.ToString())))
                            {
                                lopMaterialDao.Create(lopProdukt);
                            }
                        }
                        catch
                        {
                            TempData[MessagesHelper.Danger] = Resources.LopTexts.NepodariloSePripojeniProduktuKUkolu;
                        }
                    }

                    //TEST SHODNOTI
                    /*
                    if (collection.IsSame(history)) //vrací FALSE, pokud nastala změna
                    {
                        if (produktyInput != history.MaterialyInput)
                        {
                            TempData[MessagesHelper.Success] = Resources.LopTexts.ProvazaneMaterialyBylyAktualizovany;
                            return RedirectToAction("Detail/" + collection.Id);
                        }
                        TempData[MessagesHelper.Info] = Resources.LopTexts.NebylyProvedenyZadneZmeny;
                        return RedirectToAction("Detail/" + collection.Id);
                    }
                    */

                    //KONTROLA JESTLI LZE ÚKOL UZAVŘÍT
                    if (collection.Status == StatusUkolu.Closed)
                    {
                        int celkemUkolu = history.PocetSubUkoluCelkem();
                        int ukoluHotovych = history.PocetSubUkoluCompleted();

                        if (celkemUkolu != ukoluHotovych)
                        {
                            TempData[MessagesHelper.Info] = Resources.LopTexts.NelzeUkoncitUkol1 + " (" +
                                                            (celkemUkolu - ukoluHotovych) + ") " + Resources.LopTexts.NelzeUkoncitUkol2;
                        }
                    }
                    else
                    {
                        //ZMĚNA? - ULOŽÍME DO HISTORIE A PŘEPÍŠEME
                        var lopDao = new LopDao();
                        var lopHistDao = new LopHistoryDao();
                        /*
                        //uložení historie
                        var copy = new LopHistory(history);
                        lopHistDao.Create(copy);
                        */
                        //update objektu
                        collection.LastChangedDate = DateTime.Now;
                        lopDao.Update(collection);
                        TempData[MessagesHelper.Success] = Resources.LopTexts.LopUspesneAktualizovan;

                        if (poslatMailResiteli)
                        {
                            var s = Resources.LopTexts.BylUpravenLop +
                                       " <a href='/Lop/Detail/" + collection.Id + "?showHistory=true#historyBtn'>" + collection.Nazev + "</a>";
                            Notifikace.Create(collection.Resitel.Id, s);
                        }
                        if (poslatMailZadavateli)
                        {
                            var s = Resources.LopTexts.BylUpravenLop +
                                       " <a href='/Lop/Detail/" + collection.Id + "?showHistory=true#historyBtn'>" + collection.Nazev + "</a>";
                            Notifikace.Create(collection.Zadavatel.Id, s);
                        }
                        return Redirect(Url.RouteUrl(new { controller = "Lop", action = "Detail", id = collection.Id, showHistory = true }));
                    }
                    //return RedirectToAction("Detail", new {id = collection.Id, showHistory = true});
                }
                TempData[MessagesHelper.Warning] = Resources.LopTexts.ZkontrolujteZadaneUdaje;
            }
            catch (Exception)
            {
                TempData[MessagesHelper.Danger] = Resources.LopTexts.DosloKNeocekavaneChybe;
            }
            TempData["LopHistory"] = history;
            var matDao = new MaterialDao();
            var materialList = matDao.GetAll();
            ViewBag.materialList = materialList;
            collection.SubUkoly = history.SubUkoly;
            collection.Materialy = history.Materialy;
            return View(collection);
        }
        // GET: Lop/Delete/5
        [Authorize(Roles = "authorized")]
        public ActionResult Delete(int id)
        {
            var lopDao = new LopDao();
            var lop = lopDao.GetById(id);
            try
            {
                lop.Deleted = true;
                lopDao.Update(lop);
            }
            catch (Exception)
            {
                TempData[MessagesHelper.Danger] = Resources.LopTexts.DosloKNeocekavaneChybe;
                return RedirectToAction("Detail", new { id = lop.Id });
            }
            TempData[MessagesHelper.Success] = Resources.LopTexts.UkolSmazan;
            return RedirectToAction("Index");
        }
        #endregion
        #region workflow finish, deny
        [HttpPost]
        [Authorize(Roles = "authorized")]
        public ActionResult FinishUkol(int id, bool finished, string komentar)
        {
            try
            {
                LopDao lopD = new LopDao();
                Lop lop = lopD.GetById(id);

                lop.LastChangedDate = DateTime.Now; //něco se děje

                if (finished)
                {
                    //KONTROLA JESTLI LZE UKONČIT ÚKOL, DOKUD NEJSOU UKONČENY PODÚKOLY
                    //teoreticky můžeme přesměrovat na hlášku, jestli chce zadavatel označit všechny podúkoly
                    //jako vyřešené?
                    //btw .. tlačítko na tuhle akci ukážu jen pokud jsou všechny done ;)
                    int celkemUkolu = lop.PocetSubUkoluCelkem();
                    int ukoluHotovych = lop.PocetSubUkoluCompleted();

                    if (celkemUkolu != ukoluHotovych)
                    {
                        TempData[MessagesHelper.Info] = Resources.LopTexts.NelzeUkoncitUkol1 + " (" + (celkemUkolu - ukoluHotovych) + ") " + Resources.LopTexts.NelzeUkoncitUkol2;
                        return RedirectToAction("Index");
                    }
                    Uzivatel u = new UzivatelDao().GetByWindowsId(User.Identity.Name.ToUpper());
                    bool isZadavatel = u.Id == lop.Zadavatel.Id;
                    bool isZadavatelATeprveUzavira = false;
                    if (isZadavatel && lop.Status == StatusUkolu.Open) //v případě že je uživatel zároveň Zadavatel a Řešitel
                    {
                        isZadavatelATeprveUzavira = true;
                    }

                    if (isZadavatel && lop.Status == StatusUkolu.Closed)
                    {
                        lop.CheckDate = DateTime.Now; //proběhla kontrola (ať už jakákoliv)
                        lop.FinishDate = DateTime.Now; //v pořádku    

                        #region workflow-event
                        LopWorkFlowEvent.Create(lop,
                            LopWorkFlowEvent.Colors.Success,
                            LopWorkFlowEvent.Icons.Ok,
                            "Úkol uzavřen",
                            "Zadavatel přijal řešení úkolu a úkol uzavřel jako splněný. <q>"+komentar+"</q>",
                            User.Identity.Name
                            );
                        #endregion
                    }
                    else
                    {
                        #region workflow-event
                        LopWorkFlowEvent.Create(lop,
                            LopWorkFlowEvent.Colors.Info,
                            LopWorkFlowEvent.Icons.User,
                            "Úkol dokončen",
                            "Řešitel označil úkol jako splněný - čeká na schválení zadavatelem. <q>" + komentar + "</q>",
                            User.Identity.Name
                            );
                        #endregion
                        lop.CloseDate = DateTime.Now;
                    }

                    lop.Status = StatusUkolu.Closed; //řešitel pouze uzavírá na close
                    if (!string.IsNullOrEmpty(komentar))
                    {
                        var koment = komentar + "<br />";
                        koment += $"<span class=\"glyphicon glyphicon-time\"></span><i>{DateTime.Now} - by {new Uzivatel(User.Identity.Name)}</i>";
                        lop.Komentar = (!string.IsNullOrEmpty(lop.Komentar) ? lop.Komentar + "<hr />" + koment : koment);
                    }
                    //update objektu
                    lopD.Update(lop);
                    if (isZadavatel && !isZadavatelATeprveUzavira) //tuto hlášku nechceme generovat, pokud je to sice zadavatel ale zároveň řešitel a teprve uzavírá úkol poprvé ;)
                    {
                        string s = Resources.LopTexts.ByloPrijatoReseniLopu +
                                   " <a href='/Lop/Detail/" + lop.Id + "?showHistory=true#historyBtn'>" + lop.Nazev +
                                   "</a>";
                        Notifikace.Create(lop.Resitel.Id, s);
                    }
                    else
                    {
                        string s = Resources.LopTexts.UkolBylOznacenJakoVyreseny +
                               " <a href='/Lop/Detail/" + lop.Id + "'>" + lop.Nazev + "</a>";
                        Notifikace.Create(lop.Zadavatel.Id, s);
                    }
                    TempData[MessagesHelper.Success] = Resources.LopTexts.UkolBylVyresen;
                }
                else
                {
                    //pokud chceme označit jako nevyřešený
                    lop.CheckDate = DateTime.Now; //proběhla kontrola (ať už jakákoliv)
                    lop.CloseDate = null;
                    lop.FinishDate = null; //nepřijato
                    lop.Status = StatusUkolu.Open; //úkol musí být dále zpracováván
                    if (!string.IsNullOrEmpty(komentar))
                    {
                        var koment = komentar + "<br />";
                        koment += $"<span class=\"glyphicon glyphicon-time\"></span><i>{DateTime.Now} - by {new Uzivatel(User.Identity.Name)}</i>";
                        lop.Komentar = (!string.IsNullOrEmpty(lop.Komentar) ? lop.Komentar + "<hr />" + koment : koment);
                    }
                    lopD.Update(lop);
                    #region workflow-event
                    LopWorkFlowEvent.Create(lop,
                            LopWorkFlowEvent.Colors.Danger,
                            LopWorkFlowEvent.Icons.Remove,
                            "Řešení zamítnuto",
                            "Zadavatel odmítl odevzdané řešení úkolu. <q>" + komentar + "</q>",
                            User.Identity.Name
                            );
                    #endregion
                    string s = Resources.LopTexts.ByloOdmitnutoReseniUkolu +
                               " <a href='/Lop/Detail/" + lop.Id + "?showHistory=true#historyBtn'>" + lop.Nazev + "</a>";
                    Notifikace.Create(lop.Resitel.Id, s);
                    TempData[MessagesHelper.Warning] = Resources.LopTexts.UkolJeZnovuVReseni;
                }
            }
            catch (Exception e)
            {
                TempData[MessagesHelper.Warning] = Resources.LopTexts.NepodariloSeUpravitZaznam;
            }

            return RedirectToAction("Detail/" + id);

        }
        [Authorize(Roles = "authorized")]
        public ActionResult DenyLop(int id, string deniedMessage)
        {

            try
            {
                if (String.IsNullOrEmpty(deniedMessage.Trim()))
                {
                    deniedMessage = "Odmítnuto (bez udání důvodu)"; //natvrdo popisek
                }

                LopDao lopDao = new LopDao();
                Lop lop = lopDao.GetById(id);
                /*
                //uložení historie
                var histDao = new LopHistoryDao();
                var copy = new LopHistory(lop);
                histDao.Create(copy);
                */

                lop.LastChangedDate = DateTime.Now; //něco se děje
                lop.DeniedMessage = deniedMessage;

                //update objektu
                lopDao.Update(lop);

                LopWorkFlowEvent.Create(lop,
                            LopWorkFlowEvent.Colors.Danger,
                            LopWorkFlowEvent.Icons.User,
                            "Odmítnutí odpovědnosti",
                            "Řešitel odmítl řešit tento úkol. <q>"+deniedMessage+"</q>",
                            User.Identity.Name
                            );

                var s = lop.Resitel + " " + Resources.LopTexts.OdmitlResitUkol + " " +
                               "<a href='/Lop/Detail/" + lop.Id + "'>" + lop.Nazev +
                               "</a><br />" + lop.DeniedMessage;
                Notifikace.Create(lop.Zadavatel.Id, s);

                TempData[MessagesHelper.Success] = Resources.LopTexts.UkolBylOdmitnut;

                return RedirectToAction("Index");
            }
            catch
            {
                TempData[MessagesHelper.Warning] = Resources.LopTexts.NepodariloSeOdmitnoutUkol;
            }

            return RedirectToAction("Detail/" + id);
        }
        #endregion
        #region historie
        [Authorize(Roles = "authorized")]
        public ActionResult DetailHistory(int id)
        {
            var lopHist = new LopHistoryDao().GetById(id); //natáhneme historii a pošleme i s jejím lopem
            return View(new LopHistoryVsLop(lopHist, new LopDao().GetById(lopHist.Lop.Id)));
        }
        #endregion
        #endregion
        #region SubUkol
        //SEKCE podúkolŮ
        [Authorize(Roles = "authorized")]
        public ActionResult DetailSubUkol(int id, bool? showHistory, bool? smazane)
        {
            if (showHistory != null && showHistory == true)
                ViewBag.ShowHistory = true;
            else
                ViewBag.ShowHistory = false;


            if (smazane == true)
            {
                ViewBag.ZeSmazanych = true;
            }

            var item = new SubUkolDao().GetById(id);
            return View(item);
        }
        #region Create, Edit, Delete
        [Authorize(Roles = "authorized")]
        public ActionResult CreateSubUkol(int lopId)
        {
            var item = new SubUkol
            {
                Action = 1,
                LopId = lopId,
                Zadavatel = new Uzivatel(User.Identity.Name)
            }; //naplnění lopu a akce 
            ViewBag.FirstCreate = true;
            return View(item);
        }
        [HttpPost]
        [Authorize(Roles = "authorized")]
        public ActionResult CreateSubUkol(SubUkol collection, bool poslatMail)
        {
            ViewBag.FirstCreate = false;
            try
            {
                if (ModelState.IsValid)
                {
                    SubUkolDao sDao = new SubUkolDao();

                    collection.StartDate = DateTime.Now;
                    collection.LastChangedDate = DateTime.Now;
                    collection.Status = StatusUkolu.Open;
                    var latest = sDao.GetLatestSubUkolThisYear();
                    collection.Action = latest?.Action + 1 ?? 1;
                    collection.Id = (int) sDao.Create(collection);
                    TempData[MessagesHelper.Success] = Resources.LopTexts.PodukolPridany;
                    //posíláme vždy
                    //if (poslatMail)
                    var s = Resources.LopTexts.BylVamPridelenNovyPodukol +
                            " <a href='/Lop/DetailSubUkol/" + collection.Id + "'>" + collection.Nazev + "</a>";
                    Notifikace.Create(collection.Resitel.Id, s);
                }
                else
                {
                    TempData[MessagesHelper.Warning] = Resources.LopTexts.ZkontrolujteZadaneUdaje;
                    return View(collection);
                }
            }
            catch
            {
                TempData[MessagesHelper.Warning] = Resources.LopTexts.DosloKNeocekavaneChybe;
                return View(collection);
            }
            

            return RedirectToAction("DetailSubUkol", new {id = collection.Id});
        }

        public ActionResult EditSubUkol(int id) //id zbytečné? - naopak, stačí Id, tohle je před odelsáním formuláře
        {
            var item = new SubUkolDao().GetById(id);
            if (item == null)
                return RedirectToAction("Index");

            TempData["LopSubUkolHistory"] = item;
            return View(item);
        }
        [HttpPost]
        [Authorize(Roles = "authorized")]
        public ActionResult EditSubUkol(int id, SubUkol collection, bool poslatMailResiteli, bool poslatMailZadavateli)
        {
            var history = TempData["LopSubUkolHistory"] as SubUkol;
            if (history == null)
                return RedirectToAction("Index");
            try
            {
                if (ModelState.IsValid)
                {
                    //TEST SHODNOTI
                    if (collection.IsSame(history)) //vrací FALSE, pokud nastala změna
                    {
                        TempData[MessagesHelper.Info] = Resources.LopTexts.NebylyProvedenyZadneZmeny;
                        return RedirectToAction("Edit", new { id = collection.Lop.Id });
                    }

                    //ZMĚNA? - ULOŽÍME DO HISTORIE A PŘEPÍŠEME
                    var subUkolDao = new SubUkolDao();
                    var subUkolHistDao = new SubUkolHistoryDao();
                    /*
                    //uložení historie
                    var copy = new SubUkolHistory(history);
                    subUkolHistDao.Create(copy);
                    */
                    //update objektu
                    collection.LastChangedDate = DateTime.Now;

                    subUkolDao.Update(collection);

                    TempData[MessagesHelper.Success] = Resources.LopTexts.ZmenyUspesneUlozeny;

                    //NOTIFIKACE
                    if (poslatMailResiteli)
                    {
                        string s = Resources.LopTexts.BylUpravenLop +
                                   " <a href='/LOP/DetailSubUkol/" + collection.Id + "?showHistory=true#historyBtn'>" + collection.Nazev + "</a>";
                        Notifikace.Create(collection.Resitel.Id, s);
                    }
                    if (poslatMailZadavateli)
                    {
                        string s = Resources.LopTexts.BylUpravenLop +
                                   " <a href='/LOP/DetailSubUkol/" + collection.Id + "?showHistory=true#historyBtn'>" + collection.Nazev + "</a>";
                        Notifikace.Create(collection.Zadavatel.Id, s);
                    }

                    return Redirect(Url.RouteUrl(new { controller = "Lop", action = "DetailSubUkol", id = collection.Id, showHistory = true }) + "#historyBtn");

                    //return RedirectToAction("DetailSubUkol", new {id = collection.Id});
                }
                TempData[MessagesHelper.Warning] = Resources.LopTexts.ZkontrolujteZadaneUdaje;
            }
            catch
            {
                TempData[MessagesHelper.Danger] = Resources.LopTexts.DosloKNeocekavaneChybe;
            }

            TempData["LopSubUkolHistory"] = history;
            return View(collection);
        }
        
        [Authorize(Roles = "authorized")]
        public ActionResult DeleteSubukol(int id)
        {
            var suDao = new SubUkolDao();
            var su = suDao.GetById(id);
            try
            {
                su.Deleted = true;
                suDao.Update(su);
            }
            catch
            {
                TempData[MessagesHelper.Danger] = Resources.LopTexts.DosloKNeocekavaneChybe;
                return RedirectToAction("DetailSubUkol", new {id = su.Id});
            }
            TempData[MessagesHelper.Success] = Resources.LopTexts.PodukolSmazan;
            return RedirectToAction("Detail", new {id = su.Lop.Id});
        }
        [Authorize(Roles = "authorized")]

        #endregion
        #region workflow - finish, deny
        [HttpPost]
        [Authorize(Roles = "authorized")]
        public ActionResult FinishSubukol(int id, bool finished, string komentar, bool? backToLopDetail)
        {
            try
            {
                SubUkolDao subDao = new SubUkolDao();
                SubUkol su = subDao.GetById(id);

                su.LastChangedDate = DateTime.Now; //něco se děje

                if (finished)
                {/*
                    //uložení historie
                    var histDao = new SubUkolHistoryDao();
                    var copy = new SubUkolHistory(su);
                    histDao.Create(copy);
                    */
                    Uzivatel u = new UzivatelDao().GetByWindowsId(User.Identity.Name.ToUpper());
                    bool isZadavatel = u.Id == su.Zadavatel.Id;

                    if (isZadavatel && su.Status == StatusUkolu.Closed)
                    {
                        su.CheckDate = DateTime.Now; //proběhla kontrola (ať už jakákoliv)
                        su.FinishDate = DateTime.Now; //v pořádku    
                    }
                    else
                    {
                        su.CloseDate = DateTime.Now;
                    }

                    su.Status = StatusUkolu.Closed; //řešitel pouze uzavírá na close

                    if (!string.IsNullOrEmpty(komentar))
                    {
                        var koment = komentar + "<br />";
                        koment += $"<span class=\"glyphicon glyphicon-time\"></span><i>{DateTime.Now} - by {new Uzivatel(User.Identity.Name)}</i>";
                        su.Komentar = (!string.IsNullOrEmpty(su.Komentar) ? su.Komentar + "<hr />" + koment : koment);
                    }



                    //update objektu
                    subDao.Update(su);

                    if (isZadavatel)
                    {
                        string s = Resources.LopTexts.ByloPrijatoReseniPodukolu +
                                   " <a href='/Lop/DetailSubUkol/" + su.Id + "?showHistory=true#historyBtn'>" + su.Nazev +
                                   "</a>";
                        Notifikace.Create(su.Resitel.Id, s);
                    }
                    else
                    {
                        string s = Resources.LopTexts.PodukolBylOznacenJakoVyreseny +
                               " <a href='/Lop/DetailSubUkol/" + su.Id + "'>" + su.Nazev + "</a>";
                        Notifikace.Create(su.Zadavatel.Id, s);
                    }


                    TempData[MessagesHelper.Success] = Resources.LopTexts.PodukolBylVyresen;

                    if (backToLopDetail != null && backToLopDetail == true)
                    {
                        return RedirectToAction("Detail/" + su.Lop.Id);
                    }
                }
                else
                {/*
                    // uložení historie
                    var histDao = new SubUkolHistoryDao();
                    var copy = new SubUkolHistory(su);
                    histDao.Create(copy);
                    */
                    //pokud chceme označit jako nevyřešený
                    su.CheckDate = DateTime.Now; //proběhla kontrola (ať už jakákoliv)
                    su.CloseDate = null;
                    su.FinishDate = null; //nepřijato
                    su.Status = StatusUkolu.Open; //úkol musí být dále zpracováván

                    if (!string.IsNullOrEmpty(komentar))
                    {
                        var koment = komentar + "<br />";
                        koment += $"<span class=\"glyphicon glyphicon-time\"></span><i>{DateTime.Now} - by {new Uzivatel(User.Identity.Name)}</i>";
                        su.Komentar = (!string.IsNullOrEmpty(su.Komentar) ? su.Komentar + "<hr />" + koment : koment);
                    }


                    subDao.Update(su);

                    //update objektu
                    string s = Resources.LopTexts.ByloOdmitnutoReseniPodukolu +
                               " <a href='/Lop/DetailSubUkol/" + su.Id + "?showHistory=true#historyBtn'>" + su.Nazev + "</a>";
                    Notifikace.Create(su.Resitel.Id, s);


                    TempData[MessagesHelper.Warning] = Resources.LopTexts.PodukolJeZnovuVReseni;

                    if (backToLopDetail != null && backToLopDetail == true)
                    {
                        return RedirectToAction("Detail/" + su.Lop.Id);
                    }
                }
            }
            catch
            {
                TempData[MessagesHelper.Warning] = Resources.LopTexts.NepodariloSeUpravitZaznam;
            }

            return RedirectToAction("DetailSubUkol/" + id);
        }
        [Authorize(Roles = "authorized")]
        public ActionResult DenySubUkol(int id, string deniedMessage)
        {

            try
            {
                SubUkolDao subDao = new SubUkolDao();
                SubUkol su = subDao.GetById(id);
                /*
                //uložení historie
                var histDao = new SubUkolHistoryDao();
                var copy = new SubUkolHistory(su);
                histDao.Create(copy);
                */

                su.LastChangedDate = DateTime.Now; //něco se děje
                su.DeniedMessage = deniedMessage;

                //update objektu
                subDao.Update(su);

                var s = su.Resitel + " " + Resources.LopTexts.OdmitlResitPodukol + " " +
                               "<a href='/Lop/DetailSubUkol/" + su.Id + "'>" + su.Nazev +
                               "</a><br />" + su.DeniedMessage;
                Notifikace.Create(su.Zadavatel.Id, s);

                TempData[MessagesHelper.Success] = Resources.LopTexts.PodukolBylOdmitnut;

                return RedirectToAction("Detail", new { su.Lop.Id });
            }
            catch
            {
                TempData[MessagesHelper.Warning] = Resources.LopTexts.NepodariloSeOdmitnoutPodukol;
            }

            return RedirectToAction("SubUkolDetail/" + id);
        }
        #endregion
        #region history
        [Authorize(Roles = "authorized")]
        public ActionResult DetailSubUkolHistory(int id)
        {
            var lopHist = new SubUkolHistoryDao().GetById(id); //natáhneme historii a pošleme i s jejím lopem
            return View(new SubUkolHistoryVsSubUkol(lopHist, new SubUkolDao().GetById(lopHist.SubUkol.Id)));
        }
        #endregion

        [Authorize(Roles = "authorized")]
        public ActionResult KomentyBySubukol(int id)
        {
            var subUkol = new SubUkolDao().GetById(id);
            return View(subUkol);
        }
        #endregion
    }
}
