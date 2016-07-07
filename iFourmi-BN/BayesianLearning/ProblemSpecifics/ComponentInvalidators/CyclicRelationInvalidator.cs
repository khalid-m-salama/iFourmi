using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.BayesianNetworks.Model;


namespace iFourmi.BayesianLearning.ProblemSpecifics.ComponentInvalidators
{
    public class CyclicRelationInvalidator : IComponentInvalidator<Edge>
    {
        #region Data Members
        private int _maxDependencies;
        #endregion

        public int MaxDependencies
        {
            get { return this._maxDependencies; }
            set { this._maxDependencies = value; }
        }

     
        

        public void Invalidate(ACO.DecisionComponent<Edge> input, Solution<Edge> solution, ACO.ConstructionGraph<Edge> graph)
        {
            int dependencies = 0;
            foreach (DecisionComponent<Edge> solutionElement in solution.Components)
            {
                if (input.Element.ChildIndex == solutionElement.Element.ChildIndex)
                    dependencies++;
            }

            List<int> descendantIndexes = this.GetDescendantIndexes(input.Element, solution);
            List<int> ancestorIndexes = this.GetAncestorIndexes(input.Element , solution);



            foreach (DecisionComponent<Edge> component in graph.GetValidComponents())
            {
                if (component.Element.ChildIndex == input.Element.ChildIndex && component.Element.ParentIndex == input.Element.ParentIndex)
                    component.IsValid = false;

                if (component.Element.ChildIndex == input.Element.ChildIndex)
                {
                    if (dependencies == this._maxDependencies)
                        component.IsValid = false;

                }

                if (descendantIndexes.Contains(component.Element.ParentIndex))
                {
                    if (ancestorIndexes.Contains(component.Element.ChildIndex))
                        component.IsValid = false;

                }

                if (ancestorIndexes.Contains(component.Element.ChildIndex))
                {
                    if (descendantIndexes.Contains(component.Element.ParentIndex))
                        component.IsValid = false;

                }

            }



        }

        private List<int> GetAncestorIndexes(Edge input, Solution<Edge> solution)
        {
            List<int> indexes = new List<int>();
            Stack<int> stack = new Stack<int>();
            stack.Push(input.ChildIndex);

            while (stack.Count != 0)
            {
                int index = stack.Pop();
                indexes.Add(index);

                foreach (DecisionComponent<Edge> element in solution.Components)
                {
                    if (element.Element.ChildIndex == index)
                        stack.Push(element.Element.ParentIndex);
                }
            }

            return indexes;
        }


        private List<int> GetDescendantIndexes(Edge input, Solution<Edge> solution)
        {
            List<int> indexes = new List<int>();
            Stack<int> stack = new Stack<int>();
            stack.Push(input.ParentIndex);

            while (stack.Count != 0)
            {
                int index = stack.Pop();
                indexes.Add(index);

                foreach (DecisionComponent<Edge> element in solution.Components)
                {
                    if (element.Element.ParentIndex == index)
                        stack.Push(element.Element.ChildIndex);
                }
            }

            return indexes;
        }
    }
}
