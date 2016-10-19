using System;
using DataAccess.Models.Interface;

namespace DataAccess.Models.DataUnit
{
    public class LopMaterial : IEntity
    {
        public virtual int Id { get; set; }
        public virtual Material Produkt { get; set; }
        public virtual Lop Lop { get; set; }
        public virtual DateTime? DateAdded { get; set; } = DateTime.Now;
    }
}
