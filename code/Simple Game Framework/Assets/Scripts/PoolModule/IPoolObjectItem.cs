namespace PoolModule
{
    public interface IPoolObjectItem
    {
        void OnGetHandle();
        void OnRecycleHandle();
    }
}