namespace DataAccess.Models.DataUnit
{
    public class SubUkolHistoryVsSubUkol
    {
        public SubUkolHistory LH { get; set; }
        public SubUkol L { get; set; }

        public SubUkolHistoryVsSubUkol(SubUkolHistory lh, SubUkol l)
        {
            this.LH = lh;
            this.L = l;
        }
    }
}
