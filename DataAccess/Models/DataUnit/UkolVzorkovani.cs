using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using DataAccess.Models.Dao;
using DataAccess.Models.DataUnit.Users;
using DataAccess.Models.Interface;

namespace DataAccess.Models.DataUnit
{
    public class UkolVzorkovani : IEntity
    {
        public virtual int Id { get; set; }
        public virtual int Action { get; set; }
        public virtual StatusUkolu Status { get; set; }
        public virtual Uzivatel Zadavatel { get; set; }

        public virtual string MaterialyInput
        {
            get
            {
                return Materialy == null ? "" : Materialy.Aggregate("", (current, material) => current + (";" + material.Produkt.Id + ";"));
            }
        }
        public virtual IList<UkolVzorkovaniMaterial> Materialy { get; set; }
        public virtual Material Produkt { get; set; } //rolovací menu + ve formuláři pole pro zobrazení názvu
        [Required]
        [StringLength(50)]
        public virtual string Nazev { get; set; }
        [AllowHtml]
        public virtual string Popis { get; set; }
        [AllowHtml]
        public virtual string Komentar { get; set; }
        public virtual Uzivatel Resitel { get; set; }
        public virtual DateTime DateStart { get; set; } //automaticky
        public virtual DateTime DatePlannedClose { get; set; } //ručně zadavatel
        public virtual DateTime? DateDeadline { get; set; } //ručně zadavatel
        public virtual string Poznamka { get; set; } //stanovisko
        
        public virtual DateTime? DateFinish { get; set; } //zadává řešitel - kdy to udělal
        public virtual string DateCheckInput
        {
            get
            {
                return DateCheck == null ? "" : DateCheck.GetValueOrDefault().ToShortDateString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    DateCheck = Convert.ToDateTime(value);
            }
        }
        public virtual DateTime? DateCheck { get; set; }
        public virtual DateTime DateLastChanged { get; set; } //automaticky
        public virtual bool LessonLearned { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual IList<UkolVzorkovaniHistory> History { get; set; }
        public virtual IList<string> UpraveneHodnotyVHistorii(int kolikataZHistorie)
        {
            IList<string> historieZmen = new List<string>();

            //první historie se porovnává s nynějším objektem - bohužel nejde jen tak přetipovat - takže se takhle vytvoří nová "historie"
            UkolVzorkovaniHistory thisAsHistory = new UkolVzorkovaniHistory(this)
            {
                Action = this.Action
                ,
                Status = this.Status
                ,
                DateStart = this.DateStart
                ,
                DatePlannedClose = this.DatePlannedClose
                ,
                DateDeadline = this.DateDeadline
                ,
                DateFinish = this.DateFinish
                ,
                //LastChangedDate = this.LastChangedDate
                //,
                Zadavatel = this.Zadavatel
                ,
                Resitel = this.Resitel
                ,
                Nazev = this.Nazev
                ,
                Popis = this.Popis
                ,
                LessonLearned = this.LessonLearned

            };

            UkolVzorkovaniHistory lastHist = History[0]; //a mrkneme se na první záznam historie

            if (lastHist != null) //pokud alespoň nějaká je, zapíšeme ji do seznamu
            {

                if (kolikataZHistorie == 0)
                {
                    IList<string> zmeny = GetRozdilyString(thisAsHistory, lastHist);
                    foreach (string s in zmeny)
                    {
                        historieZmen.Add(s); //PRVNÍ ZMĚNY   
                    }
                }
                else
                {
                    IList<string> zmeny = GetRozdilyString(History[kolikataZHistorie-1], History[kolikataZHistorie]);
                    foreach (string s in zmeny)
                    {
                        historieZmen.Add(s); //OSTATNÍ ZMĚNY   
                    }
                }
            }

            return historieZmen; //vrací seznam dvojic stringů (vždy CO, HODNOTA) se změněnými hodnotami
        }

        private IList<String> GetRozdilyString(UkolVzorkovaniHistory newer, UkolVzorkovaniHistory older)
        {

            //MYŠLENKA: - zapíšeme do pole vždycky dvojici (ATRIBUT,HODNOTA .... ATRIBUT,HODNOTA .... 
            //Z toho potom hezky vytáhneme která hodnota se změnila a jaká byla původní hodnota před změnou ;)

            IList<String> zmeny = new List<string>();

            if (newer.Action != older.Action) { zmeny.Add("Action"); zmeny.Add(newer.Action.ToString()); }
            if (newer.Status != older.Status) { zmeny.Add("Status"); zmeny.Add(newer.Status.ToString()); }
            if (newer.DateStart != older.DateStart) { zmeny.Add("DateStart"); zmeny.Add(newer.DateStart.ToString()); }
            if (newer.DatePlannedClose != older.DatePlannedClose) { zmeny.Add("DatePlannedClose"); zmeny.Add(newer.DatePlannedClose.ToString()); }
            if (newer.DateDeadline != older.DateDeadline) { zmeny.Add("DateDeadline"); zmeny.Add(newer.DateDeadline.ToString()); }
            if (newer.DateFinish != older.DateFinish) { zmeny.Add("DateFinish"); zmeny.Add(newer.DateFinish.ToString()); }
            if (newer.DateCheck != older.DateCheck) { zmeny.Add("DateCheck"); zmeny.Add(newer.DateCheck.ToString()); }
            if (newer.Zadavatel != older.Zadavatel) { zmeny.Add("Zadavatel"); zmeny.Add(newer.Zadavatel.Prijmeni + " " + Zadavatel.Jmeno); }
            if (newer.Resitel != older.Resitel) { zmeny.Add("Resitel"); zmeny.Add(newer.Resitel.Prijmeni + " " + Resitel.Jmeno); }
            if (newer.Nazev != older.Nazev) { zmeny.Add("Nazev"); zmeny.Add(newer.Nazev); }
            if (newer.Popis != older.Popis) { zmeny.Add("Popis"); zmeny.Add(newer.Popis); }
            //if (newer.Komentar != older.Komentar && older.Komentar != null) { zmeny.Add("Komentar"); zmeny.Add(older.Komentar); }
            if (newer.Poznamka != older.Poznamka) { zmeny.Add("Poznamka"); zmeny.Add(newer.Poznamka); }
            if (newer.LessonLearned != older.LessonLearned) { zmeny.Add("LessonLearned"); zmeny.Add(newer.LessonLearned.ToString()); }
            if (newer.Deleted != older.Deleted) { zmeny.Add("Deleted"); zmeny.Add(newer.Deleted.ToString()); }


            return zmeny;
        }

        public virtual bool IsSame(UkolVzorkovani history)
        {
            return Action == history.Action
                   && DateDeadline == history.DateDeadline
                   && DateFinish == history.DateFinish
                   && DateCheck == history.DateCheck
                   //&& LastChangedDate == history.LastChangedDate //nelogické - plníme jen při ukládání
                   && Nazev == history.Nazev
                   && DatePlannedClose == history.DatePlannedClose
                   && Popis == history.Popis
                   && Resitel.Id == history.Resitel.Id
                   && DateStart == history.DateStart //nesedí ?
                   && Status == history.Status
                   && Zadavatel.Id == history.Zadavatel.Id //TODO do budoucna přepsat z ID na celé objekty??
                   && Poznamka == history.Poznamka
                   && LessonLearned == history.LessonLearned
                   && Deleted == history.Deleted;
        }

        public virtual bool ShouldBeAlreadyDone()
        {
            return (DateTime.Compare((DateTime)DatePlannedClose, DateTime.Now) < 0 && Status != StatusUkolu.Closed);
        }

    }

