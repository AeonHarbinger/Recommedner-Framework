using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RecommenderFramework
{
    /// <summary>
    /// Represents a node of linked list.
    /// </summary>
    /// <typeparam name="T">Type of value stored in node.</typeparam>
    class Node<T>
    {
        public T Item;
        public Node<T> Next;

        /// <summary>
        /// Creates new instance of node with given value stored inside.
        /// </summary>
        /// <param name="item">Item to be stored in node.</param>
        public Node(T item)
        {
            Item = item;
        }
    }

    /// <summary>
    /// Represents a thread safe linked list.
    /// </summary>
    /// <typeparam name="T">Type of value stored in list.</typeparam>
    public class SafeLinkedList<T>
    {
        Node<T> last;
        readonly Node<T> head;

        /// <summary>
        /// Creates new instance of SafeLinkedList.
        /// </summary>
        public SafeLinkedList()
        {
            head = new Node<T>(default(T));
            last = head;
        }

        /// <summary>
        /// Adds an item to the end of LinkedList.
        /// </summary>
        /// <param name="item">Item to be added.</param>
        public void Add(T item)
        {
            var node = new Node<T>(item);
            Node<T> lst;
            do
            {
                lst = last;
                node.Next = lst;
            }
            while (lst != Interlocked.CompareExchange(ref last, node, lst));
        }
        
        /// <summary>
        /// Returns values stored in this LinkedList.
        /// </summary>
        /// <returns>List containing values from this list.</returns>
        public List<T> Values()
        {
            List<T> list = new List<T>();
            var node = last;
            while (node != head)
            {
                list.Add(node.Item);
                node = node.Next;
            }

            list.Reverse();
            return list;
        }
    }    
}
