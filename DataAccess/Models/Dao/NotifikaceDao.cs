using System.Collections.Generic;
using DataAccess.Models.DataUnit;
using DataAccess.Models.DataUnit.Users;
using NHibernate.Criterion;

namespace DataAccess.Models.Dao
{
    public class NotifikaceDao : DaoBase<Notifikace>
    {
        public IList<Notifikace> GetByUser(Uzivatel uzivatel, bool seen=true)
        {
            return Session.CreateCriteria<Notifikace>()
                .AddOrder(Order.Desc("Created"))
                .Add(Restrictions.Eq("Seen",seen))
                .Add(Restrictions.Eq("Uzivatel", uzivatel))
                .List<Notifikace>();
        }
        public IList<Notifikace> GetByUser(int uzivatelId, bool seen = true)
        {
            return Session.CreateCriteria<Notifikace>()
                .AddOrder(Order.Desc("Created"))
                .Add(Restrictions.Eq("Seen", seen))
                .Add(Restrictions.Eq("Uzivatel", new UzivatelDao().GetById(uzivatelId)))
                .List<Notifikace>();
        }
    }
}
