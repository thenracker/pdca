using System;
using DataAccess.Models.Interface;

namespace DataAccess.Models.DataUnit
{
    public class LopHistoryMaterial : IEntity
    {
        public virtual int Id { get; set; }
        public virtual Material Produkt { get; set; }
        public virtual LopHistory LopHistory { get; set; }
        public virtual DateTime? DateAdded { get; set; } = DateTime.Now;
    }
}
