using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PriorityQueue
{
    public class PriorityQueue<T> where T : IComparable<T>
    {
        public Node<T> First { get; private set; }
        public Node<T> Last { get; private set; }
        public int Size { get; private set; }
        public bool Empty { get; private set; }

        private delegate int Cmp(T a, T b);

        private Cmp comparer;

        public PriorityQueue()
        {
            Size = 0;
            Empty = true;
            comparer = (T a, T b) => { return a.CompareTo(b); };
        }

        public PriorityQueue(IComparer<T> comp)
        {
            Size = 0;
            Empty = true;
            comparer = comp.Compare;
        }

        // public PriorityQueue<T> this[int index]
        // {
        //     get
        //     {
        //         int i = 0;
        //         Node<T> curNode = First;
        //         while (curNode.Next != null)
        //         {
        //             if (i == index)
        //                 return new Optional<T>();
        //             else
        //             {
        //                 i++;
        //                 curNode = curNode.Next;
        //             }
        //         }
        //     }
        //     set
        //     {
        //         
        //     }
        // }

        public void Add(T item)
        {
            if (!Empty)
            {
                int i = Size - 1;
                Node<T> cur = Last;
                while (i >= 0 && comparer(cur.Value, item) < 0)
                {
                    --i;
                    cur = cur.Prev;
                }

                if (i == -1)
                    InsertInFirst(new Node<T>(item));
                else if (i == Size - 1)
                    InsertInLast(new Node<T>(item));
                else
                    Insert(cur, new Node<T>(item));
            }
            else
                EmptyInsert(new Node<T>(item));
        }

        // Для вставки CircleEvent.
        public void Add(Node<T> node)
        {
            if (!Empty)
            {
                int i = Size - 1;
                Node<T> cur = Last;
                while (i >= 0 && comparer(cur.Value, node.Value) < 0)
                {
                    --i;
                    cur = cur.Prev;
                }

                if (i == -1)
                    InsertInFirst(node);
                else if (i == Size - 1)
                    InsertInLast(node);
                else
                    Insert(cur, node);
            }
            else
                EmptyInsert(node);
        }

        public Optional<T> Pop()
        {
            if (Empty)
                return new Optional<T>();
            else
            {
                Node<T> ans = First;
                Delete(First);
                return new Optional<T>(ans.Value);
            }
        }

        public void Delete(Node<T> deleted)
        {
            if (deleted != null)
            {
                if (deleted.Prev == null && deleted.Next == null)
                {
                    Empty = true;
                    First = null;
                    Last = null;
                }
                else if (deleted.Prev == null)
                {
                    First = deleted.Next;
                    deleted.Next.Prev = null;
                }
                else if (deleted.Next == null)
                {
                    Last = null;
                    deleted.Prev.Next = null;
                }
                else
                {
                    deleted.Prev.Next = deleted.Next;
                    deleted.Next.Prev = deleted.Prev;
                }
                Size--;
            }
        }

        private void InsertInFirst(Node<T> ins)
        {
            First.Prev = ins;
            ins.Next = First;
            First = ins;
            if (Size == 1)
            {
                First.Next = Last;
                Last.Prev = First;
            }

            ++Size;
        }

        private void InsertInLast(Node<T> ins)
        {
            Last.Next = ins;
            ins.Prev = Last;
            Last = ins;
            if (Size == 1)
            {
                First.Next = Last;
                Last.Prev = First;
            }

            ++Size;
        }

        private void Insert(Node<T> cur, Node<T> ins)
        {
            cur.Next.Prev = ins;
            ins.Next = cur.Next;
            ins.Prev = cur;
            cur.Next = ins;
            ++Size;
        }

        private void EmptyInsert(Node<T> ins)
        {
            First = ins;
            Last = ins;
            ++Size;
            Empty = false;
        }


        public bool Contains(T item)
        {
            int i = 0;
            Node<T> cur = Last;
            while (i < Size)
            {
                if (comparer(cur.Value, item) == 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}