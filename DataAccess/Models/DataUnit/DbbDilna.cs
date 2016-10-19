using System.Collections.Generic;
using DataAccess.Models.Dao;
using DataAccess.Models.DataUnit.Users;
using DataAccess.Models.Interface;

namespace DataAccess.Models.DataUnit
{
    public class DbbDilna : IEntity
    {
        public virtual int Id { get; set; }

        public virtual string Nazev { get; set; }

    }
}

