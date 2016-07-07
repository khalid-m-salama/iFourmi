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
    public class ADRMiner2 : IWekaClassificationAlgorithm
    {
        #region Data Members


        protected bool _attributeFirst;
        protected DataMining.Data.Dataset _trainingSet;
        protected bool _performLocalSearch;

        protected ADRMiner _aco1;
        protected ADRMiner _aco2;

        private Solution<DRComponent> _bestSolution;

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

        public ADRMiner ACO1
        {
            get { return this._aco1; }
        }

        public ADRMiner ACO2
        {
            get { return this._aco2; }
        }


        public Solution<DRComponent> BestSolution
        {
            get { return this._bestSolution; }
        }

        #endregion

        #region Constructors

        public ADRMiner2(int maxIterations, int colonySize, int convergenceIterations, Problem<DRComponent> problem, bool attributeFirst, bool performLocalSearch, Dataset trainingSet)
        {
            this.Dataset = trainingSet;

            this._attributeFirst = attributeFirst;
            this._performLocalSearch = performLocalSearch;

            if (attributeFirst)
            {
                _aco1 = new ADRMiner(maxIterations / 2, colonySize, convergenceIterations, problem, true, false, performLocalSearch, _trainingSet);
                _aco2 = new ADRMiner(maxIterations / 2, colonySize, convergenceIterations, problem, false, true, performLocalSearch, _trainingSet);
            }
            else
            {
                _aco1 = new ADRMiner(maxIterations / 2, colonySize, convergenceIterations, problem, false, true, performLocalSearch, _trainingSet);
                _aco2 = new ADRMiner(maxIterations / 2, colonySize, convergenceIterations, problem, true, false, performLocalSearch, _trainingSet);
            }
        }



        #endregion

        #region Methods

        public WekaClassifier CreateWekaClassifier()
        {            
            _aco1.Initialize();
            _aco1.Work();
            //_aco1.PostProcessing();

            Dataset reduced = null;
            if (_attributeFirst)
                reduced = _trainingSet.ReduceAttributes(ACO1.BestAnt.Solution.AttributesToRemove());
            else
                reduced = _trainingSet.ReduceInstances(ACO1.BestAnt.Solution.InstancesToRemove());
                

            _aco2.Initialize(_aco1.BestAnt.Solution);
            _aco2.Work();
            //_aco2.PostProcessing();



            this._bestSolution = ACO2.BestAnt.Solution;
            weka.classifiers.Classifier classifier = ((WekaClassificationQualityEvaluator)_aco1.Problem.SolutionQualityEvaluator).CreateClassifier(this._bestSolution);

            WekaClassifier wekaClassifier = new WekaClassifier();
            wekaClassifier.Classifier = classifier;
            wekaClassifier.AttributesToRemove = this._bestSolution.AttributesToRemove();

            return wekaClassifier;


        }

        public override string ToString()
        {

            return "ADRMiner+2 -" + _trainingSet.Metadata.DatasetName;
        }


        #endregion
    }

}
