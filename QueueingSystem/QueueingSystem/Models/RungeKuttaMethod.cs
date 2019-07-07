using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueingSystem.Models
{
    public static class RungeKuttaMethod
    {
        public static double[] Calculate(Func<double[], double>[] funcs, double[] starts, double h, int N)
        {
            double[] nextParams = starts;
            for (int i = 0; i < N; i++)
            {
                double[] k1 = funcs.Select(f => h * f(nextParams)).ToArray(),
                    k2 = funcs.Select(f => h * f(nextParams.Select((p, ind) => p + k1[ind] / 2).ToArray())).ToArray(),
                    k3 = funcs.Select(f => h * f(nextParams.Select((p, ind) => p + k2[ind] / 2).ToArray())).ToArray(),
                    k4 = funcs.Select(f => h * f(nextParams.Select((p, ind) => p + k3[ind]).ToArray())).ToArray();

                for (int j = 0; j < nextParams.Length; j++)
                    nextParams[j] += (1d / 6d) * (k1[j] + 2 * k2[j] + 2 * k3[j] + k4[j]);
            }

            return nextParams;
        }
    }
}
