using HashMap.Interface;
using HashMap.Helper;
using HashMap.Exception;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HashMap.ThreadSafe;

namespace HashMap
{
    public class HashMap<K, V> : IMap<K, V>, IEnumerable<KeyValuePair<K, V>>
    {
        private const Int32 INITIAL_CAPACITY = 16;
        private const float LOAD_FACTOR = 0.5f;
        private const float MULTIPLIER = 2.0f;
        private const Int32 NUMBER_OF_ATTEMPS = 25;
        
        private readonly float loadFactor;
        private readonly float multiplier;

        private KeyValuePair<K, V>[] buckets;
        private Int32 initialCapacity;
        private Int32 usedBucketsCount = 0;
        private Int32 q;

        private RWLock rwLock = new RWLock();

        public IReadOnlyCollection<K> Keys {
            get
            {
                using (rwLock.ReadLock()) {
                    return buckets.Where(b => b.Value != null).Select(b => b.Key).ToArray();
                }
            }
        }
        public IReadOnlyCollection<V> Values
        {
            get
            {
                using (rwLock.ReadLock())
                {
                    return buckets.Where(b => b.Value != null).Select(b => b.Value).ToArray();
                }
            }
        }

        public HashMap(Int32 initialCapacity, float loadFactor, float multiplier) {
            this.initialCapacity = initialCapacity > INITIAL_CAPACITY ? initialCapacity : INITIAL_CAPACITY;
            this.loadFactor = loadFactor > 0.0f && loadFactor <= 1.0f ? loadFactor : LOAD_FACTOR;
            this.multiplier = multiplier > MULTIPLIER ? multiplier : MULTIPLIER;
            this.q = GetSimpleNumberCloserTo(this.initialCapacity);
            buckets = new KeyValuePair<K, V>[initialCapacity];
        }

        #region user friendly interface

        public bool ContainsKey(K key)
        {
            using (rwLock.ReadLock())
            {
                Int32 index = GetIndex(GetHashCode(key.GetHashCode()));
                Int32? nullableIndex = FindKeyIndexFromIndex(index, key);
                return nullableIndex != null;
            }
        }

        public V Get(K key)
        {
            using (rwLock.ReadLock())
            {
                Int32 index = GetIndex(GetHashCode(key.GetHashCode()));
                Int32? nullableIndex = FindKeyIndexFromIndex(index, key);
                return nullableIndex != null ? buckets[(Int32)nullableIndex].Value : default;
            }
        }

        public void Put(K key,V value)
        {
            using (rwLock.WriteLock())
            {
                SecurePut(key, value);
            }
        }

        public V Remove(K key)
        {
            using (rwLock.WriteLock())
            {
                Int32 index = GetIndex(GetHashCode(key.GetHashCode()));
                Int32? nullableIndex = FindKeyIndexFromIndex(index, key);
                if (nullableIndex != null)
                {
                    V removedObject = buckets[(Int32)nullableIndex].Value;
                    buckets[(Int32)nullableIndex] = default;
                    return removedObject;
                }
                return default;
            }
        }

        public Int32 Size()
        {
            using (rwLock.ReadLock())
            {
                return usedBucketsCount;
            }
        }

        public void Clear()
        {
            using (rwLock.WriteLock())
            {
                Array.Clear(buckets, 0, buckets.Length);
                usedBucketsCount = 0;
            }
        }

        public bool ContainsValue(V value)
        {
            using (rwLock.ReadLock())
            {
                if (value == null)
                    return false;

                foreach (KeyValuePair<K, V> bucket in buckets)
                {
                    if (bucket.Value != null && bucket.Value.Equals(value))
                        return true;
                }
                return false;
            }
        }

        public bool isEmpty()
        {
            using (rwLock.ReadLock())
            {
                return usedBucketsCount == 0;
            }
        }

        public void PutAll(IMap<K, V> map)
        {
            using (rwLock.WriteLock())
            {
                foreach (KeyValuePair<K, V> bucket in map)
                {
                    SecurePut(bucket.Key, bucket.Value);
                }
            }
        }

