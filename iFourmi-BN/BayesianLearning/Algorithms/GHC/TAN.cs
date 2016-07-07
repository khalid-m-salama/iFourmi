using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.BayesianNetworks.Model;
using iFourmi.BayesianLearning;
using iFourmi.BayesianLearning.ProblemSpecifics.ComponentInvalidators;
using iFourmi.BayesianLearning.ProblemSpecifics.HeuristicCalculators;
using iFourmi.BayesianLearning.ProblemSpecifics.LocalSearch;
using iFourmi.BayesianLearning.ProblemSpecifics.QualityEvaluators;

namespace iFourmi.BayesianLearning.Algorithms.GHC
{
    public class TAN : iFourmi.DataMining.Algorithms.IClassificationAlgorithm
    {
        private iFourmi.DataMining.Data.Dataset _trainingset;

        private List<DecisionComponent<BayesianNetworks.Model.Edge>> decisionComponents;

        public iFourmi.DataMining.Data.Dataset Dataset
        {
            get
            {
                return this._trainingset;
            }
            set
            {
                this._trainingset = value;
                this.InitializeDecisionComponents();
            }
        }

        public TAN()
        {
        }

        public TAN(DataMining.Data.Dataset trainingset)
        {
            this._trainingset = trainingset;

            this.InitializeDecisionComponents();
        }

        private void InitializeDecisionComponents()
        {
            this.decisionComponents = new List<DecisionComponent<BayesianNetworks.Model.Edge>>();
        }

        public iFourmi.DataMining.Model.IClassifier CreateClassifier()
        {
            Solution<Edge> solution = new Solution<Edge>();
            CyclicRelationInvalidator invalidator = new CyclicRelationInvalidator();
            invalidator.MaxDependencies = 1;

            ConstructionGraph<Edge> constructionGraph = ConstructionGraphBuilder.BuildBNConstructionGraph(this._trainingset.Metadata);
            CMICalculator cmiCalculator = new CMICalculator();
            cmiCalculator.Dataset = this._trainingset;

            constructionGraph.SetHeuristicValues(cmiCalculator,false);
            List<DecisionComponent<Edge>> components = constructionGraph.Components.OrderByDescending(e => e.Heuristic).ToList();

            while (components.Count != 0)
            {
                DecisionComponent<Edge> component = components[0];
                solution.Components.Add(component);
                invalidator.Invalidate(component, solution, constructionGraph);
                components = constructionGraph.Components.Where(e => e.IsValid).OrderByDescending(e => e.Heuristic).ToList();
            }


            BayesianNetworkClassifier BNClassifier = new BayesianNetworks.Model.BayesianNetworkClassifier(this._trainingset.Metadata, solution.ToList());
            BNClassifier.LearnParameters(this._trainingset);
            return BNClassifier;
        }
    }
}
