using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;

namespace iFourmi.ACO.ProblemSpecifics
{
    public interface IHeuristicValueCalculator<T>
    {
       
         void CalculateHieristicValue(DecisionComponent<T> Element);
    }
}
