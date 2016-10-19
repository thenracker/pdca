using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Models.DataUnit;
using DataAccess.Models.DataUnit.Users;
using NHibernate.Criterion;

namespace DataAccess.Models.Dao
{
    public class UzivatelDao : DaoBase<Uzivatel>
    {
        private static IList<Uzivatel> Cache; 
        public Uzivatel GetByOsCislo(string osCislo)
        {
            return Session.CreateCriteria<Uzivatel>().Add(Restrictions.Eq("OsCislo", osCislo)).UniqueResult<Uzivatel>();
        }
        public Uzivatel GetByWindowsId(string windowsId)
        {
            var winId = windowsId.Trim().ToUpper();
            if (Cache==null)
                Cache = new List<Uzivatel>();

            if(Cache.Any(x => x.WindowsId == winId))
            {
                var item = Cache.First(x => x.WindowsId == winId);
                if (item.PlatnostDo > DateTime.Today)
                    return item;
                Cache.Remove(item);
            }

            try
            {
                var item = Session.CreateCriteria<Uzivatel>()
                        .Add(Restrictions.Eq("WindowsId", winId))
                        .UniqueResult<Uzivatel>();
                item.PlatnostDo = DateTime.Today.AddDays(7);
                Cache.Add(item);
                return item;
            }
            catch{ return null; }
            
        }

        public IList<Uzivatel> GetAllNotVedouciOddeleni(Oddeleni oddeleni)
        {

            Uzivatel alias = null;
            return Session.QueryOver<Uzivatel>(() => alias)
                .WithSubquery.WhereNotExists(QueryOver.Of<VedouciOdeleni>()
                    .Where(c => c.WindowsId == alias.WindowsId)
                    .And(c => c.Oddeleni.Id == oddeleni.Id)
                    .Select(c => c.WindowsId))
                .List<Uzivatel>();
            //nebo-li
            //select * from Uzivatel where Uzivatel.WindowsId NOT IN (SELECT WindowsId from VedouciOddeleni Where Oddeleni == oddeleni.Id)
        }

        public static void ClearFromCache(Uzivatel user)
        {
            try
            {
                Cache.Remove(user);
            }
            catch { }
        }
    }

}
