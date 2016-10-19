using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataAccess.Models.Dao;

namespace Web.Controllers
{
    public class ApiUzivatelController : ApiController
    {
        public IEnumerable<UzivatelShort> Get()
        {
            var dao = new UzivatelDao();

            return dao.GetAll().Select(item => new UzivatelShort()
            {
                Jmeno = item.Jmeno,
                Prijmeni = item.Prijmeni,
                WindowsId = item.WindowsId
            })
            .ToList();
            
        }

        public IEnumerable<UzivatelShort> Get(int oddeleniId)
        {
            var dao = new UzivatelDao();

            return dao.GetAllNotVedouciOddeleni(new OddeleniDao().GetById(oddeleniId)).Select(item => new UzivatelShort()
            {
                Jmeno = item.Jmeno,
                Prijmeni = item.Prijmeni,
                WindowsId = item.WindowsId
            })
            .ToList();

        }


        public class UzivatelShort
        {
            public string Jmeno { get; set; }   
            public string Prijmeni { get; set; }
            public string WindowsId { get; set; }
        }

        public IEnumerable<DilnaShort> Get(int uzivatelId, string dalsiFunkce) //dotáhne všechny dílny, které uživatel nemá ;)
        {
            var dao = new DbbDilnaDao();
            return dao.GetAllDilnaNotWithUzival(new UzivatelDao().GetById(uzivatelId)).Select(item => new DilnaShort()
            {
                Nazev = item.Nazev,
                DilnaId = item.Id
            })
                .ToList();
        }

        public class DilnaShort
        {
            public string Nazev { get; set; }
            public int DilnaId { get; set; }
        }
    }
}
