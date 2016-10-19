using System.Collections.Generic;

namespace DataAccess.Models.DataUnit
{
    public class HomeCollection
    {
        public IList<Lop> Lops { get; set; }
        public IList<UkolOddeleni> UkolyOddeleni { get; set; }
        public IList<UkolVedeni> UkolyVedeni { get; set; }
        public IList<UkolVzorkovani> UkolyVzorkovani { get; set; }

        public HomeCollection(IList<Lop> a, IList<UkolOddeleni> b, IList<UkolVedeni> c, IList<UkolVzorkovani> d)
        {
            Lops = a;
            UkolyOddeleni = b;
            UkolyVedeni = c;
            UkolyVzorkovani = d;
        }
    }
}
