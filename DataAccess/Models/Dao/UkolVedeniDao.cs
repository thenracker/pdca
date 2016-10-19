using System;
using System.Collections.Generic;
using DataAccess.Models.DataUnit;
using DataAccess.Models.DataUnit.Users;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace DataAccess.Models.Dao
{
    public partial class UkolVedeniDao : DaoBase<UkolVedeni>
    {
        public UkolVedeni GetLatestLopThisYear()
        {
            return Session.CreateCriteria<UkolVedeni>().SetMaxResults(1)
                .AddOrder(Order.Desc("DateStart"))
                .Add(Restrictions.Eq(Projections.SqlFunction("year", NHibernateUtil.DateTime, Projections.Property("DateStart")), DateTime.Today.Year))
                .UniqueResult<UkolVedeni>();
        }

        public IList<UkolVedeni> GetAllNonDelete()
        {
            return Session.CreateCriteria<UkolVedeni>().Add(Restrictions.Eq("Deleted", false)).List<UkolVedeni>();
        }


        public new IList<UkolVedeni> GetOnlyDeleted(Uzivatel user) //POUŽÍVAT POUZE PRO LOP, SUBUKOL, UKOLVEDENI, UKOLODDELENI, UKOLVZORKOVANI
        {
            return Session.CreateCriteria<UkolVedeni>()

                .CreateAlias("Zadavatel", "zadavatel")
                .CreateAlias("Resitel", "resitel")
                
                //.SetProjection(Projections.Distinct(Projections.ProjectionList())) //DISTINCT ??

                .Add(Restrictions.Eq("Deleted", true)) //smazané
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

                .List<UkolVedeni>();
        }
    }
}
