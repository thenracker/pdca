using System.Collections.Generic;
using DataAccess.Models.DataUnit.Users;
using NHibernate.Criterion;

namespace DataAccess.Models.Dao
{
    public class CustomTableSettingDao : DaoBase<CustomTableSetting>
    {
        public IList<CustomTableSetting> GetByUser(Uzivatel u)
        {
            return Session.CreateCriteria<CustomTableSetting>()
               .Add(Restrictions.Eq("Uzivatel",u))
               .List<CustomTableSetting>();
        }

        public CustomTableSetting Get(Uzivatel u, string tableName, string collumn)
        {
            return Session.CreateCriteria<CustomTableSetting>()
               .Add(Restrictions.Eq("Uzivatel", u))
               .Add(Restrictions.Eq("Tabulka", tableName))
               .Add(Restrictions.Eq("Sloupec", collumn))
               .UniqueResult<CustomTableSetting>();
        }
    }
}
