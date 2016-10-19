using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataAccess.Models.Dao;

namespace Web.Controllers
{
    public class ApiOddeleniController : ApiController
    {
        public IEnumerable<OddeleniShort> Get(int usek)
        {
            var dao = new OddeleniDao();

            return dao.GetAllNotInUsek(new UsekDao().GetById(usek)).Select(item => new OddeleniShort()
            {
                Nazev = item.Nazev,
                OddeleniId = item.Id
            }
                ).ToList();
        }


        public class OddeleniShort
        {
            public string Nazev { get; set; }   
            public int OddeleniId { get; set; }
        }
    }
}
