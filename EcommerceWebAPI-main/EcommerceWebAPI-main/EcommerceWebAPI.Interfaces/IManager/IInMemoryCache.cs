namespace EcommerceWebAPI.Interfaces.IManager
{
    public interface IInMemoryCache
    {
        void Set<T>(string key, T value,TimeSpan? duration=null);
        T? Get<T>(string key);
        void Remove(string key);
        bool TryGet<T>(string key, out T value);   
    }
}
