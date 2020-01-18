using System;
using System.Threading;

namespace HashMap.ThreadSafe
{
    public class RWLock : IDisposable
    {
        public struct WriteLockToken : IDisposable
        {
            private readonly ReaderWriterLockSlim _lock;
            public WriteLockToken(ReaderWriterLockSlim _lock)
            {
                this._lock = _lock;
                _lock.EnterWriteLock();
            }
            public void Dispose() => _lock.ExitWriteLock();
        }

        public struct ReadLockToken : IDisposable
        {
            private readonly ReaderWriterLockSlim _lock;
            public ReadLockToken(ReaderWriterLockSlim _lock)
            {
                this._lock = _lock;
                _lock.EnterReadLock();
            }
            public void Dispose() => _lock.ExitReadLock();
        }

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public ReadLockToken ReadLock() => new ReadLockToken(_lock);
        public WriteLockToken WriteLock() => new WriteLockToken(_lock);
        public void Dispose() => _lock.Dispose();
    }
}
