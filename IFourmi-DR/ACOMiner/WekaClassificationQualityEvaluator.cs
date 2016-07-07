using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.ACO;


namespace iFourmi.ACOMiner
{
    public class WekaClassificationQualityEvaluator : ISolutionQualityEvaluator<DRComponent>
    {
        #region Data Members

        //private DataMining.Data.Dataset _learningSet;
        //private DataMining.Data.Dataset _validationSet;
        //private DataMining.ClassificationMeasures.IClassificationMeasure _measure;

        private bool _skipAttibutesValidation = true;

        #endregion

        #region Properties


        public WekaNETBridge.WekaClassification WekaClassification
        {
            get;
            set;
        }
        
        //public DataMining.Data.Dataset LearningSet
        //{
        //    get { return this._learningSet; }
        //    set { this._learningSet = value; }
        //}

        //public DataMining.Data.Dataset ValidationSet
        //{
        //    get { return this._validationSet; }
        //    set { this._validationSet = value; }
        //}

        //public DataMining.ClassificationMeasures.IClassificationMeasure Measure
        //{
        //    get { return this._measure; }
        //}

        #endregion

        #region Constructor

        public WekaClassificationQualityEvaluator(WekaNETBridge.WekaClassification wekaClasification,bool skipAttributesValidation)
        {
            this._skipAttibutesValidation = skipAttributesValidation;
            this.WekaClassification = wekaClasification;
        }
        #endregion

        #region Methods

        public void EvaluateSolutionQuality(Solution<DRComponent> solution)
        {            
            double quality = 0;

            weka.core.Instances validationSet = this.WekaClassification.OriginalDataset;
            weka.classifiers.Classifier classifier=this.CreateClassifier(solution);

            if (!this._skipAttibutesValidation)
            {
                int[] attributesToRmove = solution.AttributesToRemove();
                if (attributesToRmove.Length != 0)
                    validationSet = WekaNETBridge.WekaClassification.GetReducedDataset(this.WekaClassification.OriginalDataset, attributesToRmove, null);
            }

            quality = WekaNETBridge.WekaClassification.EvaluateClassifier(classifier,validationSet);

            solution.Quality = quality;
        }

        public weka.classifiers.Classifier CreateClassifier(Solution<DRComponent> solution)
        {
            weka.core.Instances reducedDataset = WekaNETBridge.WekaClassification.GetReducedDataset(this.WekaClassification.OriginalDataset,solution.AttributesToRemove(), solution.InstancesToRemove());
             weka.classifiers.Classifier classifier = this.WekaClassification.CreateClassifier(reducedDataset);
             return classifier;
        }

        #endregion

        public WekaNETBridge.WekaClassifier CreateWekaClassifier(weka.classifiers.Classifier currentClassifier, Solution<DRComponent> solution)
        {
            weka.core.Instances reducedDataset = WekaNETBridge.WekaClassification.GetReducedDataset(this.WekaClassification.OriginalDataset,solution.AttributesToRemove(), solution.InstancesToRemove());
            weka.classifiers.Classifier classifier = this.WekaClassification.CreateClassifier(reducedDataset,currentClassifier);

            WekaNETBridge.WekaClassifier wekaClassifier = new WekaNETBridge.WekaClassifier();
            wekaClassifier.Classifier = classifier;
            wekaClassifier.AttributesToRemove = solution.AttributesToRemove();
            return wekaClassifier;
        }
    }
}
