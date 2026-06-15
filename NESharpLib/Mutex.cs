using System.Threading;

namespace NESharpLib
{
    public struct MutLock<T> : IDisposable
    {
        private Mutex mutex;
        public T Item;
        private Action<T> callback;
        public MutLock(Mutex mut, T value, Action<T> disposed)
        {
            callback = disposed;
            mutex = mut;
            Item = value;
        }
        public void Release()
        {
            Dispose();
        }
        public void Dispose()
        {
            callback.Invoke(Item);
            mutex.ReleaseMutex();
            mutex.Dispose();
        }
    }
    public class Mut<T> : IDisposable
    {
        private Mutex mutex = new Mutex();
        private T inner;
        public Mut(T value)
        {
            inner = value;
        }
        public void Access(out MutLock<T> value)
        {
            mutex.WaitOne();
            value = new MutLock<T>(mutex, inner, (res) =>
            {
                inner = res;
            });
        }
        public void Dispose()
        {
            mutex.ReleaseMutex();
            mutex.Dispose();
        }
    }
}