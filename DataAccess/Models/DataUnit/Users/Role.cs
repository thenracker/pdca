using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DataAccess.Models.Dao;
using DataAccess.Models.Interface;

namespace DataAccess.Models.DataUnit.Users
{
    public class Role : IEntity
    {
        public virtual int Id { get; set; }
        public virtual string Identificator { get; set; }
        public virtual string Popis { get; set; }

        public static SelectList GetSelectListRoles()
        {
            return new SelectList(new RoleDao().GetAll()
                .OrderBy(x => x.Popis)
                .Select(item => new SelectListItem()
                {
                    Value = item.Id.ToString(),
                    Text = item.Popis
                })
                .ToList(),"Value","Text");
        }
        public static SelectList GetSelectListRolesWithDefault()
        {
            var role = new List<SelectListItem> { new SelectListItem() { Value = "0", Text = "Default" } };
            role.AddRange(new SelectList(new RoleDao().GetAll()
                .OrderBy(x => x.Popis)
                .Select(item => new SelectListItem()
                {
                    Value = item.Id.ToString(),
                    Text = item.Popis
                })
                .ToList(), "Value", "Text"));
            return new SelectList(role,"Value","Text");
        }

        public override string ToString()
        {
            return Popis;
        }
    }
}
