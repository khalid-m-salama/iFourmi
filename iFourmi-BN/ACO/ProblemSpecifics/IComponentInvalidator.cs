using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.ACO.ProblemSpecifics
{
    public interface IComponentInvalidator<T>
    {
        void Invalidate(DecisionComponent<T> component,Solution<T> solution,ConstructionGraph<T> graph);
        
    }
}
