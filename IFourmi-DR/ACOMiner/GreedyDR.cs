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
    public class GreedyDR:AntColony<DRComponent>, IWekaClassificationAlgorithm
    {

        #region Data Members

     
        protected bool _useAttributes;
        protected bool _useInstances;
        protected DataMining.Data.Dataset _trainingSet;

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

        public GreedyDR(int maxIterations, int colonySize, int convergenceIterations, Problem<DRComponent> problem, bool useAttributes, bool useInstances, Dataset trainingSet)
            : this(maxIterations, colonySize, convergenceIterations, problem, useAttributes, useInstances)
        {
            this.Dataset = trainingSet;
        }

        public GreedyDR(int maxIterations, int colonySize, int convergenceIterations, Problem<DRComponent> problem, bool useAttributes, bool useInstances)
            : base(maxIterations, colonySize, convergenceIterations, problem)
        {
            this._useAttributes = useAttributes;
            this._useInstances = useInstances;
        }

        #endregion

        #region Events

        public event EventHandler OnPostAntSolutionContruction;
        public event EventHandler OnPostColonyIteration;

        #endregion
        
        #region Methods

        public override void Initialize()
        {
            
            
        }

   

        public override void Work()
        {
            this.CreateSolution();
            this.EvaluateSolutionQuality(this._bestAnt.Solution);
            this.PostProcessing();         
            
        }

        public override void CreateSolution()
        {
            Solution<DRComponent> solution = new Solution<DRComponent>();
            if (this._useAttributes)
                for (int i = 0; i < this.Dataset.Metadata.Attributes.Length; i++)
                    solution.Components.Add(new DecisionComponent<DRComponent>(i, new DRComponent(DatasetElementType.Attribute, i, true)));

            if (this._useInstances)
                for (int i = 0; i < this.Dataset.Size; i++)
                    solution.Components.Add(new DecisionComponent<DRComponent>(i, new DRComponent(DatasetElementType.Instance, i, true)));

            this._bestAnt = new Ant<DRComponent>(-1, this);
            this._bestAnt.Solution = solution;
        }

        public override void EvaluateSolutionQuality(Solution<DRComponent> solution)
        {
            this._problem.SolutionQualityEvaluator.EvaluateSolutionQuality(solution); ;
        }

        public override void PostProcessing()
        {
            this._problem.LocalSearch.PerformLocalSearch(this._bestAnt); 
        }



        public WekaClassifier CreateWekaClassifier()
        {
            
            this.Work();
            weka.classifiers.Classifier classifier = ((WekaClassificationQualityEvaluator)this._problem.SolutionQualityEvaluator).CreateClassifier(this.BestAnt.Solution);

            WekaClassifier wekaClassifier = new WekaClassifier();
            wekaClassifier.Classifier = classifier;
            wekaClassifier.AttributesToRemove = this.BestAnt.Solution.AttributesToRemove();

            return wekaClassifier;
        }


        public override string ToString()
        {
            
            return "GreedyDR -" + _trainingSet.Metadata.DatasetName;
        }

        #endregion
    }

}
