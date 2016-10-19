using DataAccess.Models.Interface;

namespace DataAccess.Models.DataUnit.Users
{
    /// <summary>
    /// Slouží k nastavení vyšších oprávnění jako je například Admin pro vybrané uživatele
    /// </summary>
    public class Permission : IEntity
    {
        public virtual int Id { get; set; }
        public virtual string WindowsId { get; set; }
        public virtual Role Role { get; set; }
    }
}
