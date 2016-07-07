using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.DataMining.Model;
using iFourmi.ACO;

namespace iFourmi.ACOMiner
{

    public class DRComponentInvalidator : IComponentInvalidator<DRComponent>
    {
        public void Invalidate(ACO.DecisionComponent<DRComponent> component, ACO.Solution<DRComponent> solution, ACO.ConstructionGraph<DRComponent> graph)
        {
            int index = component.Index;
            if (component.Element.Include)
                index += 2;
            else
                index += 1;

            for (int i = index - 2; i < index; i++)
                graph.Components[i].IsValid = false;

            if (index < graph.Components.Length)
                for (int i = index; i < index + 2; i++)
                    graph.Components[i].IsValid = true;
        }

    }
}
