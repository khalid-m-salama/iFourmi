using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.ContinuousACO.ProblemSpecifics;
using iFourmi.DataMining.Algorithms;
using iFourmi.DataMining.Model.IBLearning;

namespace iFourmi.ContinuousACO.Algorithms
{
    public class AntIBMiner:ACO_R,IClassificationAlgorithm
    {
        #region Data Members

        protected DataMining.Data.Dataset _trainingSet;

        #endregion

        public new event EventHandler OnPostColonyIteration;

        public DataMining.Data.Dataset Dataset
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

        public AntIBMiner(int maxIterations, int colonySize, int convergenceIterations, Problem<double> problem, int problemSize, int archive, double q, double segma, DataMining.Data.Dataset trainingSet)
            :base(maxIterations,colonySize,convergenceIterations,problem,problemSize,archive,0,q,segma)
        {
            this._trainingSet = trainingSet;
            
        }

        public override void Work()
        {
                     
            for (this._currentIteration = 0; this._currentIteration < this._maxIterations; this._currentIteration++)
            {
                //if (this._currentIteration % 5 == 0)
                    //SetLearningAndValidationSets();

                this.CreateSolution();

                if (this.IsConverged())
                    break;

                this.UpdateBestAnt();

                this.UpdatePheromoneLevels();

                if (OnPostColonyIteration != null)
                    this.OnPostColonyIteration(this, null);

            }
        }


        private void SetLearningAndValidationSets()
        {
            DataMining.Data.Dataset[] datasets = this._trainingSet.SplitRandomly(0.8);
            ((IBClassificationQualityEvaluator)this.Problem.SolutionQualityEvaluator).LearningSet = datasets[0];
            ((IBClassificationQualityEvaluator)this.Problem.SolutionQualityEvaluator).ValidationSet = datasets[1];
        }

        public DataMining.Model.IClassifier CreateClassifier()
        {
            this.Work();
      
            this._problem.SolutionQualityEvaluator.EvaluateSolutionQuality(this.GlobalBest);
            AbstractLazyClassifier clasifier = ((IBClassificationQualityEvaluator)this._problem.SolutionQualityEvaluator).Classifier;
            
            return clasifier;
        }

        public DataMining.Model.Ensembles.EnsembleClassifier CreateEnsembleClassifier()
        {
            this.Work();
            List<DataMining.Model.ClassifierInfo> ensemble = new List<DataMining.Model.ClassifierInfo>();
            for (int i = 0; i < this._archiveSize; i++)
            {
                Solution<double> solution = Solution<double>.FromList(this._solutionArchive[i].ToList());
                this._problem.SolutionQualityEvaluator.EvaluateSolutionQuality(solution);
                AbstractLazyClassifier clasifier = ((IBClassificationQualityEvaluator)this._problem.SolutionQualityEvaluator).Classifier;
                DataMining.Model.ClassifierInfo classifierInfo = new DataMining.Model.ClassifierInfo() { Desc = clasifier.Desc + "-" + i.ToString(), Classifier = clasifier, Quality = solution.Quality };
                ensemble.Add(classifierInfo);
           
            }

            DataMining.Model.Ensembles.EnsembleClassifier ensmebleClassifier = new DataMining.Model.Ensembles.EnsembleClassifier(ensemble);
            return ensmebleClassifier;
        }
    }
}
