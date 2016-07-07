using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.BayesianLearning.Algorithms.ACO;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.DataMining.Model;
using iFourmi.BayesianNetworks.Model;

namespace iFourmi.BayesianLearning.ProblemSpecifics.ComponentInvalidators          
{
    public class VariableTypeAssigmentInvalidator : IComponentInvalidator<VariableTypeAssignment>
    {
        public void Invalidate(ACO.DecisionComponent<VariableTypeAssignment> component, ACO.Solution<VariableTypeAssignment> solution, ACO.ConstructionGraph<VariableTypeAssignment> graph)
        {
            int index = component.Element.VariableIndex;
            int elementIndex = (index + 1) * 3;

            for (int i = elementIndex - 3; i < elementIndex; i++)
                graph.Components[i].IsValid = false;

            if (elementIndex < graph.Components.Length)
                for (int i = elementIndex; i < elementIndex + 3; i++)
                    graph.Components[i].IsValid = true;
        }
    }
}
