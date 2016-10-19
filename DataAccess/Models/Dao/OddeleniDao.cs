using System.Collections.Generic;
using DataAccess.Models.DataUnit;
using DataAccess.Models.DataUnit.Users;
using NHibernate.Criterion;

namespace DataAccess.Models.Dao
{
    public class OddeleniDao : DaoBase<Oddeleni>
    {
        public IList<Oddeleni> GetAllByUsek(Usek usek)
        {
            return Session.CreateCriteria<Oddeleni>().Add(Restrictions.Eq("Usek", usek)).List<Oddeleni>();
        }

        public IList<Oddeleni> GetAllNotInUsek(Usek usek)
        {
            return Session.CreateCriteria<Oddeleni>().Add(Restrictions.Not(Restrictions.Eq("Usek", usek))).List<Oddeleni>();
        }
    }
}
