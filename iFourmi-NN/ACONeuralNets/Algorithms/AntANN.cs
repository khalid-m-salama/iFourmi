using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.ACONeuralNets.ProblemSpecifics.ComponentInvalidators;

namespace iFourmi.ACONeuralNets.Algorithms
{
    public class AntANN:Ant<ConnectionDC>
    {
        public AntANN(int index, ACO.AntColony<ConnectionDC> colony)
            : base(index, colony) { }
       
        public override void CreateSoltion()
        {
            NNConnectorInvalidator invalidator = (NNConnectorInvalidator)this._colony.Problem.ComponentInvalidator;

            for (int i = 0; i < 2; i++)
                this._colony.ConstructionGraph.Components[i].IsValid = true;

            int length = 0;
            int position = 0;

            while (true)
            {  

                List<DecisionComponent<ConnectionDC>> validElements = null;

                if (!invalidator.SecondPhase)
                    validElements = this._colony.ConstructionGraph.CalculateProbabilities(this._alpha, this._beta, position, 2);
                else
                {
                    length = this._colony.ConstructionGraph.Components.Length - position;
                    validElements = this._colony.ConstructionGraph.CalculateProbabilities(this._alpha, this._beta, position, length);
                }
                
                if (validElements.Count == 0)
                    break;

                DecisionComponent<ConnectionDC> element = this.SelectElementProbablistically(validElements);
                if (element == null)
                    break;
                this._solution.Components.Add(element);
                this._trail.Add(element.Index);

                if (!invalidator.SecondPhase)
                    position += 2;

                invalidator.Invalidate(element, this.Solution, this._colony.ConstructionGraph);
            }
        }
    }
}
