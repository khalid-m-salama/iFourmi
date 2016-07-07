using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.ACO.ProblemSpecifics
{
    public interface ILocalSearch<T>
    {
        void PerformLocalSearch(Ant<T> ant);
    }
}
