using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using DataAccess.Models.Dao;
using DataAccess.Models.DataUnit.Users;
using DataAccess.Models.Interface;

namespace DataAccess.Models.DataUnit
{
    public class Lop : IEntity
    {
        public virtual int Id { get; set; }
        public virtual int Action { get; set; }  //od jedničky v kalendářním roce - zpětně se dotázat na poslední číslo letos
        public virtual StatusUkolu Status { get; set; }

        public virtual string MaterialyInput
        {
            get
            {
                return Materialy == null ? "" : Materialy.Aggregate("", (current, lopMaterial) => current + (";" + lopMaterial.Produkt.Id + ";"));
            }
        }
        public virtual IList<LopMaterial> Materialy { get; set; }

        public virtual DateTime StartDate { get; set; } = DateTime.Now; // automaticky
        /*
        public virtual string PlannedCloseDateInput
        {
            get
            {
                return PlannedCloseDate == null ? "" : PlannedCloseDate.GetValueOrDefault().ToShortDateString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    PlannedCloseDate = Convert.ToDateTime(value);
            }
        }
        public virtual DateTime? PlannedCloseDate { get; set; }  // Zadavatel
        */
        public virtual DateTime PlannedCloseDate { get; set; }  // Zadavatel
        public virtual string CloseDateInput
        {
            get
            {
                return CloseDate == null ? "" : CloseDate.GetValueOrDefault().ToShortDateString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    CloseDate = Convert.ToDateTime(value);
            }
        }
        public virtual DateTime? CloseDate { get; set; }         // zadavatel

        public virtual string FinishDateInput
        {
            get
            {
                return FinishDate == null ? "" : FinishDate.GetValueOrDefault().ToShortDateString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    FinishDate = Convert.ToDateTime(value);
            }
        }
        public virtual DateTime? FinishDate { get; set; }    // zadá řešitel
        public virtual string CheckDateInput
        {
            get
            {
                return CheckDate == null ? "" : CheckDate.GetValueOrDefault().ToShortDateString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    CheckDate = Convert.ToDateTime(value);
            }
        }
        public virtual DateTime? CheckDate { get; set; }    // zadá zadavatel
        public virtual DateTime LastChangedDate { get; set; } // automaticky
        public virtual Uzivatel Zadavatel { get; set; }
        public virtual Uzivatel Resitel { get; set; }
        [Required]
        [StringLength(50)]
        public virtual string Nazev { get; set; }

