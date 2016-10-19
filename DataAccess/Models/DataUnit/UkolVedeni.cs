using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using DataAccess.Models.DataUnit.Users;
using DataAccess.Models.Interface;

namespace DataAccess.Models.DataUnit
{
    public class UkolVedeni : IEntity
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
        public virtual IList<UkolVedeniMaterial> Materialy { get; set; }
        [Required]
        [StringLength(50)]
        public virtual string Nazev { get; set; }
        [AllowHtml]
        public virtual string Popis { get; set; }
        [AllowHtml]
        public virtual string Komentar { get; set; }
        public virtual string DeniedMessage { get; set; }
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
        public virtual Dilna Dilna { get; set; }
        public virtual IList<UkolVedeniHistory> History { get; set; }

        public virtual IList<string> UpraveneHodnotyVHistorii(int kolikataZHistorie)
        {
            IList<string> historieZmen = new List<string>();

            //první historie se porovnává s nynějším objektem - bohužel nejde jen tak přetipovat - takže se takhle vytvoří nová "historie"
            UkolVedeniHistory thisAsHistory = new UkolVedeniHistory(this)
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
                ,
                Dilna = this.Dilna
                ,
                DeniedMessage = this.DeniedMessage
                ,
                Poznamka = this.Poznamka

            };

            //kopie materiálu trochu těžkopádně
            IList<UkolVedeniHistoryMaterial> matrose = new List<UkolVedeniHistoryMaterial>();
            for (int i = 0; i < Materialy.Count; i++)
            {
                UkolVedeniHistoryMaterial lhm = new UkolVedeniHistoryMaterial() { DateAdded = Materialy[i].DateAdded, Id = Materialy[i].Id, Produkt = Materialy[i].Produkt };
                lhm.UkolVedeniHistory = new UkolVedeniHistory();
                lhm.UkolVedeniHistory.Id = Materialy[i].Ukol.Id;
                //matrose[i] = lhm; //stačí jen ID??? TODO
                matrose.Add(lhm);
            }
            thisAsHistory.Materialy = matrose;

            //není naplněn Material na Historii -_-
            UkolVedeniHistory lastHist = History[0]; //a mrkneme se na první záznam historie

