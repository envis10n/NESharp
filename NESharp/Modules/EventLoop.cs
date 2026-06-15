using System.Collections.Concurrent;

namespace NESharp
{
    class EventLoop
    {
        private ConcurrentQueue<Action<ProgramState>> _queue = new ConcurrentQueue<Action<ProgramState>>();
        public void Enqueue(Action<ProgramState> ev)
        {
            _queue.Enqueue(ev);
        }
        public bool HasActions()
        {
            return !_queue.IsEmpty;
        }
        public void Process(ref ProgramState state)
        {
            if (_queue.IsEmpty) return;
            foreach (Action<ProgramState> a in _queue.ToArray())
            {
                a.Invoke(state);
            }
        }
    }
}