        [AllowHtml]
        [StringLength(5000)]
        public virtual string Popis { get; set; }
        [AllowHtml]
        public virtual string Komentar { get; set; }
        public virtual string DeniedMessage { get; set; }
        public virtual bool Investice { get; set; }
        public virtual bool LessonLearned { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual Dilna Dilna { get; set; }
        public virtual IList<SubUkol> SubUkoly { get; set; }
        public virtual IList<LopHistory> History { get; set; }

        //DALŠÍ DOPLŇUJÍCÍ FUNKCE
        public virtual int PocetSubUkoluCelkem()
        {
            return SubUkoly.Count(s => s.Deleted != true);
        }

        public virtual int PocetSubUkoluCompleted()
        {
            return SubUkoly.Count(s => s.Deleted == false && s.Status == StatusUkolu.Closed && s.FinishDate != null);
        }

        public virtual int PocetSubUkoluKeKontrole()
        {
            return SubUkoly.Count(s => s.Deleted == false && s.Status == StatusUkolu.Closed && s.FinishDate == null);
        }

        public virtual int PocetSubUkoluDeadlined()
        {
            return (from s in SubUkoly where s.Deleted == false let i = DateTime.Compare(s.PlannedCloseDate, DateTime.Now) where i < 0 select s).Count(s => s.Status != StatusUkolu.Closed);
        }

        public virtual bool CouldBeDone() //vrací jestli jsou splněny všechny podúkoly - jakože jestli celkový LOP může být uzavřen (done)
        {
            return (PocetSubUkoluCelkem() == PocetSubUkoluCompleted()); //pokud jsou všechny nesmazané vyřešené
        }

        public virtual bool ShouldBeAlreadyDone()
        {
            return (DateTime.Compare(PlannedCloseDate, DateTime.Now) < 0 && Status != StatusUkolu.Closed);
        }

        public virtual bool IsAtLeastOneDone()
        {
            return SubUkoly.Any(s => s.Deleted == false && s.Status == StatusUkolu.Closed);
        }
        
        public virtual DateTime MaxPlannedCloseDate
        {   // Maximální plánované datum ukončení včetně podúkolů (teda reálné plánované ukončení celého úkolu)
            get
            {
                if (SubUkoly == null)
                    return PlannedCloseDate;
                return SubUkoly.Count == 0 ? 
                    PlannedCloseDate : 
                    (PlannedCloseDate < SubUkoly.Max(x => x.PlannedCloseDate) ? 
                        SubUkoly.Max(x => x.PlannedCloseDate): 
                        PlannedCloseDate);
            }
        }

        public virtual bool IsResitel(Uzivatel u)
        {
            return Resitel.Compare(u) || SubUkoly.Any(subUkol => subUkol.Resitel.Compare(u));
        }
        public virtual bool IsZadavatel(Uzivatel u)
        {
            if (Zadavatel.Compare(u))
                return true;
            if (SubUkoly.Any(x => x.Zadavatel.Compare(u)))
                return true;
            return false;
        }


        private IEnumerable<string> GetRozdilyString(LopHistory newer, LopHistory older)
        {
            var whiteSpace = "-";
            //MYŠLENKA: - zapíšeme do pole vždycky dvojici (ATRIBUT,HODNOTA .... ATRIBUT,HODNOTA .... 
            //Z toho potom hezky vytáhneme která hodnota se změnila a jaká byla původní hodnota před změnou ;)

            IList<string> zmeny = new List<string>();

            if (newer.Action != older.Action)                      { zmeny.Add("Action");            zmeny.Add(older.Action.ToString());                        }
            if (newer.Status != older.Status)                      { zmeny.Add("Status");            zmeny.Add(older.Status.ToString());                        }
            if (newer.StartDate != older.StartDate)                { zmeny.Add("StartDate");         zmeny.Add(older.StartDate.ToString(CultureInfo.CurrentCulture));                     }
            if (newer.PlannedCloseDate != older.PlannedCloseDate)  { zmeny.Add("PlannedCloseDate");  zmeny.Add(older.PlannedCloseDate == null ? whiteSpace : older.PlannedCloseDate.GetValueOrDefault().ToShortDateString());              }
            if (newer.CloseDate != older.CloseDate)                { zmeny.Add("CloseDate");         zmeny.Add(older.CloseDate==null? whiteSpace : older.CloseDate.GetValueOrDefault().ToShortDateString());                     }
            if (newer.FinishDate != older.FinishDate)              { zmeny.Add("FinishDate");        zmeny.Add(older.FinishDate == null ? whiteSpace : older.FinishDate.GetValueOrDefault().ToShortDateString());                    }
            if (newer.CheckDate != older.CheckDate)                { zmeny.Add("CheckDate");         zmeny.Add(older.CheckDate == null ? whiteSpace : older.CheckDate.GetValueOrDefault().ToShortDateString());                     }
            //if (newer.LastChangedDate != older.LastChangedDate)    { zmeny.Add("LastChangedDate");   zmeny.Add(older.LastChangedDate.ToShortDateString());     }
            if (newer.Zadavatel != older.Zadavatel)                { zmeny.Add("Zadavatel");         zmeny.Add(older.Zadavatel.Prijmeni+" "+Zadavatel.Jmeno);   }
            if (newer.Resitel != older.Resitel)                    { zmeny.Add("Resitel");           zmeny.Add(older.Resitel.Prijmeni+" "+Resitel.Jmeno);       }
            if (newer.Nazev != older.Nazev)                        { zmeny.Add("Nazev");             zmeny.Add(older.Nazev);                                    }
            if (newer.Popis != older.Popis)                        { zmeny.Add("Popis");             zmeny.Add("Pro zobrazení popisu otevřete detail historie");                                    }
            //if (newer.Komentar != older.Komentar&&older.Komentar!=null){ zmeny.Add("Komentar");          zmeny.Add(older.Komentar);                                 }
            if (newer.Investice != older.Investice)                { zmeny.Add("Investice");         zmeny.Add(older.Investice.ToString());                     }
            if (newer.LessonLearned != older.LessonLearned)        { zmeny.Add("LessonLearned");     zmeny.Add(older.LessonLearned.ToString());                 }
            if (newer.Dilna != older.Dilna)                        { zmeny.Add("Dilna");             zmeny.Add(older.Dilna.ToString());                         }
            
            if (newer.Materialy.Count != older.Materialy.Count)
            {
                zmeny.Add("Produkty");
                if(older.Materialy.Count != 0)
                    zmeny.Add("Změna v počtu z " + older.Materialy.Count + " na " + newer.Materialy.Count);
                else
                    zmeny.Add("Byly přidány "+newer.Materialy.Count + " produkty");
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

        public virtual IList<string> UpraveneHodnotyVHistorii(int kolikataZHistorie)
        {
            IList<string> historieZmen = new List<string>();
            
            //první historie se porovnává s nynějším objektem - bohužel nejde jen tak přetipovat - takže se takhle vytvoří nová "historie"
            var thisAsHistory = new LopHistory()
            {
                 Action = Action
                ,Status = Status
                //,Materialy = Materialy //TODO? 
                ,StartDate = StartDate
                ,PlannedCloseDate = PlannedCloseDate
                ,CloseDate = CloseDate
                ,FinishDate = FinishDate
                ,CheckDate = CheckDate
                ,LastChangedDate = LastChangedDate
                ,Zadavatel = Zadavatel
                ,Resitel = Resitel
                ,Nazev = Nazev
                ,Popis = Popis
                ,Komentar = Komentar
                ,DeniedMessage = DeniedMessage
                ,Investice = Investice
                ,LessonLearned = LessonLearned
                ,Dilna = Dilna
            };
            //kopie materiálu trochu těžkopádně
            IList<LopHistoryMaterial> matrose = new List<LopHistoryMaterial>();
            for (int i = 0; i < Materialy.Count; i++)
            {
                LopHistoryMaterial lhm = new LopHistoryMaterial() { DateAdded = Materialy[i].DateAdded, Id = Materialy[i].Id, Produkt = Materialy[i].Produkt};
                lhm.LopHistory = new LopHistory();
                lhm.LopHistory.Id = Materialy[i].Lop.Id;
                //matrose[i] = lhm; //stačí jen ID??? TODO
                matrose.Add(lhm);
            }
            thisAsHistory.Materialy = matrose;

            //není naplněn Material na Historii -_-
            LopHistory lastHist = History[0]; //a mrkneme se na první záznam historie

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
                    IEnumerable<string> zmeny = GetRozdilyString(History[kolikataZHistorie-1], History[kolikataZHistorie]);
                    foreach (string s in zmeny)
                    {
                        historieZmen.Add(s); //OSTATNÍ ZMĚNY   
                    }
                }
            }
            return historieZmen; //vrací seznam dvojic stringů (vždy CO, HODNOTA) se změněnými hodnotami
        } 


        public virtual bool IsSame(Lop history)
        {
            bool shoda = true; //kontrola shodnosti materiálů
            if (Materialy.Count == history.Materialy.Count) shoda = false;
            else
            {
                for (int i = 0; i < Materialy.Count; i++)
                {
                    if (Materialy[i] != history.Materialy[i]) shoda = false;
                }
            }
            
            return Investice == history.Investice
                   && LessonLearned == history.LessonLearned
                   && Action == history.Action
                   && CheckDate == history.CheckDate
                   && CloseDate == history.CloseDate
                   && FinishDate == history.FinishDate
                //&& LastChangedDate == history.LastChangedDate //nelogické - plníme jen při ukládání
                   && Nazev == history.Nazev
                   && PlannedCloseDate == history.PlannedCloseDate
                   && Popis == history.Popis
                   && Komentar == history.Komentar
                   && DeniedMessage == history.DeniedMessage
                   && Resitel.Id == history.Resitel.Id
                   && StartDate == history.StartDate //nesedí ?
                   && Status == history.Status
                   && Dilna == history.Dilna
                   && Zadavatel.Id == history.Zadavatel.Id //TODO do budoucna přepsat z ID na celé objekty??
                   && shoda
                   ;

        }
        
        public static IEnumerable<SelectListItem> GetSelectListDilny()
        {
            return (Enum.GetValues(typeof(Dilna)).Cast<Dilna>().Select(
                enu => new SelectListItem() { Text = enu.ToString(), Value = ((int)enu).ToString() })).ToList();
        }
    }
    public class SubUkol : IEntity
    {
        public virtual int Id { get; set; }
        public virtual int LopId
        {
            get
            {
                return Lop?.Id ?? 0;
            }
            set
            {
                Lop = new LopDao().GetById(value);
            }
        }
        public virtual Lop Lop { get; set; }
        public virtual int Action { get; set; }
        public virtual StatusUkolu Status { get; set; }
        public virtual DateTime StartDate { get; set; } = DateTime.Now; //automaticky
        