            if (lastHist != null) //pokud alespoň nějaká je, zapíšeme ji do seznamu
            {
                if (kolikataZHistorie == 0)
                {
                    IEnumerable<string> zmeny = GetRozdilyString(thisAsHistory, lastHist);
                    foreach (string s in zmeny)
                    {
                        historieZmen.Add(s); //PRVNÍ ZMĚNY   
                    }
                }
                else
                {
                    IEnumerable<string> zmeny = GetRozdilyString(History[kolikataZHistorie - 1], History[kolikataZHistorie]);
                    foreach (string s in zmeny)
                    {
                        historieZmen.Add(s); //OSTATNÍ ZMĚNY   
                    }
                }
            }
            return historieZmen; //vrací seznam dvojic stringů (vždy CO, HODNOTA) se změněnými hodnotami
        }

        private IList<String> GetRozdilyString(UkolVedeniHistory newer, UkolVedeniHistory older)
        {

            var whiteSpace = "-";
            //MYŠLENKA: - zapíšeme do pole vždycky dvojici (ATRIBUT,HODNOTA .... ATRIBUT,HODNOTA .... 
            //Z toho potom hezky vytáhneme která hodnota se změnila a jaká byla původní hodnota před změnou ;)

            IList<string> zmeny = new List<string>();

            if (newer.Action != older.Action) { zmeny.Add("Action"); zmeny.Add(older.Action.ToString()); }
            if (newer.Status != older.Status) { zmeny.Add("Status"); zmeny.Add(older.Status.ToString()); }
            if (newer.DateStart != older.DateStart) { zmeny.Add("StartDate"); zmeny.Add(older.DateStart.ToString(CultureInfo.CurrentCulture)); }
            if (newer.DatePlannedClose != older.DatePlannedClose) { zmeny.Add("PlannedCloseDate"); zmeny.Add(older.DatePlannedClose == null ? whiteSpace : older.DatePlannedClose.ToShortDateString()); }
            if (newer.DateDeadline != older.DateDeadline) { zmeny.Add("CloseDate"); zmeny.Add(older.DateDeadline == null ? whiteSpace : older.DateDeadline.GetValueOrDefault().ToShortDateString()); }
            if (newer.DateFinish != older.DateFinish) { zmeny.Add("FinishDate"); zmeny.Add(older.DateFinish == null ? whiteSpace : older.DateFinish.GetValueOrDefault().ToShortDateString()); }
            if (newer.DateCheck != older.DateCheck) { zmeny.Add("CheckDate"); zmeny.Add(older.DateCheck == null ? whiteSpace : older.DateCheck.GetValueOrDefault().ToShortDateString()); }
            //if (newer.LastChangedDate != older.LastChangedDate)    { zmeny.Add("LastChangedDate");   zmeny.Add(older.LastChangedDate.ToShortDateString());     }
            if (newer.Zadavatel != older.Zadavatel) { zmeny.Add("Zadavatel"); zmeny.Add(older.Zadavatel.Prijmeni + " " + Zadavatel.Jmeno); }
            if (newer.Resitel != older.Resitel) { zmeny.Add("Resitel"); zmeny.Add(older.Resitel.Prijmeni + " " + Resitel.Jmeno); }
            if (newer.Nazev != older.Nazev) { zmeny.Add("Nazev"); zmeny.Add(older.Nazev); }
            if (newer.Popis != older.Popis) { zmeny.Add("Popis"); zmeny.Add("Pro zobrazení popisu otevřete detail historie"); }
            //if (newer.Komentar != older.Komentar&&older.Komentar!=null){ zmeny.Add("Komentar");          zmeny.Add(older.Komentar);                                 }
            if (newer.LessonLearned != older.LessonLearned) { zmeny.Add("LessonLearned"); zmeny.Add(older.LessonLearned.ToString()); }
            if (newer.Dilna != older.Dilna) { zmeny.Add("Dilna"); zmeny.Add(older.Dilna.ToString()); }
            if (newer.Poznamka != older.Poznamka) { zmeny.Add("Poznamka"); zmeny.Add(older.Poznamka); }

            if (newer.Materialy.Count != older.Materialy.Count)
            {
                zmeny.Add("Produkty");
                if (older.Materialy.Count != 0)
                    zmeny.Add("Změna v počtu z " + older.Materialy.Count + " na " + newer.Materialy.Count);
                else
                    zmeny.Add("Byly přidány " + newer.Materialy.Count + " produkty");
            }
            else
            {
                string produkty = "";
                for (int i = 0; i < newer.Materialy.Count; i++)
                {
                    if (newer.Materialy[i].Produkt.Id != older.Materialy[i].Produkt.Id)
                        produkty += older.Materialy[i].Produkt.Druh + ", ";
                }
                if (produkty.Length > 2)
                {
                    zmeny.Add("Produkty");
                    zmeny.Add(produkty.Substring(0, produkty.Length - 2));
                }
            }


            return zmeny;
        }

        public virtual bool IsSame(UkolVedeni history)
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

    public class UkolVedeniHistory : IEntity
    {
        public virtual int Id { get; set; }
        public virtual UkolVedeni UkolVedeni { get; set; }
        public virtual int Action { get; set; }
        public virtual StatusUkolu Status { get; set; }
        public virtual IList<UkolVedeniHistoryMaterial> Materialy { get; set; }
        public virtual Uzivatel Zadavatel { get; set; }
        public virtual string Nazev { get; set; }
        [AllowHtml]
        public virtual string Popis { get; set; }
        public virtual string Komentar { get; set; }
        public virtual string DeniedMessage { get; set; }
        public virtual Uzivatel Resitel { get; set; }
        public virtual DateTime DateStart { get; set; } //automaticky - 
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
        public virtual Dilna Dilna { get; set; }

        public UkolVedeniHistory()
        {
            
        }
        public UkolVedeniHistory(UkolVedeni arg)
        {
            UkolVedeni = arg;
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