    public class UkolVzorkovaniHistory : IEntity
    {
        public virtual int Id { get; set; }
        public virtual UkolVzorkovani UkolVzorkovani { get; set; }
        public virtual int Action { get; set; }
        public virtual StatusUkolu Status { get; set; }
        public virtual Uzivatel Zadavatel { get; set; }
        public virtual Material Produkt { get; set; } //rolovací menu + ve formuláři pole pro zobrazení názvu
        public virtual string Nazev { get; set; }
        [AllowHtml]
        public virtual string Popis { get; set; }
        [AllowHtml]
        public virtual string Komentar { get; set; }
        public virtual Uzivatel Resitel { get; set; }
        public virtual DateTime DateStart { get; set; } //automaticky
        public virtual DateTime DatePlannedClose { get; set; } //ručně zadavatel
        public virtual DateTime? DateDeadline { get; set; } //ručně zadavatel
        public virtual string Poznamka { get; set; } //stanovisko
        public virtual DateTime? DateFinish { get; set; } //zadává řešitel - kdy to udělal
        public virtual string DateCheckInput
        {
            get
            {
                return DateCheck == null ? "" : DateCheck.GetValueOrDefault().ToShortDateString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    DateCheck = Convert.ToDateTime(value);
            }
        }
        public virtual DateTime? DateCheck { get; set; }
        public virtual DateTime DateLastChanged { get; set; } //automaticky
        public virtual bool LessonLearned { get; set; }
        public virtual bool Deleted { get; set; }

        public UkolVzorkovaniHistory()
        {

        }
        public UkolVzorkovaniHistory(UkolVzorkovani arg)
        {
            UkolVzorkovani = arg;
            Action = arg.Action;
            Status = arg.Status;
            DateStart = arg.DateStart;
            DatePlannedClose = arg.DatePlannedClose;
            DateFinish = arg.DateFinish;
            DateCheck = arg.DateCheck;
            DateDeadline = arg.DateDeadline;
            Zadavatel = arg.Zadavatel;
            Resitel = arg.Resitel;
            Nazev = arg.Nazev;
            Popis = arg.Popis;
            Komentar = arg.Komentar;
            Poznamka = arg.Poznamka;
            DateLastChanged = arg.DateLastChanged;
            LessonLearned = arg.LessonLearned;
            Deleted = arg.Deleted;
        }
    }
}
