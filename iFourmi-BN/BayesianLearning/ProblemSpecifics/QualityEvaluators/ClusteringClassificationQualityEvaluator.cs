using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.ACO;
using iFourmi.DataMining.Model;
using iFourmi.DataMining.Algorithms;
using iFourmi.DataMining.ClassificationMeasures;

namespace iFourmi.BayesianLearning.ProblemSpecifics.QualityEvaluators
{
    public class ClusteringClassificationQualityEvaluator : ClusteringQualityEvaluator
    {
        #region Data Members
                
        private IClassificationAlgorithm _classificationAlgorithm;
        private DataMining.Data.Dataset _dataset;
        private  IClassificationQualityMeasure _measure;

        #endregion

        #region Propoerties

        public DataMining.Algorithms.IClassificationAlgorithm ClassificationAlgorithm
        {
            get { return _classificationAlgorithm; }
            set { _classificationAlgorithm = value; }
        }

        #endregion

        public DataMining.Data.Dataset Dataset
        {
            get { return this._dataset; }
            set { this._dataset = value; }
        }

        #region Constructor

        public ClusteringClassificationQualityEvaluator(DataMining.ClassificationMeasures.IClassificationQualityMeasure measure, IClassificationAlgorithm algorithm)
            :base(null)
        {
            this._classificationAlgorithm = algorithm;
            this._measure = measure;
        }

        #endregion

        public override void EvaluateSolutionQuality(Solution<ClusterExampleAssignment> solution)
        {
            double quality = 0;
            this.ClusteringSolution.SetClusterExampleAssignment(solution.ToList());
            IClassifier classifier = BayesianNetworks.Model.BayesianClusterMultinetClassifier.ConstructClusterBMNClassifier(this.ClusteringSolution, this._classificationAlgorithm, this._dataset);
            DataMining.ClassificationMeasures.ConfusionMatrix[] list = DataMining.ClassificationMeasures.ConfusionMatrix.GetConfusionMatrixes(classifier, this._dataset);
            quality = _measure.CalculateMeasure(list);
            solution.Quality = quality;

        }

        public override void EvaluateSolutionQuality(Solution<int> solution)
        {
            double quality = 0;
            this.ClusteringSolution.SetClusterExampleAssignment(solution.ToList());
            IClassifier classifier = BayesianNetworks.Model.BayesianClusterMultinetClassifier.ConstructClusterBMNClassifier(this.ClusteringSolution, this._classificationAlgorithm, this._dataset);
            DataMining.ClassificationMeasures.ConfusionMatrix[] list = DataMining.ClassificationMeasures.ConfusionMatrix.GetConfusionMatrixes(classifier, this._dataset);
            quality = _measure.CalculateMeasure(list);
            solution.Quality = quality;

        }
    }
}
