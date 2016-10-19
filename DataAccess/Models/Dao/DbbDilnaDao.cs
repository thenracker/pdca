using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Models.DataUnit;
using DataAccess.Models.DataUnit.Users;
using NHibernate.Criterion;

namespace DataAccess.Models.Dao
{
    public class DbbDilnaDao : DaoBase<DbbDilna>
    {

        public IList<DbbDilna> GetAllDilnaNotWithUzival(Uzivatel uzivatel)
        {

            DbbDilna alias = null;
            return Session.QueryOver<DbbDilna>(() => alias)
                .WithSubquery.WhereNotExists(QueryOver.Of<DilnyUzivatelu>()
                    .Where(c => c.Dilna.Id == alias.Id)
                    .And(c => c.Uzivatel.Id == uzivatel.Id)
                    .Select(c => c.Dilna.Id))
                .List<DbbDilna>();
            //nebo-li
            //select * from Uzivatel where Uzivatel.WindowsId NOT IN (SELECT WindowsId from VedouciOddeleni Where Oddeleni == oddeleni.Id)
        }
    }

}
