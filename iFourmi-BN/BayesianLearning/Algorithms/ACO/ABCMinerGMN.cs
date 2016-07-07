using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.DataMining.Data;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.BayesianNetworks.Model;
using iFourmi.DataMining.Model;
using iFourmi.DataMining.Algorithms;
using iFourmi.BayesianLearning.ProblemSpecifics.HeuristicCalculators;


namespace iFourmi.BayesianLearning.Algorithms.ACO
{
    public class ABCMinerGMN: IClassificationAlgorithm
    {
        #region Data Members


        private int _maxIterations;
        private int _colonySize;
        private int _convergenceIterations;
        private int _currentIteration;
        private int _convergenceCounter;
        private Problem<Edge> _problem;
        private DataMining.ClassificationMeasures.IClassificationQualityMeasure _classificationQualityEvaluator;

        List<ACOB> _colonies;
        private List<Ant<Edge>> _bestAnts;
        private List<Ant<Edge>> _iterationBestAnts;
        private DataMining.Data.Dataset _trainingSet;
        private double _lastQuality;

        private Dataset[] _datasets;

        private int _maxDependencies;
        private double _iterationBestQuality;
        private double _bestQuality;
        
        private int[] _bestDependencies;
        private BayesianNetworks.Model.BayesianMultinetClassifier _mnClassifier;
        
        #endregion

        #region Events

        public event EventHandler OnPostAntSolutionContruction;
        public event EventHandler OnPostColonyIteration;

        #endregion

        #region Properties

        public BayesianMultinetClassifier MultinetBayesianClassifier
        {
            get { return this._mnClassifier; }
        }

        public Dataset Dataset
        {
            get { return this._trainingSet; }
            set { this._trainingSet = value; }

        }


        public int CurrentIteration
        {
            get { return this._currentIteration; }
        }

        public double IterationBestQuality
        {
            get { return this._iterationBestQuality; }
        }

        public double BestQuality
        {
            get { return this._bestQuality; }
        }

        #endregion

        #region Constructors


        public ABCMinerGMN(int maxIterations, int colonySize, int convergenceIterations, Problem<Edge> problem, int dependencies, Dataset trainingset)
        {
            this._colonySize = colonySize;
            this._maxIterations = maxIterations * trainingset.Metadata.Target.Values.Length;
            this._convergenceIterations = convergenceIterations;
            this._currentIteration = 0;
                   
            this._trainingSet = trainingset;
            this._problem = problem;
            this._datasets = this._trainingSet.Split();

            this._classificationQualityEvaluator = new DataMining.ClassificationMeasures.AccuracyMeasure();

            this._bestDependencies = new int[this._trainingSet.Metadata.Attributes.Length];
            this._maxDependencies = dependencies;
            
            this.Initialize();

          
            

        }

        #endregion


        #region Methods

        private void Initialize()
        {
            this._colonies = new List<ACOB>();

            for (int classIndex = 0; classIndex < this._trainingSet.Metadata.Target.Values.Length; classIndex++)
            {
                ACOB colony = new ACOB(this._maxIterations, this._colonySize, this._convergenceIterations, this._problem , this._maxDependencies, _datasets[classIndex]);
                ((CMICalculator)colony.Problem.HeuristicsCalculator).Dataset = this._datasets[classIndex];
                colony.Initialize();
                this._colonies.Add(colony);
            }
        }

        public void Work()
        {

            for (this._currentIteration = 0; this._currentIteration < this._maxIterations; this._currentIteration++)
            {
                this.CreateSolution();
                this.PerformLocalSearch();
                this.UpdateBestAnt();

                if (this.IsConverged())
                    break;

                this.UpdatePheromoneLevels();

                if (OnPostColonyIteration != null)
                    this.OnPostColonyIteration(this, null);
            }
          


            this._mnClassifier = this.ConstructMulitNetClassifier(this._bestAnts);
            
        }

        private void CreateSolution()
        {
            this._iterationBestAnts = null;
            this._iterationBestQuality = 0;

            for (int antIndex = 0; antIndex < this._colonySize; antIndex++)
            {              
                List<Ant<Edge>> currentAnts=new List<Ant<Edge>>();
                for (int classIndex = 0; classIndex < this._datasets.Length; classIndex++)
                {
                    this._colonies[classIndex].ConstructionGraph.ResetValidation();
                    this._colonies[classIndex].SetAntVariableMaxDependencies();

                    Ant<Edge> ant = new Ant<Edge>(antIndex,this._colonies[classIndex]);
                    ant.CreateSoltion();
                    currentAnts.Add(ant);
                }

                double currentQuality = this.EvaluateQuality(currentAnts);
             
                foreach (Ant<Edge> ant in currentAnts)
                    ant.Solution.Quality = currentQuality;

                if (this._iterationBestAnts != null)
                {
                    if (currentQuality > this._iterationBestQuality)
                    {
                        this._iterationBestQuality = currentQuality;
                        this._iterationBestAnts = currentAnts;

                        for (int classIndex = 0; classIndex < this._datasets.Length; classIndex++)
                            _bestDependencies[classIndex] = this._colonies[classIndex].CurrentDepenencies;               
                        
                    }
                }
                else
                {
                    this._iterationBestQuality  = currentQuality;
                    this._iterationBestAnts = currentAnts;

                    for (int classIndex = 0; classIndex < this._datasets.Length; classIndex++)
                        _bestDependencies[classIndex] = this._colonies[classIndex].CurrentDepenencies;
                }

                if (this.OnPostAntSolutionContruction != null)
                    this.OnPostAntSolutionContruction(currentAnts[0], null);
                
            }

        }

