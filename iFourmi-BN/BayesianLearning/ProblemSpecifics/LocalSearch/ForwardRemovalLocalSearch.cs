using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.ACO;
using iFourmi.BayesianNetworks.Model;

namespace iFourmi.BayesianLearning.ProblemSpecifics.LocalSearch
{
    public class ForwardRemovalLocalSearch:ILocalSearch<Edge>
    {
        #region Data Members

        public ISolutionQualityEvaluator<Edge> _classificationQualityEvaluator;

        #endregion

        #region Proporties

        public ConstructionGraph<Edge> ConstructionGraph
        {
            get;
            set;
        }

        #endregion

        #region Constructor

        public ForwardRemovalLocalSearch(ISolutionQualityEvaluator<Edge> qualityEvaluator)
        {
            this._classificationQualityEvaluator = qualityEvaluator;
        }

        #endregion

        public void PerformLocalSearch(ACO.Ant<Edge> ant)
        {
            Solution<Edge> solution = ant.Solution;

            double bestQuality=solution.Quality;

            for (int elementIndex = 0; elementIndex < solution.Components.Count; elementIndex++)
            {
                DecisionComponent<Edge> remove=solution.Components[elementIndex];
                double previousQuality=solution.Quality;

                solution.Components.RemoveAt(elementIndex);
                this._classificationQualityEvaluator.EvaluateSolutionQuality(solution);
                if (solution.Quality >= bestQuality)
                {
                    bestQuality = solution.Quality;
                    ant.Trail[elementIndex] = -1;
                }
                else
                {
                    solution.Components.Insert(elementIndex,remove);
                    solution.Quality = previousQuality;
                }

            }


        }
    }
}
