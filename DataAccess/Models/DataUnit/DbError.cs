using System;
using DataAccess.Models.Interface;

namespace DataAccess.Models.DataUnit
{
    public class DbError : IEntity
    {
        public virtual int Id { get; set; }
        public virtual string WindowsId { get; set; }
        public virtual string Message { get; set; }
        public virtual string Source { get; set; }
        public virtual string StackTrace { get; set; }
        public virtual DateTime Time { get; set; }
    }
}
