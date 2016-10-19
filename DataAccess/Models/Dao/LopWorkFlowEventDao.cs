using System.Collections.Generic;
using DataAccess.Models.DataUnit;
using NHibernate.Criterion;

namespace DataAccess.Models.Dao
{
    public class LopWorkFlowEventDao : DaoBase<LopWorkFlowEvent>
    {
        public IList<LopWorkFlowEvent> GetByLop(Lop lop)
        {
            return Session.CreateCriteria<LopWorkFlowEvent>()
                .Add(Restrictions.Eq("Lop", lop))
                .AddOrder(Order.Desc("Datum"))
                .List<LopWorkFlowEvent>();
        } 
    }
}
