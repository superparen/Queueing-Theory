using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueingSystem.Models
{
    public class Node<T>
    {
        public T Value { get; private set; }
        public List<Edge<T>> Edges { get; private set; }

        public Node(T value)
        {
            Value = value;
            Edges = new List<Edge<T>>();
        }

        public void AddEdge(Edge<T> edge)
        {
            Edges.Add(edge);
        }
    }
}
