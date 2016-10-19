using DataAccess.Models.DataUnit;

namespace DataAccess.Models.Dao
{
    public class UkolVedeniMaterialDao : DaoBase<UkolVedeniMaterial>
    {
        public void ClearWrongHistoryAfterCreate()
        {
            Session.CreateQuery("delete UkolVedeniHistoryMaterial where UkolVedeniHistory = -1").ExecuteUpdate();
            /*using (var transaction = Session.BeginTransaction())
            {
                Session.Delete(new LopHistoryMaterial() {Id = -1});
                transaction.Commit();
            }*/
        }
    }
}
