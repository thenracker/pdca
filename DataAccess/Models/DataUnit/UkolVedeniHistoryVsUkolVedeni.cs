namespace DataAccess.Models.DataUnit
{
    public class UkolVedeniHistoryVsUkolVedeni
    {
        public UkolVedeniHistory LH { get; set; }
        public UkolVedeni L { get; set; }

        public UkolVedeniHistoryVsUkolVedeni(UkolVedeniHistory uh, UkolVedeni u)
        {
            this.LH = uh;
            this.L = u;
        }
    }
}
