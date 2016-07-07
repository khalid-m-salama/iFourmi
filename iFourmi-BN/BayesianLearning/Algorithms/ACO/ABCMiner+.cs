using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.BayesianNetworks.Model;
using iFourmi.BayesianLearning.ProblemSpecifics.ComponentInvalidators;
using iFourmi.BayesianLearning.ProblemSpecifics.HeuristicCalculators;
using iFourmi.BayesianLearning.ProblemSpecifics.LocalSearch;
using iFourmi.BayesianLearning.ProblemSpecifics.QualityEvaluators;
using iFourmi.DataMining.Algorithms;

namespace iFourmi.BayesianLearning.Algorithms.ACO
{
    public class ABCMinerPlus:IClassificationAlgorithm
    {
        #region Data Members

        private ABC abcAlgorithm;
        private ABCMiner abcminerAlgorithm;
        private DataMining.Data.Dataset _trainingSet;

        #endregion

        #region Propoerties
        public Dataset Dataset
        {
            get
            {
                return this._trainingSet;
            }
            set
            {
                this._trainingSet = value;
                this.abcAlgorithm.Dataset=this._trainingSet;
                this.abcminerAlgorithm.Dataset = this._trainingSet;
            }
        }

        public ABC ABCAlgorithm
        {
            get { return this.abcAlgorithm; }
        }

        public ABCMiner ABCMinerAlgorithm
        {
            get {return this.abcminerAlgorithm;}
        }
        

        #endregion

        #region Events

        public event EventHandler OnVariableTypeAssignmentCompleted;
     
        #endregion

        #region Constructors

        public ABCMinerPlus(int maxIterations, int colonySize, int convergenceIterations, Problem<Edge> abcMinerProblem,Problem<VariableTypeAssignment> abcProblem, int maxDependencies, Dataset trainingset, bool performLocalSearch)           
            :this(maxIterations,colonySize,convergenceIterations,abcMinerProblem,abcProblem,maxDependencies,performLocalSearch)
        {  
            this._trainingSet=trainingset;
            this.abcAlgorithm.Dataset = this._trainingSet;
            this.abcminerAlgorithm.Dataset = this._trainingSet;
        }

        public ABCMinerPlus(int maxIterations, int colonySize, int convergenceIterations, Problem<Edge> abcMinerProblem, Problem<VariableTypeAssignment> abcProblem, int maxDependencies, bool performLocalSearch)
        {
            this.abcAlgorithm = new ABC(maxIterations, colonySize, convergenceIterations, abcProblem, performLocalSearch);
            this.abcminerAlgorithm = new ABCMiner(maxIterations, colonySize, convergenceIterations, abcMinerProblem, maxDependencies,performLocalSearch);
            

            
        }

        #endregion

        #region Methods

        public IClassifier CreateClassifier()
        {
            abcAlgorithm.Initialize();
            abcAlgorithm.Work();
            List<VariableTypeAssignment> variableTypeAssignments= abcAlgorithm.BestAnt.Solution.ToList();
            OnVariableTypeAssignmentCompleted(abcAlgorithm.BayesianNetworkClassifier, null);
            this.abcminerAlgorithm.Initialize();
            this.abcminerAlgorithm.SetInputVariableTypes(variableTypeAssignments);
            this.abcminerAlgorithm.Work();
            return this.abcminerAlgorithm.BayesianNetworkClassifier;
        }

        #endregion
    }
}
