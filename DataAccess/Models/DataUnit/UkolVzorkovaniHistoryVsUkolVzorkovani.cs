namespace DataAccess.Models.DataUnit
{
    public class UkolVzorkovaniHistoryVsUkolVzorkovani
    {
        public UkolVzorkovaniHistory UH { get; set; }
        public UkolVzorkovani U { get; set; }

        public UkolVzorkovaniHistoryVsUkolVzorkovani(UkolVzorkovaniHistory uh, UkolVzorkovani u)
        {
            this.UH = uh;
            this.U = u;
        }
    }
}
