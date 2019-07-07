using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueingSystem.Models
{
    public struct Section
    {
        public double Value { get; set; }
        public double Tau { get; set; }
    }
    public class SimulateSystem<T>
    {
        public Node<T>[] Nodes { get; private set; }
        public SimulateSystem(Node<T>[] nodes)
        {
            Nodes = nodes;
        }

        public Dictionary<Node<T>, List<Section>> Simulate(double Tmax, int defaultNode = 0)
        {
            Dictionary<Node<T>, List<Section>> res = new Dictionary<Node<T>, List<Section>>();
            foreach (var node in Nodes)
                res.Add(node, new List<Section>());

            Random random = new Random();
            double Tcurr = 0;
            Node<T> current = Nodes[defaultNode];
            while (Tcurr < Tmax)
            {
                var edge = current.Edges.Select(e => new { e.Node, Tau = 1 / e.Weight * Math.Log(1 / (1 - random.NextDouble())) }).OrderBy(e => e.Tau).ToArray()[0];

                res[current].Add(new Section() {
                    Tau = Tcurr + edge.Tau,
                    Value = res[current].Count == 0 ? edge.Tau : res[current].Last().Value + edge.Tau });
                foreach (var el in res.Where(r => r.Key != current))
                    el.Value.Add(new Section() {
                        Tau = Tcurr + edge.Tau,
                        Value = res[el.Key].Count == 0 ? 0 : res[el.Key].Last().Value
                    });
                Tcurr += edge.Tau;
                current = edge.Node;
            }

            return res;
        }
    }
}
