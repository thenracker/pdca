using System.Collections.Generic;
using DataAccess.Models.Dao;
using DataAccess.Models.DataUnit.Users;
using DataAccess.Models.Interface;

namespace DataAccess.Models.DataUnit
{
    public class VedouciOdeleni : IEntity
    {
        public virtual int Id { get; set; }
        public virtual string WindowsId  { get; set; }
        public virtual Oddeleni Oddeleni { get; set; }

        public virtual Uzivatel Uzivatel => new Uzivatel(WindowsId);
    }
}

