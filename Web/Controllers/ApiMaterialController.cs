using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DataAccess.Models.Dao;

namespace Web.Controllers
{
    public class ApiMaterialController : ApiController
    {
        public IEnumerable<MaterialImpl> Get()
        {
            var dao = new MaterialDao();

            return dao.GetAll().Select(item => new MaterialImpl()
            {
                Id = item.Id, Druh=item.Druh, Pn = item.Pn, Popis = item.Popis, SapCislo = item.SapCislo, SapPopis = item.PopisSap
            }).ToList();
        }
        public MaterialImpl Get(int id)
        {
            var dao = new MaterialDao();
            var item = dao.GetById(id);

            return new MaterialImpl()
            {
                Id = item.Id,
                Druh = item.Druh,
                Pn = item.Pn,
                Popis = item.Popis,
                SapCislo = item.SapCislo,
                SapPopis = item.PopisSap
            };
        }

        public class MaterialImpl
        {
            public int Id { get; set; }
            public string Druh { get; set; }
            public string Pn { get; set; }
            public string Popis { get; set; }
            public string SapCislo { get; set; }
            public string SapPopis { get; set; }

        }
    }
}
