using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using DataAccess.Models.Dao;
using DataAccess.Models.Interface;

namespace DataAccess.Models.DataUnit.Users
{
    public class Uzivatel : IEntity
    {
        public virtual int SmartId
        {
            get { return Id; }
            set
            {
                Id = value;
                CopyToLocal(new UzivatelDao().GetById(value));
            }
        }
        public virtual int Id { get; set; }
        [Required]
        [StringLength(50)]
        public virtual string WindowsId { get; set; }
        [StringLength(50)]
        public virtual string OsCislo { get; set; }
        [Required]
        [StringLength(50)]
        public virtual string Jmeno { get; set; }
        [Required]
        [StringLength(50)]
        public virtual string Prijmeni { get; set; }
        [StringLength(50)]
        public virtual string Email { get; set; }
        //public virtual Usek Usek { get; set; } //DO ÚSEKU SE LEZE PŘES ODDĚLENÍ

        public virtual Oddeleni Oddeleni { get; set; }  // oddělení do kterého uživatel patří
        
        public virtual Role Role { get; set; }
        public virtual IList<DilnyUzivatelu> DilnyUzivatelu { get; set; } 

        public virtual IList<CustomTableSetting> VlastniNastaveniZobrazeni { get; set; }

        public virtual DateTime PlatnostDo { get; set; } // kvůli kešování

        public Uzivatel()
        {

        }

        public Uzivatel(string arg)
        {
            WindowsId = arg.ToUpper();
            var dao = new UzivatelDao();
            var item = dao.GetByWindowsId(arg);
            if (item == null) return;
            Id = item.Id;
            OsCislo = item.OsCislo;
            Jmeno = item.Jmeno;
            Prijmeni = item.Prijmeni;
            //Usek = item.Usek;
            Oddeleni = item.Oddeleni;
            Role = item.Role;

            DilnyUzivatelu = item.DilnyUzivatelu;
            LoadPermission();
        }

        public override string ToString()
        {
            return $"{Prijmeni} {Jmeno}";
        }

        public virtual bool Compare(Uzivatel obj)
        {
            return OsCislo == obj.OsCislo;
        }

        public virtual void LoadPermission()
        {
            var permDao = new PermissionDao();
            var permItem = permDao.GetByUser(WindowsId);
            if (permItem != null)
                Role = permItem.Role;
        }

        protected void CopyToLocal(Uzivatel arg)
        {
            WindowsId = arg.WindowsId;
            OsCislo = arg.OsCislo;
            Jmeno = arg.Jmeno;
            Prijmeni = arg.Prijmeni;
            Email = arg.Email;
            Oddeleni = arg.Oddeleni;
            //Usek = arg.Usek;
            Role = arg.Role;
        }
        public static bool UserExists(string windowsId)
        {
            var dao = new UzivatelDao();
            var item = dao.GetByWindowsId(windowsId.Trim().ToUpper());
            return item != null;
        }

        public virtual bool Match(string identity)
        {
            return WindowsId == identity.Trim().ToUpper();
        }

        public static IList<SelectListItem> GetSelectListUzivatele()
        {
            var dao = new UzivatelDao();
            var list = dao.GetAll();

            return list.OrderBy(x => x.Prijmeni).Select(m => new SelectListItem()
            {
                Value = m.Id.ToString(),
                Text = m.ToString()
            }).ToList();
        }

        public virtual bool ShowCollumn(string table, string coll)
        {
            if (table != "lop/index-lopList" && table != "home/index-meAsZadavatel" && table != "home/index-meAsResitel")
                return (VlastniNastaveniZobrazeni
                    .Where(x => x.Tabulka == table)
                    .Where(x => x.Sloupec == coll)
                    .DefaultIfEmpty(new CustomTableSetting() { Zobrazit = false })
                    .Select(x => x.Zobrazit).First());
            {
                switch (coll)
                {
                    case "Id":
                    case "Status":
                    case "Dilna":
                    case "Nazev":
                    case "MaxPlannedCloseDate":
                    case "Resitel":
                    case "PrehledPodukolu":
                    case "LastChangedDate":
                        return (VlastniNastaveniZobrazeni
                            .Where(x => x.Tabulka == table)
                            .Where(x => x.Sloupec == coll)
                            .DefaultIfEmpty(new CustomTableSetting() { Zobrazit = true })
                            .Select(x => x.Zobrazit).First());
                    default:
                        return (VlastniNastaveniZobrazeni
                            .Where(x => x.Tabulka == table)
                            .Where(x => x.Sloupec == coll)
                            .DefaultIfEmpty(new CustomTableSetting() { Zobrazit = false })
                            .Select(x => x.Zobrazit).First());
                }
            }

        }
        public virtual string ToTextBtn()
        {
            return $"<a href=\"#\" onclick=\"showUserDetail('{this.WindowsId.Replace("\\", "\\\\")}')\">{this}</a> ";
        }
        public static string ToTextBtn(string winId)
        {
            var uzivatel = new Uzivatel(winId);
            return string.IsNullOrWhiteSpace(winId) ? "" : $"<a href=\"#\" onclick=\"showUserDetail('{winId.Replace("\\","\\\\")}')\">{uzivatel}</a> ";
        }
    }
}