        public virtual DateTime PlannedCloseDate { get; set; }  // Zadavatel
        public virtual string CloseDateInput
        {
            get
            {
                return CloseDate == null ? "" : CloseDate.GetValueOrDefault().ToShortDateString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    CloseDate = Convert.ToDateTime(value);
            }
        }
        public virtual DateTime? CloseDate { get; set; }         // zadavatel

        public virtual string FinishDateInput
        {
            get
            {
                return FinishDate == null ? "" : FinishDate.GetValueOrDefault().ToShortDateString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    FinishDate = Convert.ToDateTime(value);
            }
        }
        public virtual DateTime? FinishDate { get; set; }    // zadá řešitel
        public virtual string CheckDateInput
        {
            get
            {
                return CheckDate == null ? "" : CheckDate.GetValueOrDefault().ToShortDateString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    CheckDate = Convert.ToDateTime(value);
            }
        }
        public virtual DateTime? CheckDate { get; set; }    // zadá zadavatel
        public virtual DateTime LastChangedDate { get; set; }
        
        public virtual Uzivatel Zadavatel { get; set; }
        public virtual Uzivatel Resitel { get; set; }
        [Required]
        [StringLength(50)]
        public virtual string Nazev { get; set; }
        [AllowHtml]
        public virtual string Popis { get; set; }
        [AllowHtml]
        public virtual string Komentar { get; set; }
        public virtual string DeniedMessage { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual IList<SubUkolHistory> History { get; set; }

        public virtual string GetClassByState()
        {
            if (Status == StatusUkolu.Open)
                return ShouldBeAlreadyDone() ? "danger" : "";
            return FinishDate == null ? "warning" : "success";
        }

        public virtual bool ShouldBeAlreadyDone()
        {
            return (DateTime.Compare(PlannedCloseDate, DateTime.Now) < 0 && Status != StatusUkolu.Closed);
        }

        public virtual bool IsSame(SubUkol history)
        {
            return Action == history.Action
                        && CheckDate == history.CheckDate
                        && CloseDate == history.CloseDate
                        && FinishDate == history.FinishDate
                        //&& LastChangedDate == history.LastChangedDate //to ukládáme až při ukládání
                        && Nazev == history.Nazev
                        && PlannedCloseDate == history.PlannedCloseDate
                        && Popis == history.Popis
                        && Resitel.Id == history.Resitel.Id
                        //&& StartDate == history.StartDate //history.StartDate má detailnější datum i s hodinama vteřinama a tak TODO tváří se jako rozdílný
                        && Status == history.Status
                        && Komentar == history.Komentar
                        && DeniedMessage == history.DeniedMessage
                        && Zadavatel.Id == history.Zadavatel.Id;
            
        }

        private IEnumerable<string> GetRozdilyString(SubUkolHistory newer, SubUkolHistory older)
        {

            //MYŠLENKA: - zapíšeme do pole vždycky dvojici (ATRIBUT,HODNOTA .... ATRIBUT,HODNOTA .... 
            //Z toho potom hezky vytáhneme která hodnota se změnila a jaká byla původní hodnota před změnou ;)
            var whiteSpace = "-";
            IList<string> zmeny = new List<string>();

            if (newer.Action != older.Action)                      { zmeny.Add("Action");            zmeny.Add(older.Action.ToString());                        }
            if (newer.Status != older.Status)                      { zmeny.Add("Status");            zmeny.Add(older.Status.ToString());                        }
            if (newer.StartDate != older.StartDate)                { zmeny.Add("StartDate");         zmeny.Add(older.StartDate.ToString(CultureInfo.CurrentCulture));                     }
            if (newer.PlannedCloseDate != older.PlannedCloseDate)  { zmeny.Add("PlannedCloseDate");  zmeny.Add(older.PlannedCloseDate == null ? whiteSpace : older.PlannedCloseDate.GetValueOrDefault().ToShortDateString()); }
            if (newer.CloseDate != older.CloseDate)                { zmeny.Add("CloseDate");         zmeny.Add(older.CloseDate == null ? whiteSpace : older.CloseDate.GetValueOrDefault().ToShortDateString()); }
            if (newer.FinishDate != older.FinishDate)              { zmeny.Add("FinishDate");        zmeny.Add(older.FinishDate == null ? whiteSpace : older.FinishDate.GetValueOrDefault().ToShortDateString()); }
            if (newer.CheckDate != older.CheckDate)                { zmeny.Add("CheckDate");         zmeny.Add(older.CheckDate == null ? whiteSpace : older.CheckDate.GetValueOrDefault().ToShortDateString()); }
            //if (newer.LastChangedDate != older.LastChangedDate)    { zmeny.Add("LastChangedDate");   zmeny.Add(older.LastChangedDate.ToShortDateString());     }
            if (newer.Zadavatel != older.Zadavatel)                { zmeny.Add("Zadavatel");         zmeny.Add(older.Zadavatel.Prijmeni+" "+Zadavatel.Jmeno);   }
            if (newer.Resitel != older.Resitel)                    { zmeny.Add("Resitel");           zmeny.Add(older.Resitel.Prijmeni+" "+Resitel.Jmeno);       }
            if (newer.Nazev != older.Nazev)                        { zmeny.Add("Nazev");             zmeny.Add(older.Nazev);                                    }
            if (newer.Popis != older.Popis)                        { zmeny.Add("Popis");             zmeny.Add(older.Popis);                                    }
            
            //if (newer.Komentar != older.Popis)                        { zmeny.Add("Popis");             zmeny.Add(newer.Popis);                                    }
            

            return zmeny;
        }

        public virtual IList<string> UpraveneHodnotyVHistorii(int kolikataZHistorie)
        {
            IList<string> historieZmen = new List<string>();

            //první historie se porovnává s nynějším objektem - bohužel nejde jen tak přetipovat - takže se takhle vytvoří nová "historie"
            var thisAsHistory = new SubUkolHistory()
            {
                Action = Action
                ,
                Status = Status
                ,
                StartDate = StartDate
                ,
                PlannedCloseDate = PlannedCloseDate
                ,
                CloseDate = CloseDate
                ,
                FinishDate = FinishDate
                ,
                CheckDate = CheckDate
                ,
                LastChangedDate = LastChangedDate
                ,
                Zadavatel = Zadavatel
                ,
                Resitel = Resitel
                ,
                Nazev = Nazev
                ,
                Popis = Popis
                ,
                Komentar = Komentar
                ,
                DeniedMessage = DeniedMessage

            };

            var lastHist = History[0]; //a mrkneme se na první záznam historie

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
                    IEnumerable<string> zmeny = GetRozdilyString(History[kolikataZHistorie], History[kolikataZHistorie - 1]);
                    foreach (string s in zmeny)
                    {
                        historieZmen.Add(s); //OSTATNÍ ZMĚNY   
                    }
                }
            }

