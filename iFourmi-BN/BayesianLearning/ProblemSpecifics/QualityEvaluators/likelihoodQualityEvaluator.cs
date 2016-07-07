using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.ACO;
using iFourmi.BayesianNetworks.Model;
using iFourmi.BayesianLearning.Algorithms.ACO;
using iFourmi.BayesianLearning.Algorithms.GHC;


namespace iFourmi.BayesianLearning.ProblemSpecifics.QualityEvaluators
{
    public class likelihoodQualityEvaluator: ISolutionQualityEvaluator<Edge>
    {
        #region Data Members

        private DataMining.Data.Dataset _trainigSet;
        private DataMining.Data.Dataset _validationSet;
        private DataMining.ClassificationMeasures.IClassificationQualityMeasure _evaluator;
        private int _targetClass;

        #endregion

        #region Properties

        public int TaregtClass
        {
            get { return this._targetClass; }
            set { this._targetClass = value; }

        }

        public double QualityFactor
        {
            get;
            set;
        }

        public DataMining.Data.Dataset TrainingSet
        {
            get { return this._trainigSet; }
            set { this._trainigSet = value; }
        }




        #endregion

        #region Constructor

        public likelihoodQualityEvaluator(DataMining.Data.Dataset validationSet)
        {
            this._validationSet = validationSet;            
        }

        #endregion

        public void EvaluateSolutionQuality(Solution<Edge> solution)
        {
            double positiveLikelihood = 0;
            double negativeLikelihood = 0;
            double sumLikelihood = 0;
                        
            BayesianNetworks.Model.BayesianNetwork bayesianNetwork = new BayesianNetworks.Model.BayesianNetwork(this._trainigSet.Metadata, solution.ToList());
            bayesianNetwork.LearnParameters(this._trainigSet);

            foreach (DataMining.Data.Example example in this._validationSet)
            {
                if (example.Label == this._targetClass)                
                    positiveLikelihood += bayesianNetwork.GetProbability(example);                
                else                
                    negativeLikelihood += bayesianNetwork.GetProbability(example);
                
            }

            int positiveExamples = this._validationSet.Filter(this._targetClass).Count;
            int negativeExamples = this._validationSet.Size - positiveExamples;
                        
            positiveLikelihood /= positiveExamples;
            negativeLikelihood /= negativeExamples;
            
            if (QualityFactor == 0)
            {
                QualityFactor = 1;
                double value = positiveLikelihood;
                while (value < 1)
                {
                    QualityFactor *= 10;                   
                    value = positiveLikelihood * QualityFactor;
                    
                }
                QualityFactor /= 10;
            }
                          
            sumLikelihood = positiveLikelihood + negativeLikelihood;                        
            solution.Quality = (positiveLikelihood - negativeLikelihood) * QualityFactor;

            if (solution.Quality < 0)
                solution.Quality = 0;          

        }
    }
}
