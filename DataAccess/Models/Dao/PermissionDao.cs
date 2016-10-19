using DataAccess.Models.DataUnit.Users;
using NHibernate.Criterion;

namespace DataAccess.Models.Dao
{
    public class PermissionDao : DaoBase<Permission>
    {
        public Permission GetByUser(string loginId)
        {
            return Session.CreateCriteria<Permission>()
                .Add(Restrictions.Eq("WindowsId", loginId))
                .UniqueResult<Permission>();
        }

        public Permission GetByUser(Uzivatel arg)
        {
            return Session.CreateCriteria<Permission>()
                .Add(Restrictions.Eq("WindowsId", arg.WindowsId))
                .UniqueResult<Permission>();
        }
    }
}
