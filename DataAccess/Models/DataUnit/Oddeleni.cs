using System.Collections.Generic;
using DataAccess.Models.Interface;

namespace DataAccess.Models.DataUnit
{
    public class Oddeleni : IEntity
    {
        public virtual int Id { get; set; }
        public virtual IList<VedouciOdeleni> VedouciOdeleni { get; set; }
        public virtual Usek Usek { get; set; }
        public virtual string Nazev { get; set; }
        public virtual int UsekId { get; set; }
        public override string ToString()
        {
            return $"{Nazev}";
        }
    }
}

