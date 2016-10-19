using System;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;

namespace DataAccess.Models
{
    public class NHibernateHelper
    {
        private static ISessionFactory _factory;

        public static ISession Session
        {
            get
            {
                if (_factory != null) return _factory.OpenSession();
                try
                {
#if DEBUG
                    var conn = "Data Source=JARA-PC\\SQLEXPRESS;Initial Catalog=PDCAList;Integrated Security=True";

                    conn = "Data Source=PETR-NB\\SQLEXPRESS;Initial Catalog=PDCAList;Integrated Security=True";
                    //conn = "Data Source=GLEVA_PC\\SQLEXPRESS;Initial Catalog=PDCAList;Integrated Security=True";
#else
                    const string conn = "Data Source=SERVER10\\SQLEXPRESS;Initial Catalog=PDCAList;Persist Security Info=True;User ID=sa;Password=Swotes9";
#endif
                    var cfg = new Configuration().DataBaseIntegration(db =>
                    {
                        db.ConnectionString = conn;
                        db.Dialect<MsSql2008Dialect>();
                        db.Driver<SqlClientDriver>();
                    });
                    cfg.AddAssembly("DataAccess");
                    _factory = cfg.BuildSessionFactory();
                }
                catch(Exception e)
                {
                    // ignored
                }
                return _factory.OpenSession();
            }
        }
    }
}
