using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;

namespace iFourmi.ACO.ProblemSpecifics
{
    public interface IHeuristicsCalculator<T>
    {
        DataMining.Data.Dataset Dataset
        {
            get;
            set;
        }
         void CalculateHeuristics(DecisionComponent<T> component);
    }
}
