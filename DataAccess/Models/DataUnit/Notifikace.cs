using System;
using System.Web.Mvc;
using DataAccess.Models.Dao;
using DataAccess.Models.DataUnit.Users;
using DataAccess.Models.Interface;

namespace DataAccess.Models.DataUnit
{
    public class Notifikace : IEntity
    {
        const string AppUrl = "http://server10.swotes.cz";
        public virtual int Id { get; set; }
        public virtual Uzivatel Uzivatel { get; set; }
        [AllowHtml]
        public virtual string Text { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual bool Seen { get; set; }

        

        public static int Create(int userId, string text)
        {
#if DEBUG
#else
            text = text.Replace("<a href=\"", "<a href=\"" + AppUrl);
            text = text.Replace("<a href='", "<a href='" + AppUrl);
#endif

            var dao = new NotifikaceDao();
            var uzivatel = new UzivatelDao().GetById(userId);
            var notifikace = new Notifikace()
            {
                Uzivatel = uzivatel,
                Text = text,
                Created = DateTime.Now,
                Seen = false
            };
            notifikace.Id = (int) dao.Create(notifikace);

            if (string.IsNullOrEmpty(uzivatel.Email))
                return notifikace.Id;

           
            var email = new Email
            {
                Subject = "Nové upozornění",
                Body = text
            };
            email.AddMailToAddress(uzivatel.Email);
#if DEBUG
#else
            try{
                email.Send();
            }
            catch
            {
                // ignored
            }

#endif


            return notifikace.Id;

        }
    }
}
