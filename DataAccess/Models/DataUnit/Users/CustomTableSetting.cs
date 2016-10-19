using DataAccess.Models.Interface;

namespace DataAccess.Models.DataUnit.Users
{
    public class CustomTableSetting : IEntity
    {
        public virtual int Id { get; set; }
        
        public virtual Uzivatel Uzivatel { get; set; }

        public virtual string Tabulka { get; set; }

        public virtual string Sloupec { get; set; }

        public virtual bool Zobrazit { get; set; }
    }
}
