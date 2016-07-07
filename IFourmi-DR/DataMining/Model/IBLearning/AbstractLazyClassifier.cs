using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;
using iFourmi.DataMining.Algorithms;
using iFourmi.DataMining.ClassificationMeasures;
using iFourmi.DataMining.ProximityMeasures;

namespace iFourmi.DataMining.Model.IBLearning
{
    public abstract class AbstractLazyClassifier:IClassifier
    {
        #region Data members

        protected string _desc;

        protected Dataset _database;

        protected double[][] _classBasedWeights;

        protected IDistanceMeasure _distanceMeasure;

        #endregion
        
        #region Properties

        public Data.Metadata Metadata
        {
            get { return this._database.Metadata; }
        }

        public string Desc
        {
            get { return this._desc; }

            set { this._desc=value;  }
        }


        public double[][] Weights
        {
            get { return this._classBasedWeights; }
           
        }

        public void SetWeights(double [][] weights)
        {
            this._classBasedWeights = weights;
 
        }

        public void SetWeights(double[] weights)
        {
            for (int i = 0; i < this._classBasedWeights.GetLength(0); i++)
                this._classBasedWeights[i] = weights;

        }

        public IDistanceMeasure DistanceMeasure
        {
            get { return this._distanceMeasure; }
        }

        public Dataset Database
        {
            get { return this._database; }
            set { this._database = value; }
        }
      
        #endregion
        
        #region Constructors

        public AbstractLazyClassifier(IDistanceMeasure distanceMeasuer, Dataset database, double[][] weights)
        {            
            this._distanceMeasure = distanceMeasuer;
            this._database = database;
            this._classBasedWeights = weights;
            
        }

        public AbstractLazyClassifier(IDistanceMeasure distanceMeasure, Dataset database, double[] weights)
        {
            this._distanceMeasure = distanceMeasure;
            this._database = database;
            this._classBasedWeights = new double[this._database.Metadata.Target.Values.Length][];
            for (int i = 0; i < this._classBasedWeights.GetLength(0); i++)
                this._classBasedWeights[i] = weights;


        }

        public AbstractLazyClassifier(IDistanceMeasure distanceMeasure, Dataset database)
        {
            this._distanceMeasure = distanceMeasure;
            this._database = database;
            this._classBasedWeights = new double[this._database.Metadata.Target.Values.Length][];
            for (int i = 0; i < this._classBasedWeights.GetLength(0); i++)
            {
                this._classBasedWeights[i] = new double[this._database.Metadata.Attributes.Length];
                for (int j = 0; j < this._database.Metadata.Attributes.Length; j++)
                    this._classBasedWeights[i][j] = 1;
            }

        }

        

        #endregion

        #region Methods

        public abstract Prediction Classify(Data.Instance instance);
     
        #endregion


    }
}
