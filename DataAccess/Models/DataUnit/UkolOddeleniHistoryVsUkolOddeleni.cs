namespace DataAccess.Models.DataUnit
{
    public class UkolOddeleniHistoryVsUkolOddeleni
    {
        public UkolOddeleniHistory UH { get; set; }
        public UkolOddeleni U { get; set; }

        public UkolOddeleniHistoryVsUkolOddeleni(UkolOddeleniHistory uh, UkolOddeleni u)
        {
            this.UH = uh;
            this.U = u;
        }
    }
}
