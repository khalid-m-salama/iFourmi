using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;

namespace iFourmi.ACO.ProblemSpecifics
{
    public interface ISolutionQualityEvaluator<T>
    {
         void EvaluateSolutionQuality(Solution<T> solution);
    }
}
