using System.Collections.Generic;

namespace HashMap.Interface
{
    public interface IMap<K, V>
    {
        void Clear();
        bool ContainsKey(K key);
        bool ContainsValue(V value);
        bool isEmpty();
        void Put(K key, V value);
        void PutAll(IMap<K, V> map);
        V Remove(K key);
        int Size();
        V Get(K key);
        IEnumerator<KeyValuePair<K, V>> GetEnumerator();
    }
}