            return historieZmen; //vrací seznam dvojic stringů (vždy CO, HODNOTA) se změněnými hodnotami
        }
    }

    public class LopHistory : IEntity
    {
        public virtual int Id { get; set; }

        public virtual int LopId
        {
            get
            {
                return Lop?.Id ?? 0;
            }
            set
            {
                Lop = new LopDao().GetById(value);
            }
        }
        public virtual Lop Lop { get; set; }
        public virtual int Action { get; set; }
        public virtual StatusUkolu Status { get; set; }
        public virtual IList<LopHistoryMaterial> Materialy { get; set; }
        public virtual DateTime StartDate { get; set; }         // automaticky
        public virtual DateTime? PlannedCloseDate { get; set; }  // Zadavatel
        public virtual DateTime? CloseDate { get; set; }         // zadavatel
        public virtual DateTime? FinishDate { get; set; }    // zadá řešitel
        public virtual DateTime? CheckDate { get; set; }    // zadá zadavatel
        
        public virtual Uzivatel Zadavatel { get; set; }
        public virtual Uzivatel Resitel { get; set; }
        public virtual DateTime LastChangedDate { get; set; }
        public virtual string Nazev { get; set; }
        //[AllowHtml]
        public virtual string Popis { get; set; }
        public virtual string Komentar { get; set; }
        public virtual string DeniedMessage { get; set; }
        public virtual bool Investice { get; set; }
        public virtual bool LessonLearned { get; set; }
        public virtual Dilna Dilna { get; set; }
        public LopHistory() { }
        /*public LopHistory(Lop arg)  // TODO Hledal jsem místa, dke je tato metoda využívaná, ale nidke nic, je ke smazání?
        {
            Lop = arg;
            Action = arg.Action;
            Status = arg.Status;
            //Materialy = arg.Materialy; TODO problém s přetypováním, tato metoda se ale nikde nepoužívá
            StartDate = arg.StartDate;
            CheckDate = arg.CheckDate;
            PlannedCloseDate = arg.PlannedCloseDate;
            Zadavatel = arg.Zadavatel;
            Resitel = arg.Resitel;
            LastChangedDate = arg.LastChangedDate;
            Nazev = arg.Nazev;
            Popis = arg.Popis;
            Komentar = arg.Komentar;
            DeniedMessage = arg.DeniedMessage;
            Investice = arg.Investice;
            LessonLearned= arg.LessonLearned;
        }*/
    }
    
