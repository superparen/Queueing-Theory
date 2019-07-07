using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using QueueingSystem.Models;
using LiveCharts.Wpf;
using LiveCharts;
using LiveCharts.Defaults;

namespace QueueingSystem
{
    class MainView : INotifyPropertyChanged
    {
        SeriesCollection imitateSeries;
        public SeriesCollection ImitateSeries
        {
            get => imitateSeries;
            set
            {
                imitateSeries = value;
                OnPropertyChanged();
            }
        }

        SeriesCollection analyticalSeries;
        public SeriesCollection AnalyticalSeries
        {
            get => analyticalSeries;
            set
            {
                analyticalSeries = value;
                OnPropertyChanged();
            }
        }

        SeriesCollection numericalSeries;
        public SeriesCollection NumericalSeries
        {
            get => numericalSeries;
            set
            {
                numericalSeries = value;
                OnPropertyChanged();
            }
        }
        double tmax;
        public double TMax
        {
            get => tmax;
            set
            {
                tmax = value;
                OnPropertyChanged();
            }
        }

        RelayCommand imitateCommand;
        public ICommand ImitateCommand
        {
            get => imitateCommand;
        }
        public MainView()
        {
            LoadSystem();
            imitateCommand = new RelayCommand(o => Imitate());
            TMax = 10000;
        }

        SimulateSystem<int> SimulateSystem;
        void LoadSystem()
        {
            var Nodes = new Node<int>[3];
            Nodes[0] = new Node<int>(0);
            Nodes[1] = new Node<int>(1);
            Nodes[2] = new Node<int>(2);

            Nodes[0].AddEdge(new Edge<int>(Nodes[1], 0.02));
            Nodes[0].AddEdge(new Edge<int>(Nodes[2], 0.06));

            Nodes[1].AddEdge(new Edge<int>(Nodes[2], 0.02));
            Nodes[1].AddEdge(new Edge<int>(Nodes[0], 0.3));

            Nodes[2].AddEdge(new Edge<int>(Nodes[0], 0.3));

            SimulateSystem = new SimulateSystem<int>(Nodes);
        }

        Dictionary<int, Func<double, double>> GetAnalyticalResult()
        {
            Func<double, double> p0 = t => 0.21056 * Math.Exp(-0.38 * t) + 0.78944,
                p1 = t => 0.020846 * Math.Exp(-0.32 * t) - 0.07018 * Math.Exp(-0.38 * t) + 0.04934,
                p2 = t => -0.020846 * Math.Exp(-0.32 * t) - 0.140372 * Math.Exp(-0.38 * t) + 0.16122;

            return new Dictionary<int, Func<double, double>>() {
                { 0, p0 }, { 1, p1 }, { 2, p2 }
            };
        }

        Dictionary<int, double> GetNumericalResult(int n)
        {
            var res = RungeKuttaMethod.Calculate(new Func<double[], double>[]
            {
                prms => prms[0] * -0.08 + (1 - prms[0] - prms[1]) * 0.3 + prms[1] * 0.3,
                prms => prms[1] * -0.3 + (1 - prms[0] - prms[1]) * 0.02 + prms[0] * 0.06
            }, new double[] { 1, 0 }, 0.1, n);

            return new Dictionary<int, double>()
            {
                { 0, res[0] }, { 2, res[1] }, { 1, 1d - res[0] - res[1] }
            };
        }

        void Imitate()
        {
            var result = SimulateSystem.Simulate(TMax);
            var analyticalResult = GetAnalyticalResult();

            var grouped = result.GroupBy(nd => nd.Key);       
            int ind = result[SimulateSystem.Nodes[0]].Count < 100 ? 1 : result[SimulateSystem.Nodes[0]].Count / 100,
                n = result[SimulateSystem.Nodes[0]].Count - 1;

            ImitateSeries = new SeriesCollection();
            foreach (var node in grouped)
            {
                Series series = new LineSeries() { Title = "Imitate P" + node.Key.Value, PointGeometrySize = 0 };
                series.Values = new ChartValues<ObservablePoint>(
                    result[node.Key].Where((el, i) => i % ind == 0).Select(el => new ObservablePoint(el.Tau, el.Value / el.Tau)));

                ImitateSeries.Add(series);
            }

            AnalyticalSeries = new SeriesCollection();
            foreach (var node in grouped)
            {
                Series series = new LineSeries() { Title = "Analytical P" + node.Key.Value, PointGeometrySize = 0 };
   
                series.Values = new ChartValues<ObservablePoint>(
                    result[node.Key].Where((el, i) => i % ind == 0).Select(el => new ObservablePoint(el.Tau, analyticalResult[node.Key.Value](el.Tau))));
                series.Values.Insert(0, new ObservablePoint(0, analyticalResult[node.Key.Value](0)));

                AnalyticalSeries.Add(series);
            }

            NumericalSeries = new SeriesCollection();
            foreach (var node in grouped)
            {
                Series series = new LineSeries() { Title = "Numerical P" + node.Key.Value, PointGeometrySize = 0 };

                series.Values = new ChartValues<ObservablePoint>(
                    result[node.Key].Where((el, i) => i % ind == 0).Select((el, i) => new ObservablePoint(el.Tau, GetNumericalResult(i * ind + 1)[node.Key.Value])));
                series.Values.Insert(0, new ObservablePoint(0, analyticalResult[node.Key.Value](0)));

                NumericalSeries.Add(series);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
