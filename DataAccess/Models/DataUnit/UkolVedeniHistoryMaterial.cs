using System;
using DataAccess.Models.Interface;

namespace DataAccess.Models.DataUnit
{
    public class UkolVedeniHistoryMaterial : IEntity
    {
        public virtual int Id { get; set; }
        public virtual Material Produkt { get; set; }
        public virtual UkolVedeniHistory UkolVedeniHistory { get; set; }
        public virtual DateTime? DateAdded { get; set; } = DateTime.Now;
    }
}
