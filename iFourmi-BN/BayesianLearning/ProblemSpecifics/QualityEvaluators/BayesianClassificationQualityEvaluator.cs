using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.ACO;
using iFourmi.BayesianNetworks.Model;





namespace iFourmi.BayesianLearning.ProblemSpecifics.QualityEvaluators
{
    public class BayesianClassificationQualityEvaluator:ISolutionQualityEvaluator<Edge>,ISolutionQualityEvaluator<VariableTypeAssignment>
    {
        #region Data Members

        
        private DataMining.Data.Dataset _validationSet;
        private DataMining.Data.Dataset _learningSet;
        private DataMining.ClassificationMeasures.IClassificationQualityMeasure _measure;
        
        #endregion

        #region properties

        public DataMining.Data.Dataset LearningSet
        {
            get { return this._learningSet; }
            set { this._learningSet = value; }
        }

        public DataMining.Data.Dataset ValidationSet
        {
            get { return this._validationSet; }
            set { this._validationSet = value; }
        }

        public List<VariableTypeAssignment> VariableTypeAssignments
        {
            get;
            set;
        }

        public List<Edge> InputVariableDependencies
        {
            get;
            set;
        }


        #endregion

        #region Constructor

        public BayesianClassificationQualityEvaluator(DataMining.ClassificationMeasures.IClassificationQualityMeasure evaluator)
        {            
            
            this._measure = evaluator;
        }

        #endregion

        public void EvaluateSolutionQuality(Solution<Edge> solution)
        {
            double quality = 0;
            BayesianNetworks.Model.BayesianNetworkClassifier bnclassifier = null;

            if(VariableTypeAssignments==null)
                bnclassifier = new BayesianNetworks.Model.BayesianNetworkClassifier(this._validationSet.Metadata, solution.ToList());
            else
                bnclassifier = new BayesianNetworks.Model.BayesianNetworkClassifier(this._validationSet.Metadata, VariableTypeAssignments,solution.ToList());

            bnclassifier.LearnParameters(this._learningSet);

            DataMining.ClassificationMeasures.ConfusionMatrix[] list = DataMining.ClassificationMeasures.ConfusionMatrix.GetConfusionMatrixes(bnclassifier, _validationSet);
            quality = _measure.CalculateMeasure(list);
            
            //penalty
            int x = VariableTypeAssignments.FindAll(c => c.Type == VariableType.Cause).Count;
            double penality = Math.Pow(2, x - VariableTypeAssignments.Count);

            solution.Quality = quality - penality;


        }

        public void EvaluateSolutionQuality(Solution<VariableTypeAssignment> solution)
        {
            double quality = 0;
            BayesianNetworks.Model.BayesianNetworkClassifier bnclassifier=null;

            if(InputVariableDependencies==null)
                bnclassifier = new BayesianNetworks.Model.BayesianNetworkClassifier(this._validationSet.Metadata, solution.ToList());
            else
                bnclassifier = new BayesianNetworks.Model.BayesianNetworkClassifier(this._validationSet.Metadata, solution.ToList(),InputVariableDependencies);

            bnclassifier.LearnParameters(this._learningSet);

            DataMining.ClassificationMeasures.ConfusionMatrix[] list = DataMining.ClassificationMeasures.ConfusionMatrix.GetConfusionMatrixes(bnclassifier, _validationSet);
            quality = _measure.CalculateMeasure(list);

            //penalty
            int x = solution.Components.FindAll(c => c.Element.Type == VariableType.Cause).Count;
            double penality = Math.Pow(2,x - solution.Components.Count);

            solution.Quality = quality-penality;
        }
    }
}
