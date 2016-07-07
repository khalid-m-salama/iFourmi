using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.ACO;

namespace iFourmi.ACOMiner
{
    public class DRLocalSearch : ILocalSearch<DRComponent>
    {
        #region Data Members

        public ISolutionQualityEvaluator<DRComponent> _qualityEvaluator;

        #endregion

        #region Properties

        public ConstructionGraph<DRComponent> ConstructionGraph
        {
            get;
            set;
        }


        #endregion

        #region Constructor

        public DRLocalSearch(ISolutionQualityEvaluator<DRComponent> qualityEvaluator)
        {
            this._qualityEvaluator = qualityEvaluator;
        }

        #endregion

        public void PerformLocalSearch(Ant<DRComponent> ant)
        {
            Solution<DRComponent> solution = ant.Solution;

            double bestQuality = solution.Quality;

            for (int elementIndex = 0; elementIndex < solution.Components.Count; elementIndex++)
            {
                DecisionComponent<DRComponent> toChange = solution.Components[elementIndex];
                double previousQuality = solution.Quality;

                toChange.Element.Include = !toChange.Element.Include;
                this._qualityEvaluator.EvaluateSolutionQuality(solution);
                if (solution.Quality >= bestQuality)
                    bestQuality = solution.Quality;
                else
                {
                    toChange.Element.Include = !toChange.Element.Include;
                    solution.Quality = previousQuality;
                }

            }
        }


    }
}
