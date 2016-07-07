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

    public class NNConnectorInvalidator_old : IComponentInvalidator<ConnectionDC>
    {
        public int InputUnitCount
        {
            get;
            set;
        }

        public int OutputUnitCount
        {
            get;
            set;
        }

        public int HiddenUnitCount
        {
            get;
            set;
        }


        public bool SecondPhase
        {
            get;
            set;
        }


        public int MaxParents
        {
            get;
            set;
        }


        public void Invalidate(ACO.DecisionComponent<ConnectionDC> component, ACO.Solution<ConnectionDC> solution, ACO.ConstructionGraph<ConnectionDC> graph)
        {
            int switchIndex=(InputUnitCount*HiddenUnitCount*2)+(InputUnitCount*OutputUnitCount*2)+(HiddenUnitCount*OutputUnitCount*2);

            if (SecondPhase)
                this.InvalidateLoops(component, solution, graph);

            else
            {
                int index = component.Index;
                if (component.Element.Include)
                    index += 2;
                else
                    index += 1;

                for (int i = index - 2; i < index; i++)
                    graph.Components[i].IsValid = false;

                if (index >= switchIndex)
                {
                    this.SecondPhase = true;
                    for (int i = index; i < graph.Components.Length; i++)
                        graph.Components[i].IsValid = true;

                }
                else
                {
                    for (int i = index; i < index + 2; i++)
                        graph.Components[i].IsValid = true;
                }
            }


        }

        private void InvalidateLoops(ACO.DecisionComponent<ConnectionDC> input, ACO.Solution<ConnectionDC> solution, ACO.ConstructionGraph<ConnectionDC> graph)
        {
            int dependencies = 0;

            int switchIndex = (InputUnitCount * HiddenUnitCount) + (InputUnitCount * OutputUnitCount) + (HiddenUnitCount * OutputUnitCount);
            int switchIndex2 = (InputUnitCount * HiddenUnitCount * 2) + (InputUnitCount * OutputUnitCount * 2) + (HiddenUnitCount * OutputUnitCount * 2);

            for(int index=switchIndex; index<solution.Components.Count; index++)            
            {
                DecisionComponent<ConnectionDC> component=solution.Components[index];
                if (input.Element.Connection.To == component.Element.Connection.To)
                    dependencies++;
            }

            List<int> descendantIndexes = this.GetDescendantIndexes(input.Element, solution);
            List<int> ancestorIndexes = this.GetAncestorIndexes(input.Element, solution);

            List<DecisionComponent<ConnectionDC>> validComponents = graph.GetValidComponents(switchIndex2, graph.Components.Length - switchIndex2);

            foreach (DecisionComponent<ConnectionDC> component in validComponents)
            {
                if (component.Element.Connection.To == input.Element.Connection.To && component.Element.Connection.From == input.Element.Connection.From)
                {
                    component.IsValid = false;
                    continue;
                }

                if (component.Element.Connection.To == input.Element.Connection.To)
                {
                    if (dependencies == MaxParents)
                    {
                        component.IsValid = false;
                        continue;
                    }

                }


                if (descendantIndexes.Contains(component.Element.Connection.From))
                {
                    if (ancestorIndexes.Contains(component.Element.Connection.To))
                    {
                        component.IsValid = false;
                        continue;
                    }

                }

                if (ancestorIndexes.Contains(component.Element.Connection.To))
                {
                    if (descendantIndexes.Contains(component.Element.Connection.From))
                    {
                        component.IsValid = false;
                        continue;
                    }

                }

            }



        }

        private List<int> GetDescendantIndexes(ConnectionDC input, Solution<ConnectionDC> solution)
        {
            int switchIndex = (InputUnitCount * HiddenUnitCount) + (InputUnitCount * OutputUnitCount) + (HiddenUnitCount * OutputUnitCount);

            List<int> indexes = new List<int>();
            Stack<int> stack = new Stack<int>();
            stack.Push(input.Connection.To);

            while (stack.Count != 0)
            {
                int index = stack.Pop();
                

                for (; switchIndex < solution.Components.Count; switchIndex++)                
                {
                    DecisionComponent<ConnectionDC> component = solution.Components[switchIndex];

                    if (component.Element.Connection.To == index)
                    {
                        stack.Push(component.Element.Connection.From);
                        indexes.Add(index);
                    }
                }
            }

            return indexes;
        }

        private List<int> GetAncestorIndexes(ConnectionDC input, Solution<ConnectionDC> solution)
        {
            int switchIndex = (InputUnitCount * HiddenUnitCount) + (InputUnitCount * OutputUnitCount) + (HiddenUnitCount * OutputUnitCount);

            List<int> indexes = new List<int>();
            Stack<int> stack = new Stack<int>();
            stack.Push(input.Connection.From);

            while (stack.Count != 0)
            {
                int index = stack.Pop();
                

                for (; switchIndex < solution.Components.Count; switchIndex++)
                {
                    DecisionComponent<ConnectionDC> component = solution.Components[switchIndex];

                    if (component.Element.Connection.From == index)
                    {
                        stack.Push(component.Element.Connection.To);
                        indexes.Add(index);
                    }
                }
            }

            return indexes;
        }
    }
}
