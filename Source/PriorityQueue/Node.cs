using System;

namespace PriorityQueue
{
    public class Node<T> : IComparable<T> where T : IComparable<T>
    {
        public T Value { get; private set; }
        public Node<T> Next { get; set; }
        public Node<T> Prev { get; set; }

        public Node(T item)
        {
            Next = null;
            Prev = null;
            Value = item;
        }
        public Node()
        {
            Next = null;
            Prev = null;
        }

        public int CompareTo(T other)
        {
            return Value.CompareTo(other);
        }
        
    }
}