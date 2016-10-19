using DataAccess.Models.Interface;

namespace DataAccess.Models.DataUnit
{
    public class Material : IEntity
    {
        public virtual int Id { get; set; }
        public virtual string Druh { get; set; }
        public virtual string Pn { get; set; }
        public virtual string Popis { get; set; }
        public virtual string PopisSap { get; set; }
        public virtual string SapCislo { get; set; }
        public virtual string VedouciProjektu { get; set; }
        public virtual string ZodpovednostZaKvalitu { get; set; }
        public virtual string Metrologie { get; set; }
        public virtual string Baleni { get; set; }

        public override string ToString()
        {
            return $"{SapCislo}";
        }
    }
}

