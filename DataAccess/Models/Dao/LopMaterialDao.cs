using DataAccess.Models.DataUnit;

namespace DataAccess.Models.Dao
{
    public class LopMaterialDao : DaoBase<LopMaterial>
    {
        public void ClearWrongHistoryAfterCreate()
        {
            Session.CreateQuery("delete LopHistoryMaterial where LopHistory = -1").ExecuteUpdate();
            /*using (var transaction = Session.BeginTransaction())
            {
                Session.Delete(new LopHistoryMaterial() {Id = -1});
                transaction.Commit();
            }*/
        }
        /*
        using (ITransaction transaction = Session.BeginTransaction())
            {
                Session.Delete(entity);
                transaction.Commit();
            }/*/
    }
}
