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
using iFourmi.DataMining.ProximityMeasures;
using iFourmi.DataMining.ClassificationMeasures;

namespace iFourmi.BayesianLearning.Algorithms.ACO
{
    public class AntClustBMN_MB : ACOClustering_MB, IClassificationAlgorithm 
    {
        #region Data Member

        private IClassificationQualityMeasure _classificationQualityMeasure;
        private IClassificationAlgorithm _classificationAlgorithm;
        private BayesianClusterMultinetClassifier _BMNClassifier;

        #endregion

        #region Properties

        public BayesianClusterMultinetClassifier BMNClassifier
        {
            get
            {
                return this._BMNClassifier;
            }
        }

        #endregion
        
        #region Constructors

        public AntClustBMN_MB(int maxIterations, int colonySize, int convergenceIterations, Problem<int> problem, int clustersNumber, DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure, Dataset dataset, bool performLocalSearch,IClassificationAlgorithm  algorithm,IClassificationQualityMeasure qualityMeasure)
            : base(maxIterations,colonySize,convergenceIterations,problem,clustersNumber,similarityMeasure,dataset,performLocalSearch)
        {
            this._classificationAlgorithm = algorithm;
            this._classificationQualityMeasure = qualityMeasure;
        }


        public AntClustBMN_MB(int maxIterations, int colonySize, int convergenceIterations, Problem<int> problem, int clustersNumber, DataMining.ProximityMeasures.ISimilarityMeasure similarityMeasure, bool performLocalSearch, IClassificationAlgorithm algorithm, IClassificationQualityMeasure qualityMeasure)
            : base(maxIterations, colonySize, convergenceIterations, problem, clustersNumber, similarityMeasure, performLocalSearch)
        {
            this._classificationAlgorithm = algorithm;
            this._classificationQualityMeasure = qualityMeasure;
        }

        #endregion
        
        #region Mehods

        public IClassifier CreateClassifier()
        {
            this.Initialize();
            this.Work();
            this.PostProcessing();
            this._clusteringSolution.SetClusterExampleAssignment(this._bestAnt.Solution.ToList());
            this._BMNClassifier = BayesianClusterMultinetClassifier.ConstructClusterBMNClassifier(this._clusteringSolution, this._classificationAlgorithm, this.Dataset);
            return this._BMNClassifier;
        }

        public override string ToString()
        {
            return "AntClustBMN_MB:" + "k=" + this._clustersNumber.ToString() + "-" + _dataset.Metadata.DatasetName;
        }

        #endregion
    }
}
