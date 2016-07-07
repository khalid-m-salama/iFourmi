using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.BayesianNetworks.Model;
using iFourmi.DataMining.Data;
using iFourmi.ACO.ProblemSpecifics;

namespace iFourmi.BayesianLearning.Algorithms.GHC
{
    public class K2Algorithm : AntColony<Edge>,DataMining.Algorithms.IClassificationAlgorithm
    {
        #region Data Members

        private int _maxEvaluations;
        private int _evaluationsCounter;
        private BayesianNetworks.Model.BayesianNetworkClassifier _bnclassifier;
        private DataMining.Data.Dataset _trainingSet;
        private DataMining.Data.Dataset _validationSet;
        private Solution<Edge> _solution;
        private bool _stop = false;
        private double[] _variablesK2;
        private BayesianNetworks.Utilities.K2Calculator _K2Calculator;
        private BayesianNetworks.Model.BayesianNetworkClassifier _bayesianNetwork;

        #endregion

        #region Properties

        public int EvaluationsCounter
        {
            get { return this._evaluationsCounter; }
        }

        public BayesianNetworkClassifier BayesianNetworkClassifier
        {
            get { return this._bnclassifier; }
        }

        public double[] VariablesK2
        {
            get { return this._variablesK2; }
        }

        public Solution<Edge> BestSolution
        {
            get { return this._solution; }
        }

        public Dataset Dataset
        {
            get
            {
                return this._trainingSet;
            }
            set
            {
                this._trainingSet = value;
            }
        }


        #endregion

        #region Events

        public event EventHandler OnPostEvaluation;
        public event EventHandler OnProgress;

        #endregion


        #region Constructor

        public K2Algorithm(int maxIterations, int colonySize, int convergenceIterations, Problem<Edge> problem, int maxEvaluations, Dataset trainingset, Dataset validationSet)
            : base(maxIterations, colonySize, convergenceIterations, BuildConstructionGraph(trainingset.Metadata), problem)
        {

            this._maxEvaluations = maxEvaluations;
            this._solution = new Solution<Edge>();
            this._trainingSet = trainingset;
            this._validationSet = validationSet;
            this._variablesK2 = new double[this._trainingSet.Metadata.Attributes.Length];
            this._K2Calculator = new BayesianNetworks.Utilities.K2Calculator();
            this._bayesianNetwork = new BayesianNetworkClassifier(trainingset.Metadata);



        }

        public static ConstructionGraph<Edge> BuildConstructionGraph(Metadata metadata)
        {
            int counter = 0;
            List<DecisionComponent<Edge>> decisionComponents = new List<DecisionComponent<Edge>>();
            for (int parentIndex = 0; parentIndex < metadata.Attributes.Length; parentIndex++)
                for (int childIndex = 0; childIndex < metadata.Attributes.Length; childIndex++)
                    if (parentIndex != childIndex)
                        decisionComponents.Add(new DecisionComponent<Edge>(counter++, new Edge(parentIndex, childIndex)));

            return new ConstructionGraph<Edge>(decisionComponents.ToArray());
        }


        #endregion

        #region Methods

        public override void Work()
        {
            this.CreateSolution();

            this._bnclassifier = new BayesianNetworks.Model.BayesianNetworkClassifier(_trainingSet.Metadata, _solution.ToList());
            this._bnclassifier.LearnParameters(this._trainingSet);

        }

        public override void CreateSolution()
        {
            while (true)
            {
                List<DecisionComponent<Edge>> validComponents = this.ConstructionGraph.GetValidComponents();
                if (validComponents.Count == 0 || this._stop)
                    break;

                DecisionComponent<Edge> component = this.SelectBestComponent(validComponents);
                if (component == null)
                    break;

                this._bayesianNetwork.Connect(component.Element.ParentIndex, component.Element.ChildIndex);

                this._solution.Components.Add(component);
                this._solution.Quality = this._variablesK2.Sum();

                this.Problem.ComponentInvalidator.Invalidate(component, this._solution, this.ConstructionGraph);

                if (this.OnProgress != null)
                    this.OnProgress(this, null);

            }
        }

        public BayesianNetwork LearnBayesianNetwork()
        {
            while (true)
            {
                List<DecisionComponent<Edge>> validComponents = this.ConstructionGraph.GetValidComponents();
                if (validComponents.Count == 0 || this._stop)
                    break;

                DecisionComponent<Edge> component = this.SelectBestComponent(validComponents);
                if (component == null)
                    break;

                this._bayesianNetwork.Connect(component.Element.ParentIndex, component.Element.ChildIndex);

                this._solution.Components.Add(component);
                this.EvaluateSolutionQuality(this._solution);
                

                this.Problem.ComponentInvalidator.Invalidate(component, this._solution, this.ConstructionGraph);

                if (this.OnProgress != null)
                    this.OnProgress(this, null);

            }

            BayesianNetwork network = new BayesianNetwork(this._trainingSet.Metadata, this._solution.ToList());
            network.LearnParameters(this._trainingSet);
            return network;

        }

        public override void EvaluateSolutionQuality(Solution<Edge> solution)
        {
            solution.Quality = this._variablesK2.Sum();
        }

        private DecisionComponent<Edge> SelectBestComponent(List<DecisionComponent<Edge>> components)
        {
            int bestIndex = 0;
            double bestQuality = 0.0;
            double bestK2 = 0.0;
            int currentIndex = 0;

            double solutionQuality = this._solution.Quality;

            foreach (DecisionComponent<Edge> component in components)
            {
                if (_evaluationsCounter == this._maxEvaluations)
                {
                    _stop = true;
                    break;
                }


                this._solution.Components.Add(component);
                this._bayesianNetwork.Connect(component.Element.ParentIndex, component.Element.ChildIndex);

                double temp = this._variablesK2[component.Element.ChildIndex];

                this._variablesK2[component.Element.ChildIndex] = _K2Calculator.Calculate(_bayesianNetwork.Variables[component.Element.ChildIndex], this._trainingSet);

                this._solution.Quality = this._variablesK2.Sum();

                if (this._solution.Quality > bestQuality)
                {
                    bestIndex = currentIndex;
                    bestQuality = this._solution.Quality;
                    bestK2 = this._variablesK2[component.Element.ChildIndex];
                }

                this._bayesianNetwork.Disconnect(component.Element.ParentIndex, component.Element.ChildIndex);
                this._variablesK2[component.Element.ChildIndex] = temp;
                this._solution.Components.RemoveAt(this._solution.Components.Count - 1);
                this._solution.Quality = solutionQuality;

                this._currentIteration++;
                this._evaluationsCounter++;


                if (this.OnPostEvaluation != null)
                    this.OnPostEvaluation(this, null);

                currentIndex++;
            }

            if (bestQuality == _solution.Quality)
                return null;

            this._variablesK2[components[bestIndex].Element.ChildIndex] = bestK2;
            return components[bestIndex];



        }


        public DataMining.Model.IClassifier CreateClassifier()
        {
            this.Work();
            return this._bnclassifier;
        }

        #endregion

    }
}
     