        private double EvaluateQuality(List<Ant<Edge>> ants)
        {
            BayesianMultinetClassifier classifier = this.ConstructMulitNetClassifier(ants);
            double quality = this._classificationQualityEvaluator.CalculateMeasure(DataMining.ClassificationMeasures.ConfusionMatrix.GetConfusionMatrixes(classifier, this._trainingSet));
            return quality;
        }

        private void UpdatePheromoneLevels()
        {
            for(int classIndex=0; classIndex< this._datasets.Length ; classIndex++)
            {
                this._iterationBestAnts[classIndex].DepositePheromone(0.5);
                //this._bestAnts[classIndex].DepositePheromone((double)this._currentIteration / (double)this._maxIterations);                
                this._colonies[classIndex].ConstructionGraph.EvaporatePheromone();

                this._colonies[classIndex].BestDependencies = _bestDependencies[classIndex];
                this._colonies[classIndex].UpdateVariableMaxDependencies(this._iterationBestAnts[classIndex]);
            }
          
        }

        private void UpdateBestAnt()
        {
            if (this._bestAnts != null)
            {
                if (_iterationBestAnts[0].Solution.Quality > this._bestQuality)
                {
                    this._bestAnts = new List<Ant<Edge>>();
                    foreach (Ant<Edge> ant in this._iterationBestAnts)
                        this._bestAnts.Add(ant.Clone() as Ant<Edge>);
                }

            }
            else
            {
                this._bestAnts = new List<Ant<Edge>>();
                foreach (Ant<Edge> ant in this._iterationBestAnts)
                    this._bestAnts.Add(ant.Clone() as Ant<Edge>);

            }


            this._bestQuality = this._bestAnts[0].Solution.Quality;
        }

        private void PerformLocalSearch()
        {
            double bestQuality = this._iterationBestAnts[0].Solution.Quality;

            for(int index=0; index< this._iterationBestAnts.Count; index++)
            {
                Ant<Edge> ant = this._iterationBestAnts[index];
                Solution<Edge> solution = ant.Solution;
                
                for (int elementIndex = solution.Components.Count - 1; elementIndex >= 0; elementIndex--)
                {
                    DecisionComponent<Edge> remove = solution.Components[elementIndex];
                    //double previousQuality = solution.Quality;

                    solution.Components.RemoveAt(elementIndex);

                    double currentQuality = this.EvaluateQuality(this._iterationBestAnts);

                    if (currentQuality >= bestQuality)
                    {
                        bestQuality = currentQuality;
                        ant.Trail[elementIndex] = -1;
                    }
                    else
                    {
                        solution.Components.Insert(elementIndex, remove);
                        //solution.Quality = previousQuality;
                    }

                }
            }

            foreach (Ant<Edge> ant in this._iterationBestAnts)
                ant.Solution.Quality = bestQuality;

        }
        
        private BayesianMultinetClassifier ConstructMulitNetClassifier(List<Ant<Edge>> ants)
        {
            BayesianMultinetClassifier mnClassifier = new BayesianMultinetClassifier(this._trainingSet.Metadata);
            for (int classIndex = 0; classIndex < this._trainingSet.Metadata.Target.Values.Length; classIndex++)
            {
                BayesianNetwork bayesianNetwork = new BayesianNetwork(this._datasets[classIndex].Metadata, ants[classIndex].Solution.ToList());
                bayesianNetwork.LearnParameters(this._datasets[classIndex]);
                mnClassifier.AddBayesianNetwork(classIndex, bayesianNetwork); 
            }
            return mnClassifier;
        }

        private bool IsConverged()
        {

            bool IsConverged = false;

            if (this._bestAnts[0].Solution.Quality == 1)
                return true;

            if (this._iterationBestAnts[0].Solution.Quality == this._lastQuality)
                this._convergenceCounter++;
            else
            {
                this._convergenceCounter = 0;
                this._lastQuality = this._iterationBestAnts[0].Solution.Quality; ;
            }

            if (_convergenceCounter == this._convergenceIterations)
                IsConverged = true;
            else
                IsConverged = false;

            if (this._bestAnts[0].Solution.Quality == 1)
                IsConverged = true;

            return IsConverged;

        }

        public IClassifier CreateClassifier()
        {
            this.Work();
            this._mnClassifier.Desc = "ABCMiner-" + this._trainingSet.Metadata.DatasetName;
            return this._mnClassifier;
        }

        #endregion



        
    }

   
}
