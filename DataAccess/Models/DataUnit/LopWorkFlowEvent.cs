using System;
using DataAccess.Models.Dao;
using DataAccess.Models.Interface;

namespace DataAccess.Models.DataUnit
{
    public class LopWorkFlowEvent : IEntity
    {
        public virtual int Id { get; set; }
        public virtual Lop Lop { get; set; }
        public virtual string Color { get; set; }
        public virtual string Obrazek { get; set; }
        public virtual string Nadpis { get; set; }
        public virtual string Text { get; set; }
        public virtual DateTime Datum { get; set; }
        public virtual string Autor { get; set; }

        public static void Create(Lop lop,string color, string obrazek, string nadpis, string text, string autor)
        {
            var dao = new LopWorkFlowEventDao();
            dao.Create(new LopWorkFlowEvent()
            {
                Lop = lop,
                Color = color,
                Obrazek = obrazek,
                Nadpis = nadpis,
                Text = text,
                Datum = DateTime.Now,
                Autor = autor
            });
        }
        public static class Colors
        {
            public static string Primary = "primary";
            public static string Info = "info";
            public static string Success = "success";
            public static string Warning = "warning";
            public static string Danger = "danger";
        }

        public static class Icons
        {
            public static string Pencil = "glyphicon-pencil";
            public static string Ok = "glyphicon-ok";
            public static string Remove = "glyphicon-remove";
            public static string User = "glyphicon-user";
            public static string File = "glyphicon-file";
            public static string Message = "glyphicon-envelope";
        }
    }
}
