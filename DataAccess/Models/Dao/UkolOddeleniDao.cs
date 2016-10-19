using System;
using System.Collections.Generic;
using DataAccess.Models.DataUnit;
using NHibernate;
using NHibernate.Criterion;

namespace DataAccess.Models.Dao
{
    public partial class UkolOddeleniDao : DaoBase<UkolOddeleni>
    {
        public UkolOddeleni GetLatestLopThisYear()
        {
            return Session.CreateCriteria<UkolOddeleni>().SetMaxResults(1)
                .AddOrder(Order.Desc("DateStart"))
                .Add(Restrictions.Eq(Projections.SqlFunction("year", NHibernateUtil.DateTime, Projections.Property("DateStart")), DateTime.Today.Year))
                .UniqueResult<UkolOddeleni>();
        }

        public IList<UkolOddeleni> GetAllNonDelete()
        {
            return Session.CreateCriteria<UkolOddeleni>().Add(Restrictions.Eq("Deleted", false)).List<UkolOddeleni>();
        }
    }
}
