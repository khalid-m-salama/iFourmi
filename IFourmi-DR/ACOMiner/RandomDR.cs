using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iFourmi.ACO;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.DataMining;
using iFourmi.DataMining.Algorithms;
using iFourmi.DataMining.ClassificationMeasures;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;
using iFourmi.WekaNETBridge;

namespace iFourmi.ACOMiner
{
    public class RandomDR:AntColony<DRComponent>, IWekaClassificationAlgorithm
    {

        #region Data Members

     
        protected bool _useAttributes;
        protected bool _useInstances;
        protected DataMining.Data.Dataset _trainingSet;
        protected bool _performLocalSearch;
        protected Solution<DRComponent> _complement;

        #endregion

        #region Properties

        public Dataset Dataset
        {
            get { return this._trainingSet; }
            set
            {
                this._trainingSet = value;

            }
        }



        #endregion

        #region Constructors

        public RandomDR(int maxIterations, int colonySize, int convergenceIterations, Problem<DRComponent> problem, bool useAttributes, bool useInstances, bool performLocalSearch, Dataset trainingSet)
            : this(maxIterations, colonySize, convergenceIterations, problem, useAttributes, useInstances, performLocalSearch)
        {
            this.Dataset = trainingSet;
        }

        public RandomDR(int maxIterations, int colonySize, int convergenceIterations, Problem<DRComponent> problem, bool useAttributes, bool useInstances, bool performLocalSearch)
            : base(maxIterations, colonySize, convergenceIterations, problem)
        {
            this._useAttributes = useAttributes;
            this._useInstances = useInstances;
            this._performLocalSearch = performLocalSearch;
        }

        #endregion

        #region Events

        public event EventHandler OnPostAntSolutionContruction;
        public event EventHandler OnPostColonyIteration;

        #endregion
        
        #region Methods

        public override void Initialize()
        {

            this._graph = ConstructionGraphBuilder.BuildDRConstructionGraph(this.Dataset.Metadata, _useAttributes,_useInstances);
            this.ConstructionGraph.InitializePheromone(1);
            this.ConstructionGraph.SetHeuristicValues(this._problem.HeuristicsCalculator, false);
            this._bestAnt = null;
            this._iterationBestAnt = null;

        }

        public  void Initialize(Solution<DRComponent> complement)
        {
            this._graph = ConstructionGraphBuilder.BuildDRConstructionGraph(this.Dataset.Metadata, _useAttributes, _useInstances);
            this.ConstructionGraph.InitializePheromone(1);
            this.ConstructionGraph.SetHeuristicValues(this._problem.HeuristicsCalculator, false);
            this._complement = complement;
            this._bestAnt = null;
            this._iterationBestAnt = null;

        }

   

        public override void Work()
        {
            //SetLearningAndValidationSets();

            for (this._currentIteration = 0; this._currentIteration < this._maxIterations; this._currentIteration++)
            {
                //if (this._currentIteration % 5 == 0)
                    //SetLearningAndValidationSets();

                this.CreateSolution();

                if (this._performLocalSearch)
                    this.PerformLocalSearch(this._iterationBestAnt);

                this.UpdateBestAnt();

                if (this.IsConverged())
                    break;

                //this.UpdatePheromoneLevels();

                if (OnPostColonyIteration != null)
                    this.OnPostColonyIteration(this, null);
            }


        }

        public override void CreateSolution()
        {
            this._iterationBestAnt = null;

            //for (int antIndex = 0; antIndex < this.ColonySize; antIndex++)
            {
                this.ConstructionGraph.ResetValidation(false);

                for (int i = 0; i < 2; i++)
                    this.ConstructionGraph.Components[i].IsValid = true;

                Ant<DRComponent> ant = new Ant<DRComponent>(0, this);

                ant.CreateSoltion();


                if (this._complement != null)
                    ant.Solution = ExtensionMethodsUtility.Merge(ant.Solution, _complement);

                this.EvaluateSolutionQuality(ant.Solution);

                if (this._iterationBestAnt == null || ant.Solution.Quality > this._iterationBestAnt.Solution.Quality)                
                    this._iterationBestAnt = ant;
                

                if (this.OnPostAntSolutionContruction != null)
                    this.OnPostAntSolutionContruction(ant, null);
            }
        }



        public override void EvaluateSolutionQuality(Solution<DRComponent> solution)
        {
            this._problem.SolutionQualityEvaluator.EvaluateSolutionQuality(solution);
        }

        protected override void UpdateBestAnt()
        {
            if (this._bestAnt == null || _iterationBestAnt.Solution.Quality > this._bestAnt.Solution.Quality)            
                this._bestAnt = _iterationBestAnt.Clone() as Ant<DRComponent>;

            
        }

      

        public override void PostProcessing()
        {
            this._problem.LocalSearch.PerformLocalSearch(BestAnt);

        }



        public WekaClassifier CreateWekaClassifier()
        {
            
            this.Initialize();
            this.Work();
            weka.classifiers.Classifier classifier = ((WekaClassificationQualityEvaluator)this._problem.SolutionQualityEvaluator).CreateClassifier(this.BestAnt.Solution);

            WekaClassifier wekaClassifier = new WekaClassifier();
            wekaClassifier.Classifier = classifier;
            wekaClassifier.AttributesToRemove = this.BestAnt.Solution.AttributesToRemove();

            return wekaClassifier;
        }


        public override string ToString()
        {
            
            return "RandomDR -" + _trainingSet.Metadata.DatasetName;
        }

        #endregion
    }

}
