using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACONeuralNets.Algorithms;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.DataMining.Model;
using iFourmi.ACO;

namespace iFourmi.ACONeuralNets.ProblemSpecifics.ComponentInvalidators
{

    public class NNConnectorInvalidator : IComponentInvalidator<ConnectionDC>
    {
        public void Invalidate(ACO.DecisionComponent<ConnectionDC> component, ACO.Solution<ConnectionDC> solution, ACO.ConstructionGraph<ConnectionDC> graph)
        {
            int index = component.Index;
            if (component.Element.Include)
                index += 2;
            else
                index += 1;

            for (int i = index - 2; i < index; i++)
                graph.Components[i].IsValid = false;

            if(index<graph.Components.Length)
                for (int i = index; i < index + 2; i++)
                    graph.Components[i].IsValid = true;
        }

    }
}
