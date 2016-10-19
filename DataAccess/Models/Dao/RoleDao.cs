using DataAccess.Models.DataUnit.Users;
using NHibernate.Criterion;

namespace DataAccess.Models.Dao
{
    public class RoleDao : DaoBase<Role>
    {
        public Role GetByIdentificator(string identificator)
        {
            return Session.CreateCriteria<Role>()
                .Add(Restrictions.Eq("Identificator", identificator))
                .UniqueResult<Role>();
        }
    }
}
