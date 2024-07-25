namespace NewsApp.Repositories.Interfaces
{
    public interface IRepository<K, T, V> where T : class
    {
        public Task<T> Add(T item);
        public Task<T> Delete(V key);
        public Task<T> Update(T item, V key);
        public Task<T> Get(K key, V value);
        public Task<IEnumerable<T>> GetAll(K key, V value);
    }
}
