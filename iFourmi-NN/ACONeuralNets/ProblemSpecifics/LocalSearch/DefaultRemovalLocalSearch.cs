using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.ACO;

namespace iFourmi.ACONeuralNets.ProblemSpecifics.LocalSearch
{
    public class DefaultRemovalLocalSearch<T>:ILocalSearch<T>
    {
        #region Data Members

        public ISolutionQualityEvaluator<T> _qualityEvaluator;

        #endregion

        #region Properties

        public ConstructionGraph<T> ConstructionGraph
        {
            get;
            set;
        }


        #endregion

        #region Constructor

        public DefaultRemovalLocalSearch(ISolutionQualityEvaluator<T> qualityEvaluator)
        {
            this._qualityEvaluator = qualityEvaluator;
        }

        #endregion

        public void PerformLocalSearch(Ant<T> ant)
        {
            Solution<T> solution = ant.Solution;

            double bestQuality = solution.Quality;

            for (int elementIndex = solution.Components.Count - 1; elementIndex >= 0; elementIndex--)
            {
                DecisionComponent<T> remove = solution.Components[elementIndex];
                double previousQuality = solution.Quality;

                solution.Components.RemoveAt(elementIndex);
                this._qualityEvaluator.EvaluateSolutionQuality(solution);
                if (solution.Quality >= bestQuality)
                {
                    bestQuality = solution.Quality;
                    ant.Trail[elementIndex] = -1;
                }
                else
                {
                    solution.Components.Insert(elementIndex, remove);
                    solution.Quality = previousQuality;
                }

            }
        }

       
    }
}