        #region Implementation of IEnumerable
        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            IEnumerable<KeyValuePair<K, V>> fillteredBuckets;
            using (rwLock.WriteLock())
            {
                fillteredBuckets = buckets.Where(b => b.Value != null);
            }
            foreach (KeyValuePair<K, V> bucket in fillteredBuckets)
            {
                yield return bucket;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #endregion

        #region inside methods
        private Int32 GetHashCode(Int32 key)
        {
            Int32 restrictedkey = (Int32) Convert.ToUInt32(key) % Int32.MaxValue;
            restrictedkey ^= (restrictedkey >> 20) ^ (restrictedkey >> 12);
            return restrictedkey ^ (restrictedkey >> 7) ^ (restrictedkey >> 4);
        }

        private Int32 GetIndex(Int32 hash) => hash % buckets.Length;

        private void RehashUp(bool shouldBeExtended = false)
        {
            if (!shouldBeExtended && (usedBucketsCount / buckets.Length <= loadFactor))
                return;

            this.initialCapacity = GetNewLengthForBucketArray();
            KeyValuePair<K, V>[] newBuckets = new KeyValuePair<K, V>[this.initialCapacity];
            this.q = GetSimpleNumberCloserTo(this.initialCapacity);

            foreach (KeyValuePair<K, V> bucket in buckets)
            {
                if (bucket.Value != null)
                {
                    Int32 index = GetIndex(GetHashCode(bucket.Key.GetHashCode()));
                    index = GetIndexWithOffset(newBuckets, index);
                    PutBucket(newBuckets, index, bucket);
                }
            }

            buckets = newBuckets;
        }

        private void SecurePut(K key, V value, bool shouldBeExtended = false)
        {
            try
            {
                RehashUp(shouldBeExtended);
                Int32 hashCode = key != null ? key.GetHashCode() : 0;
                Int32 index = GetIndex(GetHashCode(hashCode));
                index = GetIndexWithOffset(buckets, index);
                PutBucket(buckets, index, new KeyValuePair<K, V>(key, value));
                usedBucketsCount++;
            }
            catch (MapCantFitItemException)
            {
                SecurePut(key, value, true);
            }
        }
        private void PutBucket(KeyValuePair<K, V>[] _buckets, Int32 index, KeyValuePair<K, V> bucket)
        {
            _buckets[index] = bucket;
        }

        private Int32 GetIndexWithOffset(KeyValuePair<K, V>[] _buckets, Int32 currentIndex)
        {
            Int32 indexWithOffset = currentIndex;
            for (Int32 i = 0; _buckets[indexWithOffset].Value != null; i++)
            {
                indexWithOffset = (currentIndex + i * i) % q + 1;
                if (i > NUMBER_OF_ATTEMPS)
                    throw new MapCantFitItemException();
            }
            return indexWithOffset;
        }

        private Int32 GetSimpleNumberCloserTo(Int32 n)
        {
            Int32 checkNumber = n - ((n - 1) % 2);
            while (!MillerRabin.IsSimplyNumber(checkNumber)) {
                checkNumber -= 2;
            }
            return checkNumber;
        }

        private Int32? FindKeyIndexFromIndex(Int32 currentIndex, K key)
        {
            Int32 indexWithOffset = currentIndex;
            for (Int32 i = 0; buckets[indexWithOffset].Value == null
                || !buckets[indexWithOffset].Key.Equals(key); i++)
            {
                indexWithOffset = (currentIndex + i * i) % q + 1;
                if (i > NUMBER_OF_ATTEMPS)
                    return null;
            }
            return indexWithOffset;
        }

        private Int32 GetNewLengthForBucketArray()
        {
            float fLength = initialCapacity * multiplier;
            fLength = (fLength + fLength % 2);

            if (buckets.Length == Int32.MaxValue)
            {
                throw new MapIsFilledException();
            }

            return Convert.ToInt32((fLength - 1) % Int32.MaxValue) + 1;
        }

        #endregion
    }
}
