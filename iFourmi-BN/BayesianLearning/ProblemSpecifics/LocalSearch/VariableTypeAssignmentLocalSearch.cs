using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.ACO;
using iFourmi.BayesianNetworks.Model;

namespace iFourmi.BayesianLearning.ProblemSpecifics.LocalSearch
{
    public class VariableTypeAssignmentLocalSearch : ILocalSearch<VariableTypeAssignment>
    {
        #region Data Members

        public ISolutionQualityEvaluator<VariableTypeAssignment> _classificationQualityEvaluator;
        public ConstructionGraph<VariableTypeAssignment> _constructionGraph;

        #endregion

        #region Properties

        public ConstructionGraph<VariableTypeAssignment> ConstructionGraph
        {
            get { return this._constructionGraph; }
            set {this._constructionGraph=value;}
        }

        #endregion

        #region Constructor

        public VariableTypeAssignmentLocalSearch(ISolutionQualityEvaluator<VariableTypeAssignment> qualityEvaluator)
        {
            this._classificationQualityEvaluator = qualityEvaluator;
         
        }

        #endregion


        public void PerformLocalSearch(Ant<VariableTypeAssignment> ant)
        {
            Solution<VariableTypeAssignment> solution = ant.Solution;

            double bestQuality = solution.Quality;

            for (int index =0; index < solution.Components.Count; index++)
            {
                int firstOptionIndex = solution.Components[index].Element.VariableIndex * 3;
                int currentOptionIndex = solution.Components[index].Index;

                for (int i = firstOptionIndex; i < firstOptionIndex + 3; i++)
                {
                    double previousQuality = solution.Quality;

                    if (i == currentOptionIndex)
                        continue;

                    DecisionComponent<VariableTypeAssignment> current = solution.Components[index];
                    solution.Components[index] = this._constructionGraph.Components[i];
                    this._classificationQualityEvaluator.EvaluateSolutionQuality(solution);

                    if (solution.Quality >= bestQuality)
                    {
                        bestQuality = solution.Quality;
                        ant.Trail[index] = i;
                    }

                    else
                    {
                        solution.Components[index] = current;
                        solution.Quality = previousQuality;
                    }
                }


            }
        }
    }
}
