

namespace PriorityQueue
{
    public class Optional<T>
    {
        public T Value;
        public bool Empty;

        public Optional(T item)
        {
            Value = item;
            Empty = false;
        }
        public Optional()
        {
            Empty = true;
        }
    }
}