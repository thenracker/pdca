using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Models.DataUnit;
using DataAccess.Models.DataUnit.Users;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;


namespace DataAccess.Models.Dao
{
    public class ReportDao : DaoBase<Object> //nebude používat DaoBase
    {

        public enum StavUkolu
        {
            Vsechny, Vyresene, Nevyresene, CekajiciNaSchvaleni, PoDeadlinu
        }
        public enum Tabulka
        {
            Lop, UkolVedeni, UkolOddeleni, UkolVzorkovani    
        }

        public IList<UsekOddeleniWithCount> GetLopCounts(StavUkolu su, Tabulka ta)
        {
            string table = "";
            string whereClause = " WHERE " + ta + ".Deleted = 0 ";

            switch (su)
            {
                //default:
                case StavUkolu.Vsechny:
                    break;
                case StavUkolu.Vyresene:
                    whereClause += " AND " + ta + ".FinishDate is not null ";
                    break;
                case StavUkolu.Nevyresene:
                    whereClause += " AND " + ta + ".FinishDate is null ";
                    break;
                case StavUkolu.CekajiciNaSchvaleni:
                    whereClause += " AND " + ta + ".CloseDate is not null AND " + ta + ".FinishDate is null";
                    break;
                case StavUkolu.PoDeadlinu:
                    whereClause += " AND " + ta + ".FinishDate is null AND " + ta + ".PlannedCloseDate < NOW() "; 
                    break;
            }

            string sql = "SELECT Usek.Nazev AS Usek, Oddeleni.Nazev AS Oddeleni, COUNT(" + ta + ".Id) AS Pocet " +
                         " FROM Usek " +
                         " LEFT OUTER JOIN Oddeleni ON Oddeleni.Usek = Usek.Id " +
                         " LEFT OUTER JOIN Uzivatel ON Uzivatel.Oddeleni = Oddeleni.Id " +
                         " LEFT OUTER JOIN " + ta + " ON " + ta + ".Resitel = Uzivatel.Id " +
                         whereClause +
                         " GROUP BY Usek.Nazev, Oddeleni.Nazev";

            IQuery query = Session.CreateSQLQuery(sql)
                .SetResultTransformer(Transformers.AliasToBean<UsekOddeleniWithCount>());

            return query.List<UsekOddeleniWithCount>();
            
        }

        public class UsekOddeleniWithCount
        {
            public string Usek { get; set; }
            public string Oddeleni { get; set; }
            public int Pocet { get; set; }
        }


        /*
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
                
                //.Add(Restrictions.Disjunction()
                //    .Add(Restrictions.Eq("subUkoly.Deleted", true)) //nesmazané subúkoly
                //    .Add(Restrictions.IsNull("subUkoly.Deleted")) //nesmazané subúkoly
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
    */
    }
}
