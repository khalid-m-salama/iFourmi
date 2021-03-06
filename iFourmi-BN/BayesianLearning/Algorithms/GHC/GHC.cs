﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.BayesianNetworks.Model;
using iFourmi.DataMining.Data;
using iFourmi.ACO.ProblemSpecifics;

namespace iFourmi.BayesianLearning.Algorithms.GHC
{
    public class GHC: AntColony<Edge>, DataMining.Algorithms.IClassificationAlgorithm
    {
        #region Data Members

        private int _maxEvaluations;
        private int _evaluationsCounter;        
        private BayesianNetworks.Model.BayesianNetworkClassifier _bnclassifier;        
        private DataMining.Data.Dataset _trainingSet;
        private DataMining.Data.Dataset _validationSet;
        private Solution<Edge> _solution;
        private bool _stop=false;

        #endregion

        #region Properties

        public int EvaluationsCounter
        {
            get { return this._evaluationsCounter; }
        }

        public Solution<Edge> BestSolution
        {
            get { return this._solution; }
        }

        public BayesianNetworkClassifier BayesianNetworkClassifier
        {
            get { return this._bnclassifier; }
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

        public GHC(int maxIterations, int colonySize, int convergenceIterations, Problem<Edge> problem, int maxEvaluations, Dataset trainingset, Dataset validationSet)
            : base(maxIterations, colonySize, convergenceIterations, BuildConstructionGraph(trainingset.Metadata), problem)
        {

            this._maxEvaluations = maxEvaluations;
            this._solution = new Solution<Edge>();
            this._trainingSet = trainingset;
           //(( ClassificationQualityEvaluator)this._problem.SolutionEvaluator).ValidationSet 
            
            
            

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

            this._bnclassifier = new BayesianNetworks.Model.BayesianNetworkClassifier(_trainingSet.Metadata , _solution.ToList());
            this._bnclassifier.LearnParameters(this._trainingSet);
            
        }

        public override void CreateSolution()
        {
            while (true)
            {
                List<DecisionComponent<Edge>> validElements = this.ConstructionGraph.GetValidComponents();
                if (validElements.Count == 0 || this._stop)
                    break;

                DecisionComponent<Edge> element = this.SelectBestElement(validElements);
                if (element == null)
                    break;

                this._solution.Components.Add(element);
                this.EvaluateSolutionQuality(this._solution);
                
                this.Problem.ComponentInvalidator.Invalidate(element, this._solution, this.ConstructionGraph);

                if (this.OnProgress != null)
                    this.OnProgress(this, null);

            }
            
        }

        public override void EvaluateSolutionQuality(Solution<Edge> solution)
        {
            this.Problem.SolutionQualityEvaluator.EvaluateSolutionQuality(solution);
        }

        private DecisionComponent<Edge> SelectBestElement(List<DecisionComponent<Edge>> elements)
        {
            int bestIndex = 0;
            double bestQuality = 0.0;
            int currentIndex=0; 
            
            double solutionQuality=this._solution.Quality;

            foreach (DecisionComponent<Edge> component in elements)
            {
                if (_evaluationsCounter == this._maxEvaluations)
                {
                    _stop = true;
                    break;
                }
                

                this._solution.Components.Add(component);
                this.Problem.SolutionQualityEvaluator.EvaluateSolutionQuality(this._solution);
                if (this._solution.Quality > bestQuality)
                {
                    bestIndex = currentIndex;
                    bestQuality = this._solution.Quality;
                }

                this._solution.Components.RemoveAt(this._solution.Components.Count - 1);
                this._solution.Quality=solutionQuality;

                this._currentIteration++;
                this._evaluationsCounter++;


                if (this.OnPostEvaluation != null)
                    this.OnPostEvaluation(this, null);

                currentIndex++;
            }

            if (bestQuality == _solution.Quality)                            
                return null;
            

            return elements[bestIndex];



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