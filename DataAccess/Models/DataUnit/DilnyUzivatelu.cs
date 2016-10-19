using DataAccess.Models.DataUnit.Users;
using DataAccess.Models.Interface;

namespace DataAccess.Models.DataUnit
{
    public class DilnyUzivatelu : IEntity
    {
        public virtual int Id { get; set; }
        public virtual Uzivatel Uzivatel { get; set; }
        public virtual DbbDilna Dilna { get; set; }

    }
}

