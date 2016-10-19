using System;
using System.Collections.Generic;
using DataAccess.Models.DataUnit;
using NHibernate;
using NHibernate.Criterion;

namespace DataAccess.Models.Dao
{
    public partial class UkolVzorkovaniDao : DaoBase<UkolVzorkovani>
    {
        public UkolVzorkovani GetLatestLopThisYear()
        {
            return Session.CreateCriteria<UkolVzorkovani>().SetMaxResults(1)
                .AddOrder(Order.Desc("DateStart"))
                .Add(Restrictions.Eq(Projections.SqlFunction("year", NHibernateUtil.DateTime, Projections.Property("DateStart")), DateTime.Today.Year))
                .UniqueResult<UkolVzorkovani>();
        }

        public IList<UkolVzorkovani> GetAllNonDelete()
        {
            return Session.CreateCriteria<UkolVzorkovani>().Add(Restrictions.Eq("Deleted", false)).List<UkolVzorkovani>();
        }
    }
}
