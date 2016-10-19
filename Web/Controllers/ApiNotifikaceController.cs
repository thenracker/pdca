using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Http;
using DataAccess.Models.Dao;

namespace Web.Controllers
{
    public class ApiNotifikaceController : ApiController
    {
        // GET: api/ApiNotifikace/5
        public IList<NotifikaceImpl> Get(int id, string lang="cs")
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);

            if (id == 0)
                return null;
            return new NotifikaceDao().GetByUser(id, false).Select(notifikace => new NotifikaceImpl()
            {
                Id = notifikace.Id, Text = notifikace.Text, Created = notifikace.Created.ToString(CultureInfo.CurrentCulture), Seen = notifikace.Seen
            }).ToList();

        }

        // POST: api/ApiNotifikace
        [AcceptVerbs("POST")]
        [Route("api/ApiNotifikace")]
        public void Post(Pozadavek value)
        {
            var dao = new NotifikaceDao();
            if (value.Typ == "Submit")
            {
                var item = dao.GetById(value.Id);
                item.Seen = true;
                dao.Update(item);
            }
            
        }
        public class Pozadavek
        {
            public int Id { get; set; }
            public string Typ { get; set; }
        }

        public class NotifikaceImpl
        {
            public int Id { get; set; }
            public string Text { get; set; }
            public string Created { get; set; }
            public bool Seen { get; set; }
        }
    }
}
