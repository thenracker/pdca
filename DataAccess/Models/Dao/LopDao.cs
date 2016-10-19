using System;
using System.Collections.Generic;
using DataAccess.Models.DataUnit;
using DataAccess.Models.DataUnit.Users;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;


namespace DataAccess.Models.Dao
{
    public class LopDao : DaoBase<Lop>
    {

        public new IList<Lop> GetAll(Uzivatel user) //POUŽÍVAT POUZE PRO LOP, SUBUKOL, UKOLVEDENI, UKOLODDELENI, UKOLVZORKOVANI
        {
            return Session.CreateCriteria<Lop>()
                
                .CreateAlias("Zadavatel", "zadavatel")
                .CreateAlias("Resitel", "resitel")

                .CreateAlias("SubUkoly", "subUkoly",NHibernate.SqlCommand.JoinType.LeftOuterJoin)
                .CreateAlias("SubUkoly.Zadavatel", "suZadavatel", NHibernate.SqlCommand.JoinType.LeftOuterJoin)
                .CreateAlias("SubUkoly.Resitel", "suResitel", NHibernate.SqlCommand.JoinType.LeftOuterJoin)

                //.SetProjection(Projections.Distinct(Projections.ProjectionList())) //DISTINCT ??

                .Add(Restrictions.Eq("Deleted", false)) //nesmazané

                .Add(Restrictions.Disjunction()
                    .Add(Restrictions.Eq("subUkoly.Deleted", false)) //nesmazané subúkoly
                    .Add(Restrictions.IsNull("subUkoly.Deleted")) //nesmazané subúkoly
                )
                

                .Add(

                    Restrictions.Disjunction()//začátek disjunkce

                    .Add(Restrictions.Eq("Zadavatel", user)).Add(Restrictions.Eq("Resitel", user)) //jde o toho uzivatele
                    .Add(Restrictions.Eq("zadavatel.Oddeleni", user.Oddeleni)).Add(Restrictions.Eq("resitel.Oddeleni", user.Oddeleni)) //jde o stejné oddělení

                    .Add(Restrictions.Eq("subUkoly.Zadavatel", user)).Add(Restrictions.Eq("subUkoly.Resitel", user)) //jde o stejné oddělení subúkolu
                    .Add(Restrictions.Eq("suZadavatel.Oddeleni", user.Oddeleni)).Add(Restrictions.Eq("suResitel.Oddeleni", user.Oddeleni)) //jde o stejné oddělení subúkolu


                ) //konec disjunkce

                .SetResultTransformer(new DistinctRootEntityResultTransformer()) //DISTINCT .. tohle jediný na tom bylo lehký..

                .List<Lop>();
        }

        public new IList<Lop> GetOnlyDeleted(Uzivatel user) //POUŽÍVAT POUZE PRO LOP, SUBUKOL, UKOLVEDENI, UKOLODDELENI, UKOLVZORKOVANI
        {
            return Session.CreateCriteria<Lop>()

                .CreateAlias("Zadavatel", "zadavatel")
                .CreateAlias("Resitel", "resitel")

                .CreateAlias("SubUkoly", "subUkoly", NHibernate.SqlCommand.JoinType.LeftOuterJoin)
                .CreateAlias("SubUkoly.Zadavatel", "suZadavatel", NHibernate.SqlCommand.JoinType.LeftOuterJoin)
                .CreateAlias("SubUkoly.Resitel", "suResitel", NHibernate.SqlCommand.JoinType.LeftOuterJoin)

                //.SetProjection(Projections.Distinct(Projections.ProjectionList())) //DISTINCT ??

                .Add(Restrictions.Eq("Deleted", true)) //SMAZANÉ
                /*
                .Add(Restrictions.Disjunction()
                    .Add(Restrictions.Eq("subUkoly.Deleted", true)) //nesmazané subúkoly
                    .Add(Restrictions.IsNull("subUkoly.Deleted")) //nesmazané subúkoly
                )
                */

                .Add(

                    Restrictions.Disjunction()//začátek disjunkce

                    .Add(Restrictions.Eq("Zadavatel", user)).Add(Restrictions.Eq("Resitel", user)) //jde o toho uzivatele
                    .Add(Restrictions.Eq("zadavatel.Oddeleni", user.Oddeleni)).Add(Restrictions.Eq("resitel.Oddeleni", user.Oddeleni)) //jde o stejné oddělení

                    .Add(Restrictions.Eq("subUkoly.Zadavatel", user)).Add(Restrictions.Eq("subUkoly.Resitel", user)) //jde o stejné oddělení subúkolu
                    .Add(Restrictions.Eq("suZadavatel.Oddeleni", user.Oddeleni)).Add(Restrictions.Eq("suResitel.Oddeleni", user.Oddeleni)) //jde o stejné oddělení subúkolu


                ) //konec disjunkce

                .SetResultTransformer(new DistinctRootEntityResultTransformer()) //DISTINCT .. tohle jediný na tom bylo lehký..

                .List<Lop>();
        }
        

        public IList<Lop> GetAllNonDelete()
        {
            return Session.CreateCriteria<Lop>().Add(Restrictions.Eq("Deleted", false)).List<Lop>();
        } 

        public Lop GetLatestLopThisYear()
        {
            return Session.CreateCriteria<Lop>().SetMaxResults(1)
                .AddOrder(Order.Desc("StartDate"))
                .Add(Restrictions.Eq(Projections.SqlFunction("year", NHibernateUtil.DateTime, Projections.Property("StartDate")), DateTime.Today.Year))
                .UniqueResult<Lop>();
        }
    }
    public partial class LopHistoryDao : DaoBase<LopHistory>
    {
    }
    public partial class SubUkolDao : DaoBase<SubUkol>
    {
    }
    public class SubUkolHistoryDao : DaoBase<SubUkolHistory>
    {
    }
}