    public class SubUkolHistory : IEntity
    {
        public virtual int Id { get; set; }
        public virtual int SubUkolId
        {
            get
            {
                return SubUkol?.Id ?? 0;
            }
            set
            {
                SubUkol = new SubUkolDao().GetById(value);
            }
        }
        public virtual SubUkol SubUkol { get; set; }
        public virtual int Action { get; set; }
        public virtual StatusUkolu Status { get; set; }
        public virtual DateTime StartDate { get; set; }     //automaticky
        public virtual DateTime? PlannedCloseDate { get; set; }     // ručně
        public virtual DateTime? CloseDate { get; set; }     // ručně
        public virtual DateTime? FinishDate { get; set; }    // zadá řešitel
        public virtual DateTime? CheckDate { get; set; }     // zadá zadavatel
        public virtual DateTime LastChangedDate { get; set; }
        public virtual Uzivatel Zadavatel { get; set; }
        public virtual Uzivatel Resitel { get; set; }
        public virtual string Nazev { get; set; }
        //[AllowHtml]
        public virtual string Popis { get; set; }
        [AllowHtml]
        public virtual string Komentar { get; set; }
        public virtual string DeniedMessage { get; set; }

        public SubUkolHistory()
        {
            
        }
        public SubUkolHistory(SubUkol arg)
        {
            SubUkol = arg;
            Action = arg.Action;
            Status = arg.Status;
            StartDate = arg.StartDate;
            PlannedCloseDate = arg.PlannedCloseDate;
            CloseDate = arg.CloseDate;
            FinishDate = arg.FinishDate;
            CheckDate = arg.CheckDate;
            Zadavatel = arg.Zadavatel;
            Resitel = arg.Resitel;
            LastChangedDate = arg.LastChangedDate;
            Nazev = arg.Nazev;
            Popis = arg.Popis;
            Komentar = arg.Komentar;
            DeniedMessage = arg.DeniedMessage;
        }
        
    }
    public enum StatusUkolu
    {
        Open = 0,
        Closed = 1,
        Denied = 2
    }

    public enum Dilna
    {
        None = 0,
        VD1 = 1,
        VD2 = 2,
        VD3 = 3,
        VD4 = 4,
        VD5 = 5,
        VD6 = 6
    }

}
