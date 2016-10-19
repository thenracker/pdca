using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess.Models.Dao;
using DataAccess.Models.DataUnit;
using Web.Models;

namespace Web.Controllers
{
    public class OrganizaceController : Controller
    {
        // GET: Organizace
        public ActionResult Index()
        {
            var dao = new UsekDao();

            var useky = dao.GetAll();

            return View(useky);
        }
        //ÚSEK------------------------------
        public ActionResult DetailUsek(int id)
        {
            var item = new UsekDao().GetById(id);
            return View(item);
        }
        
        public ActionResult EditUsek(int id)
        {
            var item = new UsekDao().GetById(id);
            return View(item);
        }

        [HttpPost]
        public ActionResult EditUsek(int id, Usek collection)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var dao = new UsekDao();
                    dao.Update(collection);
                    return RedirectToAction("DetailUsek", new { id = id });
                }

                TempData[MessagesHelper.Warning] = Resources.LopTexts.ZkontrolujteZadaneUdaje;
            }
            catch
            {
                TempData[MessagesHelper.Danger] = Resources.LopTexts.DosloKNeocekavaneChybe;
            }

            return View(collection);
        }

        public ActionResult AddOddeleniToUsek(int usekId, int oddeleniId)
        {
            var dao = new OddeleniDao();
            var usek = new UsekDao().GetById(usekId);
            var oddeleni = dao.GetById(oddeleniId);

            oddeleni.Usek = usek; //změna úseku

            dao.Update(oddeleni);

            return RedirectToAction("EditUsek", new {id = usekId});
        }

        //ODDĚLENÍ-----------------------------------
        public ActionResult DetailOddeleni(int id)
        {
            var item = new OddeleniDao().GetById(id);
            return View(item);
        }

        public ActionResult EditOddeleni(int id)
        {
            var item = new OddeleniDao().GetById(id);
            return View(item);
            
        }
        [HttpPost]
        public ActionResult EditOddeleni(int id, Oddeleni collection)
        {
            
            try
            {
                var dao = new OddeleniDao();
                var puvodniCollection = dao.GetById(id);

                if (ModelState.IsValid)
                {
                    if (collection.UsekId != 0) //pokud není nula, byla provedena změna úseku a musíme dotáhnout objekt
                    {
                        puvodniCollection.Usek = new UsekDao().GetById(collection.UsekId);
                    }

                    if (!puvodniCollection.Nazev.Equals(collection.Nazev)) puvodniCollection.Nazev = collection.Nazev;

                    dao.Update(puvodniCollection); //bude potřeba dotáhnout seznam oddělení??
                    return RedirectToAction("DetailOddeleni", new { id = id });
                }

                TempData[MessagesHelper.Warning] = Resources.LopTexts.ZkontrolujteZadaneUdaje;
            }
            catch
            {
                TempData[MessagesHelper.Danger] = Resources.LopTexts.DosloKNeocekavaneChybe;
            }

            return View(collection);
        }
        public ActionResult AddVedouciToOddeleni(int oddeleniId, string windowsId)
        {
            var oddeleni = new OddeleniDao().GetById(oddeleniId);
            var novyVedouci = new UzivatelDao().GetByWindowsId(windowsId);

            var dao = new VedouciOddeleniDao();
            var item = new VedouciOdeleni() {Oddeleni = oddeleni, WindowsId = windowsId };

            dao.Create(item);

            return RedirectToAction("EditOddeleni", new { id = oddeleniId });
        }

        public ActionResult RemoveVedouciFromOddeleni(string windowsId , int oddeleniId)
        {
            var dao = new OddeleniDao();
            var oddeleni = dao.GetById(oddeleniId);

            if (oddeleni.VedouciOdeleni.Count > 1)//jeden vedoucí musí zůstat ;)
            {
                var found = oddeleni.VedouciOdeleni.SingleOrDefault(x => x.WindowsId == windowsId);
                if (found != null)
                {
                    oddeleni.VedouciOdeleni.Remove(found);
                    dao.Update(oddeleni);
                }
            }

            return RedirectToAction("EditOddeleni", new { id = oddeleniId });
        }

        //UŽIVATELÉ --------
        public ActionResult DetailUzivatel(int id)
        {
            var dao = new UzivatelDao();
            var item = dao.GetById(id);
            return View(item);
        }

        public ActionResult EditUzivatel(int id)
        {
            var dao = new UzivatelDao();
            var item = dao.GetById(id);
            return View(item);
        }

        public ActionResult AddDilnaToUzivatel(int uzivatelId, int dilnaId)
        {
            var dao = new UzivatelDao();
            var uzivatel = dao.GetById(uzivatelId);
            var novaDilna = new DbbDilnaDao().GetById(dilnaId);
            uzivatel.DilnyUzivatelu.Add(new DilnyUzivatelu() {Dilna = novaDilna, Uzivatel = uzivatel});

            dao.Update(uzivatel); //tady máme chybku hele

            return RedirectToAction("EditUzivatel",new {id = uzivatelId});
        }

        public ActionResult RemoveDilnaFromUzivatel(int uzivatelId, int dilnaId)
        {
            var dao = new UzivatelDao();
            var item = dao.GetById(uzivatelId);
            item.DilnyUzivatelu.Remove(item.DilnyUzivatelu.Single(s => s.Dilna.Id == dilnaId));
            dao.Update(item);
            return RedirectToAction("EditUzivatel",new {id=uzivatelId});
        }
    }
}