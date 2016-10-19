using System;
using System.Collections.Generic;
using DataAccess.Models.DataUnit;
using NHibernate;
using NHibernate.Criterion;

namespace DataAccess.Models.Dao
{
    public partial class SubUkolDao : DaoBase<SubUkol>
    {
        public IList<SubUkol> GetAllNonDelete()
        {
            return Session.CreateCriteria<SubUkol>().Add(Restrictions.Eq("Deleted", false)).List<SubUkol>();
        }

        public IList<SubUkol> GetAllByLop(Lop lop)
        {
            return Session.CreateCriteria<SubUkol>().Add(Restrictions.Eq("Lop", lop)).List<SubUkol>();
        }
        public IList<SubUkol> GetAllByLop(int lopId)
        {
            return Session.CreateCriteria<SubUkol>().Add(Restrictions.Eq("Lop.Id", lopId)).List<SubUkol>();
        }



        public SubUkol GetLatestSubUkolThisYear()
        {
            
            return Session.CreateCriteria<SubUkol>().SetMaxResults(1)
                .AddOrder(Order.Desc("StartDate"))
                .Add(Restrictions.Eq(Projections.SqlFunction("year", NHibernateUtil.DateTime, Projections.Property("StartDate")), DateTime.Today.Year))
                .Add(Restrictions.Eq("Deleted", false))
                .UniqueResult<SubUkol>();
                

            //return Session.CreateSQLQuery("SELECT TOP(1) * FROM SubUkol WHERE YEAR(StartDate) = YEAR(GETDATE())").AddEntity(typeof(SubUkol)).UniqueResult<SubUkol>();
        }
        
    }
}
