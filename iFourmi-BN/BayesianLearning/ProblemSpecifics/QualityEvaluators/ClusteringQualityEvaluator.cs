using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.ACO;
using iFourmi.DataMining.Model;

namespace iFourmi.BayesianLearning.ProblemSpecifics.QualityEvaluators
{
    public class ClusteringQualityEvaluator : ISolutionQualityEvaluator<ClusterExampleAssignment>, ISolutionQualityEvaluator<int>
    {
        #region Data Members

        private ClusteringSolution _clusteringSolution;
        private DataMining.ProximityMeasures.IClusteringQualityMeasure _measure;

        #endregion

        #region Propoerties

        public ClusteringSolution ClusteringSolution
        {
            get { return this._clusteringSolution; }
            set { this._clusteringSolution = value; }
        }

        #endregion

        #region Constructor

        public ClusteringQualityEvaluator(DataMining.ProximityMeasures.IClusteringQualityMeasure evaluator)
        {                        
            this._measure = evaluator;
        }

        #endregion

        public virtual void EvaluateSolutionQuality(Solution<ClusterExampleAssignment> solution)
        {
            double quality = 0;
            this._clusteringSolution.SetClusterExampleAssignment(solution.ToList());
            quality = _measure.CalculateQuality(this._clusteringSolution);
            solution.Quality = quality;
           
        }

        public virtual void EvaluateSolutionQuality(Solution<int> solution)
        {
            double quality = 0;
            this._clusteringSolution.SetClusterExampleAssignment(solution.ToList());
            quality = _measure.CalculateQuality(this._clusteringSolution);
            solution.Quality = quality;
        }
    }
}
