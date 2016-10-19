using System.Collections.Generic;
using DataAccess.Models.DataUnit.Users;
using DataAccess.Models.Interface;
using NHibernate;
using NHibernate.Criterion;

namespace DataAccess.Models.Dao
{
    public class DaoBase<T> : IDaoBase<T> where T : class
    {
        protected ISession Session;
        protected DaoBase()
        {
            Session = NHibernateHelper.Session;
        }
        public IList<T> GetAll()
        {
            return Session.QueryOver<T>().List<T>();
        }

        public IList<T> GetAll(Uzivatel user) //POUŽÍVAT POUZE PRO LOP, SUBUKOL, UKOLVEDENI, UKOLODDELENI, UKOLVZORKOVANI
        {
            return Session.CreateCriteria<T>()

                .CreateAlias("Zadavatel", "zadavatel")
                .CreateAlias("Resitel", "resitel")


                .Add(Restrictions.Eq("Deleted", false)) //nesmazané

                .Add(Restrictions.Disjunction()//začátek disjunkce

                .Add(Restrictions.Eq("Zadavatel", user)).Add(Restrictions.Eq("Resitel", user)) //jde o toho uzivatele
                .Add(Restrictions.Eq("zadavatel.Oddeleni", user.Oddeleni)).Add(Restrictions.Eq("resitel.Oddeleni", user.Oddeleni)) //jde o stejné oddělení


                ) //konec disjunkce
                .List<T>();
        }


        public object Create(T entity)
        {
            object o;
            using (ITransaction transaction = Session.BeginTransaction())
            {
                o = Session.Save(entity);
                transaction.Commit();
            }
            return o;
        }

        public void Update(T entity)
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                Session.Update(entity);
                transaction.Commit();
            }
        }

        public void Delete(T entity)
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                Session.Delete(entity);
                transaction.Commit();
            }
        }

        public T GetById(int id)
        {
            return Session.CreateCriteria<T>().Add(Restrictions.Eq("Id", id)).UniqueResult<T>();
        }
    }
}
