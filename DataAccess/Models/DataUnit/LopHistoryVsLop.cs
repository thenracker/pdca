namespace DataAccess.Models.DataUnit
{
    public class LopHistoryVsLop
    {
        public LopHistory LH { get; set; }
        public Lop L { get; set; }

        public LopHistoryVsLop(LopHistory lh, Lop l)
        {
            this.LH = lh;
            this.L = l;
        }
    }
}
