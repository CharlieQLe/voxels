using System.Collections;
using System.Collections.Generic;

namespace Voxels {
    public class UniqueQueue<T> : IEnumerable<T> {
        private Queue<T> _queue;
        private HashSet<T> _hashSet;

        public int Count => _queue.Count;
        
        public UniqueQueue() {
            _queue = new Queue<T>();
            _hashSet = new HashSet<T>();
        }

        public bool Enqueue(T item) {
            if (!_hashSet.Add(item)) return false;
            _queue.Enqueue(item);
            return true;
        }

        public T Dequeue() {
            T item = _queue.Dequeue();
            _hashSet.Remove(item);
            return item;
        }

        public T Peek() => _queue.Peek();

        public void Clear() {
            _hashSet.Clear();
            _queue.Clear();
        }
        
        public IEnumerator<T> GetEnumerator() => _queue.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _queue.GetEnumerator();
    }
}