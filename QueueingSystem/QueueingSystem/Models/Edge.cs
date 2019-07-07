using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueingSystem.Models
{
    public class Edge<T>
    {
        public Node<T> Node { get; private set; }
        public double Weight { get; private set; }

        public Edge(Node<T> node, double weight)
        {
            Node = node;
            Weight = weight;
        }
    }
}
