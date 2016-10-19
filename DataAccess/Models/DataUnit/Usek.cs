using System.Collections.Generic;
using DataAccess.Models.Dao;
using DataAccess.Models.DataUnit.Users;
using DataAccess.Models.Interface;

namespace DataAccess.Models.DataUnit
{
    public class Usek : IEntity
    {
        public virtual int Id { get; set; }
        public virtual string VedouciWinId { get; set; }

        public virtual Uzivatel Vedouci => new Uzivatel(VedouciWinId);

        public virtual string Nazev { get; set; }

        public virtual IList<Oddeleni> Oddeleni
        {
            get { return new OddeleniDao().GetAllByUsek(this); }
        }

        public override string ToString()
        {
            return $"{Nazev}";
        }
    }
}

