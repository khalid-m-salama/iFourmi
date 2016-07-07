using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;
using iFourmi.DataMining.Model.IBLearning;
using iFourmi.DataMining.ClassificationMeasures;


namespace iFourmi.ContinuousACO.ProblemSpecifics
{
    public class IBClassificationQualityEvaluator:AbstractContinousOptimizationEvaluator
    {
        #region Data Members

        //private double[,] _similarityCache;

        private AbstractLazyClassifier _IBClassifier;

        private IClassificationMeasure _measure;

        

        #endregion

        #region Properties

        public AbstractLazyClassifier Classifier
        {
            get { return this._IBClassifier; }
        }

        public Dataset LearningSet
        {
            get;
            set;
        }

        public Dataset ValidationSet
        {
            get;
            set;         
        }

        #endregion

        #region Constructor

        public IBClassificationQualityEvaluator(AbstractLazyClassifier classifer,  IClassificationMeasure measure)
            :base(0,1)
        {
            this._IBClassifier = classifer;
            this._measure = measure;
            //this.LoadSimilarityCache();
        }

        //private void LoadSimilarityCache()
        //{
        //    if (this._similarityCache == null)
        //        this._similarityCache = new double[this._IBClassifier.Database.Size, this._IBClassifier.Database.Size];

        //    //for(int i=0; i< this._IBClassifier.Metadata.Size; i++)
        //    //    for(int j=0; j< this._IBClassifier.Metadata.Size; j++)
        //    //        this._similarityCache[i,j]=this._IBClassifier.distanceMeasure.CalculateSimilarity(this._IBClassifier.Database[i],this._IBClassifier.Database[j]);
        //}

        #endregion

        public override void EvaluateSolutionQuality(Solution<double> solution)
        {
            int classCount = this.LearningSet.Metadata.Target.Values.Length;
            int attributesCount = this.LearningSet.Metadata.Attributes.Length;

            if (this._IBClassifier is GaussianKernelEstimator)
            {
                double[][] classBasedWeights = new double[classCount][];                    
                double kernelParameter = solution.Components[0].Element; 
                int counter=1;

                if (solution.Components.Count > attributesCount + 1)
                {                  
                    for (int i = 0; i < classCount; i++)
                    {
                        classBasedWeights[i] = new double[attributesCount];
                        for (int j = 0; j < attributesCount; j++)
                        {
                            classBasedWeights[i][j] = solution.Components[counter].Element;
                            counter++;
                        }
                    }
                    
                }
                else
                {
                    for (int i = 0; i < classCount; i++)
                    {
                        classBasedWeights[i] = new double[attributesCount];
                        for (int j = 0; j < attributesCount; j++)
                        {
                            classBasedWeights[i][j] = solution.Components[counter].Element;
                            counter++;
                        }
                        counter = 1;
                    }
                }

                GaussianKernelEstimator GKClassfier = this._IBClassifier as GaussianKernelEstimator;
                GKClassfier.KernelParameter = kernelParameter/10;
                //GKClassfier.KernelParameter = 0;
                GKClassfier.SetWeights(classBasedWeights);

            }
            else if (this._IBClassifier is NearestClassClassifier)
            {
                double[][] classBasedWeights = new double[classCount][];
                double similarityTheshold = solution.Components[0].Element;
                int counter = 1;

                if (solution.Components.Count > attributesCount + 1)
                {
                    for (int i = 0; i < classCount; i++)
                    {
                        classBasedWeights[i] = new double[attributesCount];
                        for (int j = 0; j < attributesCount; j++)
                        {
                            classBasedWeights[i][j] = solution.Components[counter].Element;
                            counter++;
                        }
                    }

                }
                else
                {
                    for (int i = 0; i < classCount; i++)
                    {
                        classBasedWeights[i] = new double[attributesCount];
                        for (int j = 0; j < attributesCount; j++)
                        {
                            classBasedWeights[i][j] = solution.Components[counter].Element;
                            counter++;
                        }
                        counter = 1;
                    }
                }

                NearestClassClassifier NNClassfier = this._IBClassifier as NearestClassClassifier;
                NNClassfier.SimilarityThreshold = similarityTheshold;
                NNClassfier.SetWeights(classBasedWeights);
            }
            else if(this._IBClassifier is KNearestNeighbours)
            {

                double[][] classBasedWeights = new double[classCount][];
                int k = ((int)(Math.Round((solution.Components[0].Element)*20,0)))+1;
                //int k = 1;
                int counter = 1;

                if (solution.Components.Count > attributesCount + 1)
                {
                    for (int i = 0; i < classCount; i++)
                    {
                        classBasedWeights[i] = new double[attributesCount];
                        for (int j = 0; j < attributesCount; j++)
                        {
                            classBasedWeights[i][j] = solution.Components[counter].Element;
                            counter++;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < classCount; i++)
                    {
                        classBasedWeights[i] = new double[attributesCount];
                        for (int j = 0; j < attributesCount; j++)
                        {
                            classBasedWeights[i][j] = solution.Components[counter].Element;
                            counter++;
                        }
                        counter = 0;
                    }
                }

                KNearestNeighbours KNNClassifier = this._IBClassifier as KNearestNeighbours;
                KNNClassifier.KNeighbours = k;
                this._IBClassifier.SetWeights(classBasedWeights);

            }

                       
            this._IBClassifier.Database = this.LearningSet;
            double quality = _measure.CalculateMeasure(this._IBClassifier, ValidationSet);

            solution.Quality = quality;
        }
                
    }
}
