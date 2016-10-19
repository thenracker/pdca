using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataAccess.Models.Dao;

namespace Web.Controllers
{
    public class ApiUsekController : ApiController
    {
        public IEnumerable<UsekShort> Get()
        {
            var dao = new UsekDao();

            return dao.GetAll().Select(item => new UsekShort()
            {
                Nazev = item.Nazev,
                UsekId = item.Id
            }
                ).ToList();
        }


        public class UsekShort
        {
            public string Nazev { get; set; }   
            public int UsekId { get; set; }
        }
    }
}
